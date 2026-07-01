using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ComboBox;

[LanguageProvider(LanguageCode.zh_TW, ComboBoxShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string ComponentCategory = "導航";
    public const string ComponentStatusStable = "穩定";
    public const string PageSubtitle = "用於從緊湊彈出列表中選擇內容的選擇輸入控件。";
    public const string PageDescription = "ComboBox 將輸入式佈局與下拉選擇結合，支持項模板、前後置標籤、內部前後綴、校驗狀態和尺寸形態。";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計令牌";

    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "基礎的組合框用法。";
    public const string ItemsSourceTitle = "通過 ItemsSource 生成 ComboBoxItem";
    public const string ItemsSourceDescription = "基於 ItemsSource 和模板生成結構。";
    public const string EditableFilterTitle = "可編輯過濾";
    public const string EditableFilterDescription = "在 ComboBox 輸入區輸入文本，過濾彈出候選項，同時不替換原始 ItemsSource。";
    public const string DisabledTitle = "禁用狀態";
    public const string DisabledDescription = "禁用狀態的組合框。";
    public const string ThreeSizesTitle = "三種尺寸";
    public const string ThreeSizesDescription = "ComboBox 支持大號（40px）、默認（32px）、小號（24px），也支持通過 Custom 配合本地尺寸屬性自定義。";
    public const string VariantsTitle = "不同形態";
    public const string VariantsDescription = "輸入框的不同形態。";
    public const string PrePostTabTitle = "前置/後置標籤";
    public const string PrePostTabDescription = "前置和後置標籤的使用示例。";
    public const string PrefixSuffixTitle = "前綴和後綴";
    public const string PrefixSuffixDescription = "在輸入框內部添加前綴或後綴圖標。";
    public const string StatusTitle = "狀態";
    public const string StatusDescription = "通過 status 為輸入框添加錯誤或警告狀態。";
    public const string P2PlaceholderTextPleaseSelect = "請選擇";
    public const string P2PlaceholderTextTypeToFilter = "輸入內容過濾";
    public const string P2PlaceholderSizeTypeLarge = "SizeType：Large";
    public const string P2PlaceholderSizeTypeMiddle = "SizeType：Middle";
    public const string P2PlaceholderSizeTypeSmall = "SizeType：Small";
    public const string P2PlaceholderSizeTypeCustom = "SizeType：Custom";
    public const string P2ContentPoemLine1 = "床前明月光";
    public const string P2ContentPoemLine2 = "疑是地上霜";
    public const string P2ContentPoemLine3 = "舉頭望明月";
    public const string P2ContentPoemLine4 = "低頭思故鄉";

    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertyItemsSource = "用於生成彈出項的集合。";
    public const string ApiPropertySelectedItem = "當前選中項。";
    public const string ApiPropertySelectedIndex = "當前選中項索引。";
    public const string ApiPropertyPlaceholderText = "未選擇內容時顯示的佔位文本。";
    public const string ApiPropertyIsEditable = "允許在 ComboBox 選擇區域輸入文本。";
    public const string ApiPropertyText = "可編輯輸入文本；啟用過濾時會驅動 FilterValue。";
    public const string ApiPropertyIsFilterEnabled = "為可編輯 ComboBox 候選項啟用過濾。";
    public const string ApiPropertyFilter = "用於將候選值與 FilterValue 匹配的過濾謂詞。";
    public const string ApiPropertyFilterValue = "當前過濾值，通常在可編輯過濾模式下由 Text 同步。";
    public const string ApiPropertyFilterValueSelector = "過濾前從每個候選項中提取可比較值。";
    public const string ApiPropertyLeftAddOn = "顯示在輸入區域外左側的內容。";
    public const string ApiPropertyRightAddOn = "顯示在輸入區域外右側的內容。";
    public const string ApiPropertyContentLeftAddOn = "顯示在輸入區域內選中內容前方的內容。";
    public const string ApiPropertyContentRightAddOn = "顯示在輸入區域內選中內容後方的內容。";
    public const string ApiPropertySizeType = "控制 ComboBox 尺寸。";
    public const string ApiPropertyStyleVariant = "控制描邊、填充和無邊框視覺形態。";
    public const string ApiPropertyStatus = "應用校驗狀態樣式。";
    public const string ApiPropertyIsAllowClear = "允許清除當前選擇。";
    public const string ApiPropertyOptionFontSize = "覆蓋彈出選項字號。";
    public const string ApiPropertyDropDownDisplayPageSize = "控制出現滾動前展示的選項數量。";
    public const string ApiPropertyShouldUseOverlayPopup = "控制下拉是否使用 Overlay Popup 宿主。";
    public const string ApiPropertyIsMotionEnabled = "在主題允許時啟用控件動效。";

    public const string TokenColumnToken = "令牌";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "範圍";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNameControlWidth = "繼承自 ButtonSpinner 的默認控件寬度。";
    public const string TokenNameHandleWidth = "下拉操作柄寬度。";
    public const string TokenNameHandleIconSize = "下拉操作柄圖標尺寸。";
    public const string TokenNameHandleBg = "下拉操作柄背景色。";
    public const string TokenNameHandleActiveBg = "下拉操作柄激活背景色。";
    public const string TokenNameHandleHoverColor = "下拉操作柄懸浮前景色。";
    public const string TokenNameHandleBorderColor = "下拉操作柄邊框色。";
    public const string TokenNameFilledHandleBg = "填充形態下的下拉操作柄背景色。";
    public const string TokenNameInputFontSize = "繼承自 LineEdit 的默認輸入字號。";
    public const string TokenNameInputFontSizeLG = "繼承自 LineEdit 的大號輸入字號。";
    public const string TokenNameInputFontSizeSM = "繼承自 LineEdit 的小號輸入字號。";
    public const string TokenNamePopupContentPadding = "下拉彈出內容內邊距。";
    public const string TokenNameItemColor = "選項文本色。";
    public const string TokenNameItemHoverColor = "選項懸浮文本色。";
    public const string TokenNameItemSelectedColor = "選中選項文本色。";
    public const string TokenNameItemDisabledColor = "禁用選項文本色。";
    public const string TokenNameItemBgColor = "選項背景色。";
    public const string TokenNameItemHoverBgColor = "選項懸浮背景色。";
    public const string TokenNameItemSelectedBgColor = "選中選項背景色。";
    public const string TokenNameItemPadding = "選項內容內邊距。";
    public const string TokenNameItemMargin = "選項外邊距。";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusStable = "穩定";

    protected override Type GetResourceKindType() => typeof(ComboBoxShowCaseLangResourceKind);
}
