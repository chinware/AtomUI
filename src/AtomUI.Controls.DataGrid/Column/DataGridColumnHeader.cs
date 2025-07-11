// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using AtomUI.Animations;
using AtomUI.Controls.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.Utilities;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

[PseudoClasses(StdPseudoClass.DragIndicator, StdPseudoClass.Pressed, StdPseudoClass.SortAscending, StdPseudoClass.SortDescending)]
internal partial class DataGridColumnHeader : ContentControl, ICustomHitTest
{
    internal enum DragMode
    {
        None = 0,
        MouseDown = 1,
        Drag = 2,
        Resize = 3,
        Reorder = 4
    }

    #region 公共属性定义

    public static readonly StyledProperty<IBrush?> SeparatorBrushProperty =
        AvaloniaProperty.Register<DataGridColumnHeader, IBrush?>(nameof(SeparatorBrush));
    
    public static readonly StyledProperty<bool> IsSeparatorsVisibleProperty =
        AvaloniaProperty.Register<DataGridColumnHeader, bool>(
            nameof(IsSeparatorsVisible),
            defaultValue: true);
    
    public IBrush? SeparatorBrush
    {
        get => GetValue(SeparatorBrushProperty);
        set => SetValue(SeparatorBrushProperty, value);
    }
    
    public bool IsSeparatorsVisible
    {
        get => GetValue(IsSeparatorsVisibleProperty);
        set => SetValue(IsSeparatorsVisibleProperty, value);
    }

    #endregion

    #region 公共事件定义

    public event EventHandler<KeyModifiers>? LeftClick;

    #endregion

    #region 内部属性定义
    
    internal static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<DataGridColumnHeader>();
    
    internal static readonly DirectProperty<DataGridColumnHeader, bool> IsFirstVisibleProperty =
        AvaloniaProperty.RegisterDirect<DataGridColumnHeader, bool>(
            nameof(IsFirstVisible),
            o => o.IsFirstVisible,
            (o, v) => o.IsFirstVisible = v);
    
    internal static readonly DirectProperty<DataGridColumnHeader, bool> IsLastVisibleProperty =
        AvaloniaProperty.RegisterDirect<DataGridColumnHeader, bool>(
            nameof(IsLastVisible),
            o => o.IsLastVisible,
            (o, v) => o.IsLastVisible = v);
    
    internal static readonly DirectProperty<DataGridColumnHeader, bool> IsMiddleVisibleProperty =
        AvaloniaProperty.RegisterDirect<DataGridColumnHeader, bool>(
            nameof(IsMiddleVisible),
            o => o.IsMiddleVisible,
            (o, v) => o.IsMiddleVisible = v);
    
    internal static readonly DirectProperty<DataGridColumnHeader, bool> IndicatorLayoutVisibleProperty =
        AvaloniaProperty.RegisterDirect<DataGridColumnHeader, bool>(
            nameof(IndicatorLayoutVisible),
            o => o.IndicatorLayoutVisible,
            (o, v) => o.IndicatorLayoutVisible = v);
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<DataGridColumnHeader>();
    
    private bool _isFirstVisible = false;
    internal bool IsFirstVisible
    {
        get => _isFirstVisible;
        set
        {
            SetAndRaise(IsFirstVisibleProperty, ref _isFirstVisible, value);
            PseudoClasses.Set(DataGridPseudoClass.FirstColumnHeader, value);
        }
    }
    
    private bool _isLastVisible = false;
    internal bool IsLastVisible
    {
        get => _isLastVisible;
        set
        {
            SetAndRaise(IsLastVisibleProperty, ref _isLastVisible, value);
            PseudoClasses.Set(DataGridPseudoClass.LastColumnHeader, value);
        }
    }
    
    private bool _isMiddleVisible = false;
    internal bool IsMiddleVisible
    {
        get => _isMiddleVisible;
        set
        {
            SetAndRaise(IsLastVisibleProperty, ref _isMiddleVisible, value);
            PseudoClasses.Set(DataGridPseudoClass.MiddleColumnHeader, value);
        }
    }
    
