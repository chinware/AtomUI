using AtomUI.Desktop.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Input;
using Avalonia.Media;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunTextBlockStateVerification()
    {
        var failures = new List<string>();
        VerifyHighlightableInlineSegments(failures);
        VerifyHighlightableEmptyQueryClearsInlines(failures);
        VerifyHighlightableWholeStrategy(failures);
        VerifySelectableTextBlockSelectionBrushes(failures);
        VerifySelectableTextBlockCursor(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("TextBlock state verification passed.");
            return true;
        }

        Console.Error.WriteLine("TextBlock state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyHighlightableInlineSegments(ICollection<string> failures)
    {
        // "Lorem ipsum" with HighlightWords = "ipsum" → expected segments:
        //   "Lorem " (unmatched) + "ipsum" (matched) = 2 Runs.
        // The pre-optimization implementation produces one Run per character (11 Runs).
        var block = new HighlightableTextBlock
        {
            Text           = "Lorem ipsum",
            HighlightWords = "ipsum"
        };
        using var realized = RealizeControl(block);
        RefreshLayout(realized.Window);

        var inlines = block.Inlines;
        Expect(inlines is not null,
            "HighlightableTextBlock should produce Inlines when HighlightWords is set.",
            failures);
        if (inlines is null)
        {
            return;
        }
        Expect(inlines.Count <= 4,
            $"HighlightableTextBlock should emit segment-level Runs (got {inlines.Count} for an 11-char text with one match).",
            failures);

        var combined = string.Concat(inlines.OfType<Run>().Select(r => r.Text ?? string.Empty));
        Expect(combined == "Lorem ipsum",
            $"HighlightableTextBlock segments should round-trip to original text (got '{combined}').",
            failures);

        // Default HighlightStrategy is `All` which includes BoldedMatch — the matched segment(s) must be bolded.
        var boldRuns = inlines.OfType<Run>().Where(r => r.FontWeight == FontWeight.Bold).ToList();
        Expect(boldRuns.Count >= 1 && boldRuns.All(r => (r.Text ?? string.Empty).Equals("ipsum", StringComparison.OrdinalIgnoreCase)),
            "HighlightableTextBlock matched runs should be bold and cover only the highlighted substring.",
            failures);
    }

    private static void VerifyHighlightableEmptyQueryClearsInlines(ICollection<string> failures)
    {
        var block = new HighlightableTextBlock
        {
            Text           = "Lorem ipsum",
            HighlightWords = "ipsum"
        };
        using var realized = RealizeControl(block);
        RefreshLayout(realized.Window);
        Expect(block.Inlines is not null,
            "HighlightableTextBlock should produce Inlines while HighlightWords is set.",
            failures);

        block.SetCurrentValue(HighlightableTextBlock.HighlightWordsProperty, string.Empty);
        RefreshLayout(realized.Window);
        Expect(block.Inlines is null,
            "HighlightableTextBlock should clear Inlines when HighlightWords becomes empty.",
            failures);
    }

    private static void VerifyHighlightableWholeStrategy(ICollection<string> failures)
    {
        var block = new HighlightableTextBlock
        {
            Text              = "Lorem ipsum dolor",
            HighlightWords    = "ipsum",
            HighlightStrategy = TextBlockHighlightStrategy.HighlightedWhole
        };
        using var realized = RealizeControl(block);
        RefreshLayout(realized.Window);

        var inlines = block.Inlines;
        Expect(inlines is not null,
            "HighlightableTextBlock with HighlightedWhole should produce Inlines.",
            failures);
        if (inlines is null)
        {
            return;
        }
        Expect(inlines.Count == 1,
            $"HighlightableTextBlock with HighlightedWhole should emit a single Run (got {inlines.Count}).",
            failures);

        var only = inlines.OfType<Run>().FirstOrDefault();
        Expect(only?.Text == "Lorem ipsum dolor",
            "HighlightableTextBlock HighlightedWhole single Run should carry the entire text.",
            failures);
    }

    private static void VerifySelectableTextBlockSelectionBrushes(ICollection<string> failures)
    {
        var block = new SelectableTextBlock { Text = "Lorem ipsum" };
        using var realized = RealizeControl(block);
        RefreshLayout(realized.Window);

        Expect(block.SelectionBrush is not null,
            "SelectableTextBlock SelectionBrush should resolve from theme on attach.",
            failures);
        Expect(block.SelectionForegroundBrush is not null,
            "SelectableTextBlock SelectionForegroundBrush should resolve from theme on attach.",
            failures);
    }

    private static void VerifySelectableTextBlockCursor(ICollection<string> failures)
    {
        var first  = new SelectableTextBlock { Text = "alpha" };
        var second = new SelectableTextBlock { Text = "beta" };
        using var firstRealized  = RealizeControl(first);
        using var secondRealized = RealizeControl(second);
        RefreshLayout(firstRealized.Window);
        RefreshLayout(secondRealized.Window);

        Expect(first.Cursor is not null,
            "SelectableTextBlock should resolve a Cursor from theme.",
            failures);
        Expect(second.Cursor is not null,
            "SelectableTextBlock should resolve a Cursor on a second instance too.",
            failures);
        Expect(ReferenceEquals(first.Cursor, second.Cursor),
            "SelectableTextBlock Cursor should be shared across instances when sourced from theme Setter.",
            failures);
    }
}
