using System.Reflection;
using System.Runtime.Loader;
using Avalonia.Controls;

namespace AtomUI.Platform.Windows;

internal static class WindowExt
{
    public const uint WS_EX_TRANSPARENT = 32; // 0x00000020
    public const uint WS_EX_LAYERED = 0x00080000;
    private static readonly Type WindowImplType;
    private static readonly MethodInfo GetExtendedStyleInfo;
    private static readonly MethodInfo SetExtendedStyleInfo;

    static WindowExt()
    {
        var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName("Avalonia.Win32"));
        WindowImplType = assembly.GetType("Avalonia.Win32.WindowImpl")!;
        GetExtendedStyleInfo =
            WindowImplType.GetMethod("GetExtendedStyle", BindingFlags.Instance | BindingFlags.NonPublic)!;
        SetExtendedStyleInfo =
            WindowImplType.GetMethod("SetExtendedStyle", BindingFlags.Instance | BindingFlags.NonPublic)!;
    }

    public static void SetTransparentForMouseEvents(this WindowBase window, bool flag)
    {
        var impl          = window.PlatformImpl!;
        var currentStyles = GetExtendedStyle(impl);

        // 不是确定这样处理是否合适
        if (flag)
            currentStyles |= WS_EX_TRANSPARENT | WS_EX_LAYERED;
        else
            currentStyles &= ~(WS_EX_TRANSPARENT | WS_EX_LAYERED);
        SetExtendedStyle(impl, currentStyles, false);
    }

    public static void SetTransparentForMouseEvents(this Window window, bool flag)
    {
        var impl          = window.PlatformImpl!;
        var currentStyles = GetExtendedStyle(impl);

        // 不是确定这样处理是否合适
        if (flag)
            currentStyles |= WS_EX_TRANSPARENT | WS_EX_LAYERED;
        else
            currentStyles &= ~(WS_EX_TRANSPARENT | WS_EX_LAYERED);
        SetExtendedStyle(impl, currentStyles, false);
    }

    private static uint GetExtendedStyle(object instance)
    {
        return (uint)GetExtendedStyleInfo.Invoke(instance, new object[] { })!;
    }

    private static void SetExtendedStyle(object instance, uint styles, bool save)
    {
        SetExtendedStyleInfo.Invoke(instance, new object[] { styles, save });
    }

    public static bool IsTransparentForMouseEvents(this WindowBase window)
    {
        var impl   = window.PlatformImpl!;
        var styles = GetExtendedStyle(impl);
        return (styles & WS_EX_TRANSPARENT) != 0;
    }
}