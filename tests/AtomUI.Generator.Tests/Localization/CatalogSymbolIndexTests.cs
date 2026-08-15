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

public class CatalogSymbolIndexTests
{
    [Fact]
    public void Create_Does_Not_Collapse_Same_File_Id_Across_Modules()
    {
        var first = Catalog("First.Module", "Shared.Catalog");
        var second = Catalog("Second.Module", "Shared.Catalog");

        var result = CatalogSymbolIndex.Create(
            [first, second],
            ImmutableArray<LanguageFileInput>.Empty,
            compilation: null);

        result.Diagnostics.ShouldBeEmpty();
        result.Index.ShouldNotBeNull().TryGet(new CatalogKey("First.Module", "Shared.Catalog"), out var firstEntry)
              .ShouldBeTrue();
        result.Index.ShouldNotBeNull().TryGet(new CatalogKey("Second.Module", "Shared.Catalog"), out var secondEntry)
              .ShouldBeTrue();
        firstEntry.Catalog.ModuleId.ShouldBe("First.Module");
        secondEntry.Catalog.ModuleId.ShouldBe("Second.Module");
    }

    [Fact]
    public void Create_Indexes_Requested_Referenced_Catalogs_By_Structured_Key()
    {
        var reference = CreateMetadataReference(
            "ReferencedAssembly",
            """
            [assembly: System.Reflection.AssemblyMetadata("AtomUILanguageModuleId", "Referenced.Module")]
            namespace AtomUI.Localization
            {
                [System.AttributeUsage(System.AttributeTargets.Enum)]
                public sealed class LanguageCatalogAttribute : System.Attribute;
            }
            namespace Referenced
            {
                [AtomUI.Localization.LanguageCatalog]
                public enum Strings { Title }
            }
            """);
        var compilation = CreateCompilation(
            "namespace TestApp { public sealed class Marker { } }",
            [reference]);
        var input = Input("Referenced.Module", "Referenced.Strings");

        var result = CatalogSymbolIndex.Create(
            ImmutableArray<LanguageCatalogInfo>.Empty,
            [input],
            compilation);

        result.Diagnostics.ShouldBeEmpty();
        result.Index.ShouldNotBeNull().TryGet(new CatalogKey("Referenced.Module", "Referenced.Strings"), out var entry)
              .ShouldBeTrue();
        entry.OwnsCatalog.ShouldBeFalse();
        entry.Catalog.Units.ShouldHaveSingleItem().Key.ShouldBe("Title");
    }

    private static LanguageCatalogInfo Catalog(string moduleId, string metadataName)
    {
        return new LanguageCatalogInfo(
            moduleId,
            metadataName,
            "TestApp.Localization",
            $"global::{metadataName}",
            [new LanguageCatalogUnitInfo("Title", Location.None)],
            Location.None);
    }

    private static LanguageFileInput Input(
        string moduleId,
        string fileId)
    {
        const string target = """
            <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US" trgLang="ja-JP">
              <file id="Referenced.Strings">
                <unit id="Title"><segment><source>Hello</source><target state="final">こんにちは</target></segment></unit>
              </file>
            </xliff>
            """;
        var content = target.Replace("Referenced.Strings", fileId, StringComparison.Ordinal);
        return new LanguageFileInput(
            new AdditionalLanguageFile(
                $"packages/{moduleId}/ja-JP.xlf",
                SourceText.From(content),
                Xliff21Parser.Parse(content).Document!),
            moduleId,
            LanguageFileSourceKind.StaticLanguagePack,
            $"{moduleId}.I18n.JaJP",
            LanguageFileContractValidation.Verified,
            null);
    }
}
