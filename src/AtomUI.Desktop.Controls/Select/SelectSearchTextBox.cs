using Avalonia.Input;

namespace AtomUI.Controls;

internal class SelectSearchTextBox : TextBox
{
    protected override Type StyleKeyOverride { get; } = typeof(SelectSearchTextBox);
    
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