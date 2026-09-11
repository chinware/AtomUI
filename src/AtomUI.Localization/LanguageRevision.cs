namespace AtomUI.Localization;

internal sealed class LanguageRevision
{
    internal LanguageRevision(LanguageSnapshot snapshot, LanguageState state)
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
