using AtomUI.Theme.TokenSystem;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls.DesignTokens
{
    public enum DataGridTokenKind
    {
        BodySortBg,
        BorderColor,
        CellFontSize,
        CellFontSizeMD,
        CellFontSizeSM,
        CellPadding,
        CellPaddingMD,
        CellPaddingSM,
        ColumnReorderActiveBg,
        ExpandIconBg,
        ExpandIconHalfInner,
        ExpandIconMargin,
        ExpandIconScale,
        ExpandIconSize,
        FilterDropdownBg,
        FilterDropdownMenuBg,
        FilterIndicatorPadding,
        FixedHeaderSortActiveBg,
        FooterBg,
        FooterColor,
        HeaderBg,
        HeaderBorderRadius,
        HeaderColor,
        HeaderFilterHoverBg,
        HeaderIconColor,
        HeaderIconHoverColor,
        HeaderSortActiveBg,
        HeaderSortHoverBg,
        HeaderSplitColor,
        LeftFrozenShadows,
        PaginationMargin,
        PaginationMarginSM,
        RightFrozenShadows,
        RowExpandedBg,
        RowHoverBg,
        RowReorderIndicatorSize,
        RowSelectedBg,
        RowSelectedHoverBg,
        SelectionColumnWidth,
        SortIconSize,
        SortIndicatorLayoutMargin,
        TableBg,
        TableBodySortBg,
        TableBorderColor,
        TableExpandColumnWidth,
        TableExpandedRowBg,
        TableExpandIconBg,
        TableFilterButtonContainerMargin,
        TableFilterButtonLayoutSeparatorMargin,
        TableFilterButtonSpacing,
        TableFilterDropdownBg,
        TableFilterDropdownHeight,
        TableFilterDropdownPadding,
        TableFilterDropdownSearchWidth,
        TableFilterDropdownWidth,
        TableFixedHeaderSortActiveBg,
        TableFontSize,
        TableFontSizeMiddle,
        TableFontSizeSmall,
        TableFooterBg,
        TableFooterTextColor,
        TableHeaderBg,
        TableHeaderCellSplitColor,
        TableHeaderFilterActiveBg,
        TableHeaderSortBg,
        TableHeaderSortHoverBg,
        TableHeaderTextColor,
        TablePadding,
        TablePaddingMiddle,
        TablePaddingSmall,
        TableRadius,
        TableRowHoverBg,
        TableSelectedRowBg,
        TableSelectedRowHoverBg,
        TableSelectionColumnWidth,
        TableTopLeftColumnCornerRadius
    }

    public class DataGridTokenResourceExtension : TokenResourceExtension<DataGridTokenKind>
    {
        public DataGridTokenResourceExtension()
        {
        }

        public DataGridTokenResourceExtension(DataGridTokenKind kind) : base(kind)
        {
        }
    }
}