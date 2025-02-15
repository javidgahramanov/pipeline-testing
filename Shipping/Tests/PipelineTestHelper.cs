using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Shipping.Tests.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Shipping.Tests;

public static class PipelineTestHelper
{
    public static IEnumerable<string> GetYamlPaths(string yamlFilePath)
    {
        var yamlContent = File.ReadAllText(yamlFilePath);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        var config = deserializer.Deserialize<Yaml>(yamlContent);

        return (config.Trigger.Paths.Include ?? [])
            .Where(path => path.Contains("sdk", StringComparison.OrdinalIgnoreCase));
    }

    public static IList<string> GetProjectDependencies(string path)
    {
        var solution = SolutionFile.Parse(path);
        var list = new List<string>();

        foreach (var projectInSolution in solution.ProjectsInOrder)
        {
            if (projectInSolution.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat)
            {
                var projectPath = projectInSolution.AbsolutePath;
                var project = new Project(projectPath);

                var dependencies = project.Items
                    .Where(item => item.ItemType == "ProjectReference" &&
                                   Path.GetFileNameWithoutExtension(item.EvaluatedInclude)
                                       .EndsWith(".SDK", StringComparison.OrdinalIgnoreCase))
                    .Select(item => Path.GetFileNameWithoutExtension(item.EvaluatedInclude))
                    .ToList();
                list.AddRange(dependencies);

                ProjectCollection.GlobalProjectCollection.UnloadProject(project);
            }
        }

        return list.Distinct().ToList();
    }
}