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
    public void Linked_Mode_Inputs_Are_Normalized(string propertyName)
    {
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.LinkedRegistration.targets"));
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
    public void Generated_Registration_Does_Not_Enable_Linked_Analysis()
    {
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.LinkedRegistration.targets"));
        targets.Descendants()
               .Where(element => element.Name.LocalName == "AtomUILinkedPublish")
               .Select(element => (string?)element.Attribute("Condition") ?? string.Empty)
               .ShouldAllBe(condition =>
                   !condition.Contains("$(AtomUIUseGeneratedRegistration)", StringComparison.Ordinal));
    }

    [Fact]
    public void Ordinary_Build_Defaults_To_Full_Registration()
    {
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.LinkedRegistration.targets"));

        targets.Descendants()
               .Single(element =>
                   element.Name.LocalName == "AtomUILinkedPublish" &&
                   (string?)element.Attribute("Condition") == "'$(AtomUILinkedPublish)' == ''")
               .Value.Trim()
               .ShouldBe("false");
    }

    [Fact]
    public void Registration_Granularity_Defaults_To_Package()
    {
        var props = XDocument.Load(GetRepoFile("build/AtomUI.LinkedRegistration.props"));
        var property = props.Descendants()
                            .Single(element =>
                                element.Name.LocalName == "AtomUIRegistrationGranularity");

        property.Value.Trim().ShouldBe("Package");
        ((string?)property.Attribute("Condition"))
            .ShouldBe("'$(AtomUIRegistrationGranularity)' == ''");
    }

    [Fact]
    public void Sidecar_Output_Is_Derived_After_Target_Framework_Output_Path()
    {
        var props = XDocument.Load(GetRepoFile("build/AtomUI.LinkedRegistration.props"));
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.LinkedRegistration.targets"));

        props.Descendants("AtomUILinkedSidecarOutputPath").ShouldBeEmpty();
        var outputPath = targets.Descendants("AtomUILinkedSidecarOutputPath").ShouldHaveSingleItem();
        ((string?)outputPath.Attribute("Condition"))
            .ShouldBe("'$(AtomUILinkedSidecarOutputPath)' == ''");
        outputPath.Value.Trim().ShouldBe(
            "$(TargetPath).atomui-link.json");
    }

    [Fact]
    public void Application_Plan_Owner_Is_Derived_After_Project_OutputType()
    {
        var props = XDocument.Load(GetRepoFile("build/AtomUI.LinkedRegistration.props"));
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.LinkedRegistration.targets"));

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
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.LinkedRegistration.targets"));
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
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.Repository.targets"));
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

    [Theory]
    [InlineData("EnableTrimAnalyzer", "PublishTrimmed", "PublishAot", "RunAOTCompilation")]
    [InlineData("EnableAotAnalyzer", "PublishAot", "RunAOTCompilation")]
    [InlineData("EnableSingleFileAnalyzer", "PublishSingleFile")]
    public void Publish_Analyzers_Run_Only_For_Matching_Publish_Modes(
        string analyzerProperty,
        params string[] enablingProperties)
    {
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.Repository.targets"));
        var properties = targets.Descendants()
                                .Where(element => element.Name.LocalName == analyzerProperty)
                                .ToArray();

        properties.Length.ShouldBe(2);
        var enabled = properties.Single(element => element.Value.Trim() == "true");
        var enabledCondition = (string?)enabled.Attribute("Condition");
        enabledCondition.ShouldNotBeNull();
        enabledCondition.ShouldContain($"'$({analyzerProperty})' == ''");
        foreach (var enablingProperty in enablingProperties)
        {
            enabledCondition.ShouldContain($"'$({enablingProperty})' == 'true'");
        }
        foreach (var unrelatedProperty in new[]
                 {
                     "PublishTrimmed",
                     "PublishAot",
                     "RunAOTCompilation",
                     "PublishSingleFile"
                 }.Except(enablingProperties, StringComparer.Ordinal))
        {
            enabledCondition.ShouldNotContain($"'$({unrelatedProperty})' == 'true'");
        }

        var disabled = properties.Single(element => element.Value.Trim() == "false");
        ((string?)disabled.Attribute("Condition"))
            .ShouldBe($"'$({analyzerProperty})' == ''");

        var repositoryProperties = targets.Descendants()
                                          .Where(element =>
                                              element.Parent?.Name.LocalName == "PropertyGroup")
                                          .ToArray();
        var analyzerIndex = Array.IndexOf(repositoryProperties, enabled);
        var linkerCompatibilityIndex = Array.FindIndex(
            repositoryProperties,
            element => element.Name.LocalName == "IsTrimmable");
        analyzerIndex.ShouldBeLessThan(linkerCompatibilityIndex);
    }

    [Fact]
    public void Registration_Product_Packages_Embed_Generator_Consumer_Assets()
    {
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.Repository.targets"));
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
            "$(MSBuildThisFileDirectory)AtomUI.GeneratorConsumer.targets" &&
            (string?)element.Attribute("PackagePath") == "buildTransitive/$(PackageId).targets");
        packedItems.ShouldContain(element =>
            (string?)element.Attribute("Include") == "@(AtomUINuGetBuildAsset)" &&
            (string?)element.Attribute("Pack") == "true");
        packedItems.ShouldContain(element =>
            (string?)element.Attribute("Include") == "@(AtomUIGeneratorToolAsset)" &&
            (string?)element.Attribute("Pack") == "true");
        packedItems.ShouldContain(element =>
            (string?)element.Attribute("Include") ==
            "$(MSBuildThisFileDirectory)AtomUI.Generator.props" &&
            (string?)element.Attribute("PackagePath") == "buildTransitive/$(PackageId).props" &&
            (string?)element.Attribute("Condition") == "'@(AtomUILanguage)' == ''");

        var repositoryProps = XDocument.Load(GetRepoFile("build/AtomUI.Repository.props"));
        repositoryProps.Descendants("AtomUINuGetBuildAsset")
                       .ShouldHaveSingleItem()
                       .Elements("PackagePath")
                       .ShouldHaveSingleItem()
                       .Value.ShouldBe("buildTransitive/%(Filename)%(Extension)");
        repositoryProps.Descendants("AtomUIGeneratorToolAsset")
                       .ShouldHaveSingleItem()
                       .Elements("PackagePath")
                       .ShouldHaveSingleItem()
                       .Value.ShouldBe("tools/netstandard2.0/%(Filename)%(Extension)");
    }

    [Fact]
    public void Only_Registration_Product_Packages_Emit_Sidecars_When_Packing()
    {
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.LinkedRegistration.targets"));
        var target = targets.Descendants("Target")
                            .Single(element =>
                                (string?)element.Attribute("Name") ==
                                "AtomUIPrepareLinkedRegistrationSidecarForPack");

        var condition = (string?)target.Attribute("Condition");
        condition.ShouldNotBeNull();
        condition.ShouldContain("'$(IsPackable)' == 'true'");
        condition.ShouldContain("'$(AtomUIRegistrationPackageId)' != ''");
    }

    [Fact]
    public void Pack_Sidecar_Inner_Build_Does_Not_Inherit_Publish_Or_NoBuild_Globals()
    {
        // dotnet pack --no-build exports NoBuild=true as a global property; if the inner
        // MSBuild call that emits the linked sidecar inherits it, the SDK fails the pack
        // with NETSDK1085 and registration packages silently lose their sidecar assets.
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.LinkedRegistration.targets"));
        var target = targets.Descendants("Target")
                            .Single(element =>
                                (string?)element.Attribute("Name") ==
                                "AtomUIPrepareLinkedRegistrationSidecarForPack");

        var removeProperties = (string?)target.Descendants("MSBuild")
                                              .Single()
                                              .Attribute("RemoveProperties");
        removeProperties.ShouldNotBeNull();
        var removed = removeProperties.Split(';', StringSplitOptions.RemoveEmptyEntries |
                                                StringSplitOptions.TrimEntries);
        removed.ShouldContain("NoBuild");
        removed.ShouldContain("PublishAot");
        removed.ShouldContain("PublishTrimmed");
        removed.ShouldContain("RunAOTCompilation");
    }

    [Fact]
    public void Pack_Sidecar_Inner_Build_Reuses_Existing_Project_Reference_Outputs()
    {
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.LinkedRegistration.targets"));
        var target = targets.Descendants("Target")
                            .Single(element =>
                                (string?)element.Attribute("Name") ==
                                "AtomUIPrepareLinkedRegistrationSidecarForPack");

        var properties = ((string?)target.Descendants("MSBuild")
                                                .Single()
                                                .Attribute("Properties"))
            .ShouldNotBeNull()
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        properties.ShouldContain("BuildProjectReferences=false");
    }

    [Fact]
    public void ProjectReference_Sidecars_Are_Collected_From_Resolved_Assembly_Paths()
    {
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.LinkedRegistration.targets"));
        var collectTarget = targets.Descendants("Target")
                                   .Single(element =>
                                       (string?)element.Attribute("Name") ==
                                       "CollectAtomUIProjectReferenceSidecars");
        var getTarget = targets.Descendants("Target")
                               .Single(element =>
                                   (string?)element.Attribute("Name") ==
                                   "GetAtomUILinkedRegistrationSidecar");

        ((string?)collectTarget.Attribute("DependsOnTargets"))
            .ShouldBe("ResolveReferences");
        collectTarget.Descendants("MSBuild").ShouldBeEmpty();
        var legacyCandidate = collectTarget.Descendants("_AtomUILegacyLinkedSidecarCandidate")
                                           .ShouldHaveSingleItem();
        legacyCandidate.Attribute("Include")
                       ?.Value.ShouldBe(
                           "@(AdditionalFiles->WithMetadataValue('AtomUILinkedSidecar', 'true'))");
        ((string?)legacyCandidate.Attribute("KeepMetadata"))
            .ShouldBe("AtomUILinkedSidecarSource");
        collectTarget.Descendants("AdditionalFiles")
                     .ShouldContain(element =>
                         (string?)element.Attribute("Remove") ==
                         "@(_AtomUILegacyLinkedSidecarCandidate)");
        var referencePath = collectTarget.Descendants("_AtomUILinkedReferencePath")
                                         .ShouldHaveSingleItem();
        referencePath.Attribute("Include")
                     ?.Value.ShouldBe("@(ReferencePath->'%(FullPath)')");
        ((string?)referencePath.Attribute("KeepMetadata"))
            .ShouldBe("FullPath");
        var referenceCandidate = collectTarget.Descendants("_AtomUILinkedReferenceSidecarCandidate")
                                              .ShouldHaveSingleItem();
        referenceCandidate.Attribute("Include")
                          ?.Value.ShouldBe("@(ReferencePath->'%(FullPath).atomui-link.json')");
        ((string?)referenceCandidate.Attribute("KeepMetadata"))
            .ShouldBe("AtomUILinkedSidecarSource");
        collectTarget.Descendants("AtomUILinkedSidecarCandidate")
                     .ShouldAllBe(element =>
                         (string?)element.Attribute("KeepMetadata") ==
                         "AtomUILinkedSidecarSource");
        var resolvers = collectTarget.Descendants("AtomUI.Build.Tasks.ResolveLinkedRegistrationSidecarCandidatesTask")
                                     .ToArray();
        resolvers.Length.ShouldBe(2);
        resolvers.ShouldAllBe(element =>
            (string?)element.Attribute("ReferencePaths") == "@(_AtomUILinkedReferencePath)");
        collectTarget.Descendants("Output")
                     .ShouldContain(element =>
                         (string?)element.Attribute("TaskParameter") == "CanonicalSidecars" &&
                         (string?)element.Attribute("ItemName") == "_AtomUICanonicalLinkedSidecar");
        collectTarget.Descendants("AdditionalFiles")
                     .ShouldContain(element =>
                         (string?)element.Attribute("Include") == "@(_AtomUICanonicalLinkedSidecar)");
        getTarget.Attribute("DependsOnTargets").ShouldBeNull();
        getTarget.Descendants("_AtomUILinkedRegistrationSidecarTargetOutput")
                 .ShouldAllBe(element =>
                     (string?)element.Attribute("Include") !=
                     "@(_AtomUIProjectReferenceSidecar)");

        var packTarget = targets.Descendants("Target")
                                .Single(element =>
                                    (string?)element.Attribute("Name") ==
                                    "AtomUIPrepareLinkedRegistrationSidecarForPack");
        ((string?)packTarget.Descendants("MSBuild").Single().Attribute("Targets"))
            .ShouldBe("Build;GetAtomUILinkedRegistrationSidecar");
    }

    [Fact]
    public void Package_Sidecars_Enter_The_Candidate_Catalog_Before_AdditionalFiles()
    {
        var targets = XDocument.Load(
            GetRepoFile("build/AtomUI.LinkedRegistration.SidecarConsumer.targets"));

        targets.Descendants("AtomUILinkedSidecarCandidate")
               .ShouldHaveSingleItem()
               .ShouldSatisfyAllConditions(
                   element => ((string?)element.Attribute("Include")).ShouldBe(
                       "$(MSBuildThisFileDirectory)AtomUI.LinkedRegistration/*.atomui-link.json"),
                   element => ((string?)element.Attribute("AtomUILinkedSidecarSource"))
                       .ShouldBe("Package"));
        targets.Descendants("AdditionalFiles").ShouldBeEmpty();
    }

    [Fact]
    public void Missing_ProjectReference_Sidecars_Are_Extracted_With_A_Fallback_Marker()
    {
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.LinkedRegistration.targets"));
        var collectTarget = targets.Descendants("Target")
                                   .Single(element =>
                                       (string?)element.Attribute("Name") ==
                                       "CollectAtomUIProjectReferenceSidecars");

        collectTarget.Descendants("Output")
                     .ShouldContain(element =>
                         (string?)element.Attribute("TaskParameter") == "ExtractableReferences" &&
                         (string?)element.Attribute("ItemName") == "_AtomUILinkedExtractableReference");
        var resolver = collectTarget.Descendants("AtomUI.Build.Tasks.ResolveLinkedRegistrationSidecarCandidatesTask")
                                    .Single(element => element.Descendants("Output")
                                        .Any(output =>
                                            (string?)output.Attribute("TaskParameter") == "ExtractableReferences"));
        resolver.Descendants("Output")
                .ShouldContain(element =>
                    (string?)element.Attribute("TaskParameter") == "ExtractableReferences" &&
                    (string?)element.Attribute("ItemName") == "_AtomUILinkedExtractableReference");

        var extraction = collectTarget.Descendants("AtomUI.Build.Tasks.GenerateLinkedRegistrationSidecarTask")
                                      .ShouldHaveSingleItem();
        ((string?)extraction.Attribute("ExtractedFallback")).ShouldBe("true");
        ((string?)extraction.Attribute("ContinueOnError")).ShouldBe("WarnAndContinue");
        var outputPath = (string?)extraction.Attribute("OutputPath");
        outputPath.ShouldNotBeNull();
        outputPath.ShouldContain("AtomUIExtractedSidecars");
        outputPath.ShouldContain("%(_AtomUILinkedExtractableReference.AtomUILinkedAssemblyName)");
        collectTarget.Descendants("_AtomUILinkedSidecarCanonicalCandidate")
                     .ShouldContain(element =>
                         (string?)element.Attribute("Include") == "@(_AtomUIExtractedLinkedSidecar)" &&
                         (string?)element.Attribute("AtomUILinkedSidecarSource") == "MetadataExtraction");
        collectTarget.Descendants("AdditionalFiles")
                     .ShouldAllBe(element =>
                         (string?)element.Attribute("Include") != "@(_AtomUIExtractedLinkedSidecar)");
    }

    [Fact]
    public void Sidecars_Are_Emitted_After_The_Target_Assembly_Reaches_TargetPath()
    {
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.LinkedRegistration.targets"));
        var emitTarget = targets.Descendants("Target")
                                .Single(element =>
                                    (string?)element.Attribute("Name") ==
                                    "EmitAtomUILinkedRegistrationSidecar");

        var afterTargets = (string?)emitTarget.Attribute("AfterTargets");
        afterTargets.ShouldNotBeNull();
        afterTargets.ShouldBe("CopyFilesToOutputDirectory");
        afterTargets.ShouldNotContain("CoreCompile");
        var condition = (string?)emitTarget.Attribute("Condition");
        condition.ShouldNotBeNull();
        condition.ShouldContain("Exists('$(TargetPath)')");
        ((string?)emitTarget.Descendants("AtomUI.Build.Tasks.GenerateLinkedRegistrationSidecarTask")
                           .Single()
                           .Attribute("AssemblyPath"))
            .ShouldBe("$(TargetPath)");
    }

    [Fact]
    public void Embedded_Generator_Consumer_Assets_Are_Compile_Time_Only_And_Idempotent()
    {
        var consumerTargets = XDocument.Load(GetRepoFile("build/AtomUI.GeneratorConsumer.targets"));
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

        var generatorProps = XDocument.Load(GetRepoFile("build/AtomUI.Generator.props"));
        generatorProps.Descendants("AtomUIGeneratorPropsImported")
                      .Single()
                      .Value.Trim()
                      .ShouldBe("true");

        target.Descendants("AtomUIGeneratorAnalyzerImported")
              .Single()
                        .Value.Trim()
                        .ShouldBe("true");

        var generatorTargets = XDocument.Load(GetRepoFile("build/AtomUI.Generator.targets"));
        generatorTargets.Descendants("AtomUIGeneratorTargetsImported")
                        .Single()
                        .Value.Trim()
                        .ShouldBe("true");
    }

    [Fact]
    public void Linked_Owner_Validates_The_Generated_Plan_Marker_Before_Linking()
    {
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.LinkedRegistration.targets"));
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
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.LinkedRegistration.targets"));
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
        ((string?)target.Attribute("Condition"))
            .ShouldBe("'$(_AtomUICollectAxamlUsage)' == 'true'");
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
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.LinkedRegistration.targets"));

        targets.Descendants().ShouldContain(element =>
            element.Name.LocalName == "CompilerVisibleItemMetadata" &&
            (string?)element.Attribute("Include") == "AdditionalFiles" &&
            (string?)element.Attribute("MetadataName") == "AtomUIAxamlUsage");
    }

    [Fact]
    public void Linked_Properties_Are_Visible_To_Source_Generators()
    {
        var props = XDocument.Load(GetRepoFile("build/AtomUI.LinkedRegistration.props"));
        var visibleProperties = props.Descendants()
                                     .Where(element => element.Name.LocalName == "CompilerVisibleProperty")
                                     .Select(element => (string?)element.Attribute("Include"))
                                     .Where(static value => value is not null)
                                     .ToArray();

        visibleProperties.ShouldContain("AtomUILinkedPublish");
        visibleProperties.ShouldContain("AtomUIRegistrationStrict");
        visibleProperties.ShouldContain("AtomUIRegistrationPlanOwner");
        visibleProperties.ShouldContain("AtomUIRegistrationPackageId");
        visibleProperties.ShouldContain("AtomUIRegistrationGranularity");
        visibleProperties.ShouldNotContain("AtomUIRegistration" + "Entries");
        visibleProperties.ShouldNotContain("AtomUIRegistrationCoreFeature");
    }

    [Fact]
    public void Linked_Registration_Uses_The_Shared_Build_Tasks_Assembly()
    {
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.LinkedRegistration.targets"));

        targets.Descendants("UsingTask")
               .ShouldAllBe(element =>
                   (string?)element.Attribute("AssemblyFile") == "$(AtomUIBuildTasksAssembly)");
        targets.Descendants("Import").ShouldBeEmpty();
    }

    [Theory]
    [InlineData("SelfContained")]
    [InlineData("PublishReadyToRun")]
    [InlineData("PublishSingleFile")]
    public void Non_Linking_Publish_Inputs_Do_Not_Enable_Linked_Mode(string propertyName)
    {
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.LinkedRegistration.targets"));
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
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.ThemeAssets.targets"));
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
