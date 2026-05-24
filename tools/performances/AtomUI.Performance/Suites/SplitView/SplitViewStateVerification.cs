using System.Reflection;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Shapes;
using AtomSplitView = AtomUI.Desktop.Controls.SplitView;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunSplitViewStateVerification()
    {
        var failures = new List<string>();
        VerifySplitViewEventOrder(failures);
        VerifySplitViewTemplateSettings(failures);
        VerifySplitViewPlacementParts(failures);
        VerifySplitViewDeferredTransitions(failures);
        VerifySplitViewMotionDisabledSkipsTransitions(failures);
        VerifySplitViewTransitionAxisUpdates(failures);
        VerifySplitViewCloseDuration(failures);
        VerifySplitViewLeftOverlayColumnSpan(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("SplitView state verification passed.");
            return true;
        }

        Console.Error.WriteLine("SplitView state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifySplitViewEventOrder(ICollection<string> failures)
    {
        var splitView = CreateSplitView();
        var events    = new List<string>();
        splitView.PaneOpening += (_, _) => events.Add("opening");
        splitView.PaneOpened += (_, _) => events.Add("opened");
        splitView.PaneClosing += (_, _) => events.Add("closing");
        splitView.PaneClosed += (_, _) => events.Add("closed");

        using var realized = RealizeControl(splitView);
        splitView.IsPaneOpen = true;
        RefreshLayout(realized.Window);
        splitView.IsPaneOpen = false;
        RefreshLayout(realized.Window);

        var actual = string.Join(",", events);
        Expect(actual == "opening,opened,closing,closed",
            $"SplitView pane event order should be opening,opened,closing,closed, actual {actual}.",
            failures);
    }

    private static void VerifySplitViewTemplateSettings(ICollection<string> failures)
    {
        VerifySplitViewTemplateSettings(
            SplitViewDisplayMode.Overlay,
            0,
            GridUnitType.Pixel,
            0,
            failures);
        VerifySplitViewTemplateSettings(
            SplitViewDisplayMode.CompactOverlay,
            48,
            GridUnitType.Pixel,
            48,
            failures);
        VerifySplitViewTemplateSettings(
            SplitViewDisplayMode.Inline,
            0,
            GridUnitType.Auto,
            0,
            failures);
        VerifySplitViewTemplateSettings(
            SplitViewDisplayMode.CompactInline,
            48,
            GridUnitType.Auto,
            0,
            failures);
    }

    private static void VerifySplitViewTemplateSettings(
        SplitViewDisplayMode displayMode,
        double expectedClosedLength,
        GridUnitType expectedGridUnit,
        double expectedGridLength,
        ICollection<string> failures)
    {
        var splitView = CreateSplitView(displayMode);

        using var realized = RealizeControl(splitView);
        var settings = splitView.TemplateSettings;
        Expect(MathUtils.AreClose(settings.ClosedPaneWidth, expectedClosedLength),
            $"{displayMode} ClosedPaneWidth should be {expectedClosedLength}, actual {settings.ClosedPaneWidth}.",
            failures);
        Expect(MathUtils.AreClose(settings.ClosedPaneHeight, expectedClosedLength),
            $"{displayMode} ClosedPaneHeight should be {expectedClosedLength}, actual {settings.ClosedPaneHeight}.",
            failures);
        Expect(settings.PaneColumnGridLength.GridUnitType == expectedGridUnit &&
               MathUtils.AreClose(settings.PaneColumnGridLength.Value, expectedGridLength),
            $"{displayMode} PaneColumnGridLength should be {expectedGridLength} {expectedGridUnit}, actual {settings.PaneColumnGridLength}.",
            failures);
        Expect(settings.PaneRowGridLength.GridUnitType == expectedGridUnit &&
               MathUtils.AreClose(settings.PaneRowGridLength.Value, expectedGridLength),
            $"{displayMode} PaneRowGridLength should be {expectedGridLength} {expectedGridUnit}, actual {settings.PaneRowGridLength}.",
            failures);
    }

    private static void VerifySplitViewPlacementParts(ICollection<string> failures)
    {
        foreach (var placement in new[]
                 {
                     SplitViewPanePlacement.Left,
                     SplitViewPanePlacement.Right,
                     SplitViewPanePlacement.Top,
                     SplitViewPanePlacement.Bottom
                 })
        {
            var splitView = CreateSplitView(isPaneOpen: true, panePlacement: placement);

            using var _ = RealizeControl(splitView);
            Expect(FindVisualByName<Panel>(splitView, "PART_PaneRoot") != null,
                $"{placement} SplitView should create PART_PaneRoot.",
                failures);
            Expect(FindVisualByName<ContentPresenter>(splitView, "PART_PanePresenter") != null,
                $"{placement} SplitView should create PART_PanePresenter.",
                failures);
            Expect(FindVisualByName<ContentPresenter>(splitView, "PART_ContentPresenter") != null,
                $"{placement} SplitView should create PART_ContentPresenter.",
                failures);
            Expect(FindVisualByName<Rectangle>(splitView, "LightDismissLayer") is { IsVisible: true },
                $"{placement} open overlay SplitView should expose the LightDismissLayer.",
                failures);
        }
    }

    private static void VerifySplitViewMotionDisabledSkipsTransitions(ICollection<string> failures)
    {
        var splitView = CreateSplitView(isMotionEnabled: false);

        using var realized = RealizeControl(splitView);
        splitView.IsPaneOpen = true;
        RefreshLayout(realized.Window);
        var paneRoot = FindVisualByName<Panel>(splitView, "PART_PaneRoot");
        Expect(GetPaneOpenTransitions(splitView) == null,
            "Motion-disabled SplitView should not keep PaneOpenTransitions.",
            failures);
        Expect(GetPaneCloseTransitions(splitView) == null,
            "Motion-disabled SplitView should not keep PaneCloseTransitions.",
            failures);
        Expect(paneRoot?.Transitions == null,
            "Motion-disabled SplitView pane root should not receive transitions.",
            failures);
    }

    private static void VerifySplitViewDeferredTransitions(ICollection<string> failures)
    {
        var splitView = CreateSplitView();

        Expect(GetPaneOpenTransitions(splitView) == null,
            "Constructed SplitView should not materialize PaneOpenTransitions before loading.",
            failures);
        Expect(GetPaneCloseTransitions(splitView) == null,
            "Constructed SplitView should not materialize PaneCloseTransitions before loading.",
            failures);

        using var realized = RealizeControl(splitView);
        Expect(GetPaneOpenTransitions(splitView) == null,
            "Loaded closed SplitView should not materialize PaneOpenTransitions before the first open.",
            failures);
        Expect(GetPaneCloseTransitions(splitView) == null,
            "Loaded closed SplitView should not materialize PaneCloseTransitions before the first close.",
            failures);

        splitView.IsPaneOpen = true;
        RefreshLayout(realized.Window);
        Expect(GetTransitionPropertyName(GetPaneOpenTransitions(splitView)) == nameof(Control.Width),
            "Opening left SplitView should materialize a Width open transition.",
            failures);

        splitView.IsPaneOpen = false;
        RefreshLayout(realized.Window);
        Expect(GetTransitionPropertyName(GetPaneCloseTransitions(splitView)) == nameof(Control.Width),
            "Closing left SplitView should materialize a matching Width close transition.",
            failures);

        var initiallyOpenSplitView = CreateSplitView(isPaneOpen: true);
        using var initiallyOpenRealized = RealizeControl(initiallyOpenSplitView);
        Expect(GetPaneOpenTransitions(initiallyOpenSplitView) == null,
            "Loaded initially-open SplitView should not materialize PaneOpenTransitions before a runtime open.",
            failures);
        Expect(GetPaneCloseTransitions(initiallyOpenSplitView) == null,
            "Loaded initially-open SplitView should not materialize PaneCloseTransitions before the first close.",
            failures);

        initiallyOpenSplitView.IsPaneOpen = false;
        RefreshLayout(initiallyOpenRealized.Window);
        Expect(GetTransitionPropertyName(GetPaneCloseTransitions(initiallyOpenSplitView)) == nameof(Control.Width),
            "Closing initially-open left SplitView should materialize a Width close transition.",
            failures);
    }

    private static void VerifySplitViewTransitionAxisUpdates(ICollection<string> failures)
    {
        var splitView = CreateSplitView();

        using var realized = RealizeControl(splitView);
        splitView.IsPaneOpen = true;
        RefreshLayout(realized.Window);
        Expect(GetTransitionPropertyName(GetPaneOpenTransitions(splitView)) == nameof(Control.Width),
            "Left SplitView should animate Width.",
            failures);

        splitView.PanePlacement = SplitViewPanePlacement.Top;
        RefreshLayout(realized.Window);

        Expect(GetTransitionPropertyName(GetPaneOpenTransitions(splitView)) == nameof(Control.Height),
            "Top SplitView should animate Height after placement changes.",
            failures);
    }

    private static void VerifySplitViewCloseDuration(ICollection<string> failures)
    {
        var splitView = CreateSplitView(isPaneOpen: true);

        using var realized = RealizeControl(splitView);
        splitView.IsPaneOpen = false;
        RefreshLayout(realized.Window);
        var closeTransition = GetPaneCloseTransitions(splitView)?.OfType<DoubleTransition>().FirstOrDefault();
        Expect(closeTransition?.Duration == TimeSpan.FromMilliseconds(100),
            $"SplitView close transition should be 100ms, actual {closeTransition?.Duration}.",
            failures);
    }

    private static void VerifySplitViewLeftOverlayColumnSpan(ICollection<string> failures)
    {
        VerifySplitViewLeftColumnSpan(SplitViewDisplayMode.Overlay, failures);
        VerifySplitViewLeftColumnSpan(SplitViewDisplayMode.CompactOverlay, failures);
    }

    private static void VerifySplitViewLeftColumnSpan(
        SplitViewDisplayMode displayMode,
        ICollection<string> failures)
    {
        var splitView = CreateSplitView(displayMode, isPaneOpen: true);

        using var _ = RealizeControl(splitView);
        var paneRoot = FindVisualByName<Panel>(splitView, "PART_PaneRoot");
        var actualColumnSpan = paneRoot is null ? -1 : Grid.GetColumnSpan(paneRoot);
        Expect(actualColumnSpan == 2,
            $"Left {displayMode} SplitView pane should span two columns, actual {actualColumnSpan}.",
            failures);
    }

    private static Transitions? GetPaneOpenTransitions(AtomSplitView splitView)
    {
        return GetSplitViewTransitions(splitView, "PaneOpenTransitions");
    }

    private static Transitions? GetPaneCloseTransitions(AtomSplitView splitView)
    {
        return GetSplitViewTransitions(splitView, "PaneCloseTransitions");
    }

    private static Transitions? GetSplitViewTransitions(AtomSplitView splitView, string propertyName)
    {
        return typeof(AtomSplitView)
               .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic)
               ?.GetValue(splitView) as Transitions;
    }

    private static string? GetTransitionPropertyName(Transitions? transitions)
    {
        return transitions?.OfType<DoubleTransition>().FirstOrDefault()?.Property?.Name;
    }
}
