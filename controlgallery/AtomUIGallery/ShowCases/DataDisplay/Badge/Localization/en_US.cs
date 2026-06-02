using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Badge;

[LanguageProvider(LanguageCode.en_US, BadgeShowCase.LanguageId)]
internal class en_US : LanguageProvider
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

    protected override Type GetResourceKindType() => typeof(BadgeShowCaseLangResourceKind);
}
