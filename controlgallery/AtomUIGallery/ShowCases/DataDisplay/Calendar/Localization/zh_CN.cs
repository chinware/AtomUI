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

    public const string CardTitle = "卡片模式";
    public const string CardDescription = "用于嵌套在空间有限的容器中。";

    public const string LunarCalendarTitle = "农历日历";
    public const string LunarCalendarDescription = "完整模式的日历，展示农历日期、二十四节气、传统节日和应用提供的节假日标记。";
    public const string LunarCalendarCardTitle = "农历卡片日历";
    public const string LunarCalendarCardDescription = "适用于有限空间的紧凑农历日历。";

    public const string SelectableCalendarTitle = "可选择的日历";
    public const string SelectableCalendarDescription = "一个通用的日历面板，支持年/月切换。";
    public const string SelectableCalendarSelectedMessage = "你选择的日期：{0:yyyy-MM-dd}";

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

}
