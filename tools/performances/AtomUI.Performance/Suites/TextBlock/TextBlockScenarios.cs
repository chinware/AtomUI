using AtomUI.Desktop.Controls;

namespace AtomUI.Performance;

internal static partial class Program
{
    private const string TextBlockShortText        = "Lorem ipsum";
    private const string TextBlockMediumText       = "Lorem ipsum dolor sit amet, consectetur adipiscing elit";
    private const string TextBlockHighlightShortQ  = "psum";
    private const string TextBlockHighlightMediumQ = "ipsum";

    private static IReadOnlyList<PerfScenario> CreateTextBlockScenarios()
    {
        return
        [
            new PerfScenario("TextBlock.Plain.Short", _ => new TextBlock { Text = TextBlockShortText }),
            new PerfScenario("TextBlock.Plain.Medium", _ => new TextBlock { Text = TextBlockMediumText }),

            new PerfScenario("TextBlock.Highlightable.NoQuery", _ => new HighlightableTextBlock
            {
                Text           = TextBlockMediumText,
                HighlightWords = string.Empty
            }),
            new PerfScenario("TextBlock.Highlightable.Match.Short", _ => new HighlightableTextBlock
            {
                Text           = TextBlockShortText,
                HighlightWords = TextBlockHighlightShortQ
            }),
            new PerfScenario("TextBlock.Highlightable.Match.Medium", _ => new HighlightableTextBlock
            {
                Text           = TextBlockMediumText,
                HighlightWords = TextBlockHighlightMediumQ
            }),
            new PerfScenario("TextBlock.Highlightable.Whole", _ => new HighlightableTextBlock
            {
                Text              = TextBlockMediumText,
                HighlightWords    = TextBlockHighlightMediumQ,
                HighlightStrategy = TextBlockHighlightStrategy.HighlightedWhole
            }),
            new PerfScenario("TextBlock.Highlightable.Match.Bold", _ => new HighlightableTextBlock
            {
                Text              = TextBlockMediumText,
                HighlightWords    = TextBlockHighlightMediumQ,
                HighlightStrategy = TextBlockHighlightStrategy.HighlightedMatch | TextBlockHighlightStrategy.BoldedMatch
            }),

            new PerfScenario("TextBlock.Selectable.Plain", _ => new SelectableTextBlock { Text = TextBlockMediumText }),
            new PerfScenario("TextBlock.Selectable.Selected", _ =>
            {
                var block = new SelectableTextBlock { Text = TextBlockMediumText };
                block.SetCurrentValue(SelectableTextBlock.SelectionStartProperty, 0);
                block.SetCurrentValue(SelectableTextBlock.SelectionEndProperty, 11);
                return block;
            }),

            new PerfScenario("TextBlock.HyperLink.Default", _ => new HyperLinkTextBlock { Text = TextBlockShortText })
        ];
    }
}
