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

    public const string MiniTitle = "迷你模式";
    public const string MiniDescription = "適用於窄容器的精簡日曆（Fullscreen = false）。";

    public const string YearModeTitle = "年模式";
    public const string YearModeDescription = "顯示月份網格的年面板（Mode = Year）。";

    public const string ShowWeekTitle = "週序號";
    public const string ShowWeekDescription = "顯示額外的週序號欄（ShowWeek = true）。";

    public const string RangeTitle = "有效範圍與停用日期";
    public const string RangeDescription = "用 ValidRange 約束可選日期，用 DisabledDate 停用特定日期。";

    public const string CellTemplateTitle = "自訂儲存格內容";
    public const string CellTemplateDescription = "用 CellTemplate 在每個儲存格內渲染業務內容。";

    public const string FullCellTemplateTitle = "自訂完整儲存格";
    public const string FullCellTemplateDescription = "用 FullCellTemplate 替換整個儲存格 inner 內容。";

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

    public const string HeaderTemplateTitle = "自訂頭部";
    public const string HeaderTemplateDescription = "用 HeaderTemplate 替換預設頭部。";

    public const string EventsTitle = "選擇事件";
    public const string EventsDescription = "觀察 ValueChanged、Selected、PanelChanged 及其來源。";
    public const string EventsLogHint = "與日曆互動以檢視事件。";
}
