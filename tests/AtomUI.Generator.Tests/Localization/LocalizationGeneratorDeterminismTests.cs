using Microsoft.CodeAnalysis;
using Shouldly;
using Xunit;
using static AtomUI.Generator.Tests.Localization.LocalizationGeneratorTestHost;

namespace AtomUI.Generator.Tests.Localization;

public class LocalizationGeneratorDeterminismTests
{
    [Fact]
    public void Reordered_Catalogs_Files_And_Units_Produce_Byte_Identical_Sources()
    {
        var first = Run(
            RuntimeSource + CatalogAThenBSource,
            LanguageFile("Localization/A/en-US.xlf", CatalogAXliff),
            LanguageFile("Localization/B/en-US.xlf", CatalogBXliff));
        var second = Run(
            RuntimeSource + CatalogBThenASource,
            LanguageFile("Localization/B/en-US.xlf", ReverseUnits(CatalogBXliff)),
            LanguageFile("Localization/A/en-US.xlf", ReverseUnits(CatalogAXliff)));

        first.Diagnostics.ShouldBeEmpty();
        second.Diagnostics.ShouldBeEmpty();
        GetSources(second).ShouldBe(GetSources(first));
    }

    [Fact]
    public void Updating_One_Xliff_Changes_Only_Its_Catalog_Registration_Source()
    {
        var original = LanguageFile("Localization/A/en-US.xlf", CatalogAXliff);
        var replacement = LanguageFile(
            "Localization/A/en-US.xlf",
            CatalogAXliff.Replace("Alpha", "Changed alpha"));
        var execution = RunWithUpdatedAdditionalText(
            RuntimeSource + CatalogAThenBSource,
            original,
            replacement,
            LanguageFile("Localization/B/en-US.xlf", CatalogBXliff));
        var first = execution.FirstRun.Results.ShouldHaveSingleItem();
        var second = execution.SecondRun.Results.ShouldHaveSingleItem();

        first.Diagnostics.ShouldBeEmpty();
        second.Diagnostics.ShouldBeEmpty();
        var before = GetSources(first);
        var after = GetSources(second);
        var changedHints = before.Keys.Where(hint => before[hint] != after[hint]).ToArray();

        changedHints.ShouldBe([
            "TestApp.Localization.CatalogALangResourceKind.LanguageCatalogRegistration.g.cs"
        ]);
        after["GeneratedLanguageModuleRegistration.g.cs"]
            .ShouldBe(before["GeneratedLanguageModuleRegistration.g.cs"]);
        after["GeneratedApplicationLanguageBootstrap.g.cs"]
            .ShouldBe(before["GeneratedApplicationLanguageBootstrap.g.cs"]);

        var languageFileReasons = second.TrackedSteps["LocalizationLanguageFiles"]
                                        .SelectMany(static step => step.Outputs)
                                        .Select(static output => output.Reason)
                                        .ToArray();
        languageFileReasons.Count(static reason =>
            reason == IncrementalStepRunReason.Modified).ShouldBe(1);
        languageFileReasons.ShouldContain(IncrementalStepRunReason.Cached);
        second.TrackedSteps["LocalizationCompilation"]
              .SelectMany(static step => step.Outputs)
              .Select(static output => output.Reason)
              .ShouldContain(IncrementalStepRunReason.Modified);
    }

    private static SortedDictionary<string, string> GetSources(GeneratorRunResult result)
    {
        return new SortedDictionary<string, string>(
            result.GeneratedSources.ToDictionary(
                static source => source.HintName,
                static source => source.SourceText.ToString(),
                StringComparer.Ordinal),
            StringComparer.Ordinal);
    }

    private static TestAdditionalText LanguageFile(string path, string content)
    {
        return new TestAdditionalText(
            path,
            content,
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["build_metadata.AdditionalFiles.AtomUILanguage"] = "true",
                ["build_metadata.AdditionalFiles.AtomUILanguageSourceKind"] = "ModuleBuiltIn",
                ["build_metadata.AdditionalFiles.AtomUILanguageSourceIdentity"] = "Test.Package"
            });
    }

    private static string ReverseUnits(string source)
    {
        var firstUnitStart = source.IndexOf("    <unit", StringComparison.Ordinal);
        var firstUnitEnd = source.IndexOf("</unit>", firstUnitStart, StringComparison.Ordinal) +
                           "</unit>".Length;
        var secondUnitStart = source.IndexOf("    <unit", firstUnitEnd, StringComparison.Ordinal);
        var secondUnitEnd = source.IndexOf("</unit>", secondUnitStart, StringComparison.Ordinal) +
                            "</unit>".Length;
        var firstUnit = source.Substring(firstUnitStart, firstUnitEnd - firstUnitStart);
        var secondUnit = source.Substring(secondUnitStart, secondUnitEnd - secondUnitStart);
        return source.Substring(0, firstUnitStart) + secondUnit + "\n" + firstUnit +
               source.Substring(secondUnitEnd);
    }

    private const string RuntimeSource = """
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
        }
        """;

    private const string CatalogAThenBSource = """
        namespace TestApp.Localization
        {
            [AtomUI.Localization.LanguageCatalog]
            public enum CatalogALangResourceKind
            {
                Count,
                Title
            }

            [AtomUI.Localization.LanguageCatalog]
            public enum CatalogBLangResourceKind
            {
                Confirm,
                Cancel
            }
        }

        namespace TestApp
        {
            public partial class App : Avalonia.Application { }
        }
        """;

    private const string CatalogBThenASource = """
        namespace TestApp.Localization
        {
            [AtomUI.Localization.LanguageCatalog]
            public enum CatalogBLangResourceKind
            {
                Cancel,
                Confirm
            }

            [AtomUI.Localization.LanguageCatalog]
            public enum CatalogALangResourceKind
            {
                Title,
                Count
            }
        }

        namespace TestApp
        {
            public partial class App : Avalonia.Application { }
        }
        """;

    private const string CatalogAXliff = """
        <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US">
          <file id="TestApp.Localization.CatalogALangResourceKind">
            <unit id="Count"><segment><source>Alpha {0}</source></segment></unit>
            <unit id="Title"><segment><source>Alpha</source></segment></unit>
          </file>
        </xliff>
        """;

    private const string CatalogBXliff = """
        <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US">
          <file id="TestApp.Localization.CatalogBLangResourceKind">
            <unit id="Confirm"><segment><source>Confirm</source></segment></unit>
            <unit id="Cancel"><segment><source>Cancel</source></segment></unit>
          </file>
        </xliff>
        """;
}
