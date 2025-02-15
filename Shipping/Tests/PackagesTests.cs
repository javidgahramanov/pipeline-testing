using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;

namespace Shipping.Tests;

public class PackagesTests : BaseTest
{
    [Fact]
    public void Verify_HaveProhibitedPackages_Fail()
    {
        var baseDirectory = AppContext.BaseDirectory;
        var testsFolder = Directory.GetParent(baseDirectory)?.Parent?.Parent?.FullName!;
        var rootDirectory = Directory.GetParent(testsFolder)?.FullName!;
        var sln = Path.Combine(rootDirectory, "Shipping.sln");

        var projectDependencies = TestHelper.GetNuGetPackageReferences(solutionPath: sln, includeVersion: false);

        var vulnerablePackagesFound = WellKnownVulnerablePackages
            .Where(vulnerablePkg => projectDependencies.Any(dep => dep.Contains(vulnerablePkg.Split(' ')[0], StringComparison.OrdinalIgnoreCase)))
            .ToList();

        Assert.False(vulnerablePackagesFound.Any(),
            $"The following prohibited packages were found: {string.Join(", ", vulnerablePackagesFound)}");
    }

    [Fact]
    public void Verify_HaveProhibitedPackageVersion_Fail()
    {

        var baseDirectory = AppContext.BaseDirectory;
        var testsFolder = Directory.GetParent(baseDirectory)?.Parent?.Parent?.FullName!;
        var rootDirectory = Directory.GetParent(testsFolder)?.FullName!;
        var sln = Path.Combine(rootDirectory, "Shipping.sln");

        var projectDependencies = TestHelper.GetNuGetPackageReferences(sln, includeVersion: true);

        var vulnerablePackagesFound = WellKnownVulnerablePackages
            .Where(vulnerablePkg => projectDependencies
                .Any(dep => dep.StartsWith(vulnerablePkg.Split(' ')[0], StringComparison.OrdinalIgnoreCase) &&
                            CompareVersion(dep, vulnerablePkg)))
            .ToList();

        Assert.False(vulnerablePackagesFound.Any(),
            $"The following prohibited packages with outdated versions were found: {string.Join(", ", vulnerablePackagesFound)}");
    }

    private static bool CompareVersion(string installedPackage, string vulnerablePackage)
    {
        var installedVersion = ExtractVersion(installedPackage);
        var maxAllowedVersion = ExtractVersion(vulnerablePackage);

        return string.IsNullOrEmpty(maxAllowedVersion) ||
               (Version.TryParse(installedVersion, out var installedVer) &&
                Version.TryParse(maxAllowedVersion, out var maxVer) &&
                installedVer <= maxVer);
    }

    private static string ExtractVersion(string package)
    {
        var parts = package.Split(' ');
        return parts.Length > 1 ? parts.Last().Trim('(', ')', '<', '=', '>') : "";
    }

    // You can remove Newtonsoft and see test passes. Installing one of the packages will make test fail.
    private static readonly List<string> WellKnownVulnerablePackages =
    [
        "Newtonsoft.Json (<=13.0.1)",
        "log4net (<= 2.0.10)",
        "Microsoft.IdentityModel.Tokens (<= 6.11.0)",
        "Serilog.Sinks.File (<= 5.0.0)"
    ];
}