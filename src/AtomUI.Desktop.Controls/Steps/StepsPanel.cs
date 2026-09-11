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

    public static readonly StyledProperty<Orientation> TitlePlacementProperty =
        AvaloniaProperty.Register<StepsPanel, Orientation>(nameof(TitlePlacement), Orientation.Horizontal);

    public static readonly StyledProperty<int> OffsetProperty =
        AvaloniaProperty.Register<StepsPanel, int>(nameof(Offset));

    public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
        AvaloniaProperty.Register<StepsPanel, HorizontalAlignment>(
            nameof(HorizontalContentAlignment),
            HorizontalAlignment.Center);

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

    public Orientation TitlePlacement
    {
        get => GetValue(TitlePlacementProperty);
        set => SetValue(TitlePlacementProperty, value);
    }

    public int Offset
    {
        get => GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    public HorizontalAlignment HorizontalContentAlignment
    {
        get => GetValue(HorizontalContentAlignmentProperty);
        set => SetValue(HorizontalContentAlignmentProperty, value);
    }

    static StepsPanel()
    {
        AffectsMeasure<StepsPanel>(TypeProperty, OrientationProperty, TitlePlacementProperty, OffsetProperty);
        AffectsArrange<StepsPanel>(
            TypeProperty,
            OrientationProperty,
            TitlePlacementProperty,
            OffsetProperty,
            HorizontalContentAlignmentProperty);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var width = 0d;
        var height = 0d;
        var orientation = EffectiveOrientation;

        foreach (var child in Children)
        {
            child.Measure(availableSize);
            if (!child.IsVisible)
            {
                continue;
            }

            if (orientation == Orientation.Vertical)
            {
                width = Math.Max(width, child.DesiredSize.Width);
                height += child.DesiredSize.Height;
            }
            else
            {
                width += child.DesiredSize.Width;
                height = Math.Max(height, child.DesiredSize.Height);
            }
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

        var orientation = EffectiveOrientation;

        if (orientation == Orientation.Vertical)
        {
            if (Type == StepsType.Navigation)
            {
                ArrangeVerticalNavigation(children, finalSize);
            }
            else
            {
                ArrangeVertically(children, finalSize);
            }
            return finalSize;
        }

        if (ShouldArrangeTitleVerticalItemsEqually())
        {
            ArrangeTitleVertical(children, finalSize);
            return finalSize;
        }

        switch (Type)
        {
            case StepsType.Navigation:
            case StepsType.Panel:
                ArrangeNavigation(children, finalSize, Type == StepsType.Panel);
                break;

            default:
                ArrangeDefault(children, finalSize);
                break;
        }

        return finalSize;
    }

    private static void ArrangeVertically(IReadOnlyList<Control> children, Size finalSize)
    {
        var y = 0d;
        foreach (var child in children)
        {
            var height = child.DesiredSize.Height;
            child.Arrange(new Rect(0, y, finalSize.Width, height));
            y += height;
        }
    }

    private void ArrangeVerticalNavigation(IReadOnlyList<Control> children, Size finalSize)
    {
        var width = HorizontalContentAlignment == HorizontalAlignment.Stretch
            ? finalSize.Width
            : Math.Min(finalSize.Width, children.Max(static child => child.DesiredSize.Width));
        var x = GetHorizontalAlignedX(finalSize.Width, width);
        var y     = 0d;
        foreach (var child in children)
        {
            var height = child.DesiredSize.Height;
            child.Arrange(new Rect(x, y, width, height));
            y += height;
        }
    }

    private double GetHorizontalAlignedX(double finalWidth, double contentWidth)
    {
        return HorizontalContentAlignment switch
        {
            HorizontalAlignment.Right  => Math.Max(0, finalWidth - contentWidth),
            HorizontalAlignment.Center => Math.Max(0, (finalWidth - contentWidth) / 2),
            _                          => 0
        };
    }

    private bool ShouldArrangeTitleVerticalItemsEqually()
    {
        return StepsItemLayoutPanel.ResolveTitlePlacement(Type, EffectiveOrientation, TitlePlacement) == Orientation.Vertical;
    }

    private Orientation EffectiveOrientation => Type == StepsType.Panel ? Orientation.Horizontal : Orientation;

    private static void ArrangeNavigation(
        IReadOnlyList<Control> children,
        Size finalSize,
        bool panel = false)
    {
        var width = finalSize.Width / children.Count;
        var x = 0d;
        for (var index = 0; index < children.Count; index++)
        {
            var child = children[index];
            child.ZIndex = panel ? children.Count - index : 0;
            child.Arrange(new Rect(x, 0, width, finalSize.Height));
            x += width;
        }
    }

    private void ArrangeTitleVertical(IReadOnlyList<Control> children, Size finalSize)
    {
        var offset    = Type == StepsType.Inline ? Math.Max(0, Offset) : 0;
        var width     = finalSize.Width / (children.Count + offset);
        var rowHeight = Math.Min(finalSize.Height, children.Max(static child => child.DesiredSize.Height));
        var rowY      = Math.Max(0, (finalSize.Height - rowHeight) / 2);
        var x         = width * offset;

        foreach (var child in children)
        {
            child.Arrange(new Rect(x, rowY, width, rowHeight));
            x += width;
        }
    }

    private static void ArrangeDefault(IReadOnlyList<Control> children, Size finalSize)
    {
        var rowHeight = Math.Min(finalSize.Height, children.Max(static child => child.DesiredSize.Height));
        var rowY      = Math.Max(0, (finalSize.Height - rowHeight) / 2);

        if (children.Count == 1)
        {
            var child = children[0];
            child.Arrange(new Rect(0, rowY, child.DesiredSize.Width, rowHeight));
            return;
        }

        var lastChild = children[^1];
        var itemWidth = (finalSize.Width - lastChild.DesiredSize.Width) / (children.Count - 1);
        var x = 0d;

        for (var index = 0; index < children.Count - 1; ++index)
        {
            var child = children[index];
            var width = Math.Max(itemWidth, child.DesiredSize.Width);
            child.Arrange(new Rect(x, rowY, width, rowHeight));
            x += width;
        }

        lastChild.Arrange(new Rect(x, rowY, lastChild.DesiredSize.Width, rowHeight));
    }
}
