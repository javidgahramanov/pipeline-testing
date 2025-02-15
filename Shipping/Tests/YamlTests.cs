using Microsoft.Build.Locator;

namespace Shipping.Tests;

public class YamlTests
{
    [Fact]
    public void VerifyPipelineTriggers_MatchProjectDependencies()
    {
        MSBuildLocator.RegisterDefaults();

        var baseDirectory = AppContext.BaseDirectory;
        var testsFolder = Directory.GetParent(baseDirectory)?.Parent?.Parent?.FullName!;
        var rootDirectory = Directory.GetParent(testsFolder)?.FullName!;
        var yamlFilePath = Path.Combine(rootDirectory, "azure-pipelines.yml");
        var sln = Path.Combine(rootDirectory, "Shipping.sln");

        var yamlPaths = PipelineTestHelper.GetYamlPaths(yamlFilePath)
            .Select(c => c.Replace('/', '.').TrimStart('.'))
            .ToList();

        var projectDependencies = PipelineTestHelper.GetProjectDependencies(sln);

        var missingDependencies = projectDependencies
            .Where(dep => !yamlPaths.Contains(dep, StringComparer.OrdinalIgnoreCase))
            .ToList();

        var extraYamlPaths = yamlPaths
            .Where(path => !projectDependencies.Contains(path, StringComparer.OrdinalIgnoreCase))
            .ToList();

        Assert.False(missingDependencies.Any(),
            $"Missing dependencies in YAML file:\n{string.Join("\n", missingDependencies)}");

        Assert.False(extraYamlPaths.Any(),
            $"Extra paths found in YAML file that do not exist in project dependencies:\n{string.Join("\n", extraYamlPaths)}");
    }

}

