using AtomUI.Controls;
using AtomUI.Controls.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Desktop.Controls.Primitives;
using Avalonia.Controls;
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
