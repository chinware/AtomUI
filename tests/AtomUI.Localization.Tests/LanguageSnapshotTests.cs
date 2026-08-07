using Shouldly;
using Xunit;

namespace AtomUI.Localization.Tests;

public class LanguageSnapshotTests
{
    [Fact]
    public void Build_Resolves_Source_Priority_Then_Fallback_Specificity()
    {
        var catalog = CreateCatalog(isSecondFormatted: true);
        var registry = CreateRegistry(catalog,
            Bundle(catalog, LanguageTags.EnUS, ["English", "English {0}"]),
            Bundle(catalog, LanguageTags.Zh, ["Neutral", "Neutral {0}"]),
            Bundle(catalog, LanguageTags.ZhHans, ["Script", null]),
            Bundle(
                catalog,
                LanguageTags.ZhCN,
                ["Pack exact", "Pack exact {0}"],
                TranslationSourceKind.StaticLanguagePack,
                "Pack"),
            Bundle(
                catalog,
                LanguageTags.ZhHans,
                ["Override parent", null],
                TranslationSourceKind.ApplicationOverride,
                "Application"));

        var snapshot = LanguageSnapshotBuilder.Build(
            registry,
            LanguageTags.ZhCN,
            Definition(LanguageTags.ZhCN));

        snapshot.GetEntry(0, 0).Text.ShouldBe("Override parent");
        snapshot.GetEntry(0, 0).ResolvedLanguage.ShouldBe(LanguageTags.ZhHans);
        snapshot.GetEntry(0, 1).Text.ShouldBe("Pack exact {0}");
        snapshot.GetEntry(0, 1).ResolvedLanguage.ShouldBe(LanguageTags.ZhCN);
        snapshot.GetEntry(0, 1).CompositeFormat.ShouldNotBeNull();
    }

    [Fact]
    public void Build_Resolves_Each_Unit_Independently()
    {
        var catalog = CreateCatalog();
        var fr = LanguageTag.Parse("fr");
        var frCA = LanguageTag.Parse("fr-CA");
        var registry = CreateRegistry(catalog,
            Bundle(catalog, LanguageTags.EnUS, ["English first", "English second"]),
            Bundle(catalog, fr, [null, "Français second"]),
            Bundle(catalog, frCA, ["Français canadien first", null]));

        var snapshot = LanguageSnapshotBuilder.Build(registry, frCA, Definition(frCA));

        snapshot.GetEntry(0, 0).Text.ShouldBe("Français canadien first");
        snapshot.GetEntry(0, 0).ResolvedLanguage.ShouldBe(frCA);
        snapshot.GetEntry(0, 1).Text.ShouldBe("Français second");
        snapshot.GetEntry(0, 1).ResolvedLanguage.ShouldBe(fr);
    }

    [Fact]
    public void Build_Allows_English_To_Use_The_Required_Source()
    {
        var catalog = CreateCatalog();
        var registry = CreateRegistry(catalog,
            Bundle(catalog, LanguageTags.EnUS, ["First", "Second"]));

        var snapshot = LanguageSnapshotBuilder.Build(
            registry,
            LanguageTags.EnUS,
            Definition(LanguageTags.EnUS));

        snapshot.RequestedLanguage.ShouldBe(LanguageTags.EnUS);
        snapshot.GetEntry(0, 0).Text.ShouldBe("First");
        snapshot.GetEntry(0, 0).ResolvedLanguage.ShouldBe(LanguageTags.EnUS);
    }

    [Fact]
    public void Build_Rejects_NonEnglish_Language_That_Only_Reaches_English()
    {
        var catalog = CreateCatalog();
        var registry = CreateRegistry(catalog,
            Bundle(catalog, LanguageTags.EnUS, ["First", "Second"]));

        var exception = Should.Throw<LanguageCoverageException>(() =>
            LanguageSnapshotBuilder.Build(registry, LanguageTags.JaJP, Definition(LanguageTags.JaJP)));

        exception.Message.ShouldContain("ja-JP");
        exception.Message.ShouldContain(catalog.CatalogId);
        exception.Message.ShouldContain("First");
    }

