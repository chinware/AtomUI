using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.TimePicker;

[LanguageProvider(LanguageCode.zh_CN, TimePickerShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
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

    protected override Type GetResourceKindType() => typeof(TimePickerShowCaseLangResourceKind);
}
