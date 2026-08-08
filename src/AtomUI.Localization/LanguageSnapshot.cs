using System.Text;

namespace AtomUI.Localization;

public sealed class LanguageSnapshot
{
    private readonly LanguageSnapshotEntry[][] _entries;

    internal LanguageSnapshot(
        LanguageTag requestedLanguage,
        LanguageSnapshotEntry[][] entries)
    {
        RequestedLanguage = requestedLanguage;
        _entries = entries;
    }

    public LanguageTag RequestedLanguage { get; }

    internal LanguageSnapshotEntry GetEntry(int catalogSlot, int unitSlot)
    {
        if ((uint)catalogSlot >= (uint)_entries.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(catalogSlot));
        }
        if ((uint)unitSlot >= (uint)_entries[catalogSlot].Length)
        {
            throw new ArgumentOutOfRangeException(nameof(unitSlot));
        }

        return _entries[catalogSlot][unitSlot];
    }
}

internal readonly struct LanguageSnapshotEntry
{
    internal LanguageSnapshotEntry(
        string text,
        LanguageTag resolvedLanguage,
        CompositeFormat? compositeFormat)
    {
        Text = text;
        ResolvedLanguage = resolvedLanguage;
        CompositeFormat = compositeFormat;
    }

    internal string Text { get; }

    internal LanguageTag ResolvedLanguage { get; }

    internal CompositeFormat? CompositeFormat { get; }
}
