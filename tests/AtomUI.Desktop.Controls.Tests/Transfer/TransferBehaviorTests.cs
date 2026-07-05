using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Controls.Data;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Transfer;

public class TransferBehaviorTests
{
    static TransferBehaviorTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Key_Value_Properties_Are_TwoWay()
    {
        var selectedKeysMetadata = AbstractTransfer.SelectedKeysProperty.GetMetadata(typeof(AbstractTransfer));
        var targetKeysMetadata   = AbstractTransfer.TargetKeysProperty.GetMetadata(typeof(AbstractTransfer));

        selectedKeysMetadata.DefaultBindingMode.ShouldBe(BindingMode.TwoWay);
        targetKeysMetadata.DefaultBindingMode.ShouldBe(BindingMode.TwoWay);
    }

    [Fact]
    public void TargetKeys_ObservableCollection_Mutation_Refreshes_List_Panels()
    {
        var firstItem = CreateListItem("first");
        var secondItem = CreateListItem("second");
        var targetKeys = new ObservableCollection<EntityKey> { "first" };
        var transfer = new ListTransfer
        {
            Width       = 520,
            Height      = 260,
            ItemsSource = new[] { firstItem, secondItem },
            TargetKeys  = targetKeys
        };

        ShowInWindow(transfer, () =>
        {
            var sourceView = transfer.SourceView.ShouldBeOfType<TransferListView>();
            var targetView = transfer.TargetView.ShouldBeOfType<TransferListView>();
            sourceView.Items.Cast<IItemKey>().Select(item => item.ItemKey).ShouldBe([secondItem.ItemKey]);
            targetView.Items.Cast<IItemKey>().Select(item => item.ItemKey).ShouldBe([firstItem.ItemKey]);

            targetKeys.Add("second");
            Dispatcher.UIThread.RunJobs();

            sourceView.Items.Cast<IItemKey>().ShouldBeEmpty();
            targetView.Items.Cast<IItemKey>()
                      .Select(item => item.ItemKey)
                      .ShouldBe([firstItem.ItemKey, secondItem.ItemKey]);

            targetKeys.Remove("first");
            Dispatcher.UIThread.RunJobs();

            sourceView.Items.Cast<IItemKey>().Select(item => item.ItemKey).ShouldBe([firstItem.ItemKey]);
            targetView.Items.Cast<IItemKey>().Select(item => item.ItemKey).ShouldBe([secondItem.ItemKey]);
        });
    }

    [Fact]
    public void TargetKeys_ObservableCollection_Mutation_Refreshes_Tree_Mask()
    {
        var child = new TreeItemNode
        {
            ItemKey = "child",
            Header  = "Child"
        };
        var parent = new TreeItemNode
        {
            ItemKey = "parent",
            Header  = "Parent"
        };
        parent.Children.Add(child);
        var targetKeys = new ObservableCollection<EntityKey>();
        var transfer = new TreeTransfer
        {
            ItemsSource = new[] { parent },
            TargetKeys  = targetKeys,
            Width       = 520,
            Height      = 260
        };

        ShowInWindow(transfer, () =>
        {
            var sourceView = transfer.SourceView.ShouldBeOfType<TransferTreeView>();
            sourceView.MaskKeys.ShouldBeEmpty();

            targetKeys.Add("child");
            Dispatcher.UIThread.RunJobs();

            sourceView.MaskKeys.ShouldNotBeNull();
            sourceView.MaskKeys.ShouldContain("child");

            targetKeys.Clear();
            Dispatcher.UIThread.RunJobs();

            sourceView.MaskKeys.ShouldBeEmpty();
        });
    }

    [Fact]
    public void SelectedKeys_ObservableCollection_Mutation_Splits_Selection_Between_List_Panels()
    {
        var firstItem = CreateListItem("first");
        var secondItem = CreateListItem("second");
        var selectedKeys = new ObservableCollection<EntityKey> { "first" };
        var transfer = new ListTransfer
        {
            Width        = 520,
            Height       = 260,
            ItemsSource  = new[] { firstItem, secondItem },
            TargetKeys   = new List<EntityKey> { "second" },
            SelectedKeys = selectedKeys
        };

        ShowInWindow(transfer, () =>
        {
            var sourceView = transfer.SourceView.ShouldBeOfType<TransferListView>();
            var targetView = transfer.TargetView.ShouldBeOfType<TransferListView>();
            sourceView.SelectedKeys.ShouldBe(["first"]);
            targetView.SelectedKeys.ShouldBeNull();

            selectedKeys.Add("second");
            Dispatcher.UIThread.RunJobs();

            sourceView.SelectedKeys.ShouldBe(["first"]);
            targetView.SelectedKeys.ShouldBe(["second"]);

            selectedKeys.Clear();
            Dispatcher.UIThread.RunJobs();

            sourceView.SelectedKeys.ShouldBeNull();
            targetView.SelectedKeys.ShouldBeNull();
        });
    }

