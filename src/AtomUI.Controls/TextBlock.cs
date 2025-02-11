using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace AtomUI.Controls;

using AvaloniaTextBlock = Avalonia.Controls.TextBlock;

public class TextBlock : AvaloniaTextBlock
{
    private TextMetrics _textMetrics;
    
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        TokenResourceBinder.CreateTokenBinding(this, LineHeightProperty, SharedTokenKey.LineHeight,
            BindingPriority.Template,
            o =>
            {
                if (o is double dvalue)
                {
                    return dvalue * FontSize;
                }

                return o;
            });
        CalculateTextMetrics();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (VisualRoot is not null)
        {
            if (change.Property == FontSizeProperty || 
                change.Property == FontFamilyProperty || 
                change.Property == FontStyleProperty)
            {
                CalculateTextMetrics();
            }
        }
    }

    private void CalculateTextMetrics()
    {
        var typeface    = new Typeface(FontFamily, FontStyle, FontWeight);
        if (FontManager.Current.TryGetGlyphTypeface(typeface, out var glyphTypeface))
        {
            _textMetrics = new TextMetrics(glyphTypeface, FontSize);
        }
    }
    
    protected override void RenderTextLayout(DrawingContext context, Point origin)
    {
        var textHeight = TextLayout.Height;
        var top = origin.Y;
        var left = TextLayout.OverhangLeading;
        if (TextLayout.TextLines.Count == 1)
        {
            if (Bounds.Height >= textHeight)
            {
                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Center:
                        top += _textMetrics.Descent / 2;
                        break;
            
                    case VerticalAlignment.Bottom:
                        top += Bounds.Height - textHeight;
                        break;
                }
            }

            if (HorizontalAlignment == HorizontalAlignment.Center)
            {
                left += (TextLayout.OverhangLeading + TextLayout.OverhangTrailing) / 2;
            }
        }
        TextLayout.Draw(context, origin + new Point(left, top));
    }
}