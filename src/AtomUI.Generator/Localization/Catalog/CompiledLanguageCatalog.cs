using System.Collections.Immutable;
using AtomUI.Generator.Localization.Xliff;
using Microsoft.CodeAnalysis;

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

internal sealed class CatalogCompilationWorkItem
{
    internal CatalogCompilationWorkItem(
        CatalogSymbolEntry catalogEntry,
        ImmutableArray<LanguageInputResolution> inputs)
    {
        CatalogEntry = catalogEntry;
        Inputs = inputs;
    }

    internal CatalogSymbolEntry CatalogEntry { get; }

    internal LanguageCatalogInfo Catalog => CatalogEntry.Catalog;

    internal bool OwnsCatalog => CatalogEntry.OwnsCatalog;

    internal ImmutableArray<LanguageInputResolution> Inputs { get; }
}

internal sealed class CatalogPlanningResult
{
    internal CatalogPlanningResult(
        ImmutableArray<CatalogCompilationWorkItem> workItems,
        ImmutableArray<LanguageInputResolution> dormantInputs,
        ImmutableArray<Diagnostic> diagnostics)
    {
        WorkItems = workItems;
        DormantInputs = dormantInputs;
        Diagnostics = diagnostics;
    }

    internal ImmutableArray<CatalogCompilationWorkItem> WorkItems { get; }

    internal ImmutableArray<LanguageInputResolution> DormantInputs { get; }

    internal ImmutableArray<Diagnostic> Diagnostics { get; }
}

internal sealed class ValidatedCatalogInput
{
    internal ValidatedCatalogInput(
        CatalogCompilationWorkItem workItem,
        LanguageFileInput sourceFile)
    {
        WorkItem = workItem;
        SourceFile = sourceFile;
    }

    internal CatalogCompilationWorkItem WorkItem { get; }

    internal LanguageFileInput SourceFile { get; }
}

internal sealed class CatalogValidationResult
{
    internal CatalogValidationResult(
        ValidatedCatalogInput? input,
        ImmutableArray<Diagnostic> diagnostics)
    {
        Input = input;
        Diagnostics = diagnostics;
    }

    internal ValidatedCatalogInput? Input { get; }

    internal ImmutableArray<Diagnostic> Diagnostics { get; }
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
