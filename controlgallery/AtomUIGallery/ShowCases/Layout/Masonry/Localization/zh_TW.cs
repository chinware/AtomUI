using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Masonry;

[LanguageProvider(LanguageCode.zh_TW, MasonryShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string ScenarioExamples = "範例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計權杖";
    public const string PageSubtitle = "將高度不一的子元素按列組織成均衡的瀑布流。";
    public const string PageDescription =
        "Masonry 把卡片、圖片或任意控件按最短列策略排列成瀑布流，支援固定列數或自適應列數以及行列間距。";
    public const string ComponentCategory = "佈局";
    public const string ComponentStatusStable = "穩定";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "預設值";
    public const string ApiPropertyColumnCount = "固定列數。大於 0 時優先於自適應計算。";
    public const string ApiPropertyColumnInfo = "響應式列數。當前斷點命中配置時優先於 ColumnCount。";
    public const string ApiPropertyMinColumnWidth = "自適應佈局中計算列數所用的單列最小目標寬度。";
    public const string ApiPropertyMaxColumnCount = "自適應列數上限，防止寬螢幕下列數過多。";
    public const string ApiPropertyColumnGap = "列與列之間的水平間距。";
    public const string ApiPropertyRowGap = "行與行之間的垂直間距。";
    public const string ApiPropertyGutter = "響應式水平和垂直間距。當前斷點命中配置時優先於 ColumnGap 和 RowGap。";
    public const string ApiPropertyItemsSource = "綁定資料集合，由 ItemsControl 基底類別為每項產生容器。";
    public const string ApiPropertyItemTemplate = "用於渲染每個綁定資料項的資料範本。";
    public const string ApiPropertyMasonryColumn = "設定在子項容器上的可選附加屬性，將子項固定到指定列；為 null 時退回到最短列。";
    public const string ApiPropertyMasonrySpan = "設定在子項容器上的附加屬性，控制子項跨列方式。Auto 占一列，Full 占整行寬度。";
    public const string ApiEventLayoutChanged = "當子項有效列分配變化時觸發，延遲到佈局週期結束後派發。";
    public const string TokenColumnToken = "權杖";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "狀態";
    public const string TokenScopeComponent = "元件";
    public const string TokenStatusNotApplicable = "不適用";
    public const string TokenNameNoComponentToken = "Masonry 沒有元件級設計權杖，它依賴共享的佈局與間距權杖。";

    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "基礎用法展示。透過 ColumnCount 設定列數，ColumnGap 和 RowGap 設定間距。";
    public const string ResponsiveTitle = "響應式";
    public const string ResponsiveDescription = "使用響應式參數來適配不同螢幕寬度。ColumnInfo 可以設定在不同斷點下的列數，Gutter 可以設定不同斷點下的間距大小。";
    public const string ImageTitle = "圖片";
    public const string ImageDescription = "隨載入動態調整位置。";
    public const string DynamicTitle = "動態更新";
    public const string DynamicDescription = "展示瀑布流動態更新的效果，配合 item.column 固化位置。";
    public const string DynamicAddItemLabel = "Add Item";

}
