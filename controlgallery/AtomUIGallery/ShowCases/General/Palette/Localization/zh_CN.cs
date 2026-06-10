using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Palette;

[LanguageProvider(LanguageCode.zh_CN, PaletteShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string P2HeaderLight = "浅色";
    public const string P2HeaderDark = "深色";

    protected override Type GetResourceKindType() => typeof(PaletteShowCaseLangResourceKind);
}
