using AtomUI.Controls;
using AtomUI.Controls.Data;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunListBoxStateVerification()
    {
        var failures = new List<string>();

        VerifyListBoxDefaultShape(failures);
        VerifyListBoxSelectedIndicatorLifecycle(failures);
        VerifyListBoxFilteringLifecycle(failures);
        VerifyListViewCollectionFilterLifecycle(failures);
        VerifyListViewEmptyIndicatorLazyMaterialization(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("ListBox state verification passed.");
            return true;
        }

        Console.Error.WriteLine("ListBox state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyListBoxDefaultShape(ICollection<string> failures)
    {
        var listBox = CreateListBox(CreateListItems(5));
        using var realized = RealizeControl(listBox);

        Expect(FindSelectedIndicatorPresenter(listBox) == null,
            "Default ListBox should not materialize selected indicator presenter.",
            failures);
        Expect(FindVisualByType<HighlightableTextBlock>(listBox) == null,
            "Default ListBox should not materialize filtering HighlightableTextBlock.",
            failures);
    }

    private static void VerifyListBoxSelectedIndicatorLifecycle(ICollection<string> failures)
    {
        var listBox = CreateListBox(
            CreateListItems(5),
            isShowSelectedIndicator: true,
            selectedIndex: 0);
        using var realized = RealizeControl(listBox);

        var selectedItem = listBox.ContainerFromIndex(0) as AtomUI.Desktop.Controls.ListBoxItem;
        Expect(selectedItem != null,
            "Selected ListBox item should have a realized container.",
            failures);
        if (selectedItem == null)
        {
            return;
        }

        Expect(FindSelectedIndicatorPresenter(selectedItem) != null,
            "Selected ListBox item should materialize selected indicator presenter.",
            failures);

        listBox.SelectedIndex = -1;
        RefreshLayout(realized.Window);
        Expect(FindSelectedIndicatorPresenter(selectedItem) == null,
            "Unselected ListBox item should detach selected indicator presenter.",
            failures);

        listBox.SelectedIndex = 1;
        RefreshLayout(realized.Window);
        var nextSelectedItem = listBox.ContainerFromIndex(1) as AtomUI.Desktop.Controls.ListBoxItem;
        Expect(nextSelectedItem != null && FindSelectedIndicatorPresenter(nextSelectedItem) != null,
            "Selecting another ListBox item should materialize selected indicator presenter again.",
            failures);
    }

    private static void VerifyListBoxFilteringLifecycle(ICollection<string> failures)
    {
        var listBox = CreateListBox(CreateListBoxDemoItems());
        using var realized = RealizeControl(listBox);

        Expect(FindVisualByType<HighlightableTextBlock>(listBox) == null,
            "ListBox should not create filtering text block before filtering starts.",
            failures);

        listBox.FilterValue = "car";
        RefreshLayout(realized.Window);
        Expect(FindVisualByType<HighlightableTextBlock>(listBox) != null,
            "ListBox should create filtering text block while filtering.",
            failures);

        listBox.FilterValue = null;
        RefreshLayout(realized.Window);
        Expect(FindVisualByType<HighlightableTextBlock>(listBox) == null,
            "ListBox should detach filtering text block after filtering clears.",
            failures);
    }

    private static void VerifyListViewCollectionFilterLifecycle(ICollection<string> failures)
    {
        var listView = CreateListView(CreateListItems(20));
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

        listView.Filter = AtomUI.Controls.Utils.ValueFilterFactory.BuildFilter(AtomUI.Controls.Utils.ValueFilterMode.Contains);
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
        var listView = CreateListView(CreateListItems(5));
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

    private static IconTemplatePresenter? FindSelectedIndicatorPresenter(Control root)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<IconTemplatePresenter>()
                   .FirstOrDefault(presenter => presenter.Name == "SelectedIndicator");
    }

    private static T? FindVisualByType<T>(Control root)
        where T : Control
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<T>()
                   .FirstOrDefault();
    }
}
