using AtomUI.Desktop.Controls.Data;
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
    public void Pagination_State_Is_Replayed_When_Template_Applies_After_ItemsSource()
    {
        var grid = CreateGrid<global::AtomUI.Desktop.Controls.DataGrid>();
        var window = new Window
        {
            Width   = 640,
            Height  = 360,
            Content = grid
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

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
        var grid        = CreateGrid<TestDataGrid>();
        var firstTop    = new Pagination();
        var firstBottom = new Pagination();
        grid.ApplyPaginationParts(firstTop, firstBottom);

        var collectionView = grid.CollectionView.ShouldBeOfType<DataGridCollectionView>();
        collectionView.MoveToPage(2).ShouldBeTrue();

        var secondTop    = new Pagination();
        var secondBottom = new Pagination();
        grid.ApplyPaginationParts(secondTop, secondBottom);

        collectionView.PageIndex.ShouldBe(2);
        AssertPaginationState(secondTop, 3);
        AssertPaginationState(secondBottom, 3);

        firstTop.CurrentPage = 4;
        collectionView.PageIndex.ShouldBe(2);
    }

    private static T CreateGrid<T>()
        where T : global::AtomUI.Desktop.Controls.DataGrid, new()
    {
        return new T
        {
            AutoGenerateColumns  = false,
            PaginationVisibility = DataGridPaginationVisibility.All,
            IsHideOnSinglePage   = true,
            PageSize             = 10,
            ItemsSource          = Enumerable.Range(1, 100).ToArray(),
            Width                = 540,
            Height               = 280
        };
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