    [Fact]
    public void Clearing_TargetKeys_Keeps_Equivalent_Source_Selection_Stable()
    {
        var items = Enumerable.Range(1, 6)
                              .Select(index => CreateListItem(index.ToString()))
                              .ToList();
        var targetKeys = new ObservableCollection<EntityKey> { "3" };
        var selectedKeys = new ObservableCollection<EntityKey> { "2", "4" };
        var transfer = new ListTransfer
        {
            Width        = 520,
            Height       = 260,
            ItemsSource  = items,
            TargetKeys   = targetKeys,
            SelectedKeys = selectedKeys
        };

        ShowInWindow(transfer, () =>
        {
            var sourceView = transfer.SourceView.ShouldBeOfType<TransferListView>();
            sourceView.SelectedKeys.ShouldBe(["2", "4"]);
            sourceView.SelectedItems.ShouldNotBeNull();
            sourceView.SelectedItems
                      .Cast<IItemKey>()
                      .Select(item => item.ItemKey)
                      .ShouldBe(["2", "4"]);

            var originalSourceSelectedKeys = sourceView.SelectedKeys;
            var selectedKeysChangeCount    = 0;
            var selectedItemsChangeCount   = 0;
            var observedEmptySelection     = false;

            sourceView.PropertyChanged += (_, args) =>
            {
                if (args.Property == TransferListView.SelectedKeysProperty)
                {
                    selectedKeysChangeCount++;
                }
                else if (args.Property == global::AtomUI.Desktop.Controls.ListView.SelectedItemsProperty)
                {
                    selectedItemsChangeCount++;
                    observedEmptySelection |= sourceView.SelectedItems == null || sourceView.SelectedItems.Count == 0;
                }
            };

            targetKeys.Clear();
            Dispatcher.UIThread.RunJobs();

            sourceView.SelectedKeys.ShouldBeSameAs(originalSourceSelectedKeys);
            sourceView.SelectedKeys.ShouldBe(["2", "4"]);
            sourceView.SelectedItems
                      .Cast<IItemKey>()
                      .Select(item => item.ItemKey)
                      .ShouldBe(["2", "4"]);
            selectedKeysChangeCount.ShouldBe(0);
            selectedItemsChangeCount.ShouldBe(0);
            observedEmptySelection.ShouldBeFalse();
        });
    }

    [Fact]
    public void Clearing_Empty_TargetKeys_Does_Not_Rebuild_List_Panel_Sources()
    {
        var items = Enumerable.Range(1, 6)
                              .Select(index => CreateListItem(index.ToString()))
                              .ToList();
        var targetKeys = new ObservableCollection<EntityKey>();
        var selectedKeys = new ObservableCollection<EntityKey> { "2", "4" };
        var transfer = new ListTransfer
        {
            Width        = 520,
            Height       = 260,
            ItemsSource  = items,
            TargetKeys   = targetKeys,
            SelectedKeys = selectedKeys
        };

        ShowInWindow(transfer, () =>
        {
            var sourceView = transfer.SourceView.ShouldBeOfType<TransferListView>();
            var originalSourceViewSource = transfer.SourceViewSource;
            var originalTargetViewSource = transfer.TargetViewSource;
            var originalSourceItemsSource = sourceView.ItemsSource;
            var sourceViewSourceChangeCount = 0;
            var targetViewSourceChangeCount = 0;
            var sourceItemsSourceChangeCount = 0;

            transfer.PropertyChanged += (_, args) =>
            {
                if (args.Property == AbstractTransfer.SourceViewSourceProperty)
                {
                    sourceViewSourceChangeCount++;
                }
                else if (args.Property == AbstractTransfer.TargetViewSourceProperty)
                {
                    targetViewSourceChangeCount++;
                }
            };
            sourceView.PropertyChanged += (_, args) =>
            {
                if (args.Property == ItemsControl.ItemsSourceProperty)
                {
                    sourceItemsSourceChangeCount++;
                }
            };

            targetKeys.Clear();
            Dispatcher.UIThread.RunJobs();

            transfer.SourceViewSource.ShouldBeSameAs(originalSourceViewSource);
            transfer.TargetViewSource.ShouldBeSameAs(originalTargetViewSource);
            sourceView.ItemsSource.ShouldBeSameAs(originalSourceItemsSource);
            sourceViewSourceChangeCount.ShouldBe(0);
            targetViewSourceChangeCount.ShouldBe(0);
            sourceItemsSourceChangeCount.ShouldBe(0);
        });
    }

