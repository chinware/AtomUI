using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Statistic;

[LanguageProvider(LanguageCode.zh_CN, StatisticShowCase.LanguageId)]
internal partial class zh_CN
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
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计令牌";
    public const string PageSubtitle = "用清晰的视觉层级展示数字事实、指标和倒计时。";
    public const string PageDescription =
        "Statistic 用于展示重要数值，支持单位、图标、加载状态、动画数字，以及基于时间的倒计时或正计时。";
    public const string ComponentCategory = "数据展示";
    public const string ComponentStatusStable = "稳定";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyHeader = "显示在统计值上方的标题或标签。";
    public const string ApiPropertyValue = "Statistic 格式化前使用的原始值。";
    public const string ApiPropertyFormatter = "用于渲染 Statistic 数值的自定义格式化函数。";
    public const string ApiPropertyDecimalSeparator = "数值格式化时使用的小数分隔符。";
    public const string ApiPropertyGroupSeparator = "数值格式化时使用的分组分隔符。";
    public const string ApiPropertyPrecision = "数值格式化时保留的小数位数。";
    public const string ApiPropertyIsLoading = "使用骨架屏占位替代数值内容。";
    public const string ApiPropertyValuePrefixAddOn = "显示在数值前面的可选内容。";
    public const string ApiPropertyValueSuffixAddOn = "显示在数值后面的可选内容。";
    public const string ApiPropertyContentForeground = "统计值和附加内容使用的画刷。";
    public const string ApiPropertyContentFontSize = "统计值和附加内容使用的字号。";
    public const string ApiPropertyTimerValue = "TimerStatistic 用于倒计时或正计时的目标时间。";
    public const string ApiPropertyFormat = "TimerStatistic 输出使用的 TimeSpan 格式字符串。";
    public const string ApiPropertyRefreshDuration = "计时器刷新间隔。";
    public const string ApiEventCountdownFinished = "TimerStatistic 倒计时到零时触发。";
    public const string ApiPropertyEndValue = "StatisticCountUp 动画到达的目标数字。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "状态";
    public const string TokenNameTitleFontSize = "统计标题使用的字号。";
    public const string TokenNameContentFontSize = "统计值内容使用的字号。";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";

}
