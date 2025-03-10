using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Rendering;

namespace AtomUI.Controls;

using AvaloniaCheckBox = Avalonia.Controls.CheckBox;

public class CheckBox : AvaloniaCheckBox,
                        ICustomHitTest,
                        IWaveSpiritAwareControl,
                        IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = WaveSpiritAwareControlProperty.IsMotionEnabledProperty.AddOwner<CheckBox>();

    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty
        = WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<CheckBox>();
    
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
    string IControlSharedTokenResourcesHost.TokenId => CheckBoxToken.ID;

    #endregion

    public CheckBox()
    {
        this.RegisterResources();
        this.BindWaveSpiritProperties();
    }

    public bool HitTest(Point point)
    {
        return true;
    }
}