using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

using AvaloniaMenu = Avalonia.Controls.Menu;

public class Menu : AvaloniaMenu, 
                    ISizeTypeAware,
                    IAnimationAwareControl,
                    IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        AvaloniaProperty.Register<Menu, SizeType>(nameof(SizeType), SizeType.Middle);
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AvaloniaProperty.Register<Menu, bool>(nameof(IsMotionEnabled));

    public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = AvaloniaProperty.Register<Menu, bool>(nameof(IsWaveAnimationEnabled));

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

    public bool IsWaveAnimationEnabled
    {
        get => GetValue(IsWaveAnimationEnabledProperty);
        set => SetValue(IsWaveAnimationEnabledProperty, value);
    }

    #endregion
    
    #region 内部属性定义
    
    Control IAnimationAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => MenuToken.ID;

    #endregion

    public Menu()
    {
        this.RegisterResources();
        this.BindAnimationProperties(IsMotionEnabledProperty, IsWaveAnimationEnabledProperty);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        if (ItemContainerTheme is null)
        {
            TokenResourceBinder.CreateTokenBinding(this, ItemContainerThemeProperty, TopLevelMenuItemTheme.ID);
        }
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        if (container is MenuItem menuItem)
        {
            BindUtils.RelayBind(this, SizeTypeProperty, menuItem, MenuItem.SizeTypeProperty);
            BindUtils.RelayBind(this, IsMotionEnabledProperty, menuItem, MenuItem.IsMotionEnabledProperty);
        }

        base.PrepareContainerForItemOverride(container, item, index);
    }
}