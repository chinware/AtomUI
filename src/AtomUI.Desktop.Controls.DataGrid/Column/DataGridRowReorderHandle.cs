using System.Collections;
using System.Diagnostics;
using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace AtomUI.Desktop.Controls;

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
    
    private static Rect? _dragRowBounds;
    private static int? _dragRowIndex;
    private static int? _currentDraggingOverRowIndex;
    
    private static Point? _lastMousePositionInPresenter;
    private IconButton? _indicatorButton;

    protected override void OnPointerPressed(PointerPressedEventArgs e)
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

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
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
        if (_currentDraggingOverRowIndex != null && _currentDraggingOverRowIndex != _dragRowIndex)
        {
            if (OwningGrid.CollectionView is IList collectionView)
            {
                if (_dragRowIndex != null)
                {
                    var data = collectionView[_dragRowIndex.Value];
                    if (data is not null)
                    {
                        collectionView.RemoveAt(_dragRowIndex.Value);
                        collectionView.Insert(_currentDraggingOverRowIndex.Value, data);
                    }
                }
               
                if (_indicatorButton != null)
                {
                    _indicatorButton.EnableTransitions();
                }
                OwningGrid.CollectionView.Refresh();
            }
        }
        
        _dragDelta                    = null;
        _dragRowIndex                 = null;
        _dragRowBounds                = null;
        _dragStart                    = null;
        _lastMousePositionInPresenter = null;
        _currentDraggingOverRowIndex  = null;
        rowsPresenter.DragRowOffset   = 0.0;
        rowsPresenter.InvalidateArrange();
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed || OwningGrid == null || !IsEnabled || OwningGrid.ColumnHeaders == null)
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
        if (_dragRowIndex == null && _lastMousePositionInPresenter != null)
        {
            HandleMouseMoveBeginReorder(mousePositionPresenter);
        }

        //handle reorder mode (eg, positioning of the popup)
        if (rowsPresenter.DraggedRowIndex != null)
        {
            Debug.Assert(_dragStart != null);
            // Find row we're hovering over
            var targetRowIndex = GetReorderingTargetRow(mousePositionPresenter, true, out double scrollAmount);
            _currentDraggingOverRowIndex = targetRowIndex;
 
            if (_dragDelta == null)
            {
                _dragDelta = _dragRowBounds?.Y;
            }
            var delta = _dragDelta.HasValue ? _dragDelta.Value : 0.0;
            rowsPresenter.DragRowOffset = delta + mousePositionPresenter.Y - _dragStart.Value.Y + scrollAmount;
            rowsPresenter.DraggedRowIndex = _dragRowIndex;
            rowsPresenter.InvalidateArrange();
        }
    }

    private int? GetReorderingTargetRow(Point mousePositionPresenter, bool scroll, out double scrollAmount)
    {
        scrollAmount = 0;
        Debug.Assert(OwningGrid != null);
        Debug.Assert(OwningRow != null);
        Debug.Assert(OwningGrid.RowsPresenter != null);
        Debug.Assert(OwningGrid.ColumnsInternal.RowGroupSpacerColumn != null);

        var rowsPresenter = OwningGrid.RowsPresenter;

        double topEdge    = 0;
        double bottomEdge = rowsPresenter.DesiredSize.Height;

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
                double newVal = mousePositionPresenter.Y - bottomEdge;
                scrollAmount = Math.Min(newVal,
                    OwningGrid.VerticalScrollBar.Maximum - OwningGrid.VerticalScrollBar.Value);
                if (scrollAmount > 0)
                {
                    OwningGrid.ScrollSlotsByHeight(scrollAmount);
                }
            }

            mousePositionPresenter = mousePositionPresenter.WithY(bottomEdge - 1);
        }

        foreach (Control child in OwningGrid.DisplayData.GetScrollingElements())
        {
            if (child is DataGridRow row)
            {
                Point mousePosition = OwningGrid.RowsPresenter.Translate(row, mousePositionPresenter);
                if (mousePosition.Y >= 0 && mousePosition.Y <= row.Bounds.Height)
                {
                    return row.Index;
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
        _dragRowIndex     = OwningRow.Index;
        _dragRowBounds    = OwningRow.Bounds;
        _dragStart        = mousePosition;
        
        Debug.Assert(OwningGrid.RowsPresenter != null);
        // Display the reordering thumb
        OwningGrid.RowsPresenter.DraggedRowIndex = _dragRowIndex;
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
    }
}