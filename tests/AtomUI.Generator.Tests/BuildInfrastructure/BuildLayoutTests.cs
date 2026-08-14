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
            "$(MSBuildThisFileDirectory)../nuget/linked-registration/LinkedRegistration.props",
            "$(MSBuildThisFileDirectory)../nuget/localization/Localization.props"
        ]);
    }

    [Fact]
    public void Repository_Targets_Import_Defaults_And_Features_In_Deterministic_Order()
    {
        GetImports("build/repository/AtomUI.Repository.targets").ShouldBe([
            "$(MSBuildThisFileDirectory)ProjectDefaults.targets",
            "$(MSBuildThisFileDirectory)../nuget/linked-registration/LinkedRegistration.targets",
            "$(MSBuildThisFileDirectory)PackageGeneratorAssets.targets",
            "$(MSBuildThisFileDirectory)../nuget/localization/Localization.targets",
            "$(MSBuildThisFileDirectory)../nuget/theme/ThemeAssets.targets"
        ]);
    }

    [Fact]
    public void NuGet_Generator_Entry_Points_Import_Features_In_Deterministic_Order()
    {
        GetImports("build/nuget/AtomUI.Generator.props").ShouldBe([
            "$(MSBuildThisFileDirectory)linked-registration/LinkedRegistration.props",
            "$(MSBuildThisFileDirectory)localization/Localization.props"
        ]);
        GetImports("build/nuget/AtomUI.Generator.targets").ShouldBe([
            "$(MSBuildThisFileDirectory)linked-registration/LinkedRegistration.targets",
            "$(MSBuildThisFileDirectory)localization/Localization.targets",
            "$(MSBuildThisFileDirectory)theme/ThemeAssets.targets"
        ]);
    }

    [Fact]
    public void Legacy_NuGet_Assets_Do_Not_Remain_At_The_Build_Root()
    {
        var repositoryRoot = GetRepositoryRoot();
        foreach (var relativePath in new[]
                 {
                     "build/AtomUI.Generator.props",
                     "build/AtomUI.Generator.targets",
                     "build/AtomUI.GeneratorConsumer.targets",
                     "build/AtomUI.LinkedRegistration.props",
                     "build/AtomUI.LinkedRegistration.targets",
                     "build/AtomUI.Localization.props",
                     "build/AtomUI.Localization.targets",
                     "build/AtomUI.ThemeAssets.targets"
                 })
        {
            File.Exists(Path.Combine(repositoryRoot, relativePath)).ShouldBeFalse(relativePath);
        }
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
        var repositoryRoot = GetRepositoryRoot();
        var candidate = Path.Combine(repositoryRoot, relativePath);
        if (File.Exists(candidate))
        {
            return candidate;
        }

        throw new FileNotFoundException(relativePath);
    }

    private static string GetRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "AtomUI.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the AtomUI repository root.");
    }
}
