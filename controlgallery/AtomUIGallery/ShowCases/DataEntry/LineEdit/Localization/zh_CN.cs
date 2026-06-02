using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.LineEdit;

[LanguageProvider(LanguageCode.zh_CN, LineEditShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicUsageTitle = "基础用法";
    public const string BasicUsageDescription = "基础用法示例。";
    public const string InputSizesTitle = "输入框三种尺寸";
    public const string InputSizesDescription = "输入框有三种尺寸：大号（40px）、默认（32px）和小号（24px）。";
    public const string VariantsTitle = "变体";
    public const string VariantsDescription = "输入框的变体。";
    public const string DisabledTitle = "禁用";
    public const string DisabledDescription = "输入框禁用样式的变体。";
    public const string PrePostTabTitle = "前置/后置标签";
    public const string PrePostTabDescription = "使用前置和后置标签的示例。";
    public const string WithClearIconTitle = "带清除图标";
    public const string WithClearIconDescription = "带移除图标的输入框，点击图标可清空全部内容。";
    public const string PasswordBoxTitle = "密码框";
    public const string PasswordBoxDescription = "密码类型输入框。";
    public const string PrefixAndSuffixTitle = "前缀和后缀";
    public const string PrefixAndSuffixDescription = "在输入框内部添加前缀或后缀图标。";
    public const string InputStatusTitle = "状态";
    public const string InputStatusDescription = "通过 status 为 Input 添加状态，可设置为错误或警告。";
    public const string SearchBoxTitle = "搜索框";
    public const string SearchBoxDescription = "将标准输入框和搜索按钮组合创建搜索框的示例。";
    public const string DisabledSearchBoxTitle = "禁用搜索框";
    public const string DisabledSearchBoxDescription = "将标准输入框和搜索按钮组合创建搜索框的示例。";
    public const string SearchBoxWithLoadingTitle = "带加载状态的搜索框";
    public const string SearchBoxWithLoadingDescription = "onSearch 时显示搜索加载状态。";
    public const string TextAreaTitle = "文本域";
    public const string TextAreaDescription = "用于多行输入。";
    public const string AutoSizeTextAreaTitle = "高度自适应内容";
    public const string AutoSizeTextAreaDescription = "textarea 类型 Input 的 autoSize 属性会让高度根据内容自动调整。可以向 autoSize 提供选项对象，指定自动调整的最小和最大行数。";
    public const string CharacterCountingTitle = "带字数统计";
    public const string CharacterCountingDescription = "显示字数统计。";
    public const string TextAreaStatusTitle = "状态";
    public const string TextAreaStatusDescription = "通过 status 为 TextArea 添加状态，可设置为错误或警告。";

    protected override Type GetResourceKindType() => typeof(LineEditShowCaseLangResourceKind);
}
