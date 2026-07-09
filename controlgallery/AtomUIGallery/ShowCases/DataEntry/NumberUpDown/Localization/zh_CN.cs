using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.NumberUpDown;

[LanguageProvider(LanguageCode.zh_CN, NumberUpDownShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string ScenarioBasic = "基础";
    public const string ScenarioRange = "范围";
    public const string ScenarioStyle = "样式";
    public const string ScenarioAddon = "附加内容";

    public const string BasicUsageTitle = "基础用法";
    public const string BasicUsageDescription = "仅支持数字输入的 NumberUpDown。";
    public const string SpinnerModeTitle = "拨轮";
    public const string SpinnerModeDescription = "数字拨轮。";
    public const string HideHandleTitle = "隐藏步进按钮";
    public const string HideHandleDescription = "通过 ShowButtonSpinner=\"False\" 隐藏步进按钮。";
    public const string StringModeTitle = "字符串模式（高精度）";
    public const string StringModeDescription = "以字符串形式保留高精度值。";
    public const string KeyboardBehaviorTitle = "键盘行为";
    public const string KeyboardBehaviorDescription = "通过 Keyboard 属性禁用键盘步进。";
    public const string MouseWheelBehaviorTitle = "鼠标滚轮行为";
    public const string MouseWheelBehaviorDescription = "输入框获得焦点时滚动鼠标滚轮，可按 Increment 增减值。";
    public const string MinMaxTitle = "最小/最大值";
    public const string MinMaxDescription = "限制输入值范围。";
    public const string DecimalStepTitle = "小数步长";
    public const string DecimalStepDescription = "通过 Increment 使用小数步长。";
    public const string SizesTitle = "NumberUpDown 尺寸";
    public const string SizesDescription = "NumberUpDown 支持大号（40px）、默认（32px）、小号（24px）和自定义尺寸。";
    public const string VariantsTitle = "变体";
    public const string VariantsDescription = "NumberUpDown 的变体。";
    public const string DisabledTitle = "禁用";
    public const string DisabledDescription = "NumberUpDown 禁用样式的变体。";
    public const string PrePostTabTitle = "前置/后置标签";
    public const string PrePostTabDescription = "使用前置和后置标签的示例。";
    public const string WithClearIconTitle = "带清除图标";
    public const string WithClearIconDescription = "带移除图标的输入框，点击图标可清空全部内容。";
    public const string PrefixAndSuffixTitle = "前缀和后缀";
    public const string PrefixAndSuffixDescription = "在输入框内部添加前缀或后缀图标。";
    public const string StatusTitle = "状态";
    public const string StatusDescription = "通过 status 为 Input 添加状态，可设置为错误或警告。";
    public const string P2PlaceholderTextInputWeight = "输入重量";
    public const string P2PlaceholderTextKeyboardDisabled = "键盘已禁用";
    public const string P2PlaceholderTextFocusAndScrollWheel = "聚焦后滚动鼠标滚轮";
    public const string P2PlaceholderTextInputWithClearIcon = "带清除图标的输入框";
    public const string P2PlaceholderTextEnterYourValue = "输入数值";
    public const string P2PlaceholderTextError = "错误";
    public const string P2PlaceholderTextWarning = "警告";
    public const string P2PlaceholderTextErrorWithPrefix = "带前缀的错误";
    public const string P2PlaceholderTextWarningWithPrefix = "带前缀的警告";
    public const string P2TextRawValuePrefix = "原始值：";

    public const string P2ContentKeyboardEnabled = "启用键盘";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计 Token";
    public const string PageSubtitle = "输入并通过键盘、滚轮和步进按钮调整数值。";
    public const string PageDescription =
        "NumberUpDown 组合了数值输入、高精度字符串模式、最小/最大约束、小数步长、输入变体、附加内容、清除入口和校验状态。";
    public const string ComponentCategory = "数据录入";
    public const string ComponentStatusStable = "稳定";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyValue = "当前数值。";
    public const string ApiPropertyMinimum = "允许输入的最小数值。";
    public const string ApiPropertyMaximum = "允许输入的最大数值。";
    public const string ApiPropertyIncrement = "通过步进按钮、键盘或滚轮增减的数值。";
    public const string ApiPropertyFormatString = "用于显示数值的格式字符串。";
    public const string ApiPropertyMode = "控件展示模式，可使用默认输入框模式或三段式拨轮模式。";
    public const string ApiPropertyIsStringMode = "以文本保留高精度输入，同时保持数值编辑行为。";
    public const string ApiPropertyStringValue = "高精度字符串模式使用的字符串值。";
    public const string ApiPropertyIsKeyboardEnabled = "允许 Up、Down、PageUp、PageDown 等键盘步进快捷键。";
    public const string ApiPropertyIsAllowClear = "当输入有内容时显示清除入口。";
    public const string ApiPropertyClearIcon = "清除入口使用的自定义图标。";
    public const string ApiPropertySizeType = "输入框尺寸变体。";
    public const string ApiPropertyStyleVariant = "输入框视觉变体，例如描边、填充或无边框。";
    public const string ApiPropertyStatus = "输入框表面显示的校验状态。";
    public const string ApiPropertyLeftAddOn = "附加在输入框框架前方的内容。";
    public const string ApiPropertyRightAddOn = "附加在输入框框架后方的内容。";
    public const string ApiPropertyInnerLeftContent = "渲染在输入框内部左侧的内容。";
    public const string ApiPropertyInnerRightContent = "渲染在输入框内部右侧的内容。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "状态";
    public const string TokenNameControlWidth = "数值输入框默认宽度。";
    public const string TokenNameHandleWidth = "步进按钮区域宽度。";
    public const string TokenNameHandleIconSize = "步进按钮使用的图标尺寸。";
    public const string TokenNameHandleBg = "步进按钮区域背景色。";
    public const string TokenNameHandleActiveBg = "步进按钮区域激活背景色。";
    public const string TokenNameHandleHoverColor = "步进按钮悬浮时的前景色。";
    public const string TokenNameHandleBorderColor = "步进按钮区域边框色。";
    public const string TokenNameFilledHandleBg = "填充变体使用的步进按钮背景色。";
    public const string TokenScopeComponent = "NumericUpDown";
    public const string TokenStatusStable = "稳定";

}
