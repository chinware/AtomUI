using AtomUI.Controls.MessageBox;

namespace AtomUI.Controls;

public class DialogButtonClickedEventArgs : EventArgs
{
    public Button? SourceButton { get; }
    public DialogStandardButton StandardButton { get; }

    public DialogButtonClickedEventArgs(Button? sourceButton, DialogStandardButton standardButton)
    {
        SourceButton = sourceButton;
        StandardButton = standardButton;
    }
}