using System.Collections.ObjectModel;

namespace AtomUI.Localization;

internal sealed class LocalizationHost : IDisposable
{
    private int _disposed;

    internal LocalizationHost(
        LanguageCatalogRegistry registry,
        IReadOnlyDictionary<LanguageTag, LanguageSnapshot> snapshots,
        LanguageManager languageManager,
        Localizer localizer,
        LanguageResourceProvider resourceProvider)
    {
        Registry = registry ?? throw new ArgumentNullException(nameof(registry));
        ArgumentNullException.ThrowIfNull(snapshots);
        LanguageManager = languageManager ?? throw new ArgumentNullException(nameof(languageManager));
        Localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        ResourceProvider = resourceProvider ?? throw new ArgumentNullException(nameof(resourceProvider));
        Snapshots = new ReadOnlyDictionary<LanguageTag, LanguageSnapshot>(
            new Dictionary<LanguageTag, LanguageSnapshot>(snapshots));
    }

    internal LanguageCatalogRegistry Registry { get; }

    internal IReadOnlyDictionary<LanguageTag, LanguageSnapshot> Snapshots { get; }

    internal LanguageManager LanguageManager { get; }

    internal Localizer Localizer { get; }

    internal LanguageResourceProvider ResourceProvider { get; }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 0)
        {
            LanguageManager.Dispose();
        }
    }
}