    [Fact]
    public void Clearing_TargetKeys_Updates_List_Panel_Sources_In_Place()
    {
        var items = Enumerable.Range(1, 6)
                              .Select(index => CreateListItem(index.ToString()))
                              .ToList();
        var targetKeys = new ObservableCollection<EntityKey> { "1", "3" };
        var selectedKeys = new ObservableCollection<EntityKey> { "2", "4" };
        var transfer = new ListTransfer
        {
            Width        = 520,
            Height       = 260,
            ItemsSource  = items,
            TargetKeys   = targetKeys,
            SelectedKeys = selectedKeys
        };

        ShowInWindow(transfer, () =>
        {
            var sourceView = transfer.SourceView.ShouldBeOfType<TransferListView>();
            var targetView = transfer.TargetView.ShouldBeOfType<TransferListView>();
            var originalSourceViewSource = transfer.SourceViewSource;
            var originalTargetViewSource = transfer.TargetViewSource;
            var originalSourceItemsSource = sourceView.ItemsSource;
            var originalTargetItemsSource = targetView.ItemsSource;
            var sourceViewSourceChangeCount = 0;
            var targetViewSourceChangeCount = 0;
            var sourceItemsSourceChangeCount = 0;
            var targetItemsSourceChangeCount = 0;

            transfer.PropertyChanged += (_, args) =>
            {
                if (args.Property == AbstractTransfer.SourceViewSourceProperty)
                {
                    sourceViewSourceChangeCount++;
                }
                else if (args.Property == AbstractTransfer.TargetViewSourceProperty)
                {
                    targetViewSourceChangeCount++;
                }
            };
            sourceView.PropertyChanged += (_, args) =>
            {
                if (args.Property == ItemsControl.ItemsSourceProperty)
                {
                    sourceItemsSourceChangeCount++;
                }
            };
            targetView.PropertyChanged += (_, args) =>
            {
                if (args.Property == ItemsControl.ItemsSourceProperty)
                {
                    targetItemsSourceChangeCount++;
                }
            };

            targetKeys.Clear();
            Dispatcher.UIThread.RunJobs();

            transfer.SourceViewSource.ShouldBeSameAs(originalSourceViewSource);
            transfer.TargetViewSource.ShouldBeSameAs(originalTargetViewSource);
            sourceView.ItemsSource.ShouldBeSameAs(originalSourceItemsSource);
            targetView.ItemsSource.ShouldBeSameAs(originalTargetItemsSource);
            sourceViewSourceChangeCount.ShouldBe(0);
            targetViewSourceChangeCount.ShouldBe(0);
            sourceItemsSourceChangeCount.ShouldBe(0);
            targetItemsSourceChangeCount.ShouldBe(0);
            sourceView.Items.Cast<IItemKey>()
                      .Select(item => item.ItemKey)
                      .ShouldBe(["1", "2", "3", "4", "5", "6"]);
            targetView.Items.Cast<IItemKey>().ShouldBeEmpty();
            sourceView.SelectedKeys.ShouldBe(["2", "4"]);
        });
    }

    [Fact]
    public void Clearing_TargetKeys_Does_Not_Crash_When_List_Panels_Recycle_Containers()
    {
        var items = Enumerable.Range(1, 64)
                              .Select(index => CreateListItem(index.ToString()))
                              .ToList();
        var targetKeys = new ObservableCollection<EntityKey>(
            Enumerable.Range(1, 32).Select(index => (EntityKey)index.ToString()));
        var selectedKeys = new ObservableCollection<EntityKey> { "33", "35" };
        var transfer = new ListTransfer
        {
            Width        = 520,
            Height       = 220,
            ItemsSource  = items,
            TargetKeys   = targetKeys,
            SelectedKeys = selectedKeys
        };

        ShowInWindow(transfer, () =>
        {
            targetKeys.Clear();
            Dispatcher.UIThread.RunJobs();
            Dispatcher.UIThread.RunJobs();

            transfer.TargetKeys.ShouldBeEmpty();
            transfer.SourceView.ShouldBeOfType<TransferListView>()
                    .SelectedKeys.ShouldBe(["33", "35"]);
        });
    }

