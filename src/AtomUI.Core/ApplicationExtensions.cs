using AtomUI.Localization;
using AtomUI.Theme;
using Avalonia;

namespace AtomUI;

public static class ApplicationExtensions
{
    public static Application UseAtomUI(
        this Application application,
        Action<IAtomUIBuilder>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(application);
        if (AtomUIApplicationRuntimeStore.Get(application) is not null)
        {
            throw new InvalidOperationException(
                $"AtomUI is already initialized for Application '{application.GetType().FullName}'.");
        }

        var builder = new AtomUIBuilder(application);
        if (application is IGeneratedApplicationLanguageBootstrap bootstrap)
        {
            bootstrap.RegisterApplicationLanguages(builder.Localization);
        }
        configure?.Invoke(builder);

        LocalizationRuntime? localizationRuntime = null;
        AtomUIApplicationRuntime? applicationRuntime = null;
        try
        {
            localizationRuntime = builder.LocalizationBuilder.Build();
            var themeManager = builder.ThemeManagerBuilder.Build();
            applicationRuntime = new AtomUIApplicationRuntime(
                application,
                themeManager,
                localizationRuntime);
            AvaloniaLocator.CurrentMutable.BindToSelf(themeManager);
            applicationRuntime.InitializeApplication();
            foreach (var initializer in builder.ThemeManagerBuilder.Initializers)
            {
                initializer(themeManager);
            }

            return application;
        }
        catch
        {
            if (applicationRuntime is not null)
            {
                applicationRuntime.Dispose();
            }
            else
            {
                localizationRuntime?.Dispose();
            }
            throw;
        }
    }

    public static ILanguageManager? GetLanguageManager(this Application application)
    {
        ArgumentNullException.ThrowIfNull(application);
        return AtomUIApplicationRuntimeStore.Get(application)?.LanguageManager;
    }

    public static ILocalizer? GetLocalizer(this Application application)
    {
        ArgumentNullException.ThrowIfNull(application);
        return AtomUIApplicationRuntimeStore.Get(application)?.Localizer;
    }

    public static IThemeManager? GetThemeManager(this Application application)
    {
        ArgumentNullException.ThrowIfNull(application);
        return AtomUIApplicationRuntimeStore.Get(application)?.ThemeManager;
    }
}
