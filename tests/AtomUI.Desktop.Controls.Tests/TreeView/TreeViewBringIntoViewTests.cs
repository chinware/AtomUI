using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomTreeView = AtomUI.Desktop.Controls.TreeView;
using AtomTreeViewItem = AtomUI.Desktop.Controls.TreeViewItem;

namespace AtomUI.Desktop.Controls.Tests.TreeView;

public class TreeViewBringIntoViewTests
{
    static TreeViewBringIntoViewTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Descendant_BringIntoView_Does_Not_Bubble_To_Outer_Parent()
    {
        var child = new TreeItemNode
        {
            Header  = "child",
            ItemKey = "0-0"
        };
        var root = new TreeItemNode
        {
            Header     = "root",
            ItemKey    = "0",
            IsExpanded = true,
            Children   = [child]
        };
        var treeView = new AtomTreeView
        {
            IsShowEmptyIndicator = false,
            ItemsSource          = new[] { root },
            SelectedItem         = child
        };
        var host = new StackPanel
        {
            Children =
            {
                treeView
            }
        };
        var bringIntoViewRequests = 0;
        host.AddHandler(
            Control.RequestBringIntoViewEvent,
            (_, _) => bringIntoViewRequests++);

        ShowInWindow(host, () =>
        {
            var childContainer = FindContainerForNode(treeView, child);
            childContainer.ShouldNotBeNull();

            childContainer.BringIntoView();
            Dispatcher.UIThread.RunJobs();

            bringIntoViewRequests.ShouldBe(0);
        });
    }

    [Fact]
    public void TreeView_BringIntoView_Still_Bubbles_To_Outer_Parent()
    {
        var treeView = new AtomTreeView
        {
            IsShowEmptyIndicator = false,
            Items                = { new AtomTreeViewItem { Header = "root" } }
        };
        var host = new StackPanel
        {
            Children =
            {
                treeView
            }
        };
        var bringIntoViewRequests = 0;
        host.AddHandler(
            Control.RequestBringIntoViewEvent,
            (_, _) => bringIntoViewRequests++);

        ShowInWindow(host, () =>
        {
            treeView.BringIntoView();

            bringIntoViewRequests.ShouldBe(1);
        });
    }

    private static AtomTreeViewItem? FindContainerForNode(Control root, ITreeItemNode node)
    {
        return root.GetVisualDescendants()
                   .OfType<AtomTreeViewItem>()
                   .FirstOrDefault(item => ReferenceEquals(item.Header, node));
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new Avalonia.Controls.Window
        {
            Width   = 420,
            Height  = 320,
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
}
