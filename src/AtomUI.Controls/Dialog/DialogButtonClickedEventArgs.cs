namespace AtomUI.Controls;

public class DialogButtonClickedEventArgs : EventArgs
{
    public Button? SourceButton { get; }

    public DialogButtonClickedEventArgs(Button? sourceButton)
    {
        SourceButton = sourceButton;
    }
}