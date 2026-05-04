using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia.Input;

namespace AtomUI.Desktop.Controls;

using AvaloniaContextMenu = Avalonia.Controls.ContextMenu;

internal static class ContextMenuReflectionExtensions
{
    #region 反射信息定义

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(AvaloniaContextMenu))]
    private static readonly Lazy<FieldInfo> PopupFieldInfo = new(() =>
        typeof(AvaloniaContextMenu).GetFieldInfoOrThrow("_popup",
            BindingFlags.Instance | BindingFlags.NonPublic));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(AvaloniaContextMenu))]
    private static readonly Lazy<MethodInfo> PopupOpenedMethodInfo = new(() =>
        typeof(AvaloniaContextMenu).GetMethodInfoOrThrow("PopupOpened",
            BindingFlags.Instance | BindingFlags.NonPublic));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(AvaloniaContextMenu))]
    private static readonly Lazy<MethodInfo> PopupClosedMethodInfo = new(() =>
        typeof(AvaloniaContextMenu).GetMethodInfoOrThrow("PopupClosed",
            BindingFlags.Instance | BindingFlags.NonPublic));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(AvaloniaContextMenu))]
    private static readonly Lazy<MethodInfo> PopupClosingMethodInfo = new(() =>
        typeof(AvaloniaContextMenu).GetMethodInfoOrThrow("PopupClosing",
            BindingFlags.Instance | BindingFlags.NonPublic));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(AvaloniaContextMenu))]
    private static readonly Lazy<MethodInfo> PopupKeyUpMethodInfo = new(() =>
        typeof(AvaloniaContextMenu).GetMethodInfoOrThrow("PopupKeyUp",
            BindingFlags.Instance | BindingFlags.NonPublic));

    #endregion

    public static void SetPopup(this AvaloniaContextMenu contextMenu, Popup popup)
    {
        PopupFieldInfo.Value.SetValue(contextMenu, popup);
    }
    
    public static void OnPopupOpened(this AvaloniaContextMenu contextMenu, object? sender, EventArgs e)
    {
        PopupOpenedMethodInfo.Value.Invoke(contextMenu, [sender, e]);
    }

    public static void OnPopupClosed(this AvaloniaContextMenu contextMenu, object? sender, EventArgs e)
    {
        PopupClosedMethodInfo.Value.Invoke(contextMenu, [sender, e]);
    }

    public static void OnPopupClosing(this AvaloniaContextMenu contextMenu, object? sender, CancelEventArgs e)
    {
        PopupClosingMethodInfo.Value.Invoke(contextMenu, [sender, e]);
    }
    
    public static void OnPopupClosing(this AvaloniaContextMenu contextMenu, object? sender, KeyEventArgs e)
    {
        PopupKeyUpMethodInfo.Value.Invoke(contextMenu, [sender, e]);
    }
}
