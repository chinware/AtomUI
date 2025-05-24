using System.Reactive.Disposables;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class TimelineIndicator : Control,
                                   IResourceBindingManager
{
    #region 公共属性定义

    public static readonly StyledProperty<Icon?> IndicatorIconProperty =
        AvaloniaProperty.Register<TimelineIndicator, Icon?>(nameof(IndicatorIcon));

    public static readonly StyledProperty<IBrush?> IndicatorColorProperty =
        AvaloniaProperty.Register<TimelineIndicator, IBrush?>(nameof(IndicatorColor));

    public Icon? IndicatorIcon
    {
        get => GetValue(IndicatorIconProperty);
        set => SetValue(IndicatorIconProperty, value);
    }

    public IBrush? IndicatorColor
    {
        get => GetValue(IndicatorColorProperty);
        set => SetValue(IndicatorColorProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<TimelineIndicator, double> LineHeightRatioProperty
        = AvaloniaProperty.RegisterDirect<TimelineIndicator, double>(nameof(LineHeightRatio),
            o => o.LineHeightRatio,
            (o, v) => o.LineHeightRatio = v);

    internal static readonly DirectProperty<TimelineIndicator, bool> IsFirstProperty
        = AvaloniaProperty.RegisterDirect<TimelineIndicator, bool>(nameof(IsFirst),
            o => o.IsFirst,
            (o, v) => o.IsFirst = v);

    internal static readonly DirectProperty<TimelineIndicator, bool> IsLastProperty
        = AvaloniaProperty.RegisterDirect<TimelineIndicator, bool>(nameof(IsLast),
            o => o.IsLast,
            (o, v) => o.IsLast = v);

    internal static readonly DirectProperty<TimelineIndicator, double> IndicatorMinHeightProperty
        = AvaloniaProperty.RegisterDirect<TimelineIndicator, double>(nameof(LineHeightRatio),
            o => o.IndicatorMinHeight,
            (o, v) => o.IndicatorMinHeight = v);

    internal static readonly DirectProperty<TimelineIndicator, IBrush?> DefaultIndicatorColorProperty
        = AvaloniaProperty.RegisterDirect<TimelineIndicator, IBrush?>(nameof(DefaultIndicatorColor),
            o => o.DefaultIndicatorColor,
            (o, v) => o.DefaultIndicatorColor = v);

    internal static readonly DirectProperty<TimelineIndicator, double> IndicatorDotBorderWidthProperty
        = AvaloniaProperty.RegisterDirect<TimelineIndicator, double>(nameof(IndicatorDotBorderWidth),
            o => o.IndicatorDotBorderWidth,
            (o, v) => o.IndicatorDotBorderWidth = v);

    internal static readonly DirectProperty<TimelineIndicator, IBrush?> IndicatorTailColorProperty
        = AvaloniaProperty.RegisterDirect<TimelineIndicator, IBrush?>(nameof(IndicatorTailColor),
            o => o.IndicatorTailColor,
            (o, v) => o.IndicatorTailColor = v);

    internal static readonly DirectProperty<TimelineIndicator, double> IndicatorTailWidthProperty
        = AvaloniaProperty.RegisterDirect<TimelineIndicator, double>(nameof(IndicatorTailWidth),
            o => o.IndicatorTailWidth,
            (o, v) => o.IndicatorTailWidth = v);

    internal static readonly DirectProperty<TimelineIndicator, double> IndicatorDotSizeProperty
        = AvaloniaProperty.RegisterDirect<TimelineIndicator, double>(nameof(IndicatorDotSize),
            o => o.IndicatorDotSize,
            (o, v) => o.IndicatorDotSize = v);

    internal static readonly DirectProperty<TimelineIndicator, bool> NextIsPendingProperty
        = AvaloniaProperty.RegisterDirect<TimelineIndicator, bool>(nameof(NextIsPending),
            o => o.NextIsPending,
            (o, v) => o.NextIsPending = v);

    private double _lineHeightRatio;

    internal double LineHeightRatio
    {
        get => _lineHeightRatio;
        set => SetAndRaise(LineHeightRatioProperty, ref _lineHeightRatio, value);
    }

    private IBrush? _defaultIndicatorColor;

    internal IBrush? DefaultIndicatorColor
    {
        get => _defaultIndicatorColor;
        set => SetAndRaise(DefaultIndicatorColorProperty, ref _defaultIndicatorColor, value);
    }

    private bool _isFirst;

    internal bool IsFirst
    {
        get => _isFirst;
        set => SetAndRaise(IsFirstProperty, ref _isFirst, value);
    }

    private bool _isLast;

    internal bool IsLast
    {
        get => _isLast;
        set => SetAndRaise(IsLastProperty, ref _isLast, value);
    }

    private double _indicatorMinHeight;

    internal double IndicatorMinHeight
    {
        get => _indicatorMinHeight;
        set => SetAndRaise(IndicatorMinHeightProperty, ref _indicatorMinHeight, value);
    }

    private double _indicatorDotBorderWidth;

    internal double IndicatorDotBorderWidth
    {
        get => _indicatorDotBorderWidth;
        set => SetAndRaise(IndicatorDotBorderWidthProperty, ref _indicatorDotBorderWidth, value);
    }

    private IBrush? _indicatorTailColor;

    internal IBrush? IndicatorTailColor
    {
        get => _indicatorTailColor;
        set => SetAndRaise(IndicatorTailColorProperty, ref _indicatorTailColor, value);
    }

    private double _indicatorTailWidth;

    internal double IndicatorTailWidth
    {
        get => _indicatorTailWidth;
        set => SetAndRaise(IndicatorTailWidthProperty, ref _indicatorTailWidth, value);
    }

    private double _indicatorDotSize;

    internal double IndicatorDotSize
    {
        get => _indicatorDotSize;
        set => SetAndRaise(IndicatorDotSizeProperty, ref _indicatorDotSize, value);
    }

    private bool _nextIsPending;

    internal bool NextIsPending
    {
        get => _nextIsPending;
        set => SetAndRaise(NextIsPendingProperty, ref _nextIsPending, value);
    }

    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable => _resourceBindingsDisposable;

    #endregion

    private CompositeDisposable? _resourceBindingsDisposable;

    static TimelineIndicator()
    {
        AffectsMeasure<TimelineIndicator>(IsFirstProperty, IsLastProperty, IndicatorIconProperty);
        AffectsRender<TimelineIndicator>(IndicatorColorProperty);
        TextElement.FontSizeProperty.Changed.AddClassHandler<TimelineIndicator>((indicator, args) =>
        {
            indicator.IndicatorMinHeight = args.GetNewValue<double>() * indicator.LineHeightRatio;
        });
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _resourceBindingsDisposable = new CompositeDisposable();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        this.AddResourceBindingDisposable(
            TokenResourceBinder.CreateTokenBinding(this, LineHeightRatioProperty, SharedTokenKey.LineHeightRatio));
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, IndicatorDotBorderWidthProperty,
            TimelineTokenKey.IndicatorDotBorderWidth));
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, IndicatorDotSizeProperty,
            TimelineTokenKey.IndicatorDotSize));
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, DefaultIndicatorColorProperty,
            SharedTokenKey.ColorPrimary));
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, IndicatorTailWidthProperty,
            TimelineTokenKey.IndicatorTailWidth));
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, IndicatorTailColorProperty,
            TimelineTokenKey.IndicatorTailColor));
        SetupIndicatorIcon();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == LineHeightRatioProperty)
        {
            IndicatorMinHeight = LineHeightRatio * TextElement.GetFontSize(this);
        }
        else if (change.Property == DefaultIndicatorColorProperty ||
                 change.Property == IndicatorColorProperty)
        {
            if (IndicatorIcon != null)
            {
                IndicatorIcon.NormalFilledBrush = IndicatorColor ?? DefaultIndicatorColor;
            }
        }

        if (this.IsAttachedToLogicalTree())
        {
            if (change.Property == IndicatorIconProperty)
            {
                if (change.OldValue is Icon oldIcon)
                {
                    oldIcon.SetLogicalParent(null);
                    LogicalChildren.Remove(oldIcon);
                    VisualChildren.Remove(oldIcon);
                }

                SetupIndicatorIcon();
            }
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size   = base.MeasureOverride(availableSize);
        var height = Math.Max(size.Height, IndicatorMinHeight);
        return new Size(size.Width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (IndicatorIcon is not null)
        {
            var offsetX = (finalSize.Width - IndicatorIcon.DesiredSize.Width) / 2;
            var offsetY = (_indicatorMinHeight - IndicatorIcon.DesiredSize.Height) / 2;
            IndicatorIcon.Arrange(new Rect(offsetX, offsetY, IndicatorIcon.DesiredSize.Width,
                IndicatorIcon.DesiredSize.Height));
        }

        return finalSize;
    }

    private void SetupIndicatorIcon()
    {
        if (IndicatorIcon != null)
        {
            IndicatorIcon.SetLogicalParent(this);
            this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(IndicatorIcon, Icon.WidthProperty,
                TimelineTokenKey.IndicatorWidth));
            this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(IndicatorIcon, Icon.HeightProperty,
                TimelineTokenKey.IndicatorWidth));
            VisualChildren.Add(IndicatorIcon);
            LogicalChildren.Add(IndicatorIcon);
        }
    }

    public override void Render(DrawingContext context)
    {
        var dotBelowLineStartOffsetY = 0d;
        var dotUpLineEndOffsetY      = 0d;
        if (IndicatorIcon is null)
        {
            // 绘制内置的
            var dotPen    = new Pen(IndicatorColor ?? DefaultIndicatorColor, IndicatorDotBorderWidth);
            var dotRadius = IndicatorDotSize / 2;
            var centerX   = (Bounds.Width - IndicatorDotSize) / 2 + dotRadius;
            var centerY   = (IndicatorMinHeight - IndicatorDotSize) / 2 + dotRadius - 1;
            dotBelowLineStartOffsetY = centerY + dotRadius + IndicatorDotBorderWidth / 2;
            dotUpLineEndOffsetY      = centerY - dotRadius - IndicatorDotBorderWidth / 2;
            context.DrawEllipse(null, dotPen, new Point(centerX, centerY), dotRadius, dotRadius);
        }
        else
        {
            dotBelowLineStartOffsetY = IndicatorIcon.Bounds.Bottom;
            dotUpLineEndOffsetY      = IndicatorIcon.Bounds.Top;
        }

        var lineOffsetX = Bounds.Width / 2;
        var linePen     = new Pen(IndicatorTailColor, IndicatorTailWidth);
        if (!IsLast)
        {
            var dotBelowLineStartPoint = new Point(lineOffsetX, dotBelowLineStartOffsetY);
            var dotBelowLineEndPoint   = new Point(lineOffsetX, Bounds.Bottom);
            context.DrawLine(linePen, dotBelowLineStartPoint, dotBelowLineEndPoint);
        }

        if (!IsFirst)
        {
            var dotUpLineStartPoint = new Point(lineOffsetX, 0);
            var dotUpLineEndPoint   = new Point(lineOffsetX, dotUpLineEndOffsetY);
            context.DrawLine(linePen, dotUpLineStartPoint, dotUpLineEndPoint);
        }
    }
}