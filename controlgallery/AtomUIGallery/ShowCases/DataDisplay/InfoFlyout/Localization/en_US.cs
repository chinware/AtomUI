using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.InfoFlyout;

[LanguageProvider(LanguageCode.en_US, InfoFlyoutShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The most basic example. The size of the floating layer depends on the contents region.";
    public const string TriggerWaysTitle = "Three ways to trigger";
    public const string TriggerWaysDescription = "Mouse to click, focus and move in.";
    public const string PlacementTitle = "Placement";
    public const string PlacementDescription = "There are 12 placement options available.";
    public const string ArrowTitle = "Arrow";
    public const string ArrowDescription = "Support show, hide or keep arrow in the center.";
    public const string P2TextTheMostBasicExample = "The most basic example.";
    public const string P2ContentHoverMe = "Hover me";
    public const string P2ContentFocusMe = "Focus me";
    public const string P2ContentClickMe = "Click me";
    public const string P2ContentShow = "Show";
    public const string P2ContentHide = "Hide";
    public const string P2ContentCenter = "Center";

    public const string P2ContentLT = "LT";

    public const string P2ContentLeft = "Left";

    public const string P2ContentLB = "LB";

    public const string P2ContentTL = "TL";

    public const string P2ContentTop = "Top";

    public const string P2ContentTR = "TR";

    public const string P2ContentRT = "RT";

    public const string P2ContentRight = "Right";

    public const string P2ContentRB = "RB";

    public const string P2ContentBL = "BL";

    public const string P2ContentBottom = "Bottom";

    public const string P2ContentBR = "BR";
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string PageSubtitle = "Display rich contextual content in a floating layer.";
    public const string PageDescription =
        "InfoFlyout anchors a custom floating panel to a target control, with configurable trigger modes, placement and arrow behavior.";
    public const string ComponentCategory = "Data Display";
    public const string ComponentStatusStable = "Stable";
    public const string InfoNamespaceLabel = "Namespace";
    public const string InfoPackageLabel = "Package";
    public const string InfoBaseClassLabel = "Base class";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyFlyout = "Flyout instance displayed by the host.";
    public const string ApiPropertyTrigger = "Interaction that opens the flyout.";
    public const string ApiPropertyPlacement = "Preferred placement relative to the target control.";
    public const string ApiPropertyIsArrowVisible = "Shows or hides the flyout arrow.";
    public const string ApiPropertyIsPointAtCenter = "Points the arrow at the center of the target control.";
    public const string ApiPropertyShouldUseOverlayPopup = "Uses an overlay popup host when the flyout is opened.";
    public const string ApiPropertyMarginToAnchor = "Distance between the flyout surface and its anchor.";
    public const string ApiPropertyMouseEnterDelay = "Delay in milliseconds before hover opens the flyout.";
    public const string ApiPropertyMouseLeaveDelay = "Delay in milliseconds before hover closes the flyout.";
    public const string ApiPropertyContent = "Content displayed inside the flyout surface.";
    public const string ApiPropertyIsLightDismissEnabled = "Allows pointer or focus outside the flyout to close it.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNameMarginToAnchor = "Default distance between flyout and anchor.";
    public const string TokenNameOverlayHostShadow = "Shadow used when the flyout is hosted by the overlay layer.";
    public const string TokenNamePopupRootShadow = "Shadow used by the popup root surface.";
    public const string TokenNameHorizontalOffset = "Default horizontal offset for flyout positioning.";
    public const string TokenNameVerticalOffset = "Default vertical offset for flyout positioning.";
    public const string TokenScopeComponent = "FlyoutHost";
    public const string TokenStatusStable = "Stable";

    protected override Type GetResourceKindType() => typeof(InfoFlyoutShowCaseLangResourceKind);
}
