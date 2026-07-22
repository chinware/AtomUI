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
    public const string PageSubtitle = "输入并通过键盘、滚轮和步进按钮调整数值。";
    public const string PageDescription =
        "NumberUpDown 组合了数值输入、高精度字符串模式、最小/最大约束、小数步长、输入变体、附加内容、清除入口和校验状态。";
    public const string ComponentCategory = "数据录入";
    public const string ComponentStatusStable = "稳定";

}
