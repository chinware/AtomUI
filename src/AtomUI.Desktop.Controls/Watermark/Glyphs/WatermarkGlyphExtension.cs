using Avalonia.Markup.Xaml;

namespace AtomUI.Desktop.Controls;

public abstract class WatermarkGlyphExtension : MarkupExtension
{
    public double HorizontalSpace { get; set; } = 280d;

    public double VerticalSpace { get; set; } = 40d;

    public double HorizontalOffset { get; set; } = 0;

    public double VerticalOffset { get; set; } = 0;

    public double Rotate { get; set; } = -20;

    public double Opacity { get; set; } = 0.3;

    public bool UseMirror { get; set; } = false;

    public bool UseCross { get; set; } = true;

    protected void SetProperties(WatermarkGlyph glyph)
    {
        glyph.HorizontalSpace  = HorizontalSpace;
        glyph.VerticalSpace    = VerticalSpace;
        glyph.HorizontalOffset = HorizontalOffset;
        glyph.VerticalOffset   = VerticalOffset;
        glyph.Rotate           = Rotate;
        glyph.Opacity          = Opacity;
        glyph.UseMirror        = UseMirror;
        glyph.UseCross         = UseCross;
    }
}