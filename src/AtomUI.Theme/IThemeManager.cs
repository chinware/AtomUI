using System.Globalization;
using AtomUI.Utils;
using Avalonia.Styling;

namespace AtomUI.Theme;

public interface IThemeManager
{
    public const string DEFAULT_THEME_ID = "DaybreakBlue";
    public static readonly CultureInfo DEFAULT_LANGUAGE = new(LanguageCode.zh_CN);
    
    public IReadOnlyCollection<ITheme> AvailableThemes { get; }
    public ITheme? ActivatedTheme { get; }

    public void SetActiveTheme(ThemeVariant themeVariant);
}