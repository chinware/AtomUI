using System.Drawing;

namespace AtomUI.Controls;

public partial class DataGrid
{
    #region 内部属性定义

    // When the RowsPresenter's width increases, the HorizontalOffset will be incorrect until
    // the scrollbar's layout is recalculated, which doesn't occur until after the cells are measured.
    // This property exists to account for this scenario, and avoid collapsing the incorrect cells.
    internal double HorizontalAdjustment
    {
        get;
        private set;
    }
    
    private Size? _rowsPresenterAvailableSize;
    internal Size? RowsPresenterAvailableSize
    {
        get
        {
            return _rowsPresenterAvailableSize;
        }
        set
        {
            if (_rowsPresenterAvailableSize.HasValue && value.HasValue && value.Value.Width > RowsPresenterAvailableSize.Value.Width)
            {
                // When the available cells width increases, the horizontal offset can be incorrect.
                // Store away an adjustment to use during the CellsPresenter's measure, so that the
                // ShouldDisplayCell method correctly determines if a cell will be in view.
                //
                //     |   h. offset   |       new available cells width          |
                //     |-------------->|----------------------------------------->|
                //      __________________________________________________        |
                //     |           |           |             |            |       |
                //     |  column0  |  column1  |   column2   |  column3   |<----->|
                //     |           |           |             |            |  adj. |
                //
                double adjustment = (_horizontalOffset + value.Value.Width) - ColumnsInternal.VisibleEdgedColumnsWidth;
                HorizontalAdjustment = Math.Min(HorizontalOffset, Math.Max(0, adjustment));
            }
            else
            {
                HorizontalAdjustment = 0;
            }
            _rowsPresenterAvailableSize = value;
        }
    }

    internal double ActualRowHeaderWidth
    {
        get
        {
            if (!AreRowHeadersVisible)
            {
                return 0;
            }
            return !double.IsNaN(RowHeaderWidth) ? RowHeaderWidth : RowHeadersDesiredWidth;
        }
    }

    internal bool AreRowHeadersVisible =>
        (HeadersVisibility & DataGridHeadersVisibility.Row) == DataGridHeadersVisibility.Row;
    
    internal double RowHeadersDesiredWidth
    {
        get => _rowHeaderDesiredWidth;
        set
        {
            // We only auto grow
            if (_rowHeaderDesiredWidth < value)
            {
                double oldActualRowHeaderWidth = ActualRowHeaderWidth;
                _rowHeaderDesiredWidth = value;
                if (oldActualRowHeaderWidth != ActualRowHeaderWidth)
                {
                    EnsureRowHeaderWidth();
                }
            }
        }
    }
    #endregion
}