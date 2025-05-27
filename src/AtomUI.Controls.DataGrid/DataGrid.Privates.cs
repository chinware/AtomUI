namespace AtomUI.Controls;

public partial class DataGrid
{
    internal const bool DefaultCanUserReorderColumns = true;
    internal const bool DefaultCanUserResizeColumns = true;
    internal const bool DefaultCanUserSortColumns = true;

    /// <summary>
    /// The default order to use for columns when there is no <see cref="DisplayAttribute.Order"/>
    /// value available for the property.
    /// </summary>
    /// <remarks>
    /// The value of 10,000 comes from the DataAnnotations spec, allowing
    /// some properties to be ordered at the beginning and some at the end.
    /// </remarks>
    private const int DefaultColumnDisplayOrder = 10000;
    private const double HorizontalGridLinesThickness = 1;
    private const double MinimumRowHeaderWidth = 4;
    private const double MinimumColumnHeaderHeight = 4;
    internal const double MaximumStarColumnWidth = 10000;
    internal const double MinimumStarColumnWidth = 0.001;
    private const double MouseWheelDelta = 50.0;
    private const double MaxHeadersThickness = 32768;
    private const double DefaultRowHeight = 22;
    internal const double DefaultRowGroupSublevelIndent = 20;
    private const double DefaultMinColumnWidth = 20;
    private const double DefaultMaxColumnWidth = double.PositiveInfinity;
    
    private int _noCurrentCellChangeCount;
    internal int NoCurrentCellChangeCount
    {
        get => _noCurrentCellChangeCount;
        set
        {
            _noCurrentCellChangeCount = value;
            if (value == 0)
            {
                FlushCurrentCellChanged();
            }
        }
    }
    
    private void FlushCurrentCellChanged()
    {
        // if (_makeFirstDisplayedCellCurrentCellPending)
        // {
        //     return;
        // }
        // if (SelectionHasChanged)
        // {
        //     // selection is changing, don't raise CurrentCellChanged until it's done
        //     _flushCurrentCellChanged = true;
        //     FlushSelectionChanged();
        //     return;
        // }
        //
        // // We don't want to expand all intermediate currency positions, so we only expand
        // // the last current item before we flush the event
        // if (_collapsedSlotsTable.Contains(CurrentSlot))
        // {
        //     DataGridRowGroupInfo rowGroupInfo = RowGroupHeadersTable.GetValueAt(RowGroupHeadersTable.GetPreviousIndex(CurrentSlot));
        //     Debug.Assert(rowGroupInfo != null);
        //     if (rowGroupInfo != null)
        //     {
        //         ExpandRowGroupParentChain(rowGroupInfo.Level, rowGroupInfo.Slot);
        //     }
        // }
        //
        // if (CurrentColumn != _previousCurrentColumn
        //     || CurrentItem != _previousCurrentItem)
        // {
        //     CoerceSelectedItem();
        //     _previousCurrentColumn = CurrentColumn;
        //     _previousCurrentItem   = CurrentItem;
        //
        //     OnCurrentCellChanged(EventArgs.Empty);
        // }
        //
        // _flushCurrentCellChanged = false;
    }
}