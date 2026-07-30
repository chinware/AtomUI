using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Calendar;

[LanguageProvider(LanguageCode.zh_CN, CalendarShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string PageSubtitle = "在桌面日历面板中按日期组织业务内容。";
    public const string PageDescription = "Calendar 遵循 Ant Design 6 语义，以月日期网格或年月份网格呈现日期，支持有效范围、禁用日期、单元格定制与选择事件。";
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

    public const string HeaderTemplateTitle = "自定义头部";
    public const string HeaderTemplateDescription = "用 HeaderTemplate 替换默认头部。";

    public const string EventsTitle = "选择事件";
    public const string EventsDescription = "观察 ValueChanged、Selected、PanelChanged 及其来源。";
    public const string EventsLogHint = "与日历交互以查看事件。";
}
