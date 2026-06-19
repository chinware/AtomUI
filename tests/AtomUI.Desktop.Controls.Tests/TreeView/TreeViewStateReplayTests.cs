using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AtomTreeViewItem = AtomUI.Desktop.Controls.TreeViewItem;

namespace AtomUI.Desktop.Controls.Tests.TreeView;

public class TreeViewStateReplayTests
{
    static TreeViewStateReplayTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Loaded_Replays_Default_Selected_Checked_And_Expanded_Paths_In_Current_Order()
    {
        var treeView = new AtomUI.Desktop.Controls.TreeView
        {
            ToggleType            = ItemToggleType.CheckBox,
            IsMotionEnabled       = false,
            DefaultSelectedPaths  = [new TreeNodePath("0-0/0-0-1")],
            DefaultCheckedPaths   = [new TreeNodePath("0-0/0-0-1/0-0-1-1")],
            DefaultExpandedPaths  = [new TreeNodePath("0-0/0-0-0")],
            IsShowEmptyIndicator = false
        };
        var (root, firstChild, secondChild, checkedLeaf) = AddDefaultPathTreeItems(treeView);

        ShowInWindow(treeView, () =>
        {
            root.IsExpanded.ShouldBeTrue();
            firstChild.IsExpanded.ShouldBeTrue();
            secondChild.IsExpanded.ShouldBeFalse();
            secondChild.IsSelected.ShouldBeTrue();
            checkedLeaf.IsChecked.ShouldBe(true);
            treeView.SelectedItems.Contains(secondChild).ShouldBeTrue();
            treeView.CheckedItems.Contains(checkedLeaf).ShouldBeTrue();
        });
    }

    [Fact]
    public void IsDefaultExpandAll_Wins_Over_DefaultExpandedPaths_During_Loaded_Replay()
    {
        var treeView = new AtomUI.Desktop.Controls.TreeView
        {
            IsMotionEnabled      = false,
            IsDefaultExpandAll   = true,
            DefaultExpandedPaths = [new TreeNodePath("missing/path")]
        };
        var (root, firstChild, secondChild, _) = AddDefaultPathTreeItems(treeView);

        ShowInWindow(treeView, () =>
        {
            RunDispatcherJobsUntil(() => root.IsExpanded && firstChild.IsExpanded && secondChild.IsExpanded);

            root.IsExpanded.ShouldBeTrue();
            firstChild.IsExpanded.ShouldBeTrue();
            secondChild.IsExpanded.ShouldBeTrue();
        });
    }

    [Fact]
    public void ItemsSource_Change_Restores_Runtime_Checked_State_Including_Previous_Defaults()
    {
        var firstNodes = CreateDefaultPathDataNodes();
        var secondNodes = CreateDefaultPathDataNodes();
        var treeView = new AtomUI.Desktop.Controls.TreeView
        {
            ToggleType           = ItemToggleType.CheckBox,
            IsMotionEnabled      = false,
            ItemsSource          = firstNodes,
            DefaultSelectedPaths = [new TreeNodePath("0-0/0-0-0")],
            DefaultCheckedPaths  = [new TreeNodePath("0-0/0-0-0")]
        };

        ShowInWindow(treeView, () =>
        {
            var selectedNode = firstNodes[0].Children.ElementAt(1);
            var checkedNode  = firstNodes[0].Children.ElementAt(1).Children.ElementAt(0);
            treeView.SelectedItem = selectedNode;
            treeView.CheckedItems.Add(checkedNode);
            Dispatcher.UIThread.RunJobs();

            treeView.ItemsSource = secondNodes;
            RunDispatcherJobsUntil(() => ReferenceEquals(treeView.SelectedItem, secondNodes[0].Children.ElementAt(1)));

            treeView.SelectedItem.ShouldBeSameAs(secondNodes[0].Children.ElementAt(1));
            treeView.CheckedItems.Contains(secondNodes[0].Children.ElementAt(1).Children.ElementAt(0)).ShouldBeTrue();
            treeView.SelectedItems.Contains(secondNodes[0].Children.ElementAt(0)).ShouldBeFalse();
            treeView.CheckedItems.Contains(secondNodes[0].Children.ElementAt(0)).ShouldBeTrue();
        });
    }

