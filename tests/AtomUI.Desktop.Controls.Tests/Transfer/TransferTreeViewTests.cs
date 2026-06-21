using AtomUI.Controls;
using Avalonia.Controls;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Transfer;

public class TransferTreeViewTests
{
    static TransferTreeViewTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void SelectedKeys_Change_Selects_Nested_Tree_Node()
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

        var treeView = new TransferTreeView
        {
            ItemsSource = new[] { parent }
        };

        treeView.SelectedKeys = new List<EntityKey> { "child" };

        treeView.CheckedItems.Contains(child).ShouldBeTrue();
    }

    [Fact]
    public void Prepared_Tree_Container_Uses_Current_MaskKeys()
    {
        var child = new TreeItemNode
        {
            ItemKey = "child",
            Header  = "Child"
        };
        var treeView = new TestTransferTreeView
        {
            MaskKeys = new HashSet<EntityKey> { "child" }
        };
        var container = new TransferTreeViewItem();

        treeView.PrepareForTest(container, child);

        container.IsMasked.ShouldBeTrue();
    }

    [Fact]
    public void TreeTransfer_Applies_Initial_TargetKeys_To_SourceView_Mask()
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

        var transfer = new TreeTransfer
        {
            ItemsSource = new[] { parent },
            TargetKeys  = new List<EntityKey> { "child" },
            Width       = 520,
            Height      = 260
        };

        ShowInWindow(transfer, () =>
        {
            Dispatcher.UIThread.RunJobs();
            var sourceView = transfer.SourceView.ShouldBeOfType<TransferTreeView>();
            sourceView.MaskKeys.ShouldNotBeNull();
            sourceView.MaskKeys.ShouldContain("child");
        });
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

    private sealed class TestTransferTreeView : TransferTreeView
    {
        public void PrepareForTest(TreeViewItem treeViewItem, object? item)
        {
            PrepareTreeViewItem(treeViewItem, item, 0);
        }
    }
}
