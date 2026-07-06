using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Rate;

[LanguageProvider(LanguageCode.en_US, RateShowCase.LanguageId)]
internal partial class en_US
{
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string ComponentCategory = "Data Entry";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "Collect lightweight ratings with star, half-star, custom character, and tooltip support.";
    public const string PageDescription = "Rate lets users express preference or quality on an ordered scale. It supports clearable values, half selection, read-only display, keyboard interaction, custom glyphs, and localized copywriting.";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyIsAllowClear = "Allows clicking the selected value again to clear the rating.";
    public const string ApiPropertyIsAllowHalf = "Allows selecting half-step rating values.";
    public const string ApiPropertyCharacter = "Custom visual content used for each rating item.";
    public const string ApiPropertyStarColor = "Overrides the filled rating item color.";
    public const string ApiPropertyStarBgColor = "Overrides the unselected rating item color.";
    public const string ApiPropertyCount = "Total number of rating items.";
    public const string ApiPropertyValue = "Current selected rating value. It uses TwoWay binding by default and supports Avalonia data validation.";
    public const string ApiPropertyDefaultValue = "Initial rating value used when Value is not explicitly set.";
    public const string ApiPropertyIsKeyboardEnabled = "Controls whether keyboard interaction is enabled.";
    public const string ApiPropertyToolTips = "Tooltip text list displayed for each rating value.";
    public const string ApiPropertySizeType = "Controls small, middle, or large rating size.";
    public const string ApiPropertyIsMotionEnabled = "Enables or disables rating motion effects.";
    public const string ApiPropertyValueChanged = "Raised when the selected rating value changes.";
    public const string ApiPropertyHoverValueChanged = "Raised when the hovered rating value changes.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNameStarColor = "Color of selected rating items.";
    public const string TokenNameStarSize = "Default rating item size.";
    public const string TokenNameStarSizeSM = "Small rating item size.";
    public const string TokenNameStarSizeLG = "Large rating item size.";
    public const string TokenNameStarHoverScale = "Scale applied to a rating item while hovering.";
    public const string TokenNameStarBg = "Background color of unselected rating items.";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The simplest usage.";
    public const string TwoWayBindingTitle = "Two-way binding";
    public const string TwoWayBindingDescription = "Value uses TwoWay binding by default, so user selection and ViewModel updates stay synchronized.";
    public const string HalfStarTitle = "Half star";
    public const string HalfStarDescription = "Support select half star.";
    public const string ShowCopywritingTitle = "Show copywriting";
    public const string ShowCopywritingDescription = "Add copywriting in rate components.";
    public const string ReadOnlyTitle = "Read only";
    public const string ReadOnlyDescription = "Read only, can't use mouse to interact.";
    public const string ClearStarTitle = "Clear star";
    public const string ClearStarDescription = "Support set allow to clear star when click again.";
    public const string OtherCharacterTitle = "Other Character";
    public const string OtherCharacterDescription = "Replace the default star to other character like alphabet, digit, iconfont or even Chinese word.";
    public const string P2TextIsallowclearTrue = "IsAllowClear: true";
    public const string P2TextIsallowclearFalse = "IsAllowClear: false";
    public const string P2ContentSetFourStars = "Set 4 stars";
    public const string P2ContentClear = "Clear";
    public const string P2TwoWayValueSummaryFormat = "Selected value: {0:0.#}";
    public const string P2TooltipTerrible = "terrible";
    public const string P2TooltipBad = "bad";
    public const string P2TooltipNormal = "normal";
    public const string P2TooltipGood = "good";
    public const string P2TooltipWonderful = "wonderful";

}
