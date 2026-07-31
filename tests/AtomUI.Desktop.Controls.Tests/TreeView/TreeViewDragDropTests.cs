using System.Reflection;
using System.Runtime.ExceptionServices;
using AtomUI.Controls;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomTreeView = AtomUI.Desktop.Controls.TreeView;
using AtomTreeViewItem = AtomUI.Desktop.Controls.TreeViewItem;

namespace AtomUI.Desktop.Controls.Tests.TreeView;

public class TreeViewDragDropTests
{
    static TreeViewDragDropTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void ItemsSource_Root_Drop_Reorders_Source_Collection()
    {
        var first  = new TreeItemNode { Header = "first" };
        var second = new TreeItemNode { Header = "second" };
        var third  = new TreeItemNode { Header = "third" };
        var source = new AvaloniaList<ITreeItemNode> { first, second, third };
        var droppedCount = 0;
        var treeView = new AtomTreeView
        {
            IsShowEmptyIndicator = false,
            ItemsSource          = source
        };
        treeView.ItemDropped += (_, _) => droppedCount++;

        ShowInWindow(treeView, () =>
        {
            var secondContainer = FindContainerForNode(treeView, second)!;

            Should.NotThrow(() => PerformDrop(treeView, secondContainer, new DropTargetInfo
            {
                IsRoot = true,
                Index  = 0
            }));

            source.ShouldBe([second, first, third]);
            droppedCount.ShouldBe(1);
        });
    }

    [Fact]
    public void ItemsSource_Child_Drop_Moves_Node_Between_Children_And_Updates_Parent()
    {
        var childA = new TreeItemNode { Header = "child A" };
        var childB = new TreeItemNode { Header = "child B" };
        var parentA = new TreeItemNode
        {
            Header     = "parent A",
            IsExpanded = true,
            Children   = [childA]
        };
        var parentB = new TreeItemNode
        {
            Header     = "parent B",
            IsExpanded = true,
            Children   = [childB]
        };
        var source = new AvaloniaList<ITreeItemNode> { parentA, parentB };
        var droppedCount = 0;
        var treeView = new AtomTreeView
        {
            IsShowEmptyIndicator = false,
            ItemsSource          = source
        };
        treeView.ItemDropped += (_, _) => droppedCount++;

        ShowInWindow(treeView, () =>
        {
            RunDispatcherJobsUntil(() =>
                FindContainerForNode(treeView, childA) is not null &&
                FindContainerForNode(treeView, parentB) is not null);
            var childAContainer = FindContainerForNode(treeView, childA)!;
            var parentBContainer = FindContainerForNode(treeView, parentB)!;

            Should.NotThrow(() => PerformDrop(treeView, childAContainer, new DropTargetInfo
            {
                TargetTreeItem = parentBContainer,
                Index          = 1,
                IsRoot         = false
            }));

            parentA.Children.ShouldBeEmpty();
            parentB.Children.ShouldBe([childB, childA]);
            childA.ParentNode.ShouldBeSameAs(parentB);
            droppedCount.ShouldBe(1);
        });
    }

    [Fact]
    public void Same_Collection_Root_Drop_Does_Not_Adjust_Index_When_Source_Is_After_Target()
    {
        var first  = new TreeItemNode { Header = "first" };
        var second = new TreeItemNode { Header = "second" };
        var third  = new TreeItemNode { Header = "third" };
        var fourth = new TreeItemNode { Header = "fourth" };
        var source = new AvaloniaList<ITreeItemNode> { first, second, third, fourth };
        var treeView = new AtomTreeView
        {
            IsShowEmptyIndicator = false,
            ItemsSource          = source
        };

        ShowInWindow(treeView, () =>
        {
            var thirdContainer = FindContainerForNode(treeView, third)!;

            Should.NotThrow(() => PerformDrop(treeView, thirdContainer, new DropTargetInfo
            {
                IsRoot = true,
                Index  = 1
            }));

            source.ShouldBe([first, third, second, fourth]);
        });
    }

    [Fact]
    public void ItemsSource_Drop_Preserves_Selected_And_Checked_Node_State()
    {
        var first  = new TreeItemNode { Header = "first" };
        var second = new TreeItemNode { Header = "second" };
        var third  = new TreeItemNode { Header = "third" };
        var source = new AvaloniaList<ITreeItemNode> { first, second, third };
        var treeView = new AtomTreeView
        {
            IsShowEmptyIndicator = false,
            ItemsSource          = source,
            SelectionMode        = SelectionMode.Multiple,
            ToggleType           = ItemToggleType.CheckBox
        };

        ShowInWindow(treeView, () =>
        {
            treeView.SelectedItems.Add(second);
            treeView.CheckedItems.Add(second);
            Dispatcher.UIThread.RunJobs();
            var secondContainer = FindContainerForNode(treeView, second)!;

            Should.NotThrow(() => PerformDrop(treeView, secondContainer, new DropTargetInfo
            {
                IsRoot = true,
                Index  = 3
            }));

            source.ShouldBe([first, third, second]);
            treeView.SelectedItems.Contains(second).ShouldBeTrue();
            treeView.CheckedItems.Contains(second).ShouldBeTrue();
        });
    }

    [Fact]
    public void Immutable_ItemsSource_Drop_Cancels_Without_Throwing_Or_Raising_Dropped()
    {
        var first  = new TreeItemNode { Header = "first" };
        var second = new TreeItemNode { Header = "second" };
        var source = new ITreeItemNode[] { first, second };
        var droppedCount = 0;
        var treeView = new AtomTreeView
        {
            IsShowEmptyIndicator = false,
            ItemsSource          = source
        };
        treeView.ItemDropped += (_, _) => droppedCount++;

        ShowInWindow(treeView, () =>
        {
            var secondContainer = FindContainerForNode(treeView, second)!;

            Should.NotThrow(() => PerformDrop(treeView, secondContainer, new DropTargetInfo
            {
                IsRoot = true,
                Index  = 0
            }));

            source.ShouldBe([first, second]);
            droppedCount.ShouldBe(0);
        });
    }

    private static void PerformDrop(AtomTreeView treeView, AtomTreeViewItem source, DropTargetInfo targetInfo)
    {
        SetPrivateField(treeView, "_beingDraggedTreeItem", source);
        SetPrivateField(treeView, "_dropTargetInfo", targetInfo);

        try
        {
            typeof(AtomTreeView)
                .GetMethod("PerformDropOperation", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(treeView, null);
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
        }
    }

    private static void SetPrivateField(AtomTreeView treeView, string fieldName, object? value)
    {
        typeof(AtomTreeView)
            .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(treeView, value);
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

    private static void RunDispatcherJobsUntil(Func<bool> condition, int maxPasses = 128)
    {
        for (var i = 0; i < maxPasses; i++)
        {
            Dispatcher.UIThread.RunJobs();
            if (condition())
            {
                return;
            }
        }
    }
}
