using Avalonia.Input;

namespace AtomUI.Desktop.Controls;

internal class RowReorderButton : IconButton
{
    protected override Type StyleKeyOverride { get; } = typeof(IconButton);
    
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

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        e.Handled = false;
    }
}