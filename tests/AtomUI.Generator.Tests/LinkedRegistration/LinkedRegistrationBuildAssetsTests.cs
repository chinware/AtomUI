using System.Xml.Linq;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.LinkedRegistration;

public sealed class LinkedRegistrationBuildAssetsTests
{
    [Theory]
    [InlineData("PublishTrimmed")]
    [InlineData("PublishAot")]
    [InlineData("RunAOTCompilation")]
    [InlineData("AtomUIUseGeneratedRegistration")]
    public void Linked_Mode_Inputs_Are_Normalized(string propertyName)
    {
        var targets = XDocument.Load(GetRepoFile("build/nuget/linked-registration/LinkedRegistration.targets"));
        var linkedProperties = targets.Descendants()
                                      .Where(element =>
                                          element.Name.LocalName == "AtomUILinkedPublish")
                                      .ToArray();

        linkedProperties.Any(element =>
        {
            var condition = (string?)element.Attribute("Condition");
            return condition is not null &&
                   condition.Contains($"$({propertyName})", StringComparison.Ordinal) &&
                   string.Equals(element.Value.Trim(), "true", StringComparison.Ordinal);
        }).ShouldBeTrue();
    }

    [Fact]
    public void Ordinary_Build_Defaults_To_Full_Registration()
    {
        var targets = XDocument.Load(GetRepoFile("build/nuget/linked-registration/LinkedRegistration.targets"));

        targets.Descendants()
               .Single(element =>
                   element.Name.LocalName == "AtomUILinkedPublish" &&
                   (string?)element.Attribute("Condition") == "'$(AtomUILinkedPublish)' == ''")
               .Value.Trim()
               .ShouldBe("false");
    }

    [Fact]
    public void Application_Plan_Owner_Is_Derived_After_Project_OutputType()
    {
        var props = XDocument.Load(GetRepoFile("build/nuget/linked-registration/LinkedRegistration.props"));
        var targets = XDocument.Load(GetRepoFile("build/nuget/linked-registration/LinkedRegistration.targets"));

        props.Descendants().ShouldNotContain(element =>
            element.Name.LocalName == "AtomUIRegistrationPlanOwner");
        props.Descendants().ShouldNotContain(element =>
            element.Name.LocalName == "AtomUILinkedPublish");

        var ownerProperties = targets.Descendants()
                                     .Where(element =>
                                         element.Name.LocalName == "AtomUIRegistrationPlanOwner")
                                     .ToArray();
        ownerProperties.Length.ShouldBe(2);
        ownerProperties.Any(element =>
        {
            var condition = (string?)element.Attribute("Condition");
            return element.Value.Trim() == "true" &&
                   condition is not null &&
                   condition.Contains("'$(OutputType)' == 'Exe'", StringComparison.Ordinal) &&
                   condition.Contains("'$(OutputType)' == 'WinExe'", StringComparison.Ordinal);
        }).ShouldBeTrue();
        ownerProperties.Any(element =>
            element.Value.Trim() == "false" &&
            (string?)element.Attribute("Condition") ==
            "'$(AtomUIRegistrationPlanOwner)' == ''").ShouldBeTrue();
    }

    [Fact]
    public void Linked_Application_Owner_Installs_A_Trimmable_Feature_Switch()
    {
        var targets = XDocument.Load(GetRepoFile("build/nuget/linked-registration/LinkedRegistration.targets"));
        var option = targets.Descendants()
                            .Single(element =>
                                element.Name.LocalName == "RuntimeHostConfigurationOption" &&
                                (string?)element.Attribute("Include") ==
                                "AtomUI.AotTrimRegistration.Enabled");

        ((string?)option.Attribute("Value")).ShouldBe("true");
        ((string?)option.Attribute("Trim")).ShouldBe("true");
        var condition = (string?)option.Attribute("Condition");
        condition.ShouldNotBeNull();
        condition.ShouldContain("'$(AtomUILinkedPublish)' == 'true'");
        condition.ShouldContain("'$(AtomUIRegistrationPlanOwner)' == 'true'");
    }

