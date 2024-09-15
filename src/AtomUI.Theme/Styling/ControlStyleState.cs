namespace AtomUI.Theme.Styling;

[Flags]
public enum ControlStyleState
{
    None = 0x00000000,
    Enabled = 0x00000001,
    Raised = 0x00000002,
    Sunken = 0x00000004,
    Indeterminate = 0x00000008,
    On = 0x00000010,
    Off = 0x00000020,
    HasFocus = 0x00000040,
    MouseOver = 0x00000080,
    Selected = 0x00000100,
    Active = 0x00000200,
    Editing = 0x00000400
}