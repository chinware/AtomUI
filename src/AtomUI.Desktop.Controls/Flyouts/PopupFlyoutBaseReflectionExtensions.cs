using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace AtomUI.Desktop.Controls;

using AvaloniaPopup = Avalonia.Controls.Primitives.Popup;

internal static class PopupFlyoutBaseReflectionExtensions
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(PopupFlyoutBase))]
    private static readonly Lazy<FieldInfo> PopupLazyFieldInfo = new(() =>
        typeof(PopupFlyoutBase).GetFieldInfoOrThrow("_popupLazy",
            BindingFlags.Instance | BindingFlags.NonPublic));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(PopupFlyoutBase))]
    private static readonly Lazy<MethodInfo> OnPopupOpenedMethodInfo = new(() =>
        typeof(PopupFlyoutBase).GetMethodInfoOrThrow("OnPopupOpened",
            BindingFlags.Instance | BindingFlags.NonPublic));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(PopupFlyoutBase))]
    private static readonly Lazy<MethodInfo> OnPopupClosedMethodInfo = new(() =>
        typeof(PopupFlyoutBase).GetMethodInfoOrThrow("OnPopupClosed",
            BindingFlags.Instance | BindingFlags.NonPublic));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(PopupFlyoutBase))]
    private static readonly Lazy<MethodInfo> OnPopupClosingMethodInfo = new(() =>
        typeof(PopupFlyoutBase).GetMethodInfoOrThrow("OnPopupClosing",
            BindingFlags.Instance | BindingFlags.NonPublic));

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(PopupFlyoutBase))]
    private static readonly Lazy<MethodInfo> OnPlacementTargetOrPopupKeyUpMethodInfo = new(() =>
        typeof(PopupFlyoutBase).GetMethodInfoOrThrow("OnPlacementTargetOrPopupKeyUp",
            BindingFlags.Instance | BindingFlags.NonPublic));

    public static void SetPopupLazy(this PopupFlyoutBase flyoutBase, Lazy<AvaloniaPopup> popupFactory)
    {
        PopupLazyFieldInfo.Value.SetValue(flyoutBase, popupFactory);
    }

    public static void OnPopupOpened(this PopupFlyoutBase flyoutBase, object? sender, EventArgs e)
    {
        OnPopupOpenedMethodInfo.Value.Invoke(flyoutBase, [sender, e]);
    }

    public static void OnPopupClosed(this PopupFlyoutBase flyoutBase, object? sender, EventArgs e)
    {
        OnPopupClosedMethodInfo.Value.Invoke(flyoutBase, [sender, e]);
    }

    public static void OnPopupClosing(this PopupFlyoutBase flyoutBase, object? sender, CancelEventArgs e)
    {
        OnPopupClosingMethodInfo.Value.Invoke(flyoutBase, [sender, e]);
    }

    public static void OnPlacementTargetOrPopupKeyUp(this PopupFlyoutBase flyoutBase, object? sender, KeyEventArgs e)
    {
        OnPlacementTargetOrPopupKeyUpMethodInfo.Value.Invoke(flyoutBase, [sender, e]);
    }
}
