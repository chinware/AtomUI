using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Pagination;

[LanguageProvider(LanguageCode.zh_TW, PaginationShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string ComponentCategory = "導航";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "用頁碼、頁大小、總數和快速跳轉瀏覽長列表。";
    public const string PageDescription = "Pagination 將大型數據集拆分為可預測的頁面，支持對齊方式、頁大小選擇、快速跳轉、總數信息、迷你尺寸以及簡潔只讀或可編輯模式。";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertyTotal = "所有頁面中的記錄總數。";
    public const string ApiPropertyCurrentPage = "當前頁碼，從 1 開始。";
    public const string ApiPropertyPageSize = "每頁表示的記錄數量。";
    public const string ApiPropertyPageCount = "根據總數和頁大小計算出的頁數。";
    public const string ApiPropertyIsHideOnSinglePage = "當所有記錄能放入單頁時隱藏分頁。";
    public const string ApiPropertyAlign = "將分頁內容對齊到起始、中間或末尾。";
    public const string ApiPropertySizeType = "控制默認或小號分頁密度。";
    public const string ApiPropertyIsMotionEnabled = "啟用或禁用動效過渡。";
    public const string ApiPropertyCurrentPageChanged = "當前頁變化時觸發。";
    public const string ApiPropertyIsShowSizeChanger = "顯示頁大小選擇器。";
    public const string ApiPropertyIsShowQuickJumper = "顯示快速跳轉輸入框。";
    public const string ApiPropertyIsShowTotalInfo = "顯示總記錄信息。";
    public const string ApiPropertyTotalInfoTemplate = "用於格式化總數信息文本的模板。";
    public const string ApiPropertyIsReadOnly = "控制 SimplePagination 使用只讀展示還是可編輯跳轉輸入。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "範圍";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNameItemBg = "分頁項背景色。";
    public const string TokenNameItemSize = "默認分頁項尺寸。";
    public const string TokenNameItemActiveBg = "激活分頁項背景色。";
    public const string TokenNameItemSizeSM = "小號分頁項尺寸。";
    public const string TokenNameItemLinkBg = "分頁項鏈接背景色。";
    public const string TokenNameItemActiveBgDisabled = "禁用激活分頁項背景色。";
    public const string TokenNameItemActiveColorDisabled = "禁用激活分頁項文本色。";
    public const string TokenNameItemInputBg = "分頁輸入控件背景色。";
    public const string TokenNameInputOutlineOffset = "分頁輸入控件輪廓偏移量。";
    public const string TokenNamePaginationLayoutSpacing = "普通分頁布局水平間距。";
    public const string TokenNamePaginationLayoutMiniSpacing = "迷你分頁布局水平間距。";
    public const string TokenNamePaginationQuickJumperInputWidth = "普通快速跳轉輸入框寬度。";
    public const string TokenNamePaginationMiniQuickJumperInputWidth = "迷你快速跳轉輸入框寬度。";
    public const string TokenNamePaginationItemPaddingInline = "分頁項橫向內間距。";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusStable = "穩定";
    public const string BasicTitle = "基礎分頁";
    public const string BasicDescription = "基礎分頁。";
    public const string AlignTitle = "對齊方式";
    public const string AlignDescription = "支持左對齊、居中對齊和右對齊三種對齊方式。";
    public const string MoreTitle = "更多頁碼";
    public const string MoreDescription = "更多頁碼。";
    public const string MiniSizeTitle = "迷你尺寸";
    public const string MiniSizeDescription = "迷你尺寸分頁。";
    public const string TotalNumberTitle = "總數";
    public const string TotalNumberDescription = "可以通過設置 showTotal 展示數據總量。";
    public const string SimpleModeTitle = "簡潔模式";
    public const string SimpleModeDescription = "簡潔模式。";

    protected override Type GetResourceKindType() => typeof(PaginationShowCaseLangResourceKind);
}
