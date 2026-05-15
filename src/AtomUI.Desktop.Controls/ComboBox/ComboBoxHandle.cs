using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls;

internal class ComboBoxHandle : TemplatedControl
{
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ComboBoxHandle>();

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public event EventHandler<RoutedEventArgs>? HandleClick;

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (!e.Handled &&
            IsEnabled &&
            e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
        {
            PseudoClasses.Set(StdPseudoClass.Pressed, true);
            e.Handled = true;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (!e.Handled &&
            IsEnabled &&
            PseudoClasses.Contains(StdPseudoClass.Pressed) &&
            e.InitialPressMouseButton == MouseButton.Left)
        {
            PseudoClasses.Set(StdPseudoClass.Pressed, false);
            HandleClick?.Invoke(this, e);
            e.Handled = true;
            return;
        }

        PseudoClasses.Set(StdPseudoClass.Pressed, false);
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        PseudoClasses.Set(StdPseudoClass.Pressed, false);
    }
}
