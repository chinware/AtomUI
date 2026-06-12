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
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string PageSubtitle = "Display contextual hints when users hover or focus an element.";
    public const string PageDescription =
        "Tooltip provides concise helper text, placement control, arrow behavior and preset or custom colors for lightweight contextual guidance.";
    public const string ComponentCategory = "Data Display";
    public const string ComponentStatusStable = "Stable";
    public const string InfoNamespaceLabel = "Namespace";
    public const string InfoPackageLabel = "Package";
    public const string InfoBaseClassLabel = "Base class";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyTip = "Content displayed inside the tooltip.";
    public const string ApiPropertyPlacement = "Preferred popup placement relative to the target control.";
    public const string ApiPropertyIsArrowVisible = "Shows or hides the tooltip arrow when the placement supports it.";
    public const string ApiPropertyIsPointAtCenter = "Points the arrow at the center of the target control.";
    public const string ApiPropertyPresetColor = "Preset color used by the tooltip background.";
    public const string ApiPropertyColor = "Custom background color used by the tooltip.";
    public const string ApiPropertyShowDelay = "Delay in milliseconds before showing the tooltip.";
    public const string ApiPropertyShowOnDisabled = "Allows tooltip display for disabled target controls.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNameToolTipBackground = "Default tooltip background color.";
    public const string TokenNameToolTipColor = "Default tooltip foreground color.";
    public const string TokenNameToolTipMaxWidth = "Maximum width before tooltip content wraps.";
    public const string TokenNameBorderRadiusOuter = "Outer corner radius used by the tooltip surface.";
    public const string TokenNamePadding = "Padding around tooltip content.";
    public const string TokenNameMotionDuration = "Motion duration used by tooltip open and close animations.";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";

    protected override Type GetResourceKindType() => typeof(TooltipShowCaseLangResourceKind);
}
