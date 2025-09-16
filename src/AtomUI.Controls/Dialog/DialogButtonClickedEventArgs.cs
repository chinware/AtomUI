namespace AtomUI.Controls;

public class DialogButtonClickedEventArgs : EventArgs
{
    public DialogButton SourceButton { get; }
    
    public bool Handled { get; set; }

    public DialogButtonClickedEventArgs(DialogButton sourceButton)
    {
        SourceButton = sourceButton;
    }
}