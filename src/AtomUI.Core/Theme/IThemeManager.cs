namespace AtomUI.Theme;

public interface IThemeManager
{
    const string DEFAULT_THEME_ID = "DaybreakBlue";

    IReadOnlyList<ThemeInfo> AvailableThemes { get; }
    ThemeState? CurrentTheme { get; }

    Task<ThemeTransitionResult> ApplyThemeAsync(
        ThemeRequest request,
        CancellationToken cancellationToken = default);

    event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
    event EventHandler<ThemeChangeFailedEventArgs>? ThemeChangeFailed;
}
