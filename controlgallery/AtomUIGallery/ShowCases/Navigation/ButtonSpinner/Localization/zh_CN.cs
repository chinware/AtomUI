using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ButtonSpinner;

[LanguageProvider(LanguageCode.zh_CN, ButtonSpinnerShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string ComponentCategory = "导航";
    public const string ComponentStatusStable = "稳定";
    public const string PageSubtitle = "带按钮操作柄的紧凑微调输入控件。";
    public const string PageDescription = "ButtonSpinner 将输入式内容区与递增/递减按钮结合，支持前后置标签、内部前后缀、状态、尺寸和视觉形态。";
    public const string InfoNamespaceLabel = "命名空间";
    public const string InfoPackageLabel = "包";
    public const string InfoBaseClassLabel = "基类";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计令牌";

    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "基础的按钮微调器。";
    public const string ThreeSizesTitle = "三种尺寸";
    public const string ThreeSizesDescription = "按钮微调器提供三种尺寸：大号（40px）、默认（32px）和小号（24px）。";
    public const string VariantsTitle = "不同形态";
    public const string VariantsDescription = "输入框的不同形态。";
    public const string DisabledTitle = "禁用状态";
    public const string DisabledDescription = "禁用状态下的输入框形态。";
    public const string PrePostTabTitle = "前置/后置标签";
    public const string PrePostTabDescription = "前置和后置标签的使用示例。";
    public const string PrefixSuffixTitle = "前缀和后缀";
    public const string PrefixSuffixDescription = "在输入框内部添加前缀或后缀图标。";
    public const string StatusTitle = "状态";
    public const string StatusDescription = "通过 status 为输入框添加错误或警告状态。";

    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyIsSpinEnabled = "启用指针、键盘和滚轮触发的微调操作。";
    public const string ApiPropertyIsButtonSpinnerVisible = "控制微调按钮操作柄是否可见。";
    public const string ApiPropertyButtonSpinnerLocation = "设置微调按钮操作柄位于左侧或右侧。";
    public const string ApiPropertyLeftAddOn = "显示在输入区域外左侧的内容。";
    public const string ApiPropertyRightAddOn = "显示在输入区域外右侧的内容。";
    public const string ApiPropertyInnerLeftContent = "显示在输入区域内数值前方的内容。";
    public const string ApiPropertyInnerRightContent = "显示在输入区域内数值后方的内容。";
    public const string ApiPropertySizeType = "控制按钮微调器尺寸。";
    public const string ApiPropertyStyleVariant = "控制描边、填充和无边框视觉形态。";
    public const string ApiPropertyStatus = "应用校验状态样式。";
    public const string ApiPropertyIsButtonSpinnerFloatable = "允许微调按钮操作柄在交互前浮动在内容上方。";
    public const string ApiPropertyIsMotionEnabled = "在主题允许时启用控件动效。";
    public const string ApiPropertySpinnerHandleWidth = "覆盖微调按钮操作柄宽度。";
    public const string ApiPropertySpin = "用户请求递增或递减微调操作时触发。";

    public const string TokenColumnToken = "令牌";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "范围";
    public const string TokenColumnStatus = "状态";
    public const string TokenNameControlWidth = "默认控件宽度。";
    public const string TokenNameHandleWidth = "微调按钮操作柄宽度。";
    public const string TokenNameHandleIconSize = "微调按钮操作柄图标尺寸。";
    public const string TokenNameHandleBg = "微调按钮操作柄背景色。";
    public const string TokenNameHandleActiveBg = "微调按钮操作柄激活背景色。";
    public const string TokenNameHandleHoverColor = "微调按钮操作柄悬浮前景色。";
    public const string TokenNameHandleBorderColor = "微调按钮操作柄边框色。";
    public const string TokenNameFilledHandleBg = "填充形态下的微调按钮操作柄背景色。";
    public const string TokenNameInputFontSize = "继承自 LineEdit 的默认输入字号。";
    public const string TokenNameInputFontSizeLG = "继承自 LineEdit 的大号输入字号。";
    public const string TokenNameInputFontSizeSM = "继承自 LineEdit 的小号输入字号。";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";

    protected override Type GetResourceKindType() => typeof(ButtonSpinnerShowCaseLangResourceKind);
}
