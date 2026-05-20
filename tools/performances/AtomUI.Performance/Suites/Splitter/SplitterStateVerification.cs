using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using AtomSplitter = AtomUI.Desktop.Controls.Splitter;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunSplitterStateVerification()
    {
        var failures = new List<string>();
        VerifySplitterNonCollapsibleHandleDoesNotCreateCollapseButtons(failures);
        VerifySplitterCollapsibleButtonsLifecycle(failures);
        VerifySplitterLazyPreviewReusesTransform(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Splitter state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Splitter state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifySplitterNonCollapsibleHandleDoesNotCreateCollapseButtons(ICollection<string> failures)
    {
        var splitter = CreateBasicSplitter(Orientation.Vertical);

        using var _ = RealizeControl(splitter);
        Expect(CountSplitterHandles(splitter) == 1,
            $"Basic Splitter should create one handle, actual {CountSplitterHandles(splitter)}.",
            failures);
        Expect(CountIconButtons(splitter) == 0,
            $"Non-collapsible Splitter should not create collapse IconButtons, actual {CountIconButtons(splitter)}.",
            failures);
    }

    private static void VerifySplitterCollapsibleButtonsLifecycle(ICollection<string> failures)
    {
        var splitter = CreateCollapsibleSplitter(SplitterCollapsibleIconDisplayMode.Always);

        using var realized = RealizeControl(splitter);
        var initialButtons = splitter.GetSelfAndVisualDescendants().OfType<IconButton>().ToList();
        Expect(initialButtons.Count > 0,
            "Collapsible Splitter should create collapse IconButtons.",
            failures);
        Expect(initialButtons.All(button => button.Icon != null),
            "Visible collapse IconButtons should have an icon.",
            failures);

        foreach (var child in splitter.Children.OfType<Control>())
        {
            AtomSplitter.SetCollapsible(child, null);
        }
        RefreshLayout(realized.Window);

        Expect(CountIconButtons(splitter) == 0,
            $"Splitter should remove collapse IconButtons after collapsible is cleared, actual {CountIconButtons(splitter)}.",
            failures);
        Expect(initialButtons.All(button => button.GetVisualParent() == null),
            "Removed collapse IconButtons should not keep a visual parent.",
            failures);

        foreach (var child in splitter.Children.OfType<Control>())
        {
            AtomSplitter.SetCollapsible(child, new SplitterPanelCollapsible
            {
                IsEnabled           = true,
                ShowCollapsibleIcon = SplitterCollapsibleIconDisplayMode.Hidden
            });
        }
        RefreshLayout(realized.Window);

        Expect(CountIconButtons(splitter) == 0,
            $"Hidden collapsible icons should not materialize IconButtons, actual {CountIconButtons(splitter)}.",
            failures);
    }

    private static void VerifySplitterLazyPreviewReusesTransform(ICollection<string> failures)
    {
        var splitter = CreateLazySplitter(Orientation.Vertical);

        using var _ = RealizeControl(splitter);
        var panel  = FindVisualByName<SplitterPanel>(splitter, "PART_SplitterPanel");
        var handle = splitter.GetSelfAndVisualDescendants().OfType<SplitterHandle>().FirstOrDefault();
        Expect(panel != null, "Lazy Splitter should create PART_SplitterPanel.", failures);
        Expect(handle != null, "Lazy Splitter should create a SplitterHandle.", failures);
        if (panel == null || handle == null)
        {
            return;
        }

        InvokePrivateMethod(panel, "AtomUI.Desktop.Controls.SplitterPanel", "ApplyHandlePreview", handle, 12d);
        var firstTransform = handle.RenderTransform as TranslateTransform;
        Expect(firstTransform != null,
            "Lazy Splitter preview should use a TranslateTransform.",
            failures);

        InvokePrivateMethod(panel, "AtomUI.Desktop.Controls.SplitterPanel", "ApplyHandlePreview", handle, 24d);
        var secondTransform = handle.RenderTransform as TranslateTransform;
        Expect(ReferenceEquals(firstTransform, secondTransform),
            "Lazy Splitter preview should reuse the TranslateTransform instance.",
            failures);
        Expect(secondTransform?.X == 24d && secondTransform.Y == 0d,
            $"Vertical lazy Splitter preview should update X only, actual {secondTransform?.X},{secondTransform?.Y}.",
            failures);

        InvokePrivateMethod(panel, "AtomUI.Desktop.Controls.SplitterPanel", "ClearHandlePreview", handle);
        Expect(handle.RenderTransform == null,
            "Lazy Splitter should clear handle preview transform after drag.",
            failures);
    }

    private static int CountSplitterHandles(Control root)
    {
        return root.GetSelfAndVisualDescendants().OfType<SplitterHandle>().Count();
    }

    private static int CountIconButtons(Control root)
    {
        return root.GetSelfAndVisualDescendants().OfType<IconButton>().Count();
    }
}