    internal SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    private DataGridColumn? _owningColumn;
    internal DataGridColumn? OwningColumn
    {
        get => _owningColumn;
        set
        {
            _owningColumn = value;
            if (_filterIndicator != null)
            {
                _filterIndicator.OwningColumn = OwningColumn;
            }
        }
    }
    
    private bool _indicatorLayoutVisible = true;
    internal bool IndicatorLayoutVisible
    {
        get => _indicatorLayoutVisible;
        set => SetAndRaise(IndicatorLayoutVisibleProperty, ref _indicatorLayoutVisible, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    internal DataGrid? OwningGrid => OwningColumn?.OwningGrid;

    internal int ColumnIndex
    {
        get
        {
            if (OwningColumn == null)
            {
                return -1;
            }
            return OwningColumn.Index;
        }
    }

    private bool IsMouseOver { get; set; }

    private bool IsPressed { get; set; }
    
    #endregion

    private const int ResizeRegionWidth = 5;
    private const int ColumnsDragThreshold = 5;
    
    private static DragMode _dragMode;
    private static Point? _lastMousePositionHeaders;
    private static Cursor? _originalCursor;
    private static double _originalHorizontalOffset;
    private static double _originalWidth;
    private static Point? _dragStart;
    private static DataGridColumn? _dragColumn;
    private static double _frozenColumnsWidth;
    private bool _areHandlersSuspended;
    private bool _desiredSeparatorVisibility = true;
    private StackPanel? _indicatorsLayout;
    private static Lazy<Cursor> _resizeCursor = new Lazy<Cursor>(() => new Cursor(StandardCursorType.SizeWestEast));
    private Border? _frame;
    
    static DataGridColumnHeader()
    {
        AffectsMeasure<DataGridColumnHeader>(CanUserSortProperty);
        IsSeparatorsVisibleProperty.Changed.AddClassHandler<DataGridColumnHeader>((x, e) => x.HandleIsSeparatorsVisibleChanged(e));
        PressedMixin.Attach<DataGridColumnHeader>();
        IsTabStopProperty.OverrideDefaultValue<DataGridColumnHeader>(false);
        AutomationProperties.IsOffscreenBehaviorProperty.OverrideDefaultValue<DataGridColumnHeader>(IsOffscreenBehavior.FromClip);
    }
    
    public DataGridColumnHeader()
    {
        PointerPressed  += HandlePointerPressed;
        PointerReleased += HandlePointerReleased;
        PointerMoved    += HandlePointerMoved;
        PointerEntered  += HandlePointerEntered;
        PointerExited   += HandlePointerExited;
    }

    // protected override AutomationPeer OnCreateAutomationPeer()
    // {
    //     return new DataGridColumnHeaderAutomationPeer(this);
    // }
    
    internal void UpdateSeparatorVisibility(DataGridColumn? lastVisibleColumn)
    {
        bool newVisibility = _desiredSeparatorVisibility;

        // Collapse separator for the last column if there is no filler column
        if (OwningColumn != null &&
            OwningGrid != null &&
            _desiredSeparatorVisibility &&
            OwningColumn == lastVisibleColumn &&
            OwningGrid.ColumnsInternal.FillerColumn != null &&
            !OwningGrid.ColumnsInternal.FillerColumn.IsActive)
        {
            newVisibility = false;
        }
        // Update the public property if it has changed
        if (IsSeparatorsVisible != newVisibility)
        {
            SetValueNoCallback(IsSeparatorsVisibleProperty, newVisibility);
        }
    }
    
    internal void HandleMouseLeftButtonUpClick(KeyModifiers keyModifiers, ref bool handled)
    {
        LeftClick?.Invoke(this, keyModifiers);
        // completed a click without dragging, so we're sorting
        InvokeProcessSort(keyModifiers);
        handled = true;
    }

    internal void UpdatePseudoClasses()
    {
        if (OwningGrid != null && OwningGrid.DataConnection.AllowSort)
        {
            var sort = OwningColumn?.GetSortDescription();
            if (sort != null)
            {
                CurrentSortingState = sort.Direction;
            }
            else
            {
                CurrentSortingState = null;
            }
        }
        PseudoClasses.Set(StdPseudoClass.SortAscending, CurrentSortingState == ListSortDirection.Ascending);
        PseudoClasses.Set(StdPseudoClass.SortDescending, CurrentSortingState == ListSortDirection.Descending);
    }
    
    //TODO DragDrop

    internal void HandleMouseLeftButtonDown(ref bool handled, PointerEventArgs args, Point mousePosition)
    {
        IsPressed = true;

        if (OwningGrid != null && OwningGrid.ColumnHeaders != null)
        {
            _dragMode                 = DragMode.MouseDown;
            _frozenColumnsWidth       = OwningGrid.ColumnsInternal.GetVisibleFrozenEdgedColumnsWidth();
            _lastMousePositionHeaders = this.Translate(OwningGrid.ColumnHeaders, mousePosition);

            double          distanceFromLeft  = mousePosition.X;
            double          distanceFromRight = Bounds.Width - distanceFromLeft;
            DataGridColumn? currentColumn     = OwningColumn;
            DataGridColumn? previousColumn    = null;
            if (!(OwningColumn is DataGridFillerColumn))
            {
                Debug.Assert(currentColumn != null);
                previousColumn = OwningGrid.ColumnsInternal.GetPreviousVisibleNonFillerColumn(currentColumn);
            }

            if (_dragMode == DragMode.MouseDown && _dragColumn == null && (distanceFromRight <= ResizeRegionWidth))
            {
                Debug.Assert(currentColumn != null);
                handled = TrySetResizeColumn(currentColumn);
            }
            else if (_dragMode == DragMode.MouseDown && _dragColumn == null && distanceFromLeft <= ResizeRegionWidth && previousColumn != null)
            {
                handled = TrySetResizeColumn(previousColumn);
            }

            if (_dragMode == DragMode.Resize && _dragColumn != null)
            {
                _dragStart                = _lastMousePositionHeaders;
                _originalWidth            = _dragColumn.ActualWidth;
                _originalHorizontalOffset = OwningGrid.HorizontalOffset;

                handled = true;
            }
        }
    }

    //TODO DragEvents
    //TODO MouseCapture
    internal void HandleMouseLeftButtonUp(ref bool handled, PointerEventArgs args, Point mousePosition, Point mousePositionHeaders)
    {
        IsPressed = false;

        if (OwningGrid != null && OwningGrid.ColumnHeaders != null)
        {
            if (_dragMode == DragMode.MouseDown)
            {
                HandleMouseLeftButtonUpClick(args.KeyModifiers, ref handled);
            }
            else if (_dragMode == DragMode.Reorder)
            {
                // Find header we're hovering over
                int targetIndex = GetReorderingTargetDisplayIndex(mousePositionHeaders);
                Debug.Assert(OwningColumn != null);
                if (((!OwningColumn.IsFrozen && targetIndex >= OwningGrid.FrozenColumnCount)
                     || (OwningColumn.IsFrozen && targetIndex < OwningGrid.FrozenColumnCount)))
                {
                    OwningColumn.DisplayIndex = targetIndex;

                    DataGridColumnEventArgs ea = new DataGridColumnEventArgs(OwningColumn);
                    OwningGrid.NotifyColumnReordered(ea);
                }
            }

            SetDragCursor(mousePosition);

            // Variables that track drag mode states get reset in DataGridColumnHeader_LostMouseCapture
            args.Pointer.Capture(null);
            HandleLostMouseCapture();
            _dragMode = DragMode.None;
            handled   = true;
        }
    }

    //TODO DragEvents
    internal void HandleMouseMove(PointerEventArgs args, Point mousePosition, Point mousePositionHeaders)
    {
        var handled = args.Handled;
        if (handled || OwningGrid == null || OwningGrid.ColumnHeaders == null)
        {
            return;
        }

        Debug.Assert(OwningGrid.Parent is InputElement);

        HandleMouseMoveResize(ref handled, mousePositionHeaders);
        HandleMouseMoveReorder(ref handled, mousePosition, mousePositionHeaders);
        SetDragCursor(mousePosition);
    }
    
    private void HandleIsSeparatorsVisibleChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (!_areHandlersSuspended)
        {
            _desiredSeparatorVisibility = (bool)(e.NewValue ?? false);
            if (OwningGrid != null)
            {
                UpdateSeparatorVisibility(OwningGrid.ColumnsInternal.LastVisibleColumn);
            }
            else
            {
                UpdateSeparatorVisibility(null);
            }
        }
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

    private bool CanReorderColumn(DataGridColumn column)
    {
        Debug.Assert(OwningGrid != null);
        return OwningGrid.CanUserReorderColumns
               && !(column is DataGridFillerColumn)
               && (column.CanUserReorderInternal.HasValue && column.CanUserReorderInternal.Value || !column.CanUserReorderInternal.HasValue);
    }

    /// <summary>
    /// Determines whether a column can be resized by dragging the border of its header.  If star sizing
    /// is being used, there are special conditions that can prevent a column from being resized:
    /// 1. The column is the last visible column.
    /// 2. All columns are constrained by either their maximum or minimum values.
    /// </summary>
    /// <param name="column">Column to check.</param>
    /// <returns>Whether or not the column can be resized by dragging its header.</returns>
    private static bool CanResizeColumn(DataGridColumn column)
    {
        if (column.OwningGrid != null && column.OwningGrid.UsesStarSizing &&
            (column.OwningGrid.ColumnsInternal.LastVisibleColumn == column || !MathUtilities.AreClose(column.OwningGrid.ColumnsInternal.VisibleEdgedColumnsWidth, column.OwningGrid.CellsWidth)))
        {
            return false;
        }
        return column.ActualCanUserResize;
    }

    private static bool TrySetResizeColumn(DataGridColumn column)
    {
        // If datagrid.CanUserResizeColumns == false, then the column can still override it
        if (CanResizeColumn(column))
        {
            _dragColumn = column;

            _dragMode = DragMode.Resize;

            return true;
        }
        return false;
    }

    private void HandlePointerEntered(object? sender, PointerEventArgs e)
    {
        if (!IsEnabled)
        {
            return;
        }

        Point mousePosition = e.GetPosition(this);
        HandleMouseEnter(mousePosition);
        UpdatePseudoClasses();
    }

    private void HandlePointerExited(object? sender, PointerEventArgs e)
    {
        if (!IsEnabled)
        {
            return;
        }

        HandleMouseLeave();
        UpdatePseudoClasses();
    }

    private void HandlePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (OwningColumn == null || e.Handled || !IsEnabled || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            return;
        }

        Point mousePosition = e.GetPosition(this);
        bool  handled       = e.Handled;
        HandleMouseLeftButtonDown(ref handled, e, mousePosition);
        e.Handled = handled;

        UpdatePseudoClasses();
    }

    private void HandlePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (OwningColumn == null || e.Handled || !IsEnabled || e.InitialPressMouseButton != MouseButton.Left)
        {
            return;
        }
        Debug.Assert(OwningGrid != null);
        Point mousePosition        = e.GetPosition(this);
        Point mousePositionHeaders = e.GetPosition(OwningGrid.ColumnHeaders);
        bool  handled              = e.Handled;
        HandleMouseLeftButtonUp(ref handled, e, mousePosition, mousePositionHeaders);
        e.Handled = handled;

        UpdatePseudoClasses();
    }

