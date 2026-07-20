namespace AtomUI.Theme;

public sealed class ThemeCatalogChangedEventArgs : EventArgs
{
    public ThemeCatalogChangedEventArgs(
        long generation,
        IReadOnlyList<ThemeInfo> availableThemes,
        ThemeState? currentTheme)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(generation);
        ArgumentNullException.ThrowIfNull(availableThemes);

        Generation      = generation;
        AvailableThemes = Array.AsReadOnly(availableThemes.ToArray());
        CurrentTheme    = currentTheme;
    }

    public long Generation { get; }
    public IReadOnlyList<ThemeInfo> AvailableThemes { get; }
    public ThemeState? CurrentTheme { get; }
}
