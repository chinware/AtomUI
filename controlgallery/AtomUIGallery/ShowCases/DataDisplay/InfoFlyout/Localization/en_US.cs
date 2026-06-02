using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.InfoFlyout;

[LanguageProvider(LanguageCode.en_US, InfoFlyoutShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The most basic example. The size of the floating layer depends on the contents region.";
    public const string TriggerWaysTitle = "Three ways to trigger";
    public const string TriggerWaysDescription = "Mouse to click, focus and move in.";
    public const string PlacementTitle = "Placement";
    public const string PlacementDescription = "There are 12 placement options available.";
    public const string ArrowTitle = "Arrow";
    public const string ArrowDescription = "Support show, hide or keep arrow in the center.";

    protected override Type GetResourceKindType() => typeof(InfoFlyoutShowCaseLangResourceKind);
}