    [Fact]
    public void Build_Rejects_Partial_NonEnglish_Coverage_Per_Unit()
    {
        var catalog = CreateCatalog();
        var registry = CreateRegistry(catalog,
            Bundle(catalog, LanguageTags.EnUS, ["First", "Second"]),
            Bundle(catalog, LanguageTags.JaJP, ["最初", null]));

        var exception = Should.Throw<LanguageCoverageException>(() =>
            LanguageSnapshotBuilder.Build(registry, LanguageTags.JaJP, Definition(LanguageTags.JaJP)));

        exception.Message.ShouldContain("Second");
    }

    [Fact]
    public void Build_Derives_Translation_Coverage_From_Language_Tag_Not_Formatting_Culture()
    {
        var catalog = CreateCatalog();
        var language = LanguageTag.Parse("fr-CA-x-acme");
        var definition = new LanguageDefinition(
            language,
            System.Globalization.CultureInfo.GetCultureInfo("en-US"),
            "Acme French",
            LanguageTextDirection.LeftToRight);
        var registry = CreateRegistry(catalog,
            Bundle(catalog, LanguageTags.EnUS, ["First", "Second"]));

        var exception = Should.Throw<LanguageCoverageException>(() =>
            LanguageSnapshotBuilder.Build(registry, language, definition));

        exception.Message.ShouldContain(language.Value);
        exception.Message.ShouldContain("First");
    }

    [Fact]
    public void Build_Wraps_Invalid_Compiled_Format_With_Catalog_Context()
    {
        var catalog = CreateCatalog(isSecondFormatted: true);
        var registry = CreateRegistry(catalog,
            Bundle(catalog, LanguageTags.EnUS, ["First", "Broken {"]));

        var exception = Should.Throw<LanguageCatalogException>(() =>
            LanguageSnapshotBuilder.Build(registry, LanguageTags.EnUS, Definition(LanguageTags.EnUS)));

        exception.InnerException.ShouldBeOfType<FormatException>();
        exception.Message.ShouldContain(catalog.CatalogId);
        exception.Message.ShouldContain("Second");
        exception.Message.ShouldContain("Acme");
    }

    private static LanguageCatalogDescriptor<ResourceKind> CreateCatalog(bool isSecondFormatted = false)
    {
        return new LanguageCatalogDescriptor<ResourceKind>(
            "Acme:Acme.ResourceKind",
            1,
            [
                new LanguageCatalogUnitDescriptor("First"),
                new LanguageCatalogUnitDescriptor("Second", isSecondFormatted)
            ],
            static key => key switch
            {
                ResourceKind.First => 0,
                ResourceKind.Second => 1,
                _ => -1
            });
    }

    private static LanguageCatalogRegistry CreateRegistry(
        LanguageCatalogDescriptor catalog,
        params TranslationBundleDescriptor[] bundles)
    {
        var builder = new LocalizationBuilder();
        builder.AddCatalog(catalog);
        foreach (var bundle in bundles)
        {
            builder.AddTranslationBundle(bundle);
        }
        return builder.FreezeRegistry();
    }

    private static TranslationBundleDescriptor Bundle(
        LanguageCatalogDescriptor catalog,
        LanguageTag language,
        IReadOnlyList<string?> values,
        TranslationSourceKind sourceKind = TranslationSourceKind.ModuleBuiltIn,
        string sourceIdentity = "Acme")
    {
        return new TranslationBundleDescriptor(
            catalog.CatalogId,
            catalog.ContractVersion,
            language,
            sourceKind,
            sourceIdentity,
            values);
    }

    private static LanguageDefinition Definition(LanguageTag tag)
    {
        StandardLanguageDefinitions.TryCreate(tag, out var definition).ShouldBeTrue();
        return definition!;
    }

    private enum ResourceKind
    {
        First,
        Second
    }
}
