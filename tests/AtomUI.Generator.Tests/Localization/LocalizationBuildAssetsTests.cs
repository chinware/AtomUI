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
        var contractVersion = propertyGroups.SelectMany(static group => group.Elements())
                                            .Single(element =>
                                                element.Name.LocalName == "AtomUILanguageContractVersion");
        contractVersion.Value.ShouldBe("1");
        ((string?)contractVersion.Attribute("Condition"))
            .ShouldBe("'$(AtomUILanguageContractVersion)' == ''");

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
        discoveredLanguage.Elements()
                          .Single(element =>
                              element.Name.LocalName == "AtomUILanguageContractVersion")
                          .Value.ShouldBe("$(AtomUILanguageContractVersion)");

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
        AssertMetadataForwarded(additionalFiles, "AtomUILanguageSourceFingerprint");

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
            "AtomUILanguageContractVersion",
            "AtomUILanguageSourceFingerprint"
        ],
            ignoreOrder: true);

        var visibleProperties = targets.Descendants()
                                       .Where(element => element.Name.LocalName == "CompilerVisibleProperty")
                                       .Select(element => (string?)element.Attribute("Include"))
                                       .ToArray();
        visibleProperties.ShouldBe(
            ["PackageId", "AssemblyName", "RootNamespace", "AtomUIBuildLanguagePackage"],
            ignoreOrder: true);

        var usingTasks = targets.Descendants("UsingTask")
                                .Select(element => (string?)element.Attribute("TaskName"))
                                .ToArray();
        usingTasks.ShouldBe(
        [
            "AtomUI.Build.Tasks.CollectLanguageCatalogsTask",
            "AtomUI.Build.Tasks.ValidateLanguageFilesTask",
            "AtomUI.Build.Tasks.ExportLanguageTemplatesTask",
            "AtomUI.Build.Tasks.PrepareLanguagePackageTask",
            "AtomUI.Build.Tasks.GenerateLanguagePackagePropsTask"
        ],
            ignoreOrder: true);
        targets.Descendants("Target")
               .Single(element => (string?)element.Attribute("Name") == "AtomUIValidateLanguageFiles")
               .Attribute("BeforeTargets")!.Value.ShouldBe("CoreCompile");
        targets.Descendants("Target")
               .Single(element => (string?)element.Attribute("Name") == "AtomUIExportLanguageTemplates")
               .ShouldNotBeNull();
    }

    [Fact]
    public void Localization_Targets_Registers_Build_Tasks_Before_Source_Outputs_Exist()
    {
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.Localization.targets"));
        var usingTasks = targets.Descendants("UsingTask")
                                .Where(element =>
                                    ((string?)element.Attribute("TaskName"))?.StartsWith(
                                        "AtomUI.Build.Tasks.",
                                        StringComparison.Ordinal) == true)
                                .ToArray();

        usingTasks.ShouldNotBeEmpty();
        foreach (var usingTask in usingTasks)
        {
            ((string?)usingTask.Attribute("AssemblyFile"))
                .ShouldBe("$(AtomUILocalizationBuildTasksAssembly)");
            usingTask.Attribute("Condition").ShouldBeNull();
        }
    }

    [Fact]
    public void Module_Package_Exports_Only_The_Authoritative_EnUs_Catalog_Sources()
    {
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.Localization.targets"));
        var moduleLanguageFiles = targets.Descendants()
                                         .Single(element =>
                                             element.Name.LocalName == "_AtomUIModuleLanguageFile");

        ((string?)moduleLanguageFiles.Attribute("Include")).ShouldBe("@(AtomUILanguage)");
        var condition = (string?)moduleLanguageFiles.Attribute("Condition");
        condition.ShouldNotBeNull();
        condition.ShouldContain("'%(AtomUILanguageSourceKind)' == 'ModuleBuiltIn'");
        condition.ShouldContain("'%(Filename)%(Extension)' == 'en-US.xlf'");
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
        packedFiles["$(OutputPath)/AtomUI.Build.Tasks.dll"]
            .ShouldBe("tools/netstandard2.0/AtomUI.Build.Tasks.dll");

        project.Descendants("ProjectReference")
               .Single(element =>
                   ((string?)element.Attribute("Include"))?.EndsWith(
                       "AtomUI.Build.Tasks/AtomUI.Build.Tasks.csproj",
                       StringComparison.Ordinal) == true &&
                   (string?)element.Attribute("ReferenceOutputAssembly") == "false")
               .ShouldNotBeNull();

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

    [Fact]
    public void Generator_Project_Isolates_Runtime_Publish_Properties()
    {
        var project = XDocument.Load(GetRepoFile("src/AtomUI.Generator/AtomUI.Generator.csproj"));
        var isolatedProperties = ((string?)project.Root!.Attribute("TreatAsLocalProperty"))!
                                 .Split(';', StringSplitOptions.RemoveEmptyEntries);

        isolatedProperties.ShouldContain("PublishAot");
        isolatedProperties.ShouldContain("PublishTrimmed");
        isolatedProperties.ShouldContain("PublishSingleFile");
        isolatedProperties.ShouldContain("SelfContained");
        isolatedProperties.ShouldContain("RuntimeIdentifier");

        var properties = project.Descendants()
                                .Where(element => element.Parent?.Name.LocalName == "PropertyGroup")
                                .ToDictionary(
                                    element => element.Name.LocalName,
                                    element => element.Value,
                                    StringComparer.Ordinal);
        properties["PublishAot"].ShouldBe("false");
        properties["PublishTrimmed"].ShouldBe("false");
        properties["PublishSingleFile"].ShouldBe("false");
        properties["SelfContained"].ShouldBe("false");
        properties["RuntimeIdentifier"].ShouldBeEmpty();
    }

    private static void AssertMetadataForwarded(XElement additionalFiles, string name)
    {
        ((string?)additionalFiles.Attribute(name)).ShouldBe($"%({name})");
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
