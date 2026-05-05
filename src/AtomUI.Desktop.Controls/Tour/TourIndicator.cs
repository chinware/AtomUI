using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

public abstract class TourIndicator : TemplatedControl, IMotionAwareControl
{
    #region 公共属性定义
    
    public static readonly StyledProperty<int> StepCountProperty = 
        AvaloniaProperty.Register<TourIndicator, int>(nameof(StepCount));
    
    public static readonly StyledProperty<int> ActiveIndexProperty = 
        AvaloniaProperty.Register<TourIndicator, int>(nameof(ActiveIndex));

    public static readonly StyledProperty<TourStyleType> StyleTypeProperty =
        Tour.StyleTypeProperty.AddOwner<TourIndicator>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<TourIndicator>();

    public int StepCount
    {
        get => GetValue(StepCountProperty);
        set => SetValue(StepCountProperty, value);
    }
    
    public int ActiveIndex
    {
        get => GetValue(ActiveIndexProperty);
        set => SetValue(ActiveIndexProperty, value);
    }
    
    public TourStyleType StyleType
    {
        get => GetValue(StyleTypeProperty);
        set => SetValue(StyleTypeProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    #endregion

    static TourIndicator()
    {
        AffectsRender<TourIndicator>(ActiveIndexProperty);
        AffectsMeasure<TourIndicator>( StepCountProperty);
    }
}