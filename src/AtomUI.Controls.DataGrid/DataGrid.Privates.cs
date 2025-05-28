using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using AtomUI.Controls.Data;
using AtomUI.Controls.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public partial class DataGrid
{
    internal const bool DefaultCanUserReorderColumns = true;
    internal const bool DefaultCanUserResizeColumns = true;
    internal const bool DefaultCanUserSortColumns = true;

    #region 内部属性定义

    internal IndexToValueTable<DataGridRowGroupInfo> RowGroupHeadersTable
    {
        get;
        private set;
    }
    
    internal DataGridDisplayData DisplayData
    {
        get;
        private set;
    }
    
    internal double RowHeightEstimate
    {
        get;
        private set;
    }
    
    internal double RowDetailsHeightEstimate
    {
        get;
        private set;
    }
    
    internal DataGridDataConnection DataConnection
    {
        get;
        private set;
    }
    
    internal int AnchorSlot
    {
        get;
        private set;
    }
    
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
    
    private DataGridCellCoordinates CurrentCellCoordinates { get; set; }
    
    internal double RowGroupHeaderHeightEstimate { get; private set; }

    internal bool ContainsFocus { get; private set; }
    
    internal double NegVerticalOffset { get; private set; }
    
    private int NoSelectionChangeCount
    {
        get => _noSelectionChangeCount;
        set
        {
            _noSelectionChangeCount = value;
            if (value == 0)
            {
                FlushSelectionChanged();
            }
        }
    }
    
    // This flag indicates whether selection has actually changed during a selection operation,
    // and exists to ensure that FlushSelectionChanged doesn't unnecessarily raise SelectionChanged.
    internal bool SelectionHasChanged { get; set; }

    /// <summary>
    /// Indicates whether or not to use star-sizing logic.  If the DataGrid has infinite available space,
    /// then star sizing doesn't make sense.  In this case, all star columns grow to a predefined size of
    /// 10,000 pixels in order to show the developer that star columns shouldn't be used.
    /// </summary>
    internal bool UsesStarSizing => ColumnsInternal.VisibleStarColumnCount > 0 &&
                                    (!RowsPresenterAvailableSize.HasValue || !double.IsPositiveInfinity(RowsPresenterAvailableSize.Value.Width));
    
    /// <summary>
    /// Indicates whether or not at least one auto-sizing column is waiting for all the rows
    /// to be measured before its final width is determined.
    /// </summary>
    internal bool AutoSizingColumns
    {
        get => _autoSizingColumns;
        set
        {
            if (_autoSizingColumns && !value)
            {
                double adjustment = CellsWidth - ColumnsInternal.VisibleEdgedColumnsWidth;
                AdjustColumnWidths(0, adjustment, false);
                foreach (DataGridColumn column in ColumnsInternal.GetVisibleColumns())
                {
                    column.IsInitialDesiredWidthDetermined = true;
                }
                ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
                ComputeScrollBarsLayout();
                InvalidateColumnHeadersMeasure();
                InvalidateRowsMeasure(true);
            }
            _autoSizingColumns = value;
        }
    }
    
    internal int? MouseOverRowIndex
    {
        get => _mouseOverRowIndex;
        set
        {
            if (_mouseOverRowIndex != value)
            {
                DataGridRow? oldMouseOverRow = null;
                if (_mouseOverRowIndex.HasValue)
                {
                    int oldSlot = SlotFromRowIndex(_mouseOverRowIndex.Value);
                    if (IsSlotVisible(oldSlot))
                    {
                        oldMouseOverRow = DisplayData.GetDisplayedElement(oldSlot) as DataGridRow;
                    }
                }

                _mouseOverRowIndex = value;

                // State for the old row needs to be applied after setting the new value
                if (oldMouseOverRow != null)
                {
                    oldMouseOverRow.ApplyState();
                }

                if (_mouseOverRowIndex.HasValue)
                {
                    int newSlot = SlotFromRowIndex(_mouseOverRowIndex.Value);
                    if (IsSlotVisible(newSlot))
                    {
                        DataGridRow? newMouseOverRow = DisplayData.GetDisplayedElement(newSlot) as DataGridRow;
                        Debug.Assert(newMouseOverRow != null);
                        newMouseOverRow.ApplyState();
                    }
                }
            }
        }
    }
    
    internal ScrollBar? VerticalScrollBar => _vScrollBar;
    
    #endregion

    /// <summary>
    /// The default order to use for columns when there is no <see cref="DisplayAttribute.Order"/>
    /// value available for the property.
    /// </summary>
    /// <remarks>
    /// The value of 10,000 comes from the DataAnnotations spec, allowing
    /// some properties to be ordered at the beginning and some at the end.
    /// </remarks>
    private const int DefaultColumnDisplayOrder = 10000;

    internal const double HorizontalGridLinesThickness = 1;
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
    
    private List<Exception> _bindingValidationErrors = [];
    private IDisposable? _validationSubscription;
    
    private INotifyCollectionChanged? _topLevelGroup;
    private ContentControl? _clipboardContentControl;
    
    private Visual? _bottomRightCorner;
    private DataGridRowsPresenter? _rowsPresenter;
    private ScrollBar? _vScrollBar;
    private ScrollBar? _hScrollBar;
    
    private ContentControl? _topLeftCornerHeader;
    private ContentControl? _topRightCornerHeader;
    private Control? _frozenColumnScrollBarSpacer;
    
    // Nth row of rows 0..N that make up the RowHeightEstimate
    private int _lastEstimatedRow;
    private List<DataGridRow> _loadedRows;
    
    // prevents reentry into the VerticalScroll event handler
    private Queue<Action> _lostFocusActions;
    private IndexToValueTable<bool> _showDetailsTable;
    private DataGridSelectedItemsCollection _selectedItems;
    private double _rowHeaderDesiredWidth;
    private int? _mouseOverRowIndex;    // -1 is used for the 'new row'
    private bool _makeFirstDisplayedCellCurrentCellPending;
    private bool _measured;
    private int _noSelectionChangeCount;
    private bool _scrollingByHeight;
    private bool _temporarilyResetCurrentCell;
    private DataGridColumn? _previousCurrentColumn;
    private object? _previousCurrentItem;
    private double[] _rowGroupHeightsByLevel;
    private object? _uneditedValue; // Represents the original current cell value at the time it enters editing mode.
    
    // the sum of the widths in pixels of the scrolling columns preceding
    // the first displayed scrolling column
    private double _horizontalOffset;
    
    // the number of pixels of the firstDisplayedScrollingCol which are not displayed
    private double _negHorizontalOffset;
    private byte _autoGeneratingColumnOperationCount;
    private bool _areHandlersSuspended;
    private bool _autoSizingColumns;
    private IndexToValueTable<bool> _collapsedSlotsTable;
    private Control? _clickedElement;
    
    // used to store the current column during a Reset
    private int _desiredCurrentColumnIndex;
    private int _editingColumnIndex;
    
    // this is a workaround only for the scenarios where we need it, it is not all encompassing nor always updated
    private RoutedEventArgs? _editingEventArgs;
    private bool _executingLostFocusActions;
    private bool _flushCurrentCellChanged;
    private bool _focusEditingControl;
    private Visual? _focusedObject;
    private byte _horizontalScrollChangesIgnored;
    private DataGridRow? _focusedRow;
    private bool _ignoreNextScrollBarsLayout;
    private bool _successfullyUpdatedSelection;
    
    // An approximation of the sum of the heights in pixels of the scrolling rows preceding
    // the first displayed scrolling row.  Since the scrolled off rows are discarded, the grid
    // does not know their actual height. The heights used for the approximation are the ones
    // set as the rows were scrolled off.
    private double _verticalOffset;
    private byte _verticalScrollChangesIgnored;

    private void FlushCurrentCellChanged()
    {
        if (_makeFirstDisplayedCellCurrentCellPending)
        {
            return;
        }
        if (SelectionHasChanged)
        {
            // selection is changing, don't raise CurrentCellChanged until it's done
            _flushCurrentCellChanged = true;
            FlushSelectionChanged();
            return;
        }
        
        // We don't want to expand all intermediate currency positions, so we only expand
        // the last current item before we flush the event
        if (_collapsedSlotsTable.Contains(CurrentSlot))
        {
            var rowGroupInfo = RowGroupHeadersTable.GetValueAt(RowGroupHeadersTable.GetPreviousIndex(CurrentSlot));
            Debug.Assert(rowGroupInfo != null);
            ExpandRowGroupParentChain(rowGroupInfo.Level, rowGroupInfo.Slot);
        }
        
        if (CurrentColumn != _previousCurrentColumn
            || CurrentItem != _previousCurrentItem)
        {
            CoerceSelectedItem();
            _previousCurrentColumn = CurrentColumn;
            _previousCurrentItem   = CurrentItem;
        
            OnCurrentCellChanged(EventArgs.Empty);
        }
        
        _flushCurrentCellChanged = false;
    }

    #region 事件处理器

    private void HandleKeyDown(object? sender, KeyEventArgs e)
    {
        // if (!e.Handled)
        // {
        //     e.Handled = ProcessDataGridKey(e);
        // }
    }

    private void HandleKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Tab && CurrentColumnIndex != -1 && e.Source == this)
        {
            bool success =
                ScrollSlotIntoView(
                    CurrentColumnIndex, CurrentSlot,
                    forCurrentCellChange: false,
                    forceHorizontalScroll: true);
            Debug.Assert(success);
            if (CurrentColumnIndex != -1 && SelectedItem == null)
            {
                SetRowSelection(CurrentSlot, isSelected: true, setAnchorSlot: true);
            }
        }
    }

    private void HandleGotFocus(object? sender, RoutedEventArgs e)
    {
        if (!ContainsFocus)
        {
            ContainsFocus = true;
            ApplyDisplayedRowsState(DisplayData.FirstScrollingSlot, DisplayData.LastScrollingSlot);
            if (CurrentColumnIndex != -1 && IsSlotVisible(CurrentSlot))
            {
                if (DisplayData.GetDisplayedElement(CurrentSlot) is DataGridRow row)
                {
                    row.Cells[CurrentColumnIndex].UpdatePseudoClasses();
                }
            }
        }
        
        // Keep track of which row contains the newly focused element
        DataGridRow? focusedRow     = null;
        Visual?      focusedElement = e.Source as Visual;
        _focusedObject = focusedElement;
        while (focusedElement != null)
        {
            focusedRow = focusedElement as DataGridRow;
            if (focusedRow != null && focusedRow.OwningGrid == this && _focusedRow != focusedRow)
            {
                ResetFocusedRow();
                _focusedRow = focusedRow.IsVisible ? focusedRow : null;
                break;
            }
            focusedElement = focusedElement.GetVisualParent();
        }
    }

    private void HandleLostFocus(object? sender, RoutedEventArgs e)
    {
        _focusedObject = null;
        if (ContainsFocus)
        {
            bool focusLeftDataGrid = true;
            bool dataGridWillReceiveRoutedEvent = true;
            Visual? focusedObject = FocusUtils.GetFocusManager(this)?.GetFocusedElement() as Visual;
            DataGridColumn? editingColumn = null;
        
            while (focusedObject != null)
            {
                if (focusedObject == this)
                {
                    focusLeftDataGrid = false;
                    break;
                }
        
                // Walk up the visual tree.  If we hit the root, try using the framework element's
                // parent.  We do this because Popups behave differently with respect to the visual tree,
                // and it could have a parent even if the VisualTreeHelper doesn't find it.
                var parent = focusedObject.Parent as Visual;
                if (parent == null)
                {
                    parent = focusedObject.GetVisualParent();
                }
                else
                {
                    dataGridWillReceiveRoutedEvent = false;
                }
                focusedObject = parent;
            }
        
            if (EditingRow != null && EditingColumnIndex != -1)
            {
                editingColumn = ColumnsItemsInternal[EditingColumnIndex];
        
                if (focusLeftDataGrid && editingColumn is DataGridTemplateColumn)
                {
                    dataGridWillReceiveRoutedEvent = false;
                }
            }
        
            if (focusLeftDataGrid && !(editingColumn is DataGridTemplateColumn))
            {
                ContainsFocus = false;
                if (EditingRow != null)
                {
                    CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true);
                }
                ResetFocusedRow();
                ApplyDisplayedRowsState(DisplayData.FirstScrollingSlot, DisplayData.LastScrollingSlot);
                if (CurrentColumnIndex != -1 && IsSlotVisible(CurrentSlot))
                {
                    if (DisplayData.GetDisplayedElement(CurrentSlot) is DataGridRow row)
                    {
                        row.Cells[CurrentColumnIndex].UpdatePseudoClasses();
                    }
                }
            }
            else if (!dataGridWillReceiveRoutedEvent)
            {
                if (focusedObject is Control focusedElement)
                {
                    focusedElement.LostFocus += HandleExternalEditingElementLostFocus;
                }
            }
        }
    }
    
    private void HandleColumnsInternalCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // if (e.Action == NotifyCollectionChangedAction.Add
        //     || e.Action == NotifyCollectionChangedAction.Remove
        //     || e.Action == NotifyCollectionChangedAction.Reset)
        // {
        //     UpdatePseudoClasses();
        // }
    }

    /// <summary>
    /// ItemsSourceProperty property changed handler.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    private void HandleItemsSourcePropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {

    }

    #endregion
    
    internal void UpdatePseudoClasses()
    {
        // PseudoClasses.Set(":empty-columns", !ColumnsInternal.GetVisibleColumns().Any());
        // PseudoClasses.Set(":empty-rows", !DataConnection.Any());
    }
    
    private void SetValueNoCallback<T>(AvaloniaProperty<T> property, T value, BindingPriority priority = BindingPriority.LocalValue)
    {
        _areHandlersSuspended = true;
        try
        {
            SetValue(property, value, priority);
        }
        finally
        {
            _areHandlersSuspended = false;
        }
    }
    
    private void EnsureVerticalGridLines()
    {
        if (AreColumnHeadersVisible)
        {
            double totalColumnsWidth = 0;
            foreach (DataGridColumn column in ColumnsInternal)
            {
                totalColumnsWidth += column.ActualWidth;

                column.HeaderCell.AreSeparatorsVisible = (column != ColumnsInternal.LastVisibleColumn || totalColumnsWidth < CellsWidth);
            }
        }

        foreach (DataGridRow row in GetAllRows())
        {
            row.EnsureGridLines();
        }
    }
}