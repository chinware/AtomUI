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
        var packedTarget = project.Descendants("None")
                                  .SingleOrDefault(element =>
                                      string.Equals(
                                          (string?)element.Attribute("PackagePath"),
                                          "buildTransitive/AtomUI.Generator.targets",
                                          StringComparison.Ordinal));

        packedTarget.ShouldNotBeNull();
        ((string?)packedTarget.Attribute("Pack")).ShouldBe("true");
        ((string?)packedTarget.Attribute("Include")).ShouldBe("../../build/AtomUI.Generator.targets");

        var generatorTargets = XDocument.Load(GetRepoFile("build/AtomUI.Generator.targets"));
        generatorTargets.Descendants("Import")
                        .Single(element =>
                            ((string?)element.Attribute("Project"))?.EndsWith(
                                "AtomUI.ThemeAssets.targets",
                                StringComparison.Ordinal) == true)
                        .ShouldNotBeNull();

        project.Descendants("None")
               .Single(element =>
                   (string?)element.Attribute("Include") == "../../build/AtomUI.ThemeAssets.targets" &&
                   (string?)element.Attribute("PackagePath") ==
                   "buildTransitive/AtomUI.ThemeAssets.targets")
               .ShouldNotBeNull();
    }

    [Fact]
    public void Theme_Asset_Target_Defaults_On_For_Package_Consumers_Only()
    {
        var target = XDocument.Load(GetRepoFile("build/AtomUI.ThemeAssets.targets"));
        var property = target.Descendants()
                             .SingleOrDefault(element =>
                                 element.Name.LocalName == "AtomUIGenerateControlThemeAssetResources");
        property.ShouldNotBeNull();
        property.Value.ShouldBe("true");
        ((string?)property.Attribute("Condition"))
            .ShouldBe("'$(AtomUIGenerateControlThemeAssetResources)' == ''");

        var directoryTargets = XDocument.Load(GetRepoFile("Directory.Build.targets"));
        var import = directoryTargets.Descendants("Import")
                                     .Single(element =>
                                         ((string?)element.Attribute("Project"))?
                                         .EndsWith("build/AtomUI.ThemeAssets.targets", StringComparison.Ordinal) == true);
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

        var directoryProps = XDocument.Load(GetRepoFile("Directory.Build.props"));
        directoryProps.Descendants()
                      .Single(element => element.Name.LocalName == "AtomUIThemeControlCatalog")
                      .Value.ShouldBe("AtomUI");
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
