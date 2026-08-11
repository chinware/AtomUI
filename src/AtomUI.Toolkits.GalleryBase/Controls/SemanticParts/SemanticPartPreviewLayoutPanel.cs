using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Toolkits.GalleryBase.Controls;

internal sealed class SemanticPartPreviewLayoutPanel : Panel
{
    public static readonly StyledProperty<double> CompactBreakpointProperty =
        AvaloniaProperty.Register<SemanticPartPreviewLayoutPanel, double>(nameof(CompactBreakpoint), 820);

    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<SemanticPartPreviewLayoutPanel, double>(nameof(Spacing), 24);

    public double CompactBreakpoint
    {
        get => GetValue(CompactBreakpointProperty);
        set => SetValue(CompactBreakpointProperty, value);
    }

    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Children.Count == 0)
        {
            return default;
        }

        if (Children.Count == 1)
        {
            Children[0].Measure(availableSize);
            return Children[0].DesiredSize;
        }

        if (ShouldUseCompactLayout(availableSize.Width))
        {
            var childSize = new Size(availableSize.Width, double.PositiveInfinity);
            Children[0].Measure(childSize);
            Children[1].Measure(childSize);
            return new Size(
                Math.Max(Children[0].DesiredSize.Width, Children[1].DesiredSize.Width),
                Children[0].DesiredSize.Height + Spacing + Children[1].DesiredSize.Height);
        }

        var rightWidth = ResolveRightWidth(availableSize.Width);
        var leftWidth = Math.Max(0, availableSize.Width - Spacing - rightWidth);
        Children[0].Measure(new Size(leftWidth, availableSize.Height));
        Children[1].Measure(new Size(rightWidth, availableSize.Height));
        return new Size(
            Children[0].DesiredSize.Width + Spacing + Children[1].DesiredSize.Width,
            Math.Max(Children[0].DesiredSize.Height, Children[1].DesiredSize.Height));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Children.Count == 0)
        {
            return finalSize;
        }

        if (Children.Count == 1)
        {
            Children[0].Arrange(new Rect(finalSize));
            return finalSize;
        }

        if (ShouldUseCompactLayout(finalSize.Width))
        {
            var firstHeight = Children[0].DesiredSize.Height;
            Children[0].Arrange(new Rect(0, 0, finalSize.Width, firstHeight));
            Children[1].Arrange(new Rect(
                0,
                firstHeight + Spacing,
                finalSize.Width,
                Math.Max(0, finalSize.Height - firstHeight - Spacing)));
            return finalSize;
        }

        var rightWidth = ResolveRightWidth(finalSize.Width);
        var leftWidth = Math.Max(0, finalSize.Width - Spacing - rightWidth);
        Children[0].Arrange(new Rect(0, 0, leftWidth, finalSize.Height));
        Children[1].Arrange(new Rect(leftWidth + Spacing, 0, rightWidth, finalSize.Height));
        return finalSize;
    }

    private bool ShouldUseCompactLayout(double width)
    {
        return double.IsFinite(width) && width < CompactBreakpoint;
    }

    private static double ResolveRightWidth(double width)
    {
        if (!double.IsFinite(width))
        {
            return 420;
        }
        return Math.Clamp(width * 0.38, 340, 460);
    }
}
