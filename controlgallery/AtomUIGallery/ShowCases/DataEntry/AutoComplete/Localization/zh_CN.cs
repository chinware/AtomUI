using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.AutoComplete;

[LanguageProvider(LanguageCode.zh_CN, AutoCompleteShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicUsageTitle = "基础用法";
    public const string BasicUsageDescription = "基础用法，通过 options 属性设置自动完成的数据源。";
    public const string CustomizedTitle = "自定义";
    public const string CustomizedDescription = "可以设置自定义的选项标签。";
    public const string CustomOptionRenderingTitle = "自定义选项渲染";
    public const string CustomOptionRenderingDescription = "使用 OptionTemplate 渲染包含多字段、徽标和多行布局的丰富选项内容。";
    public const string LookupPatternsUncertainCategoryTitle = "查询模式 - 不确定分类";
    public const string LookupPatternsUncertainCategoryDescription = "演示查询模式中的不确定分类场景。";
    public const string TextAreaAutoCompletionTitle = "TextArea 类型自动完成";
    public const string TextAreaAutoCompletionDescription = "可以在 TextArea 类型中使用自动完成。";
    public const string NonCaseSensitiveTitle = "大小写不敏感的自动完成";
    public const string NonCaseSensitiveDescription = "大小写不敏感的 AutoComplete。";
    public const string StatusTitle = "状态";
    public const string StatusDescription = "为 AutoComplete 添加状态，可设置为错误或警告。";
    public const string VariantsTitle = "变体";
    public const string VariantsDescription = "可选择描边、填充、无边框和下划线等变体。";
    public const string CustomizeClearButtonTitle = "自定义清除按钮";
    public const string CustomizeClearButtonDescription = "自定义清除按钮。";
    public const string P2PlaceholderTextInputHere = "在此输入";
    public const string P2PlaceholderTextTryAOrB = "try 'a' or 'b'";
    public const string P2TextResults = "results";
    public const string P2PlaceholderTextTryToTypeB = "try to type `b`";
    public const string P2PlaceholderTextOutline = "Outline";
    public const string P2PlaceholderTextFilled = "填充风格";
    public const string P2PlaceholderTextBorderless = "Borderless";
    public const string P2PlaceholderTextUnderlined = "Underlined";
    public const string P2PlaceholderTextUnclearable = "UnClearable";
    public const string P2PlaceholderTextCustomizedClearIcon = "Customized clear icon";

    protected override Type GetResourceKindType() => typeof(AutoCompleteShowCaseLangResourceKind);
}
