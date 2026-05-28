using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunWatermarkStateVerification()
    {
        var failures = new List<string>();
        VerifyPendingGlyphChangesInstallSingleWatermark(failures);
        VerifyArrangedGlyphReplacementDoesNotDuplicateWatermark(failures);
        VerifyClearingGlyphRemovesWatermark(failures);
        VerifyExistingAdornerIsNotReplacedByWatermark(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Watermark state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Watermark state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }

        return false;
    }

    private static void VerifyPendingGlyphChangesInstallSingleWatermark(ICollection<string> failures)
    {
        using var realized = RealizeControl(CreateWatermarkPendingReplaceHost(4));
        RefreshLayout(realized.Window);

        Expect(CountVisualByTypeName(realized.Window, "Watermark") == 1,
            "Pending glyph replacements before first layout should install exactly one Watermark.",
            failures);
    }

    private static void VerifyArrangedGlyphReplacementDoesNotDuplicateWatermark(ICollection<string> failures)
    {
        var target = CreateWatermarkTarget();
        var host = new VisualLayerManager
        {
            Child = target
        };

        using var realized = RealizeControl(host);

        var firstGlyph = CreateTextWatermarkGlyph("First");
        Watermark.SetGlyph(target, firstGlyph);
        RefreshLayout(realized.Window);
        var firstWatermark = ScopeAwareAdornerLayer.GetAdorner(target);
        Expect(firstWatermark is Watermark,
            "Setting Glyph on arranged target should install a Watermark.",
            failures);
        Expect(CountVisualByTypeName(realized.Window, "Watermark") == 1,
            "First arranged glyph should install exactly one Watermark.",
            failures);

        var secondGlyph = CreateTextWatermarkGlyph("Second");
        Watermark.SetGlyph(target, secondGlyph);
        RefreshLayout(realized.Window);
        var secondWatermark = ScopeAwareAdornerLayer.GetAdorner(target);

        Expect(secondWatermark is Watermark,
            "Replacing Glyph on arranged target should keep a Watermark installed.",
            failures);
        Expect(!ReferenceEquals(firstWatermark, secondWatermark),
            "Replacing Glyph should replace the Watermark instance so old glyph subscriptions are released.",
            failures);
        Expect(CountVisualByTypeName(realized.Window, "Watermark") == 1,
            "Replacing Glyph on arranged target should not duplicate Watermark visuals.",
            failures);
    }

    private static void VerifyClearingGlyphRemovesWatermark(ICollection<string> failures)
    {
        var target = CreateWatermarkTarget();
        var host = new VisualLayerManager
        {
            Child = target
        };

        using var realized = RealizeControl(host);

        Watermark.SetGlyph(target, CreateTextWatermarkGlyph("Visible"));
        RefreshLayout(realized.Window);
        Expect(CountVisualByTypeName(realized.Window, "Watermark") == 1,
            "Setting Glyph should install Watermark before clear.",
            failures);

        Watermark.SetGlyph(target, null);
        RefreshLayout(realized.Window);

        Expect(ScopeAwareAdornerLayer.GetAdorner(target) == null,
            "Clearing Glyph should clear the scope-aware adorner reference.",
            failures);
        Expect(CountVisualByTypeName(realized.Window, "Watermark") == 0,
            "Clearing Glyph should remove Watermark from visual tree.",
            failures);
    }

    private static void VerifyExistingAdornerIsNotReplacedByWatermark(ICollection<string> failures)
    {
        var target = CreateWatermarkTarget();
        var host = new VisualLayerManager
        {
            Child = target
        };

        using var realized = RealizeControl(host);

        var existingAdorner = new Border();
        ScopeAwareAdornerLayer.SetAdorner(target, existingAdorner);
        RefreshLayout(realized.Window);

        Watermark.SetGlyph(target, CreateTextWatermarkGlyph("Blocked"));
        RefreshLayout(realized.Window);

        Expect(ReferenceEquals(ScopeAwareAdornerLayer.GetAdorner(target), existingAdorner),
            "Watermark should not replace an existing non-Watermark scope-aware adorner.",
            failures);
        Expect(CountVisualByTypeName(realized.Window, "Watermark") == 0,
            "Watermark should not add a visual when the target already owns a different adorner.",
            failures);

        Watermark.SetGlyph(target, null);
        RefreshLayout(realized.Window);

        Expect(ReferenceEquals(ScopeAwareAdornerLayer.GetAdorner(target), existingAdorner),
            "Clearing Glyph should not clear an existing non-Watermark scope-aware adorner.",
            failures);
    }
}
