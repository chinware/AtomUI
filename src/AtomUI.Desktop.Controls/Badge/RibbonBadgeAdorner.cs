using AtomUI.Controls;
using AtomUI.Desktop.Controls.Themes;
using AtomUI.Media;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal class RibbonBadgeAdorner : TemplatedControl
{
    #region 内部属性定义

    internal static readonly StyledProperty<IBrush?> RibbonColorProperty =
        AvaloniaProperty.Register<RibbonBadgeAdorner, IBrush?>(
            nameof(RibbonColor));

    internal static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<RibbonBadgeAdorner, string?>(
            nameof(Text));

    internal static readonly StyledProperty<RibbonBadgePlacement> PlacementProperty =
        AvaloniaProperty.Register<RibbonBadgeAdorner, RibbonBadgePlacement>(
            nameof(Placement));

    internal static readonly StyledProperty<Point> OffsetProperty =
        AvaloniaProperty.Register<RibbonBadgeAdorner, Point>(
            nameof(Offset));

    internal static readonly StyledProperty<Point> BadgeRibbonOffsetProperty =
        AvaloniaProperty.Register<RibbonBadgeAdorner, Point>(
            nameof(BadgeRibbonOffset));

    internal static readonly StyledProperty<int> BadgeRibbonCornerDarkenAmountProperty =
        AvaloniaProperty.Register<RibbonBadgeAdorner, int>(
            nameof(BadgeRibbonCornerDarkenAmount));

    internal static readonly StyledProperty<ImmutableTransform?> BadgeRibbonCornerTransformProperty =
        AvaloniaProperty.Register<RibbonBadgeAdorner, ImmutableTransform?>(
            nameof(BadgeRibbonCornerTransform));

    internal static readonly DirectProperty<RibbonBadgeAdorner, bool> IsAdornerModeProperty =
        AvaloniaProperty.RegisterDirect<RibbonBadgeAdorner, bool>(
            nameof(IsAdornerMode),
            o => o.IsAdornerMode,
            (o, v) => o.IsAdornerMode = v);
    
    internal IBrush? RibbonColor
    {
        get => GetValue(RibbonColorProperty);
        set => SetValue(RibbonColorProperty, value);
    }

    internal string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    
    private bool _isAdornerMode;

    internal bool IsAdornerMode
    {
        get => _isAdornerMode;
        set => SetAndRaise(IsAdornerModeProperty, ref _isAdornerMode, value);
    }

    internal RibbonBadgePlacement Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    internal Point Offset
    {
        get => GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    internal Point BadgeRibbonOffset
    {
        get => GetValue(BadgeRibbonOffsetProperty);
        set => SetValue(BadgeRibbonOffsetProperty, value);
    }

    internal int BadgeRibbonCornerDarkenAmount
    {
        get => GetValue(BadgeRibbonCornerDarkenAmountProperty);
        set => SetValue(BadgeRibbonCornerDarkenAmountProperty, value);
    }

    internal ImmutableTransform? BadgeRibbonCornerTransform
    {
        get => GetValue(BadgeRibbonCornerTransformProperty);
        set => SetValue(BadgeRibbonCornerTransformProperty, value);
    }
    
    #endregion
    
    private TextBlock? _labelText;
    private Geometry? _cornerGeometry;
    private readonly BorderRenderHelper _borderRenderHelper;

    static RibbonBadgeAdorner()
    {
        AffectsMeasure<RibbonBadgeAdorner>(TextProperty, IsAdornerModeProperty);
        AffectsMeasure<RibbonBadgeAdorner>(PlacementProperty);
        AffectsRender<RibbonBadgeAdorner>(RibbonColorProperty, OffsetProperty);
    }

    public RibbonBadgeAdorner()
    {
        _borderRenderHelper = new BorderRenderHelper();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _labelText = e.NameScope.Find<TextBlock>(RibbonBadgeAdornerThemeConstants.TextPartPart);
        BuildCornerGeometry();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size         = base.MeasureOverride(availableSize);
        var targetWidth  = availableSize.Width;
        var targetHeight = availableSize.Height;
        if (!IsAdornerMode)
        {
            targetHeight = size.Height + _cornerGeometry?.Bounds.Height ?? 0;
            targetWidth  = size.Width;
        }

        return new Size(targetWidth, targetHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (_labelText is not null)
        {
            _labelText.Arrange(GetTextRect());
        }

        return finalSize;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (this.IsAttachedToVisualTree())
        {
            if (e.Property == PlacementProperty || e.Property == BadgeRibbonOffsetProperty)
            {
                BuildCornerGeometry(true);
            }
        }
    }

    public override void Render(DrawingContext context)
    {
        var backgroundBrush = RibbonColor as ISolidColorBrush;
        {
            var       textRect = GetTextRect();
            using var state    = context.PushTransform(Matrix.CreateTranslation(textRect.X, textRect.Y));

            _borderRenderHelper.Render(context,
                borderThickness: new Thickness(0),
                backgroundSizing: BackgroundSizing.OuterBorderEdge,
                finalSize: textRect.Size,
                cornerRadius: new CornerRadius(CornerRadius.TopLeft,
                    CornerRadius.TopRight,
                    bottomLeft: Placement == RibbonBadgePlacement.Start
                        ? 0
                        : CornerRadius.BottomLeft,
                    bottomRight: Placement == RibbonBadgePlacement.End
                        ? 0
                        : CornerRadius.BottomRight),
                background: backgroundBrush,
                borderBrush: null,
                boxShadows: new BoxShadows());
        }
        {
            var       cornerRect      = GetCornerRect();
            using var state           = context.PushTransform(Matrix.CreateTranslation(cornerRect.X, cornerRect.Y));
            var       backgroundColor = backgroundBrush?.Color;
            var cornerBrush = backgroundColor.HasValue
                ? new SolidColorBrush(backgroundColor.Value.Darken(BadgeRibbonCornerDarkenAmount))
                : default;
            context.DrawGeometry(cornerBrush, null, _cornerGeometry!);
        }
    }

    private Rect GetTextRect()
    {
        if (_labelText is null)
        {
            return default;
        }

        var offsetX = 0d;
        var offsetY = 0d;
        if (IsAdornerMode)
        {
            offsetY += BadgeRibbonOffset.Y;
            if (Placement == RibbonBadgePlacement.End)
            {
                offsetX = DesiredSize.Width - _labelText.DesiredSize.Width + BadgeRibbonOffset.X;
            }
            else
            {
                offsetX = -BadgeRibbonOffset.X;
            }
        }

        return new Rect(new Point(offsetX, offsetY), _labelText.DesiredSize);
    }

    private Rect GetCornerRect()
    {
        if (_cornerGeometry is null)
        {
            return default;
        }

        var targetWidth  = _cornerGeometry.Bounds.Width;
        var targetHeight = _cornerGeometry.Bounds.Height;
        var offsetX      = 0d;
        var offsetY      = 0d;
        if (!IsAdornerMode)
        {
            offsetY = DesiredSize.Height - targetHeight;
            if (Placement == RibbonBadgePlacement.End)
            {
                offsetX = DesiredSize.Width - targetWidth;
            }
        }
        else
        {
            var textRect = GetTextRect();
            if (Placement == RibbonBadgePlacement.End)
            {
                offsetX = textRect.Right - targetWidth;
            }

            offsetY = textRect.Bottom;
        }

        return new Rect(new Point(offsetX, offsetY), new Size(targetWidth, targetHeight));
    }

    private void BuildCornerGeometry(bool force = false)
    {
        if (force || _cornerGeometry is null)
        {
            var       width          = BadgeRibbonOffset.X;
            var       height         = BadgeRibbonOffset.Y;
            var       geometryStream = new StreamGeometry();
            using var context        = geometryStream.Open();
            var       p1             = new Point(0, 0);
            var       p2             = new Point(0, height);
            var       p3             = new Point(width, 0);
            context.LineTo(p1, true);
            context.LineTo(p2);
            context.LineTo(p3);
            context.EndFigure(true);
            _cornerGeometry = geometryStream;
            var transforms = new TransformGroup();
            if (BadgeRibbonCornerTransform is not null)
            {
                transforms.Children.Add(new MatrixTransform(BadgeRibbonCornerTransform.Value));
            }

            if (Placement == RibbonBadgePlacement.Start)
            {
                transforms.Children.Add(new ScaleTransform(-1, 1));
            }

            _cornerGeometry.Transform = transforms;
        }
    }
}