using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls.Primitives;

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
}
