using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Tour;

[LanguageProvider(LanguageCode.en_US, TourShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The most basic usage.";
    public const string NonModalTitle = "Non-modal";
    public const string NonModalDescription = "Use mask={false} to make Tour non-modal. At the meantime it is recommended to use with type=primary to emphasize the guide itself.";
    public const string PlacementTitle = "Placement";
    public const string PlacementDescription = "Change the placement of the guide relative to the target, there are 12 placements available. When target={null} the guide will show in the center.";
    public const string CustomIndicatorTitle = "Custom indicator";
    public const string CustomIndicatorDescription = "Custom indicator.";
    public const string CustomActionTitle = "Custom action";
    public const string CustomActionDescription = "Custom action.";
    public const string CustomHighlightedAreaStyleTitle = "Custom highlighted area style";
    public const string CustomHighlightedAreaStyleDescription = "Using gap to control the radius of highlight area and the offset between highlight area and the element.";

    protected override Type GetResourceKindType() => typeof(TourShowCaseLangResourceKind);
}
