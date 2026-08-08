using System.Collections.Immutable;
using AtomUI.Generator.Localization.Catalog;
using AtomUI.Generator.Localization.Xliff;
using AtomUI.Build.Tasks.LocalizationBuild;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;
using static AtomUI.Generator.Tests.Localization.LocalizationGeneratorTestHost;

namespace AtomUI.Generator.Tests.Localization;

public class CatalogCompilationPlannerTests
{
    [Theory]
    [InlineData(nameof(LanguageFileContractValidation.Verified))]
    [InlineData(nameof(LanguageFileContractValidation.Deferred))]
    public void Plan_Keeps_Static_Input_Dormant_When_Module_Is_Absent(
        string contractValidationName)
    {
        var contractValidation = Enum.Parse<LanguageFileContractValidation>(contractValidationName);
        var input = Input(
            "Optional.Module",
            "Optional.Strings",
            LanguageFileSourceKind.StaticLanguagePack,
            contractValidation,
            contractValidation == LanguageFileContractValidation.Verified ? 3 : null);
        var index = CreateIndex([], [input], []);

        var result = CatalogCompilationPlanner.Plan(index, [input]);

        result.Diagnostics.ShouldBeEmpty();
        result.WorkItems.ShouldBeEmpty();
        var resolution = result.DormantInputs.ShouldHaveSingleItem();
        resolution.ActivationState.ShouldBe(LanguageInputActivationState.Dormant);
        resolution.Input.ContractValidation.ShouldBe(contractValidation);
        resolution.Catalog.ShouldBeNull();
        resolution.EffectiveContractVersion.ShouldBeNull();
    }

    [Fact]
    public void Plan_Reports_Unknown_Catalog_When_Static_Module_Is_Active()
    {
        var input = Input(
            "Installed.Module",
            "Installed.Strings",
            LanguageFileSourceKind.StaticLanguagePack,
            LanguageFileContractValidation.Deferred,
            contractVersion: null);
        var moduleReference = CreateModuleReference("Installed.Module");
        var index = CreateIndex([], [input], [moduleReference]);

        var result = CatalogCompilationPlanner.Plan(index, [input]);

        result.DormantInputs.ShouldBeEmpty();
        result.WorkItems.ShouldBeEmpty();
        result.Diagnostics.ShouldHaveSingleItem()
              .GetMessage()
              .ShouldContain("Installed.Strings");
    }

    [Fact]
    public void Plan_Binds_Deferred_Active_Input_To_Actual_Catalog_Contract_Version()
    {
        var input = Input(
            "Installed.Module",
            "Installed.Strings",
            LanguageFileSourceKind.StaticLanguagePack,
            LanguageFileContractValidation.Deferred,
            contractVersion: null);
        var catalog = Catalog("Installed.Module", "Installed.Strings", contractVersion: 5);
        var index = CreateIndex([catalog], [input], []);

        var result = CatalogCompilationPlanner.Plan(index, [input]);

        result.Diagnostics.ShouldBeEmpty();
        result.DormantInputs.ShouldBeEmpty();
        var resolution = result.WorkItems.ShouldHaveSingleItem()
                               .Inputs
                               .ShouldHaveSingleItem();
        resolution.ActivationState.ShouldBe(LanguageInputActivationState.Active);
        resolution.Catalog.ShouldBe(catalog);
        resolution.EffectiveContractVersion.ShouldBe(5);
    }

    [Fact]
    public void Plan_Reports_Verified_Contract_Version_Mismatch_In_Active_Module()
    {
        var input = Input(
            "Installed.Module",
            "Installed.Strings",
            LanguageFileSourceKind.StaticLanguagePack,
            LanguageFileContractValidation.Verified,
            contractVersion: 4);
        var catalog = Catalog("Installed.Module", "Installed.Strings", contractVersion: 5);
        var index = CreateIndex([catalog], [input], []);

        var result = CatalogCompilationPlanner.Plan(index, [input]);

        result.WorkItems.SelectMany(static item => item.Inputs).ShouldBeEmpty();
        result.Diagnostics.ShouldHaveSingleItem()
              .GetMessage()
              .ShouldContain("ContractVersion");
    }

    private static CatalogSymbolIndex CreateIndex(
        ImmutableArray<LanguageCatalogInfo> catalogs,
        ImmutableArray<LanguageFileInput> inputs,
        IReadOnlyList<MetadataReference> references)
    {
        var compilation = CreateCompilation(
            "namespace TestApp { public sealed class Marker { } }",
            references);
        var result = CatalogSymbolIndex.Create(catalogs, inputs, compilation);
        result.Diagnostics.ShouldBeEmpty();
        return result.Index;
    }

    private static MetadataReference CreateModuleReference(string moduleId)
    {
        return CreateMetadataReference(
            $"{moduleId}.Runtime",
            $$"""
            [assembly: System.Reflection.AssemblyMetadata("AtomUILanguageModuleId", "{{moduleId}}")]

            namespace Installed
            {
                public sealed class Marker;
            }
            """);
    }

    private static LanguageCatalogInfo Catalog(
        string moduleId,
        string metadataName,
        int contractVersion)
    {
        return new LanguageCatalogInfo(
            moduleId,
            metadataName,
            "Installed",
            $"global::{metadataName}",
            contractVersion,
            [new LanguageCatalogUnitInfo("Title", Location.None)],
            Location.None);
    }

    private static LanguageFileInput Input(
        string moduleId,
        string fileId,
        LanguageFileSourceKind sourceKind,
        LanguageFileContractValidation contractValidation,
        int? contractVersion)
    {
        var content = TargetXliff(fileId);
        return new LanguageFileInput(
            new AdditionalLanguageFile(
                $"packages/{moduleId}/zh-CN.xlf",
                SourceText.From(content),
                Xliff21Parser.Parse(content).Document!),
            moduleId,
            sourceKind,
            $"{moduleId}.I18n.ZhCN",
            contractValidation,
            contractVersion,
            LanguageSourceFingerprint.Compute(Xliff21Parser.Parse(content).Document!));
    }

    private static string TargetXliff(string fileId)
    {
        return $$"""
            <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US" trgLang="zh-CN">
              <file id="{{fileId}}">
                <unit id="Title"><segment><source>Title</source><target state="final">标题</target></segment></unit>
              </file>
            </xliff>
            """;
    }
}
