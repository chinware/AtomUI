using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace AtomUI.Controls;

internal class SelectResultOptionsBox : TemplatedControl
{
    public static readonly StyledProperty<SelectMode> ModeProperty =
        AvaloniaProperty.Register<SelectResultOptionsBox, SelectMode>(nameof(Mode));
    
    public SelectMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }
    
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        e.Handled = false;
    }
    
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        e.Handled = false;
    }
}