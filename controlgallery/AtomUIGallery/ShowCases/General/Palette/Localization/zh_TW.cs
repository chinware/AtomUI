using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Palette;

[LanguageProvider(LanguageCode.zh_TW, PaletteShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string P2HeaderLight = "淺色";
    public const string P2HeaderDark = "深色";

    protected override Type GetResourceKindType() => typeof(PaletteShowCaseLangResourceKind);
}

