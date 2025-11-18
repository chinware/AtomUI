using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

public class TextGlyphExtension : WatermarkGlyphExtension
{
    private string? Text { get; }

    public double FontSize { get; set; } = 16;

    public IBrush Foreground { get; set; } = Brushes.Black;

    public TextGlyphExtension(string text)
    {
        Text = text;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var glyph = new TextGlyph
        {
            Text       = Text,
            FontSize   = FontSize,
            Foreground = Foreground
        };

        SetProperties(glyph);

        return glyph;
    }
}