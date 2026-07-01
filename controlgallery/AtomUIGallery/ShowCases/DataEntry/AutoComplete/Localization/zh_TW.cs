using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.AutoComplete;

[LanguageProvider(LanguageCode.zh_TW, AutoCompleteShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
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
    public const string SizeTypeTitle = "SizeType";
    public const string SizeTypeDescription = "AutoComplete 支持大號、中號、小號，也支持通過 Custom 配合本地高度自定義。";
    public const string P2PlaceholderTextInputHere = "在此輸入";
    public const string P2PlaceholderTextTryAOrB = "try 'a' or 'b'";
    public const string P2PlaceholderSizeTypeLarge = "SizeType：Large";
    public const string P2PlaceholderSizeTypeMiddle = "SizeType：Middle";
    public const string P2PlaceholderSizeTypeSmall = "SizeType：Small";
    public const string P2PlaceholderSizeTypeCustom = "SizeType：Custom";
    public const string P2TextResults = "results";
    public const string P2PlaceholderTextTryToTypeB = "try to type `b`";
    public const string P2PlaceholderTextOutline = "Outline";
    public const string P2PlaceholderTextFilled = "填充風格";
    public const string P2PlaceholderTextBorderless = "Borderless";
    public const string P2PlaceholderTextUnderlined = "Underlined";
    public const string P2PlaceholderTextUnclearable = "UnClearable";
    public const string P2PlaceholderTextCustomizedClearIcon = "Customized clear icon";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計 Token";
    public const string PageSubtitle = "輸入時提前匹配，並從本地或異步選項中選擇建議值。";
    public const string PageDescription =
        "AutoComplete 將文本輸入與候選過濾、異步加載、自定義選項渲染、狀態反饋以及搜索框或文本域變體組合在一起。";
    public const string ComponentCategory = "數據錄入";
    public const string ComponentStatusStable = "穩定";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertyValue = "自動完成輸入框的當前文本值。";
    public const string ApiPropertyOptionsSource = "用於候選建議的靜態選項數據源。";
    public const string ApiPropertyOptionsAsyncLoader = "根據當前輸入上下文提供選項的異步加載器。";
    public const string ApiPropertyOptionTemplate = "用於渲染每一個建議選項的模板。";
    public const string ApiPropertyFilter = "決定哪些選項保持可見的過濾器。";
    public const string ApiPropertyFilterValueSelector = "選擇傳遞給過濾器的選項值。";
    public const string ApiPropertyIsAllowClear = "當輸入有內容時顯示清除入口。";
    public const string ApiPropertyClearIcon = "清除入口使用的自定義圖標。";
    public const string ApiPropertyStyleVariant = "輸入框視覺變體，例如描邊、填充、無邊框或下劃線。";
    public const string ApiPropertyStatus = "輸入框表面顯示的校驗狀態。";
    public const string ApiPropertyPlacement = "候選列表彈窗的首選彈出位置。";
    public const string ApiPropertyMinimumPrefixLength = "自動完成建議可打開前的最小輸入長度。";
    public const string ApiPropertyDisplayCandidateCount = "用於計算彈窗高度的候選行數。";
    public const string ApiPropertyMaxDropDownHeight = "候選彈窗最大高度。";
    public const string ApiPropertyIsPopupMatchSelectWidth = "讓彈窗寬度匹配輸入框寬度。";
    public const string ApiPropertyShouldUseOverlayPopup = "打開建議時使用 overlay popup 宿主。";
    public const string ApiPropertyAutoCompleteSearchButtonStyle = "AutoCompleteSearchEdit 使用的按鈕樣式。";
    public const string ApiPropertyAutoCompleteTextAreaLines = "AutoCompleteTextArea 顯示的文本行數。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNamePopupContentPadding = "候選彈窗內部邊距。";
    public const string TokenNameOptionHeight = "每個建議選項的高度。";
    public const string TokenNameMinPopupWidth = "候選彈窗最小寬度。";
    public const string TokenNameMaxPopupWidth = "彈窗不跟隨輸入框寬度時的最大寬度。";
    public const string TokenScopeComponent = "AutoComplete";
    public const string TokenStatusStable = "穩定";

    protected override Type GetResourceKindType() => typeof(AutoCompleteShowCaseLangResourceKind);
}
