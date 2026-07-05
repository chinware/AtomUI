using System.Collections;
using AtomUI.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

public partial class TreeView
{
    private void ReplayLoadedState()
    {
        ConfigureDefaultSelectedPaths();
        ConfigureDefaultCheckedPaths();

        FilterTreeNode();

        if (IsDefaultExpandAll)
        {
            Dispatcher.Post(() => ExpandAll(false));
        }
        else
        {
            ConfigureDefaultExpandedPaths();
        }
    }

    private void ConfigureStateAfterItemsSourceChanged()
    {
        if (!IsLoaded)
        {
            return;
        }

        var selectedItemPath  = BuildNodeIdentityPath(SelectedItem as ITreeItemNode);
        var selectedItemPaths = BuildNodeIdentityPaths(SelectedItems);
        var checkedItemPaths  = BuildNodeIdentityPaths(CheckedItems);

        SetCurrentValue(SelectedItemProperty, null);
        SelectedItems.Clear();
        CheckedItems.Clear();

        var selectionRestored = false;
        if (selectedItemPath != null)
        {
            selectionRestored = TrySelectNodePath(selectedItemPath);
        }
        if (!selectionRestored && selectedItemPaths != null)
        {
            foreach (var path in selectedItemPaths)
            {
                selectionRestored |= TrySelectNodePath(path);
            }
        }
        if (!selectionRestored)
        {
            ConfigureDefaultSelectedPaths();
        }

        var checkedRestored = false;
        if (checkedItemPaths != null)
        {
            foreach (var path in checkedItemPaths)
            {
                checkedRestored |= TryCheckNodePath(path);
            }
        }
        if (!checkedRestored)
        {
            ConfigureDefaultCheckedPaths();
        }

        if (IsDefaultExpandAll)
        {
            ExpandAll(false);
        }
        else
        {
            ConfigureDefaultExpandedPaths();
        }
    }

    private static List<TreeNodePath>? BuildNodeIdentityPaths(IList? nodes)
    {
        if (nodes == null)
        {
            return null;
        }

        var paths = new List<TreeNodePath>(nodes.Count);
        foreach (var node in nodes)
        {
            if (node is ITreeItemNode treeItemNode)
            {
                var path = BuildNodeIdentityPath(treeItemNode);
                if (path != null)
                {
                    paths.Add(path);
                }
            }
        }
        return paths;
    }

    private static TreeNodePath? BuildNodeIdentityPath(ITreeItemNode? node)
    {
        if (node == null)
        {
            return null;
        }

        var depth    = CountTreeItemPathDepth(node);
        var segments = new string[depth];
        var current  = node;
        for (var i = segments.Length - 1; current != null; i--)
        {
            var segment = current.ItemKey?.ToString() ?? current.Value?.ToString();
            if (string.IsNullOrEmpty(segment))
            {
                return null;
            }

            segments[i] = segment;
            current     = current.ParentNode as ITreeItemNode;
        }

        return new TreeNodePath(segments);
    }

    private static int CountTreeItemPathDepth(ITreeItemNode itemData)
    {
        var count   = 0;
        var current = itemData;
        while (current != null)
        {
            count++;
            current = current.ParentNode as ITreeItemNode;
        }

        return count;
    }

    private bool TrySelectNodePath(TreeNodePath path)
    {
        var selected = false;
        TraverseDefaultStatePath(path, (treeViewItem, i) =>
        {
            if (i == path.Length - 1)
            {
                var item = TreeItemFromContainer(treeViewItem);
                if (item != null)
                {
                    treeViewItem.SetCurrentValue(TreeViewItem.IsSelectedProperty, true);
                    if (!SelectedItems.Contains(item))
                    {
                        SelectedItems.Add(item);
                    }
                    if (SelectionMode == SelectionMode.Single)
                    {
                        SetCurrentValue(SelectedItemProperty, item);
                    }
                    selected = true;
                }
            }
        });
        return selected;
    }

    private bool TryCheckNodePath(TreeNodePath path)
    {
        var isChecked = false;
        TraverseDefaultStatePath(path, (treeViewItem, i) =>
        {
            if (i == path.Length - 1)
            {
                treeViewItem.SetCurrentValue(TreeViewItem.IsCheckedProperty, true);
                isChecked = true;
            }
        });
        return isChecked;
    }

    private void ConfigureDefaultCheckedPaths()
    {
        if (DefaultCheckedPaths != null)
        {
            foreach (var checkedPath in DefaultCheckedPaths)
            {
                TraverseDefaultStatePath(checkedPath, (treeViewItem, i) =>
                {
                    if (i == checkedPath.Length - 1)
                    {
                        treeViewItem.SetCurrentValue(TreeViewItem.IsCheckedProperty, true);
                    }
                });
            }
        }
    }

    private void ConfigureDefaultExpandedPaths()
    {
        if (DefaultExpandedPaths != null)
        {
            foreach (var path in DefaultExpandedPaths)
            {
                TraverseTreeViewPath(
                    path,
                    TreePathTraversalOptions.DisableMotion |
                    TreePathTraversalOptions.ExpandMatchedNodes |
                    TreePathTraversalOptions.EnsureContainers);
            }
        }
    }

