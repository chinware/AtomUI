using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.DataGrid.Lifecycle;

public class DataGridPaginationTemplateLifecycleTests
{
    static DataGridPaginationTemplateLifecycleTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Pagination_State_Is_Replayed_When_Template_Applies_After_Source()
    {
        using var source = CreateSource();
        var grid = CreateGrid<global::AtomUI.Desktop.Controls.DataGrid>(
            source,
            new DataGridPageRequest(0, 10));
        var window = new Window
        {
            Width   = 640,
            Height  = 360,
            Content = grid
        };

        try
        {
            window.Show();
            PumpUntil(() => grid.LoadState == DataGridLoadState.Ready);

            var topPagination    = FindPagination(grid, "PART_TopPagination");
            var bottomPagination = FindPagination(grid, "PART_BottomPagination");

            AssertPaginationState(topPagination, 1);
            AssertPaginationState(bottomPagination, 1);
            AssertPaginationRootVisible(topPagination);
            AssertPaginationRootVisible(bottomPagination);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void Pagination_State_Is_Replayed_Without_Resetting_Page_When_Template_Is_Reapplied()
    {
        using var source = CreateSource();
        var grid = CreateGrid<TestDataGrid>(
            source,
            new DataGridPageRequest(20, 10));
        var window = new Window { Content = grid };
        var firstTop = new Pagination();
        var firstBottom = new Pagination();
        var secondTop = new Pagination();
        var secondBottom = new Pagination();
        try
        {
            window.Show();
            PumpUntil(() => grid.LoadState == DataGridLoadState.Ready);

            grid.ApplyPaginationParts(firstTop, firstBottom);
            AssertPaginationState(firstTop, 3);
            AssertPaginationState(firstBottom, 3);

            grid.ApplyPaginationParts(secondTop, secondBottom);
            grid.PageRequest.ShouldBe(new DataGridPageRequest(20, 10));
            AssertPaginationState(secondTop, 3);
            AssertPaginationState(secondBottom, 3);

            firstTop.CurrentPage = 4;
            grid.PageRequest.ShouldBe(new DataGridPageRequest(20, 10));

            secondBottom.CurrentPage = 4;
            grid.PageRequest.ShouldBe(new DataGridPageRequest(30, 10));

            window.Close();
            Dispatcher.UIThread.RunJobs();
            secondBottom.CurrentPage = 5;
            grid.PageRequest.ShouldBe(new DataGridPageRequest(30, 10));
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static T CreateGrid<T>(
        IDataGridSource source,
        DataGridPageRequest pageRequest)
        where T : global::AtomUI.Desktop.Controls.DataGrid, new()
    {
        return new T
        {
            AutoGenerateColumns  = false,
            PaginationVisibility = DataGridPaginationVisibility.All,
            IsHideOnSinglePage   = true,
            PageRequest           = pageRequest,
            ItemsSource               = source,
            Width                = 540,
            Height               = 280
        };
    }

    private static DataGridLocalSource<Row> CreateSource()
    {
        var field = new DataGridFieldId("value");
        var descriptor = DataGridLocalSourceDescriptor.For<Row>(
                static row => DataGridRowKey.FromInt64(row.Value))
            .Field(field, static row => row.Value);
        return DataGridLocalSource.Create(
            Enumerable.Range(1, 100).Select(static value => new Row(value)).ToArray(),
            descriptor);
    }

    private static Pagination FindPagination(
        global::AtomUI.Desktop.Controls.DataGrid grid,
        string name)
    {
        return grid.GetVisualDescendants()
                   .OfType<Pagination>()
                   .Single(pagination => pagination.Name == name);
    }

    private static void AssertPaginationState(Pagination pagination, int currentPage)
    {
        pagination.Total.ShouldBe(100);
        pagination.PageSize.ShouldBe(10);
        pagination.PageCount.ShouldBe(10);
        pagination.CurrentPage.ShouldBe(currentPage);
    }

    private static void AssertPaginationRootVisible(Pagination pagination)
    {
        pagination.GetVisualDescendants()
                  .OfType<StackPanel>()
                  .Single(panel => panel.Name == "PART_RootLayout")
                  .IsVisible
                  .ShouldBeTrue();
    }

    private static void PumpUntil(Func<bool> condition)
    {
        if (!SpinWait.SpinUntil(() =>
            {
                Dispatcher.UIThread.RunJobs();
                return condition();
            }, TimeSpan.FromSeconds(5)))
        {
            throw new TimeoutException("The expected grid state was not reached.");
        }
    }

    private sealed record Row(long Value);

    private sealed class TestDataGrid : global::AtomUI.Desktop.Controls.DataGrid
    {
        public void ApplyPaginationParts(Pagination topPagination, Pagination bottomPagination)
        {
            var nameScope = new NameScope();
            nameScope.Register("PART_TopPagination", topPagination);
            nameScope.Register("PART_BottomPagination", bottomPagination);
            OnApplyTemplate(new TemplateAppliedEventArgs(nameScope));
        }
    }
}
