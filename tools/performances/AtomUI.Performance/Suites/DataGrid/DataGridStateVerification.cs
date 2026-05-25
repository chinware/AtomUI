using System.Collections;
using System.ComponentModel;
using System.Reflection;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunDataGridStateVerification()
    {
        var failures = new List<string>();
        VerifyDataGridPlainHeadersDoNotCreateFilterFlyouts(failures);
        VerifyDataGridFilterFlyoutContentIsLazy(failures);
        VerifyDataGridFilterIndicatorVisibilityTracksFilterItems(failures);
        VerifyDataGridColumnHeaderPointerHandlersUseClassHandlers(failures);
        VerifyDataGridColumnGroupHeaderPointerHandlersUseClassHandlers(failures);
        VerifyDataGridRowHeaderPointerHandlerUsesClassHandler(failures);
        VerifyDataGridRowGroupHeaderPointerHandlerUsesClassHandler(failures);
        VerifyDataGridCoreInputHandlersUseOverrides(failures);
        VerifyDataGridRowsPresenterScrollGestureUsesClassHandler(failures);
        VerifyDataGridRowsPresenterReusesClipGeometry(failures);
        VerifyDataGridRowReusesBottomGridLineClipGeometry(failures);
        VerifyDataGridCellHeaderStateBindings(failures);
        VerifyDataGridSpecialColumnsReleaseGridSubscriptionsOnClear(failures);
        VerifyDataGridColumnDragIndicatorCachesPen(failures);
        VerifyDataGridDetailsPresenterContentHeightInvalidatesMeasure(failures);
        VerifyDataGridRowExpanderDetailsVisibilityBindings(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("DataGrid state verification passed.");
            return true;
        }

        Console.Error.WriteLine("DataGrid state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyDataGridPlainHeadersDoNotCreateFilterFlyouts(ICollection<string> failures)
    {
        var grid = CreateBasicDataGrid(rowCount: 4, columnCount: 3);
        using var realized = RealizeControl(grid);

        var indicators = GetDataGridFilterIndicators(grid);
        Expect(indicators.Count == 4,
            $"Plain DataGrid should create one filter indicator slot per column plus the top-left header. Actual: {indicators.Count}.",
            failures);
        Expect(indicators.All(indicator => GetDataGridFilterFlyout(indicator) is null),
            "Plain DataGrid headers without filter items should not create filter flyout shells.",
            failures);
    }

    private static void VerifyDataGridFilterFlyoutContentIsLazy(ICollection<string> failures)
    {
        var grid = CreateFilterDataGrid(DataGridFilterMode.Menu);
        var realized = RealizeControl(grid);

        var indicators = GetDataGridFilterIndicators(grid);
        Expect(indicators.Count == 4,
            $"Filter DataGrid should create one filter indicator slot per column plus the top-left header. Actual: {indicators.Count}.",
            failures);

        var indicatorsWithFlyout = indicators
            .Where(indicator => GetDataGridFilterFlyout(indicator) is not null)
            .ToList();
        Expect(indicatorsWithFlyout.Count == 2,
            $"Only columns with filter items should create filter flyout shells. Actual: {indicatorsWithFlyout.Count}.",
            failures);

        var closedFlyoutItemCount = indicatorsWithFlyout
            .Select(indicator => GetDataGridFilterFlyout(indicator))
            .Where(flyout => flyout is not null)
            .Sum(flyout => GetDataGridFlyoutItemCount(flyout!));
        Expect(closedFlyoutItemCount == 0,
            $"Closed filter flyout shells should not materialize menu items. Actual item count: {closedFlyoutItemCount}.",
            failures);

        var firstFilterIndicator = indicatorsWithFlyout[0];
        MaterializeDataGridFilterFlyoutContent(firstFilterIndicator);
        var firstFlyout = GetDataGridFilterFlyout(firstFilterIndicator);
        Expect(firstFlyout is not null && GetDataGridFlyoutItemCount(firstFlyout) > 0,
            "First filter flyout should materialize its items immediately before opening.",
            failures);

        var secondFlyout = GetDataGridFilterFlyout(indicatorsWithFlyout[1]);
        Expect(secondFlyout is not null && GetDataGridFlyoutItemCount(secondFlyout) == 0,
            "Unopened sibling filter flyout should remain unmaterialized.",
            failures);

        realized.Dispose();
        Expect(indicators.All(indicator => GetDataGridFilterFlyout(indicator) is null),
            "Detached DataGrid filter indicators should clear their flyout shells.",
            failures);
    }

    private static void VerifyDataGridFilterIndicatorVisibilityTracksFilterItems(ICollection<string> failures)
    {
        var grid = CreateDataGridShell(4);
        grid.CanUserFilterColumns = true;

        var filterColumn = new DataGridTextColumn
        {
            Header           = "Name",
            Binding          = new Binding(nameof(PerfDataGridRow.Name)),
            FilterMemberPath = nameof(PerfDataGridRow.Name)
        };
        grid.Columns.Add(filterColumn);
        grid.Columns.Add(new DataGridTextColumn
        {
            Header  = "Age",
            Binding = new Binding(nameof(PerfDataGridRow.Age))
        });

        using var realized = RealizeControl(grid);
        Expect(CountVisibleDataGridFilterIndicators(grid) == 0,
            "Columns without filter items should keep filter indicators hidden even when filtering is enabled.",
            failures);

        filterColumn.Filters.Add(new DataGridFilterItem { Text = "Joe", Value = "Joe" });
        RefreshLayout(realized.Window);
        Expect(CountVisibleDataGridFilterIndicators(grid) == 1,
            $"Adding one filter item should reveal one filter indicator. Actual: {CountVisibleDataGridFilterIndicators(grid)}.",
            failures);
        Expect(GetDataGridFilterIndicators(grid).Count(indicator => GetDataGridFilterFlyout(indicator) is not null) == 1,
            "Adding one filter item should create exactly one closed filter flyout shell.",
            failures);

        filterColumn.Filters.Clear();
        RefreshLayout(realized.Window);
        Expect(CountVisibleDataGridFilterIndicators(grid) == 0,
            $"Clearing filter items should hide the filter indicator. Actual: {CountVisibleDataGridFilterIndicators(grid)}.",
            failures);
        Expect(GetDataGridFilterIndicators(grid).All(indicator => GetDataGridFilterFlyout(indicator) is null),
            "Clearing filter items should release the closed filter flyout shell.",
            failures);
    }

    private static void VerifyDataGridColumnHeaderPointerHandlersUseClassHandlers(ICollection<string> failures)
    {
        var grid = CreateBasicDataGrid(rowCount: 4, columnCount: 3);
        using var realized = RealizeControl(grid);

        var headers = GetDataGridColumnHeaders(grid);
        Expect(headers.Count >= 3,
            $"Basic 3-column grid should realize at least 3 standard column headers. Actual: {headers.Count}.",
            failures);

        foreach (var header in headers)
        {
            var localHandlerNames = GetLocalRoutedHandlerNames(header);
            Expect(!localHandlerNames.Contains("InputElement.PointerEntered"),
                "DataGrid column headers should handle PointerEntered through OnPointerEntered instead of per-instance handlers.",
                failures);
            Expect(!localHandlerNames.Contains("InputElement.PointerExited"),
                "DataGrid column headers should handle PointerExited through OnPointerExited instead of per-instance handlers.",
                failures);
            Expect(!localHandlerNames.Contains("InputElement.PointerPressed"),
                "DataGrid column headers should handle PointerPressed through class handlers instead of per-instance handlers.",
                failures);
            Expect(!localHandlerNames.Contains("InputElement.PointerReleased"),
                "DataGrid column headers should handle PointerReleased through class handlers instead of per-instance handlers.",
                failures);
            Expect(!localHandlerNames.Contains("InputElement.PointerMoved"),
                "DataGrid column headers should handle PointerMoved through class handlers instead of per-instance handlers.",
                failures);
        }

        var firstColumn = grid.Columns[0];
        var firstHeader = GetDataGridColumnHeader(firstColumn, failures);
        if (firstHeader is null)
        {
            return;
        }

        var pressedCount = 0;
        object? pressedSender = null;
        void HandleHeaderPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            pressedCount++;
            pressedSender = sender;
        }

        firstColumn.HeaderPointerPressed += HandleHeaderPointerPressed;
        try
        {
            RaiseDataGridHeaderPrimaryPointerPressed(firstHeader, realized.Window);
        }
        finally
        {
            firstColumn.HeaderPointerPressed -= HandleHeaderPointerPressed;
        }

        Expect(pressedCount == 1,
            $"DataGrid column HeaderPointerPressed should be forwarded exactly once from the header class handler. Actual: {pressedCount}.",
            failures);
        Expect(ReferenceEquals(pressedSender, firstColumn),
            "DataGrid column HeaderPointerPressed should preserve the DataGridColumn sender.",
            failures);
    }

    private static void VerifyDataGridColumnGroupHeaderPointerHandlersUseClassHandlers(ICollection<string> failures)
    {
        var grid = CreateColumnGroupDataGrid();
        using var realized = RealizeControl(grid);

        var groupHeaders = GetDataGridColumnGroupHeaders(grid);
        Expect(groupHeaders.Count >= 2,
            $"Column-group DataGrid should realize group header controls. Actual: {groupHeaders.Count}.",
            failures);

        foreach (var header in groupHeaders)
        {
            var localHandlerNames = GetLocalRoutedHandlerNames(header);
            Expect(!localHandlerNames.Contains("InputElement.PointerPressed"),
                "DataGrid column group headers should forward PointerPressed through class handlers instead of per-instance handlers.",
                failures);
            Expect(!localHandlerNames.Contains("InputElement.PointerReleased"),
                "DataGrid column group headers should forward PointerReleased through class handlers instead of per-instance handlers.",
                failures);
        }

        var firstGroup = grid.ColumnGroups.OfType<DataGridColumnGroupItem>().FirstOrDefault();
        var firstHeader = firstGroup == null
            ? null
            : GetDataGridColumnGroupHeader(firstGroup, groupHeaders);
        Expect(firstGroup != null && firstHeader != null,
            "Column-group DataGrid should expose a group item and group header for pointer forwarding verification.",
            failures);
        if (firstGroup == null || firstHeader == null)
        {
            return;
        }

        var pressedCount = 0;
        object? pressedSender = null;
        void HandleHeaderPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            pressedCount++;
            pressedSender = sender;
        }

        firstGroup.HeaderPointerPressed += HandleHeaderPointerPressed;
        try
        {
            RaiseDataGridHeaderPrimaryPointerPressed(firstHeader, realized.Window);
        }
        finally
        {
            firstGroup.HeaderPointerPressed -= HandleHeaderPointerPressed;
        }

        Expect(pressedCount == 1,
            $"DataGrid column group HeaderPointerPressed should be forwarded exactly once from the header class handler. Actual: {pressedCount}.",
            failures);
        Expect(ReferenceEquals(pressedSender, firstGroup),
            "DataGrid column group HeaderPointerPressed should preserve the DataGridColumnGroupItem sender.",
            failures);
    }

    private static void VerifyDataGridRowHeaderPointerHandlerUsesClassHandler(ICollection<string> failures)
    {
        var grid = CreateBasicDataGrid(rowCount: 4, columnCount: 3);
        grid.HeadersVisibility = DataGridHeadersVisibility.All;
        grid.SelectionMode     = DataGridSelectionMode.Single;
        using var realized = RealizeControl(grid);

        var rowHeaders = GetDataGridRowHeaders(grid);
        Expect(rowHeaders.Count >= 4,
            $"DataGrid with row headers visible should realize row header controls. Actual: {rowHeaders.Count}.",
            failures);

        foreach (var header in rowHeaders)
        {
            var localHandlerNames = GetLocalRoutedHandlerNames(header);
            Expect(!localHandlerNames.Contains("InputElement.PointerPressed"),
                "DataGrid row headers should handle PointerPressed through class handlers instead of per-instance handlers.",
                failures);
        }

        var firstRowHeader = rowHeaders
            .Select(header => new
            {
                Header = header,
                Owner  = GetDataGridRowHeaderOwner(header)
            })
            .FirstOrDefault(x => x.Owner is DataGridRow);
        Expect(firstRowHeader?.Owner is DataGridRow,
            "DataGrid row header should expose a DataGridRow owner for pointer behavior verification.",
            failures);
        if (firstRowHeader?.Owner is not DataGridRow row)
        {
            return;
        }

        var rowSlot = GetDataGridRowSlot(row);
        Expect(rowSlot >= 0,
            $"DataGrid row header owner should expose a realized row slot. Actual: {rowSlot}.",
            failures);

        RaiseDataGridHeaderPrimaryPointerPressed(firstRowHeader.Header, realized.Window);

        Expect(row.IsSelected,
            "DataGrid row header left press should still select the owning row after moving to a class handler.",
            failures);
        Expect(GetDataGridCurrentSlot(grid) == rowSlot,
            "DataGrid row header left press should still update DataGrid.CurrentSlot after moving to a class handler.",
            failures);
    }

    private static void VerifyDataGridRowGroupHeaderPointerHandlerUsesClassHandler(ICollection<string> failures)
    {
        var grid = CreateRowGroupDataGrid();
        using var realized = RealizeControl(grid);

        var groupHeaders = GetDataGridRowGroupHeaders(grid);
        Expect(groupHeaders.Count >= 1,
            $"Grouped DataGrid should realize row group header controls. Actual: {groupHeaders.Count}.",
            failures);

        foreach (var header in groupHeaders)
        {
            var localHandlerNames = GetLocalRoutedHandlerNames(header);
            Expect(!localHandlerNames.Contains("InputElement.PointerPressed"),
                "DataGrid row group headers should handle PointerPressed through class handlers instead of per-instance handlers.",
                failures);
        }

        var firstGroupHeader = groupHeaders.FirstOrDefault();
        if (firstGroupHeader == null)
        {
            return;
        }

        var groupSlot = GetDataGridRowGroupHeaderSlot(firstGroupHeader);
        Expect(groupSlot >= 0,
            $"DataGrid row group header should expose a realized group slot. Actual: {groupSlot}.",
            failures);

        RaiseDataGridHeaderPrimaryPointerPressed(firstGroupHeader, realized.Window);

        Expect(GetDataGridCurrentSlot(grid) == groupSlot,
            "DataGrid row group header left press should still update DataGrid.CurrentSlot after moving to a class handler.",
            failures);
    }

    private static void VerifyDataGridCoreInputHandlersUseOverrides(ICollection<string> failures)
    {
        var grid = CreateBasicDataGrid(rowCount: 4, columnCount: 3);
        using var realized = RealizeControl(grid);

        var localHandlerNames = GetLocalRoutedHandlerNames(grid);
        Expect(!localHandlerNames.Contains("InputElement.KeyDown"),
            "DataGrid should process KeyDown through OnKeyDown instead of a per-instance handler.",
            failures);
        Expect(!localHandlerNames.Contains("InputElement.KeyUp"),
            "DataGrid should process KeyUp through OnKeyUp instead of a per-instance handler.",
            failures);
        Expect(!localHandlerNames.Contains("InputElement.GotFocus"),
            "DataGrid should process GotFocus through OnGotFocus instead of a per-instance handler.",
            failures);
        Expect(!localHandlerNames.Contains("InputElement.LostFocus"),
            "DataGrid should process LostFocus through OnLostFocus instead of a per-instance handler.",
            failures);

        grid.RaiseEvent(new FocusChangedEventArgs(InputElement.GotFocusEvent)
        {
            NewFocusedElement = grid,
            NavigationMethod  = NavigationMethod.Tab
        });
        Expect(GetDataGridContainsFocus(grid),
            "DataGrid GotFocus override should still update ContainsFocus.",
            failures);

        Expect(SetDataGridCurrentCell(grid, columnIndex: 0, slot: 0),
            "DataGrid should accept a reflected current-cell setup for keyboard navigation verification.",
            failures);

        var keyDown = new KeyEventArgs
        {
            RoutedEvent  = InputElement.KeyDownEvent,
            Key          = Key.Down,
            KeyModifiers = KeyModifiers.None
        };
        grid.RaiseEvent(keyDown);
        RefreshLayout(realized.Window);

        Expect(keyDown.Handled,
            "DataGrid OnKeyDown override should still handle a Down key navigation request.",
            failures);
        Expect(GetDataGridCurrentSlot(grid) == 1,
            $"DataGrid Down key should still move the current slot from 0 to 1. Actual: {GetDataGridCurrentSlot(grid)}.",
            failures);
    }

    private static void VerifyDataGridRowsPresenterScrollGestureUsesClassHandler(ICollection<string> failures)
    {
        var grid = CreateBasicDataGrid(rowCount: 40, columnCount: 3);
        using var realized = RealizeControl(grid);

        var rowsPresenters = GetDataGridRowsPresenters(grid);
        Expect(rowsPresenters.Count == 1,
            $"DataGrid should realize one rows presenter. Actual: {rowsPresenters.Count}.",
            failures);

        foreach (var presenter in rowsPresenters)
        {
            var localHandlerNames = GetLocalRoutedHandlerNames(presenter);
            Expect(!localHandlerNames.Contains("InputElement.ScrollGesture"),
                "DataGrid rows presenter should handle ScrollGesture through a class handler instead of a per-instance handler.",
                failures);
        }

        var rowsPresenter = rowsPresenters.FirstOrDefault();
        if (rowsPresenter == null)
        {
            return;
        }

        var initialVerticalOffset = GetDataGridVerticalOffset(grid);
        var scrollGesture = new ScrollGestureEventArgs(
            ScrollGestureEventArgs.GetNextFreeId(),
            new Vector(0, 48));

        rowsPresenter.RaiseEvent(scrollGesture);
        RefreshLayout(realized.Window);

        Expect(scrollGesture.Handled,
            "DataGrid rows presenter ScrollGesture class handler should still mark handled gestures that scroll the grid.",
            failures);
        Expect(GetDataGridVerticalOffset(grid) > initialVerticalOffset,
            $"DataGrid rows presenter ScrollGesture should still update vertical offset. Initial: {initialVerticalOffset}, actual: {GetDataGridVerticalOffset(grid)}.",
            failures);
    }

    private static void VerifyDataGridRowsPresenterReusesClipGeometry(ICollection<string> failures)
    {
        var grid = CreateBasicDataGrid(rowCount: 12, columnCount: 4);
        using var realized = RealizeControl(grid);

        var rowsPresenter = GetDataGridRowsPresenters(grid).FirstOrDefault();
        if (rowsPresenter == null)
        {
            Expect(false,
                "DataGrid should realize one rows presenter for clip geometry verification.",
                failures);
            return;
        }

        var firstClip = rowsPresenter.Clip as RectangleGeometry;
        Expect(firstClip is not null,
            $"DataGrid rows presenter clip should be a RectangleGeometry. Actual: {rowsPresenter.Clip?.GetType().Name ?? "null"}.",
            failures);
        if (firstClip == null)
        {
            return;
        }

        var firstRect = firstClip.Rect;
        Expect(firstRect.Width > 0 && firstRect.Height > 0,
            $"DataGrid rows presenter clip should have non-zero bounds. Actual: {firstRect}.",
            failures);

        rowsPresenter.InvalidateArrange();
        RefreshLayout(realized.Window);

        var secondClip = rowsPresenter.Clip as RectangleGeometry;
        Expect(ReferenceEquals(firstClip, secondClip),
            "DataGrid rows presenter should reuse its clip RectangleGeometry across repeated arrange passes.",
            failures);

        grid.Width = 520;
        RefreshLayout(realized.Window);

        var resizedClip = rowsPresenter.Clip as RectangleGeometry;
        Expect(ReferenceEquals(firstClip, resizedClip),
            "DataGrid rows presenter should keep reusing the same clip RectangleGeometry after a size change.",
            failures);
        Expect(resizedClip is not null && !resizedClip.Rect.Equals(firstRect),
            $"DataGrid rows presenter reused clip geometry should update Rect after a size change. Before: {firstRect}, after: {resizedClip?.Rect}.",
            failures);
    }

    private static void VerifyDataGridRowReusesBottomGridLineClipGeometry(ICollection<string> failures)
    {
        var grid = CreateBasicDataGrid(rowCount: 12, columnCount: 10);
        grid.Width = 260;
        using var realized = RealizeControl(grid);

        var row = GetDataGridRows(grid).FirstOrDefault();
        Expect(row is not null,
            "DataGrid should realize rows for bottom grid-line clip verification.",
            failures);
        if (row is null)
        {
            return;
        }

        var bottomGridLine = GetDataGridRowBottomGridLine(row);
        Expect(bottomGridLine is not null,
            "DataGrid row should realize its bottom grid-line template part.",
            failures);
        if (bottomGridLine is null)
        {
            return;
        }

        var firstClip = bottomGridLine.Clip as RectangleGeometry;
        Expect(firstClip is not null,
            $"DataGrid row bottom grid-line clip should be a RectangleGeometry. Actual: {bottomGridLine.Clip?.GetType().Name ?? "null"}.",
            failures);
        if (firstClip is null)
        {
            return;
        }

        var firstRect = firstClip.Rect;
        Expect(firstRect.Width > 0 && firstRect.Height >= 0,
            $"DataGrid row bottom grid-line clip should have usable bounds. Actual: {firstRect}.",
            failures);

        row.InvalidateArrange();
        RefreshLayout(realized.Window);

        var secondClip = bottomGridLine.Clip as RectangleGeometry;
        Expect(ReferenceEquals(firstClip, secondClip),
            "DataGrid row should reuse its bottom grid-line clip RectangleGeometry across repeated arrange passes.",
            failures);

        Expect(UpdateDataGridHorizontalOffset(grid, 80),
            "DataGrid should accept a reflected horizontal offset update for row bottom grid-line clip verification.",
            failures);
        RefreshLayout(realized.Window);

        var scrolledClip = bottomGridLine.Clip as RectangleGeometry;
        Expect(ReferenceEquals(firstClip, scrolledClip),
            "DataGrid row should keep reusing the same bottom grid-line clip RectangleGeometry after horizontal offset changes.",
            failures);
        Expect(scrolledClip is not null && !scrolledClip.Rect.Equals(firstRect),
            $"DataGrid row reused bottom grid-line clip geometry should update Rect after horizontal offset changes. Before: {firstRect}, after: {scrolledClip?.Rect}.",
            failures);
    }

    private static void VerifyDataGridCellHeaderStateBindings(ICollection<string> failures)
    {
        var grid = CreateDataGridShell(4);
        grid.CanUserSortColumns = true;

        var firstColumn = new DataGridTextColumn
        {
            Header = "Name",
            Binding = new Binding(nameof(PerfDataGridRow.Name)),
            SortMemberPath = nameof(PerfDataGridRow.Name)
        };
        grid.Columns.Add(firstColumn);
        grid.Columns.Add(new DataGridTextColumn
        {
            Header = "Age",
            Binding = new Binding(nameof(PerfDataGridRow.Age)),
            SortMemberPath = nameof(PerfDataGridRow.Age)
        });

        var realized = RealizeControl(grid);
        var firstColumnCells = GetDataGridCells(grid)
            .Where(cell => GetDataGridCellColumnIndex(cell) == 0)
            .ToList();
        Expect(firstColumnCells.Count > 0,
            "Sortable DataGrid should realize first-column cells for header state binding verification.",
            failures);

        var firstHeader = GetDataGridColumnHeader(firstColumn, failures);
        if (firstHeader != null)
        {
            SetDataGridColumnHeaderProperty(firstHeader, "CurrentSortingState", ListSortDirection.Ascending, failures);
            RefreshLayout(realized.Window);
            Expect(firstColumnCells.All(cell => cell.IsSorting),
                "DataGrid cells should track the owning header active sort state.",
                failures);

            SetDataGridColumnHeaderProperty(firstHeader, "CurrentSortingState", null, failures);
            RefreshLayout(realized.Window);
            Expect(firstColumnCells.All(cell => !cell.IsSorting),
                "DataGrid cells should clear sorting state when the owning header sort is cleared.",
                failures);

            SetDataGridColumnHeaderDragMode(firstHeader, "Reorder", failures);
            RefreshLayout(realized.Window);
            Expect(firstColumnCells.All(cell => cell.OwningColumnDragging),
                "DataGrid cells should track the owning header reorder drag state.",
                failures);

            SetDataGridColumnHeaderDragMode(firstHeader, "None", failures);
            RefreshLayout(realized.Window);
            Expect(firstColumnCells.All(cell => !cell.OwningColumnDragging),
                "DataGrid cells should clear dragging state when the owning header drag mode is reset.",
                failures);
        }

        realized.Dispose();
        foreach (var cell in firstColumnCells)
        {
            ExpectDataGridCellHeaderSubscriptionsReleased(cell, failures);
        }
    }

    private static void VerifyDataGridSpecialColumnsReleaseGridSubscriptionsOnClear(ICollection<string> failures)
    {
        var grid = CreateDataGridShell(4);
        grid.CanUserReorderRows = true;

        var detailColumn = new DataGridDetailExpanderColumn();
        var reorderColumn = new DataGridRowReorderColumn();
        var selectionColumn = new DataGridSelectionColumn();
        var checkBoxColumn = new DataGridCheckBoxColumn
        {
            Binding = new Binding(nameof(PerfDataGridRow.Age))
        };
        var operationColumn = CreateDataGridOperationColumn(failures);

        grid.Columns.Add(detailColumn);
        grid.Columns.Add(reorderColumn);
        grid.Columns.Add(selectionColumn);
        grid.Columns.Add(checkBoxColumn);
        if (operationColumn != null)
        {
            grid.Columns.Add(operationColumn);
        }
        grid.Columns.Add(new DataGridTextColumn
        {
            Header = "Name",
            Binding = new Binding(nameof(PerfDataGridRow.Name))
        });

        using var realized = RealizeControl(grid);

        grid.Columns.Clear();

        ExpectColumnOwningGridReleased(detailColumn, failures);
        ExpectColumnOwningGridReleased(reorderColumn, failures);
        ExpectColumnOwningGridReleased(selectionColumn, failures);
        ExpectColumnOwningGridReleased(checkBoxColumn, failures);
        if (operationColumn != null)
        {
            ExpectColumnOwningGridReleased(operationColumn, failures);
        }
    }

    private static void VerifyDataGridColumnDragIndicatorCachesPen(ICollection<string> failures)
    {
        var type = typeof(DataGrid).Assembly.GetType("AtomUI.Desktop.Controls.DataGridColumnDraggingOverIndicator");
        Expect(type != null,
            "DataGrid column drag indicator type should be available for render resource verification.",
            failures);
        if (type == null)
        {
            return;
        }

        var indicator = Activator.CreateInstance(type, nonPublic: true) as Control;
        Expect(indicator != null,
            "DataGrid column drag indicator should be constructible for render resource verification.",
            failures);
        if (indicator == null)
        {
            return;
        }

        var getPen = type.GetMethod("GetIndicatorPen", BindingFlags.Instance | BindingFlags.NonPublic);
        Expect(getPen != null,
            "DataGrid column drag indicator should expose a cached render pen factory.",
            failures);
        if (getPen == null)
        {
            return;
        }

        var firstPen = getPen.Invoke(indicator, null);
        var secondPen = getPen.Invoke(indicator, null);
        Expect(ReferenceEquals(firstPen, secondPen),
            "DataGrid column drag indicator should reuse the same dashed Pen while Foreground is unchanged.",
            failures);

        TextElement.SetForeground(indicator, Brushes.Red);

        var thirdPen = getPen.Invoke(indicator, null);
        var fourthPen = getPen.Invoke(indicator, null);
        Expect(!ReferenceEquals(firstPen, thirdPen),
            "DataGrid column drag indicator should rebuild its cached Pen after Foreground changes.",
            failures);
        Expect(ReferenceEquals(thirdPen, fourthPen),
            "DataGrid column drag indicator should reuse the rebuilt dashed Pen while the new Foreground is unchanged.",
            failures);
    }

    private static void VerifyDataGridDetailsPresenterContentHeightInvalidatesMeasure(ICollection<string> failures)
    {
        var presenter = new DataGridDetailsPresenter();

        presenter.Measure(new Size(100, 100));
        Expect(presenter.IsMeasureValid,
            "DataGrid details presenter should have a valid measure after Measure().",
            failures);

        presenter.ContentHeight = 24;
        Expect(!presenter.IsMeasureValid,
            "DataGrid details presenter ContentHeight changes should invalidate measure.",
            failures);
    }

    private static void VerifyDataGridRowExpanderDetailsVisibilityBindings(ICollection<string> failures)
    {
        var grid = CreateDataGridShell(4);
        grid.Columns.Add(new DataGridDetailExpanderColumn());
        grid.Columns.Add(new DataGridTextColumn
        {
            Header  = "Name",
            Binding = new Binding(nameof(PerfDataGridRow.Name))
        });

        var realized = RealizeControl(grid);
        var expander = GetDataGridRowExpanders(grid).FirstOrDefault();
        Expect(expander != null,
            "DataGrid with a detail expander column should realize row expander controls.",
            failures);
        if (expander == null)
        {
            realized.Dispose();
            return;
        }

        var row = expander.GetVisualAncestors().OfType<DataGridRow>().FirstOrDefault();
        Expect(row != null,
            "DataGrid row expander should be hosted inside a DataGridRow for details visibility verification.",
            failures);
        if (row == null)
        {
            realized.Dispose();
            return;
        }

        expander.IsChecked = true;
        RefreshLayout(realized.Window);
        Expect(row.IsDetailsVisible,
            "Checking a DataGrid row expander should update the owning row details visibility.",
            failures);

        row.IsDetailsVisible = false;
        RefreshLayout(realized.Window);
        Expect(expander.IsChecked == false,
            "Clearing row details visibility should update the DataGrid row expander checked state.",
            failures);

        row.IsDetailsVisible = true;
        RefreshLayout(realized.Window);
        Expect(expander.IsChecked == true,
            "Setting row details visibility should update the DataGrid row expander checked state.",
            failures);

        realized.Dispose();
        Expect(GetPrivateFieldValue(expander, "_detailsVisibilitySubscription") is null,
            "DataGrid row expander should release details visibility subscriptions when its row unloads or detaches.",
            failures);
        Expect(GetPrivateFieldValue(expander, "_owningRow") is null,
            "DataGrid row expander should clear its owning row reference when its row unloads or detaches.",
            failures);
    }

    private static List<Control> GetDataGridFilterIndicators(Control root)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<Control>()
                   .Where(control => control.GetType().Name == "DataGridFilterIndicator")
                   .ToList();
    }

    private static List<Control> GetDataGridColumnHeaders(Control root)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<Control>()
                   .Where(control => control.GetType().Name == "DataGridColumnHeader")
                   .ToList();
    }

    private static List<Control> GetDataGridColumnGroupHeaders(Control root)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<Control>()
                   .Where(control => control.GetType().Name == "DataGridColumnGroupHeader")
                   .ToList();
    }

    private static List<DataGridRowHeader> GetDataGridRowHeaders(Control root)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<DataGridRowHeader>()
                   .Where(header => GetDataGridRowHeaderOwner(header) is DataGridRow)
                   .ToList();
    }

    private static List<DataGridRowGroupHeader> GetDataGridRowGroupHeaders(Control root)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<DataGridRowGroupHeader>()
                   .ToList();
    }

    private static List<DataGridRowsPresenter> GetDataGridRowsPresenters(Control root)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<DataGridRowsPresenter>()
                   .ToList();
    }

    private static List<DataGridRow> GetDataGridRows(Control root)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<DataGridRow>()
                   .ToList();
    }

    private static Control? GetDataGridRowHeaderOwner(DataGridRowHeader header)
    {
        return header.GetType()
                     .GetProperty("Owner", BindingFlags.Instance | BindingFlags.NonPublic)
                     ?.GetValue(header) as Control;
    }

    private static int GetDataGridRowSlot(DataGridRow row)
    {
        return row.GetType()
                  .GetProperty("Slot", BindingFlags.Instance | BindingFlags.NonPublic)
                  ?.GetValue(row) as int? ?? -1;
    }

    private static int GetDataGridCurrentSlot(DataGrid grid)
    {
        return typeof(DataGrid)
                   .GetProperty("CurrentSlot", BindingFlags.Instance | BindingFlags.NonPublic)
                   ?.GetValue(grid) as int? ?? -1;
    }

    private static double GetDataGridVerticalOffset(DataGrid grid)
    {
        return typeof(DataGrid)
                   .GetProperty("VerticalOffset", BindingFlags.Instance | BindingFlags.NonPublic)
                   ?.GetValue(grid) as double? ?? 0;
    }

    private static bool GetDataGridContainsFocus(DataGrid grid)
    {
        return typeof(DataGrid)
                   .GetProperty("ContainsFocus", BindingFlags.Instance | BindingFlags.NonPublic)
                   ?.GetValue(grid) as bool? ?? false;
    }

    private static bool SetDataGridCurrentCell(DataGrid grid, int columnIndex, int slot)
    {
        var method = typeof(DataGrid).GetMethod(
            "SetCurrentCellCore",
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: [typeof(int), typeof(int)],
            modifiers: null);
        return method?.Invoke(grid, new object[] { columnIndex, slot }) as bool? ?? false;
    }

    private static bool UpdateDataGridHorizontalOffset(DataGrid grid, double horizontalOffset)
    {
        var method = typeof(DataGrid).GetMethod(
            "UpdateHorizontalOffset",
            BindingFlags.Instance | BindingFlags.NonPublic);
        return method?.Invoke(grid, new object[] { horizontalOffset }) as bool? ?? false;
    }

    private static int GetDataGridRowGroupHeaderSlot(DataGridRowGroupHeader header)
    {
        var rowGroupInfo = header.GetType()
                                 .GetProperty("RowGroupInfo", BindingFlags.Instance | BindingFlags.NonPublic)
                                 ?.GetValue(header);
        return rowGroupInfo?.GetType()
                            .GetProperty("Slot", BindingFlags.Instance | BindingFlags.Public)
                            ?.GetValue(rowGroupInfo) as int? ?? -1;
    }

    private static Control? GetDataGridColumnGroupHeader(
        DataGridColumnGroupItem groupItem,
        IEnumerable<Control> groupHeaders)
    {
        foreach (var header in groupHeaders)
        {
            var owningGroupItem = header.GetType()
                                        .GetProperty("OwningGroupItem", BindingFlags.Instance | BindingFlags.NonPublic)
                                        ?.GetValue(header);
            if (ReferenceEquals(owningGroupItem, groupItem))
            {
                return header;
            }
        }
        return null;
    }

    private static List<ToggleButton> GetDataGridRowExpanders(Control root)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<ToggleButton>()
                   .Where(control => control.GetType().Name == "DataGridRowExpander")
                   .ToList();
    }

    private static List<DataGridCell> GetDataGridCells(Control root)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<DataGridCell>()
                   .ToList();
    }

    private static Avalonia.Controls.Shapes.Rectangle? GetDataGridRowBottomGridLine(DataGridRow row)
    {
        return GetPrivateFieldValue(row, "_bottomGridLine") as Avalonia.Controls.Shapes.Rectangle;
    }

    private static int CountVisibleDataGridFilterIndicators(Control root)
    {
        return GetDataGridFilterIndicators(root).Count(indicator => indicator.IsVisible);
    }

    private static HashSet<string> GetLocalRoutedHandlerNames(Control control)
    {
        var field = typeof(Interactive).GetField("_eventHandlers", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field?.GetValue(control) is not IDictionary eventHandlers)
        {
            return [];
        }

        var names = new HashSet<string>();
        foreach (DictionaryEntry entry in eventHandlers)
        {
            if (entry.Key != null)
            {
                names.Add(entry.Key.ToString() ?? string.Empty);
            }
        }
        return names;
    }

    private static DataGridColumn? CreateDataGridOperationColumn(ICollection<string> failures)
    {
        var type = typeof(DataGrid).Assembly.GetType("AtomUI.Desktop.Controls.DataGridOperationColumn");
        Expect(type != null,
            "DataGrid operation column type should be available for subscription lifecycle verification.",
            failures);
        return type == null
            ? null
            : (DataGridColumn?)Activator.CreateInstance(type, nonPublic: true);
    }

    private static void ExpectColumnOwningGridReleased(DataGridColumn column, ICollection<string> failures)
    {
        var field = column.GetType().GetField("_owningGrid", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null)
        {
            return;
        }

        Expect(field.GetValue(column) is null,
            $"{column.GetType().Name} should release its cached owning grid and grid event subscriptions when DataGrid.Columns.Clear() detaches the column.",
            failures);
    }

    private static int GetDataGridCellColumnIndex(DataGridCell cell)
    {
        var property = typeof(DataGridCell).GetProperty(
            "ColumnIndex",
            BindingFlags.Instance | BindingFlags.NonPublic);
        return property?.GetValue(cell) as int? ?? -1;
    }

    private static Control? GetDataGridColumnHeader(DataGridColumn column, ICollection<string> failures)
    {
        var property = typeof(DataGridColumn).GetProperty(
            "HeaderCell",
            BindingFlags.Instance | BindingFlags.NonPublic);
        var header = property?.GetValue(column) as Control;
        Expect(header != null,
            "DataGrid column header should be available for cell header state binding verification.",
            failures);
        return header;
    }

    private static void SetDataGridColumnHeaderProperty(
        Control header,
        string propertyName,
        object? value,
        ICollection<string> failures)
    {
        var property = header.GetType().GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        Expect(property != null,
            $"DataGrid column header should expose {propertyName} for state binding verification.",
            failures);
        property?.SetValue(header, value);
    }

    private static void SetDataGridColumnHeaderDragMode(
        Control header,
        string modeName,
        ICollection<string> failures)
    {
        var dragModeType = header.GetType().GetNestedType("DragMode", BindingFlags.NonPublic);
        Expect(dragModeType != null,
            "DataGrid column header DragMode enum should be available for state binding verification.",
            failures);
        if (dragModeType == null)
        {
            return;
        }

        SetDataGridColumnHeaderProperty(header, "HeaderDragMode", Enum.Parse(dragModeType, modeName), failures);
    }

    private static void ExpectDataGridCellHeaderSubscriptionsReleased(
        DataGridCell cell,
        ICollection<string> failures)
    {
        Expect(GetPrivateFieldValue(cell, "_sortingStateSubscription") is null,
            "DataGrid cells should release header sort-state subscriptions when detached.",
            failures);
        Expect(GetPrivateFieldValue(cell, "_headerDragModeSubscription") is null,
            "DataGrid cells should release header drag-state subscriptions when detached.",
            failures);
    }

    private static object? GetPrivateFieldValue(object instance, string fieldName)
    {
        return instance.GetType()
                       .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)
                       ?.GetValue(instance);
    }

    private static void RaiseDataGridHeaderPrimaryPointerPressed(Control target, Visual root)
    {
        var pointer = new Avalonia.Input.Pointer(
            Avalonia.Input.Pointer.GetNextFreeId(),
            PointerType.Mouse,
            true);
        var properties = new PointerPointProperties(
            RawInputModifiers.LeftMouseButton,
            PointerUpdateKind.LeftButtonPressed);
        var localPoint = new Point(
            Math.Max(1, target.Bounds.Width / 2),
            Math.Max(1, target.Bounds.Height / 2));
        var rootPoint = target.TranslatePoint(localPoint, root) ?? localPoint;

        target.RaiseEvent(new PointerPressedEventArgs(
            target,
            pointer,
            root,
            rootPoint,
            1,
            properties,
            KeyModifiers.None));
    }

    private static PopupFlyoutBase? GetDataGridFilterFlyout(Control indicator)
    {
        var property = indicator.GetType().GetProperty(
            "Flyout",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        return property?.GetValue(indicator) as PopupFlyoutBase;
    }

    private static void MaterializeDataGridFilterFlyoutContent(Control indicator)
    {
        var method = indicator.GetType().GetMethod(
            "MaterializeFlyoutContent",
            BindingFlags.Instance | BindingFlags.NonPublic);
        method?.Invoke(indicator, null);
    }

    private static int GetDataGridFlyoutItemCount(PopupFlyoutBase flyout)
    {
        var items = flyout.GetType()
                          .GetProperty("Items", BindingFlags.Instance | BindingFlags.Public)?
                          .GetValue(flyout) as IEnumerable;
        return CountDataGridFlyoutItems(items);
    }

    private static int CountDataGridFlyoutItems(IEnumerable? items)
    {
        if (items is null)
        {
            return 0;
        }

        var count = 0;
        foreach (var item in items)
        {
            count++;
            if (item is ItemsControl itemsControl)
            {
                count += CountDataGridFlyoutItems(itemsControl.Items);
            }
        }
        return count;
    }
}
