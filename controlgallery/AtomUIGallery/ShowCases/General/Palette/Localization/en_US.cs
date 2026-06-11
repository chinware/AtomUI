using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Palette;

[LanguageProvider(LanguageCode.en_US, PaletteShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string P2HeaderLight = "Light";
    public const string P2HeaderDark = "Dark";

    protected override Type GetResourceKindType() => typeof(PaletteShowCaseLangResourceKind);
}
