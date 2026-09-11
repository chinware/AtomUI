using System.Globalization;
using Shouldly;
using Xunit;

namespace AtomUI.Localization.Tests;

public class LocalizerTests
{
    [Fact]
    public void Get_Uses_The_Strongly_Typed_Catalog_Slot()
    {
        var runtime = CreateRuntime(
            LanguageTags.EnUS,
            CultureInfo.GetCultureInfo("en-US"),
            "Sign in",
            isFormatted: false);
        var localizer = new Localizer(runtime.Context);

        localizer.Get(ResourceKind.Value).ShouldBe("Sign in");
    }

    [Fact]
    public void Format_Uses_Formatting_Culture_From_The_Same_Revision()
    {
        var culture = new CultureInfo("en-US");
        culture.NumberFormat.NumberGroupSeparator = "_";
        culture.NumberFormat.NumberDecimalSeparator = ",";
        var runtime = CreateRuntime(
            LanguageTags.EnUS,
            culture,
            "Total: {0:N2}",
            isFormatted: true);
        var localizer = new Localizer(runtime.Context);

        localizer.Format(ResourceKind.Value, 1234.5).ShouldBe("Total: 1_234,50");
    }

    [Fact]
    public void Get_Rejects_Unregistered_Enum_Type()
    {
        var runtime = CreateRuntime(
            LanguageTags.EnUS,
            CultureInfo.GetCultureInfo("en-US"),
            "Value",
            isFormatted: false);
        var localizer = new Localizer(runtime.Context);

        var exception = Should.Throw<LanguageCatalogException>(() => localizer.Get(OtherResourceKind.Value));

        exception.Message.ShouldContain(typeof(OtherResourceKind).FullName!);
    }

    [Fact]
    public void Get_Rejects_Unmapped_Enum_Value()
    {
        var runtime = CreateRuntime(
            LanguageTags.EnUS,
            CultureInfo.GetCultureInfo("en-US"),
            "Value",
            isFormatted: false);
        var localizer = new Localizer(runtime.Context);

        var exception = Should.Throw<LanguageCatalogException>(() =>
            localizer.Get((ResourceKind)999));

        exception.Message.ShouldContain("Acme:Acme.ResourceKind");
        exception.Message.ShouldContain("not mapped");
    }

    [Fact]
    public void Format_Preserves_FormatException_With_Catalog_Context()
    {
        var runtime = CreateRuntime(
            LanguageTags.EnUS,
            CultureInfo.GetCultureInfo("en-US"),
            "Value: {0}",
            isFormatted: true);
        var localizer = new Localizer(runtime.Context);

        var exception = Should.Throw<LanguageCatalogException>(() =>
            localizer.Format(ResourceKind.Value));

        exception.InnerException.ShouldBeOfType<FormatException>();
        exception.Message.ShouldContain("Acme:Acme.ResourceKind");
        exception.Message.ShouldContain("Value");
        exception.Message.ShouldContain("en-US");
    }

    [Fact]
    public async Task Format_Observes_Only_Whole_Revisions_During_Concurrent_Publish()
    {
        var englishCulture = new CultureInfo("en-US");
        englishCulture.NumberFormat.NumberDecimalSeparator = ".";
        var chineseCulture = new CultureInfo("zh-CN");
        chineseCulture.NumberFormat.NumberDecimalSeparator = ",";
        var english = CreateRuntime(
            LanguageTags.EnUS,
            englishCulture,
            "EN {0:F1}",
            isFormatted: true);
        var chinese = CreateRevision(
            english.Registry,
            LanguageTags.ZhCN,
            chineseCulture,
            "ZH {0:F1}",
            revision: 1);
        var localizer = new Localizer(english.Context);
        var englishRevision = english.Context.Current;

        var publishTask = Task.Run(() =>
        {
            for (var index = 0; index < 10_000; index++)
            {
                english.Context.Publish(index % 2 == 0 ? chinese : englishRevision);
            }
        }, TestContext.Current.CancellationToken);

        for (var index = 0; index < 10_000; index++)
        {
            var value = localizer.Format(ResourceKind.Value, 1.5);
            value.ShouldBeOneOf("EN 1.5", "ZH 1,5");
        }

        await publishTask;
    }

    private static TestRuntime CreateRuntime(
        LanguageTag language,
        CultureInfo culture,
        string text,
        bool isFormatted)
    {
        var catalog = CreateCatalog(isFormatted);
        var registry = CreateRegistry(catalog, language, text);
        var revision = CreateRevision(registry, language, culture, text, revision: 0);
        return new TestRuntime(registry, new LanguageContext(registry, revision));
    }

    private static LanguageRevision CreateRevision(
        LanguageCatalogRegistry registry,
        LanguageTag language,
        CultureInfo culture,
        string text,
        long revision)
    {
        var catalog = registry.Catalogs[0];
        var builder = new LocalizationBuilder();
        builder.AddCatalog(catalog);
        builder.AddTranslationBundle(new TranslationBundleDescriptor(
            catalog.CatalogId,
            LanguageTags.EnUS,
            TranslationSourceKind.ModuleBuiltIn,
            "Acme",
            [language == LanguageTags.EnUS ? text : "English {0:F1}"]));
        if (language != LanguageTags.EnUS)
        {
            builder.AddTranslationBundle(new TranslationBundleDescriptor(
                catalog.CatalogId,
                language,
                TranslationSourceKind.ModuleBuiltIn,
                "Acme",
                [text]));
        }
        var snapshotRegistry = builder.FreezeRegistry();
        var definition = new LanguageDefinition(
            language,
            culture,
            culture.NativeName,
            LanguageTextDirection.LeftToRight);
        var snapshot = LanguageSnapshotBuilder.Build(snapshotRegistry, language, definition);
        var state = new LanguageState(language, culture, LanguageTextDirection.LeftToRight, revision);
        return new LanguageRevision(snapshot, state);
    }

    private static LanguageCatalogRegistry CreateRegistry(
        LanguageCatalogDescriptor catalog,
        LanguageTag language,
        string text)
    {
        var builder = new LocalizationBuilder();
        builder.AddCatalog(catalog);
        builder.AddTranslationBundle(new TranslationBundleDescriptor(
            catalog.CatalogId,
            LanguageTags.EnUS,
            TranslationSourceKind.ModuleBuiltIn,
            "Acme",
            [language == LanguageTags.EnUS ? text : "English"]));
        if (language != LanguageTags.EnUS)
        {
            builder.AddTranslationBundle(new TranslationBundleDescriptor(
                catalog.CatalogId,
                language,
                TranslationSourceKind.ModuleBuiltIn,
                "Acme",
                [text]));
        }
        return builder.FreezeRegistry();
    }

    private static LanguageCatalogDescriptor<ResourceKind> CreateCatalog(bool isFormatted)
    {
        return new LanguageCatalogDescriptor<ResourceKind>(
            "Acme:Acme.ResourceKind",
            [new LanguageCatalogUnitDescriptor("Value", isFormatted)],
            static key => key == ResourceKind.Value ? 0 : -1);
    }

    private sealed record TestRuntime(
        LanguageCatalogRegistry Registry,
        LanguageContext Context);

    private enum ResourceKind
    {
        Value
    }

    private enum OtherResourceKind
    {
        Value
    }
}