    private void HandlePointerMoved(object? sender, PointerEventArgs e)
    {
        if (OwningGrid == null || !IsEnabled)
        {
            return;
        }

        Point mousePosition        = e.GetPosition(this);
        Point mousePositionHeaders = e.GetPosition(OwningGrid.ColumnHeaders);

        HandleMouseMove(e, mousePosition, mousePositionHeaders);
    }

    /// <summary>
    /// Returns the column against whose top-left the reordering caret should be positioned
    /// </summary>
    /// <param name="mousePositionHeaders">Mouse position within the ColumnHeadersPresenter</param>
    /// <param name="scroll">Whether or not to scroll horizontally when a column is dragged out of bounds</param>
    /// <param name="scrollAmount">If scroll is true, returns the horizontal amount that was scrolled</param>
    /// <returns></returns>
    private DataGridColumn? GetReorderingTargetColumn(Point mousePositionHeaders, bool scroll, out double scrollAmount)
    {
        scrollAmount = 0;
        Debug.Assert(OwningGrid != null);
        Debug.Assert(OwningColumn != null);
        Debug.Assert(OwningGrid.ColumnsInternal.RowGroupSpacerColumn != null);
        double leftEdge = OwningGrid.ColumnsInternal.RowGroupSpacerColumn.IsRepresented ? OwningGrid.ColumnsInternal.RowGroupSpacerColumn.ActualWidth : 0;
        double rightEdge = OwningGrid.CellsWidth;
        if (OwningColumn.IsFrozen)
        {
            rightEdge = Math.Min(rightEdge, _frozenColumnsWidth);
        }
        else if (OwningGrid.FrozenColumnCount > 0)
        {
            leftEdge = _frozenColumnsWidth;
        }

        if (mousePositionHeaders.X < leftEdge)
        {
            if (scroll &&
                OwningGrid.HorizontalScrollBar != null &&
                OwningGrid.HorizontalScrollBar.IsVisible &&
                OwningGrid.HorizontalScrollBar.Value > 0)
            {
                double newVal = mousePositionHeaders.X - leftEdge;
                scrollAmount = Math.Min(newVal, OwningGrid.HorizontalScrollBar.Value);
                OwningGrid.UpdateHorizontalOffset(scrollAmount + OwningGrid.HorizontalScrollBar.Value);
            }
            mousePositionHeaders = mousePositionHeaders.WithX(leftEdge);
        }
        else if (mousePositionHeaders.X >= rightEdge)
        {
            if (scroll &&
                OwningGrid.HorizontalScrollBar != null &&
                OwningGrid.HorizontalScrollBar.IsVisible &&
                OwningGrid.HorizontalScrollBar.Value < OwningGrid.HorizontalScrollBar.Maximum)
            {
                double newVal = mousePositionHeaders.X - rightEdge;
                scrollAmount = Math.Min(newVal, OwningGrid.HorizontalScrollBar.Maximum - OwningGrid.HorizontalScrollBar.Value);
                OwningGrid.UpdateHorizontalOffset(scrollAmount + OwningGrid.HorizontalScrollBar.Value);
            }
            mousePositionHeaders = mousePositionHeaders.WithX(rightEdge - 1);
        }

        Debug.Assert(OwningGrid.ColumnHeaders != null);
        foreach (DataGridColumn column in OwningGrid.ColumnsInternal.GetVisibleColumns())
        {
            Point  mousePosition = OwningGrid.ColumnHeaders.Translate(column.HeaderCell, mousePositionHeaders);
            double columnMiddle  = column.HeaderCell.Bounds.Width / 2;
            if (mousePosition.X >= 0 && mousePosition.X <= columnMiddle)
            {
                return column;
            }
            if (mousePosition.X > columnMiddle && mousePosition.X < column.HeaderCell.Bounds.Width)
            {
                return OwningGrid.ColumnsInternal.GetNextVisibleColumn(column);
            }
        }

        return null;
    }

