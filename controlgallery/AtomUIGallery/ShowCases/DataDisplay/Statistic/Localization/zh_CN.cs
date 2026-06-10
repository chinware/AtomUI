using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Statistic;

[LanguageProvider(LanguageCode.zh_CN, StatisticShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
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
    public const string P2HeaderActiveUsers = "活跃用户";
    public const string P2HeaderAccountBalanceCny = "账户余额（CNY）";
    public const string P2HeaderFeedback = "反馈";
    public const string P2HeaderUnmerged = "未合并";
    public const string P2HeaderActive = "启用";
    public const string P2HeaderIdle = "空闲";
    public const string P2HeaderMillionSeconds = "毫秒";
    public const string P2HeaderCountdown = "倒计时";
    public const string P2HeaderCountup = "正计时";
    public const string P2HeaderDayLevelCountdown = "天级倒计时";
    public const string P2HeaderDayLevelCountup = "天级正计时";
    public const string P2ContentRecharge = "充值";
    public const string P2DayLevelFormat = "d\\ \\天\\ h\\ \\时\\ m\\ \\分\\ s\\ \\秒";

    protected override Type GetResourceKindType() => typeof(StatisticShowCaseLangResourceKind);
}