    [Fact]
    public void Transfer_To_Target_Updates_Existing_TargetKeys_Collection()
    {
        var firstItem = CreateListItem("first");
        var secondItem = CreateListItem("second");
        var targetKeys = new ObservableCollection<EntityKey> { "first" };
        var transfer = new ListTransfer
        {
            Width       = 520,
            Height      = 260,
            ItemsSource = new[] { firstItem, secondItem },
            TargetKeys  = targetKeys
        };

        ShowInWindow(transfer, () =>
        {
            var sourceView = transfer.SourceView.ShouldBeOfType<TransferListView>();
            sourceView.SelectedKeys = new List<EntityKey> { "second" };
            Dispatcher.UIThread.RunJobs();

            TransferForTest(transfer, TransferDirection.ToTarget);
            Dispatcher.UIThread.RunJobs();

            transfer.TargetKeys.ShouldBeSameAs(targetKeys);
            targetKeys.ShouldBe(["first", "second"]);
            transfer.SelectedKeys.ShouldBeEmpty();
        });
    }

    [Fact]
    public void RemoveAll_Clears_Existing_TargetKeys_Collection()
    {
        var firstItem = CreateListItem("first");
        var secondItem = CreateListItem("second");
        var targetKeys = new ObservableCollection<EntityKey> { "first", "second" };
        var transfer = new ListTransfer
        {
            Width       = 520,
            Height      = 260,
            ItemsSource = new[] { firstItem, secondItem },
            TargetKeys  = targetKeys
        };

        ShowInWindow(transfer, () =>
        {
            transfer.RaiseEvent(new TransferSelectActionEventArgs(TransferSelectAction.RemoveAll)
            {
                RoutedEvent = TransferSelectDropdown.SelectActionRequestEvent,
                Source      = transfer
            });
            Dispatcher.UIThread.RunJobs();

            transfer.TargetKeys.ShouldBeSameAs(targetKeys);
            targetKeys.ShouldBeEmpty();
        });
    }

    [Fact]
    public void TransferListView_SelectedKeys_ObservableCollection_Mutation_Refreshes_SelectedItems()
    {
        var firstItem = CreateListItem("first");
        var secondItem = CreateListItem("second");
        var selectedKeys = new ObservableCollection<EntityKey> { "first" };
        var view = new TransferListView
        {
            ItemsSource  = new[] { firstItem, secondItem },
            SelectedKeys = selectedKeys
        };

        ShowInWindow(view, () =>
        {
            view.SelectedItems.ShouldNotBeNull();
            view.SelectedItems.Cast<IListItemData>().ShouldBe([firstItem]);

            selectedKeys.Add("second");
            Dispatcher.UIThread.RunJobs();

            view.SelectedItems.Cast<IListItemData>().ShouldBe([firstItem, secondItem]);

            selectedKeys.Clear();
            Dispatcher.UIThread.RunJobs();

            view.SelectedItems.Count.ShouldBe(0);
        });
    }

    [Fact]
    public void TransferTreeView_SelectedKeys_ObservableCollection_Mutation_Refreshes_CheckedItems()
    {
        var firstNode = new TreeItemNode
        {
            ItemKey = "first",
            Header  = "First"
        };
        var secondNode = new TreeItemNode
        {
            ItemKey = "second",
            Header  = "Second"
        };
        var selectedKeys = new ObservableCollection<EntityKey> { "first" };
        var view = new TransferTreeView
        {
            ItemsSource  = new[] { firstNode, secondNode },
            SelectedKeys = selectedKeys
        };
        var selectionCount = 0;
        view.SelectionCountChanged += (_, args) => selectionCount = args.Count;

        ShowInWindow(view, () =>
        {
            view.SelectedKeys.ShouldBeSameAs(selectedKeys);
            view.CheckedItems.Cast<ITreeItemNode>().ShouldBe([firstNode]);

            selectedKeys.Add("second");
            Dispatcher.UIThread.RunJobs();

            selectionCount.ShouldBe(2);
            view.CheckedItems
                .Cast<ITreeItemNode>()
                .Select(item => item.ItemKey)
                .ShouldBe(["first", "second"]);
            view.CheckedItems.Count.ShouldBe(2);
            view.CheckedItems.Cast<ITreeItemNode>().ShouldContain(firstNode);
            view.CheckedItems.Cast<ITreeItemNode>().ShouldContain(secondNode);

            selectedKeys.Clear();
            Dispatcher.UIThread.RunJobs();

            view.CheckedItems.Count.ShouldBe(0);
        });
    }

    private static ListItemData CreateListItem(string key)
    {
        return new ListItemData
        {
            ItemKey = key,
            Content = key
        };
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 640,
            Height  = 360,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
        }
    }

    private static void TransferForTest(AbstractTransfer transfer, TransferDirection transferDirection)
    {
        var method = typeof(AbstractTransfer).GetMethod(
            "TransferItems",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        method.ShouldNotBeNull();
        method.Invoke(transfer, [transferDirection]);
    }
}
