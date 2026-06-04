using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Statistic;

[LanguageProvider(LanguageCode.zh_TW, StatisticShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最簡單的用法。";
    public const string UnitTitle = "單位";
    public const string UnitDescription = "通過 prefix 和 suffix 添加單位。";
    public const string InCardTitle = "在卡片中展示";
    public const string InCardDescription = "在 Card 中展示統計數據。";
    public const string AnimatedNumberTitle = "動畫數字";
    public const string AnimatedNumberDescription = "使用 StatisticCountUp 展示動畫數字。";
    public const string TimerTitle = "計時器";
    public const string TimerDescription = "計時器組件。";
    public const string P2HeaderActiveUsers = "活躍用戶";
    public const string P2HeaderAccountBalanceCny = "賬戶餘額（CNY）";
    public const string P2HeaderFeedback = "反饋";
    public const string P2HeaderUnmerged = "未合併";
    public const string P2HeaderActive = "啓用";
    public const string P2HeaderIdle = "空閒";
    public const string P2HeaderMillionSeconds = "毫秒";
    public const string P2HeaderCountdown = "倒計時";
    public const string P2HeaderCountup = "正計時";
    public const string P2HeaderDayLevelCountdown = "天級倒計時";
    public const string P2HeaderDayLevelCountup = "天級正計時";
    public const string P2ContentRecharge = "充值";
    public const string P2DayLevelFormat = "d\\ \\天\\ h\\ \\時\\ m\\ \\分\\ s\\ \\秒";

    protected override Type GetResourceKindType() => typeof(StatisticShowCaseLangResourceKind);
}

