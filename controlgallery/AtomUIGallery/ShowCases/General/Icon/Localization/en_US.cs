using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Icon;

[LanguageProvider(LanguageCode.en_US, IconShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string P2HeaderOutlined = "Outlined";
    public const string P2HeaderFilled = "Filled";
    public const string P2HeaderTwoTone = "Two Tone";

    protected override Type GetResourceKindType() => typeof(IconShowCaseLangResourceKind);
}
