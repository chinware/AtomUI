using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace AtomUI.Desktop.Controls;

/// <summary>
/// Arranges Breadcrumb item containers and their sibling separators in one horizontal flow:
/// container 0, separator 0, container 1, separator 1, ..., container N - 1.
/// Item containers stay in <see cref="Panel.Children"/> so the item generator keeps full
/// ownership of that collection; separator visuals are appended to the panel's visual
/// children so they render without polluting the generator's index mapping.
/// </summary>
internal class BreadcrumbItemsPanel : Panel
{
    private readonly List<Control> _separators = new();

    internal void SyncSeparators(IReadOnlyList<Control> separators)
    {
        ClearSeparatorVisuals();

        for (var i = 0; i < Children.Count - 1 && i < separators.Count; i++)
        {
            var separator = separators[i];
            if (!VisualChildren.Contains(separator))
            {
                VisualChildren.Add(separator);
            }

            _separators.Add(separator);
        }

        InvalidateMeasure();
    }

    internal void ClearSeparatorVisuals()
    {
        for (var i = VisualChildren.Count - 1; i >= 0; i--)
        {
            if (VisualChildren[i] is Control control && _separators.Contains(control))
            {
                VisualChildren.RemoveAt(i);
            }
        }

        _separators.Clear();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var desired = new Size();
        for (var i = 0; i < Children.Count; i++)
        {
            MeasureFlowChild(Children[i], availableSize, ref desired);
            if (i < Children.Count - 1 && i < _separators.Count)
            {
                MeasureFlowChild(_separators[i], availableSize, ref desired);
            }
        }

        return desired;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var offset = 0.0;
        for (var i = 0; i < Children.Count; i++)
        {
            offset = ArrangeFlowChild(Children[i], offset, finalSize);
            if (i < Children.Count - 1 && i < _separators.Count)
            {
                offset = ArrangeFlowChild(_separators[i], offset, finalSize);
            }
        }

        return finalSize;
    }

    private static void MeasureFlowChild(Control child, Size availableSize, ref Size desired)
    {
        child.Measure(availableSize);
        var childDesired = child.DesiredSize;
        desired = new Size(
            desired.Width + childDesired.Width + child.Margin.Left + child.Margin.Right,
            Math.Max(desired.Height, childDesired.Height + child.Margin.Top + child.Margin.Bottom));
    }

    private static double ArrangeFlowChild(Control child, double offset, Size finalSize)
    {
        var height = child.VerticalAlignment == VerticalAlignment.Stretch
            ? Math.Max(0, finalSize.Height - child.Margin.Top - child.Margin.Bottom)
            : child.DesiredSize.Height;

        var y = child.VerticalAlignment switch
        {
            VerticalAlignment.Center => (finalSize.Height - height - child.Margin.Top - child.Margin.Bottom) / 2 + child.Margin.Top,
            VerticalAlignment.Bottom => finalSize.Height - height - child.Margin.Bottom,
            _                        => child.Margin.Top
        };

        child.Arrange(new Rect(offset + child.Margin.Left, y, child.DesiredSize.Width, height));
        return offset + child.DesiredSize.Width + child.Margin.Left + child.Margin.Right;
    }
}