    private void ConfigureDefaultSelectedPaths()
    {
        if (!IsSelectable)
        {
            return;
        }

        if (SelectedItems.Count == 0 && SelectedItem == null)
        {
            if (DefaultSelectedPaths != null)
            {
                foreach (var selectedPath in DefaultSelectedPaths)
                {
                    TraverseDefaultStatePath(selectedPath, (treeViewItem, i) =>
                    {
                        if (i == selectedPath.Length - 1)
                        {
                            var item = TreeItemFromContainer(treeViewItem);
                            if (!SelectedItems.Contains(item))
                            {
                                SelectedItems.Add(item);
                            }
                        }
                    });
                }
            }
        }
        else if ((SelectionMode & SelectionMode.Multiple) == SelectionMode.Multiple &&
                 SelectedItems.Count > 0)
        {
            ConfigureSelectedItems();
        }
        else if (SelectedItem != null)
        {
            var paths = GetTreePathFromItem(SelectedItem);
            SelectTreeItemByPath(paths);
        }
    }

    private void ConfigureSelectedItems()
    {
        var selectedPaths = BuildNodeIdentityPaths(SelectedItems);
        if (selectedPaths != null)
        {
            foreach (var selectedPath in selectedPaths)
            {
                TrySelectNodePath(selectedPath);
            }
        }
    }

    private List<object> GetTreePathFromItem(object item)
    {
        List<object> paths;
        if (item is ITreeItemNode itemData)
        {
            var pathDepth = CountTreeItemPathDepth(itemData);
            paths = new List<object>(pathDepth);
            for (var i = 0; i < pathDepth; i++)
            {
                paths.Add(null!);
            }

            var current = itemData;
            for (var i = pathDepth - 1; current != null; i--)
            {
                paths[i] = current;
                current  = current.ParentNode as ITreeItemNode;
            }
        }
        else if (item is TreeViewItem treeViewItem)
        {
            var pathDepth = CountTreeViewItemPathDepth(treeViewItem);
            paths = new List<object>(pathDepth);
            for (var i = 0; i < pathDepth; i++)
            {
                paths.Add(null!);
            }

            var current = treeViewItem;
            for (var i = pathDepth - 1; current != null; i--)
            {
                paths[i] = current;
                current  = current.Parent as TreeViewItem;
            }
        }
        else
        {
            throw new ArgumentException("Invalid item type, Must ITreeItemNode or TreeItem.");
        }

        return paths;
    }

    private void SelectTreeItemByPath(IList paths)
    {
        if (paths.Count == 0)
        {
            return;
        }
        ItemsControl current             = this;
        bool         originMotionEnabled = IsMotionEnabled;
        try
        {
            SetCurrentValue(IsMotionEnabledProperty, false);
            for (var i = 0; i < paths.Count; i++)
            {
                var pathNode = paths[i];
                if (pathNode != null)
                {
                    TreeViewItem? child          = null;
                    bool?         originExpanded = null;
                    try
                    {
                        if (current is TreeViewItem item)
                        {
                            originExpanded = item.IsExpanded;
                            item.SetCurrentValue(TreeViewItem.IsExpandedProperty, true);
                        }

                        child = GetTreeViewItemContainer(pathNode, current);
                    }
                    finally
                    {
                        if (current is TreeViewItem item)
                        {
                            if (originExpanded != null)
                            {
                                item.SetCurrentValue(TreeViewItem.IsExpandedProperty, originExpanded.Value);
                            }
                        }
                    }

                    if (child != null)
                    {
                        current = child;
                    }
                }
            }

            if (current is TreeViewItem treeViewItem)
            {
                var item = TreeItemFromContainer(treeViewItem);
                if (item != null && !SelectedItems.Contains(item))
                {
                    SelectedItems.Add(item);
                }
                treeViewItem.SetCurrentValue(TreeViewItem.IsSelectedProperty, true);
            }
        }
        finally
        {
            SetCurrentValue(IsMotionEnabledProperty, originMotionEnabled);
        }
    }

    private TreeViewItem? GetTreeViewItemContainer(object childNode, ItemsControl current)
    {
        if (current.Presenter?.Panel == null)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel != null)
            {
                topLevel.GetLayoutManager()?.ExecuteLayoutPass();
            }
        }
        if (current.Presenter?.Panel is { })
        {
            return current.ContainerFromItem(childNode) as TreeViewItem;
        }
        return null;
    }

    private List<TreeViewItem>? TraverseDefaultStatePath(TreeNodePath path, Action<TreeViewItem, int>? action)
    {
        return TraverseTreeViewPath(
            path,
            TreePathTraversalOptions.DisableMotion |
            TreePathTraversalOptions.ExpandMatchedNodes |
            TreePathTraversalOptions.RestoreExpandedState |
            TreePathTraversalOptions.EnsureContainers,
            action);
    }
}
