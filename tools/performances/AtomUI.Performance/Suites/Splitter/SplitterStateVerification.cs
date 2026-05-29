using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
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
        VerifySplitterNonCollapsibleHandleKeepsCollapseButtonsInactive(failures);
        VerifySplitterCollapsibleButtonsLifecycle(failures);
        VerifySplitterLazyPreviewReusesTransform(failures);
        VerifySplitterChildrenCollectionChangesSyncPanel(failures);
        VerifySplitterResizeEventsReportPanelSizes(failures);

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

    private static void VerifySplitterNonCollapsibleHandleKeepsCollapseButtonsInactive(ICollection<string> failures)
    {
        var splitter = CreateBasicSplitter(Orientation.Vertical);

        using var _ = RealizeControl(splitter);
        Expect(CountSplitterHandles(splitter) == 1,
            $"Basic Splitter should create one handle, actual {CountSplitterHandles(splitter)}.",
            failures);
        ExpectCollapseButtonsInactive(splitter,
            "Non-collapsible Splitter collapse IconButtons should stay hidden without icons.",
            failures);
    }

    private static void VerifySplitterCollapsibleButtonsLifecycle(ICollection<string> failures)
    {
        var splitter = CreateCollapsibleSplitter(SplitterCollapsibleIconDisplayMode.Always);

        using var realized = RealizeControl(splitter);
        var initialButtons = splitter.GetSelfAndVisualDescendants().OfType<IconButton>().ToList();
        Expect(initialButtons.Count > 0,
            "Collapsible Splitter should keep template collapse IconButtons.",
            failures);
        Expect(initialButtons.All(button => button.IsVisible),
            "Always-visible collapse IconButtons should be visible.",
            failures);
        Expect(initialButtons.All(button => button.Icon != null),
            "Visible collapse IconButtons should have an icon.",
            failures);

        foreach (var child in splitter.Children.OfType<Control>())
        {
            AtomSplitter.SetCollapsible(child, null);
        }
        RefreshLayout(realized.Window);

        ExpectCollapseButtonsInactive(splitter,
            "Splitter should hide collapse IconButtons and clear icons after collapsible is cleared.",
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

        ExpectCollapseButtonsInactive(splitter,
            "Hidden collapsible icons should leave template IconButtons hidden without icons.",
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

    private static void VerifySplitterChildrenCollectionChangesSyncPanel(ICollection<string> failures)
    {
        var splitter = CreateBasicSplitter(Orientation.Vertical);

        using var realized = RealizeControl(splitter);
        var panel = FindVisualByName<SplitterPanel>(splitter, "PART_SplitterPanel");
        Expect(panel != null,
            "Splitter collection sync should create PART_SplitterPanel.",
            failures);
        if (panel == null)
        {
            return;
        }

        Expect(CountSplitterPanelContentChildren(panel) == splitter.Children.Count,
            $"Splitter panel should start with the same content child count as Splitter.Children. Actual panel/splitter: {CountSplitterPanelContentChildren(panel)}/{splitter.Children.Count}.",
            failures);

        var inserted = CreateSplitterPanel("Inserted");
        splitter.Children.Insert(1, inserted);
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(panel.Children[1], inserted),
            "Splitter panel should insert new children at the requested index.",
            failures);

        var replacement = CreateSplitterPanel("Replacement");
        splitter.Children[1] = replacement;
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(panel.Children[1], replacement) && !panel.Children.Contains(inserted),
            "Splitter panel should replace children without retaining the old child.",
            failures);

        splitter.Children.Remove(replacement);
        RefreshLayout(realized.Window);
        Expect(!panel.Children.Contains(replacement) && CountSplitterPanelContentChildren(panel) == splitter.Children.Count,
            $"Splitter panel should remove deleted children and keep content counts aligned. Actual panel/splitter: {CountSplitterPanelContentChildren(panel)}/{splitter.Children.Count}.",
            failures);
    }

    private static void VerifySplitterResizeEventsReportPanelSizes(ICollection<string> failures)
    {
        var splitter = CreateBasicSplitter(Orientation.Vertical);
        SplitterResizeEventArgs? started   = null;
        SplitterResizeEventArgs? delta     = null;
        SplitterResizeEventArgs? completed = null;
        splitter.ResizeStarted += (_, e) => started = e;
        splitter.ResizeDelta += (_, e) => delta = e;
        splitter.ResizeCompleted += (_, e) => completed = e;

        using var realized = RealizeControl(splitter);
        RefreshLayout(realized.Window);
        var panel  = FindVisualByName<SplitterPanel>(splitter, "PART_SplitterPanel");
        var handle = splitter.GetSelfAndVisualDescendants().OfType<SplitterHandle>().FirstOrDefault();
        Expect(panel != null, "Splitter resize event verification should create PART_SplitterPanel.", failures);
        Expect(handle != null, "Splitter resize event verification should create a SplitterHandle.", failures);
        if (panel == null || handle == null)
        {
            return;
        }

        InvokePrivateMethod(panel, "AtomUI.Desktop.Controls.SplitterPanel", "HandleDragStarted",
            handle, new VectorEventArgs { Vector = new Vector(0, 0) });
        InvokePrivateMethod(panel, "AtomUI.Desktop.Controls.SplitterPanel", "HandleDragDelta",
            handle, new VectorEventArgs { Vector = new Vector(12, 0) });
        InvokePrivateMethod(panel, "AtomUI.Desktop.Controls.SplitterPanel", "HandleDragCompleted",
            handle, new VectorEventArgs { Vector = new Vector(12, 0) });

        Expect(started?.HandleIndex == 0,
            $"Splitter ResizeStarted should report handle index 0, actual {started?.HandleIndex}.",
            failures);
        Expect(started?.Sizes.Count == splitter.Children.Count,
            $"Splitter ResizeStarted should report one size per content child, actual {started?.Sizes.Count}/{splitter.Children.Count}.",
            failures);
        Expect(delta?.Sizes.Count == splitter.Children.Count,
            $"Splitter ResizeDelta should report one size per content child, actual {delta?.Sizes.Count}/{splitter.Children.Count}.",
            failures);
        Expect(completed?.Sizes.Count == splitter.Children.Count,
            $"Splitter ResizeCompleted should report one size per content child, actual {completed?.Sizes.Count}/{splitter.Children.Count}.",
            failures);

        if (started?.Sizes.Count >= 2 && delta?.Sizes.Count >= 2 && completed?.Sizes.Count >= 2)
        {
            Expect(delta.Sizes[0] > started.Sizes[0] && delta.Sizes[1] < started.Sizes[1],
                $"Splitter ResizeDelta should apply drag delta to adjacent sizes. Started={DescribeSizes(started.Sizes)}, delta={DescribeSizes(delta.Sizes)}.",
                failures);
            Expect(completed.Sizes[0] > started.Sizes[0] && completed.Sizes[1] < started.Sizes[1],
                $"Splitter ResizeCompleted should keep final adjacent sizes. Started={DescribeSizes(started.Sizes)}, completed={DescribeSizes(completed.Sizes)}.",
                failures);
        }
    }

    private static int CountSplitterHandles(Control root)
    {
        return root.GetSelfAndVisualDescendants().OfType<SplitterHandle>().Count();
    }

    private static int CountSplitterPanelContentChildren(SplitterPanel panel)
    {
        var count = 0;
        foreach (var child in panel.Children)
        {
            if (child is not SplitterHandle)
            {
                count++;
            }
        }

        return count;
    }

    private static void ExpectCollapseButtonsInactive(Control root, string message, ICollection<string> failures)
    {
        var buttons = root.GetSelfAndVisualDescendants().OfType<IconButton>().ToList();
        Expect(buttons.Count > 0,
            $"{message} Actual template IconButton count: {buttons.Count}.",
            failures);
        Expect(buttons.All(button => !button.IsVisible && button.Icon == null),
            $"{message} Actual visible/icon state: {DescribeCollapseButtons(buttons)}.",
            failures);
    }

    private static string DescribeCollapseButtons(IReadOnlyList<IconButton> buttons)
    {
        return string.Join(", ",
            buttons.Select(button => $"{button.Name ?? "<unnamed>"} visible={button.IsVisible} icon={button.Icon != null}"));
    }

    private static string DescribeSizes(IReadOnlyList<double> sizes)
    {
        return string.Join(", ", sizes.Select(size => size.ToString("0.###")));
    }
}
