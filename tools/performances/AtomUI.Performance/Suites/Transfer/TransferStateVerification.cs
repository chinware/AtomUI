using AtomUI.Controls;
using AtomUI.Controls.Data;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunTransferStateVerification()
    {
        var failures = new List<string>();
        VerifyListTransferFilteringKeepsExpectedSides(failures);
        VerifyTreeTransferTargetKeepsNestedOrder(failures);
        VerifyTransferListViewClearsRemovedPaginationState(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Transfer state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Transfer state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyListTransferFilteringKeepsExpectedSides(ICollection<string> failures)
    {
        var items = CreateTransferListItems(6);
        var transfer = new ListTransfer
        {
            ItemsSource = items,
            TargetKeys = ["1", "3"]
        };

        using var realized = RealizeControl(transfer);
        var views = GetTransferListViews(transfer);
        Expect(views.Count == 2,
            $"ListTransfer should create source and target TransferListView instances. Actual: {views.Count}.",
            failures);

        var sourceKeys = GetViewItemKeys(views.Single(view => view.ViewType == TransferViewType.Source));
        var targetKeys = GetViewItemKeys(views.Single(view => view.ViewType == TransferViewType.Target));
        Expect(sourceKeys.SequenceEqual(["0", "2", "4", "5"]),
            $"ListTransfer source side should exclude target keys. Actual: {string.Join(", ", sourceKeys)}.",
            failures);
        Expect(targetKeys.SequenceEqual(["1", "3"]),
            $"ListTransfer target side should contain only target keys. Actual: {string.Join(", ", targetKeys)}.",
            failures);

        transfer.TargetKeys = null;
        RefreshLayout(realized.Window);

        sourceKeys = GetViewItemKeys(views.Single(view => view.ViewType == TransferViewType.Source));
        targetKeys = GetViewItemKeys(views.Single(view => view.ViewType == TransferViewType.Target));
        Expect(sourceKeys.SequenceEqual(items.Select(item => item.ItemKey ?? default)),
            "ListTransfer source side should restore all items after TargetKeys is cleared.",
            failures);
        Expect(targetKeys.Count == 0,
            "ListTransfer target side should be empty after TargetKeys is cleared.",
            failures);
    }

    private static void VerifyTreeTransferTargetKeepsNestedOrder(ICollection<string> failures)
    {
        var root = new TreeItemNode
        {
            ItemKey    = "root",
            Header     = "Root",
            IsExpanded = true,
            Children =
            {
                new TreeItemNode
                {
                    ItemKey = "child-1",
                    Header  = "Child 1"
                },
                new TreeItemNode
                {
                    ItemKey    = "child-2",
                    Header     = "Child 2",
                    IsExpanded = true,
                    Children =
                    {
                        new TreeItemNode
                        {
                            ItemKey = "leaf-1",
                            Header  = "Leaf 1"
                        }
                    }
                }
            }
        };
        var transfer = new TreeTransfer
        {
            ItemsSource = [root],
            TargetKeys  = ["child-2", "leaf-1"]
        };

        using var realized = RealizeControl(transfer);
        var views      = GetTransferListViews(transfer);
        var targetKeys = GetViewItemKeys(views.Single(view => view.ViewType == TransferViewType.Target));

        Expect(targetKeys.SequenceEqual(["child-2", "leaf-1"]),
            $"TreeTransfer target side should flatten selected nested nodes in tree order. Actual: {string.Join(", ", targetKeys)}.",
            failures);

        transfer.TargetKeys = null;
        RefreshLayout(realized.Window);

        targetKeys = GetViewItemKeys(views.Single(view => view.ViewType == TransferViewType.Target));
        Expect(targetKeys.Count == 0,
            "TreeTransfer target side should be empty after TargetKeys is cleared.",
            failures);
    }

    private static void VerifyTransferListViewClearsRemovedPaginationState(ICollection<string> failures)
    {
        var view = new TransferListView
        {
            ItemsSource = CreateTransferListItems(20),
            PageSize = 10
        };

        using var realized = RealizeControl(view);
        var defaultPagination = view.BottomPagination;
        Expect(defaultPagination is SimplePagination,
            "TransferListView should create a default SimplePagination on template application.",
            failures);

        view.BottomPagination = null;
        RefreshLayout(realized.Window);

        Expect(view.BottomPagination == null,
            "Clearing BottomPagination should keep the public property null.",
            failures);
        Expect(GetPrivateField(view, "AtomUI.Desktop.Controls.ListView", "_bottomPagination") == null,
            "Clearing BottomPagination should also clear ListView's cached pagination reference.",
            failures);
        Expect(defaultPagination?.GetVisualParent() == null,
            "Cleared Transfer SimplePagination should not keep a visual parent.",
            failures);

        view.ItemsSource = CreateTransferListItems(30);
        RefreshLayout(realized.Window);
        Expect(view.BottomPagination == null,
            "Reconfiguring items after BottomPagination is cleared should not resurrect a stale pagination.",
            failures);
    }

    private static List<TransferListView> GetTransferListViews(Control root)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<TransferListView>()
                   .ToList();
    }

    private static IReadOnlyList<EntityKey> GetViewItemKeys(TransferListView view)
    {
        return view.ItemsSource?
                   .Cast<IItemKey>()
                   .Select(item => item.ItemKey ?? default)
                   .ToArray() ?? [];
    }

    private static IReadOnlyList<IListItemData> CreateTransferListItems(int count)
    {
        var items = new List<IListItemData>(count);
        for (var i = 0; i < count; i++)
        {
            items.Add(new ListItemData
            {
                ItemKey = i.ToString(),
                Content = $"Content {i + 1}"
            });
        }
        return items;
    }
}
