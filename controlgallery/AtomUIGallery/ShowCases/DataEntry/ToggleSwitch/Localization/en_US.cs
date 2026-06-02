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

    protected override Type GetResourceKindType() => typeof(ToggleSwitchShowCaseLangResourceKind);
}