    [Theory]
    [InlineData("IsTrimmable")]
    [InlineData("IsAotCompatible")]
    public void First_Party_Runtime_Libraries_Declare_Linker_Compatibility(string propertyName)
    {
        var targets = XDocument.Load(GetRepoFile("build/repository/ProjectDefaults.targets"));
        var property = targets.Descendants()
                              .Single(element =>
                                  element.Name.LocalName == propertyName &&
                                  element.Value.Trim() == "true");

        var groupCondition = (string?)property.Parent?.Attribute("Condition");
        groupCondition.ShouldNotBeNull();
        groupCondition.ShouldContain("$(MSBuildProjectName.StartsWith('AtomUI.'))");
        groupCondition.ShouldContain("'$(OutputType)' == 'Library'");
        groupCondition.ShouldContain("'$(IsTestProject)' != 'true'");
        groupCondition.ShouldContain("'$(IsRoslynComponent)' != 'true'");
        groupCondition.ShouldContain("$([MSBuild]::IsTargetFrameworkCompatible('$(TargetFramework)', 'net8.0'))");
        ((string?)property.Attribute("Condition")).ShouldBe($"'$({propertyName})' == ''");
    }

    [Fact]
    public void Registration_Product_Packages_Embed_Generator_Consumer_Assets()
    {
        var repositoryTargets = XDocument.Load(GetRepoFile(
            "build/repository/AtomUI.Repository.targets"));
        repositoryTargets.Descendants("Import")
                         .Single(element =>
                             (string?)element.Attribute("Project") ==
                             "$(MSBuildThisFileDirectory)PackageGeneratorAssets.targets")
                         .ShouldNotBeNull();

        var targets = XDocument.Load(GetRepoFile(
            "build/repository/PackageGeneratorAssets.targets"));
        var target = targets.Descendants("Target")
                            .Single(element =>
                                (string?)element.Attribute("Name") ==
                                "AtomUIPrepareGeneratorConsumerPackageAssets");

        ((string?)target.Attribute("BeforeTargets")).ShouldBe("_GetPackageFiles");
        var condition = (string?)target.Attribute("Condition");
        condition.ShouldNotBeNull();
        condition.ShouldContain("'$(IsPackable)' == 'true'");
        condition.ShouldContain("'$(AtomUIRegistrationPackageId)' != ''");

        var packedItems = target.Descendants("None").ToArray();
        packedItems.ShouldContain(element =>
            (string?)element.Attribute("Include") ==
            "$(MSBuildThisFileDirectory)../nuget/consumer/ProductPackage.targets" &&
            (string?)element.Attribute("PackagePath") == "buildTransitive/$(PackageId).targets");
        packedItems.ShouldContain(element =>
            (string?)element.Attribute("Include") == "@(AtomUINuGetBuildAsset)" &&
            (string?)element.Attribute("Pack") == "true");
        packedItems.ShouldContain(element =>
            (string?)element.Attribute("Include") == "@(AtomUIGeneratorToolAsset)" &&
            (string?)element.Attribute("Pack") == "true");
        packedItems.ShouldNotContain(element =>
            (string?)element.Attribute("PackagePath") == "buildTransitive/$(PackageId).props");

        var manifest = XDocument.Load(GetRepoFile("build/repository/GeneratorBuildAssets.props"));
        manifest.Descendants("AtomUINuGetBuildAsset")
                .ShouldHaveSingleItem()
                .Elements("PackagePath")
                .ShouldHaveSingleItem()
                .Value.ShouldBe("buildTransitive/%(RecursiveDir)%(Filename)%(Extension)");
        manifest.Descendants("AtomUIGeneratorToolAsset")
                .ShouldHaveSingleItem()
                .Elements("PackagePath")
                .ShouldHaveSingleItem()
                .Value.ShouldBe("tools/netstandard2.0/%(Filename)%(Extension)");
    }

