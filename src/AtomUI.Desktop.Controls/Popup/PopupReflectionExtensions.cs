using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

using AvaloniaPopup = Avalonia.Controls.Primitives.Popup;

internal static class PopupReflectionExtensions
{
    #region 反射信息定义

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicEvents, typeof(AvaloniaPopup))]
    private static readonly Lazy<EventInfo> ClosingEventInfo = new Lazy<EventInfo>(() =>
        typeof(AvaloniaPopup).GetEventInfoOrThrow("Closing", BindingFlags.NonPublic | BindingFlags.Instance));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(AvaloniaPopup))]
    private static readonly Lazy<FieldInfo> IgnoreIsOpenChangedFieldInfo = new Lazy<FieldInfo>(() =>
        typeof(AvaloniaPopup).GetFieldInfoOrThrow("_ignoreIsOpenChanged",
            BindingFlags.Instance | BindingFlags.NonPublic));
    
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(AvaloniaPopup))]
    private static readonly Lazy<MethodInfo> SetPopupParentMethodInfo = new Lazy<MethodInfo>(() =>
        typeof(AvaloniaPopup).GetMethodInfoOrThrow("SetPopupParent",
            BindingFlags.Instance | BindingFlags.NonPublic));

    #endregion
    
    public static void AddClosingEventHandler(this AvaloniaPopup popup, EventHandler<CancelEventArgs> handler)
    {
        var closingEventAddMethod = ClosingEventInfo.Value.GetAddMethod(true);
        closingEventAddMethod?.Invoke(popup, [handler]);
    }

    public static void RemoveClosingEventHandler(this AvaloniaPopup popup, EventHandler<CancelEventArgs> handler)
    {
        var closingEventRemoveMethod = ClosingEventInfo.Value.GetRemoveMethod(true);
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
    
    public static void SetPopupParent(this AvaloniaPopup popup, Control? newParent)
    {
        SetPopupParentMethodInfo.Value.Invoke(popup, [newParent]);
    }
}