using AtomUI.Controls;

namespace AtomUI.Desktop.Controls;

internal class EmbeddedTextBox : TextBox
{
    public EmbeddedTextBox()
    {
        SetCurrentValue(StyleVariantProperty, InputControlStyleVariant.Borderless);
    }
}
