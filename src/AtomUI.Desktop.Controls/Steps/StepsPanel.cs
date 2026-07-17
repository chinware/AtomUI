using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace AtomUI.Desktop.Controls;

internal class StepsPanel : Panel
{
    public static readonly StyledProperty<StepsType> TypeProperty =
        AvaloniaProperty.Register<StepsPanel, StepsType>(nameof(Type), StepsType.Default);

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<StepsPanel, Orientation>(nameof(Orientation), Orientation.Horizontal);

    public static readonly StyledProperty<double> VerticalItemSpacingProperty =
        AvaloniaProperty.Register<StepsPanel, double>(nameof(VerticalItemSpacing));

    public StepsType Type
    {
        get => GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public double VerticalItemSpacing
    {
        get => GetValue(VerticalItemSpacingProperty);
        set => SetValue(VerticalItemSpacingProperty, value);
    }

    static StepsPanel()
    {
        AffectsMeasure<StepsPanel>(TypeProperty, OrientationProperty, VerticalItemSpacingProperty);
        AffectsArrange<StepsPanel>(TypeProperty, OrientationProperty, VerticalItemSpacingProperty);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var width = 0d;
        var height = 0d;

        var visibleCount = 0;
        foreach (var child in Children)
        {
            child.Measure(availableSize);
            if (!child.IsVisible)
            {
                continue;
            }

            if (Orientation == Orientation.Vertical)
            {
                width = Math.Max(width, child.DesiredSize.Width);
                height += child.DesiredSize.Height;
                visibleCount++;
            }
            else
            {
                width += child.DesiredSize.Width;
                height = Math.Max(height, child.DesiredSize.Height);
            }
        }


        if (Orientation == Orientation.Vertical && visibleCount > 1)
        {
            height += VerticalItemSpacing * (visibleCount - 1);
        }

        return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var children = Children.Where(static child => child.IsVisible).ToArray();
        if (children.Length == 0)
        {
            return finalSize;
        }

        if (Orientation == Orientation.Vertical)
        {
            ArrangeVertically(children, finalSize, VerticalItemSpacing);
            return finalSize;
        }

        switch (Type)
        {
            case StepsType.Navigation:
                ArrangeNavigation(children, finalSize);
                break;

            case StepsType.Inline:
                ArrangeInline(children, finalSize);
                break;

            default:
                ArrangeDefault(children, finalSize);
                break;
        }

        return finalSize;
    }

    private static void ArrangeVertically(IReadOnlyList<Control> children, Size finalSize, double spacing)
    {
        var y = 0d;
        foreach (var child in children)
        {
            var height = child.DesiredSize.Height;
            child.Arrange(new Rect(0, y, finalSize.Width, height));
            y += height + spacing;
        }
    }

    private static void ArrangeNavigation(IReadOnlyList<Control> children, Size finalSize)
    {
        var width = finalSize.Width / children.Count;
        var x = 0d;
        foreach (var child in children)
        {
            child.Arrange(new Rect(x, 0, width, finalSize.Height));
            x += width;
        }
    }

    private static void ArrangeInline(IReadOnlyList<Control> children, Size finalSize)
    {
        var x = 0d;
        foreach (var child in children)
        {
            var width = child.DesiredSize.Width;
            child.Arrange(new Rect(x, 0, width, finalSize.Height));
            x += width;
        }
    }

    private static void ArrangeDefault(IReadOnlyList<Control> children, Size finalSize)
    {
        if (children.Count == 1)
        {
            var child = children[0];
            child.Arrange(new Rect(0, 0, child.DesiredSize.Width, finalSize.Height));
            return;
        }

        var lastChild = children[^1];
        var itemWidth = (finalSize.Width - lastChild.DesiredSize.Width) / (children.Count - 1);
        var x = 0d;

        for (var index = 0; index < children.Count - 1; ++index)
        {
            var child = children[index];
            var width = Math.Max(itemWidth, child.DesiredSize.Width);
            child.Arrange(new Rect(x, 0, width, finalSize.Height));
            x += width;
        }

        lastChild.Arrange(new Rect(x, 0, lastChild.DesiredSize.Width, finalSize.Height));
    }
}
