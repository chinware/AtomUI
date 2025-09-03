using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

using AvaloniaRadioButton = Avalonia.Controls.RadioButton;

public class RadioButton : AvaloniaRadioButton,
                           IWaveSpiritAwareControl,
                           IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<RadioButton>();

    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty =
        WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<RadioButton>();
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsWaveSpiritEnabled
    {
        get => GetValue(IsWaveSpiritEnabledProperty);
        set => SetValue(IsWaveSpiritEnabledProperty, value);
    }

    #endregion
    
    #region 内部属性定义

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => RadioButtonToken.ID;

    #endregion
    
    public RadioButton()
    {
        this.RegisterResources();
    }
}