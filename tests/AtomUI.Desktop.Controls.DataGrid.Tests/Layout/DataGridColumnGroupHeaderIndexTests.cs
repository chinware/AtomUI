using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.DataGrid.Layout;

public class DataGridColumnGroupHeaderIndexTests
{
    static DataGridColumnGroupHeaderIndexTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Group_Header_Index_Provider_Indexes_Header_View_Items_With_Selection_Column()
    {
        var grid = CreateIssue225Grid();
        var window = new Window
        {
            Width   = 420,
            Height  = 320,
            Content = grid
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var presenter = grid.GetVisualDescendants()
                                .OfType<DataGridGroupColumnHeadersPresenter>()
                                .Single();
            AssertHeaderViewItemIndexes(grid, presenter);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Group_Header_With_Selection_Column_Keeps_Header_Indexes_When_Resized_And_Scrolled()
    {
        var grid = CreateIssue225Grid();
        var window = new Window
        {
            Width   = 420,
            Height  = 320,
            Content = grid
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var presenter = grid.GetVisualDescendants()
                                .OfType<DataGridGroupColumnHeadersPresenter>()
                                .Single();

            for (var i = 0; i < 20; i++)
            {
                if ((i % 2) == 0)
                {
                    window.Width  = 1280;
                    window.Height = 720;
                }
                else
                {
                    window.Width  = 420;
                    window.Height = 320;
                    grid.UpdateHorizontalOffset(10_000);
                }

                Dispatcher.UIThread.RunJobs();
                AssertHeaderViewItemIndexes(grid, presenter);
            }
        }
        finally
        {
            window.Close();
        }
    }

    private static void AssertHeaderViewItemIndexes(global::AtomUI.Desktop.Controls.DataGrid grid,
                                                    DataGridGroupColumnHeadersPresenter presenter)
    {
        var childIndexProvider = (IChildIndexProvider)presenter;
        var headerViewItems = presenter.Children
                                       .OfType<DataGridHeaderViewItem>()
                                       .ToArray();

        headerViewItems.Length.ShouldBe(grid.Columns.Count);
        for (var i = 0; i < headerViewItems.Length; i++)
        {
            childIndexProvider.GetChildIndex(headerViewItems[i]).ShouldBe(i);
        }

        childIndexProvider.TryGetTotalCount(out var totalCount).ShouldBeTrue();
        totalCount.ShouldBe(headerViewItems.Length);
    }

    private static global::AtomUI.Desktop.Controls.DataGrid CreateIssue225Grid()
    {
        var grid = new global::AtomUI.Desktop.Controls.DataGrid
        {
            AutoGenerateColumns           = false,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            ItemsSource = new[]
            {
                new GridRow("1", "Name", "20", "A", "B", "C", "D", "E")
            },
            SelectionMode = DataGridSelectionMode.Extended,
            Width         = 380,
            Height        = 260
        };

        grid.ColumnGroups.Add(new DataGridSelectionColumn());
        grid.ColumnGroups.Add(new DataGridTextColumn
        {
            Header  = "ID",
            Binding = new Binding(nameof(GridRow.Id)),
            Width   = new DataGridLength(100)
        });
        grid.ColumnGroups.Add(new DataGridTextColumn
        {
            Header  = "Name",
            Binding = new Binding(nameof(GridRow.Name))
        });

        for (var i = 0; i < 6; i++)
        {
            grid.ColumnGroups.Add(new DataGridTextColumn
            {
                Header  = $"Value {i}",
                Binding = new Binding(GridRow.ValuePropertyNames[i]),
                Width   = new DataGridLength(100)
            });
        }

        return grid;
    }

    private sealed record GridRow(string Id,
                                  string Name,
                                  string Age,
                                  string Value1,
                                  string Value2,
                                  string Value3,
                                  string Value4,
                                  string Value5)
    {
        public static readonly string[] ValuePropertyNames =
        [
            nameof(Age),
            nameof(Value1),
            nameof(Value2),
            nameof(Value3),
            nameof(Value4),
            nameof(Value5)
        ];
    }
}
