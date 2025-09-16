namespace AtomUI.Controls;

public class DialogBoxButtonSyncEventArgs: EventArgs
{
    public IReadOnlyList<DialogButton> Buttons { get; set; }

    public DialogBoxButtonSyncEventArgs(IReadOnlyList<DialogButton> buttons)
    {
        Buttons = buttons;
    }
}