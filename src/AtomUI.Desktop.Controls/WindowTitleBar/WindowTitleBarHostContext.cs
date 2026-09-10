using Avalonia;

namespace AtomUI.Desktop.Controls;

internal sealed class WindowTitleBarHostContext
{
    private WindowTitleBarHostContext()
    {
    }
    public static readonly AttachedProperty<bool> IsWindowActiveProperty =
        AvaloniaProperty.RegisterAttached<WindowTitleBarHostContext, StyledElement, bool>(
            "IsWindowActive",
            defaultValue: true,
            inherits: true);

    public static readonly AttachedProperty<bool> HostMotionEnabledProperty =
        AvaloniaProperty.RegisterAttached<WindowTitleBarHostContext, StyledElement, bool>(
            "HostMotionEnabled",
            defaultValue: true,
            inherits: true);

    public static readonly AttachedProperty<OsType> HostOsTypeProperty =
        AvaloniaProperty.RegisterAttached<WindowTitleBarHostContext, StyledElement, OsType>(
            "HostOsType",
            defaultValue: OsType.Unknown,
            inherits: true);

    public static void SetIsWindowActive(StyledElement element, bool value)
    {
        element.SetValue(IsWindowActiveProperty, value);
    }

    public static bool GetIsWindowActive(StyledElement element)
    {
        return element.GetValue(IsWindowActiveProperty);
    }

    public static void SetHostMotionEnabled(StyledElement element, bool value)
    {
        element.SetValue(HostMotionEnabledProperty, value);
    }

    public static bool GetHostMotionEnabled(StyledElement element)
    {
        return element.GetValue(HostMotionEnabledProperty);
    }

    public static void SetHostOsType(StyledElement element, OsType value)
    {
        element.SetValue(HostOsTypeProperty, value);
    }

    public static OsType GetHostOsType(StyledElement element)
    {
        return element.GetValue(HostOsTypeProperty);
    }
}
