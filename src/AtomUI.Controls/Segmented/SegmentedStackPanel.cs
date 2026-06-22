using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls.Commons;

internal class SegmentedStackPanel : Panel
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsExpandingProperty =
        AvaloniaProperty.Register<SegmentedStackPanel, bool>(nameof(IsExpanding));

    public bool IsExpanding
    {
        get => GetValue(IsExpandingProperty);
        set => SetValue(IsExpandingProperty, value);
    }
    
    #endregion

    protected override Size MeasureOverride(Size availableSize)
    {
        if (!IsExpanding)
        {
            return MeasureOverrideNoExpanding(availableSize);
        }

        return MeasureOverrideExpanding(availableSize);
    }

    private Size MeasureOverrideNoExpanding(Size availableSize)
    {
        var layoutSlotSize = availableSize;
        layoutSlotSize = layoutSlotSize.WithWidth(double.PositiveInfinity);
        var targetWidth  = 0d;
        var targetHeight = 0d;
        foreach (var child in Children)
        {
            if (child is AbstractSegmentedItem box)
            {
                if (!box.IsVisible)
                {
                    continue;
                }

                box.Measure(layoutSlotSize);
                var childDesiredSize = box.DesiredSize;
                targetWidth  += childDesiredSize.Width;
                targetHeight = Math.Max(targetHeight, childDesiredSize.Height);
            }
        }

        return new Size(targetWidth, targetHeight);
    }

    private Size MeasureOverrideExpanding(Size availableSize)
    {
        var visibleCount = CountVisibleSegmentedItems();
        if (visibleCount == 0)
        {
            return new Size(double.IsInfinity(availableSize.Width) ? 0 : availableSize.Width, 0);
        }

        var maxHeight          = 0d;
        var availableWidth     = availableSize.Width;
        var childAvailableSize = new Size(availableWidth / visibleCount, availableSize.Height);
        foreach (var child in Children)
        {
            if (child is AbstractSegmentedItem { IsVisible: true } box)
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
        if (!IsExpanding)
        {
            ArrangeOverrideNoExpanding(finalSize);
        }
        else
        {
            ArrangeOverrideExpanding(finalSize);
        }

        return finalSize;
    }

    private Size ArrangeOverrideNoExpanding(Size finalSize)
    {
        var offsetX = 0d;
        foreach (var child in Children)
        {
            if (child is AbstractSegmentedItem box)
            {
                if (!box.IsVisible)
                {
                    continue;
                }

                box.Arrange(new Rect(new Point(offsetX, 0), box.DesiredSize));
                offsetX += box.DesiredSize.Width;
            }
        }

        return finalSize;
    }

    private Size ArrangeOverrideExpanding(Size finalSize)
    {
        var visibleCount = CountVisibleSegmentedItems();
        if (visibleCount == 0)
        {
            return finalSize;
        }

        var width   = finalSize.Width / visibleCount;
        var offsetX = 0d;
        foreach (var child in Children)
        {
            if (child is AbstractSegmentedItem { IsVisible: true } box)
            {
                box.Arrange(new Rect(offsetX, 0, width, box.DesiredSize.Height));
                offsetX += width;
            }
        }

        return finalSize;
    }

    private int CountVisibleSegmentedItems()
    {
        var count = 0;
        foreach (var child in Children)
        {
            if (child is AbstractSegmentedItem { IsVisible: true })
            {
                count++;
            }
        }

        return count;
    }
}
