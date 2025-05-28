using System.Diagnostics;
using Avalonia.Controls;

namespace AtomUI.Controls;

public partial class DataGrid
{
    #region 内部属性定义
    
    // Width currently available for cells this value is smaller.  This width is reduced by the existence of RowHeaders
    // or a vertical scrollbar.  Layout is asynchronous so changes to the RowHeaders or the vertical scrollbar are
    // not reflected immediately
    internal double CellsWidth
    {
        get
        {
            double rowsWidth = double.PositiveInfinity;
            if (RowsPresenterAvailableSize.HasValue)
            {
                rowsWidth = Math.Max(0, RowsPresenterAvailableSize.Value.Width - ActualRowHeaderWidth);
            }
            return double.IsPositiveInfinity(rowsWidth) ? ColumnsInternal.VisibleEdgedColumnsWidth : rowsWidth;
        }
    }
    
    internal int CurrentSlot
    {
        get => CurrentCellCoordinates.Slot;
        private set => CurrentCellCoordinates.Slot = value;
    }
    
    internal int SlotCount { get; private set; }
    internal double CellsEstimatedHeight => RowsPresenterAvailableSize?.Height ?? 0;
    
    private bool IsHorizontalScrollBarOverCells => _columnHeadersPresenter != null && Grid.GetColumnSpan(_columnHeadersPresenter) == 2;

    private bool IsVerticalScrollBarOverCells => _rowsPresenter != null && Grid.GetRowSpan(_rowsPresenter) == 2;
    
    internal int VisibleSlotCount { get; set; }
    
    internal double AvailableSlotElementRoom { get; set; }
    
    #endregion
    
    internal static DataGridCell? GetOwningCell(Control element)
    {
        Debug.Assert(element != null);
        DataGridCell? cell   = element as DataGridCell;
        Control?      target = element;
        while (target != null && cell == null)
        {
            target = target.Parent as Control;
            cell   = target as DataGridCell;
        }
        return cell;
    }
}