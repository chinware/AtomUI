using AtomUI.Controls;
using AtomUI.Controls.Data;
using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls;
using AtomUI.Desktop.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
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
        VerifyCandidateListSelectionMovement(failures);
        VerifyCandidateListEmptyNavigation(failures);
        VerifyTreeNodePathDerivedOperations(failures);
        VerifyMotionGhostGeometryUpdates(failures);

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

    private static void VerifyTreeNodePathDerivedOperations(ICollection<string> failures)
    {
        var path = new TreeNodePath(["zhejiang", "hangzhou"]);
        Expect(path.StartsWith(path),
            "TreeNodePath.StartsWith() should return true for the same immutable instance.",
            failures);
        Expect(path.StartsWith(TreeNodePath.Empty),
            "TreeNodePath.StartsWith() should return true for an empty prefix.",
            failures);

        var childPath = path.Append("xihu");
        Expect(childPath.ToString() == "zhejiang/hangzhou/xihu",
            $"TreeNodePath.Append(string) should preserve path segments, actual {childPath}.",
            failures);

        var appendedPath = path.Append(new TreeNodePath(["xihu", "longjing"]));
        Expect(appendedPath.ToString() == "zhejiang/hangzhou/xihu/longjing",
            $"TreeNodePath.Append(TreeNodePath) should append all segments, actual {appendedPath}.",
            failures);
        Expect(ReferenceEquals(TreeNodePath.Empty.Append(path), path),
            "TreeNodePath.Empty.Append(TreeNodePath) should reuse the appended immutable path.",
            failures);

        var parentPath = childPath.GetParent();
        Expect(parentPath?.ToString() == "zhejiang/hangzhou",
            $"TreeNodePath.GetParent() should remove one segment, actual {parentPath}.",
            failures);
        Expect(ReferenceEquals(new TreeNodePath(["zhejiang"]).GetParent(), TreeNodePath.Empty),
            "TreeNodePath.GetParent() for a single segment should reuse TreeNodePath.Empty.",
            failures);

        var replacedPath = childPath.WithSegment(2, "yuhang");
        Expect(replacedPath.ToString() == "zhejiang/hangzhou/yuhang",
            $"TreeNodePath.WithSegment() should replace the requested segment, actual {replacedPath}.",
            failures);
        Expect(ReferenceEquals(childPath.WithSegment(2, "xihu"), childPath),
            "TreeNodePath.WithSegment() should return the same immutable path when the segment is unchanged.",
            failures);

        Expect(TreeNodePath.Empty.GetParent() == null,
            "TreeNodePath.Empty.GetParent() should stay null.",
            failures);
    }

    private static void VerifyMotionGhostGeometryUpdates(ICollection<string> failures)
    {
        var ghostType = typeof(TreeNodePath).Assembly.GetType("AtomUI.Controls.Primitives.MotionGhostControl");
        Expect(ghostType != null,
            "MotionGhostControl type should be available for geometry update verification.",
            failures);
        if (ghostType == null)
        {
            return;
        }

        var ghost = Activator.CreateInstance(ghostType, nonPublic: true) as Control;
        Expect(ghost != null,
            "MotionGhostControl should be constructible for geometry update verification.",
            failures);
        if (ghost == null)
        {
            return;
        }

        SetPropertyValue(ghostType, ghost, "Content", new Border
        {
            Width  = 24,
            Height = 18
        });
        SetPropertyValue(ghostType, ghost, "MaskShadows", new BoxShadows(new BoxShadow
        {
            Blur    = 4,
            OffsetX = 1,
            OffsetY = 2
        }));
        SetPropertyValue(ghostType, ghost, "MaskCornerRadius", new CornerRadius(3));
        SetPropertyValue(ghostType, ghost, "MaskSize", new Size(24, 18));

        using var realized = RealizeControl(ghost);
        var shadowRenderer = ghost.GetVisualDescendants().OfType<Border>().FirstOrDefault(border => border.BoxShadow.Count > 0);
        Expect(shadowRenderer != null,
            "MotionGhostControl should create a shadow renderer for geometry update verification.",
            failures);
        if (shadowRenderer == null)
        {
            return;
        }

        SetPropertyValue(ghostType, ghost, "MaskCornerRadius", new CornerRadius(6));
        RefreshLayout(realized.Window);
        Expect(shadowRenderer.CornerRadius == new CornerRadius(6) &&
               Math.Abs(shadowRenderer.Width - 24) < 0.0001 &&
               Math.Abs(shadowRenderer.Height - 18) < 0.0001,
            $"MotionGhostControl should update radius without changing size, actual radius={shadowRenderer.CornerRadius}, size={shadowRenderer.Width}x{shadowRenderer.Height}.",
            failures);

        SetPropertyValue(ghostType, ghost, "MaskSize", new Size(32, 20));
        RefreshLayout(realized.Window);
        Expect(shadowRenderer.CornerRadius == new CornerRadius(6) &&
               Math.Abs(shadowRenderer.Width - 32) < 0.0001 &&
               Math.Abs(shadowRenderer.Height - 20) < 0.0001,
            $"MotionGhostControl should update size without changing radius, actual radius={shadowRenderer.CornerRadius}, size={shadowRenderer.Width}x{shadowRenderer.Height}.",
            failures);
    }

    private static void VerifyCandidateListEmptyNavigation(ICollection<string> failures)
    {
        var candidateList = CreateCandidateList([]);
        using var realized = RealizeControl(candidateList);
        var keyDown = new Avalonia.Input.KeyEventArgs
        {
            RoutedEvent  = Avalonia.Input.InputElement.KeyDownEvent,
            Key          = Avalonia.Input.Key.Down,
            KeyModifiers = Avalonia.Input.KeyModifiers.None
        };

        candidateList.HandleKeyDown(keyDown);
        RefreshLayout(realized.Window);
        Expect(keyDown.Handled && candidateList.CandidateSelectedIndex == -1 && candidateList.CandidateSelectedItem == null,
            "Empty CandidateList Down navigation should be handled without selecting an invalid candidate.",
            failures);
    }

    private static void SetPropertyValue(Type type, object target, string propertyName, object? value)
    {
        type.GetProperty(propertyName)?.SetValue(target, value);
    }

    private static void VerifyListBoxDefaultShape(ICollection<string> failures)
    {
        var listBox = CreateListBox(CreateListItems(5));
        using var realized = RealizeControl(listBox);

        Expect(FindSelectedIndicatorPresenter(listBox) is { IsVisible: false },
            "Default ListBox should keep the static selected indicator presenter hidden.",
            failures);
        Expect(FindVisualByType<HighlightableTextBlock>(listBox) is { IsVisible: false },
            "Default ListBox should keep the static filtering HighlightableTextBlock hidden.",
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

        Expect(FindSelectedIndicatorPresenter(selectedItem) is { IsVisible: true },
            "Selected ListBox item should show selected indicator presenter.",
            failures);

        listBox.SelectedIndex = -1;
        RefreshLayout(realized.Window);
        Expect(FindSelectedIndicatorPresenter(selectedItem) is { IsVisible: false },
            "Unselected ListBox item should hide selected indicator presenter.",
            failures);

        listBox.SelectedIndex = 1;
        RefreshLayout(realized.Window);
        var nextSelectedItem = listBox.ContainerFromIndex(1) as AtomUI.Desktop.Controls.ListBoxItem;
        Expect(nextSelectedItem != null && FindSelectedIndicatorPresenter(nextSelectedItem) is { IsVisible: true },
            "Selecting another ListBox item should show selected indicator presenter again.",
            failures);
    }

    private static void VerifyListBoxFilteringLifecycle(ICollection<string> failures)
    {
        var listBox = CreateListBox(CreateListBoxDemoItems());
        using var realized = RealizeControl(listBox);

        Expect(FindVisualByType<HighlightableTextBlock>(listBox) is { IsVisible: false },
            "ListBox should keep filtering text block hidden before filtering starts.",
            failures);

        listBox.FilterValue = "car";
        RefreshLayout(realized.Window);
        Expect(FindVisualByType<HighlightableTextBlock>(listBox) is { IsVisible: true },
            "ListBox should show filtering text block while filtering.",
            failures);

        listBox.FilterValue = null;
        RefreshLayout(realized.Window);
        Expect(FindVisualByType<HighlightableTextBlock>(listBox) is { IsVisible: false },
            "ListBox should hide filtering text block after filtering clears.",
            failures);
    }

    private static void VerifyCandidateListSelectionMovement(ICollection<string> failures)
    {
        var candidateList = CreateCandidateList(CreateListItems(4));
        using var realized = RealizeControl(candidateList);

        var first  = candidateList.ContainerFromIndex(0) as CandidateListItem;
        var second = candidateList.ContainerFromIndex(1) as CandidateListItem;
        var third  = candidateList.ContainerFromIndex(2) as CandidateListItem;
        Expect(first != null && second != null && third != null,
            "CandidateList should realize candidate item containers for selection movement verification.",
            failures);
        if (first == null || second == null || third == null)
        {
            return;
        }

        candidateList.CandidateSelectedIndex = 0;
        RefreshLayout(realized.Window);
        Expect(first.IsCandidateSelected && !second.IsCandidateSelected && !third.IsCandidateSelected,
            "CandidateList should mark only the first candidate when index 0 is selected.",
            failures);

        candidateList.CandidateSelectedIndex = 1;
        RefreshLayout(realized.Window);
        Expect(!first.IsCandidateSelected && second.IsCandidateSelected && !third.IsCandidateSelected,
            "CandidateList should clear the old candidate and mark only the second candidate when selection moves.",
            failures);

        candidateList.CandidateSelectedIndex = -1;
        RefreshLayout(realized.Window);
        Expect(!first.IsCandidateSelected && !second.IsCandidateSelected && !third.IsCandidateSelected,
            "CandidateList should clear the active candidate when selected index resets.",
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
