using System.Collections.Specialized;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls;

public class DataGridGroupColumnHeadersPresenter : Panel, IChildIndexProvider
{
    private EventHandler<ChildIndexChangedEventArgs>? _childIndexChanged;
    private Control? _dragIndicator;
    private Control? _dropLocationIndicator;

    internal DataGrid? OwningGrid { get; set; }

    event EventHandler<ChildIndexChangedEventArgs>? IChildIndexProvider.ChildIndexChanged
    {
        add => _childIndexChanged += value;
        remove => _childIndexChanged -= value;
    }

    /// <summary>
    /// Tracks which column is currently being dragged.
    /// </summary>
    internal DataGridColumn? DragColumn { get; set; }

    /// <summary>
    /// The current drag indicator control.  This value is null if no column is being dragged.
    /// </summary>
    internal Control? DragIndicator
    {
        get => _dragIndicator;
        set
        {
            if (value != _dragIndicator)
            {
                if (_dragIndicator != null)
                {
                    if (Children.Contains(_dragIndicator))
                    {
                        Children.Remove(_dragIndicator);
                    }
                }

                _dragIndicator = value;
                if (_dragIndicator != null)
                {
                    Children.Add(_dragIndicator);
                }
            }
        }
    }

    /// <summary>
    /// The distance, in pixels, that the DragIndicator should be positioned away from the corresponding DragColumn.
    /// </summary>
    internal Double DragIndicatorOffset { get; set; }

    internal double DropLocationIndicatorOffset { get; set; }

    /// <summary>
    /// The drop location indicator control.  This value is null if no column is being dragged.
    /// </summary>
    internal Control? DropLocationIndicator
    {
        get => _dropLocationIndicator;
        set
        {
            if (value != _dropLocationIndicator)
            {
                if (_dropLocationIndicator != null)
                {
                    if (Children.Contains(_dropLocationIndicator))
                    {
                        Children.Remove(_dropLocationIndicator);
                    }
                }

                _dropLocationIndicator = value;
                if (_dropLocationIndicator != null)
                {
                    Children.Add(_dropLocationIndicator);
                }
            }
        }
    }

    int IChildIndexProvider.GetChildIndex(ILogical child)
    {
        return child is DataGridColumnHeader header
            ? header.OwningColumn?.DisplayIndex ?? -1
            : throw new InvalidOperationException("Invalid cell type");
    }

    bool IChildIndexProvider.TryGetTotalCount(out int count)
    {
        count = Children.Count - 1; // Adjust for filler column
        return true;
    }

    protected override void ChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        base.ChildrenChanged(sender, e);

