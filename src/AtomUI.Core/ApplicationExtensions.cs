using AtomUI.Localization;
using AtomUI.Theme;
using Avalonia;
using System.Runtime.ExceptionServices;

namespace AtomUI;

public static class ApplicationExtensions
{
    public static Application UseAtomUI(
        this Application application,
        Action<IAtomUIBuilder>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(application);
        if (ApplicationScopeRegistry.Get(application) is not null)
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

        LocalizationHost? localizationHost = null;
        ApplicationScope? applicationScope = null;
        List<IAtomUIOwnedService>? ownedServices = null;
        try
        {
            localizationHost = builder.LocalizationBuilder.Build();
            var themeManager = builder.ThemeManagerBuilder.Build();
            ownedServices = BuildOwnedServices(application, builder.OwnedServiceRegistrations);
            applicationScope = new ApplicationScope(
                application,
                themeManager,
                localizationHost,
                ownedServices);
            ownedServices = null;
            AvaloniaLocator.CurrentMutable.BindToSelf(themeManager);
            applicationScope.InitializeApplication();
            foreach (var initializer in builder.ThemeManagerBuilder.Initializers)
            {
                initializer(themeManager);
            }

            return application;
        }
        catch (Exception initializationException)
        {
            try
            {
                if (applicationScope is not null)
                {
                    applicationScope.Dispose();
                }
                else
                {
                    DisposeOwnedServices(ownedServices);
                    localizationHost?.Dispose();
                }
            }
            catch (Exception cleanupException)
            {
                throw new AggregateException(
                    "AtomUI application initialization and cleanup failed.",
                    initializationException,
                    cleanupException);
            }
            ExceptionDispatchInfo.Capture(initializationException).Throw();
            throw;
        }
    }

    public static ILanguageManager? GetLanguageManager(this Application application)
    {
        ArgumentNullException.ThrowIfNull(application);
        return ApplicationScopeRegistry.Get(application)?.LanguageManager;
    }

    public static ILocalizer? GetLocalizer(this Application application)
    {
        ArgumentNullException.ThrowIfNull(application);
        return ApplicationScopeRegistry.Get(application)?.Localizer;
    }

    public static IThemeManager? GetThemeManager(this Application application)
    {
        ArgumentNullException.ThrowIfNull(application);
        return ApplicationScopeRegistry.Get(application)?.ThemeManager;
    }

    private static List<IAtomUIOwnedService> BuildOwnedServices(
        Application application,
        IReadOnlyList<AtomUIOwnedServiceRegistration> registrations)
    {
        var services = new List<IAtomUIOwnedService>(registrations.Count);
        try
        {
            foreach (var registration in registrations)
            {
                services.Add(registration.Factory(application));
            }
            return services;
        }
        catch (Exception factoryException)
        {
            try
            {
                DisposeOwnedServices(services);
            }
            catch (Exception cleanupException)
            {
                throw new AggregateException(
                    "AtomUI owned-service creation and cleanup failed.",
                    factoryException,
                    cleanupException);
            }
            ExceptionDispatchInfo.Capture(factoryException).Throw();
            throw;
        }
    }

    private static void DisposeOwnedServices(IReadOnlyList<IAtomUIOwnedService>? services)
    {
        if (services is null)
        {
            return;
        }

        List<Exception>? failures = null;
        for (var index = services.Count - 1; index >= 0; index--)
        {
            try
            {
                services[index].Dispose();
            }
            catch (Exception exception)
            {
                failures ??= [];
                failures.Add(exception);
            }
        }
        if (failures is { Count: 1 })
        {
            ExceptionDispatchInfo.Capture(failures[0]).Throw();
        }
        if (failures is { Count: > 1 })
        {
            throw new AggregateException("AtomUI owned-service disposal failed.", failures);
        }
    }
}
