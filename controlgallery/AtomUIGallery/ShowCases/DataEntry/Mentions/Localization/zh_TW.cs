using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Mentions;

[LanguageProvider(LanguageCode.zh_TW, MentionsShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最簡單的用法。";
    public const string SizeTypeTitle = "Mentions 尺寸";
    public const string SizeTypeDescription = "Mentions 支持大號、中號、小號，也支持通過 Custom 配合本地高度和字號自定義。";
    public const string VariantsTitle = "變體";
    public const string VariantsDescription = "Mentions 提供四種變體：描邊、填充、無邊框和下划線。";
    public const string AsynchronousLoadingTitle = "異步加載";
    public const string AsynchronousLoadingDescription = "異步加載。";
    public const string CustomizeTriggerTokenTitle = "自定義觸發標記";
    public const string CustomizeTriggerTokenDescription = "通過 prefix 屬性自定義觸發標記，默認為 @，也支持數組。";
    public const string DisabledOrReadOnlyTitle = "禁用或只讀";
    public const string DisabledOrReadOnlyDescription = "配置 disabled 和 readOnly。";
    public const string PlacementTitle = "彈出位置";
    public const string PlacementDescription = "改變建議列表的彈出位置。";
    public const string StatusTitle = "狀態";
    public const string StatusDescription = "通過 status 為 Mentions 添加狀態，可設置為錯誤或警告。";
    public const string AutoSizeTitle = "自動高度";
    public const string AutoSizeDescription = "高度自動調整。";
    public const string WithClearIconTitle = "帶清除圖標";
    public const string WithClearIconDescription = "自定義清除按鈕。";
    public const string P2PlaceholderTextLarge = "大號";
    public const string P2PlaceholderTextMiddle = "中號";
    public const string P2PlaceholderTextSmall = "小號";
    public const string P2PlaceholderTextCustom = "自定義";
    public const string P2PlaceholderTextOutlined = "線框風格";
    public const string P2PlaceholderTextFilled = "填充風格";
    public const string P2PlaceholderTextBorderless = "無邊框";
    public const string P2PlaceholderTextUnderlined = "下划線";
    public const string P2PlaceholderTextInputToMentionPeopleToMentionTag = "輸入 @ 提及成員，輸入 # 提及標籤";
    public const string P2PlaceholderTextThisIsDisabledMentions = "這是禁用狀態的 Mentions";
    public const string P2PlaceholderTextThisIsReadonlyMentions = "這是只讀狀態的 Mentions";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計 Token";
    public const string PageSubtitle = "在輸入過程中提及成員、標籤或自定義實體。";
    public const string PageDescription =
        "Mentions 提供基於觸發符的候選彈窗、異步選項加載、自定義觸發符、輸入變體、彈出位置、狀態和自動高度場景。";
    public const string ComponentCategory = "數據錄入";
    public const string ComponentStatusStable = "穩定";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertyValue = "Mentions 輸入框的當前文本值。";
    public const string ApiPropertyDefaultValue = "控件創建時應用的初始文本值。";
    public const string ApiPropertyOptionsSource = "用於提及建議的靜態選項數據源。";
    public const string ApiPropertyOptionsAsyncLoader = "根據當前提及上下文提供選項的異步加載器。";
    public const string ApiPropertyOptionTemplate = "用於渲染每一個建議選項的模板。";
    public const string ApiPropertyTriggerPrefix = "打開候選列表的觸發符。";
    public const string ApiPropertySplit = "選擇提及選項後插入的分隔文本。";
    public const string ApiPropertyIsAllowClear = "當輸入有內容時顯示清除入口。";
    public const string ApiPropertyClearIcon = "清除入口使用的自定義圖標。";
    public const string ApiPropertyStyleVariant = "輸入框視覺變體，例如描邊、填充、無邊框或下划線。";
    public const string ApiPropertyStatus = "Mentions 表面顯示的校驗狀態。";
    public const string ApiPropertyPlacement = "候選列表彈窗的首選彈出位置。";
    public const string ApiPropertyIsAutoSize = "允許輸入框高度隨文本內容增長。";
    public const string ApiPropertyLines = "初始可見文本行數。";
    public const string ApiPropertyMinLines = "自動高度使用的最小可見文本行數。";
    public const string ApiPropertyMaxLines = "自動高度使用的最大可見文本行數。";
    public const string ApiPropertyDisplayCandidateCount = "用於計算彈窗高度的候選行數。";
    public const string ApiPropertyIsReadOnly = "禁止編輯，同時保留內容可讀。";
    public const string ApiPropertyAsyncLoadDebounce = "調用異步選項加載器前的延遲時間。";
    public const string ApiPropertyAsyncLoadTimeout = "異步選項加載的最大等待時間。";
    public const string ApiPropertyShouldUseOverlayPopup = "打開建議時使用 overlay popup 宿主。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNamePopupContentPadding = "候選彈窗內部邊距。";
    public const string TokenNameOptionHeight = "每個建議選項的高度。";
    public const string TokenNameMinPopupWidth = "候選彈窗最小寬度。";
    public const string TokenScopeComponent = "Mentions";
    public const string TokenStatusStable = "穩定";

}
