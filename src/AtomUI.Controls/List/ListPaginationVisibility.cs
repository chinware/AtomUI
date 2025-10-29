namespace AtomUI.Controls;

[Flags]
public enum ListPaginationVisibility
{
    None = 0x00,
    Top = 0x01,
    Bottom = 0x02,
    All = Top | Bottom,
}