    [Fact]
    public void Embedded_Generator_Consumer_Assets_Are_Compile_Time_Only_And_Idempotent()
    {
        var consumerTargets = XDocument.Load(GetRepoFile("build/nuget/consumer/ProductPackage.targets"));
        consumerTargets.Descendants("Import")
                       .Single(element =>
                           (string?)element.Attribute("Project") ==
                           "$(MSBuildThisFileDirectory)AtomUI.Generator.props")
                       .ShouldNotBeNull();
        consumerTargets.Descendants("Import")
                       .ShouldAllBe(element =>
                           !(((string?)element.Attribute("Condition")) ?? string.Empty)
                            .Contains("@(", StringComparison.Ordinal));
        consumerTargets.Root!
                       .Elements()
                       .Where(element => element.Name.LocalName != "Target")
                       .SelectMany(element => element.DescendantsAndSelf())
                       .ShouldAllBe(element =>
                           !(((string?)element.Attribute("Condition")) ?? string.Empty)
                            .Contains("@(", StringComparison.Ordinal));

        var target = consumerTargets.Descendants("Target")
                                    .Single(element =>
                                        (string?)element.Attribute("Name") ==
                                        "AtomUIAddEmbeddedGeneratorAnalyzer");
        ((string?)target.Attribute("BeforeTargets")).ShouldBe("CoreCompile");
        ((string?)target.Attribute("DependsOnTargets")).ShouldBe("ResolveReferences");

        var analyzer = target.Descendants("Analyzer").Single();
        ((string?)analyzer.Attribute("Include")).ShouldBe(
            "$(MSBuildThisFileDirectory)../tools/netstandard2.0/AtomUI.Generator.dll");
        var analyzerCondition = (string?)analyzer.Attribute("Condition");
        analyzerCondition.ShouldNotBeNull();
        analyzerCondition.ShouldContain("@(_AtomUIExistingGeneratorAnalyzer)");

        consumerTargets.Descendants("Import")
                       .Single(element =>
                           (string?)element.Attribute("Project") ==
                           "$(MSBuildThisFileDirectory)AtomUI.Generator.targets")
                       .ShouldNotBeNull();

        var generatorProps = XDocument.Load(GetRepoFile("build/nuget/AtomUI.Generator.props"));
        generatorProps.Descendants("AtomUIGeneratorPropsImported")
                      .Single()
                      .Value.Trim()
                      .ShouldBe("true");

        target.Descendants("AtomUIGeneratorAnalyzerImported")
              .Single()
                        .Value.Trim()
                        .ShouldBe("true");

        var generatorTargets = XDocument.Load(GetRepoFile("build/nuget/AtomUI.Generator.targets"));
        generatorTargets.Descendants("AtomUIGeneratorTargetsImported")
                        .Single()
                        .Value.Trim()
                        .ShouldBe("true");
    }

