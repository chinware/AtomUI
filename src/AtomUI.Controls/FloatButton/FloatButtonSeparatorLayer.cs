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
    
    static FloatButtonSeparatorLayer()
    {
        AffectsRender<FloatButtonSeparatorLayer>(LinesProperty, OrientationProperty, SeparatorBrushProperty);
    }
    
    public override void Render(DrawingContext context)
    {
        if (SeparatorBrush != null && Lines != null)
        {
            var   pen        = new Pen(SeparatorBrush);
            foreach (var line in Lines)
            {
                context.DrawLine(pen, line.Item1, line.Item2);
            }
        }
    }
}