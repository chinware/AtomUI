namespace AtomUI.Controls;

/// <summary>
/// Represents a wrapped line in flex layout
/// </summary>
internal struct Section
{
    public int First { get; }     // First child index in this section
    public int Last { get; }      // Last child index in this section
    public double U { get; }      // Sum of item sizes on main axis (excluding spacing)
    public double V { get; }      // Maximum item size on cross axis

    public Section(int first, int last, double u, double v)
    {
        First = first;
        Last  = last;
        U     = u;
        V     = v;
    }
}