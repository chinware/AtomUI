using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal class ColorBlock : TemplatedControl, ISizeTypeAware
{
    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<ColorBlock>();
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<ColorBlock, IBrush?> TransparentBgBrushProperty =
        AvaloniaProperty.RegisterDirect<ColorBlock, IBrush?>(
            nameof(TransparentBgBrush),
            o => o.TransparentBgBrush,
            (o, v) => o.TransparentBgBrush = v);
    
    public static readonly StyledProperty<IBrush?> TransparentBgIntervalColorProperty =
        AvaloniaProperty.Register<StyledElement, IBrush?>(nameof(TransparentBgIntervalColor));
    
    private IBrush? _transparentBgBrush;

    internal IBrush? TransparentBgBrush
    {
        get => _transparentBgBrush;
        set => SetAndRaise(TransparentBgBrushProperty, ref _transparentBgBrush, value);
    }
    
    public IBrush? TransparentBgIntervalColor
    {
        get => GetValue(TransparentBgIntervalColorProperty);
        set => SetValue(TransparentBgIntervalColorProperty, value);
    }
    #endregion

    static ColorBlock()
    {
        AffectsRender<ColorBlock>(TransparentBgBrushProperty);
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        var size = e.NewSize.Width / 2.0;
        BuildTransparentBgBrush(size);
    }
    
    protected void BuildTransparentBgBrush(double size)
    {
        if (TransparentBgIntervalColor != null && TransparentBgIntervalColor is ISolidColorBrush solidColorBrush)
        {
            var color    = solidColorBrush.Color;
            var destRect = new Rect(0, 0, size, size);
            TransparentBgBrush = new DrawingBrush()
            {
                TileMode = TileMode.Tile,
                Stretch = Stretch.None,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top,
                DestinationRect  = new RelativeRect(0, 0, size, size, RelativeUnit.Absolute),
                Drawing = new GeometryDrawing()
                {
                    Brush = new ConicGradientBrush()
                    {
                        Center = RelativePoint.Center,
                        GradientStops = new GradientStops()
                        {
                            new GradientStop(color, 0.00),
                            new GradientStop(color, 0.25),
                            new GradientStop(Colors.Transparent, 0.25),
                            new GradientStop(Colors.Transparent, 0.50),
                            new GradientStop(color, 0.50),
                            new GradientStop(color, 0.75),
                            new GradientStop(Colors.Transparent, 0.75),
                            new GradientStop(Colors.Transparent, 1.00),
                        }
                    },
                    Geometry = new RectangleGeometry()
                    {
                        Rect = destRect
                    },
                }
            };
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == TransparentBgIntervalColorProperty)
            {
                BuildTransparentBgBrush(DesiredSize.Width / 2.0);
            }
        }
    }
}