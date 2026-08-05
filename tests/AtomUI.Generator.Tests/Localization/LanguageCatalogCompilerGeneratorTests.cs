using System.Collections.Immutable;
using AtomUI.Generator.Localization.Catalog;
using AtomUI.Generator.Localization.Xliff;
using AtomUI.Localization.Build;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;
using static AtomUI.Generator.Tests.Localization.LocalizationGeneratorTestHost;

namespace AtomUI.Generator.Tests.Localization;

public class LanguageCatalogCompilerGeneratorTests
{
    [Fact]
    public void Compiled_Bundle_Slots_Follow_Numeric_Ids_Instead_Of_File_Order()
    {
        var catalog = new LanguageCatalogInfo(
            "Test.Package",
            CatalogMetadataName,
            "TestApp.Localization",
            "global::TestApp.Localization.LoginLangResourceKind",
            1,
            [
                new LanguageCatalogUnitInfo(10, "Title", Location.None),
                new LanguageCatalogUnitInfo(30, "ItemCount", Location.None)
            ],
            Location.None);
        var file = new AdditionalLanguageFile(
            "Localization/en-US.xlf",
            "Test.Package",
            LanguageFileSourceKind.ModuleBuiltIn,
            "Test.Package",
            null,
            SourceText.From(SourceXliff),
            Xliff21Parser.Parse(SourceXliff).Document!);

        var result = LanguageCatalogCompiler.Compile(
            [new LanguageCatalogParseResult(catalog, ImmutableArray<Diagnostic>.Empty)],
            [new AdditionalLanguageFileParseResult(file, ImmutableArray<Diagnostic>.Empty)]);

        result.Diagnostics.ShouldBeEmpty();
        var compiled = result.Catalogs.ShouldHaveSingleItem();
        compiled.FormattedUnits.ShouldBe([false, true]);
        compiled.Bundles.ShouldHaveSingleItem().Values.ShouldBe(["Title", "Items {0}"]);
    }

