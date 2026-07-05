using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using AtomUI.Controls.Data;
using AtomUI.Controls.Utils;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Shouldly;
using Xunit;
using AtomListView = AtomUI.Desktop.Controls.ListView;
using AtomPagination = AtomUI.Desktop.Controls.Pagination;

namespace AtomUI.Desktop.Controls.Tests.ListView;

public class ListViewStateTests
{
    static ListViewStateTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void ItemsSource_Wrap_Does_Not_Install_Default_Filter_Without_Active_Filtering()
    {
        var listView = new AtomListView
        {
            ItemsSource = CreateItems("alpha", "beta")
        };

        var collectionView = listView.ItemsSource.ShouldBeAssignableTo<IListCollectionView>();

        collectionView.Filter.ShouldBeNull(
            "a ListView with no filter conditions should not force ListCollectionView into the local filtering path.");
        collectionView.FilterDescriptions.ShouldBeEmpty();
    }

    [Fact]
    public void Default_Filter_Callback_Is_Attached_Only_While_Filter_Conditions_Are_Active()
    {
        var listView = new AtomListView
        {
            ItemsSource = CreateItems("alpha", "beta"),
            Filter      = ValueFilterFactory.BuildFilter(ValueFilterMode.Contains)
        };
        var collectionView = listView.ItemsSource.ShouldBeAssignableTo<IListCollectionView>();

        listView.FilterValue = "alpha";

        collectionView.Filter.ShouldNotBeNull(
            "ListView needs its default callback only when FilterDescriptions should drive the collection view.");
        collectionView.FilterDescriptions.Count.ShouldBe(1);

        listView.FilterValue = null;

        collectionView.Filter.ShouldBeNull(
            "the default callback should be removed when ListView no longer owns active filter conditions.");
        collectionView.FilterDescriptions.ShouldBeEmpty();
    }

    [Fact]
    public void Bottom_Pagination_State_Tracks_Collection_View_Changes()
    {
        var items = new ObservableCollection<ListItemData>(CreateItems("alpha", "beta"));
        var pagination = new AtomPagination();
        var listView = new AtomListView
        {
            PageSize         = 10,
            ItemsSource      = items,
            BottomPagination = pagination
        };

        pagination.Total.ShouldBe(2);
        pagination.PageSize.ShouldBe(10);
        pagination.CurrentPage.ShouldBe(1);

        items.Add(new ListItemData { Content = "gamma" });

        listView.TotalItemCount.ShouldBe(3);
        pagination.Total.ShouldBe(3);
        pagination.PageSize.ShouldBe(10);
        pagination.CurrentPage.ShouldBe(1);
    }

    [Fact]
    public void Pagination_Visibility_Does_Not_Override_Pagination_Motion_State()
    {
        var pagination = new AtomPagination();
        var listView = new AtomListView
        {
            IsMotionEnabled      = true,
            ItemsSource           = CreateItems("alpha", "beta"),
            BottomPagination      = pagination,
            PaginationVisibility  = ListPaginationVisibility.None
        };

        pagination.IsVisible.ShouldBeFalse();
        pagination.IsMotionEnabled.ShouldBeTrue(
            "PaginationVisibility controls visibility only and must not be relayed into IsMotionEnabled.");

        listView.PaginationVisibility = ListPaginationVisibility.Bottom;

        pagination.IsVisible.ShouldBeTrue();
        pagination.IsMotionEnabled.ShouldBeTrue();
    }

    [Fact]
    public void Replaced_Selection_Model_Does_Not_Handle_Old_LostSelection()
    {
        var oldSelection = new SelectionModel<object?>();
        var newSelection = new SelectionModel<object?>();
        var listView = new AtomListView
        {
            ItemsSource    = CreateItems("alpha", "beta", "gamma"),
            SelectionMode  = SelectionMode.AlwaysSelected,
            Selection      = oldSelection
        };

        listView.SelectedIndex = 1;
        listView.Selection     = newSelection;
        listView.SelectedIndex = 2;

        oldSelection.Clear();

        listView.SelectedIndex.ShouldBe(2,
            "LostSelection from a replaced model should not drive the active selection model.");
    }

    [Fact]
    public void Virtualizing_Context_Tolerates_Duplicate_Recycled_Index()
    {
        var listView = new AtomListView();
        var firstContainer = new AtomUI.Desktop.Controls.ListViewItem();
        var secondContainer = new AtomUI.Desktop.Controls.ListViewItem();
        var itemData = new ListItemData { Content = "item" };

        PrepareContainerForItem(listView, firstContainer, itemData, 5);
        PrepareContainerForItem(listView, secondContainer, itemData, 5);

        ClearContainerForItem(listView, firstContainer);
        Should.NotThrow(() => ClearContainerForItem(listView, secondContainer),
            "virtualized collection mutations can recycle more than one stale container for the same index before one is restored.");
    }

    private static ListItemData[] CreateItems(params string[] values)
    {
        return values.Select(value => new ListItemData { Content = value }).ToArray();
    }

    private static void PrepareContainerForItem(
        AtomListView listView,
        AtomUI.Desktop.Controls.ListViewItem container,
        object item,
        int index)
    {
        var method = typeof(AtomListView).GetMethod(
            "PrepareContainerForItemOverride",
            BindingFlags.Instance | BindingFlags.NonPublic);
        method.ShouldNotBeNull();
        method.Invoke(listView, [container, item, index]);
    }

    private static void ClearContainerForItem(
        AtomListView listView,
        AtomUI.Desktop.Controls.ListViewItem container)
    {
        var method = typeof(AtomListView).GetMethod(
            "ClearContainerForItemOverride",
            BindingFlags.Instance | BindingFlags.NonPublic);
        method.ShouldNotBeNull();
        method.Invoke(listView, [container]);
    }
}
