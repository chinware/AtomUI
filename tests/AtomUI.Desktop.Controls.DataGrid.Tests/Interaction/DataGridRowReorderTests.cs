using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.Data;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.DataGrid.Interaction;

public class DataGridRowReorderTests
{
    static DataGridRowReorderTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Two_DataGrids_Keep_Row_Drag_Sessions_Isolated()
    {
        var firstItems  = CreateItems(4);
        var secondItems = CreateItems(4);
        var (firstGrid, firstWindow, firstHandle)    = Realize(firstItems);
        var (secondGrid, secondWindow, secondHandle) = Realize(secondItems);

        try
        {
            var firstDrag   = BeginPointerDrag(firstHandle, firstWindow);
            var firstPoint  = MovePointerDragToRow(firstGrid, firstHandle, firstWindow, firstDrag, targetIndex: 2);
            var secondDrag  = BeginPointerDrag(secondHandle, secondWindow);
            var secondPoint = MovePointerDragToRow(secondGrid, secondHandle, secondWindow, secondDrag, targetIndex: 1);

            EndPointerDrag(firstHandle, firstWindow, firstDrag, firstPoint);
            EndPointerDrag(secondHandle, secondWindow, secondDrag, secondPoint);

            firstItems.Select(item => item.Name)
                      .ShouldBe(["Row 1", "Row 2", "Row 0", "Row 3"]);
            secondItems.Select(item => item.Name)
                       .ShouldBe(["Row 1", "Row 0", "Row 2", "Row 3"]);
        }
        finally
        {
            Close(firstWindow);
            Close(secondWindow);
        }
    }

