using Avalonia;
using Avalonia.Input;

namespace AtomUI.Desktop.Controls;

/// <summary>
/// A checked/unchecked icon button intended for application-owned actions in a window title bar add-on.
/// </summary>
public class WindowTitleBarToggleButton : ToggleIconButton
{
    internal static readonly AttachedProperty<bool> IsWindowActiveProperty =
        WindowTitleBarHostContext.IsWindowActiveProperty.AddOwner<WindowTitleBarToggleButton>(
            new StyledPropertyMetadata<bool>(true));

    internal static readonly AttachedProperty<bool> HostMotionEnabledProperty =
        WindowTitleBarHostContext.HostMotionEnabledProperty.AddOwner<WindowTitleBarToggleButton>(
            new StyledPropertyMetadata<bool>(true));

    internal bool IsWindowActive
    {
        get => GetValue(IsWindowActiveProperty);
        set => SetValue(IsWindowActiveProperty, value);
    }

    internal bool HostMotionEnabled
    {
        get => GetValue(HostMotionEnabledProperty);
        set => SetValue(HostMotionEnabledProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size = base.MeasureOverride(availableSize);
        var side = Math.Min(size.Width, size.Height);
        return new Size(side, side);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        e.Handled = true;
    }
}
