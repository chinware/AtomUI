namespace AtomUI.Controls;

public class TextGlyphExtension : WatermarkGlyphExtension
{
    private string? Text { get; }

    public TextGlyphExtension(string text)
    {
        Text = text;
    }
    
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var glyph =  new TextGlyph()
        {
            Text = Text,
        };
        
        SetProperties(glyph);

        return glyph;
    }
}