    /// <summary>
    /// Returns the display index to set the column to
    /// </summary>
    /// <param name="mousePositionHeaders">Mouse position relative to the column headers presenter</param>
    /// <returns></returns>
    private int GetReorderingTargetDisplayIndex(Point mousePositionHeaders)
    {
        Debug.Assert(OwningGrid != null);
        Debug.Assert(OwningColumn != null);
        DataGridColumn? targetColumn = GetReorderingTargetColumn(mousePositionHeaders, false /*scroll*/, out double scrollAmount);
        if (targetColumn != null)
        {
            return targetColumn.DisplayIndex > OwningColumn.DisplayIndex ? targetColumn.DisplayIndex - 1 : targetColumn.DisplayIndex;
        }
        return OwningGrid.Columns.Count - 1;
    }

    /// <summary>
    /// Returns true if the mouse is
    /// - to the left of the element, or within the left half of the element
    /// and
    /// - within the vertical range of the element, or ignoreVertical == true
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <param name="element"></param>
    /// <param name="ignoreVertical"></param>
    /// <returns></returns>
    private bool IsReorderTargeted(Point mousePosition, Control element, bool ignoreVertical)
    {
        Point position = this.Translate(element, mousePosition);

        return (position.X < 0 || (position.X >= 0 && position.X <= element.Bounds.Width / 2))
               && (ignoreVertical || (position.Y >= 0 && position.Y <= element.Bounds.Height));
    }

