using System.Xml.Linq;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.Localization;

public class LocalizationBuildAssetsTests
{
    [Fact]
    public void Localization_Props_Defines_Module_Identity_And_Item_Defaults()
    {
        var props = XDocument.Load(GetRepoFile("build/AtomUI.Localization.props"));
        var propertyGroups = props.Root!.Elements()
                                  .Where(element => element.Name.LocalName == "PropertyGroup")
                                  .ToArray();

        propertyGroups.SelectMany(static group => group.Elements())
                      .Single(element =>
                          element.Name.LocalName == "AtomUILanguageModuleId" &&
                          element.Value == "$(PackageId)")
                      .ShouldNotBeNull();
        propertyGroups.SelectMany(static group => group.Elements())
             .Where(element => element.Name.LocalName == "AtomUILanguageModuleId")
             .Any(element =>
                 element.Value == "$(AssemblyName)" &&
                 (string?)element.Attribute("Condition") == "'$(AtomUILanguageModuleId)' == ''")
             .ShouldBeTrue();

        var languageDefaults = props.Descendants()
                                    .Single(element => element.Name.LocalName == "AtomUILanguage");
        languageDefaults.Elements()
                        .Single(element => element.Name.LocalName == "AtomUILanguageSourceKind")
                        .Value.ShouldBe("ModuleBuiltIn");
        languageDefaults.Elements()
                        .Single(element => element.Name.LocalName == "AtomUILanguageSourceIdentity")
                        .Value.ShouldBe("$(AtomUILanguageModuleId)");
        languageDefaults.Elements()
                        .Single(element => element.Name.LocalName == "AtomUILanguageModuleId")
                        .Value.ShouldBe("$(AtomUILanguageModuleId)");

        var overrideDefaults = props.Descendants()
                                    .Single(element => element.Name.LocalName == "AtomUILanguageOverride");
        overrideDefaults.Elements()
                        .Single(element => element.Name.LocalName == "AtomUILanguageSourceKind")
                        .Value.ShouldBe("ApplicationOverride");
    }

    [Fact]
    public void Repository_Imports_Localization_Build_Assets_For_Project_References()
    {
        var props = XDocument.Load(GetRepoFile("Directory.Build.props"));
        var targets = XDocument.Load(GetRepoFile("Directory.Build.targets"));

        props.Descendants("Import")
             .Single(element =>
                 ((string?)element.Attribute("Project"))?.EndsWith(
                     "build/AtomUI.Localization.props",
                     StringComparison.Ordinal) == true)
             .ShouldNotBeNull();
        targets.Descendants("Import")
               .Single(element =>
                   ((string?)element.Attribute("Project"))?.EndsWith(
                       "build/AtomUI.Localization.targets",
                       StringComparison.Ordinal) == true)
               .ShouldNotBeNull();
    }

