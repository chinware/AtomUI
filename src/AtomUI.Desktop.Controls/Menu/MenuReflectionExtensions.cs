using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Reflection;

namespace AtomUI.Controls;

using AvaloniaContextMenu = Avalonia.Controls.ContextMenu;

// 反射扩展定义
internal static class MenuReflectionExtensions
{
    #region 反射信息定义
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(AvaloniaContextMenu))]
    private static readonly Lazy<FieldInfo> PopupFieldInfo = new Lazy<FieldInfo>(() =>
        typeof(AvaloniaContextMenu).GetFieldInfoOrThrow("_popup",
            BindingFlags.Instance | BindingFlags.NonPublic));
    
    #endregion

    public static void SetPopup(this AvaloniaContextMenu menu, Popup popup)
    {
        PopupFieldInfo.Value.SetValue(menu, popup);
    }
    
    public static EventHandler<T>? CreateEventHandler<T>(this AvaloniaContextMenu menu, string handlerName)
    {
        var parentType = typeof(AvaloniaContextMenu);
        if (parentType.TryGetMethodInfo(handlerName, out var methodInfo, BindingFlags.NonPublic | BindingFlags.Instance))
        {
            return (EventHandler<T>)Delegate.CreateDelegate(typeof(EventHandler<T>), menu, methodInfo);
        }

        return null;
    }
    
    public static EventHandler? CreateEventHandler(this AvaloniaContextMenu menu, string handlerName)
    {
        var parentType = typeof(ContextMenu);
        if (parentType.TryGetMethodInfo(handlerName, out var methodInfo, BindingFlags.NonPublic | BindingFlags.Instance))
        {
            return (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), menu, methodInfo);
        }

        return null;
    }
}