using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal class DotBadgeAdorner : Control, IControlCustomStyle
{
    public static readonly DirectProperty<DotBadgeAdorner, DotBadgeStatus?> StatusProperty =
        AvaloniaProperty.RegisterDirect<DotBadgeAdorner, DotBadgeStatus?>(
            nameof(Status),
            o => o.Status,
            (o, v) => o.Status = v);

    public static readonly DirectProperty<DotBadgeAdorner, string?> TextProperty =
        AvaloniaProperty.RegisterDirect<DotBadgeAdorner, string?>(
            nameof(Text),
            o => o.Text,
            (o, v) => o.Text = v);

    public static readonly DirectProperty<DotBadgeAdorner, bool> IsAdornerModeProperty =
        AvaloniaProperty.RegisterDirect<DotBadgeAdorner, bool>(
            nameof(IsAdornerMode),
            o => o.IsAdornerMode,
            (o, v) => o.IsAdornerMode = v);

    internal static readonly StyledProperty<IBrush?> BadgeDotColorProperty =
        AvaloniaProperty.Register<DotBadgeAdorner, IBrush?>(
            nameof(BadgeDotColor));

    internal static readonly StyledProperty<double> DotSizeProperty =
        AvaloniaProperty.Register<DotBadgeAdorner, double>(
            nameof(DotSize));

    internal static readonly StyledProperty<double> StatusSizeProperty =
        AvaloniaProperty.Register<DotBadgeAdorner, double>(
            nameof(StatusSize));

    internal static readonly StyledProperty<IBrush?> BadgeShadowColorProperty =
        AvaloniaProperty.Register<DotBadgeAdorner, IBrush?>(
            nameof(BadgeShadowColor));

    private static readonly StyledProperty<double> BadgeShadowSizeProperty =
        AvaloniaProperty.Register<DotBadgeAdorner, double>(
            nameof(BadgeShadowSize));

    private static readonly StyledProperty<double> BadgeTextMarginInlineProperty =
        AvaloniaProperty.Register<DotBadgeAdorner, double>(
            nameof(BadgeTextMarginInline));

    public static readonly StyledProperty<Point> OffsetProperty =
        AvaloniaProperty.Register<DotBadgeAdorner, Point>(
            nameof(Offset));

    private BoxShadows _boxShadows;
    private readonly IControlCustomStyle _customStyle;

    private bool _initialized;

    private bool _isAdornerMode;

    private DotBadgeStatus? _status;

    private string? _text;

    private Label? _textLabel;

    // 不知道为什么这个值会被 AdornerLayer 重写
    // 非常不优美，但是能工作
    internal RelativePoint? AnimationRenderTransformOrigin;

    static DotBadgeAdorner()
    {
        AffectsMeasure<DotBadge>(TextProperty, IsAdornerModeProperty);
        AffectsRender<DotBadge>(BadgeDotColorProperty, OffsetProperty);
    }

    public DotBadgeAdorner()
    {
        _customStyle = this;
    }

    public DotBadgeStatus? Status
    {
        get => _status;
        set => SetAndRaise(StatusProperty, ref _status, value);
    }

    public string? Text
    {
        get => _text;
        set => SetAndRaise(TextProperty, ref _text, value);
    }

    public bool IsAdornerMode
    {
        get => _isAdornerMode;
        set => SetAndRaise(IsAdornerModeProperty, ref _isAdornerMode, value);
    }

    public Point Offset
    {
        get => GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    public double DotSize
    {
        get => GetValue(DotSizeProperty);
        set => SetValue(DotSizeProperty, value);
    }

    public double StatusSize
    {
        get => GetValue(StatusSizeProperty);
        set => SetValue(StatusSizeProperty, value);
    }

    internal IBrush? BadgeDotColor
    {
        get => GetValue(BadgeDotColorProperty);
        set => SetValue(BadgeDotColorProperty, value);
    }

    internal IBrush? BadgeShadowColor
    {
        get => GetValue(BadgeShadowColorProperty);
        set => SetValue(BadgeShadowColorProperty, value);
    }

    public double BadgeShadowSize
    {
        get => GetValue(BadgeShadowSizeProperty);
        set => SetValue(BadgeShadowSizeProperty, value);
    }

    public double BadgeTextMarginInline
    {
        get => GetValue(BadgeTextMarginInlineProperty);
        set => SetValue(BadgeTextMarginInlineProperty, value);
    }

    void IControlCustomStyle.BuildStyles()
    {
        if (Styles.Count == 0) BuildBadgeColorStyle();
    }

    public sealed override void ApplyTemplate()
    {
        base.ApplyTemplate();
        if (!_initialized)
        {
            _textLabel = new Label
            {
                Content                    = Text,
                HorizontalAlignment        = HorizontalAlignment.Left,
                VerticalAlignment          = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment   = VerticalAlignment.Center,
                Padding                    = new Thickness(0)
            };

            ((ISetLogicalParent)_textLabel).SetParent(this);
            VisualChildren.Add(_textLabel);
            BuildBoxShadow();
            _initialized = true;
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _customStyle.BuildStyles();
    }

    private void BuildBadgeColorStyle()
    {
        var commonStyle = new Style(selector => selector.OfType<DotBadgeAdorner>());
        commonStyle.Add(BadgeTextMarginInlineProperty, GlobalTokenResourceKey.MarginXS);
        commonStyle.Add(BadgeDotColorProperty, BadgeTokenResourceKey.BadgeColor);
        commonStyle.Add(DotSizeProperty, BadgeTokenResourceKey.DotSize);
        commonStyle.Add(StatusSizeProperty, BadgeTokenResourceKey.StatusSize);
        commonStyle.Add(BadgeShadowSizeProperty, BadgeTokenResourceKey.BadgeShadowSize);
        commonStyle.Add(BadgeShadowColorProperty, BadgeTokenResourceKey.BadgeShadowColor);

        var errorStatusStyle =
            new Style(selector => selector.Nesting().PropertyEquals(StatusProperty, DotBadgeStatus.Error));
        errorStatusStyle.Add(BadgeDotColorProperty, GlobalTokenResourceKey.ColorError);
        commonStyle.Add(errorStatusStyle);

        var successStatusStyle =
            new Style(selector => selector.Nesting().PropertyEquals(StatusProperty, DotBadgeStatus.Success));
        successStatusStyle.Add(BadgeDotColorProperty, GlobalTokenResourceKey.ColorSuccess);
        commonStyle.Add(successStatusStyle);

        var warningStatusStyle =
            new Style(selector => selector.Nesting().PropertyEquals(StatusProperty, DotBadgeStatus.Warning));
        warningStatusStyle.Add(BadgeDotColorProperty, GlobalTokenResourceKey.ColorWarning);
        commonStyle.Add(warningStatusStyle);

        var defaultStatusStyle =
            new Style(selector => selector.Nesting().PropertyEquals(StatusProperty, DotBadgeStatus.Default));
        defaultStatusStyle.Add(BadgeDotColorProperty, GlobalTokenResourceKey.ColorTextPlaceholder);
        commonStyle.Add(defaultStatusStyle);

        var processingStatusStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(StatusProperty, DotBadgeStatus.Processing));
        processingStatusStyle.Add(BadgeDotColorProperty, GlobalTokenResourceKey.ColorInfo);
        commonStyle.Add(processingStatusStyle);

        Styles.Add(commonStyle);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var targetWidth  = 0d;
        var targetHeight = 0d;
        if (IsAdornerMode)
        {
            targetWidth  = availableSize.Width;
            targetHeight = availableSize.Height;
        }
        else
        {
            var textSize = base.MeasureOverride(availableSize);
            targetWidth  += StatusSize;
            targetWidth  += textSize.Width;
            targetHeight += Math.Max(textSize.Height, StatusSize);
            if (textSize.Width > 0) targetWidth += BadgeTextMarginInline;
        }

        return new Size(targetWidth, targetHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (!IsAdornerMode)
        {
            double textOffsetX = 0;
            if (IsAdornerMode)
                textOffsetX += DotSize;
            else
                textOffsetX += StatusSize;

            textOffsetX += BadgeTextMarginInline;
            var textRect = new Rect(new Point(textOffsetX, 0), _textLabel!.DesiredSize);
            _textLabel.Arrange(textRect);
        }

        return finalSize;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsAdornerModeProperty)
        {
            var newValue                                     = e.GetNewValue<bool>();
            if (_textLabel is not null) _textLabel.IsVisible = !newValue;
        }
        else if (e.Property == BadgeShadowSizeProperty ||
                 e.Property == BadgeShadowColorProperty)
        {
            BuildBoxShadow();
        }
    }

    public override void Render(DrawingContext context)
    {
        var dotSize = 0d;
        if (IsAdornerMode)
            dotSize = DotSize;
        else
            dotSize = StatusSize;

        var offsetX = 0d;
        var offsetY = 0d;
        if (IsAdornerMode)
        {
            offsetX =  DesiredSize.Width - dotSize / 2;
            offsetY =  -dotSize / 2;
            offsetX -= Offset.X;
            offsetY += Offset.Y;
        }
        else
        {
            offsetY = (DesiredSize.Height - dotSize) / 2;
        }

        var dotRect = new Rect(new Point(offsetX, offsetY), new Size(dotSize, dotSize));

        if (RenderTransform is not null)
        {
            Point origin;
            if (AnimationRenderTransformOrigin.HasValue)
                origin = AnimationRenderTransformOrigin.Value.ToPixels(dotRect.Size);
            else
                origin = RenderTransformOrigin.ToPixels(dotRect.Size);

            var offset          = Matrix.CreateTranslation(new Point(origin.X + offsetX, origin.Y + offsetY));
            var renderTransform = -offset * RenderTransform.Value * offset;
            context.PushTransform(renderTransform);
        }

        context.DrawRectangle(BadgeDotColor, null, dotRect, dotSize, dotSize, _boxShadows);
    }

    private void BuildBoxShadow()
    {
        if (BadgeShadowColor is not null)
            _boxShadows = new BoxShadows(new BoxShadow
            {
                OffsetX = 0,
                OffsetY = 0,
                Blur    = 0,
                Spread  = BadgeShadowSize,
                Color   = ((SolidColorBrush)BadgeShadowColor).Color
            });
    }
}