    [Fact]
    public void Localization_Targets_Discovers_Xliff_And_Exposes_Generator_Metadata()
    {
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.Localization.targets"));
        var discoveredLanguage = targets.Descendants()
                                        .Single(element =>
                                            element.Name.LocalName == "AtomUILanguage" &&
                                            element.Attribute("Include") is not null);
        var include = (string?)discoveredLanguage.Attribute("Include");
        var exclude = (string?)discoveredLanguage.Attribute("Exclude");
        var fileExcludes = targets.Descendants()
                                  .Single(element =>
                                      element.Name.LocalName == "_AtomUILanguageFileExcludes")
                                  .Value;

        include.ShouldBe("$(MSBuildProjectDirectory)/**/Localization/**/*.xlf");
        exclude.ShouldNotBeNull();
        exclude.ShouldContain("$(_AtomUILanguageFileExcludes)");
        exclude.ShouldContain("@(AtomUILanguageOverride)");
        fileExcludes.ShouldContain("$(BaseOutputPath)");
        fileExcludes.ShouldContain("$(BaseIntermediateOutputPath)");
        fileExcludes.ShouldContain("GeneratedFiles");

        var additionalFiles = targets.Descendants()
                                     .Single(element =>
                                         element.Name.LocalName == "AdditionalFiles" &&
                                         ((string?)element.Attribute("Include"))?.Contains(
                                             "@(AtomUILanguage)",
                                             StringComparison.Ordinal) == true);
        ((string?)additionalFiles.Attribute("AtomUILanguage")).ShouldBe("true");
        AssertMetadataForwarded(additionalFiles, "AtomUILanguageSourceKind");
        AssertMetadataForwarded(additionalFiles, "AtomUILanguageSourceIdentity");
        AssertMetadataForwarded(additionalFiles, "AtomUILanguageModuleId");
        AssertMetadataForwarded(additionalFiles, "AtomUILanguageContractVersion");

        var visibleMetadata = targets.Descendants()
                                     .Where(element =>
                                         element.Name.LocalName == "CompilerVisibleItemMetadata" &&
                                         (string?)element.Attribute("Include") == "AdditionalFiles")
                                     .Select(element => (string?)element.Attribute("MetadataName"))
                                     .ToArray();
        visibleMetadata.ShouldBe(
        [
            "AtomUILanguage",
            "AtomUILanguageSourceKind",
            "AtomUILanguageSourceIdentity",
            "AtomUILanguageModuleId",
            "AtomUILanguageContractVersion"
        ],
            ignoreOrder: true);

        var visibleProperties = targets.Descendants()
                                       .Where(element => element.Name.LocalName == "CompilerVisibleProperty")
                                       .Select(element => (string?)element.Attribute("Include"))
                                       .ToArray();
        visibleProperties.ShouldBe(["PackageId", "AssemblyName", "RootNamespace"], ignoreOrder: true);
    }

    [Fact]
    public void Generator_Package_Contains_Localization_BuildTransitive_Assets()
    {
        var project = XDocument.Load(GetRepoFile("src/AtomUI.Generator/AtomUI.Generator.csproj"));
        var packedFiles = project.Descendants("None")
                                 .Where(element =>
                                     string.Equals(
                                         (string?)element.Attribute("Pack"),
                                         "true",
                                         StringComparison.OrdinalIgnoreCase))
                                 .ToDictionary(
                                     element => element.Attribute("Include")!.Value,
                                     element => (string?)element.Attribute("PackagePath"),
                                     StringComparer.Ordinal);

        packedFiles["../../build/AtomUI.Localization.props"]
            .ShouldBe("buildTransitive/AtomUI.Localization.props");
        packedFiles["../../build/AtomUI.Localization.targets"]
            .ShouldBe("buildTransitive/AtomUI.Localization.targets");
        packedFiles["../../build/AtomUI.Generator.props"]
            .ShouldBe("buildTransitive/AtomUI.Generator.props");
        packedFiles["../../build/AtomUI.Generator.targets"]
            .ShouldBe("buildTransitive/AtomUI.Generator.targets");

        var generatorProps = XDocument.Load(GetRepoFile("build/AtomUI.Generator.props"));
        generatorProps.Descendants("Import")
                      .Single(element =>
                          ((string?)element.Attribute("Project"))?.EndsWith(
                              "AtomUI.Localization.props",
                              StringComparison.Ordinal) == true)
                      .ShouldNotBeNull();

        var generatorTargets = XDocument.Load(GetRepoFile("build/AtomUI.Generator.targets"));
        generatorTargets.Descendants("Import")
                        .Single(element =>
                            ((string?)element.Attribute("Project"))?.EndsWith(
                                "AtomUI.Localization.targets",
                                StringComparison.Ordinal) == true)
                        .ShouldNotBeNull();
    }

    private static void AssertMetadataForwarded(XElement additionalFiles, string name)
    {
        ((string?)additionalFiles.Attribute(name)).ShouldBe($"%(AtomUILanguage.{name})");
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
