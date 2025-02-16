using System.ComponentModel;
using System.Reflection;
using AtomUI.Data;
using AtomUI.Reflection;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace AtomUI.Controls;

using AvaloniaContextMenu = Avalonia.Controls.ContextMenu;

public class ContextMenu : AvaloniaContextMenu,
                           IAnimationAwareControl,
                           IControlSharedTokenResourcesHost
{
    private static readonly FieldInfo PopupFieldInfo;
    private static readonly EventInfo ClosingEventInfo;

    #region 公共属性定义

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AvaloniaProperty.Register<ContextMenu, bool>(nameof(IsMotionEnabled), true);

    public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = AvaloniaProperty.Register<ContextMenu, bool>(nameof(IsWaveAnimationEnabled), true);
    
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

    static ContextMenu()
    {
        PopupFieldInfo = typeof(AvaloniaContextMenu).GetField("_popup",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)!;
        ClosingEventInfo = typeof(Popup).GetEvent("Closing", BindingFlags.NonPublic | BindingFlags.Instance)!;
    }

    public ContextMenu()
    {
        this.RegisterResources();
        this.BindAnimationProperties(IsMotionEnabledProperty, IsWaveAnimationEnabledProperty);
        // 我们在这里有一次初始化的机会
        var popup = new Popup
        {
            WindowManagerAddShadowHint     = false,
            IsLightDismissEnabled          = true,
            OverlayDismissEventPassThrough = true
        };
        BindUtils.RelayBind(this, IsMotionEnabledProperty, popup, Popup.IsMotionEnabledProperty);
        popup.Opened += CreateEventHandler("PopupOpened");
        popup.Closed += CreateEventHandler<EventArgs>("PopupClosed");

        var closingEventAddMethod = ClosingEventInfo.GetAddMethod(true);
        closingEventAddMethod?.Invoke(popup, new object?[] { CreateEventHandler<CancelEventArgs>("PopupClosing") });

        popup.KeyUp += CreateEventHandler<KeyEventArgs>("PopupKeyUp");
        PopupFieldInfo.SetValue(this, popup);
    }

    private EventHandler<T>? CreateEventHandler<T>(string methodName)
    {
        var parentType = typeof(AvaloniaContextMenu);
        if (parentType.TryGetMethodInfo(methodName, out var methodInfo, BindingFlags.NonPublic | BindingFlags.Instance))
        {
            return (EventHandler<T>)Delegate.CreateDelegate(typeof(EventHandler<T>), this, methodInfo);
        }

        return null;
    }

    private EventHandler? CreateEventHandler(string methodName)
    {
        var parentType = typeof(ContextMenu);
        if (parentType.TryGetMethodInfo(methodName, out var methodInfo, BindingFlags.NonPublic | BindingFlags.Instance))
        {
            return (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), this, methodInfo);
        }

        return null;
    }
    
    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        if (container is MenuItem menuItem)
        {
            BindUtils.RelayBind(this, IsMotionEnabledProperty, menuItem, MenuItem.IsMotionEnabledProperty);
        }

        base.PrepareContainerForItemOverride(container, item, index);
    }
}