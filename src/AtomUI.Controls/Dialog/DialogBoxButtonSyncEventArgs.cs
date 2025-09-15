namespace AtomUI.Controls;

public class DialogBoxButtonSyncEventArgs: EventArgs
{
    public IReadOnlyList<Button> Buttons { get; set; }

    public DialogBoxButtonSyncEventArgs(IReadOnlyList<Button> buttons)
    {
        Buttons = buttons;
    }
}