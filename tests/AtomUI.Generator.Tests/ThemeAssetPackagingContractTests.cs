using System.Xml.Linq;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests;

public class ThemeAssetPackagingContractTests
{
    [Fact]
    public void Generator_Package_Provides_Theme_Asset_Build_Integration()
    {
        var project = XDocument.Load(GetRepoFile("src/AtomUI.Generator/AtomUI.Generator.csproj"));
        project.Descendants("None")
               .ShouldContain(element =>
                   (string?)element.Attribute("Include") == "@(AtomUINuGetBuildAsset)" &&
                   (string?)element.Attribute("Pack") == "true");

        var manifest = XDocument.Load(GetRepoFile("build/repository/GeneratorBuildAssets.props"));
        var buildAssets = manifest.Descendants("AtomUINuGetBuildAsset").ShouldHaveSingleItem();
        ((string?)buildAssets.Attribute("Include"))
            .ShouldNotBeNull()
            .ShouldContain("../nuget/**/*.targets");
        buildAssets.Elements("PackagePath")
                   .ShouldHaveSingleItem()
                   .Value.ShouldBe("buildTransitive/%(RecursiveDir)%(Filename)%(Extension)");

        var generatorTargets = XDocument.Load(GetRepoFile("build/nuget/AtomUI.Generator.targets"));
        generatorTargets.Descendants("Import")
                        .Single(element =>
                            ((string?)element.Attribute("Project"))?.EndsWith(
                                "theme/ThemeAssets.targets",
                                StringComparison.Ordinal) == true)
                        .ShouldNotBeNull();
        File.Exists(GetRepoFile("build/nuget/theme/ThemeAssets.targets")).ShouldBeTrue();
    }

    [Fact]
    public void Theme_Asset_Target_Defaults_On_For_Package_Consumers_Only()
    {
        var target = XDocument.Load(GetRepoFile("build/nuget/theme/ThemeAssets.targets"));
        var property = target.Descendants()
                             .SingleOrDefault(element =>
                                 element.Name.LocalName == "AtomUIGenerateControlThemeAssetResources");
        property.ShouldNotBeNull();
        property.Value.ShouldBe("true");
        ((string?)property.Attribute("Condition"))
            .ShouldBe("'$(AtomUIGenerateControlThemeAssetResources)' == ''");

        var repositoryTargets = XDocument.Load(GetRepoFile(
            "build/repository/AtomUI.Repository.targets"));
        var import = repositoryTargets.Descendants("Import")
                                      .Single(element =>
                                          (string?)element.Attribute("Project") ==
                                          "$(MSBuildThisFileDirectory)../nuget/theme/ThemeAssets.targets");
        ((string?)import.Attribute("Condition"))
            .ShouldBe("'$(AtomUIGenerateControlThemeAssetResources)' == 'true'");

        var controlCatalog = target.Descendants()
                                   .SingleOrDefault(element =>
                                       element.Name.LocalName == "AtomUIThemeControlCatalog");
        controlCatalog.ShouldNotBeNull();
        controlCatalog.Value.ShouldBe("$(AssemblyName)");
        ((string?)controlCatalog.Attribute("Condition"))
            .ShouldBe("'$(AtomUIThemeControlCatalog)' == ''");

        target.Descendants()
              .Single(element =>
                  element.Name.LocalName == "CompilerVisibleProperty" &&
                  (string?)element.Attribute("Include") == "AtomUIThemeControlCatalog")
              .ShouldNotBeNull();

        var projectDefaults = XDocument.Load(GetRepoFile(
            "build/repository/ProjectDefaults.props"));
        projectDefaults.Descendants()
                       .Single(element => element.Name.LocalName == "AtomUIThemeControlCatalog")
                       .Value.ShouldBe("AtomUI");
    }

    [Fact]
    public void Theme_Asset_Target_Uses_Build_Task_Instead_Of_Inline_Code()
    {
        var target = XDocument.Load(GetRepoFile("build/nuget/theme/ThemeAssets.targets"));
        target.Descendants()
              .Where(element => element.Name.LocalName == "UsingTask")
              .ShouldContain(element =>
                  (string?)element.Attribute("TaskName") ==
                  "AtomUI.Build.Tasks.GenerateThemeAssetWrappersTask");
        target.Descendants()
              .Where(element => element.Name.LocalName == "UsingTask")
              .ShouldAllBe(element =>
                  (string?)element.Attribute("TaskFactory") != "RoslynCodeTaskFactory");
        target.Descendants("UsingTask")
              .ShouldAllBe(element =>
                  (string?)element.Attribute("AssemblyFile") == "$(AtomUIBuildTasksAssembly)");
        target.Descendants()
              .Where(element => element.Name.LocalName == "Import")
              .ShouldContain(element =>
                  (string?)element.Attribute("Project") ==
                  "$(MSBuildThisFileDirectory)../infrastructure/BuildTasks.props" &&
                  (string?)element.Attribute("Condition") ==
                  "'$(AtomUIBuildTasksAssembly)' == ''");
        target.Descendants()
              .ShouldNotContain(element => element.Name.LocalName == "Code");
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
