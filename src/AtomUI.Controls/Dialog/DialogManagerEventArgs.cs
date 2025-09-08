namespace AtomUI.Controls;

public class DialogManagerEventArgs : EventArgs
{
    public IDialog Dialog { get; set; }

    public DialogManagerEventArgs(IDialog dialog)
    {
        Dialog = dialog;
    }
    
    public delegate void DialogManagerEventHandler(object? sender, DialogManagerEventArgs args);
}