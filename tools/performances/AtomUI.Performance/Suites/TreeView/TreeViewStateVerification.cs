using AtomUI.Controls;
using AtomUI.Controls.Utils;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunTreeViewStateVerification()
    {
        var failures = new List<string>();
        VerifyNodeSwitcherTemplateShape(failures);
        VerifyNodeSwitcherDefaultIcons(failures);
        VerifyNodeSwitcherRotationAndLoadingIcons(failures);
        VerifyNodeSwitcherLeafVisibility(failures);
        VerifyTreeViewCheckboxParentStateAggregation(failures);
        VerifyTreeViewFilterStrategyDefaults(failures);
        VerifyTreeViewFilterHighlightRuns(failures);
        VerifyTreeViewFullTreeFilterStrategy(failures);
        VerifyInlineTreeViewFullTreeFilterStrategy(failures);
        VerifyTreeViewItemClearFilterWithoutOwner(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("TreeView state verification passed.");
            return true;
        }

        Console.Error.WriteLine("TreeView state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyNodeSwitcherTemplateShape(ICollection<string> failures)
    {
        var switcher = new NodeSwitcherButton();
        using var realized = RealizeControl(switcher);
        RefreshLayout(realized.Window);

        Expect(FindVisualByName<IconPresenter>(switcher, "CurrentIconPresenter") is not null,
            "NodeSwitcherButton should expose CurrentIconPresenter in its template.",
            failures);
        Expect(CountVisualsByTypeName(switcher, "IconPresenter") == 1,
            $"NodeSwitcherButton should create one IconPresenter (got {CountVisualsByTypeName(switcher, "IconPresenter")}).",
            failures);
    }

    private static void VerifyNodeSwitcherDefaultIcons(ICollection<string> failures)
    {
        var switcher = new NodeSwitcherButton();
        using var realized = RealizeControl(switcher);
        RefreshLayout(realized.Window);

        Expect(switcher.CurrentIcon is PlusSquareOutlined,
            $"Default unchecked switcher should use PlusSquareOutlined (got {switcher.CurrentIcon?.GetType().Name ?? "null"}).",
            failures);
        Expect(switcher.IsCurrentIconVisible,
            "Default unchecked switcher icon should be visible.",
            failures);

        switcher.SetCurrentValue(ToggleButton.IsCheckedProperty, true);
        RefreshLayout(realized.Window);
        Expect(switcher.CurrentIcon is MinusSquareOutlined,
            $"Default checked switcher should use MinusSquareOutlined (got {switcher.CurrentIcon?.GetType().Name ?? "null"}).",
            failures);

        var customExpand = new SearchOutlined();
        switcher.SetCurrentValue(NodeSwitcherButton.ExpandIconProperty, customExpand);
        switcher.SetCurrentValue(ToggleButton.IsCheckedProperty, false);
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(switcher.CurrentIcon, customExpand),
            "Default unchecked switcher should use custom ExpandIcon.",
            failures);
    }

    private static void VerifyNodeSwitcherRotationAndLoadingIcons(ICollection<string> failures)
    {
        var switcher = new NodeSwitcherButton
        {
            IconMode = NodeSwitcherButtonIconMode.Rotation
        };
        using var realized = RealizeControl(switcher);
        RefreshLayout(realized.Window);

        Expect(switcher.CurrentIcon is CaretRightOutlined,
            $"Rotation switcher should use CaretRightOutlined (got {switcher.CurrentIcon?.GetType().Name ?? "null"}).",
            failures);
        switcher.SetCurrentValue(ToggleButton.IsCheckedProperty, true);
        RefreshLayout(realized.Window);
        Expect(switcher.RotationIconRenderTransform is not null,
            "Checked rotation switcher should receive RotationIconRenderTransform from theme.",
            failures);

        var customRotation = new FolderOutlined();
        switcher.SetCurrentValue(NodeSwitcherButton.RotationIconProperty, customRotation);
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(switcher.CurrentIcon, customRotation),
            "Rotation switcher should use custom RotationIcon.",
            failures);

        switcher.IconMode = NodeSwitcherButtonIconMode.Loading;
        RefreshLayout(realized.Window);
        Expect(switcher.CurrentIcon is LoadingOutlined,
            $"Loading switcher should use LoadingOutlined (got {switcher.CurrentIcon?.GetType().Name ?? "null"}).",
            failures);
    }

    private static void VerifyNodeSwitcherLeafVisibility(ICollection<string> failures)
    {
        var switcher = new NodeSwitcherButton
        {
            IconMode = NodeSwitcherButtonIconMode.Leaf
        };
        using var realized = RealizeControl(switcher);
        RefreshLayout(realized.Window);

        Expect(switcher.CurrentIcon is FileOutlined,
            $"Leaf switcher should use FileOutlined when visible (got {switcher.CurrentIcon?.GetType().Name ?? "null"}).",
            failures);
        Expect(switcher.IsCurrentIconVisible,
            "Leaf switcher icon should be visible by default.",
            failures);

        switcher.SetCurrentValue(NodeSwitcherButton.IsLeafIconVisibleProperty, false);
        RefreshLayout(realized.Window);
        Expect(switcher.CurrentIcon is null,
            "Leaf switcher should clear CurrentIcon when IsLeafIconVisible is false.",
            failures);
        Expect(!switcher.IsCurrentIconVisible,
            "Leaf switcher presenter should be hidden when IsLeafIconVisible is false.",
            failures);

        var presenter = FindVisualByName<IconPresenter>(switcher, "CurrentIconPresenter");
        Expect(presenter?.IsVisible == false,
            "CurrentIconPresenter should be hidden when the leaf icon is not visible.",
            failures);

        switcher.SetCurrentValue(NodeSwitcherButton.IsLeafIconVisibleProperty, true);
        RefreshLayout(realized.Window);
        Expect(switcher.CurrentIcon is FileOutlined,
            "Leaf switcher should restore LeafIcon when IsLeafIconVisible returns true.",
            failures);
    }

    private static void VerifyTreeViewCheckboxParentStateAggregation(ICollection<string> failures)
    {
        var firstChild = new TreeItemNode
        {
            Header = "First child"
        };
        var secondChild = new TreeItemNode
        {
            Header = "Second child"
        };
        var parent = new TreeItemNode
        {
            Header     = "Parent",
            IsExpanded = true,
            Children   = { firstChild, secondChild }
        };
        var tree = new TreeView
        {
            ToggleType       = ItemToggleType.CheckBox,
            IsMotionEnabled  = false,
            Width            = 240,
            Height           = 160
        };
        tree.Items.Add(parent);

        using var realized = RealizeControl(tree);
        RefreshLayout(realized.Window);

        var parentContainer = tree.TreeContainerFromItem(parent) as TreeViewItem;
        if (parentContainer != null)
        {
            parentContainer.SetCurrentValue(TreeViewItem.IsExpandedProperty, true);
            RefreshLayout(realized.Window);
        }

        var firstContainer  = tree.TreeContainerFromItem(firstChild) as TreeViewItem;
        var secondContainer = tree.TreeContainerFromItem(secondChild) as TreeViewItem;

        Expect(parentContainer != null,
            "TreeView checkbox verification should realize the parent TreeViewItem.",
            failures);
        Expect(firstContainer != null && secondContainer != null,
            "TreeView checkbox verification should realize both child TreeViewItems.",
            failures);
        if (parentContainer == null || firstContainer == null || secondContainer == null)
        {
            return;
        }

        firstContainer.SetCurrentValue(TreeViewItem.IsCheckedProperty, true);
        RefreshLayout(realized.Window);
        Expect(parentContainer.IsChecked == null,
            $"Parent TreeViewItem should become indeterminate after one child is checked. Actual: {parentContainer.IsChecked}.",
            failures);
        Expect(tree.CheckedItems.Contains(firstChild) && !tree.CheckedItems.Contains(parent),
            "TreeView checked items should include the checked child but not the indeterminate parent.",
            failures);

        secondContainer.SetCurrentValue(TreeViewItem.IsCheckedProperty, true);
        RefreshLayout(realized.Window);
        Expect(parentContainer.IsChecked == true,
            $"Parent TreeViewItem should become checked after all children are checked. Actual: {parentContainer.IsChecked}.",
            failures);
        Expect(tree.CheckedItems.Contains(parent) &&
               tree.CheckedItems.Contains(firstChild) &&
               tree.CheckedItems.Contains(secondChild),
            "TreeView checked items should include parent and both children when all children are checked.",
            failures);

        firstContainer.SetCurrentValue(TreeViewItem.IsCheckedProperty, false);
        RefreshLayout(realized.Window);
        Expect(parentContainer.IsChecked == null,
            $"Parent TreeViewItem should return to indeterminate after one checked child is cleared. Actual: {parentContainer.IsChecked}.",
            failures);
        Expect(!tree.CheckedItems.Contains(parent) &&
               !tree.CheckedItems.Contains(firstChild) &&
               tree.CheckedItems.Contains(secondChild),
            "TreeView checked items should remove the parent and cleared child while keeping the remaining checked child.",
            failures);
    }

    private static void VerifyTreeViewFilterHighlightRuns(ICollection<string> failures)
    {
        var highlightBrush = Brushes.Orange;
        var header = new TreeViewItemHeader
        {
            Content                   = "alpha beta alpha",
            FilterHighlightForeground = highlightBrush,
            FilterStrategy            = TreeFilterStrategy.HighlightedMatch |
                                        TreeFilterStrategy.BoldedMatch,
            IsFilterMatch             = true,
            FilterHighlightWords      = "alpha"
        };

        var runs = header.FilterHighlightRuns?.OfType<Run>().ToArray();
        Expect(runs?.Length == 3,
            $"TreeView filter highlighted-match should emit segment-level runs (got {runs?.Length ?? 0}).",
            failures);
        if (runs is { Length: 3 })
        {
            Expect(runs[0].Text == "alpha" && runs[1].Text == " beta " && runs[2].Text == "alpha",
                $"TreeView filter highlighted-match run text should preserve source segments. Actual: '{runs[0].Text}'/'{runs[1].Text}'/'{runs[2].Text}'.",
                failures);
            Expect(ReferenceEquals(runs[0].Foreground, highlightBrush) &&
                   !runs[1].IsSet(TextElement.ForegroundProperty) &&
                   ReferenceEquals(runs[2].Foreground, highlightBrush),
                "TreeView filter highlighted-match should set foreground only on matched segments.",
                failures);
            Expect(runs.All(run => run.FontWeight == FontWeight.Bold),
                "TreeView filter BoldedMatch should preserve existing all-run bold behavior.",
                failures);
        }

        header.FilterStrategy = TreeFilterStrategy.HighlightedWhole |
                                TreeFilterStrategy.BoldedMatch;
        header.FilterHighlightWords = "missing";
        runs = header.FilterHighlightRuns?.OfType<Run>().ToArray();
        Expect(runs?.Length == 1 && runs[0].Text == "alpha beta alpha",
            $"TreeView filter highlighted-whole should emit one whole-text run (got {runs?.Length ?? 0}).",
            failures);
        if (runs is { Length: 1 })
        {
            Expect(ReferenceEquals(runs[0].Foreground, highlightBrush) &&
                   runs[0].FontWeight == FontWeight.Bold,
                "TreeView filter highlighted-whole should preserve foreground and bold style.",
                failures);
        }

        var replacementBrush = Brushes.Red;
        header.FilterStrategy            = TreeFilterStrategy.HighlightedMatch;
        header.FilterHighlightWords      = "alpha";
        header.FilterHighlightForeground = replacementBrush;
        runs = header.FilterHighlightRuns?.OfType<Run>().ToArray();
        Expect(runs is { Length: 3 } &&
               ReferenceEquals(runs[0].Foreground, replacementBrush) &&
               ReferenceEquals(runs[2].Foreground, replacementBrush),
            "TreeView filter highlight foreground changes should rebuild existing highlight runs.",
            failures);

        header.FilterHighlightWords = string.Empty;
        Expect(header.FilterHighlightRuns is null,
            "TreeView filter empty highlight words should clear highlight runs.",
            failures);
    }

    private static void VerifyTreeViewFilterStrategyDefaults(ICollection<string> failures)
    {
        Expect(new TreeView().FilterStrategy == TreeFilterStrategy.All,
            "TreeView FilterStrategy should preserve the old TreeFilterHighlightStrategy.All default after the rename.",
            failures);
        Expect(new TreeViewItem().FilterStrategy == TreeFilterStrategy.All,
            "TreeViewItem FilterStrategy should preserve the old TreeFilterHighlightStrategy.All default after the rename.",
            failures);
        Expect(new TreeViewItemHeader().FilterStrategy == TreeFilterStrategy.All,
            "TreeViewItemHeader FilterStrategy should preserve the old TreeFilterHighlightStrategy.All default after the rename.",
            failures);
    }

    private static void VerifyTreeViewFullTreeFilterStrategy(ICollection<string> failures)
    {
        var match = new TreeItemNode
        {
            Header = "target child"
        };
        var sibling = new TreeItemNode
        {
            Header = "other child"
        };
        var parent = new TreeItemNode
        {
            Header     = "parent",
            IsExpanded = true,
            Children   = { match, sibling }
        };
        var tree = new TreeView
        {
            Filter         = ValueFilterFactory.BuildFilter(ValueFilterMode.Contains),
            FilterValue    = "target",
            FilterStrategy = TreeFilterStrategy.FullTree,
            Width          = 240,
            Height         = 160
        };
        tree.Items.Add(parent);

        using var realized = RealizeControl(tree);
        RefreshLayout(realized.Window);
        tree.FilterTreeNode();
        RefreshLayout(realized.Window);

        var matchContainer   = tree.TreeContainerFromItem(match) as TreeViewItem;
        var siblingContainer = tree.TreeContainerFromItem(sibling) as TreeViewItem;

        Expect(matchContainer?.IsVisible == true,
            "TreeView FullTree filter strategy should keep matched nodes visible.",
            failures);
        Expect(siblingContainer?.IsVisible == true,
            "TreeView FullTree filter strategy should keep unmatched nodes visible.",
            failures);
        Expect(matchContainer?.IsFilterMatch == true &&
               matchContainer.FilterHighlightWords == "target",
            $"TreeView FullTree filter strategy should still mark and highlight matched nodes. Actual match: {matchContainer?.IsFilterMatch}, words: {matchContainer?.FilterHighlightWords}.",
            failures);
        Expect(tree.FilterResultCount == 1,
            $"TreeView FullTree filter strategy should preserve match counting. Actual: {tree.FilterResultCount}.",
            failures);
    }

    private static void VerifyInlineTreeViewFullTreeFilterStrategy(ICollection<string> failures)
    {
        var match = new TreeViewItem
        {
            Header = "0-0-1"
        };
        var sibling = new TreeViewItem
        {
            Header = "0-0-2"
        };
        var parent = new TreeViewItem
        {
            Header = "0-0"
        };
        parent.Items.Add(match);
        parent.Items.Add(sibling);

        var tree = new TreeView
        {
            FilterStrategy = TreeFilterStrategy.FullTree,
            Width          = 240,
            Height         = 160
        };
        tree.Items.Add(parent);

        using var realized = RealizeControl(tree);
        RefreshLayout(realized.Window);
        tree.FilterValue = "0-0-1";
        RefreshLayout(realized.Window);

        Expect(match.IsVisible && match.IsFilterMatch,
            $"Inline TreeView filtering should search direct TreeViewItem content after the strategy rename. Visible: {match.IsVisible}, match: {match.IsFilterMatch}.",
            failures);
        Expect(sibling.IsVisible,
            $"Inline TreeView FullTree filtering should keep unmatched TreeViewItem content visible. Actual: {sibling.IsVisible}.",
            failures);
        Expect(tree.FilterResultCount == 1,
            $"Inline TreeView filtering should count the matched TreeViewItem. Actual: {tree.FilterResultCount}.",
            failures);
    }

    private static void VerifyTreeViewItemClearFilterWithoutOwner(ICollection<string> failures)
    {
        var item = new TreeViewItem
        {
            IsExpanded = true
        };
        item.IsFilterMode         = true;
        item.IsFilterMatch        = true;
        item.FilterHighlightWords = "alpha";

        try
        {
            item.ClearFilterMode();
        }
        catch (Exception ex)
        {
            failures.Add($"TreeViewItem.ClearFilterMode should tolerate missing OwnerTreeView. Actual: {ex.GetType().Name}: {ex.Message}");
            return;
        }

        Expect(!item.IsExpanded,
            "TreeViewItem without OwnerTreeView should collapse when clearing filter mode.",
            failures);
        Expect(!item.IsFilterMode && !item.IsFilterMatch && item.FilterHighlightWords == null,
            "TreeViewItem.ClearFilterMode should clear filter state.",
            failures);
    }
}
