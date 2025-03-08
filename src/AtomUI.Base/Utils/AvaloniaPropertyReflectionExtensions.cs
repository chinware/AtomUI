using AtomUI.Reflection;
using Avalonia;

namespace AtomUI.Utils;

// 反射扩展类
internal static class AvaloniaPropertyReflectionExtensions
{
    public static void NotifyChanged<TValue>(this AvaloniaProperty<TValue> property, AvaloniaPropertyChangedEventArgs<TValue> e)
    {
        var methodInfo = typeof(AvaloniaProperty<TValue>).GetMethodInfoOrThrow("NotifyChanged");
        methodInfo.Invoke(property, [e]);
    }
}