namespace AtomUI.Controls;

public class DialogButtonClickedEventArgs : EventArgs
{
    public DialogButton? SourceButton { get; }

    public DialogButtonClickedEventArgs(DialogButton? sourceButton)
    {
        SourceButton = sourceButton;
    }
}