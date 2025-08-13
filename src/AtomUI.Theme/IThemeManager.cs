using System.Globalization;
using AtomUI.Theme.Language;
using AtomUI.Utils;
using Avalonia.Styling;

namespace AtomUI.Theme;

public interface IThemeManager
{
    public const string DEFAULT_THEME_ID = "DaybreakBlue";
    public static readonly LanguageVariant DEFAULT_LANGUAGE = LanguageVariant.zh_CN;
    
    public IReadOnlyCollection<ITheme> AvailableThemes { get; }
    public ITheme? ActivatedTheme { get; }

    public void SetActiveTheme(ThemeVariant themeVariant);
}