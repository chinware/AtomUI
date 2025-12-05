using AtomUI.Controls;
using AtomUI.Theme;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

public class Rate : TemplatedControl, 
                    IMotionAwareControl, 
                    ISizeTypeAware,
                    IControlSharedTokenResourcesHost
{
    #region 公共属性定义
    
    public static readonly StyledProperty<bool> IsAllowClearProperty =
        AvaloniaProperty.Register<Rate, bool>(nameof(IsAllowClear));
    
    public static readonly StyledProperty<bool> IsAllowHalfProperty =
        AvaloniaProperty.Register<Rate, bool>(nameof(IsAllowHalf));
    
    public static readonly StyledProperty<object?> CharacterProperty =
        AvaloniaProperty.Register<Rate, object?>(nameof(Character));
    
    public static readonly StyledProperty<int> CountProperty =
        AvaloniaProperty.Register<Rate, int>(nameof(Count));
    
    public static readonly StyledProperty<int> ValueProperty =
        AvaloniaProperty.Register<Rate, int>(nameof(Value));
    
    public static readonly StyledProperty<int> DefaultValueProperty =
        AvaloniaProperty.Register<Rate, int>(nameof(DefaultValue));
    
    public static readonly StyledProperty<bool> IsKeyboardEnabledProperty =
        AvaloniaProperty.Register<Rate, bool>(nameof(IsKeyboardEnabled));

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<Rate>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Rate>();
    
    public bool IsAllowClear
    {
        get => GetValue(IsAllowClearProperty);
        set => SetValue(IsAllowClearProperty, value);
    }
    
    public bool IsAllowHalf
    {
        get => GetValue(IsAllowHalfProperty);
        set => SetValue(IsAllowHalfProperty, value);
    }
    
    public object? Character
    {
        get => GetValue(CharacterProperty);
        set => SetValue(CharacterProperty, value);
    }
    
    public int Count
    {
        get => GetValue(CountProperty);
        set => SetValue(CountProperty, value);
    }
    
    public int Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    
    public int DefaultValue
    {
        get => GetValue(DefaultValueProperty);
        set => SetValue(DefaultValueProperty, value);
    }

    public bool IsKeyboardEnabled
    {
        get => GetValue(IsKeyboardEnabledProperty);
        set => SetValue(IsKeyboardEnabledProperty, value);
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
    #endregion

    #region 公共事件定义

    public event EventHandler<RateValueChangedEventArgs>? ValueChanged;
    public event EventHandler<RateValueChangedEventArgs>? HoverValueChanged;

    #endregion
    
    #region 内部属性定义
    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => RateToken.ID;
    #endregion
    
    public Rate()
    {
        this.RegisterResources();
    }
}