    /// <summary>
    /// Resets the static DataGridColumnHeader properties when a header loses mouse capture
    /// </summary>
    private void HandleLostMouseCapture()
    {
        // When we stop interacting with the column headers, we need to reset the drag mode
        // and close any popups if they are open.

        if (_dragColumn != null)
        {
            _dragColumn.HeaderCell.Cursor = _originalCursor;
        }
        _dragMode                 = DragMode.None;
        _dragColumn               = null;
        _dragStart                = null;
        _lastMousePositionHeaders = null;

        if (OwningGrid != null && OwningGrid.ColumnHeaders != null)
        {
            OwningGrid.ColumnHeaders.DragColumn            = null;
            OwningGrid.ColumnHeaders.DragIndicator         = null;
            OwningGrid.ColumnHeaders.DropLocationIndicator = null;
        }
    }

    /// <summary>
    /// Sets up the DataGridColumnHeader for the MouseEnter event
    /// </summary>
    /// <param name="mousePosition">mouse position relative to the DataGridColumnHeader</param>
    private void HandleMouseEnter(Point mousePosition)
    {
        IsMouseOver = true;
        SetDragCursor(mousePosition);
    }

    /// <summary>
    /// Sets up the DataGridColumnHeader for the MouseLeave event
    /// </summary>
    private void HandleMouseLeave()
    {
        IsMouseOver = false;
    }

