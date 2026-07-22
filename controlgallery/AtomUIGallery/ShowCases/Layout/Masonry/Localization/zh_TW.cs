using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Masonry;

[LanguageProvider(LanguageCode.zh_TW, MasonryShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string ScenarioExamples = "範例";
    public const string PageSubtitle = "將高度不一的子元素按列組織成均衡的瀑布流。";
    public const string PageDescription =
        "Masonry 把卡片、圖片或任意控件按最短列策略排列成瀑布流，支援固定列數或自適應列數以及行列間距。";
    public const string ComponentCategory = "佈局";
    public const string ComponentStatusStable = "穩定";
    public const string ApiEventLayoutChanged = "當子項有效列分配變化時觸發，延遲到佈局週期結束後派發。";

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
