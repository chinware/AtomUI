using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.NumberUpDown;

[LanguageProvider(LanguageCode.zh_CN, NumberUpDownShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicUsageTitle = "基础用法";
    public const string BasicUsageDescription = "仅支持数字输入的 NumberUpDown。";
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
    public const string SizesTitle = "NumberUpDown 三种尺寸";
    public const string SizesDescription = "输入框有三种尺寸：大号（40px）、默认（32px）和小号（24px）。";
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

    protected override Type GetResourceKindType() => typeof(NumberUpDownShowCaseLangResourceKind);
}