    private void HandleMouseMoveBeginReorder(Point mousePosition)
    {
        Debug.Assert(OwningGrid != null);
        Debug.Assert(OwningColumn != null);
        var dragIndicator = new DataGridColumnHeader
        {
            OwningColumn = OwningColumn,
            IsEnabled    = false,
            Content      = GetDragIndicatorContent(Content, ContentTemplate)
        };
        dragIndicator.PseudoClasses.Add(StdPseudoClass.DragIndicator);

        Control? dropLocationIndicator = OwningGrid.DropLocationIndicatorTemplate?.Build();

        // If the user didn't style the dropLocationIndicator's Height, default to the column header's height
        if (dropLocationIndicator != null && double.IsNaN(dropLocationIndicator.Height) && dropLocationIndicator is Control element)
        {
            element.Height = Bounds.Height;
        }

        // pass the caret's data template to the user for modification
        DataGridColumnReorderingEventArgs columnReorderingEventArgs = new DataGridColumnReorderingEventArgs(OwningColumn)
        {
            DropLocationIndicator = dropLocationIndicator,
            DragIndicator         = dragIndicator
        };
        OwningGrid.HandleColumnReordering(columnReorderingEventArgs);
        if (columnReorderingEventArgs.Cancel)
        {
            return;
        }

        // The user didn't cancel, so prepare for the reorder
        _dragColumn = OwningColumn;
        _dragMode   = DragMode.Reorder;
        _dragStart  = mousePosition;

        Debug.Assert(OwningGrid.ColumnHeaders != null);
        // Display the reordering thumb
        OwningGrid.ColumnHeaders.DragColumn            = OwningColumn;
        OwningGrid.ColumnHeaders.DragIndicator         = columnReorderingEventArgs.DragIndicator;
        OwningGrid.ColumnHeaders.DropLocationIndicator = columnReorderingEventArgs.DropLocationIndicator;

        // If the user didn't style the dragIndicator's Width, default it to the column header's width
        if (double.IsNaN(dragIndicator.Width))
        {
            dragIndicator.Width = Bounds.Width;
        }
    }

