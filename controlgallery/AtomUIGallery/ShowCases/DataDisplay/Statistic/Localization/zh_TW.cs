using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Statistic;

[LanguageProvider(LanguageCode.zh_TW, StatisticShowCase.LanguageId)]
internal partial class zh_TW
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
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計令牌";
    public const string PageSubtitle = "用清晰的視覺層級展示數字事實、指標和倒計時。";
    public const string PageDescription =
        "Statistic 用於展示重要數值，支持單位、圖標、加載狀態、動畫數字，以及基於時間的倒計時或正計時。";
    public const string ComponentCategory = "數據展示";
    public const string ComponentStatusStable = "穩定";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertyHeader = "顯示在統計值上方的標題或標籤。";
    public const string ApiPropertyValue = "Statistic 格式化前使用的原始值。";
    public const string ApiPropertyFormatter = "用於渲染 Statistic 數值的自定義格式化函數。";
    public const string ApiPropertyDecimalSeparator = "數值格式化時使用的小數分隔符。";
    public const string ApiPropertyGroupSeparator = "數值格式化時使用的分組分隔符。";
    public const string ApiPropertyPrecision = "數值格式化時保留的小數位數。";
    public const string ApiPropertyIsLoading = "使用骨架屏佔位替代數值內容。";
    public const string ApiPropertyValuePrefixAddOn = "顯示在數值前面的可選內容。";
    public const string ApiPropertyValueSuffixAddOn = "顯示在數值後面的可選內容。";
    public const string ApiPropertyContentForeground = "統計值和附加內容使用的畫刷。";
    public const string ApiPropertyContentFontSize = "統計值和附加內容使用的字號。";
    public const string ApiPropertyTimerValue = "TimerStatistic 用於倒計時或正計時的目標時間。";
    public const string ApiPropertyFormat = "TimerStatistic 輸出使用的 TimeSpan 格式字符串。";
    public const string ApiPropertyRefreshDuration = "計時器刷新間隔。";
    public const string ApiEventCountdownFinished = "TimerStatistic 倒計時到零時觸發。";
    public const string ApiPropertyEndValue = "StatisticCountUp 動畫到達的目標數字。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNameTitleFontSize = "統計標題使用的字號。";
    public const string TokenNameContentFontSize = "統計值內容使用的字號。";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusStable = "穩定";

}
