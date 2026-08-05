namespace AtomUI.Localization;

internal sealed class LanguageRuntimeRevision
{
    internal LanguageRuntimeRevision(LanguageSnapshot snapshot, LanguageState state)
    {
        Snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
        State = state ?? throw new ArgumentNullException(nameof(state));
        if (snapshot.RequestedLanguage != state.CurrentLanguage)
        {
            throw new ArgumentException(
                "Snapshot requested language and committed language state must match.",
                nameof(state));
        }
    }

    internal LanguageSnapshot Snapshot { get; }

    internal LanguageState State { get; }
}

internal sealed class LanguageRuntimeContext
{
    private LanguageRuntimeRevision _current;

    internal LanguageRuntimeContext(
        LanguageCatalogRegistry registry,
        LanguageRuntimeRevision initialRevision)
    {
        Registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _current = initialRevision ?? throw new ArgumentNullException(nameof(initialRevision));
    }

    internal LanguageCatalogRegistry Registry { get; }

    internal LanguageRuntimeRevision Current => Volatile.Read(ref _current);

    internal void Publish(LanguageRuntimeRevision revision)
    {
        ArgumentNullException.ThrowIfNull(revision);
        Volatile.Write(ref _current, revision);
    }
}
