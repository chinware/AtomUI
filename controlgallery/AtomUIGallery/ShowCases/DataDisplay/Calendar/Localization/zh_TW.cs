using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Calendar;

[LanguageProvider(LanguageCode.zh_TW, CalendarShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string PageSubtitle = "在桌面日曆面板中依日期組織業務內容。";
    public const string PageDescription = "Calendar 以月日期網格或年月份網格呈現日期，支援有效範圍、停用日期、儲存格定製與選擇事件。";
    public const string ComponentCategory = "資料展示";
    public const string ComponentStatusStable = "穩定";
    public const string ScenarioExamples = "範例";

    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "完整模式的月曆。";

    public const string CardTitle = "卡片模式";
    public const string CardDescription = "用於嵌套在空間有限的容器中。";

    public const string CustomHeaderTitle = "自訂 Header";
    public const string CustomHeaderDescription = "自訂日曆標頭內容。";
    public const string CustomHeaderContentTitle = "自訂 Header";
    public const string CustomHeaderMonthText = "月";
    public const string CustomHeaderYearText = "年";

    public const string LunarCalendarTitle = "農曆日曆";
    public const string LunarCalendarDescription = "完整模式的日曆，展示農曆日期、二十四節氣、傳統節日和應用提供的節假日標記。";
    public const string LunarCalendarCardTitle = "農曆卡片日曆";
    public const string LunarCalendarCardDescription = "適用於有限空間的緊湊農曆日曆。";

    public const string SelectableCalendarTitle = "可選擇的日曆";
    public const string SelectableCalendarDescription = "一個通用的日曆面板，支援年/月切換。";
    public const string SelectableCalendarSelectedMessage = "你選擇的日期：{0:yyyy-MM-dd}";

    public const string ShowWeekTitle = "顯示週數";
    public const string ShowWeekDescription = "透過將 ShowWeek 設定為 True，在完整模式和卡片模式日曆中顯示週數。";

    public const string NoticeCalendarTitle = "通知事項日曆";
    public const string NoticeCalendarDescription = "在日期儲存格內展示通知事項，在月份儲存格內展示待辦數量。";
    public const string NoticeCalendarWarningEventText = "這是警告事項。";
    public const string NoticeCalendarUsualEventText = "這是一般事項。";
    public const string NoticeCalendarErrorEventText = "這是錯誤事項。";
    public const string NoticeCalendarLongUsualEventText = "這是很長的一般事項......";
    public const string NoticeCalendarErrorEvent1Text = "這是錯誤事項 1。";
    public const string NoticeCalendarErrorEvent2Text = "這是錯誤事項 2。";
    public const string NoticeCalendarErrorEvent3Text = "這是錯誤事項 3。";
    public const string NoticeCalendarErrorEvent4Text = "這是錯誤事項 4。";
    public const string NoticeCalendarBacklogText = "待辦數量";

    public const string CrossDateEventsTitle = "跨日期事件";
    public const string CrossDateEventsDescription = "宣告 RangeBars，由 Calendar 自動繪製跨日期連續事件條。";
    public const string CrossDateEventsReleaseText = "發布窗口";
    public const string CrossDateEventsDesignReviewText = "設計評審";
    public const string CrossDateEventsMaintenanceText = "維護窗口";
    public const string CrossDateEventsBugFixText = "缺陷修復";

}
