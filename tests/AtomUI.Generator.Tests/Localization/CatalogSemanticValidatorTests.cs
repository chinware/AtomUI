using System.Collections.Immutable;
using AtomUI.Generator.Localization.Catalog;
using AtomUI.Generator.Localization.Xliff;
using AtomUI.Build.Tasks.LocalizationBuild;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.Localization;

public class CatalogSemanticValidatorTests
{
    [Fact]
    public void Validate_Requires_Exactly_One_Authoritative_English_Source()
    {
        var catalog = Catalog("Installed.Module", "Installed.Strings", ["Title"]);
        var target = Input(
            "Installed.Module",
            "Installed.Strings",
            LanguageFileSourceKind.StaticLanguagePack,
            LanguageFileContractValidation.Deferred,
            TargetXliff("Installed.Strings", "zh-CN", "Title", "标题"),
            sourceFingerprint: true);

        var result = CatalogSemanticValidator.Validate(WorkItem(catalog, [target]));

        result.Input.ShouldBeNull();
        result.Diagnostics.ShouldHaveSingleItem()
              .GetMessage()
              .ShouldContain("complete en-US");
    }

    [Fact]
    public void Validate_Applies_Source_And_Fingerprint_Rules_To_An_Active_Deferred_Input()
    {
        var catalog = Catalog("Installed.Module", "Installed.Strings", ["Title"]);
        var source = Input(
            "Installed.Module",
            "Installed.Strings",
            LanguageFileSourceKind.ModuleBuiltIn,
            LanguageFileContractValidation.Verified,
            SourceXliff("Installed.Strings", "Title"));
        var target = Input(
            "Installed.Module",
            "Installed.Strings",
            LanguageFileSourceKind.StaticLanguagePack,
            LanguageFileContractValidation.Deferred,
            TargetXliff("Installed.Strings", "zh-CN", "Changed title", "标题"),
            sourceFingerprint: true);

        var result = CatalogSemanticValidator.Validate(WorkItem(catalog, [source, target]));

        result.Input.ShouldBeNull();
        result.Diagnostics.ShouldContain(item =>
            item.GetMessage().Contains("source text", StringComparison.Ordinal));
        result.Diagnostics.ShouldContain(item =>
            item.GetMessage().Contains("source fingerprint", StringComparison.Ordinal));
    }

    private static CatalogCompilationWorkItem WorkItem(
        LanguageCatalogInfo catalog,
        ImmutableArray<LanguageFileInput> inputs)
    {
        return new CatalogCompilationWorkItem(
            new CatalogSymbolEntry(catalog, ownsCatalog: true),
            inputs.Select(input => new LanguageInputResolution(
                      input,
                      LanguageInputActivationState.Active,
                      catalog))
                  .ToImmutableArray());
    }

    private static LanguageCatalogInfo Catalog(
        string moduleId,
        string metadataName,
        ImmutableArray<string> keys)
    {
        return new LanguageCatalogInfo(
            moduleId,
            metadataName,
            "Installed",
            $"global::{metadataName}",
            keys.Select(static key => new LanguageCatalogUnitInfo(key, Location.None)).ToImmutableArray(),
            Location.None);
    }

    private static LanguageFileInput Input(
        string moduleId,
        string fileId,
        LanguageFileSourceKind sourceKind,
        LanguageFileContractValidation contractValidation,
        string content,
        bool sourceFingerprint = false)
    {
        var document = Xliff21Parser.Parse(content).Document!;
        return new LanguageFileInput(
            new AdditionalLanguageFile(
                $"{sourceKind}/{fileId}/{document.TargetLanguage ?? "en-US"}.xlf",
                SourceText.From(content),
                document),
            moduleId,
            sourceKind,
            $"{moduleId}.{sourceKind}.{document.TargetLanguage ?? "en-US"}",
            contractValidation,
            sourceFingerprint ? LanguageSourceFingerprint.Compute(document) : null);
    }

    private static string SourceXliff(string fileId, string source)
    {
        return $$"""
            <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US">
              <file id="{{fileId}}">
                <unit id="Title"><segment><source>{{source}}</source></segment></unit>
              </file>
            </xliff>
            """;
    }

    private static string TargetXliff(string fileId, string language, string source, string target)
    {
        return $$"""
            <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US" trgLang="{{language}}">
              <file id="{{fileId}}">
                <unit id="Title"><segment><source>{{source}}</source><target state="final">{{target}}</target></segment></unit>
              </file>
            </xliff>
            """;
    }
}
