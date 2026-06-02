using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Statistic;

[LanguageProvider(LanguageCode.zh_CN, StatisticShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最简单的用法。";
    public const string UnitTitle = "单位";
    public const string UnitDescription = "通过 prefix 和 suffix 添加单位。";
    public const string InCardTitle = "在卡片中展示";
    public const string InCardDescription = "在 Card 中展示统计数据。";
    public const string AnimatedNumberTitle = "动画数字";
    public const string AnimatedNumberDescription = "使用 StatisticCountUp 展示动画数字。";
    public const string TimerTitle = "计时器";
    public const string TimerDescription = "计时器组件。";

    protected override Type GetResourceKindType() => typeof(StatisticShowCaseLangResourceKind);
}
