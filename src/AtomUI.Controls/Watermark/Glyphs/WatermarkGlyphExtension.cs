using Avalonia.Markup.Xaml;

namespace AtomUI.Controls;

public abstract class WatermarkGlyphExtension : MarkupExtension
{
    public double Space { get; set; }
    
    public double Angle { get; set; }
    
    public double Opacity { get; set; }
    
    public bool UseMirror { get; set; }
    
    public bool UseCross { get; set; }

    protected void SetProperties(WatermarkGlyph glyph)
    {
        glyph.Space     = Space;
        glyph.Angle     = Angle;
        glyph.Opacity   = Opacity;
        glyph.UseMirror = UseMirror;
        glyph.UseCross  = UseCross;
    }
}