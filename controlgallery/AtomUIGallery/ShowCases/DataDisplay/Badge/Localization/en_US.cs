using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Badge;

[LanguageProvider(LanguageCode.en_US, BadgeShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Simplest Usage. Badge will be hidden when count is 0, but we can use showZero to show it.";
    public const string OverflowCountTitle = "Overflow Count";
    public const string OverflowCountDescription = "${overflowCount}+ is displayed when count is larger than overflowCount. The default value of overflowCount is 99.";
    public const string OffsetTitle = "Offset";
    public const string OffsetDescription = "Set offset of the badge dot, the format is [left, top], which represents the offset of the status dot from the left and top of the default position.";
    public const string SizeTitle = "Size";
    public const string SizeDescription = "Set size of numeral Badge.";
    public const string StandaloneTitle = "Standalone";
    public const string StandaloneDescription = "Used in standalone when children is empty.";
    public const string DynamicTitle = "Dynamic";
    public const string DynamicDescription = "The count will be animated as it changes.";
    public const string RedBadgeTitle = "Red badge";
    public const string RedBadgeDescription = "This will simply display a red badge, without a specific count. If count equals 0, it won't display the dot.";
    public const string StatusTitle = "Status";
    public const string StatusDescription = "Standalone badge with status.";
    public const string RibbonTitle = "Ribbon";
    public const string RibbonDescription = "Use ribbon badge.";
    public const string ColorfulBadgeTitle = "Colorful Badge";
    public const string ColorfulBadgeDescription = "We preset a series of colorful Badge styles for use in different situations. You can also set it to a hex color string for custom color.";
    public const string P2TextSuccess = "Success";
    public const string P2TextError = "Error";
    public const string P2TextDefault = "Default";
    public const string P2TextProcessing = "Processing";
    public const string P2TextWarning = "Warning";
    public const string P2TextPolishExperience = "Polish every detail for an excellent UI SDK experience";
    public const string P2TextJiachenPlan = "Jiachen Plan takes off";
    public const string P2TextAvaloniaExcellent = "Avalonia is excellent";
    public const string P2TextHippies = "Hippies";
    public const string P2TitlePresets = "Presets";
    public const string P2TextPink = "Pink";
    public const string P2TextRed = "Red";
    public const string P2TextYellow = "Yellow";
    public const string P2TextOrange = "Orange";
    public const string P2TextCyan = "Cyan";
    public const string P2TextGreen = "Green";
    public const string P2TextBlue = "Blue";
    public const string P2TextPurple = "Purple";
    public const string P2TextGeekblue = "GeekBlue";
    public const string P2TextMagenta = "Magenta";
    public const string P2TextVolcano = "Volcano";
    public const string P2TextGold = "Gold";
    public const string P2TextLime = "Lime";
    public const string P2TitleCustom = "Custom";
    public const string P2TextRgbN45N183N245 = "rgb(45, 183, 245)";
    public const string P2TextHslN102N53N61 = "hsl(102, 53%, 61%)";
    public const string P2TextRgbN15N141N230 = "rgb(15, 141, 230)";
    public const string P2ContentAdd = "Add";
    public const string P2ContentSub = "Sub";
    public const string P2ContentRandom = "Random";
    public const string P2TextPushesOpenTheWindow = "Pushes open the window";
    public const string P2TextAndRaisesTheSpyglass = "and raises the spyglass.";

    public const string P2ContentLinkSomething = "Link something";

    protected override Type GetResourceKindType() => typeof(BadgeShowCaseLangResourceKind);
}