    [Fact]
    public void RowReordering_Starts_Only_After_Drag_Threshold()
    {
        var items = CreateItems(4);
        var (grid, window, handle) = Realize(items);
        var reorderingCount = 0;
        grid.RowReordering += (_, _) => reorderingCount++;

        try
        {
            var drag = BeginPointerDrag(handle, window);

            MovePointerDrag(
                handle,
                window,
                drag,
                drag.Start + new Vector(Constants.DragThreshold, 0),
                timestamp: 1);
            reorderingCount.ShouldBe(0);

            MovePointerDrag(
                handle,
                window,
                drag,
                drag.Start + new Vector(Constants.DragThreshold + 1, 0),
                timestamp: 2);
            reorderingCount.ShouldBe(1);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void RowReordering_Cancel_Ends_The_Session_Without_Moving()
    {
        var items = CreateItems(4);
        var (grid, window, handle) = Realize(items);
        var rowsPresenter = grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Single();
        var reorderedCount = 0;
        grid.RowReordering += (_, e) => e.Cancel = true;
        grid.RowReordered  += (_, _) => reorderedCount++;

        try
        {
            var drag   = BeginPointerDrag(handle, window);
            var target = MovePointerDragToRow(grid, handle, window, drag, targetIndex: 2);

            EndPointerDrag(handle, window, drag, target);

            items.Select(item => item.Name)
                 .ShouldBe(["Row 0", "Row 1", "Row 2", "Row 3"]);
            drag.Pointer.Captured.ShouldBeNull();
            rowsPresenter.DraggedRowIndex.ShouldBeNull();
            rowsPresenter.DragRowOffset.ShouldBe(0);
            reorderedCount.ShouldBe(0);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void Replacing_ItemsSource_During_RowReordering_Cancels_The_Captured_View()
    {
        var items       = CreateItems(4);
        var replacement = CreateItems(2);
        var (grid, window, handle) = Realize(items);
        var rowsPresenter = grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Single();
        var reorderedCount = 0;
        grid.RowReordering += (_, _) => grid.ItemsSource = replacement;
        grid.RowReordered  += (_, _) => reorderedCount++;

        try
        {
            var drag = BeginPointerDrag(handle, window);

            Should.NotThrow(() => MovePointerDragToRow(grid, handle, window, drag, targetIndex: 2));

            grid.ItemsSource.ShouldBeSameAs(replacement);
            items.Select(item => item.Name)
                 .ShouldBe(["Row 0", "Row 1", "Row 2", "Row 3"]);
            drag.Pointer.Captured.ShouldBeNull();
            rowsPresenter.DraggedRowIndex.ShouldBeNull();
            rowsPresenter.DragRowOffset.ShouldBe(0);
            reorderedCount.ShouldBe(0);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void Losing_Move_Capability_During_RowReordering_Cancels_The_Session()
    {
        var items = CreateItems(4);
        var (grid, window, handle) = Realize(items);
        var rowsPresenter = grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Single();
        var reorderedCount = 0;
        grid.RowReordering += (_, _) => grid.CollectionView!.Filter = _ => true;
        grid.RowReordered  += (_, _) => reorderedCount++;

        try
        {
            var drag = BeginPointerDrag(handle, window);

            Should.NotThrow(() => MovePointerDragToRow(grid, handle, window, drag, targetIndex: 2));

            items.Select(item => item.Name)
                 .ShouldBe(["Row 0", "Row 1", "Row 2", "Row 3"]);
            drag.Pointer.Captured.ShouldBeNull();
            rowsPresenter.DraggedRowIndex.ShouldBeNull();
            rowsPresenter.DragRowOffset.ShouldBe(0);
            reorderedCount.ShouldBe(0);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void Releasing_Over_The_Source_Row_Does_Not_Raise_RowReordered()
    {
        var items = CreateItems(4);
        var (grid, window, handle) = Realize(items);
        var reorderedCount = 0;
        grid.RowReordered += (_, _) => reorderedCount++;

        try
        {
            var drag = BeginPointerDrag(handle, window);
            var target = drag.Start + new Vector(Constants.DragThreshold + 1, 0);
            MovePointerDrag(handle, window, drag, target, timestamp: 1);

            EndPointerDrag(handle, window, drag, target);

            items.Select(item => item.Name)
                 .ShouldBe(["Row 0", "Row 1", "Row 2", "Row 3"]);
            reorderedCount.ShouldBe(0);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void RowReordering_Is_Raised_Once_Per_Pointer_Session()
    {
        var items = CreateItems(4);
        var (grid, window, handle) = Realize(items);
        var reorderingCount = 0;
        grid.RowReordering += (_, _) => reorderingCount++;

        try
        {
            var drag = BeginPointerDrag(handle, window);

            MovePointerDragToRow(grid, handle, window, drag, targetIndex: 1);
            MovePointerDragToRow(grid, handle, window, drag, targetIndex: 2);

            reorderingCount.ShouldBe(1);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void Pointer_Capture_Loss_Cancels_An_Active_Drag()
    {
        var items = CreateItems(4);
        var (grid, window, handle) = Realize(items);
        var rowsPresenter = grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Single();

        try
        {
            var drag = BeginPointerDrag(handle, window);
            MovePointerDragToRow(grid, handle, window, drag, targetIndex: 2);

            drag.Pointer.Capture(null);

            drag.Pointer.Captured.ShouldBeNull();
            rowsPresenter.DraggedRowIndex.ShouldBeNull();
            rowsPresenter.DragRowOffset.ShouldBe(0);
            items.Select(item => item.Name)
                 .ShouldBe(["Row 0", "Row 1", "Row 2", "Row 3"]);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void Disabling_The_DataGrid_Cancels_An_Active_Drag_Immediately()
    {
        var items = CreateItems(4);
        var (grid, window, handle) = Realize(items);
        var rowsPresenter = grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Single();

        try
        {
            var drag = BeginPointerDrag(handle, window);
            MovePointerDragToRow(grid, handle, window, drag, targetIndex: 2);

            grid.IsEnabled = false;

            drag.Pointer.Captured.ShouldBeNull();
            rowsPresenter.DraggedRowIndex.ShouldBeNull();
            rowsPresenter.DragRowOffset.ShouldBe(0);
            items.Select(item => item.Name)
                 .ShouldBe(["Row 0", "Row 1", "Row 2", "Row 3"]);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void Disabling_Row_Reorder_Cancels_An_Active_Drag_Immediately()
    {
        var items = CreateItems(4);
        var (grid, window, handle) = Realize(items);
        var rowsPresenter = grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Single();

        try
        {
            var drag = BeginPointerDrag(handle, window);
            MovePointerDragToRow(grid, handle, window, drag, targetIndex: 2);

            grid.CanUserReorderRows = false;

            drag.Pointer.Captured.ShouldBeNull();
            rowsPresenter.DraggedRowIndex.ShouldBeNull();
            rowsPresenter.DragRowOffset.ShouldBe(0);
            items.Select(item => item.Name)
                 .ShouldBe(["Row 0", "Row 1", "Row 2", "Row 3"]);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void Replacing_ItemsSource_Cancels_The_Session_Before_CollectionView_Changes()
    {
        var items       = CreateItems(4);
        var replacement = CreateItems(2);
        var (grid, window, handle) = Realize(items);
        var rowsPresenter = grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Single();
        PointerDrag? drag = null;
        bool? cleanupObservedAtViewChange = null;
        grid.PropertyChanged += (_, change) =>
        {
            if (change.Property == global::AtomUI.Desktop.Controls.DataGrid.CollectionViewProperty)
            {
                cleanupObservedAtViewChange =
                    drag?.Pointer.Captured is null &&
                    rowsPresenter.DraggedRowIndex is null &&
                    rowsPresenter.DragRowOffset == 0;
            }
        };

        try
        {
            drag = BeginPointerDrag(handle, window);
            MovePointerDragToRow(grid, handle, window, drag, targetIndex: 2);

            grid.ItemsSource = replacement;

            cleanupObservedAtViewChange.ShouldBe(true);
            grid.ItemsSource.ShouldBeSameAs(replacement);
            items.Select(item => item.Name)
                 .ShouldBe(["Row 0", "Row 1", "Row 2", "Row 3"]);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void Recycling_The_Source_Row_Cancels_An_Active_Drag()
    {
        var items = CreateItems(4);
        var (grid, window, handle) = Realize(items);
        var rowsPresenter = grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Single();

        try
        {
            var drag = BeginPointerDrag(handle, window);
            MovePointerDragToRow(grid, handle, window, drag, targetIndex: 2);

            items.RemoveAt(0);

            drag.Pointer.Captured.ShouldBeNull();
            rowsPresenter.DraggedRowIndex.ShouldBeNull();
            rowsPresenter.DragRowOffset.ShouldBe(0);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void Removing_The_Reorder_Column_Cancels_An_Active_Drag()
    {
        var items = CreateItems(4);
        var (grid, window, handle) = Realize(items);
        var rowsPresenter = grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Single();

        try
        {
            var drag = BeginPointerDrag(handle, window);
            MovePointerDragToRow(grid, handle, window, drag, targetIndex: 2);

            grid.Columns.RemoveAt(0);

            drag.Pointer.Captured.ShouldBeNull();
            rowsPresenter.DraggedRowIndex.ShouldBeNull();
            rowsPresenter.DragRowOffset.ShouldBe(0);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void Detaching_The_DataGrid_Cancels_An_Active_Drag()
    {
        var items = CreateItems(4);
        var (grid, window, handle) = Realize(items);
        var rowsPresenter = grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Single();

        try
        {
            var drag = BeginPointerDrag(handle, window);
            MovePointerDragToRow(grid, handle, window, drag, targetIndex: 2);

            window.Content = null;

            drag.Pointer.Captured.ShouldBeNull();
            rowsPresenter.DraggedRowIndex.ShouldBeNull();
            rowsPresenter.DragRowOffset.ShouldBe(0);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void Reapplying_The_Template_Cancels_The_Session_Before_Old_Parts_Change()
    {
        var items = CreateItems(4);
        var (grid, window, handle) = Realize(items);
        var rowsPresenter = grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Single();
        var headersPresenter = grid.GetVisualDescendants().OfType<DataGridColumnHeadersPresenter>().Single();
        PointerDrag? drag = null;
        bool? cleanupObservedBeforePartMutation = null;
        ((INotifyCollectionChanged)headersPresenter.Children).CollectionChanged += (_, _) =>
        {
            cleanupObservedBeforePartMutation ??=
                drag?.Pointer.Captured is null &&
                rowsPresenter.DraggedRowIndex is null &&
                rowsPresenter.DragRowOffset == 0;
        };

        try
        {
            drag = BeginPointerDrag(handle, window);
            MovePointerDragToRow(grid, handle, window, drag, targetIndex: 2);

            ReapplyTemplate(grid);

            cleanupObservedBeforePartMutation.ShouldBe(true);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void AutoScroll_That_Recycles_The_Source_Row_Cancels_The_Session()
    {
        var items = CreateItems(100);
        var (grid, window, handle) = Realize(items, height: 160);
        var rowsPresenter = grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Single();

        try
        {
            var drag = BeginPointerDrag(handle, window);

            MovePointerDrag(
                handle,
                window,
                drag,
                drag.Start + new Vector(0, 1000),
                timestamp: 1);

            drag.Pointer.Captured.ShouldBeNull();
            rowsPresenter.DraggedRowIndex.ShouldBeNull();
            rowsPresenter.DragRowOffset.ShouldBe(0);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void AutoScroll_At_The_Top_Does_Not_Move_Above_Zero()
    {
        var items = CreateItems(100);
        var (grid, window, handle) = Realize(items, height: 160);
        var rowsPresenter = grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Single();

        try
        {
            var drag = BeginPointerDrag(handle, window);

            MovePointerDrag(
                handle,
                window,
                drag,
                drag.Start - new Vector(0, 1000),
                timestamp: 1);

            grid.VerticalScrollBar.ShouldNotBeNull().Value.ShouldBe(0);
            drag.Pointer.Captured.ShouldBeSameAs(handle);

            drag.Pointer.Capture(null);
            rowsPresenter.DraggedRowIndex.ShouldBeNull();
            rowsPresenter.DragRowOffset.ShouldBe(0);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void AutoScroll_At_The_Bottom_Does_Not_Exceed_Maximum()
    {
        var items = CreateItems(100);
        var (grid, window, _) = Realize(items, height: 160);
        var verticalScrollBar = grid.VerticalScrollBar.ShouldNotBeNull();
        grid.ScrollSlotsByHeight(verticalScrollBar.Maximum);
        var handle = grid.GetVisualDescendants().OfType<DataGridRowReorderHandle>().First();
        var rowsPresenter = grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Single();

        try
        {
            var drag = BeginPointerDrag(handle, window);

            MovePointerDrag(
                handle,
                window,
                drag,
                drag.Start + new Vector(0, 1000),
                timestamp: 1);

            verticalScrollBar.Value.ShouldBe(verticalScrollBar.Maximum, tolerance: 0.001);
            drag.Pointer.Captured.ShouldBeSameAs(handle);

            drag.Pointer.Capture(null);
            rowsPresenter.DraggedRowIndex.ShouldBeNull();
            rowsPresenter.DragRowOffset.ShouldBe(0);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void Observable_Source_Reorder_Completes_Without_Collection_Reentrancy_Failure()
    {
        var items = CreateItems(4);
        var (grid, window, handle) = Realize(items);

        try
        {
            var drag   = BeginPointerDrag(handle, window);
            var target = MovePointerDragToRow(grid, handle, window, drag, targetIndex: 2);

            Should.NotThrow(() => EndPointerDrag(handle, window, drag, target));

            items.Select(item => item.Name)
                 .ShouldBe(["Row 1", "Row 2", "Row 0", "Row 3"]);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void RowReordered_Is_Raised_After_Collection_And_Drag_Cleanup()
    {
        var items = CreateItems(4);
        var (grid, window, handle) = Realize(items);
        var rowsPresenter = grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Single();
        var movedItem = items[0];
        string[]? orderObservedByHandler = null;
        object? rowDataContextObservedByHandler = null;
        bool? dragStateReleasedObservedByHandler = null;
        PointerDrag? drag = null;
        grid.RowReordered += (_, e) =>
        {
            orderObservedByHandler          = items.Select(item => item.Name).ToArray();
            rowDataContextObservedByHandler = e.Row.DataContext;
            dragStateReleasedObservedByHandler =
                drag?.Pointer.Captured is null &&
                rowsPresenter.DraggedRowIndex is null &&
                rowsPresenter.DragRowOffset == 0;
        };

        try
        {
            drag = BeginPointerDrag(handle, window);
            var target = MovePointerDragToRow(grid, handle, window, drag, targetIndex: 2);

            EndPointerDrag(handle, window, drag, target);

            orderObservedByHandler.ShouldBe(["Row 1", "Row 2", "Row 0", "Row 3"]);
            rowDataContextObservedByHandler.ShouldBeSameAs(movedItem);
            dragStateReleasedObservedByHandler.ShouldBe(true);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void RowReordered_Handler_Can_Replace_ItemsSource()
    {
        var items       = CreateItems(4);
        var replacement = CreateItems(1);
        var (grid, window, handle) = Realize(items);
        grid.RowReordered += (_, _) => grid.ItemsSource = replacement;

        try
        {
            var drag   = BeginPointerDrag(handle, window);
            var target = MovePointerDragToRow(grid, handle, window, drag, targetIndex: 2);

            Should.NotThrow(() => EndPointerDrag(handle, window, drag, target));

            grid.ItemsSource.ShouldBeSameAs(replacement);
            items.Select(item => item.Name)
                 .ShouldBe(["Row 1", "Row 2", "Row 0", "Row 3"]);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void View_Replacement_During_Move_Suppresses_RowReordered_For_The_New_View()
    {
        var items       = CreateItems(4);
        var replacement = CreateItems(1);
        var (grid, window, handle) = Realize(items);
        var rowsPresenter = grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Single();
        var reorderedCount = 0;
        grid.RowReordered += (_, _) => reorderedCount++;
        grid.CollectionView!.CollectionChanged += (_, _) => grid.ItemsSource = replacement;

        try
        {
            var drag   = BeginPointerDrag(handle, window);
            var target = MovePointerDragToRow(grid, handle, window, drag, targetIndex: 2);

            Should.NotThrow(() => EndPointerDrag(handle, window, drag, target));

            grid.ItemsSource.ShouldBeSameAs(replacement);
            items.Select(item => item.Name)
                 .ShouldBe(["Row 1", "Row 2", "Row 0", "Row 3"]);
            drag.Pointer.Captured.ShouldBeNull();
            rowsPresenter.DraggedRowIndex.ShouldBeNull();
            rowsPresenter.DragRowOffset.ShouldBe(0);
            reorderedCount.ShouldBe(0);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void Unsupported_Source_Does_Not_Start_Or_Raise_RowReordered()
    {
        var items = CreateItems(4).ToArray();
        var (grid, window, handle) = Realize(items);
        var rowsPresenter = grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Single();
        var reorderedCount = 0;
        grid.RowReordered += (_, _) => reorderedCount++;

        try
        {
            var drag   = BeginPointerDrag(handle, window);
            var target = MovePointerDragToRow(grid, handle, window, drag, targetIndex: 2);

            Should.NotThrow(() => EndPointerDrag(handle, window, drag, target));

            items.Select(item => item.Name)
                 .ShouldBe(["Row 0", "Row 1", "Row 2", "Row 3"]);
            rowsPresenter.DraggedRowIndex.ShouldBeNull();
            rowsPresenter.DragRowOffset.ShouldBe(0);
            reorderedCount.ShouldBe(0);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void Sort_And_Filter_Feature_Flags_Do_Not_Disable_Row_Reorder()
    {
        var items = CreateItems(4);
        var (grid, window, handle) = Realize(items, canUserSortColumns: true, canUserFilterColumns: true);

        try
        {
            var drag   = BeginPointerDrag(handle, window);
            var target = MovePointerDragToRow(grid, handle, window, drag, targetIndex: 2);

            EndPointerDrag(handle, window, drag, target);

            items.Select(item => item.Name)
                 .ShouldBe(["Row 1", "Row 2", "Row 0", "Row 3"]);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void Unsupported_Enumerable_Source_Remains_Usable_Without_Row_Dragging()
    {
        var items = CreateItems(4);
        var replacement = Enumerable.Range(0, 4)
                                    .Select(index => new TestRow($"Replacement {index}"));
        var (grid, window, _) = Realize(items);
        var reorderedCount = 0;
        grid.RowReordered += (_, _) => reorderedCount++;

        try
        {
            Should.NotThrow(() => grid.ItemsSource = replacement);
            Dispatcher.UIThread.RunJobs();
            var handle = grid.GetVisualDescendants()
                             .OfType<DataGridRowReorderHandle>()
                             .First();

            var drag   = BeginPointerDrag(handle, window);
            var target = MovePointerDragToRow(grid, handle, window, drag, targetIndex: 2);
            EndPointerDrag(handle, window, drag, target);

            drag.Pointer.Captured.ShouldBeNull();
            reorderedCount.ShouldBe(0);
        }
        finally
        {
            Close(window);
        }
    }

    [Fact]
    public void Custom_CollectionView_Move_Capability_Is_Used_Without_Concrete_View_Coupling()
    {
        var items = CreateItems(4);
        var view = new DelegatingMoveView(items);
        var (grid, window, handle) = Realize(view);

        try
        {
            var drag   = BeginPointerDrag(handle, window);
            var target = MovePointerDragToRow(grid, handle, window, drag, targetIndex: 2);

            EndPointerDrag(handle, window, drag, target);

            view.TryMoveCallCount.ShouldBe(1);
            items.Select(item => item.Name)
                 .ShouldBe(["Row 1", "Row 2", "Row 0", "Row 3"]);
        }
        finally
        {
            Close(window);
        }
    }

    private static (
        global::AtomUI.Desktop.Controls.DataGrid Grid,
        Window Window,
        DataGridRowReorderHandle Handle) Realize(
        IEnumerable items,
        bool canUserSortColumns = false,
        bool canUserFilterColumns = false,
        double height = 260)
    {
        var grid = new global::AtomUI.Desktop.Controls.DataGrid
        {
            AutoGenerateColumns  = false,
            CanUserReorderRows   = true,
            CanUserSortColumns   = canUserSortColumns,
            CanUserFilterColumns = canUserFilterColumns,
            ItemsSource          = items,
            Width                = 400,
            Height               = height
        };
        grid.Columns.Add(new DataGridRowReorderColumn());
        grid.Columns.Add(new DataGridTextColumn
        {
            Header  = "Name",
            Binding = new Binding(nameof(TestRow.Name))
        });

        var window = new Window
        {
            Width   = 480,
            Height  = 320,
            Content = grid
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();

        var handle = grid.GetVisualDescendants()
                         .OfType<DataGridRowReorderHandle>()
                         .First();
        return (grid, window, handle);
    }

    private static PointerDrag BeginPointerDrag(DataGridRowReorderHandle handle, Visual root)
    {
        var pointer = new Avalonia.Input.Pointer(
            Avalonia.Input.Pointer.GetNextFreeId(),
            PointerType.Mouse,
            true);
        var start = GetCenterInRoot(handle, root);
        handle.RaiseEvent(new PointerPressedEventArgs(
            handle,
            pointer,
            root,
            start,
            0,
            new PointerPointProperties(
                RawInputModifiers.LeftMouseButton,
                PointerUpdateKind.LeftButtonPressed),
            KeyModifiers.None));
        return new PointerDrag(pointer, start);
    }

    private static Point MovePointerDragToRow(
        global::AtomUI.Desktop.Controls.DataGrid grid,
        DataGridRowReorderHandle handle,
        Visual root,
        PointerDrag drag,
        int targetIndex)
    {
        var row = grid.GetVisualDescendants()
                      .OfType<DataGridRow>()
                      .Single(candidate => candidate.Index == targetIndex && !candidate.IsDragging);
        var target = GetCenterInRoot(row, root);
        MovePointerDrag(handle, root, drag, target, timestamp: 1);
        return target;
    }

    private static void MovePointerDrag(
        DataGridRowReorderHandle handle,
        Visual root,
        PointerDrag drag,
        Point position,
        ulong timestamp)
    {
        handle.RaiseEvent(new PointerEventArgs(
            InputElement.PointerMovedEvent,
            handle,
            drag.Pointer,
            root,
            position,
            timestamp,
            new PointerPointProperties(
                RawInputModifiers.LeftMouseButton,
                PointerUpdateKind.Other),
            KeyModifiers.None));
    }

    private static void EndPointerDrag(
        DataGridRowReorderHandle handle,
        Visual root,
        PointerDrag drag,
        Point position)
    {
        handle.RaiseEvent(new PointerReleasedEventArgs(
            handle,
            drag.Pointer,
            root,
            position,
            2,
            new PointerPointProperties(
                RawInputModifiers.None,
                PointerUpdateKind.LeftButtonReleased),
            KeyModifiers.None,
            MouseButton.Left));
    }

    private static Point GetCenterInRoot(Control control, Visual root)
    {
        var point = control.TranslatePoint(
            new Point(control.Bounds.Width / 2, control.Bounds.Height / 2),
            root);
        return point.ShouldNotBeNull();
    }

    private static ObservableCollection<TestRow> CreateItems(int count)
    {
        return new ObservableCollection<TestRow>(
            Enumerable.Range(0, count).Select(index => new TestRow($"Row {index}")));
    }

    private static void Close(Window window)
    {
        window.Close();
        Dispatcher.UIThread.RunJobs();
    }

    private static void ReapplyTemplate(global::AtomUI.Desktop.Controls.DataGrid grid)
    {
        typeof(global::AtomUI.Desktop.Controls.DataGrid)
            .GetMethod("OnApplyTemplate", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(grid, [new TemplateAppliedEventArgs(new NameScope())]);
    }

    private sealed record TestRow(string Name);

    private sealed record PointerDrag(Avalonia.Input.Pointer Pointer, Point Start);

    private sealed class DelegatingMoveView : IDataGridCollectionView, IDataGridCollectionViewMoveSupport
    {
        private readonly DataGridCollectionView _inner;

        public DelegatingMoveView(IEnumerable source)
        {
            _inner = new DataGridCollectionView(source);
        }

        public int TryMoveCallCount { get; private set; }

        public bool CanMove => ((IDataGridCollectionViewMoveSupport)_inner).CanMove;

        public CultureInfo Culture
        {
            get => _inner.Culture;
            set => _inner.Culture = value;
        }

        public bool Contains(object item) => _inner.Contains(item);
        public IEnumerable SourceCollection => _inner.SourceCollection;

        public Func<object, bool>? Filter
        {
            get => _inner.Filter;
            set => _inner.Filter = value;
        }

        public bool CanFilter => _inner.CanFilter;
        public DataGridSortDescriptionCollection? SortDescriptions => _inner.SortDescriptions;
        public DataGridFilterDescriptionCollection? FilterDescriptions => _inner.FilterDescriptions;
        public bool CanSort => _inner.CanSort;
        public bool CanGroup => _inner.CanGroup;
        public bool IsGrouping => ((IDataGridCollectionView)_inner).IsGrouping;
        public int GroupingDepth => ((IDataGridCollectionView)_inner).GroupingDepth;
        public IAvaloniaReadOnlyList<object>? Groups => _inner.Groups;
        public bool IsEmpty => _inner.IsEmpty;
        public object? CurrentItem => _inner.CurrentItem;
        public int CurrentPosition => _inner.CurrentPosition;
        public bool IsCurrentAfterLast => _inner.IsCurrentAfterLast;
        public bool IsCurrentBeforeFirst => _inner.IsCurrentBeforeFirst;

        public event NotifyCollectionChangedEventHandler? CollectionChanged
        {
            add => _inner.CollectionChanged += value;
            remove => _inner.CollectionChanged -= value;
        }

        public event EventHandler<DataGridCurrentChangingEventArgs>? CurrentChanging
        {
            add => _inner.CurrentChanging += value;
            remove => _inner.CurrentChanging -= value;
        }

        public event EventHandler? CurrentChanged
        {
            add => _inner.CurrentChanged += value;
            remove => _inner.CurrentChanged -= value;
        }

        public bool TryMove(int sourceIndex, int targetIndex)
        {
            TryMoveCallCount++;
            return ((IDataGridCollectionViewMoveSupport)_inner).TryMove(sourceIndex, targetIndex);
        }

        public string GetGroupingPropertyNameAtDepth(int level) =>
            ((IDataGridCollectionView)_inner).GetGroupingPropertyNameAtDepth(level);
        public void Refresh() => _inner.Refresh();
        public IDisposable DeferRefresh() => _inner.DeferRefresh();
        public bool MoveCurrentToFirst() => _inner.MoveCurrentToFirst();
        public bool MoveCurrentToLast() => _inner.MoveCurrentToLast();
        public bool MoveCurrentToNext() => _inner.MoveCurrentToNext();
        public bool MoveCurrentToPrevious() => _inner.MoveCurrentToPrevious();
        public bool MoveCurrentTo(object? item) => _inner.MoveCurrentTo(item);
        public bool MoveCurrentToPosition(int position) => _inner.MoveCurrentToPosition(position);
        public object AddNew() => _inner.AddNew();
        public void CancelNew() => _inner.CancelNew();
        public void CommitNew() => _inner.CommitNew();
        public void Remove(object? item) => _inner.Remove(item);
        public void RemoveAt(int index) => _inner.RemoveAt(index);
        public IEnumerator GetEnumerator() => _inner.GetEnumerator();
    }
}
