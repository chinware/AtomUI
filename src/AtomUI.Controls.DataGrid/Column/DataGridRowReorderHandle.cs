using System.Collections;
using System.Diagnostics;
using AtomUI.Animations;
using AtomUI.Controls.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Threading;

namespace AtomUI.Controls;

internal class DataGridRowReorderHandle : TemplatedControl
{
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<DataGridRowReorderHandle>();

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    internal DataGrid? OwningGrid { get; set; }
    internal DataGridRow? OwningRow;

    private static Point? _dragStart;
    private static double? _dragDelta;
    private static DataGridRow? _dragRow;
    private static DataGridRow? _prevDraggingOverRow;
    private static DataGridRow? _currentDraggingOverRow;
    private static Point? _lastMousePositionInPresenter;
    private IconButton? _indicatorButton;

    private void HandlePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (OwningRow == null || !IsEnabled || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            return;
        }

        Point mousePosition = e.GetPosition(this);
        bool  handled       = e.Handled;
        HandleMouseLeftButtonDown(ref handled, e, mousePosition);
        e.Handled = handled;
    }

    private void HandlePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (OwningRow == null || !IsEnabled || e.InitialPressMouseButton != MouseButton.Left)
        {
            return;
        }

        Debug.Assert(OwningGrid != null);
        var rowsPresenter = OwningGrid.RowsPresenter;
        Debug.Assert(rowsPresenter != null);
        Debug.Assert(OwningRow != null);

        // Find header we're hovering over
        DataGridRowEventArgs rowEventArgs = new DataGridRowEventArgs(OwningRow);
        OwningGrid.NotifyRowReordered(rowEventArgs);
        // Variables that track drag mode states get reset in DataGridColumnHeader_LostMouseCapture
        e.Pointer.Capture(null);
        rowsPresenter.NotifyDropped();
        if (_indicatorButton != null)
        {
            _indicatorButton.DisableTransitions();
        }
        if (_prevDraggingOverRow != null && _prevDraggingOverRow != OwningRow)
        {
            if (OwningGrid.CollectionView is IList collectionView)
            {
                var data = collectionView[OwningRow.Index];
                if (data is not null)
                {
                    collectionView.RemoveAt(OwningRow.Index);
                    collectionView.Insert(_prevDraggingOverRow.Index, data);
                }
                if (_indicatorButton != null)
                {
                    _indicatorButton.EnableTransitions();
                }
                Dispatcher.UIThread.Post(() =>
                {
                    OwningGrid.CollectionView.Refresh();
                });
            }
        }
        
        OwningRow.Opacity   = 1.0;

        _dragDelta                    = null;
        _dragRow                      = null;
        _dragStart                    = null;
        _lastMousePositionInPresenter = null;
        _currentDraggingOverRow       = null;
        _prevDraggingOverRow          = null;
        rowsPresenter.DraggedRow      = null;
        rowsPresenter.DragRowOffset   = 0.0;
    }

    private void HandlePointerMoved(object? sender, PointerEventArgs e)
    {
        if (OwningGrid == null || !IsEnabled || OwningGrid.ColumnHeaders == null)
        {
            return;
        }
        
        Point mousePositionPresenter = e.GetPosition(OwningGrid.RowsPresenter);

        Debug.Assert(OwningGrid.Parent is InputElement);
        HandleMouseMoveReorder(mousePositionPresenter);
    }

    private void HandleMouseMoveReorder(Point mousePositionPresenter)
    {
        Debug.Assert(OwningGrid != null);
        Debug.Assert(OwningRow != null);
        Debug.Assert(OwningGrid.RowsPresenter != null);

        var rowsPresenter = OwningGrid.RowsPresenter;

        //handle entry into reorder mode
        if (_dragRow == null && _lastMousePositionInPresenter != null)
        {
            HandleMouseMoveBeginReorder(mousePositionPresenter);
        }

        //handle reorder mode (eg, positioning of the popup)
        if (rowsPresenter.DraggedRow != null)
        {
            Debug.Assert(_dragStart != null);
            // Find row we're hovering over
            var targetRow = GetReorderingTargetRow(mousePositionPresenter, true, out double scrollAmount);
            if (targetRow != _currentDraggingOverRow)
            {
                _prevDraggingOverRow = _currentDraggingOverRow;
                _currentDraggingOverRow = targetRow;
                if (_currentDraggingOverRow != null && _currentDraggingOverRow != _dragRow)
                {
                    rowsPresenter.SwapOrderingChildren(OwningRow, _currentDraggingOverRow);
                }
            }
            
            if (_dragDelta == null)
            {
                _dragDelta = _dragRow?.Bounds.Y;
            }
            var delta = _dragDelta.HasValue ? _dragDelta.Value : 0.0;
            rowsPresenter.DragRowOffset = delta + mousePositionPresenter.Y - _dragStart.Value.Y + scrollAmount;
            rowsPresenter.InvalidateArrange();
        }
    }

    private DataGridRow? GetReorderingTargetRow(Point mousePositionPresenter, bool scroll, out double scrollAmount)
    {
        scrollAmount = 0;
        Debug.Assert(OwningGrid != null);
        Debug.Assert(OwningRow != null);
        Debug.Assert(OwningGrid.RowsPresenter != null);
        Debug.Assert(OwningGrid.ColumnsInternal.RowGroupSpacerColumn != null);

        var rowsPresenter = OwningGrid.RowsPresenter;

        double topEdge    = rowsPresenter.Bounds.Top;
        double bottomEdge = rowsPresenter.Bounds.Bottom;

        if (mousePositionPresenter.Y < topEdge)
        {
            if (scroll &&
                OwningGrid.VerticalScrollBar != null &&
                OwningGrid.VerticalScrollBar.IsVisible &&
                OwningGrid.VerticalScrollBar.Value > 0)
            {
                double newVal = mousePositionPresenter.Y - topEdge;
                scrollAmount = Math.Min(newVal, OwningGrid.VerticalScrollBar.Value);
                OwningGrid.ScrollSlotsByHeight(scrollAmount);
            }

            mousePositionPresenter = mousePositionPresenter.WithY(topEdge);
        }
        else if (mousePositionPresenter.Y >= bottomEdge)
        {
            if (scroll &&
                OwningGrid.VerticalScrollBar != null &&
                OwningGrid.VerticalScrollBar.IsVisible &&
                OwningGrid.VerticalScrollBar.Value < OwningGrid.VerticalScrollBar.Maximum)
            {
                double newVal = mousePositionPresenter.X - bottomEdge;
                scrollAmount = Math.Min(newVal,
                    OwningGrid.VerticalScrollBar.Maximum - OwningGrid.VerticalScrollBar.Value);
                OwningGrid.ScrollSlotsByHeight(scrollAmount);
            }

            mousePositionPresenter = mousePositionPresenter.WithY(bottomEdge - 1);
        }

        foreach (Control child in rowsPresenter.Children)
        {
            if (child is DataGridRow row)
            {
                Point mousePosition = OwningGrid.RowsPresenter.Translate(row, mousePositionPresenter);
                if (mousePosition.Y >= 0 && mousePosition.Y <= row.Bounds.Height)
                {
                    return row;
                }
            }
        }

        return null;
    }

    private void HandleMouseMoveBeginReorder(Point mousePosition)
    {
        Debug.Assert(OwningGrid != null);
        Debug.Assert(OwningRow != null);
        
        // pass the caret's data template to the user for modification
        DataGridRowReorderingEventArgs rowReorderingEventArgs = new DataGridRowReorderingEventArgs(OwningRow);
        OwningGrid.NotifyRowReordering(rowReorderingEventArgs);
        if (rowReorderingEventArgs.Cancel)
        {
            return;
        }

        // The user didn't cancel, so prepare for the reorder
        _dragRow         = OwningRow;
        _dragStart       = mousePosition;
        _dragRow.Opacity = 0.0;
        
        Debug.Assert(OwningGrid.RowsPresenter != null);
        // Display the reordering thumb
        OwningGrid.RowsPresenter.DraggedRow = OwningRow;
        OwningGrid.RowsPresenter.NotifyAboutToDragging();
    }

    internal void HandleMouseLeftButtonDown(ref bool handled, PointerEventArgs args, Point mousePosition)
    {
        if (OwningGrid != null && OwningGrid.RowsPresenter != null)
        {
            _lastMousePositionInPresenter = this.Translate(OwningGrid.RowsPresenter, mousePosition);
        }
    }

    internal void NotifyLoadingRow(DataGridRow row)
    {
        OwningRow = row;
    }

    internal void NotifyUnLoadingRow(DataGridRow row)
    {
        OwningRow = null;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _indicatorButton = e.NameScope.Find<IconButton>(DataGridRowReorderHandleConstants.IndicatorIconButtonPart);
        if (_indicatorButton != null)
        {
            _indicatorButton.PassthroughPointerPressed  += HandlePointerPressed;
            _indicatorButton.PassthroughPointerReleased += HandlePointerReleased;
            _indicatorButton.PassthroughPointerMoved    += HandlePointerMoved;
        }
    }
}