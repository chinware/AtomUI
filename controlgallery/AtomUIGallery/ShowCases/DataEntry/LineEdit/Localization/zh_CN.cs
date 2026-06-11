using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.LineEdit;

[LanguageProvider(LanguageCode.zh_CN, LineEditShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string ScenarioBasic = "基础";
    public const string ScenarioState = "状态";
    public const string ScenarioSearch = "搜索";
    public const string ScenarioTextArea = "文本域";

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
    public const string P2PlaceholderTextBasicUsage = "基础用法";
    public const string P2PlaceholderTextLarge = "大号";
    public const string P2PlaceholderTextMiddle = "中号";
    public const string P2PlaceholderTextSmall = "小号";
    public const string P2TitleNormal = "普通";
    public const string P2PlaceholderTextOutlined = "线框风格";
    public const string P2PlaceholderTextFilled = "填充风格";
    public const string P2PlaceholderTextBorderless = "无边框";
    public const string P2PlaceholderTextUnderlined = "下划线";
    public const string P2TitleLeftaddonAndRightaddon = "左侧附加和右侧附加";
    public const string P2TextMysite = "我的站点";
    public const string P2PlaceholderTextInputWithClearIcon = "带清除图标的输入框";
    public const string P2PlaceholderTextTextareaWithClearIcon = "带清除图标的文本域";
    public const string P2PlaceholderTextInputPassword = "输入密码";
    public const string P2PlaceholderTextEnterYourUsername = "输入用户名";
    public const string P2PlaceholderTextError = "错误";
    public const string P2PlaceholderTextWarning = "警告";
    public const string P2PlaceholderTextErrorWithPrefix = "带前缀的错误";
    public const string P2PlaceholderTextWarningWithPrefix = "带前缀的警告";
    public const string P2TitleSearch = "搜索";
    public const string P2PlaceholderTextInputSearchText = "输入搜索文本";
    public const string P2TitleFilled = "填充风格";
    public const string P2TitleBorderless = "无边框";
    public const string P2TitleUnderlined = "下划线";
    public const string P2PlaceholderTextInputSearchLoadingDefault = "默认搜索加载";
    public const string P2PlaceholderTextInputSearchLoadingWithEnterbutton = "带搜索按钮的搜索加载";
    public const string P2PlaceholderTextMaxlengthIsN6 = "最大长度为 6";
    public const string P2PlaceholderTextDisabled = "禁用";
    public const string P2PlaceholderTextAutosizeHeightBasedOnContentLines = "根据内容行数自动调整高度";
    public const string P2PlaceholderTextAutosizeHeightWithMinimumAndMaximumNumberOf = "按最小和最大行数自动调整高度";
    public const string P2PlaceholderTextCanResize = "可调整大小";
    public const string P2PlaceholderTextDisableResize = "禁止调整大小";

    public const string P2SearchButtonTextSearch = "搜索";

    public const string P2SearchButtonTextText = "搜索一下";

    protected override Type GetResourceKindType() => typeof(LineEditShowCaseLangResourceKind);
}