    private object? GetDragIndicatorContent(object? content, IDataTemplate? dataTemplate)
    {
        if (content is ContentControl icc)
        {
            content = icc.Content;
        }

        if (content is Control control)
        {
            if (VisualRoot == null) return content;
            control.Measure(Size.Infinity);
            var rect = new Rectangle()
            {
                Width  = control.DesiredSize.Width,
                Height = control.DesiredSize.Height,
                Fill = new VisualBrush
                {
                    Visual = control, Stretch = Stretch.None, AlignmentX = AlignmentX.Left,
                }
            };
            return rect;
        }

        if (dataTemplate is not null)
        {
            return dataTemplate.Build(content);
        }
        return content;
    }
    
    //TODO DragEvents
    private void HandleMouseMoveReorder(ref bool handled, Point mousePosition, Point mousePositionHeaders)
    {
        if (handled)
        {
            return;
        }
        Debug.Assert(OwningGrid != null);
        Debug.Assert(OwningColumn != null);
        Debug.Assert(OwningGrid.ColumnHeaders != null);
        //handle entry into reorder mode
        if (_dragMode == DragMode.MouseDown && _dragColumn == null && _lastMousePositionHeaders != null)
        {
            var distanceFromInitial = (Vector)(mousePositionHeaders - _lastMousePositionHeaders);
            if (distanceFromInitial.Length > ColumnsDragThreshold)
            {
                handled = CanReorderColumn(OwningColumn);

                if (handled)
                {
                    HandleMouseMoveBeginReorder(mousePosition);
                }
            }
        }

        //handle reorder mode (eg, positioning of the popup)
        if (_dragMode == DragMode.Reorder && OwningGrid.ColumnHeaders.DragIndicator != null)
        {
            Debug.Assert(_dragStart != null);
            // Find header we're hovering over
            DataGridColumn? targetColumn = GetReorderingTargetColumn(mousePositionHeaders, !OwningColumn.IsFrozen /*scroll*/, out double scrollAmount);

            OwningGrid.ColumnHeaders.DragIndicatorOffset = mousePosition.X - _dragStart.Value.X + scrollAmount;
            OwningGrid.ColumnHeaders.InvalidateArrange();

            if (OwningGrid.ColumnHeaders.DropLocationIndicator != null)
            {
                Point targetPosition = new Point(0, 0);
                if (targetColumn == null || targetColumn == OwningGrid.ColumnsInternal.FillerColumn || targetColumn.IsFrozen != OwningColumn.IsFrozen)
                {
                    targetColumn =
                        OwningGrid.ColumnsInternal.GetLastColumn(
                            isVisible: true,
                            isFrozen: OwningColumn.IsFrozen,
                            isReadOnly: null);
                    if (targetColumn != null)
                    {
                        targetPosition = targetColumn.HeaderCell.Translate(OwningGrid.ColumnHeaders, targetPosition);
                        targetPosition = targetPosition.WithX(targetPosition.X + targetColumn.ActualWidth);
                    }
                }
                else
                {
                    targetPosition = targetColumn.HeaderCell.Translate(OwningGrid.ColumnHeaders, targetPosition);
                }
                OwningGrid.ColumnHeaders.DropLocationIndicatorOffset = targetPosition.X - scrollAmount;
            }

            handled = true;
        }
    }

