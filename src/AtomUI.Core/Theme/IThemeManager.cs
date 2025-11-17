using AtomUI.Theme.Language;
using Avalonia;
using Avalonia.Styling;

namespace AtomUI.Theme;

public interface IThemeManager
{
    const string DEFAULT_THEME_ID = "DaybreakBlue";
    static readonly LanguageVariant DEFAULT_LANGUAGE = LanguageVariant.zh_CN;
    
    static ThemeVariant DefaultThemeVariant = new ThemeVariant(IThemeManager.DEFAULT_THEME_ID, null);
    
    static readonly StyledProperty<ThemeVariant> ThemeVariantProperty =
        AvaloniaProperty.Register<StyledElement, ThemeVariant>("ThemeVariant", DefaultThemeVariant);
    
    static readonly StyledProperty<bool> IsDarkThemeModeProperty =
        AvaloniaProperty.Register<StyledElement, bool>("IsDarkThemeMode");
    
    static readonly StyledProperty<bool> IsCompactThemeModeProperty =
        AvaloniaProperty.Register<StyledElement, bool>("IsCompactThemeMode");
    
    IReadOnlyCollection<ITheme> AvailableThemes { get; }
    ITheme? ActivatedTheme { get; }
    AvaloniaObject BindingSource { get; }
    
    LanguageVariant LanguageVariant { get; set; }
    bool IsMotionEnabled { get; set; }
    bool IsWaveSpiritEnabled { get; set; }
    bool IsDarkThemeMode { get; set; }
    bool IsCompactThemeMode { get; set; }
    
    event EventHandler<LanguageVariantChangedEventArgs>? LanguageVariantChanged;
}