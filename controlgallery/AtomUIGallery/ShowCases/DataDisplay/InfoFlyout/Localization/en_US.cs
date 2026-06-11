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

    protected override Type GetResourceKindType() => typeof(InfoFlyoutShowCaseLangResourceKind);
}
