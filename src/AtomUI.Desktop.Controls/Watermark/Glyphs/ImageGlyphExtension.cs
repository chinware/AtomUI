using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

public class ImageGlyphExtension : WatermarkGlyphExtension
{
    public IImage? Source { get; set; }

    public double Height { get; set; } = 28;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var glyph = new ImageGlyph
        {
            Source = Source,
            Height = Height
        };

        SetProperties(glyph);

        return glyph;
    }
}