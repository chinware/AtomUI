using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace AtomUI.Controls;

using AvaloniaButton = Avalonia.Controls.Button;

internal class CaptionButton : AvaloniaButton, IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<Icon?> NormalIconProperty =
        AvaloniaProperty.Register<CaptionButton, Icon?>(nameof(NormalIcon));
    
    public static readonly StyledProperty<Icon?> CheckedIconProperty =
        AvaloniaProperty.Register<CaptionButton, Icon?>(nameof(CheckedIcon));
    
    public static readonly StyledProperty<double> IconWidthProperty =
        AvaloniaProperty.Register<CaptionButton, double>(nameof(IconWidth));

    public static readonly StyledProperty<double> IconHeightProperty = 
        AvaloniaProperty.Register<CaptionButton, double>(nameof(IconHeight));
    
    public static readonly StyledProperty<bool> IsCheckedProperty = 
        AvaloniaProperty.Register<CaptionButton, bool>(nameof(IsChecked), defaultBindingMode: BindingMode.TwoWay, defaultValue:false);
    
    public Icon? NormalIcon
    {
        get => GetValue(NormalIconProperty);
        set => SetValue(NormalIconProperty, value);
    }
    
    public Icon? CheckedIcon
    {
        get => GetValue(CheckedIconProperty);
        set => SetValue(CheckedIconProperty, value);
    }

    public double IconWidth
    {
        get => GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    public double IconHeight
    {
        get => GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, value);
    }
    
    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    #endregion
    
    #region 内部属性定义
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<CaptionButton>();
    
    internal static readonly StyledProperty<bool> IsWindowActiveProperty = 
        TitleBar.IsWindowActiveProperty.AddOwner<CaptionButton>();
    
    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    internal bool IsWindowActive
    {
        get => GetValue(IsWindowActiveProperty);
        set => SetValue(IsWindowActiveProperty, value);
    }
    
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ChromeToken.ID;

    #endregion
    
    public CaptionButton()
    {
        this.RegisterResources();
    }
}