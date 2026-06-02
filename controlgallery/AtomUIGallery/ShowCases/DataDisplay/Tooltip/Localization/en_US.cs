using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Tooltip;

[LanguageProvider(LanguageCode.en_US, TooltipShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The simplest usage.";
    public const string PlacementTitle = "Placement";
    public const string PlacementDescription = "There are 12 placement options available.";
    public const string ArrowTitle = "Arrow";
    public const string ArrowDescription = "Support show, hide or keep arrow in the center.";
    public const string ColorfulTooltipTitle = "Colorful Tooltip";
    public const string ColorfulTooltipDescription = "We preset a series of colorful Tooltip styles for use in different situations.";

    protected override Type GetResourceKindType() => typeof(TooltipShowCaseLangResourceKind);
}
