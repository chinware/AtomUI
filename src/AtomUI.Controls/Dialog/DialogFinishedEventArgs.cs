namespace AtomUI.Controls;

public class DialogFinishedEventArgs : EventArgs
{
    public object? Result { get; }
    public DialogFinishedEventArgs(object? result)
    {
        Result = result;
    }
}