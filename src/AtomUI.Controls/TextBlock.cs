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
        var height = size.Height;
        if (VerticalAlignment == VerticalAlignment.Center)
        {
            height += (TextLayout.Extent - TextLayout.Baseline) / 2;
        }

        return size.WithHeight(height);
    }
    
    protected override void RenderTextLayout(DrawingContext context, Point origin)
    {
        var textHeight = TextLayout.Height;
        var textWidth = TextLayout.Width;
        var top = 0d;
        switch (VerticalAlignment)
        {
            case VerticalAlignment.Center:
                top += (Bounds.Height - textHeight) / 2 + (TextLayout.Extent - TextLayout.Baseline) / 4;
                break;

            case VerticalAlignment.Bottom:
                top += Bounds.Height - textHeight;
                break;
        }

        var left = TextLayout.OverhangLeading;

        if (textWidth < Bounds.Width)
        {
            if (HorizontalAlignment == HorizontalAlignment.Center)
            {
                left += (TextLayout.OverhangLeading + TextLayout.OverhangTrailing) / 2;
            }
        }

        TextLayout.Draw(context, origin + new Point(left, top));
    }
}