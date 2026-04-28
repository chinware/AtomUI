using System.Runtime.Versioning;
using AtomUI.Native;
using Avalonia;

namespace AtomUI.Desktop.Controls;

[SupportedOSPlatform("macos")]
public class MacStandardWindowButtons : AvaloniaObject
{
    public static readonly AttachedProperty<double> SpacingProperty =
        AvaloniaProperty.RegisterAttached<MacStandardWindowButtons, Window, double>("Spacing", 0);
    
    public static readonly AttachedProperty<double> OffsetXProperty =
        AvaloniaProperty.RegisterAttached<MacStandardWindowButtons, Window, double>("OffsetX", 0);
    
    public static double GetSpacing(Window window)
    {
        return window.GetValue(SpacingProperty);
    }

    public static void SetSpacing(Window window, double value)
    {
        window.SetValue(SpacingProperty, value);
    }
    
    public static double GetOffsetX(Window window)
    {
        return window.GetValue(OffsetXProperty);
    }

    public static void SetOffsetX(Window window, double value)
    {
        window.SetValue(OffsetXProperty, value);
    }

    public static void SetStandardWindowButtonsLayout(Window window, double? height, double? x, double? y, double spacing = 20)
    {
        var originPosition = window.GetStandardWindowButtonsOriginalPosition();
        var effectX = x ?? originPosition.X;
        var originTitleHeight = window.GetSystemTitleBarHeight();
        var effectHeight = height ?? originTitleHeight ?? 0;
        var effectY      = y ?? window.CalculateVerticalCenter(effectHeight) ?? originPosition.Y;
        window.SetStandardWindowButtonsLayout(effectX, effectY, spacing);
    }
}