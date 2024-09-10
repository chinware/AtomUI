using System.ComponentModel;
using System.Reflection;
using AtomUI.Reflection;
using AtomUI.Theme.Styling;
using Avalonia.Input;
using Avalonia.Media;

namespace AtomUI.Controls;

using AvaloniaContextMenu = Avalonia.Controls.ContextMenu;

public class ContextMenu : AvaloniaContextMenu, IControlCustomStyle
{
    private static readonly FieldInfo PopupFieldInfo;
    private static readonly EventInfo ClosingEventInfo;
    private readonly IControlCustomStyle _customStyle;

    static ContextMenu()
    {
        PopupFieldInfo = typeof(AvaloniaContextMenu).GetField("_popup",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)!;
        ClosingEventInfo = typeof(Popup).GetEvent("Closing", BindingFlags.NonPublic | BindingFlags.Instance)!;
    }

    public ContextMenu()
    {
        _customStyle = this;
        Background   = new SolidColorBrush(Colors.Transparent);

        // 我们在这里有一次初始化的机会
        var popup = new Popup
        {
            WindowManagerAddShadowHint     = false,
            IsLightDismissEnabled          = true,
            OverlayDismissEventPassThrough = true
        };
        popup.Opened += CreateEventHandler("PopupOpened");
        popup.Closed += CreateEventHandler<EventArgs>("PopupClosed");

        // popup.Closing += PopupClosing;

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
}