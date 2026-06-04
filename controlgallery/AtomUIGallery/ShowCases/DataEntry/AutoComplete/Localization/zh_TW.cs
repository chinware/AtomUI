using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.AutoComplete;

[LanguageProvider(LanguageCode.zh_TW, AutoCompleteShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicUsageTitle = "基礎用法";
    public const string BasicUsageDescription = "基礎用法，通過 options 屬性設置自動完成的數據源。";
    public const string CustomizedTitle = "自定義";
    public const string CustomizedDescription = "可以設置自定義的選項標籤。";
    public const string CustomOptionRenderingTitle = "自定義選項渲染";
    public const string CustomOptionRenderingDescription = "使用 OptionTemplate 渲染包含多字段、徽標和多行佈局的豐富選項內容。";
    public const string LookupPatternsUncertainCategoryTitle = "查詢模式 - 不確定分類";
    public const string LookupPatternsUncertainCategoryDescription = "演示查詢模式中的不確定分類場景。";
    public const string TextAreaAutoCompletionTitle = "TextArea 類型自動完成";
    public const string TextAreaAutoCompletionDescription = "可以在 TextArea 類型中使用自動完成。";
    public const string NonCaseSensitiveTitle = "大小寫不敏感的自動完成";
    public const string NonCaseSensitiveDescription = "大小寫不敏感的 AutoComplete。";
    public const string StatusTitle = "狀態";
    public const string StatusDescription = "為 AutoComplete 添加狀態，可設置為錯誤或警告。";
    public const string VariantsTitle = "變體";
    public const string VariantsDescription = "可選擇描邊、填充、無邊框和下划線等變體。";
    public const string CustomizeClearButtonTitle = "自定義清除按鈕";
    public const string CustomizeClearButtonDescription = "自定義清除按鈕。";
    public const string P2PlaceholderTextInputHere = "在此輸入";
    public const string P2PlaceholderTextTryAOrB = "try 'a' or 'b'";
    public const string P2TextResults = "results";
    public const string P2PlaceholderTextTryToTypeB = "try to type `b`";
    public const string P2PlaceholderTextOutline = "Outline";
    public const string P2PlaceholderTextFilled = "填充風格";
    public const string P2PlaceholderTextBorderless = "Borderless";
    public const string P2PlaceholderTextUnderlined = "Underlined";
    public const string P2PlaceholderTextUnclearable = "UnClearable";
    public const string P2PlaceholderTextCustomizedClearIcon = "Customized clear icon";

    protected override Type GetResourceKindType() => typeof(AutoCompleteShowCaseLangResourceKind);
}

