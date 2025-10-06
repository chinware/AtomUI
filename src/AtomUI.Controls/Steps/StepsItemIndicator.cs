using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

internal class StepsItemIndicator : TemplatedControl
{
    #region 公共属性定义

    public static readonly StyledProperty<StepsItemStatus> StatusProperty =
        AvaloniaProperty.Register<StepsItemIndicator, StepsItemStatus>(nameof(Status), StepsItemStatus.Process);
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<StepsItemIndicator>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<StepsItemIndicator>();
    
    public static readonly StyledProperty<StepsItemIndicatorType> IndicatorTypeProperty =
        StepsItem.IndicatorTypeProperty.AddOwner<StepsItemIndicator>();
    
    public StepsItemStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public StepsItemIndicatorType IndicatorType
    {
        get => GetValue(IndicatorTypeProperty);
        set => SetValue(IndicatorTypeProperty, value);
    }

    #endregion
    
    #region 内部属性定义

    internal static readonly DirectProperty<StepsItemIndicator, int> PositionProperty =
        AvaloniaProperty.RegisterDirect<StepsItemIndicator, int>(
            nameof(Position),
            o => o.Position,
            (o, v) => o.Position = v);

    private int _position;

    internal int Position
    {
        get => _position;
        set => SetAndRaise(PositionProperty, ref _position, value);
    }
    #endregion

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        SetCurrentValue(CornerRadiusProperty, new CornerRadius(e.NewSize.Width));
    }
}