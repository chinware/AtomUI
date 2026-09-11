using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace AtomUI.Generator.Localization;

internal sealed class LanguageDataEntry
{
    internal LanguageDataEntry(
        string identifier,
        string tag,
        string cultureName,
        string nativeName,
        bool isRightToLeft,
        Location location)
    {
        Identifier = identifier;
        Tag = tag;
        CultureName = cultureName;
        NativeName = nativeName;
        IsRightToLeft = isRightToLeft;
        Location = location;
    }

    internal string Identifier { get; }

    internal string Tag { get; }

    internal string CultureName { get; }

    internal string NativeName { get; }

    internal bool IsRightToLeft { get; }

    internal Location Location { get; }
}

internal sealed class LanguageDataParseResult
{
    internal LanguageDataParseResult(
        ImmutableArray<LanguageDataEntry> entries,
        ImmutableArray<Diagnostic> diagnostics)
    {
        Entries = entries;
        Diagnostics = diagnostics;
    }

    internal ImmutableArray<LanguageDataEntry> Entries { get; }

    internal ImmutableArray<Diagnostic> Diagnostics { get; }
}