    private void HandleMouseMoveResize(ref bool handled, Point mousePositionHeaders)
    {
        if (handled)
        {
            return;
        }
        Debug.Assert(OwningGrid != null);
        if (_dragMode == DragMode.Resize && _dragColumn != null && _dragStart.HasValue)
        {
            // resize column
            double mouseDelta   = mousePositionHeaders.X - _dragStart.Value.X;
            double desiredWidth = _originalWidth + mouseDelta;

            desiredWidth = Math.Max(_dragColumn.ActualMinWidth, Math.Min(_dragColumn.ActualMaxWidth, desiredWidth));
            _dragColumn.Resize(_dragColumn.Width,
                new(_dragColumn.Width.Value, _dragColumn.Width.UnitType, _dragColumn.Width.DesiredValue, desiredWidth),
                true);

            OwningGrid.UpdateHorizontalOffset(_originalHorizontalOffset);

            handled = true;
        }
    }

    private void SetDragCursor(Point mousePosition)
    {
        if (_dragMode != DragMode.None || OwningGrid == null || OwningColumn == null)
        {
            return;
        }

        // set mouse if we can resize column
        double          distanceFromLeft  = mousePosition.X;
        double          distanceFromRight = Bounds.Width - distanceFromLeft;
        DataGridColumn? currentColumn     = OwningColumn;
        DataGridColumn? previousColumn    = null;

        if (!(OwningColumn is DataGridFillerColumn))
        {
            previousColumn = OwningGrid.ColumnsInternal.GetPreviousVisibleNonFillerColumn(currentColumn);
        }

        if ((distanceFromRight <= ResizeRegionWidth && currentColumn != null && CanResizeColumn(currentColumn)) ||
            (distanceFromLeft <= ResizeRegionWidth && previousColumn != null && CanResizeColumn(previousColumn)))
        {
            var resizeCursor = _resizeCursor.Value;
            if (Cursor != resizeCursor)
            {
                _originalCursor = Cursor;
                Cursor          = resizeCursor;
            }
        }
        else
        {
            if (_originalCursor != null)
            {
                Cursor = _originalCursor;
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _indicatorsLayout = e.NameScope.Find<StackPanel>(DataGridColumnHeaderThemeConstants.IndicatorsLayoutPart);
        _filterIndicator = e.NameScope.Find<DataGridFilterIndicator>(DataGridColumnHeaderThemeConstants.FilterIndicatorPart);
        _frame = e.NameScope.Find<Border>(DataGridColumnHeaderThemeConstants.FramePart);
        if (_filterIndicator != null && OwningColumn != null)
        {
            _filterIndicator.OwningColumn = OwningColumn;
        }

        ConfigureFilterIndicator();
        if (_indicatorsLayout != null)
        {
            _indicatorsLayout.Children.CollectionChanged += HandleIndicatorLayoutChildrenChanged;
        }

        ConfigureTransitions();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions();
            }
        }

        NotifyPropertyChangedForSorting(change);
    }

    private void HandleIndicatorLayoutChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Debug.Assert(_indicatorsLayout != null);
        _indicatorsLayout.IsVisible = _indicatorsLayout.Children.Count > 0;
    }
    
    private void ConfigureTransitions()
    {
        if (IsMotionEnabled)
        {
            if (Transitions != null)
            {
                Transitions = new Transitions()
                {
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty)
                };
            }
        }
        else
        {
            Transitions?.Clear();
            Transitions = null;
        }
    }

    public bool HitTest(Point point)
    {
        return true;
    }
}