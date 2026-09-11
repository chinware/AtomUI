using AtomUI.Localization;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;
using System.Runtime.ExceptionServices;

namespace AtomUI;

internal sealed class ApplicationScope : IDisposable
{
    private Application? _application;
    private readonly IReadOnlyList<IAtomUIOwnedService> _ownedServices;
    private int _attachedOwnedServiceCount;
    private int _initialized;
    private int _disposed;

    internal ApplicationScope(
        Application application,
        ThemeManager themeManager,
        LocalizationHost localizationHost,
        IReadOnlyList<IAtomUIOwnedService> ownedServices)
    {
        _application = application ?? throw new ArgumentNullException(nameof(application));
        ThemeManager = themeManager ?? throw new ArgumentNullException(nameof(themeManager));
        LocalizationHost = localizationHost ?? throw new ArgumentNullException(nameof(localizationHost));
        _ownedServices = ownedServices ?? throw new ArgumentNullException(nameof(ownedServices));
        FlowDirectionStyle = new Style(selector => selector.Is<TopLevel>());
        FlowDirectionStyle.Setters.Add(new Setter(
            Visual.FlowDirectionProperty,
            new DynamicResourceExtension(LanguageResourceKeys.FlowDirection)));
    }

    internal ThemeManager ThemeManager { get; }

    internal LocalizationHost LocalizationHost { get; }

    internal LanguageManager LanguageManager => LocalizationHost.LanguageManager;

    internal Localizer Localizer => LocalizationHost.Localizer;

    internal LanguageResourceProvider ResourceProvider => LocalizationHost.ResourceProvider;

    internal Style FlowDirectionStyle { get; }

    internal void InitializeApplication()
    {
        ThrowIfDisposed();
        if (Volatile.Read(ref _initialized) != 0)
        {
            throw new InvalidOperationException("AtomUI application scope is already initialized.");
        }

        var application = _application!;
        if (ApplicationScopeRegistry.Get(application) is not null)
        {
            throw new InvalidOperationException(
                $"AtomUI is already initialized for Application '{application.GetType().FullName}'.");
        }

        application.Resources.MergedDictionaries.Add(ResourceProvider);
        application.Styles.Add(FlowDirectionStyle);
        try
        {
            ThemeManager.InitializeApplication(application);
            ApplicationScopeRegistry.Register(application, this);
            for (var index = 0; index < _ownedServices.Count; index++)
            {
                _ownedServices[index].Attach(application);
                _attachedOwnedServiceCount = index + 1;
            }
            Volatile.Write(ref _initialized, 1);
        }
        catch (Exception initializationException)
        {
            var failures = new List<Exception> { initializationException };
            DetachOwnedServices(application, failures);
            TryCleanup(() => ApplicationScopeRegistry.Unregister(application, this), failures);
            TryCleanup(() => application.Styles.Remove(FlowDirectionStyle), failures);
            TryCleanup(() => application.Resources.MergedDictionaries.Remove(ResourceProvider), failures);
            ThrowFailures("AtomUI application initialization and rollback failed.", failures);
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        var failures = new List<Exception>();
        var application = Interlocked.Exchange(ref _application, null);
        if (application is not null)
        {
            TryCleanup(() => ApplicationScopeRegistry.Unregister(application, this), failures);
            DetachOwnedServices(application, failures);
            TryCleanup(() => application.Styles.Remove(FlowDirectionStyle), failures);
            TryCleanup(() => application.Resources.MergedDictionaries.Remove(ResourceProvider), failures);
        }

        for (var index = _ownedServices.Count - 1; index >= 0; index--)
        {
            var service = _ownedServices[index];
            TryCleanup(service.Dispose, failures);
        }

        TryCleanup(ThemeManager.Dispose, failures);
        TryCleanup(LocalizationHost.Dispose, failures);
        ThrowFailures("AtomUI application disposal failed.", failures);
    }

    private void ThrowIfDisposed()
    {
        if (Volatile.Read(ref _disposed) != 0)
        {
            throw new ObjectDisposedException(nameof(ApplicationScope));
        }
    }

    private void DetachOwnedServices(Application application, ICollection<Exception> failures)
    {
        var count = Interlocked.Exchange(ref _attachedOwnedServiceCount, 0);
        for (var index = count - 1; index >= 0; index--)
        {
            var service = _ownedServices[index];
            TryCleanup(() => service.Detach(application), failures);
        }
    }

    private static void TryCleanup(Action cleanup, ICollection<Exception> failures)
    {
        try
        {
            cleanup();
        }
        catch (Exception exception)
        {
            failures.Add(exception);
        }
    }

    private static void ThrowFailures(string message, IReadOnlyList<Exception> failures)
    {
        if (failures.Count == 0)
        {
            return;
        }
        if (failures.Count == 1)
        {
            ExceptionDispatchInfo.Capture(failures[0]).Throw();
        }
        throw new AggregateException(message, failures);
    }
}
