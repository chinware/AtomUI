using System.Collections.Immutable;
using AtomUI.Generator.Diagnostics;
using AtomUI.Generator.Localization;
using AtomUI.Generator.Localization.Catalog;
using AtomUI.Generator.Localization.Xliff;
using AtomUI.Build.Tasks.LocalizationBuild;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.Localization;

public class LocalizationPipelineTests
{
    [Fact]
    public void Compile_Preserves_Input_Diagnostics_And_Skips_The_Compilation_Plan()
    {
        var diagnostic = Diagnostic.Create(
            AtomUIDiagnosticDescriptors.LocalizationInvalidCatalog,
            Location.None,
            "BrokenCatalog",
            "the target must be a non-generic enum");

        var result = LocalizationPipeline.Compile(
            "TestApp",
            [new LanguageCatalogParseResult(null, [diagnostic])],
            ImmutableArray<LanguageFileInputResult>.Empty,
            compilation: null,
            ImmutableArray<ApplicationLanguageHostInfo>.Empty);

        result.AssemblyName.ShouldBe("TestApp");
        result.Diagnostics.ShouldHaveSingleItem().Id.ShouldBe("ATOMUILOC003");
        result.Plan.Catalogs.ShouldBeEmpty();
    }

    [Fact]
    public void Compile_Produces_A_Localization_Compilation_Plan_For_Valid_Input()
    {
        var catalog = Catalog();
        var file = Input();

        var result = LocalizationPipeline.Compile(
            "TestApp",
            [new LanguageCatalogParseResult(catalog, ImmutableArray<Diagnostic>.Empty)],
            [new LanguageFileInputResult(file, ImmutableArray<Diagnostic>.Empty)],
            compilation: null,
            ImmutableArray<ApplicationLanguageHostInfo>.Empty);

        result.Diagnostics.ShouldBeEmpty();
        var compiled = result.Plan.Catalogs.ShouldHaveSingleItem();
        compiled.Catalog.CatalogId.ShouldBe("Test.Package:TestApp.Localization.AppLangResourceKind");
        compiled.Bundles.ShouldHaveSingleItem().Values.ShouldBe(["Title"]);
    }

    private static LanguageCatalogInfo Catalog()
    {
        return new LanguageCatalogInfo(
            "Test.Package",
            "TestApp.Localization.AppLangResourceKind",
            "TestApp.Localization",
            "global::TestApp.Localization.AppLangResourceKind",
            1,
            [new LanguageCatalogUnitInfo("Title", Location.None)],
            Location.None);
    }

    private static LanguageFileInput Input()
    {
        const string content = """
            <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US">
              <file id="TestApp.Localization.AppLangResourceKind">
                <unit id="Title"><segment><source>Title</source></segment></unit>
              </file>
            </xliff>
            """;
        return new LanguageFileInput(
            new AdditionalLanguageFile(
                "Localization/en-US.xlf",
                SourceText.From(content),
                Xliff21Parser.Parse(content).Document!),
            "Test.Package",
            LanguageFileSourceKind.ModuleBuiltIn,
            "Test.Package",
            LanguageFileContractValidation.Verified,
            contractVersion: null,
            sourceFingerprint: null);
    }
}