    [Fact]
    public void Linked_Owner_Validates_The_Generated_Plan_Marker_Before_Linking()
    {
        var targets = XDocument.Load(GetRepoFile("build/nuget/linked-registration/LinkedRegistration.targets"));
        var target = targets.Descendants()
                            .Single(element =>
                                element.Name.LocalName == "Target" &&
                                (string?)element.Attribute("Name") == "ValidateAtomUIApplicationRegistrationPlan");

        var condition = (string?)target.Attribute("Condition");
        condition.ShouldNotBeNull();
        condition.ShouldContain("'$(AtomUILinkedPublish)' == 'true'");
        condition.ShouldContain("'$(AtomUIRegistrationPlanOwner)' == 'true'");
        var beforeTargets = (string?)target.Attribute("BeforeTargets");
        beforeTargets.ShouldNotBeNull();
        beforeTargets.ShouldContain("PrepareForILLink");
        beforeTargets.ShouldContain("PrepareForAotCompilation");
        var validationTask = target.Descendants()
                                   .Single(element =>
                                       element.Name.LocalName ==
                                       "AtomUI.Build.Tasks.ValidateAssemblyMetadataMarkerTask");
        ((string?)validationTask.Attribute("AssemblyPath")).ShouldBe("$(TargetPath)");
        ((string?)validationTask.Attribute("MarkerKey")).ShouldBe("AtomUI.Linked.Plan.v1");
        validationTask.Descendants().ShouldContain(element =>
            element.Name.LocalName == "Output" &&
            (string?)element.Attribute("TaskParameter") == "MarkerCount" &&
            (string?)element.Attribute("PropertyName") == "_AtomUIApplicationPlanMarkerCount");
        target.Descendants().ShouldContain(element =>
            element.Name.LocalName == "Error" &&
            ((string?)element.Attribute("Text") ?? string.Empty)
            .Contains("ATOMUILINK001", StringComparison.Ordinal) &&
            ((string?)element.Attribute("Condition") ?? string.Empty)
            .Contains("_AtomUIApplicationPlanMarkerCount", StringComparison.Ordinal) &&
            ((string?)element.Attribute("Condition") ?? string.Empty)
            .Contains("!= '1'", StringComparison.Ordinal));
    }

    [Fact]
    public void Axaml_Usage_Is_Collected_Before_Compilation_Only_When_Input_Exists()
    {
        var targets = XDocument.Load(GetRepoFile("build/nuget/linked-registration/LinkedRegistration.targets"));
        var target = targets.Descendants()
                            .Single(element =>
                                element.Name.LocalName == "Target" &&
                                (string?)element.Attribute("Name") == "CollectAtomUIAxamlUsage");
        var task = target.Descendants()
                         .Single(element =>
                             element.Name.LocalName == "AtomUI.Build.Tasks.CollectAxamlUsageTask");

        var beforeTargets = (string?)target.Attribute("BeforeTargets");
        beforeTargets.ShouldNotBeNull();
        beforeTargets.ShouldContain("GenerateMSBuildEditorConfigFileShouldRun");
        beforeTargets.ShouldContain("CoreCompile");
        ((string?)target.Attribute("Condition")).ShouldBeNull();
        var inputProperty = target.Descendants()
                                  .Single(element =>
                                      element.Name.LocalName == "_AtomUIHasAxamlUsageInput");
        var inputCondition = (string?)inputProperty.Attribute("Condition");
        inputCondition.ShouldNotBeNull();
        inputCondition.ShouldContain("@(AvaloniaXaml)");
        inputCondition.ShouldContain("@(AtomUIRegistrationUnitRoot)");
        inputCondition.ShouldContain("@(AtomUIPackageRoot)");
        ((string?)task.Attribute("AxamlFiles")).ShouldBe("@(AvaloniaXaml)");
        ((string?)task.Attribute("UnitRoots")).ShouldBe("@(AtomUIRegistrationUnitRoot)");
        ((string?)task.Attribute("PackageRoots")).ShouldBe("@(AtomUIPackageRoot)");
        ((string?)task.Attribute("ProjectPackageId")).ShouldBe("$(AtomUIRegistrationPackageId)");
        var taskCondition = (string?)task.Attribute("Condition");
        taskCondition.ShouldNotBeNull();
        taskCondition.ShouldContain("$(_AtomUIHasAxamlUsageInput)");

        targets.Descendants().ShouldContain(element =>
            element.Name.LocalName == "AdditionalFiles" &&
            (string?)element.Attribute("Include") == "$(AtomUIAxamlUsageOutputPath)" &&
            (string?)element.Attribute("AtomUIAxamlUsage") == "true");
    }

    [Fact]
    public void Axaml_Usage_Additional_File_Metadata_Is_Visible_To_Source_Generators()
    {
        var targets = XDocument.Load(GetRepoFile("build/nuget/linked-registration/LinkedRegistration.targets"));

        targets.Descendants().ShouldContain(element =>
            element.Name.LocalName == "CompilerVisibleItemMetadata" &&
            (string?)element.Attribute("Include") == "AdditionalFiles" &&
            (string?)element.Attribute("MetadataName") == "AtomUIAxamlUsage");
    }

