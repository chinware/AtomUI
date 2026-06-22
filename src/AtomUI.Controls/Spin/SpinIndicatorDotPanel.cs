using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls.Commons;

internal sealed class SpinIndicatorDotPanel : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        var desiredSize = default(Size);
        foreach (var child in Children)
        {
            child.Measure(availableSize);
            desiredSize = new Size(
                Math.Max(desiredSize.Width, child.DesiredSize.Width),
                Math.Max(desiredSize.Height, child.DesiredSize.Height));
        }

        var width  = double.IsNaN(Width) ? desiredSize.Width : Width;
        var height = double.IsNaN(Height) ? desiredSize.Height : Height;
        return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        for (var i = 0; i < Children.Count; i++)
        {
            var child = Children[i];
            child.Arrange(i < 4
                ? GetDotBounds(i, finalSize, child.DesiredSize)
                : default);
        }

        return finalSize;
    }

    private static Rect GetDotBounds(int index, Size finalSize, Size dotSize)
    {
        var centerX = finalSize.Width / 2;
        var centerY = finalSize.Height / 2;

        return index switch
        {
            0 => new Rect(
                Math.Max(0, finalSize.Width - dotSize.Width),
                centerY - dotSize.Height / 2,
                dotSize.Width,
                dotSize.Height),
            1 => new Rect(
                centerX - dotSize.Width / 2,
                Math.Max(0, finalSize.Height - dotSize.Height),
                dotSize.Width,
                dotSize.Height),
            2 => new Rect(
                0,
                centerY - dotSize.Height / 2,
                dotSize.Width,
                dotSize.Height),
            _ => new Rect(
                centerX - dotSize.Width / 2,
                0,
                dotSize.Width,
                dotSize.Height)
        };
    }
}
