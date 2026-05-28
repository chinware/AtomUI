using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public class Col : ContentControl
{
    public static readonly StyledProperty<GridColSpanInfo> SpanProperty =
        AvaloniaProperty.Register<Col, GridColSpanInfo>(nameof(Span));

    public static readonly StyledProperty<int> OffsetProperty =
        AvaloniaProperty.Register<Col, int>(nameof(Offset), validate: v => v >= 0 && v <= 24);

    public static readonly StyledProperty<int> OrderProperty =
        AvaloniaProperty.Register<Col, int>(nameof(Order));

    public static readonly StyledProperty<int> PushProperty =
        AvaloniaProperty.Register<Col, int>(nameof(Push), validate: v => v >= 0 && v <= 24);

    public static readonly StyledProperty<int> PullProperty =
        AvaloniaProperty.Register<Col, int>(nameof(Pull), validate: v => v >= 0 && v <= 24);

    public static readonly StyledProperty<GridColSize?> XsProperty =
        AvaloniaProperty.Register<Col, GridColSize?>(nameof(Xs));

    public static readonly StyledProperty<GridColSize?> SmProperty =
        AvaloniaProperty.Register<Col, GridColSize?>(nameof(Sm));

    public static readonly StyledProperty<GridColSize?> MdProperty =
        AvaloniaProperty.Register<Col, GridColSize?>(nameof(Md));

    public static readonly StyledProperty<GridColSize?> LgProperty =
        AvaloniaProperty.Register<Col, GridColSize?>(nameof(Lg));

    public static readonly StyledProperty<GridColSize?> XlProperty =
        AvaloniaProperty.Register<Col, GridColSize?>(nameof(Xl));

    public static readonly StyledProperty<GridColSize?> XxlProperty =
        AvaloniaProperty.Register<Col, GridColSize?>(nameof(Xxl));

    public GridColSpanInfo Span
    {
        get => GetValue(SpanProperty);
        set => SetValue(SpanProperty, value);
    }

    public int Offset
    {
        get => GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    public int Order
    {
        get => GetValue(OrderProperty);
        set => SetValue(OrderProperty, value);
    }

    public int Push
    {
        get => GetValue(PushProperty);
        set => SetValue(PushProperty, value);
    }

    public int Pull
    {
        get => GetValue(PullProperty);
        set => SetValue(PullProperty, value);
    }

    public GridColSize? Xs
    {
        get => GetValue(XsProperty);
        set => SetValue(XsProperty, value);
    }

    public GridColSize? Sm
    {
        get => GetValue(SmProperty);
        set => SetValue(SmProperty, value);
    }

    public GridColSize? Md
    {
        get => GetValue(MdProperty);
        set => SetValue(MdProperty, value);
    }

    public GridColSize? Lg
    {
        get => GetValue(LgProperty);
        set => SetValue(LgProperty, value);
    }

    public GridColSize? Xl
    {
        get => GetValue(XlProperty);
        set => SetValue(XlProperty, value);
    }

    public GridColSize? Xxl
    {
        get => GetValue(XxlProperty);
        set => SetValue(XxlProperty, value);
    }

    static Col()
    {
        AffectsMeasure<Col>(
            SpanProperty,
            OffsetProperty,
            OrderProperty,
            PushProperty,
            PullProperty,
            XsProperty,
            SmProperty,
            MdProperty,
            LgProperty,
            XlProperty,
            XxlProperty);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SpanProperty ||
            change.Property == OffsetProperty ||
            change.Property == OrderProperty ||
            change.Property == PushProperty ||
            change.Property == PullProperty ||
            change.Property == XsProperty ||
            change.Property == SmProperty ||
            change.Property == MdProperty ||
            change.Property == LgProperty ||
            change.Property == XlProperty ||
            change.Property == XxlProperty)
        {
            if (this.GetVisualParent() is Row row)
            {
                row.InvalidateChildLayout();
            }
            else if (this.GetVisualParent() is Layoutable parent)
            {
                parent.InvalidateMeasure();
            }
        }
    }

    internal GridColLayout ResolveLayout(MediaBreakPoint breakPoint)
    {
        var layout = new GridColLayout(Span.GetValue(breakPoint), Offset, Order, Push, Pull);

        if (Xs is not null)
        {
            layout = Xs.ApplyTo(layout);
        }

        if (breakPoint >= MediaBreakPoint.Small && Sm is not null)
        {
            layout = Sm.ApplyTo(layout);
        }

        if (breakPoint >= MediaBreakPoint.Medium && Md is not null)
        {
            layout = Md.ApplyTo(layout);
        }

        if (breakPoint >= MediaBreakPoint.Large && Lg is not null)
        {
            layout = Lg.ApplyTo(layout);
        }

        if (breakPoint >= MediaBreakPoint.ExtraLarge && Xl is not null)
        {
            layout = Xl.ApplyTo(layout);
        }

        if (breakPoint >= MediaBreakPoint.ExtraExtraLarge && Xxl is not null)
        {
            layout = Xxl.ApplyTo(layout);
        }

        return layout;
    }
}
