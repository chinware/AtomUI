using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Masonry;

[LanguageProvider(LanguageCode.zh_CN, MasonryShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string ScenarioExamples = "示例";
    public const string PageSubtitle = "将高度不一的子元素按列组织成均衡的瀑布流。";
    public const string PageDescription =
        "Masonry 把卡片、图片或任意控件按最短列策略排列成瀑布流，支持固定列数或自适应列数以及行列间距。";
    public const string ComponentCategory = "布局";
    public const string ComponentStatusStable = "稳定";
    public const string ApiEventLayoutChanged = "当子项有效列分配变化时触发，延迟到布局周期结束后派发。";

    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "基础用法展示。通过 ColumnCount 设置列数，ColumnGap 和 RowGap 设置间距。";
    public const string ResponsiveTitle = "响应式";
    public const string ResponsiveDescription = "使用响应式参数来适配不同屏幕宽度。ColumnInfo 可以设置在不同断点下的列数，Gutter 可以设置不同断点下的间距大小。";
    public const string ImageTitle = "图片";
    public const string ImageDescription = "随加载动态调整位置。";
    public const string DynamicTitle = "动态更新";
    public const string DynamicDescription = "展示瀑布流动态更新的效果，配合 item.column 固化位置。";
    public const string DynamicAddItemLabel = "Add Item";

}
