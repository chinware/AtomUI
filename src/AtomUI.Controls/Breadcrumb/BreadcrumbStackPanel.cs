using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal class BreadcrumbStackPanel : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        var  targetHeight = 0d;
        var  targetWidth  = 0d;
        foreach (var child in Children)
        {
            if (child is BreadcrumbItem box)
            {
                box.Measure(availableSize);
                targetWidth  += box.DesiredSize.Width;
                targetHeight =  Math.Max(targetHeight, box.DesiredSize.Height);
            }
        }

        return new Size(targetWidth, targetHeight);
    }
    
    protected override Size ArrangeOverride(Size finalSize)
    {
        var previousChildSize = 0.0;
        var offsetX           = 0d;
        var offsetY           = 0d;
        foreach (var child in Children)
        {
            if (child is BreadcrumbItem box)
            {
                previousChildSize = box.DesiredSize.Width;
                box.Arrange(new Rect(new Point(offsetX, offsetY), box.DesiredSize));
                offsetX += previousChildSize;
            }
        }
        return finalSize;
    }
}