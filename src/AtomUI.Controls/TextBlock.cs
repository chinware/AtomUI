using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

using AvaloniaTextBlock = Avalonia.Controls.TextBlock;

public class TextBlock : AvaloniaTextBlock
{
    protected override Size MeasureOverride(Size availableSize)
    {
        var size = base.MeasureOverride(availableSize);
        if (TextLayout.TextLines.Count == 1)
        {
            var height = size.Height;
            var width = size.Width;
            if (VerticalAlignment == VerticalAlignment.Center)
            {
                height += (TextLayout.Extent - TextLayout.Baseline) / 4;
            }

            return new Size(width, height);
        }

        return size;
    }
    
    protected override void RenderTextLayout(DrawingContext context, Point origin)
    {
        var textHeight = TextLayout.Height;
        var textWidth = TextLayout.Width;
        var top = origin.Y;
        var left = TextLayout.OverhangLeading;
        if (TextLayout.TextLines.Count == 1)
        {
            if (textHeight < Bounds.Height)
            {
                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Center:
                        top += (Bounds.Height - textHeight) / 2 + (TextLayout.Extent - TextLayout.Baseline) / 4;
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