    [Fact]
    public void Linked_Properties_Are_Visible_To_Source_Generators()
    {
        var props = XDocument.Load(GetRepoFile("build/nuget/linked-registration/LinkedRegistration.props"));
        var visibleProperties = props.Descendants()
                                     .Where(element => element.Name.LocalName == "CompilerVisibleProperty")
                                     .Select(element => (string?)element.Attribute("Include"))
                                     .Where(static value => value is not null)
                                     .ToArray();

        visibleProperties.ShouldContain("AtomUILinkedPublish");
        visibleProperties.ShouldContain("AtomUIRegistrationStrict");
        visibleProperties.ShouldContain("AtomUIRegistrationPlanOwner");
        visibleProperties.ShouldContain("AtomUIRegistrationPackageId");
        visibleProperties.ShouldContain("AtomUIRegistrationEntries");
        visibleProperties.ShouldNotContain("AtomUIRegistrationCoreFeature");
    }

    [Fact]
    public void Linked_Registration_Uses_The_Shared_Build_Tasks_Assembly()
    {
        var targets = XDocument.Load(GetRepoFile("build/nuget/linked-registration/LinkedRegistration.targets"));

        targets.Descendants("UsingTask")
               .ShouldAllBe(element =>
                   (string?)element.Attribute("AssemblyFile") == "$(AtomUIBuildTasksAssembly)");
        targets.Descendants("Import")
               .ShouldContain(element =>
                   (string?)element.Attribute("Project") ==
                   "$(MSBuildThisFileDirectory)../infrastructure/BuildTasks.props" &&
                   (string?)element.Attribute("Condition") ==
                   "'$(AtomUIBuildTasksAssembly)' == ''");
    }

    [Theory]
    [InlineData("SelfContained")]
    [InlineData("PublishReadyToRun")]
    [InlineData("PublishSingleFile")]
    public void Non_Linking_Publish_Inputs_Do_Not_Enable_Linked_Mode(string propertyName)
    {
        var targets = XDocument.Load(GetRepoFile("build/nuget/linked-registration/LinkedRegistration.targets"));
        var enablingConditions = targets.Descendants()
                                         .Where(element =>
                                             element.Name.LocalName == "AtomUILinkedPublish" &&
                                             string.Equals(element.Value.Trim(), "true", StringComparison.Ordinal))
                                         .Select(element => (string?)element.Attribute("Condition"))
                                         .Where(static condition => condition is not null)
                                         .ToArray();

        enablingConditions.ShouldNotContain(condition =>
            condition!.Contains($"$({propertyName})", StringComparison.Ordinal));
    }

    [Fact]
    public void Theme_Assets_Expose_Only_Explicit_Unit_And_PackageShared_Metadata()
    {
        var targets = XDocument.Load(GetRepoFile("build/nuget/theme/ThemeAssets.targets"));
        var visibleProperties = targets.Descendants()
                                       .Where(element => element.Name.LocalName == "CompilerVisibleProperty")
                                       .Select(element => (string?)element.Attribute("Include"))
                                       .Where(static value => value is not null)
                                       .ToArray();
        var visibleMetadata = targets.Descendants()
                                     .Where(element => element.Name.LocalName == "CompilerVisibleItemMetadata")
                                     .Select(element => new
                                     {
                                         Item = (string?)element.Attribute("Include"),
                                         Metadata = (string?)element.Attribute("MetadataName")
                                     })
                                     .ToArray();

        visibleProperties.ShouldContain("AtomUIPackageSharedThemePaths");
        visibleMetadata.ShouldContain(item =>
            item.Item == "AdditionalFiles" && item.Metadata == "AtomUIRegistrationUnit");
        visibleMetadata.ShouldContain(item =>
            item.Item == "Compile" && item.Metadata == "AtomUIRegistrationUnit");
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
