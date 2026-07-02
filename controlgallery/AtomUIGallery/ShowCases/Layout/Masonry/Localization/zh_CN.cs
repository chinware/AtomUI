using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Masonry;

[LanguageProvider(LanguageCode.zh_CN, MasonryShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计令牌";
    public const string PageSubtitle = "将高度不一的子元素按列组织成均衡的瀑布流。";
    public const string PageDescription =
        "Masonry 把卡片、图片或任意控件按最短列策略排列成瀑布流，支持固定列数或自适应列数以及行列间距。";
    public const string ComponentCategory = "布局";
    public const string ComponentStatusStable = "稳定";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyColumnCount = "固定列数。大于 0 时优先于自适应计算。";
    public const string ApiPropertyColumnInfo = "响应式列数。当前断点命中配置时优先于 ColumnCount。";
    public const string ApiPropertyMinColumnWidth = "自适应布局中计算列数所用的单列最小目标宽度。";
    public const string ApiPropertyMaxColumnCount = "自适应列数上限，防止宽屏下列数过多。";
    public const string ApiPropertyColumnGap = "列与列之间的水平间距。";
    public const string ApiPropertyRowGap = "行与行之间的垂直间距。";
    public const string ApiPropertyGutter = "响应式水平和垂直间距。当前断点命中配置时优先于 ColumnGap 和 RowGap。";
    public const string ApiPropertyItemsSource = "绑定数据集合，由 ItemsControl 基类为每项生成容器。";
    public const string ApiPropertyItemTemplate = "用于渲染每个绑定数据项的数据模板。";
    public const string ApiPropertyMasonryColumn = "设置在子项容器上的可选附加属性，将子项固定到指定列；为 null 时回退到最短列。";
    public const string ApiPropertyMasonrySpan = "设置在子项容器上的附加属性，控制子项跨列方式。Auto 占一列，Full 占整行宽度。";
    public const string ApiEventLayoutChanged = "当子项有效列分配变化时触发，延迟到布局周期结束后派发。";
    public const string TokenColumnToken = "令牌";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "状态";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusNotApplicable = "不适用";
    public const string TokenNameNoComponentToken = "Masonry 没有组件级设计令牌，它依赖共享的布局与间距令牌。";

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
