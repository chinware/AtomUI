namespace AtomUI.Localization;

internal sealed class LanguageContext
{
    private LanguageRevision _current;

    internal LanguageContext(
        LanguageCatalogRegistry registry,
        LanguageRevision initialRevision)
    {
        Registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _current = initialRevision ?? throw new ArgumentNullException(nameof(initialRevision));
    }

    internal LanguageCatalogRegistry Registry { get; }

    internal LanguageRevision Current => Volatile.Read(ref _current);

    internal void Publish(LanguageRevision revision)
    {
        ArgumentNullException.ThrowIfNull(revision);
        Volatile.Write(ref _current, revision);
    }
}
