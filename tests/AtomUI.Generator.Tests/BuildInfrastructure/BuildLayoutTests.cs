using System.Xml.Linq;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.BuildInfrastructure;

public sealed class BuildLayoutTests
{
    [Fact]
    public void Directory_Build_Files_Import_Only_Repository_Entry_Points()
    {
        GetImports("Directory.Build.props")
            .ShouldBe(["$(MSBuildThisFileDirectory)build/repository/AtomUI.Repository.props"]);
        GetImports("Directory.Build.targets")
            .ShouldBe(["$(MSBuildThisFileDirectory)build/repository/AtomUI.Repository.targets"]);
    }

    [Fact]
    public void Repository_Props_Import_Configuration_In_Deterministic_Order()
    {
        GetImports("build/repository/AtomUI.Repository.props").ShouldBe([
            "$(MSBuildThisFileDirectory)Versions.props",
            "$(MSBuildThisFileDirectory)ProjectDefaults.props",
            "$(MSBuildThisFileDirectory)PackageMetadata.props",
            "$(MSBuildThisFileDirectory)OutputPaths.props",
            "$(MSBuildThisFileDirectory)../AtomUI.LinkedRegistration.props",
            "$(MSBuildThisFileDirectory)../AtomUI.Localization.props"
        ]);
    }

    [Fact]
    public void Repository_Targets_Import_Defaults_And_Features_In_Deterministic_Order()
    {
        GetImports("build/repository/AtomUI.Repository.targets").ShouldBe([
            "$(MSBuildThisFileDirectory)ProjectDefaults.targets",
            "$(MSBuildThisFileDirectory)../AtomUI.LinkedRegistration.targets",
            "$(MSBuildThisFileDirectory)PackageGeneratorAssets.targets",
            "$(MSBuildThisFileDirectory)../AtomUI.Localization.targets",
            "$(MSBuildThisFileDirectory)../AtomUI.ThemeAssets.targets"
        ]);
    }

    private static string[] GetImports(string relativePath)
    {
        return XDocument.Load(GetRepoFile(relativePath))
                        .Descendants()
                        .Where(element => element.Name.LocalName == "Import")
                        .Select(element => (string?)element.Attribute("Project"))
                        .Where(static path => path is not null)
                        .Cast<string>()
                        .ToArray();
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException(relativePath);
    }
}
