using Avalonia.Styling;

namespace AtomUI.Theme;

public interface IThemeManager
{
    public IReadOnlyCollection<ITheme> AvailableThemes { get; }
    public ITheme? ActivatedTheme { get; }

    public void SetActiveTheme(ThemeVariant themeVariant);
}