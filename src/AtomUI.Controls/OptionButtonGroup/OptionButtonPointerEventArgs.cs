using AtomUI.Controls.Commons;

namespace AtomUI.Controls;

public class OptionButtonPointerEventArgs : EventArgs
{
    public AbstractOptionButton? Button { get; }
    public bool IsPressed { get; set; }
    public bool IsHovering { get; set; }

    public OptionButtonPointerEventArgs(AbstractOptionButton button)
    {
        Button = button;
    }
}