    [Fact]
    public void Joins_Complete_BuiltIn_Languages_To_The_Catalog()
    {
        var result = Run(
            CatalogSource,
            SourceFile(),
            TargetFile("zh-CN", "标题", "项目 {0}"),
            TargetFile("zh-TW", "標題", "項目 {0}"));

        result.Diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void Reports_A_Missing_English_Source_Bundle()
    {
        var result = Run(
            CatalogSource,
            TargetFile("zh-CN", "标题", "项目 {0}"));

        AssertHasDiagnostic(result, "ATOMUILOC006", "complete en-US");
    }

    [Fact]
    public void Reports_Duplicate_English_Source_Bundles()
    {
        var result = Run(
            CatalogSource,
            SourceFile(),
            LanguageFile("Localization/duplicate.en-US.xlf", SourceXliff));

        AssertHasDiagnostic(result, "ATOMUILOC006", "exactly one en-US");
    }

    [Fact]
    public void Reports_A_File_Id_That_Does_Not_Match_The_Catalog()
    {
        var result = Run(
            CatalogSource,
            SourceFile().WithText(SourceXliff.Replace(CatalogMetadataName, "TestApp.Localization.OtherKind")));

        AssertHasDiagnostic(result, "ATOMUILOC006", "does not match");
    }

    [Theory]
    [InlineData("name=\"Title\"", "name=\"Heading\"", "name")]
    [InlineData("id=\"10\"", "id=\"11\"", "unit ID")]
    public void Reports_A_Unit_Contract_Mismatch(
        string oldValue,
        string newValue,
        string messageFragment)
    {
        var result = Run(
            CatalogSource,
            SourceFile().WithText(SourceXliff.Replace(oldValue, newValue)));

        AssertHasDiagnostic(result, "ATOMUILOC006", messageFragment);
    }

    [Fact]
    public void Reports_Target_Source_Text_That_Differs_From_English()
    {
        var result = Run(
            CatalogSource,
            SourceFile(),
            TargetFile("zh-CN", "标题", "项目 {0}")
                .WithText(TargetXliff("zh-CN", "标题", "项目 {0}")
                    .Replace("Items {0}", "Changed {0}")));

        AssertHasDiagnostic(result, "ATOMUILOC006", "source text");
    }

    [Fact]
    public void Reports_A_Target_That_Is_Not_Publishable()
    {
        var result = Run(
            CatalogSource,
            SourceFile(),
            TargetFile("zh-CN", "标题", "项目 {0}")
                .WithText(TargetXliff("zh-CN", "标题", "项目 {0}")
                    .Replace("state=\"translated\"", "state=\"initial\"")));

        AssertHasDiagnostic(result, "ATOMUILOC007", "publishable");
    }

    [Fact]
    public void Reports_Duplicate_Same_Priority_Translation_Sources()
    {
        var result = Run(
            CatalogSource,
            SourceFile(),
            TargetFile("zh-CN", "标题", "项目 {0}"),
            TargetFile("zh-CN", "另一标题", "另一个 {0}", "Localization/duplicate.zh-CN.xlf"));

        AssertHasDiagnostic(result, "ATOMUILOC006", "same priority");
    }

    [Fact]
    public void Resolves_A_Static_Language_Pack_Catalog_From_Metadata()
    {
        var reference = CreateExternalCatalogReference();
        var result = Run(
            "namespace TestApp { public sealed class Marker { } }",
            [reference],
            StaticPackFile(contractVersion: "2"));

        result.Diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void Reports_A_Static_Pack_For_An_Unknown_Referenced_Catalog()
    {
        var result = Run(
            "namespace TestApp { public sealed class Marker { } }",
            [],
            StaticPackFile(contractVersion: "2"));

        AssertHasDiagnostic(result, "ATOMUILOC006", "referenced Catalog");
    }

    [Fact]
    public void Reports_A_Static_Pack_Contract_Version_Mismatch()
    {
        var reference = CreateExternalCatalogReference();
        var result = Run(
            "namespace TestApp { public sealed class Marker { } }",
            [reference],
            StaticPackFile(contractVersion: "1"));

        AssertHasDiagnostic(result, "ATOMUILOC006", "ContractVersion");
    }

    [Fact]
    public void Reports_A_Local_Application_Override_Contract_Version_Mismatch()
    {
        var result = Run(
            CatalogSource,
            SourceFile(),
            ApplicationOverrideFile(contractVersion: "2"));

        AssertHasDiagnostic(result, "ATOMUILOC006", "ContractVersion");
    }

    [Fact]
    public void Reports_A_Static_Pack_Module_Id_Mismatch()
    {
        var reference = CreateExternalCatalogReference();
        var result = Run(
            "namespace TestApp { public sealed class Marker { } }",
            [reference],
            StaticPackFile(contractVersion: "2", moduleId: "Other.Package"));

        AssertHasDiagnostic(result, "ATOMUILOC006", "language module");
    }

    [Fact]
    public void Resolves_Module_Id_From_Referenced_Assembly_Metadata()
    {
        var reference = CreateExternalCatalogReference(
            assemblyName: "External.Package.Runtime",
            moduleId: "External.Package");
        var result = Run(
            "namespace TestApp { public sealed class Marker { } }",
            [reference],
            StaticPackFile(contractVersion: "2"));

        result.Diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void Accepts_A_Renamed_Referenced_Unit_When_The_Id_And_Current_Name_Match()
    {
        var reference = CreateExternalCatalogReference(unitName: "Heading");
        var result = Run(
            "namespace TestApp { public sealed class Marker { } }",
            [reference],
            StaticPackFile(contractVersion: "2", unitName: "Heading"));

        result.Diagnostics.ShouldBeEmpty();
    }

    private static void AssertHasDiagnostic(
        GeneratorRunResult result,
        string id,
        string messageFragment)
    {
        var diagnostic = result.Diagnostics.FirstOrDefault(diagnostic =>
            diagnostic.Id == id &&
            diagnostic.GetMessage().Contains(messageFragment, StringComparison.Ordinal));
        diagnostic.ShouldNotBeNull();
        diagnostic.Severity.ShouldBe(DiagnosticSeverity.Error);
    }

    private static TestAdditionalText SourceFile()
    {
        return LanguageFile("Localization/en-US.xlf", SourceXliff);
    }

    private static TestAdditionalText TargetFile(
        string language,
        string title,
        string itemCount,
        string? path = null)
    {
        return LanguageFile(
            path ?? $"Localization/{language}.xlf",
            TargetXliff(language, title, itemCount));
    }

    private static TestAdditionalText LanguageFile(string path, string text)
    {
        return new TestAdditionalText(
            path,
            text,
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["build_metadata.AdditionalFiles.AtomUILanguage"] = "true",
                ["build_metadata.AdditionalFiles.AtomUILanguageSourceKind"] = "ModuleBuiltIn",
                ["build_metadata.AdditionalFiles.AtomUILanguageSourceIdentity"] = "Test.Package"
            });
    }

    private static TestAdditionalText StaticPackFile(
        string contractVersion,
        string moduleId = "External.Package",
        string unitName = "Title")
    {
        return new TestAdditionalText(
            "packages/External.Package.I18n.ZhCN/zh-CN.xlf",
            TargetXliff("zh-CN", "标题", "项目 {0}")
                .Replace("name=\"Title\"", $"name=\"{unitName}\""),
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["build_metadata.AdditionalFiles.AtomUILanguage"] = "true",
                ["build_metadata.AdditionalFiles.AtomUILanguageSourceKind"] = "StaticLanguagePack",
                ["build_metadata.AdditionalFiles.AtomUILanguageSourceIdentity"] = "External.Package.I18n.ZhCN",
                ["build_metadata.AdditionalFiles.AtomUILanguageModuleId"] = moduleId,
                ["build_metadata.AdditionalFiles.AtomUILanguageContractVersion"] = contractVersion
            });
    }

    private static TestAdditionalText ApplicationOverrideFile(string contractVersion)
    {
        return new TestAdditionalText(
            "Localization/Overrides/zh-CN.xlf",
            TargetXliff("zh-CN", "覆盖标题", "覆盖项目 {0}"),
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["build_metadata.AdditionalFiles.AtomUILanguage"] = "true",
                ["build_metadata.AdditionalFiles.AtomUILanguageSourceKind"] = "ApplicationOverride",
                ["build_metadata.AdditionalFiles.AtomUILanguageSourceIdentity"] = "TestApp",
                ["build_metadata.AdditionalFiles.AtomUILanguageModuleId"] = "Test.Package",
                ["build_metadata.AdditionalFiles.AtomUILanguageContractVersion"] = contractVersion
            });
    }

