using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.TimePicker;

[LanguageProvider(LanguageCode.zh_CN, TimePickerShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "点击 TimePicker 后，可以在面板中选择或输入时间。";
    public const string HourFormatsTitle = "12 小时和 24 小时格式";
    public const string HourFormatsDescription = "TimePicker 支持 12 小时和 24 小时两种时间格式。";
    public const string ThreeSizesTitle = "三种尺寸";
    public const string ThreeSizesDescription = "输入框提供大号、中号和小号三种尺寸。大号用于表单，中号为默认尺寸。";
    public const string DisabledTitle = "禁用";
    public const string DisabledDescription = "TimePicker 的禁用状态。";
    public const string IntervalOptionTitle = "间隔选项";
    public const string IntervalOptionDescription = "通过 MinuteIncrement 和 SecondIncrement 显示步进选项。";
    public const string TwelveHoursTitle = "12 小时制";
    public const string TwelveHoursDescription = "12 小时格式的 TimePicker，默认格式为 h:mm:ss a。";
    public const string VariantsTitle = "变体";
    public const string VariantsDescription = "无边框风格组件。";
    public const string StatusTitle = "状态";
    public const string StatusDescription = "通过 status 为 TimePicker 添加状态，可设置为错误或警告。";
    public const string TimeRangePickerTitle = "时间范围选择器";
    public const string TimeRangePickerDescription = "使用 RangeTimePicker 进行时间范围选择。";
    public const string P2PlaceholderTextSelectTime = "选择时间";
    public const string P2PlaceholderTextOutline = "描边风格";
    public const string P2PlaceholderTextFilled = "填充风格";
    public const string P2PlaceholderTextBorderless = "无边框";
    public const string P2PlaceholderTextStartTime = "开始时间";
    public const string P2SecondaryPlaceholderTextEndTime = "结束时间";
    public const string PageSubtitle = "从弹出时间面板中选择单个时间或时间范围。";
    public const string PageDescription = "TimePicker 支持 12 小时和 24 小时制、尺寸变体、禁用状态、分钟和秒的步进选项、视觉变体、校验状态以及范围选择。";
    public const string ComponentCategory = "数据录入";
    public const string ComponentStatusStable = "稳定";
    public const string InfoNamespaceLabel = "命名空间";
    public const string InfoPackageLabel = "包名";
    public const string InfoBaseClassLabel = "基类";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计变量";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertySelectedTime = "TimePicker 当前选中的时间值。";
    public const string ApiPropertyDefaultTime = "TimePicker 重置行为使用的初始时间。";
    public const string ApiPropertyIsNeedConfirm = "要求用户确认所选时间后再提交。";
    public const string ApiPropertyIsShowNow = "是否在弹出面板中显示“此刻”快捷操作。";
    public const string ApiPropertyMinuteIncrement = "生成分钟选项时使用的步进值。";
    public const string ApiPropertySecondIncrement = "生成秒选项时使用的步进值。";
    public const string ApiPropertyClockIdentifier = "选择 12 小时或 24 小时时钟显示。";
    public const string ApiPropertyRangeStartSelectedTime = "RangeTimePicker 当前选中的开始时间。";
    public const string ApiPropertyRangeEndSelectedTime = "RangeTimePicker 当前选中的结束时间。";
    public const string ApiPropertyRangeStartDefaultTime = "RangeTimePicker 重置行为使用的初始开始时间。";
    public const string ApiPropertyRangeEndDefaultTime = "RangeTimePicker 重置行为使用的初始结束时间。";
    public const string TokenColumnToken = "变量";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "状态";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";
    public const string TokenNameItemHeight = "每个可选时间项的高度。";
    public const string TokenNameItemWidth = "小时、分钟和秒列的宽度。";
    public const string TokenNamePeriodHostWidth = "上午/下午选择列的宽度。";
    public const string TokenNameItemPadding = "每个可选时间项的内边距。";
    public const string TokenNameButtonsMargin = "弹出操作按钮区域的顶部外边距。";
    public const string TokenNameRangePickerArrowMargin = "范围输入之间箭头的外边距。";
    public const string TokenNameRangePickerIndicatorThickness = "范围选择指示器的厚度。";
    public const string TokenNameHeaderMargin = "时间面板头部下方的外边距。";

    protected override Type GetResourceKindType() => typeof(TimePickerShowCaseLangResourceKind);
}
