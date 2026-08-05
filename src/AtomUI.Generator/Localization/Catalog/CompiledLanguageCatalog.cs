using System.Collections.Immutable;

namespace AtomUI.Generator.Localization.Catalog;

internal sealed class CompiledLanguageCatalog
{
    internal CompiledLanguageCatalog(
        LanguageCatalogInfo catalog,
        bool ownsCatalog,
        ImmutableArray<bool> formattedUnits,
        ImmutableArray<CompiledTranslationBundle> bundles)
    {
        Catalog = catalog;
        OwnsCatalog = ownsCatalog;
        FormattedUnits = formattedUnits;
        Bundles = bundles;
    }

    internal LanguageCatalogInfo Catalog { get; }

    internal bool OwnsCatalog { get; }

    internal ImmutableArray<bool> FormattedUnits { get; }

    internal ImmutableArray<CompiledTranslationBundle> Bundles { get; }
}

internal sealed class CompiledTranslationBundle
{
    internal CompiledTranslationBundle(
        string language,
        Xliff.LanguageFileSourceKind sourceKind,
        string sourceIdentity,
        ImmutableArray<string?> values)
    {
        Language = language;
        SourceKind = sourceKind;
        SourceIdentity = sourceIdentity;
        Values = values;
    }

    internal string Language { get; }

    internal Xliff.LanguageFileSourceKind SourceKind { get; }

    internal string SourceIdentity { get; }

    internal ImmutableArray<string?> Values { get; }
}

internal sealed class LanguageCatalogCompilationResult
{
    internal LanguageCatalogCompilationResult(
        ImmutableArray<CompiledLanguageCatalog> catalogs,
        ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> diagnostics)
    {
        Catalogs = catalogs;
        Diagnostics = diagnostics;
    }

    internal ImmutableArray<CompiledLanguageCatalog> Catalogs { get; }

    internal ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics { get; }
}
