using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Calendar;

[LanguageProvider(LanguageCode.zh_CN, CalendarShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string PageSubtitle = "在桌面日历面板中按日期组织业务内容。";
    public const string PageDescription = "Calendar 以月日期网格或年月份网格呈现日期，支持有效范围、禁用日期、单元格定制与选择事件。";
    public const string ComponentCategory = "数据展示";
    public const string ComponentStatusStable = "稳定";
    public const string ScenarioExamples = "示例";

    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "完整模式的月日历。";

    public const string MiniTitle = "迷你模式";
    public const string MiniDescription = "适用于窄容器的紧凑日历（Fullscreen = false）。";

    public const string YearModeTitle = "年模式";
    public const string YearModeDescription = "显示月份网格的年面板（Mode = Year）。";

    public const string ShowWeekTitle = "周序号";
    public const string ShowWeekDescription = "显示额外的周序号列（ShowWeek = true）。";

    public const string RangeTitle = "有效范围与禁用日期";
    public const string RangeDescription = "用 ValidRange 约束可选日期，用 DisabledDate 禁用特定日期。";

    public const string CellTemplateTitle = "自定义单元格内容";
    public const string CellTemplateDescription = "用 CellTemplate 在每个单元格内渲染业务内容。";

    public const string FullCellTemplateTitle = "自定义完整单元格";
    public const string FullCellTemplateDescription = "用 FullCellTemplate 替换整个单元格 inner 内容。";

    public const string NoticeCalendarTitle = "通知事项日历";
    public const string NoticeCalendarDescription = "在日期单元格内展示通知事项，在月份单元格内展示待办数量。";
    public const string NoticeCalendarWarningEventText = "这是警告事项。";
    public const string NoticeCalendarUsualEventText = "这是普通事项。";
    public const string NoticeCalendarErrorEventText = "这是错误事项。";
    public const string NoticeCalendarLongUsualEventText = "这是很长的普通事项......";
    public const string NoticeCalendarErrorEvent1Text = "这是错误事项 1。";
    public const string NoticeCalendarErrorEvent2Text = "这是错误事项 2。";
    public const string NoticeCalendarErrorEvent3Text = "这是错误事项 3。";
    public const string NoticeCalendarErrorEvent4Text = "这是错误事项 4。";
    public const string NoticeCalendarBacklogText = "待办数量";

    public const string CrossDateEventsTitle = "跨日期事件";
    public const string CrossDateEventsDescription = "声明 RangeBars，由 Calendar 自动绘制跨日期连续事件条。";
    public const string CrossDateEventsReleaseText = "发布窗口";
    public const string CrossDateEventsDesignReviewText = "设计评审";
    public const string CrossDateEventsMaintenanceText = "维护窗口";
    public const string CrossDateEventsBugFixText = "缺陷修复";

    public const string HeaderTemplateTitle = "自定义头部";
    public const string HeaderTemplateDescription = "用 HeaderTemplate 替换默认头部。";

    public const string EventsTitle = "选择事件";
    public const string EventsDescription = "观察 ValueChanged、Selected、PanelChanged 及其来源。";
    public const string EventsLogHint = "与日历交互以查看事件。";
}
