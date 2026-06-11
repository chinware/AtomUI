using System.Reflection;
using AtomUI.Reflection;
using Avalonia;

namespace AtomUI.Utils;

// 反射扩展类
internal static class AvaloniaPropertyReflectionExtensions
{
    public static void InvokeNotifying(this AvaloniaProperty property, AvaloniaObject target, bool status)
    {
        var notifyingProp  = typeof(AvaloniaProperty).GetPropertyInfoOrThrow("Notifying", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        var notifyingDelegate = notifyingProp.GetValue(property) 
            as Action<AvaloniaObject, bool>;
        if (notifyingDelegate == null)
        {
            return;
        }
        notifyingDelegate.Invoke(target, status);
    }
}
