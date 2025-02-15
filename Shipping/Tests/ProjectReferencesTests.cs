using Microsoft.Build.Locator;

namespace Shipping.Tests;

public abstract class BaseTest : IDisposable
{
    protected readonly string RootDirectory;
    protected readonly string SolutionPath;
    protected readonly string PipelineYamlPath;

    static BaseTest()
    {
        MSBuildLocator.RegisterDefaults();
    }

    protected BaseTest()
    {
        var baseDirectory = AppContext.BaseDirectory;
        var testsFolder = Directory.GetParent(baseDirectory)?.Parent?.Parent?.FullName!;
        RootDirectory = Directory.GetParent(testsFolder)?.FullName!;
        SolutionPath = Path.Combine(RootDirectory, "Shipping.sln");
        PipelineYamlPath = Path.Combine(RootDirectory, "azure-pipelines.yml");
    }

    public void Dispose()
    {
        // Unload projects if necessary (MSBuild API allows unloading but is generally not needed)
    }
}


public class ProjectReferencesTests : BaseTest
{
    [Fact]
    public void VerifyPipeline_MatchProjectDependencies()
    {
        var yamlPaths = TestHelper.GetYamlPaths(PipelineYamlPath)
            .Select(c => c.Replace('/', '.').TrimStart('.'))
            .ToList();

        var projectDependencies = TestHelper.GetSpecificDependencies(SolutionPath, ".SDK");

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

