namespace AtomUI.Desktop.Controls;

[Flags]
public enum ResizeHandleLocation
{
    North = 0x0001,
    South = 0x0002,
    West = 0x0004,
    East = 0x0008,
    NorthEast = North | East,
    NorthWest = North | West,
    SouthEast = South | East,
    SouthWest = South | West,
    Edges = North | South | West | East,
    Corners = NorthEast | NorthWest | SouthEast | SouthWest,
    All = Edges | Corners
}