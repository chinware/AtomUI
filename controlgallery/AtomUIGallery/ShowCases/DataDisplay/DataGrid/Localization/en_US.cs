using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.DataGrid;

[LanguageProvider(LanguageCode.en_US, DataGridShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Simple table with actions.";
    public const string SelectionTitle = "Selection";
    public const string SelectionDescription = "Rows can be selectable by making first column as a selectable column. You can use rowSelection.type to set selection type. Default is checkbox.";
    public const string DragResizeColumnTitle = "Drag resize column";
    public const string DragResizeColumnDescription = "You can drag to change the size of the column.";
    public const string SizeTitle = "Size";
    public const string SizeDescription = "There are two compacted table sizes: middle and small. The small size is used in Modals only.";
    public const string TableBorderTitleAndFooterTitle = "border, title and footer";
    public const string TableBorderTitleAndFooterDescription = "Add border, title and footer for table.";
    public const string FilterAndSorterTitle = "Filter and sorter";
    public const string FilterAndSorterDescription = "Use filters to generate filter menu in columns, onFilter to determine filtered result, and filterMultiple to indicate whether it's multiple or single selection, filterOnClose to specify whether to trigger filter when the filter menu closes. Use defaultFilteredValue to make a column filtered by default.";
    public const string FilterInTreeTitle = "Filter in Tree";
    public const string FilterInTreeDescription = "You can use filterMode to change default filter interface, options: menu(default) and tree.";
    public const string MultipleSorterTitle = "Multiple sorter";
    public const string MultipleSorterDescription = "column.sorter support multiple to config the priority of sort columns. Though sorter.compare to customize compare function. You can also leave it empty to use the interactive only.";
    public const string ResetFiltersAndSortersTitle = "Reset filters and sorters";
    public const string ResetFiltersAndSortersDescription = "Control filters and sorters by API.";
    public const string ExpandableRowTitle = "Expandable Row";
    public const string ExpandableRowDescription = "When there's too much information to show and the table can't display all at once.";
    public const string OrderSpecificColumnTitle = "Order Specific Column";
    public const string OrderSpecificColumnDescription = "You can control the order of the expand and select columns.";
    public const string RowHeaderTitle = "Row Header";
    public const string RowHeaderDescription = "You can control whether to display the Column header and RowHeader.";
    public const string GroupingTableHeadTitle = "Grouping table head";
    public const string GroupingTableHeadDescription = "Group table head with columns[n].children.";
    public const string HiddenColumnsTitle = "Hidden Columns";
    public const string HiddenColumnsDescription = "Hide columns with hidden.";
    public const string FixedHeaderTitle = "Fixed Header";
    public const string FixedHeaderDescription = "Display large amounts of data in scrollable view.";
    public const string FixedColumnsTitle = "Fixed Columns";
    public const string FixedColumnsDescription = "When there are too many columns, we can make some columns fixed.";
    public const string FixedColumnsAndHeadersTitle = "Fixed Columns And Headers";
    public const string FixedColumnsAndHeadersDescription = "When there are too many columns, we can make some columns fixed.";
    public const string DragColumnSortingTitle = "Drag Column sorting";
    public const string DragColumnSortingDescription = "Drag the Header to sort the columns.";
    public const string DragSortingWithHandlerTitle = "Drag sorting with handler";
    public const string DragSortingWithHandlerDescription = "You can drag the row indicator column to sort the rows.";
    public const string CustomEmptyAndLoadingTitle = "Custom empty and loading";
    public const string CustomEmptyAndLoadingDescription = "Custom empty status and loading.";
    public const string EditableCellsTitle = "Editable Cells";
    public const string EditableCellsDescription = "Table with editable cells.";
    public const string BasicPagingTitle = "Basic Paging";
    public const string BasicPagingDescription = "Basic pagination table.";

    protected override Type GetResourceKindType() => typeof(DataGridShowCaseLangResourceKind);
}
