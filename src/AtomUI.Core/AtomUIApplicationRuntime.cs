using AtomUI.Localization;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace AtomUI;

internal sealed class AtomUIApplicationRuntime : IDisposable
{
    private Application? _application;
    private int _initialized;
    private int _disposed;

    internal AtomUIApplicationRuntime(
        Application application,
        ThemeManager themeManager,
        LocalizationRuntime localizationRuntime)
    {
        _application = application ?? throw new ArgumentNullException(nameof(application));
        ThemeManager = themeManager ?? throw new ArgumentNullException(nameof(themeManager));
        LocalizationRuntime = localizationRuntime ?? throw new ArgumentNullException(nameof(localizationRuntime));
        FlowDirectionStyle = new Style(selector => selector.Is<TopLevel>());
        FlowDirectionStyle.Setters.Add(new Setter(
            Visual.FlowDirectionProperty,
            new DynamicResourceExtension(LanguageRuntimeResourceKeys.FlowDirection)));
    }

    internal ThemeManager ThemeManager { get; }

    internal LocalizationRuntime LocalizationRuntime { get; }

    internal LanguageManager LanguageManager => LocalizationRuntime.LanguageManager;

    internal Localizer Localizer => LocalizationRuntime.Localizer;

    internal LanguageResourceProvider ResourceProvider => LocalizationRuntime.ResourceProvider;

    internal Style FlowDirectionStyle { get; }

    internal void InitializeApplication()
    {
        ThrowIfDisposed();
        if (Volatile.Read(ref _initialized) != 0)
        {
            throw new InvalidOperationException("AtomUI runtime is already initialized.");
        }

        var application = _application!;
        if (AtomUIApplicationRuntimeStore.Get(application) is not null)
        {
            throw new InvalidOperationException(
                $"AtomUI is already initialized for Application '{application.GetType().FullName}'.");
        }

        application.Resources.MergedDictionaries.Add(ResourceProvider);
        application.Styles.Add(FlowDirectionStyle);
        try
        {
            ThemeManager.InitializeApplication(application);
            AtomUIApplicationRuntimeStore.Attach(application, this);
            Volatile.Write(ref _initialized, 1);
        }
        catch
        {
            application.Styles.Remove(FlowDirectionStyle);
            application.Resources.MergedDictionaries.Remove(ResourceProvider);
            throw;
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        var application = Interlocked.Exchange(ref _application, null);
        if (application is not null)
        {
            AtomUIApplicationRuntimeStore.Detach(application, this);
            application.Styles.Remove(FlowDirectionStyle);
            application.Resources.MergedDictionaries.Remove(ResourceProvider);
        }

        try
        {
            ThemeManager.Dispose();
        }
        finally
        {
            LocalizationRuntime.Dispose();
        }
    }

    private void ThrowIfDisposed()
    {
        if (Volatile.Read(ref _disposed) != 0)
        {
            throw new ObjectDisposedException(nameof(AtomUIApplicationRuntime));
        }
    }
}
