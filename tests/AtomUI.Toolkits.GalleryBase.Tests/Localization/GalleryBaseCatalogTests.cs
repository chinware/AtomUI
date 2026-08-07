using System.Reflection;
using AtomUI.Localization;
using AtomUI.Toolkits.GalleryBase.Localization;
using Avalonia;
using Shouldly;
using Xunit;

namespace AtomUI.Toolkits.GalleryBase.Tests.Localization;

public class GalleryBaseCatalogTests
{
    static GalleryBaseCatalogTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void UseGalleryBase_Registers_GalleryShowCaseHeader_Catalog()
    {
        var expected = new[]
        {
            new ExpectedEntry(GalleryShowCaseHeaderLangResourceKind.NamespaceLabel, "Namespace", "命名空间", "命名空間"),
            new ExpectedEntry(GalleryShowCaseHeaderLangResourceKind.PackageLabel, "Package", "包", "套件"),
            new ExpectedEntry(GalleryShowCaseHeaderLangResourceKind.BaseClassLabel, "Base class", "基类", "基底類別")
        };
        var resourceKindType = typeof(GalleryShowCaseHeaderLangResourceKind);

        resourceKindType.GetCustomAttribute<LanguageCatalogAttribute>()
                        .ShouldNotBeNull()
                        .ContractVersion.ShouldBe(2);
        Enum.GetValues<GalleryShowCaseHeaderLangResourceKind>()
            .Select(static entry => entry.ToString())
            .ShouldBe(expected.Select(static entry => entry.Kind.ToString()));

        typeof(GalleryShowCaseHeaderLangResourceExtension).IsSealed.ShouldBeTrue();
        typeof(GalleryShowCaseHeaderLangResourceExtension).BaseType.ShouldBe(
            typeof(LanguageResourceExtension<GalleryShowCaseHeaderLangResourceKind>));

        var application = Application.Current.ShouldNotBeNull();
        var languageManager = application.GetLanguageManager().ShouldNotBeNull();
        var localizer = application.GetLocalizer().ShouldNotBeNull();
        AssertLanguage(LanguageTags.EnUS, expected.Select(static entry => (entry.Kind, entry.En)), languageManager, localizer);
        AssertLanguage(LanguageTags.ZhCN, expected.Select(static entry => (entry.Kind, entry.ZhCn)), languageManager, localizer);
        AssertLanguage(LanguageTags.ZhTW, expected.Select(static entry => (entry.Kind, entry.ZhTw)), languageManager, localizer);

        resourceKindType.Assembly.GetType("AtomUI.Toolkits.GalleryBase.Localization.en_US").ShouldBeNull();
        resourceKindType.Assembly.GetType("AtomUI.Toolkits.GalleryBase.Localization.zh_CN").ShouldBeNull();
        resourceKindType.Assembly.GetType("AtomUI.Toolkits.GalleryBase.Localization.zh_TW").ShouldBeNull();
    }

    [Fact]
    public void GalleryLocalizedText_Resolves_From_Current_Localizer()
    {
        var application = Application.Current.ShouldNotBeNull();
        var languageManager = application.GetLanguageManager().ShouldNotBeNull();
        try
        {
            languageManager.ChangeLanguage(LanguageTags.ZhCN);
            var localizedText = new GalleryLocalizedText<GalleryShowCaseHeaderLangResourceKind>(
                GalleryShowCaseHeaderLangResourceKind.NamespaceLabel,
                "fallback");

            localizedText.Resolve().ShouldBe("命名空间");
        }
        finally
        {
            languageManager.ChangeLanguage(LanguageTags.EnUS);
        }
    }

    private static void AssertLanguage(
        LanguageTag language,
        IEnumerable<(GalleryShowCaseHeaderLangResourceKind Kind, string Text)> expected,
        ILanguageManager languageManager,
        ILocalizer localizer)
    {
        languageManager.ChangeLanguage(language);
        foreach (var (kind, text) in expected)
        {
            localizer.Get(kind).ShouldBe(text);
        }
    }

    private readonly record struct ExpectedEntry(
        GalleryShowCaseHeaderLangResourceKind Kind,
        string En,
        string ZhCn,
        string ZhTw);
}
