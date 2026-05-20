using AtomUI.Controls.Primitives;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;
using AtomScrollBar = AtomUI.Desktop.Controls.ScrollBar;
using AtomScrollBarThumb = AtomUI.Desktop.Controls.ScrollBarThumb;
using AtomScrollViewer = AtomUI.Desktop.Controls.ScrollViewer;
using AvaScrollBar = Avalonia.Controls.Primitives.ScrollBar;
using AvaScrollViewer = Avalonia.Controls.ScrollViewer;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunScrollViewerStateVerification()
    {
        var failures = new List<string>();
        VerifyScrollViewerTemplateParts(failures);
        VerifyScrollViewerAutoHideSpansContentHost(failures);
        VerifyAvaScrollViewerAutoHideSpansContentHost(failures);
        VerifyScrollViewerLiteModeState(failures);
        VerifyScrollViewerOverlayLayerLifecycle(failures);
        VerifyScrollBarMotionBinding(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("ScrollViewer state verification passed.");
            return true;
        }

        Console.Error.WriteLine("ScrollViewer state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyScrollViewerTemplateParts(ICollection<string> failures)
    {
        var scrollViewer = CreateAtomScrollViewer(
            width: 260,
            height: 120,
            content: CreateContentBlock(640, 900),
            horizontalVisibility: ScrollBarVisibility.Auto,
            verticalVisibility: ScrollBarVisibility.Auto);
        using var _ = RealizeControl(scrollViewer);

        Expect(FindVisualByName<ScrollContentPresenter>(scrollViewer, "PART_ContentPresenter") != null,
            "Atom ScrollViewer should expose PART_ContentPresenter.",
            failures);
        Expect(FindScrollViewerVisual<ScopeAwareOverlayLayerPanel>(scrollViewer) != null,
            "Atom ScrollViewer should expose a scoped overlay host.",
            failures);
        Expect(FindVisualByName<AtomScrollBar>(scrollViewer, "PART_HorizontalScrollBar") != null,
            "Atom ScrollViewer should expose PART_HorizontalScrollBar.",
            failures);
        Expect(FindVisualByName<AtomScrollBar>(scrollViewer, "PART_VerticalScrollBar") != null,
            "Atom ScrollViewer should expose PART_VerticalScrollBar.",
            failures);
        Expect(FindVisualByName<AtomScrollBarThumb>(scrollViewer, "PART_Thumb") != null,
            "Atom ScrollViewer scrollbars should expose PART_Thumb.",
            failures);
    }

    private static void VerifyScrollViewerAutoHideSpansContentHost(ICollection<string> failures)
    {
        var normal = CreateAtomScrollViewer(
            width: 260,
            height: 120,
            content: CreateContentBlock(640, 900),
            horizontalVisibility: ScrollBarVisibility.Auto,
            verticalVisibility: ScrollBarVisibility.Auto,
            isLiteMode: false,
            allowAutoHide: false);
        using var normalRealized = RealizeControl(normal);

        var normalHost = FindScrollViewerVisual<ScopeAwareOverlayLayerPanel>(normal);
        Expect(normalHost != null &&
               Grid.GetColumnSpan(normalHost) == 1 &&
               Grid.GetRowSpan(normalHost) == 1,
            "Non-lite ScrollViewer without auto-hide should keep the content host in the content grid cell.",
            failures);

        normal.SetCurrentValue(AtomScrollViewer.AllowAutoHideProperty, true);
        RefreshLayout(normalRealized.Window);
        normalHost = FindScrollViewerVisual<ScopeAwareOverlayLayerPanel>(normal);
        Expect(normalHost != null &&
               Grid.GetColumnSpan(normalHost) == 2 &&
               Grid.GetRowSpan(normalHost) == 2,
            "AllowAutoHide=True should span the content host under overlay scrollbars.",
            failures);

        normal.SetCurrentValue(AtomScrollViewer.AllowAutoHideProperty, false);
        normal.SetCurrentValue(AtomScrollViewer.IsLiteModeProperty, true);
        RefreshLayout(normalRealized.Window);
        normalHost = FindScrollViewerVisual<ScopeAwareOverlayLayerPanel>(normal);
        Expect(normalHost != null &&
               Grid.GetColumnSpan(normalHost) == 2 &&
               Grid.GetRowSpan(normalHost) == 2,
            "IsLiteMode=True should span the content host even when AllowAutoHide=False.",
            failures);
    }

    private static void VerifyAvaScrollViewerAutoHideSpansContentHost(ICollection<string> failures)
    {
        var scrollViewer = new AvaScrollViewer
        {
            Width                         = 260,
            Height                        = 120,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility   = ScrollBarVisibility.Auto,
            AllowAutoHide                 = true,
            Content                       = CreateContentBlock(640, 900)
        };
        using var _ = RealizeControl(scrollViewer);

        var host = FindScrollViewerVisual<ScopeAwareOverlayLayerPanel>(scrollViewer);
        Expect(host != null &&
               Grid.GetColumnSpan(host) == 2 &&
               Grid.GetRowSpan(host) == 2,
            "Ava ScrollViewer AllowAutoHide=True should target the actual scoped content host.",
            failures);
        Expect(FindVisualByName<AvaScrollBar>(scrollViewer, "PART_VerticalScrollBar") != null,
            "Ava ScrollViewer should keep the expected Avalonia scrollbar template part.",
            failures);
    }

    private static void VerifyScrollViewerLiteModeState(ICollection<string> failures)
    {
        var normal = CreateAtomScrollViewer(
            width: 260,
            height: 120,
            content: CreateContentBlock(640, 900),
            horizontalVisibility: ScrollBarVisibility.Auto,
            verticalVisibility: ScrollBarVisibility.Auto,
            isLiteMode: false,
            allowAutoHide: true);
        using (RealizeControl(normal))
        {
            var normalBar = FindVisualByName<AtomScrollBar>(normal, "PART_VerticalScrollBar");
            Expect(normalBar != null && MathUtils.AreClose(normalBar.Opacity, 1d, 0.001),
                "Non-lite ScrollViewer should keep visible scrollbars.",
                failures);
        }

        var lite = CreateAtomScrollViewer(
            width: 260,
            height: 120,
            content: CreateContentBlock(640, 900),
            horizontalVisibility: ScrollBarVisibility.Auto,
            verticalVisibility: ScrollBarVisibility.Auto,
            isLiteMode: true,
            allowAutoHide: true);
        using (RealizeControl(lite))
        {
            var liteBar = FindVisualByName<AtomScrollBar>(lite, "PART_VerticalScrollBar");
            Expect(liteBar != null && MathUtils.AreClose(liteBar.Opacity, 0d, 0.001),
                "Lite ScrollViewer should start with hidden overlay scrollbars.",
                failures);
        }
    }

    private static void VerifyScrollViewerOverlayLayerLifecycle(ICollection<string> failures)
    {
        var withoutRequester = CreateAtomScrollViewer(
            width: 260,
            height: 120,
            content: CreateContentBlock(220, 900),
            verticalVisibility: ScrollBarVisibility.Auto);
        using (RealizeControl(withoutRequester))
        {
            Expect(CountScrollViewerVisualByTypeName(withoutRequester, nameof(ScopeAwareOverlayLayerPanel)) == 1,
                "ScrollViewer should keep one scoped overlay host panel.",
                failures);
            Expect(CountScrollViewerVisualByTypeName(withoutRequester, nameof(ScopeAwareOverlayLayer)) == 0,
                "ScrollViewer should not create ScopeAwareOverlayLayer until a child requests it.",
                failures);
        }

        var withRequester = CreateAtomScrollViewer(
            width: 260,
            height: 120,
            content: CreateOverlayRequesterContent(),
            verticalVisibility: ScrollBarVisibility.Auto);
        using (RealizeControl(withRequester))
        {
            Expect(CountScrollViewerVisualByTypeName(withRequester, nameof(ScopeAwareOverlayLayer)) == 1,
                "ScrollViewer should create exactly one ScopeAwareOverlayLayer when scoped overlay is requested.",
                failures);
        }
    }

    private static void VerifyScrollBarMotionBinding(ICollection<string> failures)
    {
        var scrollViewer = CreateAtomScrollViewer(
            width: 260,
            height: 120,
            content: CreateContentBlock(640, 900),
            horizontalVisibility: ScrollBarVisibility.Auto,
            verticalVisibility: ScrollBarVisibility.Auto);
        scrollViewer.IsMotionEnabled = false;
        using var realized = RealizeControl(scrollViewer);

        var scrollBar = FindVisualByName<AtomScrollBar>(scrollViewer, "PART_VerticalScrollBar");
        var thumb = FindVisualByName<AtomScrollBarThumb>(scrollViewer, "PART_Thumb");
        Expect(scrollBar is { IsMotionEnabled: false } && thumb is { IsMotionEnabled: false },
            "ScrollViewer IsMotionEnabled=false should flow to ScrollBar and ScrollBarThumb.",
            failures);

        scrollViewer.IsMotionEnabled = true;
        RefreshLayout(realized.Window);
        scrollBar = FindVisualByName<AtomScrollBar>(scrollViewer, "PART_VerticalScrollBar");
        thumb = FindVisualByName<AtomScrollBarThumb>(scrollViewer, "PART_Thumb");
        Expect(scrollBar is { IsMotionEnabled: true } && thumb is { IsMotionEnabled: true },
            "ScrollViewer IsMotionEnabled=true should flow to ScrollBar and ScrollBarThumb.",
            failures);
    }

    private static int CountScrollViewerVisualByTypeName(Control root, string typeName)
    {
        return root.GetSelfAndVisualDescendants()
                   .Count(visual => visual.GetType().Name == typeName);
    }

    private static T? FindScrollViewerVisual<T>(Control root)
        where T : Control
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<T>()
                   .FirstOrDefault();
    }
}
