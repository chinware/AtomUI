using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal class BreadcrumbStackPanel : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        return MeasureOverrideNoExpanding(availableSize);
    }

    private Size MeasureOverrideNoExpanding(Size availableSize)
    {
        var layoutSlotSize = availableSize;
        layoutSlotSize = layoutSlotSize.WithWidth(double.PositiveInfinity);
        var hasVisibleChild = false;
        var targetWidth     = 0d;
        var targetHeight    = 0d;
        foreach (var child in Children)
        {
            if (child is BreadcrumbItem box)
            {
                var isVisible = box.IsVisible;
                if (isVisible && !hasVisibleChild)
                {
                    hasVisibleChild = true;
                }

                box.Measure(layoutSlotSize);
                var childDesiredSize = box.DesiredSize;
                targetWidth  += childDesiredSize.Width;
                targetHeight =  Math.Max(targetHeight, childDesiredSize.Height);
            }
        }

        return new Size(targetWidth, targetHeight);
    }

    private Size MeasureOverrideExpanding(Size availableSize)
    {
        var maxHeight          = 0d;
        var columns            = Children.Count;
        var availableWidth     = availableSize.Width;
        var childAvailableSize = new Size(availableWidth / columns, availableSize.Height);
        foreach (var child in Children)
        {
            if (child is BreadcrumbItem box)
            {
                box.Measure(childAvailableSize);
                if (box.DesiredSize.Height > maxHeight)
                {
                    maxHeight = box.DesiredSize.Height;
                }
            }
        }
        return new Size(availableSize.Width, maxHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        ArrangeOverrideNoExpanding(finalSize);
        return finalSize;
    }

    private Size ArrangeOverrideNoExpanding(Size finalSize)
    {
        var previousChildSize = 0.0;
        var offsetX           = 0d;
        var offsetY           = 0d;
        foreach (var child in Children)
        {
            if (child is BreadcrumbItem box)
            {
                if (!box.IsVisible)
                {
                    continue;
                }

                previousChildSize = box.DesiredSize.Width;

                box.Arrange(new Rect(new Point(offsetX, offsetY), box.DesiredSize));
                offsetX += previousChildSize;
            }
        }

        return finalSize;
    }

    private Size ArrangeOverrideExpanding(Size finalSize)
    {
        var offsetX        = 0d;
        var offsetY        = 0d;
        var columns        = Children.Count;
        var availableWidth = finalSize.Width;
        var width          = availableWidth / columns;

        var x = 0;

        foreach (var child in Children)
        {
            if (child is BreadcrumbItem box)
            {
                if (!box.IsVisible)
                {
                    continue;
                }

                box.Arrange(new Rect(x * width + offsetX, offsetY, width, box.DesiredSize.Height));
                x++;
            }
        }

        return finalSize;
    }
}