        InvalidateChildIndex();
    }

    internal void InvalidateChildIndex()
    {
        _childIndexChanged?.Invoke(this, ChildIndexChangedEventArgs.ChildIndexesReset);
    }

    private double _columnHeight;

    protected override Size MeasureOverride(Size availableSize)
    {
        if (OwningGrid == null)
        {
            return base.MeasureOverride(availableSize);
        }

        if (!OwningGrid.IsColumnHeadersVisible)
        {
            return default;
        }

        _columnHeight = OwningGrid.ColumnHeaderHeight;
        bool          autoSizeHeight;
        if (double.IsNaN(_columnHeight))
        {
            // No explicit height values were set so we can autosize
            _columnHeight  = 0;
            autoSizeHeight = true;
        }
        else
        {
            autoSizeHeight = false;
        }

        double totalDisplayWidth = 0;
        OwningGrid.ColumnsInternal.EnsureVisibleEdgedColumnsWidth();

        DataGridColumn? lastVisibleColumn = OwningGrid.ColumnsInternal.LastVisibleColumn;
        foreach (DataGridColumn column in OwningGrid.ColumnsInternal.GetVisibleColumns())
        {
            // Measure each column header
            bool                    autoGrowWidth  = column.Width.IsAuto || column.Width.IsSizeToHeader;
            DataGridHeaderViewItem? headerViewItem = null;
            if (column is IDataGridColumnGroupItemInternal groupItem)
            {
                headerViewItem = groupItem.GroupHeaderViewItem;
            }

            Debug.Assert(headerViewItem != null);
            DataGridColumnHeader columnHeader = column.HeaderCell;
            if (column != lastVisibleColumn)
            {
                columnHeader.UpdateSeparatorVisibility(lastVisibleColumn);
            }

            // If we're not using star sizing or the current column can't be resized,
            // then just set the display width according to the column's desired width
            if (!OwningGrid.UsesStarSizing || (!column.ActualCanUserResize && !column.Width.IsStar))
            {
                // In the edge-case where we're given infinite width and we have star columns, the 
                // star columns grow to their predefined limit of 10,000 (or their MaxWidth)
                double newDisplayWidth = column.Width.IsStar
                    ? Math.Min(column.ActualMaxWidth, DataGrid.MaximumStarColumnWidth)
                    : Math.Max(column.ActualMinWidth, Math.Min(column.ActualMaxWidth, column.Width.DesiredValue));
                column.SetWidthDisplayValue(newDisplayWidth);
            }

            // If we're auto-growing the column based on the header content, we want to measure it at its maximum value
            if (autoGrowWidth)
            {
                headerViewItem.Measure(new Size(column.ActualMaxWidth, double.PositiveInfinity));
                OwningGrid.AutoSizeColumn(column, headerViewItem.DesiredSize.Width);
                column.ComputeLayoutRoundedWidth(totalDisplayWidth);
            }
            else if (!OwningGrid.UsesStarSizing)
            {
                column.ComputeLayoutRoundedWidth(totalDisplayWidth);
                headerViewItem.Measure(new Size(column.LayoutRoundedWidth, double.PositiveInfinity));
            }

            // We need to track the largest height in order to auto-size
            if (autoSizeHeight)
            {
                _columnHeight = Math.Max(_columnHeight, headerViewItem.DesiredSize.Height);
            }

            totalDisplayWidth += column.ActualWidth;
        }

        // If we're using star sizing (and we're not waiting for an auto-column to finish growing)
        // then we will resize all the columns to fit the available space.
        if (OwningGrid.UsesStarSizing && !OwningGrid.AutoSizingColumns)
        {
            // Since we didn't know the final widths of the columns until we resized,
            // we waited until now to measure each header
            double leftEdge = 0;
            foreach (DataGridColumn column in OwningGrid.ColumnsInternal.GetVisibleColumns())
            {
                column.ComputeLayoutRoundedWidth(leftEdge);
                column.HeaderCell.Measure(new Size(column.LayoutRoundedWidth, double.PositiveInfinity));
                if (autoSizeHeight)
                {
                    _columnHeight = Math.Max(_columnHeight, column.HeaderCell.DesiredSize.Height);
                }

                leftEdge += column.ActualWidth;
            }
        }

        // Add the filler column if it's not represented.  We won't know whether we need it or not until Arrange
        DataGridFillerColumn? fillerColumn = OwningGrid.ColumnsInternal.FillerColumn;
        Debug.Assert(fillerColumn != null);
        if (!fillerColumn.IsRepresented)
        {
            Debug.Assert(!Children.Contains(fillerColumn.HeaderCell));
            fillerColumn.HeaderCell.IsSeparatorsVisible = false;
            Children.Insert(OwningGrid.ColumnsInternal.Count, fillerColumn.HeaderCell);
            fillerColumn.IsRepresented = true;
            // Optimize for the case where we don't need the filler cell 
            fillerColumn.HeaderCell.IsVisible = false;
        }

        fillerColumn.HeaderCell.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

        if (DragIndicator != null)
        {
            DragIndicator.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        }

        if (DropLocationIndicator != null)
        {
            DropLocationIndicator.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        }

        OwningGrid.ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
        
        double totalHeight = 0.0;
        // 递归求高度
        foreach (IDataGridColumnGroupItem item in OwningGrid.ColumnGroups)
        {
            MeasureGroupRecursive(item, out var childTotalHeight);
            totalHeight =  Math.Max(totalHeight, childTotalHeight);
        }
        
        return new Size(OwningGrid.ColumnsInternal.VisibleEdgedColumnsWidth, totalHeight);
    }

    private Size MeasureGroupRecursive(IDataGridColumnGroupItem item, out double totalHeight)
    {
        totalHeight = 0.0;
        var height = 0.0;
        var width = double.NaN;
        if (item is IDataGridColumnGroupItemInternal groupItem)
        {
            var hasChildren = groupItem.GroupChildren.Count > 0;
            if (hasChildren)
            {
                var childHeight = 0.0;
                var childWidth = 0.0;
                foreach (var child in groupItem.GroupChildren)
                {
                    var childSize = MeasureGroupRecursive(child, out var childTotalHeight);
                    childHeight =  Math.Max(childSize.Height, childHeight);
                    childWidth  += childSize.Width;
                    totalHeight = Math.Max(totalHeight, childTotalHeight);
                }
                
                width = Math.Max(double.IsNaN(width) ? 0.0 : width, childWidth);
            }
            
            if (groupItem.GroupHeaderViewItem != null)
            {
                if (hasChildren)
                {
                    groupItem.GroupHeaderViewItem.MinWidth = width;
                    groupItem.GroupHeaderViewItem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    height = groupItem.GroupHeaderViewItem.DesiredSize.Height;
                    width  = groupItem.GroupHeaderViewItem.DesiredSize.Width;
                }
                else
                {
                    if (groupItem is DataGridColumn column)
                    {
                        height = _columnHeight;
                        width  = column.ActualWidth;
                    }
                }
            }
        }

        totalHeight += height;
        return new Size(width, height);
    }

    private Size ArrangeLeafItem(DataGridColumn dataGridColumn, Rect rect)
    {
        DataGridHeaderViewItem? headerViewItem = null;
        if (dataGridColumn is IDataGridColumnGroupItemInternal groupItem)
        {
            headerViewItem = groupItem.GroupHeaderViewItem;
        }

        Debug.Assert(headerViewItem != null);
        DataGridColumnHeader columnHeader = dataGridColumn.HeaderCell;
        Debug.Assert(columnHeader.OwningColumn == dataGridColumn);
            
        if (dataGridColumn.IsFrozen)
        {
            headerViewItem.Arrange(new Rect(_frozenLeftEdge, rect.Y, dataGridColumn.LayoutRoundedWidth, rect.Height));
            headerViewItem.Clip =
                null; // The layout system could have clipped this because it's not aware of our render transform
            if (DragColumn == dataGridColumn && DragIndicator != null)
            {
                _dragIndicatorLeftEdge = _frozenLeftEdge + DragIndicatorOffset;
            }
        
            _frozenLeftEdge += dataGridColumn.ActualWidth;
        }
        else
        {
            headerViewItem.Arrange(
                new Rect(_scrollingLeftEdge, rect.Y, dataGridColumn.LayoutRoundedWidth, rect.Height));
            EnsureColumnHeaderClip(headerViewItem, dataGridColumn.ActualWidth, rect.Height, _frozenLeftEdge,
                _scrollingLeftEdge);
            if (DragColumn == dataGridColumn && DragIndicator != null)
            {
                _dragIndicatorLeftEdge = _scrollingLeftEdge + DragIndicatorOffset;
            }
        }
        
        _scrollingLeftEdge += dataGridColumn.ActualWidth;
        return headerViewItem.Bounds.Size;
    }
    
    private double _leafHeight = 0.0;
    private double _dragIndicatorLeftEdge;
    private double _frozenLeftEdge;
    private double _scrollingLeftEdge;
    
    protected override Size ArrangeOverride(Size finalSize)
    {
        if (OwningGrid == null)
        {
            return base.ArrangeOverride(finalSize);
        }
    
        if (OwningGrid.AutoSizingColumns)
        {
            // When we initially load an auto-column, we have to wait for all the rows to be measured
            // before we know its final desired size.  We need to trigger a new round of measures now
            // that the final sizes have been calculated.
            OwningGrid.AutoSizingColumns = false;
            return base.ArrangeOverride(finalSize);
        }
        
        _leafHeight = OwningGrid.ColumnHeaderHeight;
        bool   autoSizeHeight;
        if (double.IsNaN(_leafHeight))
        {
            // No explicit height values were set so we can autosize
            _leafHeight    = 0;
            autoSizeHeight = true;
        }
        else
        {
            autoSizeHeight = false;
        }
  
        foreach (DataGridColumn column in OwningGrid.ColumnsInternal.GetVisibleColumns())
        {
            DataGridHeaderViewItem? headerViewItem = null;
            if (column is IDataGridColumnGroupItemInternal groupItem)
            {
                headerViewItem = groupItem.GroupHeaderViewItem;
            }

            Debug.Assert(headerViewItem != null);
            
            // We need to track the largest height in order to auto-size
            if (autoSizeHeight)
            {
                _leafHeight = Math.Max(_leafHeight, headerViewItem.DesiredSize.Height);
            }
        }

        // If we're using star sizing (and we're not waiting for an auto-column to finish growing)
        // then we will resize all the columns to fit the available space.
        if (OwningGrid.UsesStarSizing && !OwningGrid.AutoSizingColumns)
        {
            // Since we didn't know the final widths of the columns until we resized,
            // we waited until now to measure each header
            foreach (DataGridColumn column in OwningGrid.ColumnsInternal.GetVisibleColumns())
            {
                if (autoSizeHeight)
                {
                    _leafHeight = Math.Max(_leafHeight, column.HeaderCell.DesiredSize.Height);
                }
            }
        }
        
        _dragIndicatorLeftEdge = 0;
        _frozenLeftEdge        = 0;
        _scrollingLeftEdge     = -OwningGrid.HorizontalOffset;
    
        var offsetX     = 0.0d;
        var remainWidth = finalSize.Width;
        foreach (IDataGridColumnGroupItem item in OwningGrid.ColumnGroups)
        {
            if (item is IDataGridColumnGroupItemInternal groupItem)
            {
                if (groupItem.GroupHeaderViewItem != null)
                {
                    ArrangeGroupRecursive(item, new Rect(offsetX, 0, remainWidth, finalSize.Height));
                    if (groupItem is DataGridColumn column)
                    {
                        offsetX += column.ActualWidth;
                        remainWidth -= column.ActualWidth;
                    }
                    else
                    {
                        var size = groupItem.GroupHeaderViewItem.DesiredSize;
                        offsetX += size.Width;
                        remainWidth -= size.Width;
                    }
                }
            }
        }
    
        var arrangeLeafY = finalSize.Height - _leafHeight;
        if (DragColumn != null)
        {
            if (DragIndicator != null)
            {
                EnsureColumnReorderingClip(DragIndicator, _leafHeight, _frozenLeftEdge, _dragIndicatorLeftEdge);
        
                var height = DragIndicator.Bounds.Height;
                if (height <= 0)
                    height = DragIndicator.DesiredSize.Height;
        
                DragIndicator.Arrange(new Rect(_dragIndicatorLeftEdge, arrangeLeafY, DragIndicator.Bounds.Width, height));
            }
        
            if (DropLocationIndicator != null)
            {
                if (DropLocationIndicator is Control element)
                {
                    EnsureColumnReorderingClip(element, _leafHeight, _frozenLeftEdge, DropLocationIndicatorOffset);
                }
        
                DropLocationIndicator.Arrange(new Rect(DropLocationIndicatorOffset, arrangeLeafY,
                    DropLocationIndicator.Bounds.Width, DropLocationIndicator.Bounds.Height));
            }
        }
        
        OwningGrid.HandleFillerColumnWidthNeeded(finalSize.Width);
        DataGridFillerColumn? fillerColumn = OwningGrid.ColumnsInternal.FillerColumn;
        Debug.Assert(fillerColumn != null);
        if (fillerColumn.FillerWidth > 0)
        {
            fillerColumn.HeaderCell.IsVisible = true;
            fillerColumn.HeaderCell.Arrange(new Rect(_scrollingLeftEdge, arrangeLeafY, fillerColumn.FillerWidth, _leafHeight));
        }
        else
        {
            fillerColumn.HeaderCell.IsVisible = false;
        }
        
        // This needs to be updated after the filler column is configured
        DataGridColumn? lastVisibleColumn = OwningGrid.ColumnsInternal.LastVisibleColumn;
        if (lastVisibleColumn != null)
        {
            lastVisibleColumn.HeaderCell.UpdateSeparatorVisibility(lastVisibleColumn);
        }
    
        return finalSize;
    }

    private void ArrangeGroupRecursive(IDataGridColumnGroupItem item, Rect rect)
    {
        if (item is IDataGridColumnGroupItemInternal groupItem)
        {
            var selfHeight  = 0.0d;
            var hasChildren = groupItem.GroupChildren.Count > 0;
            if (groupItem.GroupHeaderViewItem != null)
            {
                // 安排自己
                if (hasChildren)
                {
                    var size = groupItem.GroupHeaderViewItem.DesiredSize;
                    selfHeight = size.Height;
                    groupItem.GroupHeaderViewItem.Arrange(new Rect(rect.X, rect.Y, size.Width, hasChildren ? selfHeight : rect.Height));
                }
                else
                {
                    if (groupItem is DataGridColumn dataGridColumn)
                    {
                        var size = ArrangeLeafItem(dataGridColumn, rect);
                        selfHeight = size.Height;
                    }
                }
            }
            if (hasChildren)
            {
                var offsetX     = rect.X;
                var offsetY     = rect.Y + selfHeight;
                var remainWidth = rect.Width;
                foreach (var child in groupItem.GroupChildren)
                {
                    if (child is IDataGridColumnGroupItemInternal childGroupItem)
                    {
                        if (childGroupItem.GroupHeaderViewItem != null)
                        {
                            ArrangeGroupRecursive(childGroupItem, new Rect(offsetX, offsetY, remainWidth, rect.Height - selfHeight));
                            if (childGroupItem is DataGridColumn column)
                            {
                                offsetX     += column.ActualWidth;
                                remainWidth -= column.ActualWidth;
                            }
                            else
                            {
                                var size = childGroupItem.GroupHeaderViewItem.DesiredSize;
                                offsetX     += size.Width;
                                remainWidth -= size.Width;
                            }
                        }
                    }
                }
            }
        }
    }

    private static void EnsureColumnHeaderClip(DataGridHeaderViewItem headerViewItem, double width, double height,
                                               double frozenLeftEdge, double columnHeaderLeftEdge)
    {
        // Clip the cell only if it's scrolled under frozen columns.  Unfortunately, we need to clip in this case
        // because cells could be transparent
        if (frozenLeftEdge > columnHeaderLeftEdge)
        {
            RectangleGeometry rg    = new RectangleGeometry();
            double            xClip = Math.Min(width, frozenLeftEdge - columnHeaderLeftEdge);
            rg.Rect             = new Rect(xClip, 0, width - xClip, height);
            headerViewItem.Clip = rg;
        }
        else
        {
            headerViewItem.Clip = null;
        }
    }

    /// <summary>
    /// Clips the DragIndicator and DropLocationIndicator controls according to current ColumnHeaderPresenter constraints.
    /// </summary>
    /// <param name="control">The DragIndicator or DropLocationIndicator</param>
    /// <param name="height">The available height</param>
    /// <param name="frozenColumnsWidth">The width of the frozen column region</param>
    /// <param name="controlLeftEdge">The left edge of the control to clip</param>
    private void EnsureColumnReorderingClip(Control control, double height, double frozenColumnsWidth,
                                            double controlLeftEdge)
    {
        Debug.Assert(OwningGrid != null);
        Debug.Assert(DragColumn != null);
        double leftEdge  = 0;
        double rightEdge = OwningGrid.CellsWidth;
        double width     = control.Bounds.Width;
        if (DragColumn.IsFrozen)
        {
            // If we're dragging a frozen column, we want to clip the corresponding DragIndicator control when it goes
            // into the scrolling columns region, but not the DropLocationIndicator.
            if (control == DragIndicator)
            {
                rightEdge = Math.Min(rightEdge, frozenColumnsWidth);
            }
        }
        else if (OwningGrid.LeftFrozenColumnCount > 0)
        {
            // If we're dragging a scrolling column, we want to clip both the DragIndicator and the DropLocationIndicator
            // controls when they go into the frozen column range.
            leftEdge = frozenColumnsWidth;
        }

        RectangleGeometry? rg = null;
        if (leftEdge > controlLeftEdge)
        {
            rg = new RectangleGeometry();
            double xClip = Math.Min(width, leftEdge - controlLeftEdge);
            rg.Rect = new Rect(xClip, 0, width - xClip, height);
        }

        if (controlLeftEdge + width >= rightEdge)
        {
            if (rg == null)
            {
                rg = new RectangleGeometry();
            }

            rg.Rect = new Rect(rg.Rect.X, rg.Rect.Y, Math.Max(0, rightEdge - controlLeftEdge - rg.Rect.X), height);
        }

        control.Clip = rg;
    }
}