using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Transfer;

[LanguageProvider(LanguageCode.zh_TW, TransferShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "Transfer 的基礎用法需要提供源數據、目標 keys 數組，以及渲染和部分回調函數。";
    public const string ScenarioBasic = "基礎";
    public const string ScenarioAdvanced = "高級";
    public const string ScenarioTreeStatus = "樹與狀態";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計 Token";
    public const string PageSubtitle = "在兩個集合之間移動條目，支持搜索、分頁和樹形數據。";
    public const string PageDescription = "Transfer 在左側展示候選條目，在右側展示已選條目。它支持單向移動、篩選、分頁、自定義條目模板和基於樹的數據源。";
    public const string ComponentCategory = "數據錄入";
    public const string ComponentStatusStable = "穩定";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertyItemsSource = "穿梭列表展示的數據集合。";
    public const string ApiPropertyTargetKeys = "當前位於目標列表中的條目 key。預設雙向綁定，並響應集合原地變更。";
    public const string ApiPropertySelectedKeys = "當前在源列表和目標列表中被選中的條目 key。預設雙向綁定，並響應集合原地變更。";
    public const string ApiPropertySourceTitle = "源列表上方顯示的標題。";
    public const string ApiPropertyTargetTitle = "目標列表上方顯示的標題。";
    public const string ApiPropertyIsOneWay = "僅允許條目從源列表移動到目標列表。";
    public const string ApiPropertyIsFilterEnabled = "顯示用於篩選穿梭條目的搜索框。";
    public const string ApiPropertyFilterPlaceholderText = "篩選輸入框中顯示的佔位文本。";
    public const string ApiPropertyFilterValueSelector = "選擇默認篩選邏輯使用的文本。";
    public const string ApiPropertyListWidth = "每個穿梭列表的寬度。";
    public const string ApiPropertyListHeight = "每個穿梭列表的高度。";
    public const string ApiPropertyPageSize = "每頁展示的穿梭條目數量。";
    public const string ApiPropertyStatus = "穿梭框表面的校驗狀態樣式。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "範圍";
    public const string TokenColumnStatus = "狀態";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusStable = "穩定";
    public const string TokenNameListWidth = "穿梭列表默認寬度。";
    public const string TokenNameListWidthLG = "大尺寸穿梭列表寬度。";
    public const string TokenNameListHeight = "穿梭列表默認高度。";
    public const string TokenNameItemHeight = "每個穿梭條目行的高度。";
    public const string TokenNameItemPadding = "每個穿梭條目行的垂直內邊距。";
    public const string TokenNameHeaderHeight = "穿梭列表頭部高度。";
    public const string TokenNameHeaderPadding = "穿梭列表頭部內邊距。";
    public const string TokenNamePaginationMargin = "穿梭分頁區域外邊距。";
    public const string TokenNameDataGridSelectionHeaderMargin = "DataGrid 穿梭選擇頭部使用的外邊距。";
    public const string OneWayTitle = "單向模式";
    public const string OneWayDescription = "使用 oneWay 讓 Transfer 呈現單向樣式。";
    public const string SearchTitle = "搜索";
    public const string SearchDescription = "帶搜索框的 Transfer。";
    public const string ControlledKeysTitle = "受控 key";
    public const string ControlledKeysDescription = "將 TargetKeys 和 SelectedKeys 綁定到 ObservableCollection。外部集合變更和 Transfer 交互會保持同步。";
    public const string AdvancedTitle = "高級用法";
    public const string AdvancedDescription = "Transfer 的高級用法。可以自定義穿梭按鈕標籤、列寬和列高，以及頁腳中展示的內容。";
    public const string PaginationTitle = "分頁";
    public const string PaginationDescription = "通過分頁承載大量條目。";
    public const string TreeTransferTitle = "樹形穿梭框";
    public const string TreeTransferDescription = "使用 Tree 組件自定義渲染列表。";
    public const string StatusTitle = "狀態";
    public const string StatusDescription = "通過 status 為 Transfer 添加狀態，可設置為錯誤或警告。";
    public const string P2SourceTitle = "源列表";
    public const string P2TargetTitle = "目標列表";
    public const string P2TextText = "-";
    public const string P2HeaderName = "姓名";
    public const string P2HeaderTag = "標籤";
    public const string P2HeaderDescription = "描述";
    public const string P2ContentLeftButtonReload = "重新加載左側";
    public const string P2ContentRightButtonReload = "重新加載右側";
    public const string P2ContentAddTargetKey = "添加 key 3 到目標";
    public const string P2ContentClearTargetKeys = "清空目標 key";
    public const string P2ContentSelectSourceKey = "選中 key 4";
    public const string P2TargetKeysCountLabel = "目標 key：";
    public const string P2SelectedKeysCountLabel = "選中 key：";

    public const string P2OnContentDisable = "禁用";

    public const string P2OffContentEnable = "啓用";

    public const string P2FilterPlaceholderTextSearchHere = "在此搜索";

    public const string P2ToSourceButtonTextToLeft = "移到左側";

    public const string P2ToTargetButtonTextToRight = "移到右側";

    public const string P2OnContentOnyWay = "單向";

    public const string P2OffContentOnyWay = "單向";
    public const string P2ItemContentFormat = "內容{0}";
    public const string P2ItemDescriptionFormat = "內容{0}的描述";
    public const string P2TagCat = "貓";
    public const string P2TagDog = "狗";
    public const string P2TagBird = "鳥";

}
