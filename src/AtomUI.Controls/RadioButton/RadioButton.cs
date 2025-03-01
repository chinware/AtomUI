using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Rendering;

namespace AtomUI.Controls;

using AvaloniaRadioButton = Avalonia.Controls.RadioButton;

public class RadioButton : AvaloniaRadioButton,
                           ICustomHitTest,
                           IAnimationAwareControl,
                           IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AnimationAwareControlProperty.IsMotionEnabledProperty.AddOwner<RadioButton>();

    public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = AnimationAwareControlProperty.IsWaveAnimationEnabledProperty.AddOwner<RadioButton>();
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsWaveAnimationEnabled
    {
        get => GetValue(IsWaveAnimationEnabledProperty);
        set => SetValue(IsWaveAnimationEnabledProperty, value);
    }

    #endregion
    
    #region 内部属性定义

    Control IAnimationAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => RadioButtonToken.ID;

    #endregion
    
    public RadioButton()
    {
        this.RegisterResources();
        this.BindAnimationProperties(IsMotionEnabledProperty, IsWaveAnimationEnabledProperty);
    }
    
    public bool HitTest(Point point)
    {
        return true;
    }
}