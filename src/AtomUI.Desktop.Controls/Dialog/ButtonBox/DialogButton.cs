namespace AtomUI.Desktop.Controls;

public class DialogButton : Button
{
    public DialogStandardButton? StandardButtonType { get; set; }
    public DialogButtonRole Role { get; set; } = DialogButtonRole.CustomRole;
    public bool IsDefaultConfirmButton { get; set; }
    public bool IsDefaultEscapeButton { get; set; }
}