    [Fact]
    public void Default_Checked_Paths_Cascade_To_Parents_When_CheckStrictly_Is_False()
    {
        var treeView = new AtomUI.Desktop.Controls.TreeView
        {
            ToggleType           = ItemToggleType.CheckBox,
            IsMotionEnabled      = false,
            IsCheckStrictly      = false,
            DefaultCheckedPaths  = [new TreeNodePath("0-0/0-0-1/0-0-1-1")]
        };
        var (root, _, secondChild, checkedLeaf) = AddDefaultPathTreeItems(treeView);

        ShowInWindow(treeView, () =>
        {
            checkedLeaf.IsChecked.ShouldBe(true);
            secondChild.IsChecked.ShouldBe(true);
            root.IsChecked.ShouldBe(null);
            treeView.CheckedItems.Contains(checkedLeaf).ShouldBeTrue();
            treeView.CheckedItems.Contains(secondChild).ShouldBeTrue();
        });
    }

    [Fact]
    public void Strict_Default_Checked_State_Updates_Current_Item_Without_Cascade()
    {
        var nodes = CreateDefaultPathDataNodes();
        var treeView = new AtomUI.Desktop.Controls.TreeView
        {
            ToggleType           = ItemToggleType.CheckBox,
            IsMotionEnabled      = false,
            IsCheckStrictly      = true,
            ItemsSource          = nodes,
            DefaultCheckedPaths = [new TreeNodePath("0-0/0-0-1/0-0-1-1")]
        };
        var root        = nodes[0];
        var secondChild = root.Children.ElementAt(1);
        var checkedLeaf = secondChild.Children.ElementAt(0);

        ShowInWindow(treeView, () =>
        {
            treeView.CheckedItems.Contains(checkedLeaf).ShouldBeTrue();
            treeView.CheckedItems.Contains(secondChild).ShouldBeFalse();
            treeView.CheckedItems.Contains(root).ShouldBeFalse();
        });
    }

    private static (AtomTreeViewItem Root, AtomTreeViewItem FirstChild, AtomTreeViewItem SecondChild, AtomTreeViewItem CheckedLeaf)
        AddDefaultPathTreeItems(AtomUI.Desktop.Controls.TreeView treeView)
    {
        var checkedLeaf = new AtomTreeViewItem
        {
            Header  = "checked leaf",
            ItemKey = "0-0-1-1"
        };
        var secondChild = new AtomTreeViewItem
        {
            Header  = "second child",
            ItemKey = "0-0-1"
        };
        secondChild.Items.Add(checkedLeaf);
        var firstChild = new AtomTreeViewItem
        {
            Header  = "first child",
            ItemKey = "0-0-0"
        };
        firstChild.Items.Add(new AtomTreeViewItem
        {
            Header  = "first leaf",
            ItemKey = "0-0-0-0"
        });
        var root = new AtomTreeViewItem
        {
            Header  = "root",
            ItemKey = "0-0"
        };
        root.Items.Add(firstChild);
        root.Items.Add(secondChild);
        treeView.Items.Add(root);
        return (root, firstChild, secondChild, checkedLeaf);
    }

    private static List<ITreeItemNode> CreateDefaultPathDataNodes()
    {
        return
        [
            new TreeItemNode
            {
                Header  = "root",
                ItemKey = "0-0",
                Children =
                [
                    new TreeItemNode
                    {
                        Header  = "first child",
                        ItemKey = "0-0-0"
                    },
                    new TreeItemNode
                    {
                        Header  = "second child",
                        ItemKey = "0-0-1",
                        Children =
                        [
                            new TreeItemNode
                            {
                                Header  = "checked leaf",
                                ItemKey = "0-0-1-1"
                            }
                        ]
                    }
                ]
            }
        ];
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
