using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Calendar;

[LanguageProvider(LanguageCode.zh_CN, CalendarShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string PageSubtitle = "在月历面板中选择日期。";
    public const string PageDescription = "Calendar 以月、年、十年视图呈现日期，并支持单选、范围选择和多范围选择。";
    public const string ComponentCategory = "数据展示";
    public const string ComponentStatusStable = "稳定";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计变量";

    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "Calendar 的最简单用法。";

    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyFirstDayOfWeek = "设置每周的起始星期。";
    public const string ApiPropertyIsTodayHighlighted = "控制是否在月视图中高亮今天。";
    public const string ApiPropertyHeaderBackground = "设置日历头部背景画刷。";
    public const string ApiPropertyDisplayMode = "控制日历显示月、年或十年内容。";
    public const string ApiPropertySelectionMode = "控制用户可单选日期、选择单个范围、多个范围或禁止选择。";
    public const string ApiPropertySelectedDate = "获取或设置主要选中日期。";
    public const string ApiPropertyDisplayDate = "获取或设置日历当前显示的日期。";
    public const string ApiPropertyDisplayDateStart = "设置可显示的最早日期。";
    public const string ApiPropertyDisplayDateEnd = "设置可显示的最晚日期。";
    public const string ApiPropertyIsMotionEnabled = "控制日历切换动效是否启用。";

    public const string TokenColumnToken = "变量";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "状态";
    public const string TokenNameCellHoverBg = "单元格悬浮态背景色。";
    public const string TokenNameCellActiveWithRangeBg = "选取范围内单元格背景色。";
    public const string TokenNameCellHoverWithRangeBg = "选取范围内单元格悬浮态背景色。";
    public const string TokenNameCellBgDisabled = "禁用单元格背景色。";
    public const string TokenNameCellRangeBorderColor = "范围选择单元格边框色。";
    public const string TokenNameCellHeight = "日历单元格高度。";
    public const string TokenNameCellWidth = "日历单元格宽度。";
    public const string TokenNameCellLineHeight = "日历单元格文本行高。";
    public const string TokenNamePanelContentPadding = "日历面板内容内边距。";
    public const string TokenNameItemPanelMinWidth = "日历项面板最小宽度。";
    public const string TokenNameItemPanelMinHeight = "日历项面板最小高度。";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";

    protected override Type GetResourceKindType() => typeof(CalendarShowCaseLangResourceKind);
}
