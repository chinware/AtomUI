using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ToggleSwitch;

[LanguageProvider(LanguageCode.en_US, ToggleSwitchShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The most basic usage.";
    public const string DisabledTitle = "Disabled";
    public const string DisabledDescription = "Disabled state of Switch.";
    public const string TextAndIconTitle = "Text and icon";
    public const string TextAndIconDescription = "With text and icon.";
    public const string TwoSizesTitle = "Two sizes";
    public const string TwoSizesDescription = "size=Small represents a small sized switch.";
    public const string LoadingTitle = "Loading";
    public const string LoadingDescription = "Mark a pending state of switch.";
    public const string P2ContentToggleDisabled = "toggle disabled";
    public const string P2ContentToggleLoading = "toggle loading";

    public const string P2OnContentOn = "On";

    public const string P2OffContentOff = "Off";

    public const string P2OnContentText = "Open";

    public const string P2OffContentText = "Close";

    protected override Type GetResourceKindType() => typeof(ToggleSwitchShowCaseLangResourceKind);
}
