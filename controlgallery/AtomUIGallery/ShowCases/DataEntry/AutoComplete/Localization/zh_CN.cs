using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.AutoComplete;

[LanguageProvider(LanguageCode.zh_CN, AutoCompleteShowCase.LanguageId)]
internal partial class zh_CN
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
    public const string SizeTypeTitle = "SizeType";
    public const string SizeTypeDescription = "AutoComplete 支持大号、中号、小号，也支持通过 Custom 配合本地高度自定义。";
    public const string P2PlaceholderTextInputHere = "在此输入";
    public const string P2PlaceholderTextTryAOrB = "try 'a' or 'b'";
    public const string P2PlaceholderSizeTypeLarge = "SizeType：Large";
    public const string P2PlaceholderSizeTypeMiddle = "SizeType：Middle";
    public const string P2PlaceholderSizeTypeSmall = "SizeType：Small";
    public const string P2PlaceholderSizeTypeCustom = "SizeType：Custom";
    public const string P2TextResults = "results";
    public const string P2PlaceholderTextTryToTypeB = "try to type `b`";
    public const string P2PlaceholderTextOutline = "Outline";
    public const string P2PlaceholderTextFilled = "填充风格";
    public const string P2PlaceholderTextBorderless = "Borderless";
    public const string P2PlaceholderTextUnderlined = "Underlined";
    public const string P2PlaceholderTextUnclearable = "UnClearable";
    public const string P2PlaceholderTextCustomizedClearIcon = "Customized clear icon";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计 Token";
    public const string PageSubtitle = "输入时提前匹配，并从本地或异步选项中选择建议值。";
    public const string PageDescription =
        "AutoComplete 将文本输入与候选过滤、异步加载、自定义选项渲染、状态反馈以及搜索框或文本域变体组合在一起。";
    public const string ComponentCategory = "数据录入";
    public const string ComponentStatusStable = "稳定";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyValue = "自动完成输入框的当前文本值。";
    public const string ApiPropertyOptionsSource = "用于候选建议的静态选项数据源。";
    public const string ApiPropertyOptionsAsyncLoader = "根据当前输入上下文提供选项的异步加载器。";
    public const string ApiPropertyOptionTemplate = "用于渲染每一个建议选项的模板。";
    public const string ApiPropertyFilter = "决定哪些选项保持可见的过滤器。";
    public const string ApiPropertyFilterValueSelector = "选择传递给过滤器的选项值。";
    public const string ApiPropertyIsAllowClear = "当输入有内容时显示清除入口。";
    public const string ApiPropertyClearIcon = "清除入口使用的自定义图标。";
    public const string ApiPropertyStyleVariant = "输入框视觉变体，例如描边、填充、无边框或下划线。";
    public const string ApiPropertyStatus = "输入框表面显示的校验状态。";
    public const string ApiPropertyPlacement = "候选列表弹窗的首选弹出位置。";
    public const string ApiPropertyMinimumPrefixLength = "自动完成建议可打开前的最小输入长度。";
    public const string ApiPropertyDisplayCandidateCount = "用于计算弹窗高度的候选行数。";
    public const string ApiPropertyMaxDropDownHeight = "候选弹窗最大高度。";
    public const string ApiPropertyIsPopupMatchSelectWidth = "让弹窗宽度匹配输入框宽度。";
    public const string ApiPropertyShouldUseOverlayPopup = "打开建议时使用 overlay popup 宿主。";
    public const string ApiPropertyAutoCompleteSearchButtonStyle = "AutoCompleteSearchEdit 使用的按钮样式。";
    public const string ApiPropertyAutoCompleteTextAreaLines = "AutoCompleteTextArea 显示的文本行数。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "状态";
    public const string TokenNamePopupContentPadding = "候选弹窗内部边距。";
    public const string TokenNameOptionHeight = "每个建议选项的高度。";
    public const string TokenNameMinPopupWidth = "候选弹窗最小宽度。";
    public const string TokenNameMaxPopupWidth = "弹窗不跟随输入框宽度时的最大宽度。";
    public const string TokenScopeComponent = "AutoComplete";
    public const string TokenStatusStable = "稳定";

}
