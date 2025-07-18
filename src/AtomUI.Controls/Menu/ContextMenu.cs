using System.ComponentModel;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace AtomUI.Controls;

using AvaloniaContextMenu = Avalonia.Controls.ContextMenu;

public class ContextMenu : AvaloniaContextMenu,
                           IMotionAwareControl,
                           IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ContextMenu>();

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    #region 内部属性定义

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => MenuToken.ID;

    #endregion
    
    private Popup? _popup;

    public ContextMenu()
    {
        this.RegisterResources();
        this.BindMotionProperties();
        // 我们在这里有一次初始化的机会
        _popup = new Popup
        {
            WindowManagerAddShadowHint     = false,
            IsLightDismissEnabled          = false,
            OverlayDismissEventPassThrough = true,
            IsDetectMouseClickEnabled      = true
        };
       
        _popup.Opened += this.CreateEventHandler("PopupOpened");
        _popup.Closed += this.CreateEventHandler<EventArgs>("PopupClosed");
        
        _popup.AddClosingEventHandler(this.CreateEventHandler<CancelEventArgs>("PopupClosing")!);
        _popup.KeyUp += this.CreateEventHandler<KeyEventArgs>("PopupKeyUp");
        Closing += (sender, args) =>
        {
            args.Cancel = true;
        };
        this.SetPopup(_popup);
        Opened += (sender, args) =>
        {
            _popup.SetIgnoreIsOpenChanged(true);
            _popup.IsMotionAwareOpen = true;
        };
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        if (container is MenuItem menuItem)
        {
            BindUtils.RelayBind(this, IsMotionEnabledProperty, menuItem, MenuItem.IsMotionEnabledProperty);
        }

        base.PrepareContainerForItemOverride(container, item, index);
    }
    
    public override void Close()
    {
        _popup?.SetIgnoreIsOpenChanged(true);
        base.Close();
        if (_popup != null)
        {
            foreach (var childItem in Items)
            {
                if (childItem is MenuItem menuItem)
                {
                    menuItem.IsSubMenuOpen = false;
                }
            }
            _popup.IsMotionAwareOpen = false;
        }
    }
}