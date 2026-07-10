using AtomUI.Media;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls.Commons;

internal class FloatButtonSeparatorLayer : Control
{
    #region 公共属性定义

    public static readonly StyledProperty<IList<(Point, Point)>?> LinesProperty =
        AvaloniaProperty.Register<FloatButtonSeparatorLayer, IList<(Point, Point)>?>(nameof(Lines));

    public static readonly StyledProperty<Orientation> OrientationProperty =
        StackPanel.OrientationProperty.AddOwner<FloatButtonSeparatorLayer>();
    
    public static readonly StyledProperty<IBrush?> SeparatorBrushProperty =
        AvaloniaProperty.Register<FloatButtonSeparatorLayer, IBrush?>(nameof(SeparatorBrush));
    
    public IList<(Point, Point)>? Lines
    {
        get => GetValue(LinesProperty);
        set => SetValue(LinesProperty, value);
    }
    
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }
    
    public IBrush? SeparatorBrush
    {
        get => GetValue(SeparatorBrushProperty);
        set => SetValue(SeparatorBrushProperty, value);
    }
    #endregion

    private IPen? _separatorPen;
    
    static FloatButtonSeparatorLayer()
    {
        AffectsRender<FloatButtonSeparatorLayer>(
            LinesProperty,
            OrientationProperty,
            SeparatorBrushProperty,
            UseLayoutRoundingProperty);
    }
    
    public override void Render(DrawingContext context)
    {
        if (SeparatorBrush != null && Lines != null)
        {
            var lineWidth = BorderUtils.BuildRenderScaleAwareThickness(this, 1.0);
            PenUtils.TryModifyOrCreate(ref _separatorPen, SeparatorBrush, lineWidth);
            if (_separatorPen is null)
            {
                return;
            }
            foreach (var line in Lines)
            {
                context.DrawLine(_separatorPen, line.Item1, line.Item2);
            }
        }
    }
}
