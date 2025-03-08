using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Point = Avalonia.Point;

namespace AtomUI.Controls;

using AvaloniaPopup = Avalonia.Controls.Primitives.Popup;

internal static class PopupReflectionExtensions
{
    #region 反射信息定义

    private static readonly Lazy<MethodInfo> SetPopupParentMethodInfo = new Lazy<MethodInfo>(() =>
        typeof(Popup).GetMethodInfoOrThrow("SetPopupParent",
            BindingFlags.Instance | BindingFlags.NonPublic));

    private static readonly Lazy<EventInfo> ClosingEventInfo = new Lazy<EventInfo>(() =>
        typeof(Popup).GetEventInfoOrThrow("Closing", BindingFlags.NonPublic | BindingFlags.Instance));

    private static readonly Lazy<FieldInfo> IgnoreIsOpenChangedFieldInfo = new Lazy<FieldInfo>(() =>
        typeof(AvaloniaPopup).GetFieldInfoOrThrow("_ignoreIsOpenChanged",
            BindingFlags.Instance | BindingFlags.NonPublic));

    #endregion

    public static void MoveAndResize(this Popup popup, Point devicePoint, Size virtualSize)
    {
        if (popup.Host is PopupRoot popupRoot)
        {
            if (popupRoot.PlatformImpl?.PopupPositioner is ManagedPopupPositioner managedPopupPositioner)
            {
                var managedPopupPositionerPopup = managedPopupPositioner.GetManagedPopupPositionerPopup();
                managedPopupPositionerPopup.MoveAndResize(devicePoint, virtualSize);
            }
        }
    }

    public static void SetPopupParent(this Popup popup, Control? newParent)
    {
        SetPopupParentMethodInfo.Value.Invoke(popup, [newParent]);
    }

    public static void AddClosingEventHandler(this Popup popup, EventHandler<CancelEventArgs> handler)
    {
        var closingEventAddMethod = ClosingEventInfo.Value.GetAddMethod();
        closingEventAddMethod?.Invoke(popup, [handler]);
    }

    public static void RemoveClosingEventHandler(this Popup popup, EventHandler<CancelEventArgs> handler)
    {
        var closingEventRemoveMethod = ClosingEventInfo.Value.GetRemoveMethod();
        closingEventRemoveMethod?.Invoke(popup, [handler]);
    }

    public static void SetIgnoreIsOpenChanged(this AvaloniaPopup popup, bool value)
    {
        IgnoreIsOpenChangedFieldInfo.Value.SetValue(popup, value);
    }

    public static bool GetIgnoreIsOpenChanged(this AvaloniaPopup popup)
    {
        var value = IgnoreIsOpenChangedFieldInfo.Value.GetValue(popup) as bool?;
        Debug.Assert(value != null);
        return value.Value;
    }
}