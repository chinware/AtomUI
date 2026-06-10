using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.DatePicker;

[LanguageProvider(LanguageCode.zh_CN, DatePickerShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "点击 DatePicker 后，可以在面板中选择或输入日期。";
    public const string RangePickerTitle = "范围选择器";
    public const string RangePickerDescription = "通过 picker 属性设置范围选择器类型。";
    public const string NeedConfirmTitle = "需要确认";
    public const string NeedConfirmDescription = "DatePicker 会根据 picker 属性自动判断是否显示确认按钮，也可以通过 needConfirm 属性控制。设置 needConfirm 后，用户必须点击确认按钮完成选择；否则在选择器失去焦点或选择日期时提交选择。";
    public const string ChooseTimeTitle = "选择时间";
    public const string ChooseTimeDescription = "该属性提供额外的时间选择。当 showTime 为对象时，其属性会传递给内置 TimePicker。";
    public const string DisabledTitle = "禁用";
    public const string DisabledDescription = "DatePicker 的禁用状态。也可以设置为数组来禁用其中一个输入框。";
    public const string ThreeSizesTitle = "三种尺寸";
    public const string ThreeSizesDescription = "输入框提供小号、中号和大号三种尺寸。省略 size 时使用中号。";
    public const string StatusTitle = "状态";
    public const string StatusDescription = "通过 status 为 DatePicker 添加状态，可设置为错误或警告。";
    public const string VariantsTitle = "变体";
    public const string VariantsDescription = "无边框风格组件。";
    public const string PlacementTitle = "弹出位置";
    public const string PlacementDescription = "可以通过 placement 手动指定弹出层位置。";
    public const string P2PlaceholderTextSelectDate = "选择日期";
    public const string P2SecondaryPlaceholderTextEndDate = "结束日期";
    public const string P2PlaceholderTextSelectTime = "选择时间";
    public const string P2PlaceholderTextStartDate = "开始日期";
    public const string P2PlaceholderTextOutline = "描边风格";
    public const string P2PlaceholderTextFilled = "填充风格";
    public const string P2PlaceholderTextBorderless = "无边框";
    public const string P2TextExpandDirection = "选择器尺寸：";
    public const string P2ContentLarge = "大号";
    public const string P2ContentDefault = "默认";
    public const string P2ContentSmall = "小号";
    public const string P2TextPlacement = "弹出位置：";
    public const string P2ContentTopleft = "左上";
    public const string P2ContentTopright = "右上";
    public const string P2ContentBottomleft = "左下";
    public const string P2ContentBottomright = "右下";

    protected override Type GetResourceKindType() => typeof(DatePickerShowCaseLangResourceKind);
}
