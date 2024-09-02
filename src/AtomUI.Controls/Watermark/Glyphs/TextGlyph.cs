using System.Globalization;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public class TextGlyph : WatermarkGlyph
{
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    public static readonly StyledProperty<string?> TextProperty = AvaloniaProperty
        .Register<TextGlyph, string?>(nameof(Text));
    
    protected FormattedText? FormattedText { get; private set; }

    static TextGlyph()
    {
        TextProperty.Changed.AddClassHandler<TextGlyph>((glyph, args) =>
        {
            glyph.RebuildFormatText();
        });
    }

    private void RebuildFormatText()
    {
        if (Text == null)
        {
            FormattedText = null;
            return;
        }
        
        // TODO Expose properties.
        FormattedText = new FormattedText(Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface.Default, 15, Brushes.Gray);
    }

    public override void Render(DrawingContext context)
    {
        if (FormattedText == null)
        {
            return;
        }
        
        context.DrawText(FormattedText, new Point());
    }

    public override Size GetDesiredSize()
    {
        return FormattedText == null ? new Size() : new Size(FormattedText.Width, FormattedText.Height);
    }
}