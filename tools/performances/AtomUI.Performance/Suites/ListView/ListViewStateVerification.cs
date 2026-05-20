using System.Collections;
using System.ComponentModel;
using AtomUI.Controls;
using AtomUI.Controls.Data;
using AtomUI.Controls.Utils;
using AtomUI.Desktop.Controls;
using Avalonia.Controls.Selection;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunListViewStateVerification()
    {
        var failures = new List<string>();

        VerifyListViewSelectedIndicatorLifecycle(failures);
        VerifyListViewCollectionFilterLifecycle(failures);
        VerifyListViewEmptyIndicatorLazyMaterialization(failures);
        VerifyListViewPaginationLifecycle(failures);
        VerifyListViewSelectionModelLifecycle(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("ListView state verification passed.");
            return true;
        }

        Console.Error.WriteLine("ListView state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyListViewSelectedIndicatorLifecycle(ICollection<string> failures)
    {
        var listView = CreateListView(
            CreateListViewItems(5),
            isShowSelectedIndicator: true,
            selectedIndex: 0);
        using var realized = RealizeControl(listView);

        var selectedItem = listView.ContainerFromIndex(0) as ListViewItem;
        Expect(selectedItem != null,
            "Selected ListView item should have a realized container.",
            failures);
        if (selectedItem == null)
        {
            return;
        }

        Expect(FindSelectedIndicatorPresenter(selectedItem) != null,
            "Selected ListView item should materialize selected indicator presenter.",
            failures);

        listView.SelectedIndex = -1;
        RefreshLayout(realized.Window);
        Expect(FindSelectedIndicatorPresenter(selectedItem) == null,
            "Unselected ListView item should detach selected indicator presenter.",
            failures);

        listView.SelectedIndex = 1;
        RefreshLayout(realized.Window);
        var nextSelectedItem = listView.ContainerFromIndex(1) as ListViewItem;
        Expect(nextSelectedItem != null && FindSelectedIndicatorPresenter(nextSelectedItem) != null,
            "Selecting another ListView item should materialize selected indicator presenter again.",
            failures);
    }

    private static void VerifyListViewCollectionFilterLifecycle(ICollection<string> failures)
    {
        var listView = CreateListView(CreateListViewItems(20));
        using var realized = RealizeControl(listView);
        var collectionView = listView.ItemsSource as IListCollectionView;

        Expect(collectionView != null,
            "ListView should wrap ItemsSource in IListCollectionView.",
            failures);
        if (collectionView == null)
        {
            return;
        }

        Expect(collectionView.Filter == null,
            "Default ListView collection view should not install a filter callback.",
            failures);

        listView.Filter = ValueFilterFactory.BuildFilter(ValueFilterMode.Contains);
        listView.FilterValue = "1";
        RefreshLayout(realized.Window);
        Expect(collectionView.Filter != null,
            "Filtered ListView should install a filter callback.",
            failures);

        listView.FilterValue = null;
        RefreshLayout(realized.Window);
        Expect(collectionView.Filter == null,
            "Clearing ListView filter should remove the internal filter callback.",
            failures);
    }

    private static void VerifyListViewEmptyIndicatorLazyMaterialization(ICollection<string> failures)
    {
        var listView = CreateListView(CreateListViewItems(5));
        using var realized = RealizeControl(listView);

        Expect(FindVisualByType<Empty>(listView) == null,
            "Non-empty ListView should not materialize the default Empty indicator.",
            failures);

        listView.ItemsSource = Array.Empty<IListItemData>();
        RefreshLayout(realized.Window);
        Expect(FindVisualByType<Empty>(listView) != null,
            "Empty ListView should lazily materialize the default Empty indicator.",
            failures);
    }

    private static void VerifyListViewPaginationLifecycle(ICollection<string> failures)
    {
        var oldPagination = new Pagination();
        var listView = CreateListView(
            CreateListViewItems(200),
            pageSize: 100,
            pagination: oldPagination);
        using var realized = RealizeControl(listView);

        Expect(oldPagination.Align == listView.BottomPaginationAlign,
            "Bottom pagination should sync align from ListView.",
            failures);
        Expect(oldPagination.IsMotionEnabled == listView.IsMotionEnabled,
            "Bottom pagination should sync motion from ListView.",
            failures);

        listView.PaginationVisibility = ListPaginationVisibility.None;
        RefreshLayout(realized.Window);
        Expect(!oldPagination.IsVisible,
            "Bottom pagination should hide when PaginationVisibility is None.",
            failures);
        Expect(oldPagination.IsMotionEnabled == listView.IsMotionEnabled,
            "Changing PaginationVisibility must not overwrite pagination motion state.",
            failures);

        var collectionView = listView.ItemsSource as IListCollectionView;
        var firstPageItems = collectionView?.Cast<IListItemData>().ToList();
        Expect(firstPageItems?.Count == 100,
            "Paged ListView should enumerate only the current page.",
            failures);
        Expect(firstPageItems?.FirstOrDefault()?.Content?.ToString() == "Content 0",
            "Paged ListView first page should start with the first source item.",
            failures);

        collectionView?.MoveToPage(1);
        RefreshLayout(realized.Window);
        var secondPageItems = collectionView?.Cast<IListItemData>().ToList();
        Expect(secondPageItems?.Count == 100,
            "Paged ListView should enumerate the requested page size after page changes.",
            failures);
        Expect(secondPageItems?.FirstOrDefault()?.Content?.ToString() == "Content 100",
            "Paged ListView second page should start at the page offset.",
            failures);

        listView.PaginationVisibility = ListPaginationVisibility.Bottom;
        RefreshLayout(realized.Window);
        Expect(oldPagination.IsVisible,
            "Bottom pagination should show when PaginationVisibility is Bottom.",
            failures);

        listView.BottomPagination = null;
        RefreshLayout(realized.Window);
        listView.PaginationVisibility = ListPaginationVisibility.None;
        RefreshLayout(realized.Window);
        Expect(oldPagination.IsVisible,
            "Detached bottom pagination should not be mutated by later ListView visibility changes.",
            failures);

        var newPagination = new Pagination();
        listView.BottomPagination = newPagination;
        RefreshLayout(realized.Window);
        Expect(newPagination.PageSize == listView.PageSize,
            "New bottom pagination should receive the current ListView page size.",
            failures);
    }

    private static void VerifyListViewSelectionModelLifecycle(ICollection<string> failures)
    {
        var firstSelection = new CountingSelectionModel();
        var secondSelection = new CountingSelectionModel();
        var listView = CreateListView(CreateListViewItems(5));
        listView.Selection = firstSelection;
        using var realized = RealizeControl(listView);

        listView.Selection = secondSelection;
        RefreshLayout(realized.Window);

        Expect(firstSelection.PropertyChangedSubscriberCount == 0,
            "Replaced ListView selection model should release PropertyChanged subscription.",
            failures);
        Expect(firstSelection.SelectionChangedSubscriberCount == 0,
            "Replaced ListView selection model should release SelectionChanged subscription.",
            failures);
        Expect(firstSelection.LostSelectionSubscriberCount == 0,
            "Replaced ListView selection model should release LostSelection subscription.",
            failures);
    }

    private sealed class CountingSelectionModel : ISelectionModel
    {
        private readonly List<int> _selectedIndexes = [];
        private readonly List<object?> _selectedItems = [];
        private PropertyChangedEventHandler? _propertyChanged;
        private EventHandler<SelectionModelIndexesChangedEventArgs>? _indexesChanged;
        private EventHandler<SelectionModelSelectionChangedEventArgs>? _selectionChanged;
        private EventHandler? _lostSelection;
        private EventHandler? _sourceReset;

        public int PropertyChangedSubscriberCount { get; private set; }
        public int SelectionChangedSubscriberCount { get; private set; }
        public int LostSelectionSubscriberCount { get; private set; }
        public IEnumerable? Source { get; set; }
        public bool SingleSelect { get; set; }
        public int SelectedIndex { get; set; } = -1;
        public IReadOnlyList<int> SelectedIndexes => _selectedIndexes;
        public object? SelectedItem { get; set; }
        public IReadOnlyList<object?> SelectedItems => _selectedItems;
        public int AnchorIndex { get; set; } = -1;
        public int Count => Source is ICollection collection ? collection.Count : 0;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _propertyChanged += value;
                PropertyChangedSubscriberCount++;
            }
            remove
            {
                _propertyChanged -= value;
                PropertyChangedSubscriberCount--;
            }
        }

        public event EventHandler<SelectionModelIndexesChangedEventArgs>? IndexesChanged
        {
            add => _indexesChanged += value;
            remove => _indexesChanged -= value;
        }

        public event EventHandler<SelectionModelSelectionChangedEventArgs>? SelectionChanged
        {
            add
            {
                _selectionChanged += value;
                SelectionChangedSubscriberCount++;
            }
            remove
            {
                _selectionChanged -= value;
                SelectionChangedSubscriberCount--;
            }
        }

        public event EventHandler? LostSelection
        {
            add
            {
                _lostSelection += value;
                LostSelectionSubscriberCount++;
            }
            remove
            {
                _lostSelection -= value;
                LostSelectionSubscriberCount--;
            }
        }

        public event EventHandler? SourceReset
        {
            add => _sourceReset += value;
            remove => _sourceReset -= value;
        }

        public void BeginBatchUpdate()
        {
        }

        public void EndBatchUpdate()
        {
        }

        public bool IsSelected(int index) => _selectedIndexes.Contains(index);

        public void Select(int index)
        {
            _selectedIndexes.Clear();
            _selectedItems.Clear();
            _selectedIndexes.Add(index);
            SelectedIndex = index;
            AnchorIndex   = index;
        }

        public void Deselect(int index)
        {
            _selectedIndexes.Remove(index);
            if (SelectedIndex == index)
            {
                SelectedIndex = -1;
            }
        }

        public void SelectRange(int start, int end)
        {
            for (var i = start; i <= end; i++)
            {
                Select(i);
            }
        }

        public void DeselectRange(int start, int end)
        {
            for (var i = start; i <= end; i++)
            {
                Deselect(i);
            }
        }

        public void SelectAll()
        {
        }

        public void Clear()
        {
            _selectedIndexes.Clear();
            _selectedItems.Clear();
            SelectedIndex = -1;
            SelectedItem  = null;
        }
    }
}
