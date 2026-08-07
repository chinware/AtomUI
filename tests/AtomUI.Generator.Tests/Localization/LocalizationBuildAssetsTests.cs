using System.Diagnostics;
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
        var minimumState = propertyGroups.SelectMany(static group => group.Elements())
                                         .Single(element =>
                                             element.Name.LocalName == "AtomUILanguageMinimumState");
        minimumState.Value.ShouldBe("translated");
        ((string?)minimumState.Attribute("Condition"))
            .ShouldBe("'$(AtomUILanguageMinimumState)' == ''");

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
        var templateOutputRoot = targets.Descendants()
                                        .Single(element =>
                                            element.Name.LocalName ==
                                            "AtomUILanguageTemplateOutputRootDirectory");
        templateOutputRoot.Value.ShouldBe("$(MSBuildProjectDirectory)/Localization");
        ((string?)templateOutputRoot.Attribute("Condition"))
            .ShouldBe("'$(AtomUILanguageTemplateOutputRootDirectory)' == ''");
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
        var validationTarget = targets.Descendants("Target")
                                      .Single(element =>
                                          (string?)element.Attribute("Name") == "AtomUIValidateLanguageFiles");
        var validationTask = validationTarget.Descendants()
                                              .Single(element =>
                                                  element.Name.LocalName == "AtomUI.Build.Tasks.ValidateLanguageFilesTask");
        ((string?)validationTask.Attribute("MinimumTargetState"))
            .ShouldBe("$(AtomUILanguageMinimumState)");
        var prepareTask = targets.Descendants("Target")
                                 .Single(element =>
                                     (string?)element.Attribute("Name") == "AtomUIPrepareLanguagePackage")
                                 .Descendants()
                                 .Single(element =>
                                     element.Name.LocalName == "AtomUI.Build.Tasks.PrepareLanguagePackageTask");
        ((string?)prepareTask.Attribute("MinimumTargetState"))
            .ShouldBe("$(AtomUILanguageMinimumState)");
        var exportTarget = targets.Descendants("Target")
                                  .Single(element =>
                                      (string?)element.Attribute("Name") ==
                                      "AtomUIExportLanguageTemplates");
        ((string?)exportTarget.Descendants()
                              .Single(element =>
                                  element.Name.LocalName ==
                                  "AtomUI.Build.Tasks.ExportLanguageTemplatesTask")
                              .Attribute("OutputRootDirectory"))
            .ShouldBe("$(AtomUILanguageTemplateOutputRootDirectory)");
    }

    [Fact]
    public void Localization_Targets_Define_The_Language_Pack_Project_Reference_Protocol()
    {
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.Localization.targets"));

        var provider = targets.Descendants("Target")
                              .Single(element =>
                                  (string?)element.Attribute("Name") ==
                                  "AtomUIGetLanguagePackProjectAssets");
        ((string?)provider.Attribute("Condition"))
            .ShouldBe("'$(AtomUIBuildLanguagePackage)' == 'true'");
        ((string?)provider.Attribute("Returns"))
            .ShouldBe("@(_AtomUILanguagePackProjectAsset)");

        var prepare = provider.Descendants()
                              .Single(element =>
                                  element.Name.LocalName ==
                                  "AtomUI.Build.Tasks.PrepareLanguagePackageTask");
        ((string?)prepare.Attribute("PackageId")).ShouldBe("$(PackageId)");
        ((string?)prepare.Attribute("ExpectedLanguage")).ShouldBe("$(AtomUILanguageTag)");
        ((string?)prepare.Attribute("MinimumTargetState"))
            .ShouldBe("$(AtomUILanguageMinimumState)");
        ((string?)prepare.Attribute("LanguageFiles"))
            .ShouldBe("@(_AtomUILanguagePackProjectTargetFile)");
        prepare.Elements("Output")
               .Single(element =>
                   (string?)element.Attribute("TaskParameter") == "PreparedLanguageFiles")
               .Attribute("ItemName")!.Value.ShouldBe("_AtomUIPreparedLanguagePackProjectTargetFile");

        var returnedAssets = provider.Descendants("_AtomUILanguagePackProjectAsset").ToArray();
        returnedAssets.Length.ShouldBe(2);
        returnedAssets.Any(element =>
                ((string?)element.Attribute("Include"))?.Contains(
                    "_AtomUILanguagePackProjectSourceFile",
                    StringComparison.Ordinal) == true)
            .ShouldBeTrue();
        var returnedTarget = returnedAssets.Single(element =>
            ((string?)element.Attribute("Include"))?.Contains(
                "_AtomUIPreparedLanguagePackProjectTargetFile",
                StringComparison.Ordinal) == true);
        AssertProjectAssetMetadata(returnedTarget, requireFingerprint: true);

        var resolver = targets.Descendants("Target")
                              .Single(element =>
                                  (string?)element.Attribute("Name") ==
                                  "AtomUIResolveLanguagePackProjectReferences");
        var beforeTargets = ((string?)resolver.Attribute("BeforeTargets"))!
            .Split(';', StringSplitOptions.RemoveEmptyEntries);
        beforeTargets.ShouldContain("GenerateMSBuildEditorConfigFileShouldRun");
        beforeTargets.ShouldContain("AtomUIValidateLanguageFiles");
        beforeTargets.ShouldContain("CoreCompile");

        var msbuild = resolver.Descendants("MSBuild").ShouldHaveSingleItem();
        ((string?)msbuild.Attribute("Projects"))
            .ShouldBe("@(AtomUILanguagePackProjectReference)");
        ((string?)msbuild.Attribute("Targets")).ShouldBe("AtomUIGetLanguagePackProjectAssets");
        msbuild.Elements("Output")
               .Single(element =>
                   (string?)element.Attribute("TaskParameter") == "TargetOutputs")
               .Attribute("ItemName")!.Value.ShouldBe("_AtomUIResolvedLanguagePackProjectAsset");

        var additionalFiles = resolver.Descendants("AdditionalFiles").ShouldHaveSingleItem();
        ((string?)additionalFiles.Attribute("Include"))
            .ShouldBe("@(_AtomUIResolvedLanguagePackProjectAsset)");
        ((string?)additionalFiles.Attribute("AtomUILanguage")).ShouldBe("true");
        AssertMetadataForwarded(
            additionalFiles,
            "AtomUILanguageSourceKind",
            "%(_AtomUIResolvedLanguagePackProjectAsset.AtomUILanguageSourceKind)");
        AssertMetadataForwarded(
            additionalFiles,
            "AtomUILanguageSourceIdentity",
            "%(_AtomUIResolvedLanguagePackProjectAsset.AtomUILanguageSourceIdentity)");
        AssertMetadataForwarded(
            additionalFiles,
            "AtomUILanguageModuleId",
            "%(_AtomUIResolvedLanguagePackProjectAsset.AtomUILanguageModuleId)");
        AssertMetadataForwarded(
            additionalFiles,
            "AtomUILanguageContractVersion",
            "%(_AtomUIResolvedLanguagePackProjectAsset.AtomUILanguageContractVersion)");
        AssertMetadataForwarded(
            additionalFiles,
            "AtomUILanguageSourceFingerprint",
            "%(_AtomUIResolvedLanguagePackProjectAsset.AtomUILanguageSourceFingerprint)");
    }

    [Fact]
    public async Task Project_Referenced_Language_Pack_Contributes_A_Generated_Bundle()
    {
        var repoRoot = Path.GetDirectoryName(GetRepoFile("AtomUI.slnx"))!;
        var configuration = new DirectoryInfo(AppContext.BaseDirectory).Parent?.Name ?? "Debug";
        var buildTasksAssembly = Path.Combine(
            repoRoot,
            "output",
            "bin",
            configuration,
            "netstandard2.0",
            "AtomUI.Build.Tasks.dll");
        var generatorAssembly = Path.Combine(AppContext.BaseDirectory, "AtomUI.Generator.dll");
        File.Exists(buildTasksAssembly).ShouldBeTrue();
        File.Exists(generatorAssembly).ShouldBeTrue();

        var fixtureRoot = Path.Combine(
            Path.GetTempPath(),
            $"atomui-language-pack-project-reference-{Guid.NewGuid():N}");
        Directory.CreateDirectory(fixtureRoot);
        try
        {
            var moduleDirectory = Path.Combine(fixtureRoot, "Module");
            var packDirectory = Path.Combine(fixtureRoot, "Pack");
            var consumerDirectory = Path.Combine(fixtureRoot, "Consumer");
            Directory.CreateDirectory(moduleDirectory);
            Directory.CreateDirectory(packDirectory);
            Directory.CreateDirectory(consumerDirectory);

            WriteProject(
                Path.Combine(moduleDirectory, "Module.csproj"),
                new XElement("PropertyGroup",
                    new XElement("TargetFramework", "net10.0"),
                    new XElement("Nullable", "enable")));
            await File.WriteAllTextAsync(
                Path.Combine(moduleDirectory, "Runtime.cs"),
                FixtureRuntimeSource,
                TestContext.Current.CancellationToken);

            var sourceXliffPath = Path.Combine(packDirectory, "Localization", "en-US.xlf");
            var targetXliffPath = Path.Combine(packDirectory, "Localization", "pt-BR.xlf");
            Directory.CreateDirectory(Path.GetDirectoryName(sourceXliffPath)!);
            await File.WriteAllTextAsync(
                sourceXliffPath,
                FixtureSourceXliff,
                TestContext.Current.CancellationToken);
            await File.WriteAllTextAsync(
                targetXliffPath,
                FixtureTargetXliff,
                TestContext.Current.CancellationToken);

            WriteLanguagePackProject(
                Path.Combine(packDirectory, "Pack.csproj"),
                repoRoot,
                buildTasksAssembly);
            WriteConsumerProject(
                Path.Combine(consumerDirectory, "Consumer.csproj"),
                repoRoot,
                generatorAssembly,
                Path.Combine(moduleDirectory, "Module.csproj"),
                Path.Combine(packDirectory, "Pack.csproj"));
            await File.WriteAllTextAsync(
                Path.Combine(consumerDirectory, "App.cs"),
                "namespace Fixture.App; public partial class App : Avalonia.Application { }",
                TestContext.Current.CancellationToken);

            var result = await RunDotNetBuildAsync(
                Path.Combine(consumerDirectory, "Consumer.csproj"),
                configuration);

            result.ExitCode.ShouldBe(0, result.Output);
            var generatedBootstrap = Directory.EnumerateFiles(
                                                  Path.Combine(consumerDirectory, "obj", "Generated"),
                                                  "GeneratedApplicationLanguageBootstrap.g.cs",
                                                  SearchOption.AllDirectories)
                                              .ShouldHaveSingleItem();
            var generatedSource = await File.ReadAllTextAsync(
                generatedBootstrap,
                TestContext.Current.CancellationToken);
            generatedSource.ShouldContain("TranslationSourceKind.StaticLanguagePack");
            generatedSource.ShouldContain("Fixture.Module.I18n.PtBR");
            generatedSource.ShouldContain("Cancelar");
        }
        finally
        {
            Directory.Delete(fixtureRoot, recursive: true);
        }
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

    [Fact]
    public void Official_PtBr_Language_Package_Projects_Match_The_Publishing_Contract()
    {
        var moduleProjects = new[]
        {
            new LanguagePackageProjectContract(
                "AtomUI.Controls.I18n.PtBR",
                "AtomUI.Controls",
                [
                    new(
                        "../../../AtomUI.Controls/Localization/Common/en-US.xlf",
                        "Localization/Common/en-US.xlf")
                ]),
            new LanguagePackageProjectContract(
                "AtomUI.Desktop.Controls.I18n.PtBR",
                "AtomUI.Desktop.Controls",
                [
                    new(
                        "../../../AtomUI.Desktop.Controls/Calendar/Localization/en-US.xlf",
                        "Localization/Calendar/en-US.xlf"),
                    new(
                        "../../../AtomUI.Desktop.Controls/DatePicker/Localization/en-US.xlf",
                        "Localization/DatePicker/en-US.xlf"),
                    new(
                        "../../../AtomUI.Desktop.Controls/Dialog/Localization/en-US.xlf",
                        "Localization/Dialog/en-US.xlf"),
                    new(
                        "../../../AtomUI.Desktop.Controls/ImagePreviewer/Localization/en-US.xlf",
                        "Localization/ImagePreviewer/en-US.xlf"),
                    new(
                        "../../../AtomUI.Desktop.Controls/Pagination/Localization/en-US.xlf",
                        "Localization/Pagination/en-US.xlf"),
                    new(
                        "../../../AtomUI.Desktop.Controls/QRCode/Localization/en-US.xlf",
                        "Localization/QRCode/en-US.xlf"),
                    new(
                        "../../../AtomUI.Desktop.Controls/TimePicker/Localization/en-US.xlf",
                        "Localization/TimePicker/en-US.xlf"),
                    new(
                        "../../../AtomUI.Desktop.Controls/Tour/Localization/en-US.xlf",
                        "Localization/Tour/en-US.xlf"),
                    new(
                        "../../../AtomUI.Desktop.Controls/Transfer/Localization/en-US.xlf",
                        "Localization/Transfer/en-US.xlf"),
                    new(
                        "../../../AtomUI.Desktop.Controls/Upload/Localization/en-US.xlf",
                        "Localization/Upload/en-US.xlf")
                ]),
            new LanguagePackageProjectContract(
                "AtomUI.Desktop.Controls.DataGrid.I18n.PtBR",
                "AtomUI.Desktop.Controls.DataGrid",
                [
                    new(
                        "../../../AtomUI.Desktop.Controls.DataGrid/Localization/en-US.xlf",
                        "Localization/en-US.xlf")
                ]),
            new LanguagePackageProjectContract(
                "AtomUI.Desktop.Controls.ColorPicker.I18n.PtBR",
                "AtomUI.Desktop.Controls.ColorPicker",
                [
                    new(
                        "../../../AtomUI.Desktop.Controls.ColorPicker/Localization/en-US.xlf",
                        "Localization/en-US.xlf")
                ])
        };

        foreach (var contract in moduleProjects)
        {
            AssertOfficialLanguageModuleProject(contract);
        }

        const string aggregatePackageId = "AtomUI.I18n.PtBR";
        var aggregateProjectPath = $"src/LanguagePacks/pt-BR/{aggregatePackageId}/{aggregatePackageId}.csproj";
        var aggregateProject = XDocument.Load(GetRepoFile(aggregateProjectPath));
        var aggregateProperties = GetProjectProperties(aggregateProject);

        aggregateProperties["TargetFramework"].ShouldBe("netstandard2.0");
        aggregateProperties["IsPackable"].ShouldBe("true");
        aggregateProperties["IncludeBuildOutput"].ShouldBe("false");
        aggregateProperties["SuppressDependenciesWhenPacking"].ShouldBe("false");
        aggregateProperties["PackageId"].ShouldBe(aggregatePackageId);
        aggregateProperties.ShouldNotContainKey("AtomUIBuildLanguagePackage");
        aggregateProperties["NuspecFile"].ShouldBe("AtomUI.I18n.PtBR.nuspec");
        aggregateProperties["NuspecBasePath"].ShouldBe("$(IntermediateOutputPath)nuspec-base");
        aggregateProperties["NuspecProperties"].ShouldBe("version=$(AtomUIVersion)");

        aggregateProject.Descendants()
                        .Where(element => element.Parent?.Name.LocalName == "ItemGroup" &&
                                          element.Name.LocalName != "ProjectReference")
                        .ShouldBeEmpty();
        var prepareNuspecBase = aggregateProject.Descendants("Target")
                                                  .Single(element =>
                                                      (string?)element.Attribute("Name") ==
                                                      "AtomUIPrepareAggregateNuspecBase");
        ((string?)prepareNuspecBase.Attribute("BeforeTargets")).ShouldBe("GenerateNuspec");
        ((string?)prepareNuspecBase.Descendants("RemoveDir").Single().Attribute("Directories"))
            .ShouldBe("$(NuspecBasePath)");
        ((string?)prepareNuspecBase.Descendants("MakeDir").Single().Attribute("Directories"))
            .ShouldBe("$(NuspecBasePath)");

        var expectedModuleProjectPaths = moduleProjects
            .Select(contract => $"../{contract.PackageId}/{contract.PackageId}.csproj")
            .ToArray();
        var aggregateProjectReferences = aggregateProject.Descendants("ProjectReference").ToArray();
        aggregateProjectReferences.Select(reference => (string)reference.Attribute("Include")!)
                                  .ShouldBe(expectedModuleProjectPaths, ignoreOrder: true);
        foreach (var projectReference in aggregateProjectReferences)
        {
            ((string?)projectReference.Attribute("ReferenceOutputAssembly")).ShouldBe("false");
            projectReference.Attribute("PrivateAssets").ShouldBeNull();
        }

        var aggregateNuspec = XDocument.Load(GetRepoFile(
            "src/LanguagePacks/pt-BR/AtomUI.I18n.PtBR/AtomUI.I18n.PtBR.nuspec"));
        var nuspecMetadata = aggregateNuspec.Descendants()
                                             .Single(element => element.Name.LocalName == "metadata");
        nuspecMetadata.Elements().Single(element => element.Name.LocalName == "id").Value
                      .ShouldBe(aggregatePackageId);
        nuspecMetadata.Elements().Single(element => element.Name.LocalName == "version").Value
                      .ShouldBe("$version$");
        nuspecMetadata.Descendants()
                      .Where(element => element.Name.LocalName == "group")
                      .ShouldBeEmpty();
        nuspecMetadata.Descendants()
                      .Where(element => element.Name.LocalName == "dependency")
                      .Select(element => new
                      {
                          Id = (string?)element.Attribute("id"),
                          Version = (string?)element.Attribute("version")
                      })
                      .ShouldBe(
                          moduleProjects.Select(contract => new
                          {
                              Id = (string?)contract.PackageId,
                              Version = (string?)"[$version$]"
                          }),
                          ignoreOrder: true);
        aggregateNuspec.Descendants()
                       .Where(element => element.Name.LocalName == "files")
                       .ShouldBeEmpty();

        var solution = XDocument.Load(GetRepoFile("AtomUI.slnx"));
        var solutionProjectPaths = solution.Descendants("Project")
                                           .Select(element => (string?)element.Attribute("Path"))
                                           .ToArray();
        foreach (var projectPath in moduleProjects
                     .Select(contract =>
                         $"src/LanguagePacks/pt-BR/{contract.PackageId}/{contract.PackageId}.csproj")
                     .Append(aggregateProjectPath))
        {
            solutionProjectPaths.ShouldContain(projectPath);
        }
    }

    [Fact]
    public void Gallery_Hosts_Consume_Official_PtBr_Translations_Through_Project_References()
    {
        string[] expectedReferences =
        [
            "../../src/LanguagePacks/pt-BR/AtomUI.Controls.I18n.PtBR/AtomUI.Controls.I18n.PtBR.csproj",
            "../../src/LanguagePacks/pt-BR/AtomUI.Desktop.Controls.I18n.PtBR/AtomUI.Desktop.Controls.I18n.PtBR.csproj",
            "../../src/LanguagePacks/pt-BR/AtomUI.Desktop.Controls.DataGrid.I18n.PtBR/AtomUI.Desktop.Controls.DataGrid.I18n.PtBR.csproj",
            "../../src/LanguagePacks/pt-BR/AtomUI.Desktop.Controls.ColorPicker.I18n.PtBR/AtomUI.Desktop.Controls.ColorPicker.I18n.PtBR.csproj"
        ];
        foreach (var projectPath in new[]
                 {
                     "controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj",
                     "controlgallery/AtomUIGallery.Browser/AtomUIGallery.Browser.csproj",
                     "tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj"
                 })
        {
            var project = XDocument.Load(GetRepoFile(projectPath));
            project.Descendants("AtomUILanguagePackProjectReference")
                   .Select(element => (string?)element.Attribute("Include"))
                   .ShouldBe(expectedReferences, ignoreOrder: true);

            var generatorReference = project.Descendants("ProjectReference")
                                            .Single(element =>
                                                ((string?)element.Attribute("Include"))?.EndsWith(
                                                    "src/AtomUI.Generator/AtomUI.Generator.csproj",
                                                    StringComparison.Ordinal) == true);
            ((string?)generatorReference.Attribute("OutputItemType")).ShouldBe("Analyzer");
            ((string?)generatorReference.Attribute("ReferenceOutputAssembly")).ShouldBe("false");
            ((string?)generatorReference.Attribute("PrivateAssets")).ShouldBe("all");
        }

        var galleryProject = XDocument.Load(
            GetRepoFile("controlgallery/AtomUIGallery/AtomUIGallery.csproj"));
        galleryProject.Descendants("AtomUILanguagePackProjectReference").ShouldBeEmpty();
    }

    private static void AssertOfficialLanguageModuleProject(LanguagePackageProjectContract contract)
    {
        var projectPath = $"src/LanguagePacks/pt-BR/{contract.PackageId}/{contract.PackageId}.csproj";
        var project = XDocument.Load(GetRepoFile(projectPath));
        var properties = GetProjectProperties(project);

        properties["TargetFramework"].ShouldBe("netstandard2.0");
        properties["IsPackable"].ShouldBe("true");
        properties["IncludeBuildOutput"].ShouldBe("false");
        properties["SuppressDependenciesWhenPacking"].ShouldBe("true");
        properties["PackageId"].ShouldBe(contract.PackageId);
        properties["AtomUIBuildLanguagePackage"].ShouldBe("true");
        properties["AtomUILanguageTag"].ShouldBe("pt-BR");
        properties["AtomUILanguageModuleId"].ShouldBe(contract.ModuleId);
        properties["AtomUILanguageContractVersion"].ShouldBe("2");
        properties["AtomUILanguageMinimumState"].ShouldBe("final");

        var projectReferences = project.Descendants("ProjectReference").ToArray();
        projectReferences.Length.ShouldBe(1);
        var generatorReference = projectReferences.Single();
        ((string?)generatorReference.Attribute("Include"))
            .ShouldBe("../../../AtomUI.Generator/AtomUI.Generator.csproj");
        ((string?)generatorReference.Attribute("OutputItemType")).ShouldBe("Analyzer");
        ((string?)generatorReference.Attribute("ReferenceOutputAssembly")).ShouldBe("false");
        ((string?)generatorReference.Attribute("PrivateAssets")).ShouldBe("all");

        var languageItems = project.Descendants("AtomUILanguage").ToArray();
        languageItems.Length.ShouldBe(contract.SourceLanguages.Count + 1);
        foreach (var sourceContract in contract.SourceLanguages)
        {
            var sourceLanguage = languageItems.Single(item =>
                (string?)item.Attribute("Include") == sourceContract.Include);
            AssertLanguageItemMetadata(
                sourceLanguage,
                "ModuleBuiltIn",
                "$(AtomUILanguageModuleId)",
                contract.ModuleId,
                "2");
            sourceLanguage.Elements()
                          .Single(element => element.Name.LocalName == "AtomUILanguagePackagePath")
                          .Value.ShouldBe(sourceContract.PackagePath);
        }

        var targetLanguage = languageItems.Single(item =>
            (string?)item.Attribute("Include") == "Localization/**/pt-BR.xlf");
        AssertLanguageItemMetadata(
            targetLanguage,
            "StaticLanguagePack",
            "$(PackageId)",
            contract.ModuleId,
            "2");
        targetLanguage.Elements()
                      .Single(element => element.Name.LocalName == "AtomUILanguagePackagePath")
                      .Value.ShouldBe("%(RecursiveDir)%(Filename)%(Extension)");
    }

    private static Dictionary<string, string> GetProjectProperties(XDocument project)
    {
        return project.Descendants()
                      .Where(element => element.Parent?.Name.LocalName == "PropertyGroup")
                      .ToDictionary(
                          element => element.Name.LocalName,
                          element => element.Value,
                          StringComparer.Ordinal);
    }

    private static void AssertLanguageItemMetadata(
        XElement item,
        string sourceKind,
        string sourceIdentity,
        string moduleId,
        string contractVersion)
    {
        item.Elements().Single(element => element.Name.LocalName == "AtomUILanguageSourceKind").Value
            .ShouldBe(sourceKind);
        item.Elements().Single(element => element.Name.LocalName == "AtomUILanguageSourceIdentity").Value
            .ShouldBe(sourceIdentity);
        item.Elements().Single(element => element.Name.LocalName == "AtomUILanguageModuleId").Value
            .ShouldBe(moduleId);
        item.Elements().Single(element => element.Name.LocalName == "AtomUILanguageContractVersion").Value
            .ShouldBe(contractVersion);
    }

    private static void AssertMetadataForwarded(
        XElement additionalFiles,
        string name,
        string? expected = null)
    {
        ((string?)additionalFiles.Attribute(name)).ShouldBe(expected ?? $"%({name})");
    }

    private static void AssertProjectAssetMetadata(XElement asset, bool requireFingerprint)
    {
        asset.Elements()
             .Single(element => element.Name.LocalName == "AtomUILanguageSourceKind")
             .Value.ShouldBe("%(_AtomUIPreparedLanguagePackProjectTargetFile.AtomUILanguageSourceKind)");
        asset.Elements()
             .Single(element => element.Name.LocalName == "AtomUILanguageSourceIdentity")
             .Value.ShouldBe("%(_AtomUIPreparedLanguagePackProjectTargetFile.AtomUILanguageSourceIdentity)");
        asset.Elements()
             .Single(element => element.Name.LocalName == "AtomUILanguageModuleId")
             .Value.ShouldBe("%(_AtomUIPreparedLanguagePackProjectTargetFile.AtomUILanguageModuleId)");
        asset.Elements()
             .Single(element => element.Name.LocalName == "AtomUILanguageContractVersion")
             .Value.ShouldBe("%(_AtomUIPreparedLanguagePackProjectTargetFile.AtomUILanguageContractVersion)");
        asset.Elements()
             .Single(element => element.Name.LocalName == "AtomUILanguagePackagePath")
             .Value.ShouldBe("%(_AtomUIPreparedLanguagePackProjectTargetFile.AtomUILanguagePackagePath)");
        if (requireFingerprint)
        {
            asset.Elements()
                 .Single(element => element.Name.LocalName == "AtomUILanguageSourceFingerprint")
                 .Value.ShouldBe("%(_AtomUIPreparedLanguagePackProjectTargetFile.AtomUILanguageSourceFingerprint)");
        }
    }

    private static void WriteProject(string path, params XElement[] content)
    {
        new XDocument(new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk"), content))
            .Save(path);
    }

    private static void WriteLanguagePackProject(
        string path,
        string repoRoot,
        string buildTasksAssembly)
    {
        new XDocument(
            new XElement(
                "Project",
                new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                new XElement(
                    "PropertyGroup",
                    new XElement("TargetFramework", "netstandard2.0"),
                    new XElement("IncludeBuildOutput", "false"),
                    new XElement("PackageId", "Fixture.Module.I18n.PtBR"),
                    new XElement("AtomUIBuildLanguagePackage", "true"),
                    new XElement("AtomUILanguageTag", "pt-BR"),
                    new XElement("AtomUILanguageModuleId", "Fixture.Module"),
                    new XElement("AtomUILanguageContractVersion", "2"),
                    new XElement("AtomUILanguageMinimumState", "final"),
                    new XElement("AtomUILocalizationBuildTasksAssembly", buildTasksAssembly)),
                new XElement(
                    "Import",
                    new XAttribute("Project", Path.Combine(repoRoot, "build", "AtomUI.Localization.props"))),
                new XElement(
                    "ItemGroup",
                    LanguageItem("Localization/en-US.xlf", "ModuleBuiltIn", "Fixture.Module"),
                    LanguageItem("Localization/pt-BR.xlf", "StaticLanguagePack", "$(PackageId)")),
                new XElement(
                    "Import",
                    new XAttribute("Project", Path.Combine(repoRoot, "build", "AtomUI.Localization.targets")))))
            .Save(path);
    }

    private static XElement LanguageItem(string include, string sourceKind, string sourceIdentity)
    {
        return new XElement(
            "AtomUILanguage",
            new XAttribute("Include", include),
            new XElement("AtomUILanguageSourceKind", sourceKind),
            new XElement("AtomUILanguageSourceIdentity", sourceIdentity),
            new XElement("AtomUILanguageModuleId", "Fixture.Module"),
            new XElement("AtomUILanguageContractVersion", "2"),
            new XElement("AtomUILanguagePackagePath", include));
    }

    private static void WriteConsumerProject(
        string path,
        string repoRoot,
        string generatorAssembly,
        string moduleProject,
        string languagePackProject)
    {
        new XDocument(
            new XElement(
                "Project",
                new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                new XElement(
                    "PropertyGroup",
                    new XElement("TargetFramework", "net10.0"),
                    new XElement("Nullable", "enable"),
                    new XElement("EmitCompilerGeneratedFiles", "true"),
                    new XElement("CompilerGeneratedFilesOutputPath", "$(BaseIntermediateOutputPath)Generated")),
                new XElement(
                    "Import",
                    new XAttribute("Project", Path.Combine(repoRoot, "build", "AtomUI.Localization.props"))),
                new XElement(
                    "ItemGroup",
                    new XElement("ProjectReference", new XAttribute("Include", moduleProject)),
                    new XElement("Analyzer", new XAttribute("Include", generatorAssembly)),
                    new XElement(
                        "AtomUILanguagePackProjectReference",
                        new XAttribute("Include", languagePackProject))),
                new XElement(
                    "Import",
                    new XAttribute("Project", Path.Combine(repoRoot, "build", "AtomUI.Localization.targets")))))
            .Save(path);
    }

    private static async Task<BuildResult> RunDotNetBuildAsync(string projectPath, string configuration)
    {
        var startInfo = new ProcessStartInfo(Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet")
        {
            WorkingDirectory = Path.GetDirectoryName(projectPath)!,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add("build");
        startInfo.ArgumentList.Add(projectPath);
        startInfo.ArgumentList.Add("--configuration");
        startInfo.ArgumentList.Add(configuration);
        startInfo.ArgumentList.Add("--nologo");

        using var process = Process.Start(startInfo).ShouldNotBeNull();
        var standardOutput = process.StandardOutput.ReadToEndAsync(TestContext.Current.CancellationToken);
        var standardError = process.StandardError.ReadToEndAsync(TestContext.Current.CancellationToken);
        await process.WaitForExitAsync(TestContext.Current.CancellationToken);
        return new BuildResult(
            process.ExitCode,
            await standardOutput,
            await standardError);
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

    private sealed record LanguagePackageProjectContract(
        string PackageId,
        string ModuleId,
        IReadOnlyList<SourceLanguageContract> SourceLanguages);

    private sealed record SourceLanguageContract(string Include, string PackagePath);

    private sealed record BuildResult(int ExitCode, string StandardOutput, string StandardError)
    {
        internal string Output => StandardOutput + StandardError;
    }

    private const string FixtureRuntimeSource = """
        using System.Reflection;

        [assembly: AssemblyMetadata("AtomUILanguageModuleId", "Fixture.Module")]

        namespace Avalonia
        {
            public abstract class Application { }
        }

        namespace AtomUI.Localization
        {
            [System.AttributeUsage(System.AttributeTargets.Enum, AllowMultiple = false)]
            public sealed class LanguageCatalogAttribute : System.Attribute
            {
                public int ContractVersion { get; set; } = 1;
            }

            public interface IGeneratedApplicationLanguageBootstrap
            {
                void RegisterApplicationLanguages(ILocalizationBuilder builder);
            }

            public interface ILocalizationBuilder
            {
                void AddCatalog(LanguageCatalogDescriptor descriptor);
                void AddTranslationBundle(TranslationBundleDescriptor descriptor);
            }

            public abstract class LanguageCatalogDescriptor { }

            public sealed class LanguageCatalogDescriptor<TResourceKind> : LanguageCatalogDescriptor
                where TResourceKind : struct, System.Enum
            {
                public LanguageCatalogDescriptor(
                    string catalogId,
                    int contractVersion,
                    System.Collections.Generic.IReadOnlyList<LanguageCatalogUnitDescriptor> units,
                    System.Func<TResourceKind, int> unitSlotResolver) { }
            }

            public sealed class LanguageCatalogUnitDescriptor
            {
                public LanguageCatalogUnitDescriptor(string key, bool isFormatted = false) { }
            }

            public readonly struct LanguageTag
            {
                public static LanguageTag Parse(string value) => default;
            }

            public enum TranslationSourceKind : byte
            {
                ModuleBuiltIn,
                StaticLanguagePack,
                ApplicationOverride
            }

            public sealed class TranslationBundleDescriptor
            {
                public TranslationBundleDescriptor(
                    string catalogId,
                    int contractVersion,
                    LanguageTag language,
                    TranslationSourceKind sourceKind,
                    string sourceIdentity,
                    System.Collections.Generic.IReadOnlyList<string?> values) { }
            }
        }

        namespace Fixture.Localization
        {
            [AtomUI.Localization.LanguageCatalog(ContractVersion = 2)]
            public enum Messages
            {
                Cancel
            }
        }
        """;

    private const string FixtureSourceXliff = """
        <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US">
          <file id="Fixture.Localization.Messages">
            <unit id="Cancel"><segment><source>Cancel</source></segment></unit>
          </file>
        </xliff>
        """;

    private const string FixtureTargetXliff = """
        <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US" trgLang="pt-BR">
          <file id="Fixture.Localization.Messages">
            <unit id="Cancel"><segment><source>Cancel</source><target state="final">Cancelar</target></segment></unit>
          </file>
        </xliff>
        """;
}
