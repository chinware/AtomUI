using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Badge;

[LanguageProvider(LanguageCode.zh_CN, BadgeShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最简单的用法。count 为 0 时 Badge 会隐藏，但可以使用 showZero 显示。";
    public const string OverflowCountTitle = "封顶数字";
    public const string OverflowCountDescription = "当 count 大于 overflowCount 时显示 ${overflowCount}+。overflowCount 的默认值是 99。";
    public const string OffsetTitle = "偏移量";
    public const string OffsetDescription = "设置徽标点的偏移量，格式为 [left, top]，表示状态点相对于默认位置左侧和顶部的偏移。";
    public const string SizeTitle = "尺寸";
    public const string SizeDescription = "设置数字 Badge 的尺寸。";
    public const string StandaloneTitle = "独立使用";
    public const string StandaloneDescription = "children 为空时可独立使用。";
    public const string DynamicTitle = "动态变化";
    public const string DynamicDescription = "数字变化时会带有动画。";
    public const string RedBadgeTitle = "红点徽标";
    public const string RedBadgeDescription = "简单展示一个红色徽标，不显示具体数字。如果 count 等于 0，则不会显示圆点。";
    public const string StatusTitle = "状态";
    public const string StatusDescription = "带状态的独立徽标。";
    public const string RibbonTitle = "缎带徽标";
    public const string RibbonDescription = "使用缎带样式徽标。";
    public const string ColorfulBadgeTitle = "多彩徽标";
    public const string ColorfulBadgeDescription = "预设了一系列多彩 Badge 样式，可用于不同场景。也可以设置十六进制颜色字符串来自定义颜色。";

    protected override Type GetResourceKindType() => typeof(BadgeShowCaseLangResourceKind);
}
