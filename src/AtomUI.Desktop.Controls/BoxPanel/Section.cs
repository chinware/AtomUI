namespace AtomUI.Controls;

/// <summary>
/// Represents a wrapped line in flex layout
/// </summary>
internal struct Section
{
    public int First { get; }                   // First child index in this section
    public int Last { get; }                    // Last child index in this section
    public double TotalMainAxisSize { get; }    // Sum of item sizes on main axis (excluding spacing)
    public double MaxCrossAxisSize { get; }     // Maximum item size on cross axis

    public Section(int first, int last, double totalMainAxisSize, double maxCrossAxisSize)
    {
        First             = first;
        Last              = last;
        TotalMainAxisSize = totalMainAxisSize;
        MaxCrossAxisSize  = maxCrossAxisSize;
    }
}