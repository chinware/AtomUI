using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Tooltip;

[LanguageProvider(LanguageCode.en_US, TooltipShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The simplest usage.";
    public const string PlacementTitle = "Placement";
    public const string PlacementDescription = "There are 12 placement options available.";
    public const string ArrowTitle = "Arrow";
    public const string ArrowDescription = "Support show, hide or keep arrow in the center.";
    public const string ColorfulTooltipTitle = "Colorful Tooltip";
    public const string ColorfulTooltipDescription = "We preset a series of colorful Tooltip styles for use in different situations.";
    public const string P2TextTooltipWillShowOnMouseEnter = "Tooltip will show on mouse enter.";
    public const string P2ContentShow = "Show";
    public const string P2ContentHide = "Hide";
    public const string P2ContentCenter = "Center";
    public const string P2TextPresets = "Presets";
    public const string P2TextCustom = "Custom";

    public const string P2ToolTipTipPromptText = "prompt text";

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

    public const string P2ContentBlue = "Blue";

    public const string P2ContentRed = "Red";

    public const string P2ContentVolcano = "Volcano";

    public const string P2ContentOrange = "Orange";

    public const string P2ContentGold = "Gold";

    public const string P2ContentYellow = "Yellow";

    public const string P2ContentLime = "Lime";

    public const string P2ContentGreen = "Green";

    public const string P2ContentCyan = "Cyan";

    public const string P2ContentGeekBlue = "GeekBlue";

    public const string P2ContentPurple = "Purple";

    public const string P2ContentPink = "Pink";

    public const string P2ContentMagenta = "Magenta";

    public const string P2ContentGrey = "Grey";

    protected override Type GetResourceKindType() => typeof(TooltipShowCaseLangResourceKind);
}
