using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

public class DefaultTourIndicator : TourIndicator
{
    #region 公共属性定义

    public static readonly StyledProperty<double> IndicatorSizeProperty = 
        AvaloniaProperty.Register<DefaultTourIndicator, double>(nameof(IndicatorSize));
    
    public static readonly StyledProperty<IBrush?> IndicatorColorProperty = 
        AvaloniaProperty.Register<DefaultTourIndicator, IBrush?>(nameof(IndicatorColor));
    
    public static readonly StyledProperty<IBrush?> IndicatorActiveColorProperty = 
        AvaloniaProperty.Register<DefaultTourIndicator, IBrush?>(nameof(IndicatorActiveColor));
    
    public static readonly StyledProperty<double> ItemSpacingProperty = 
        AvaloniaProperty.Register<DefaultTourIndicator, double>(nameof(ItemSpacing));

    public double IndicatorSize
    {
        get => GetValue(IndicatorSizeProperty);
        set => SetValue(IndicatorSizeProperty, value);
    }
        
    public IBrush? IndicatorColor
    {
        get => GetValue(IndicatorColorProperty);
        set => SetValue(IndicatorColorProperty, value);
    }
    
    public IBrush? IndicatorActiveColor
    {
        get => GetValue(IndicatorActiveColorProperty);
        set => SetValue(IndicatorActiveColorProperty, value);
    }
    
    public double ItemSpacing
    {
        get => GetValue(ItemSpacingProperty);
        set => SetValue(ItemSpacingProperty, value);
    }
    #endregion

    static DefaultTourIndicator()
    {
        AffectsRender<DefaultTourIndicator>(IndicatorColorProperty, IndicatorActiveColorProperty);
        AffectsMeasure<DefaultTourIndicator>(ItemSpacingProperty, IndicatorSizeProperty);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var height = IndicatorSize;
        var width  = StepCount * IndicatorSize + ItemSpacing * (StepCount + 1);
        return new Size(width, height);
    }

    public override void Render(DrawingContext context)
    {
        var offsetX = ItemSpacing;
        var offsetY = (DesiredSize.Height - IndicatorSize) / 2;
        for (var i = 0; i < StepCount; i++)
        {
            IBrush? brush = null;
            if (ActiveIndex == i)
            {
                brush = IndicatorActiveColor;
            }
            else
            {
                brush = IndicatorColor;
            }
            context.DrawEllipse(brush, null, new Rect(offsetX, offsetY, IndicatorSize, IndicatorSize));
            offsetX += IndicatorSize + ItemSpacing;
        }
    }
}