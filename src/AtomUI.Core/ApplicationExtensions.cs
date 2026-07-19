using AtomUI.Theme;
using AtomUI.Theme.Language;
using Avalonia;
using Avalonia.Data;

namespace AtomUI;

public static class ApplicationExtensions
{
    public static Application UseAtomUI(this Application application, Action<IThemeManagerBuilder>? themeConfigureAction = null)
    {
        var themeManagerBuilder = new ThemeManagerBuilder();
        themeManagerBuilder.WithDefaultLanguageVariant(LanguageVariant.en_US);
        themeConfigureAction?.Invoke(themeManagerBuilder);
        
        var themeManager = themeManagerBuilder.Build();
        AvaloniaLocator.CurrentMutable.BindToSelf(themeManager);
        themeManager.SetValue(ThemeManager.LanguageVariantProperty, themeManagerBuilder.LanguageVariant, BindingPriority.Template);
        themeManager.InitializeApplication(application);
        foreach (var initializer in themeManagerBuilder.Initializers)
        {
            initializer(themeManager);
        }
        return application;
    }
}