    private static MetadataReference CreateExternalCatalogReference(
        string assemblyName = "External.Package",
        string? moduleId = null,
        string unitName = "Title")
    {
        var assemblyMetadata = moduleId is null
            ? string.Empty
            : $"[assembly: System.Reflection.AssemblyMetadata(\"AtomUILanguageModuleId\", \"{moduleId}\")]";
        return LocalizationGeneratorTestHost.CreateMetadataReference(
            assemblyName,
            $$"""
            {{assemblyMetadata}}

            namespace AtomUI.Localization
            {
                [System.AttributeUsage(System.AttributeTargets.Enum, AllowMultiple = false)]
                public sealed class LanguageCatalogAttribute : System.Attribute
                {
                    public int ContractVersion { get; set; } = 1;
                }
            }

            namespace TestApp.Localization
            {
                [AtomUI.Localization.LanguageCatalog(ContractVersion = 2)]
                public enum LoginLangResourceKind
                {
                    ItemCount = 30,
                    {{unitName}} = 10
                }
            }
            """);
    }

    private static string TargetXliff(string language, string title, string itemCount)
    {
        return $$"""
            <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US" trgLang="{{language}}">
              <file id="{{CatalogMetadataName}}">
                <unit id="30" name="ItemCount"><segment><source>Items {0}</source><target state="translated">{{itemCount}}</target></segment></unit>
                <unit id="10" name="Title"><segment><source>Title</source><target state="translated">{{title}}</target></segment></unit>
              </file>
            </xliff>
            """;
    }

    private const string CatalogMetadataName = "TestApp.Localization.LoginLangResourceKind";

    private const string CatalogSource = """
        namespace AtomUI.Localization
        {
            [System.AttributeUsage(System.AttributeTargets.Enum, AllowMultiple = false)]
            public sealed class LanguageCatalogAttribute : System.Attribute
            {
                public int ContractVersion { get; set; } = 1;
            }
        }

        namespace TestApp.Localization
        {
            using AtomUI.Localization;

            [LanguageCatalog(ContractVersion = 1)]
            public enum LoginLangResourceKind
            {
                ItemCount = 30,
                Title = 10
            }
        }
        """;

    private const string SourceXliff = """
        <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US">
          <file id="TestApp.Localization.LoginLangResourceKind">
            <unit id="30" name="ItemCount"><segment><source>Items {0}</source></segment></unit>
            <unit id="10" name="Title"><segment><source>Title</source></segment></unit>
          </file>
        </xliff>
        """;
}

internal static class TestAdditionalTextExtensions
{
    internal static LocalizationGeneratorTestHost.TestAdditionalText WithText(
        this LocalizationGeneratorTestHost.TestAdditionalText source,
        string text)
    {
        return new LocalizationGeneratorTestHost.TestAdditionalText(
            source.Path,
            text,
            source.Metadata);
    }
}
