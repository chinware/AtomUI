using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.DataGrid;

[LanguageProvider(LanguageCode.zh_CN, DataGridShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础表格";
    public const string BasicDescription = "带操作列的简单表格。";
    public const string SelectionTitle = "选择";
    public const string SelectionDescription = "通过将第一列设置为可选择列，可以让行支持选择。可使用 rowSelection.type 设置选择类型，默认是 checkbox。";
    public const string DragResizeColumnTitle = "拖拽调整列宽";
    public const string DragResizeColumnDescription = "可以通过拖拽改变列宽。";
    public const string SizeTitle = "尺寸";
    public const string SizeDescription = "表格提供 middle 和 small 两种紧凑尺寸，其中 small 尺寸仅用于模态框中。";
    public const string TableBorderTitleAndFooterTitle = "边框、标题和页脚";
    public const string TableBorderTitleAndFooterDescription = "为表格添加边框、标题和页脚。";
    public const string FilterAndSorterTitle = "筛选和排序";
    public const string FilterAndSorterDescription = "使用 filters 生成列筛选菜单，onFilter 判断筛选结果，filterMultiple 表示单选或多选，filterOnClose 指定筛选菜单关闭时是否触发筛选。使用 defaultFilteredValue 可以让列默认处于筛选状态。";
    public const string FilterInTreeTitle = "树形筛选";
    public const string FilterInTreeDescription = "可以使用 filterMode 改变默认筛选界面，可选 menu（默认）和 tree。";
    public const string MultipleSorterTitle = "多列排序";
    public const string MultipleSorterDescription = "column.sorter 支持通过 multiple 配置排序优先级。可以通过 sorter.compare 自定义比较函数，也可以留空只使用交互排序。";
    public const string ResetFiltersAndSortersTitle = "重置筛选和排序";
    public const string ResetFiltersAndSortersDescription = "通过 API 控制筛选和排序。";
    public const string ExpandableRowTitle = "可展开行";
    public const string ExpandableRowDescription = "当需要展示的信息过多，表格无法一次性完整展示时使用。";
    public const string OrderSpecificColumnTitle = "指定列顺序";
    public const string OrderSpecificColumnDescription = "可以控制展开列和选择列的顺序。";
    public const string RowHeaderTitle = "行头";
    public const string RowHeaderDescription = "可以控制是否显示列头和行头。";
    public const string GroupingTableHeadTitle = "分组表头";
    public const string GroupingTableHeadDescription = "使用 columns[n].children 对表头进行分组。";
    public const string HiddenColumnsTitle = "隐藏列";
    public const string HiddenColumnsDescription = "使用 hidden 隐藏列。";
    public const string FixedHeaderTitle = "固定表头";
    public const string FixedHeaderDescription = "在可滚动视图中展示大量数据。";
    public const string FixedColumnsTitle = "固定列";
    public const string FixedColumnsDescription = "当列过多时，可以固定部分列。";
    public const string FixedColumnsAndHeadersTitle = "固定列和表头";
    public const string FixedColumnsAndHeadersDescription = "当列过多时，可以固定部分列和表头。";
    public const string DragColumnSortingTitle = "拖拽列排序";
    public const string DragColumnSortingDescription = "拖拽表头可以对列进行排序。";
    public const string DragSortingWithHandlerTitle = "使用拖拽手柄排序";
    public const string DragSortingWithHandlerDescription = "可以拖拽行指示列对行进行排序。";
    public const string CustomEmptyAndLoadingTitle = "自定义空状态和加载状态";
    public const string CustomEmptyAndLoadingDescription = "自定义空状态和加载状态。";
    public const string EditableCellsTitle = "可编辑单元格";
    public const string EditableCellsDescription = "包含可编辑单元格的表格。";
    public const string BasicPagingTitle = "基础分页";
    public const string BasicPagingDescription = "基础分页表格。";

    protected override Type GetResourceKindType() => typeof(DataGridShowCaseLangResourceKind);
}
