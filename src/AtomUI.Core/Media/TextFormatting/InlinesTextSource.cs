using Avalonia.Media.TextFormatting;
using Avalonia.Utilities;

namespace AtomUI.Media.TextFormatting;

/// <summary>
/// Text source for inline text runs with optional style overrides.
/// </summary>
internal readonly struct InlinesTextSource : ITextSource
{
    private readonly IReadOnlyList<TextRun> _textRuns;
    private readonly IReadOnlyList<ValueSpan<TextRunProperties>>? _textModifier;

    public InlinesTextSource(IReadOnlyList<TextRun> textRuns, IReadOnlyList<ValueSpan<TextRunProperties>>? textModifier = null)
    {
        _textRuns = textRuns;
        _textModifier = textModifier;
    }

    public TextRun? GetTextRun(int textSourceIndex)
    {
        var currentPosition = 0;

        foreach (var textRun in _textRuns)
        {
            if (textRun.Length == 0)
            {
                continue;
            }

            if (textSourceIndex >= currentPosition + textRun.Length)
            {
                currentPosition += textRun.Length;
                continue;
            }

            if (textRun is TextCharacters textCharacters)
            {
                var skip = Math.Max(0, textSourceIndex - currentPosition);
                var textStyleRun = FormattedTextSource.CreateTextStyleRun(textRun.Text.Slice(skip).Span, textSourceIndex, textCharacters.Properties, _textModifier);

                return new TextCharacters(textRun.Text.Slice(skip, textStyleRun.Length), textStyleRun.Value);
            }

            return textRun;
        }

        return new TextEndOfParagraph();
    }
}
