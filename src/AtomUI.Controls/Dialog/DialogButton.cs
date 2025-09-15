namespace AtomUI.Controls;

public class DialogButton : Button
{
    public DialogStandardButton? StandardButtonType { get; set; }
    public bool IsDefaultConfirmButton { get; set; }
    public bool IsDefaultEscapeButton { get; set; }
}