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
        var validationTask = targets.Descendants()
                                    .Single(element =>
                                        element.Name.LocalName == "AtomUI.Build.Tasks.ValidateLanguageFilesTask");
        ((string?)validationTask.Attribute("MinimumTargetState"))
            .ShouldBe("$(AtomUILanguageMinimumState)");
        var prepareTask = targets.Descendants()
                                 .Single(element =>
                                     element.Name.LocalName == "AtomUI.Build.Tasks.PrepareLanguagePackageTask");
        ((string?)prepareTask.Attribute("MinimumTargetState"))
            .ShouldBe("$(AtomUILanguageMinimumState)");
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

    private sealed record LanguagePackageProjectContract(
        string PackageId,
        string ModuleId,
        IReadOnlyList<SourceLanguageContract> SourceLanguages);

    private sealed record SourceLanguageContract(string Include, string PackagePath);
}
