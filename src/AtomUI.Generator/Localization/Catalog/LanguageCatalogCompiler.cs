using System.Collections.Immutable;
using AtomUI.Generator.Diagnostics;
using AtomUI.Generator.Localization.Xliff;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AtomUI.Generator.Localization.Catalog;

internal static class LanguageCatalogCompiler
{
    internal static LanguageCatalogCompilationResult Compile(
        ImmutableArray<LanguageCatalogParseResult> catalogResults,
        ImmutableArray<AdditionalLanguageFileParseResult> fileResults)
    {
        return Compile(catalogResults, fileResults, compilation: null);
    }

    internal static LanguageCatalogCompilationResult Compile(
        ImmutableArray<LanguageCatalogParseResult> catalogResults,
        ImmutableArray<AdditionalLanguageFileParseResult> fileResults,
        Compilation? compilation)
    {
        if (catalogResults.Any(static result => !result.Diagnostics.IsEmpty) ||
            fileResults.Any(static result => !result.Diagnostics.IsEmpty))
        {
            return Empty();
        }

        var ownedCatalogs = catalogResults.Select(static result => result.Catalog!)
                                          .OrderBy(static catalog => catalog.CatalogId, StringComparer.Ordinal)
                                          .ToArray();
        var files = fileResults.Select(static result => result.File!)
                               .OrderBy(static file => file.ModuleId, StringComparer.Ordinal)
                               .ThenBy(static file => file.Document.File.Id, StringComparer.Ordinal)
                               .ThenBy(static file => GetLanguage(file), StringComparer.Ordinal)
                               .ThenBy(static file => file.SourceKind)
                               .ThenBy(static file => file.SourceIdentity, StringComparer.Ordinal)
                               .ThenBy(static file => file.Path, StringComparer.Ordinal)
                               .ToArray();
        var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
        var filesByCatalog = new Dictionary<string, List<AdditionalLanguageFile>>(StringComparer.Ordinal);
        var catalogsById = ownedCatalogs.ToDictionary(
            static catalog => catalog.CatalogId,
            static catalog => new CatalogCompilationInput(catalog, ownsCatalog: true),
            StringComparer.Ordinal);

        foreach (var file in files)
        {
            var catalogId = $"{file.ModuleId}:{file.Document.File.Id}";
            if (!catalogsById.TryGetValue(catalogId, out var catalogInput))
            {
                if (compilation is null || file.SourceKind == LanguageFileSourceKind.ModuleBuiltIn)
                {
                    diagnostics.Add(Mismatch(
                        file,
                        catalogId,
                        $"file id '{file.Document.File.Id}' does not match a Catalog in module '{file.ModuleId}'"));
                    continue;
                }

                if (!ReferencedLanguageCatalogResolver.TryResolve(
                        compilation,
                        file,
                        out var referencedCatalog,
                        out var resolutionError))
                {
                    diagnostics.Add(Mismatch(file, catalogId, resolutionError));
                    continue;
                }

                catalogInput = new CatalogCompilationInput(referencedCatalog!, ownsCatalog: false);
                catalogsById.Add(catalogId, catalogInput);
            }

            if (file.SourceKind != LanguageFileSourceKind.ModuleBuiltIn &&
                file.ContractVersion != catalogInput.Catalog.ContractVersion)
            {
                diagnostics.Add(Mismatch(
                    file,
                    catalogId,
                    $"language input ContractVersion '{file.ContractVersion}' does not match " +
                    $"target Catalog ContractVersion '{catalogInput.Catalog.ContractVersion}'"));
                continue;
            }

            if (!filesByCatalog.TryGetValue(catalogId, out var catalogFiles))
            {
                catalogFiles = [];
                filesByCatalog.Add(catalogId, catalogFiles);
            }
            catalogFiles.Add(file);
        }

        var compiledCatalogs = ImmutableArray.CreateBuilder<CompiledLanguageCatalog>();
        foreach (var input in catalogsById.Values.OrderBy(
                     static item => item.Catalog.CatalogId,
                     StringComparer.Ordinal))
        {
            var catalog = input.Catalog;
            filesByCatalog.TryGetValue(catalog.CatalogId, out var catalogFiles);
            catalogFiles ??= [];
            var englishFiles = catalogFiles.Where(static file =>
                file.SourceKind == LanguageFileSourceKind.ModuleBuiltIn &&
                GetLanguage(file) == "en-US").ToArray();
            if (input.OwnsCatalog && englishFiles.Length != 1)
            {
                diagnostics.Add(Diagnostic.Create(
                    AtomUIDiagnosticDescriptors.LocalizationCatalogXliffMismatch,
                    catalog.Location,
                    catalog.MetadataName,
                    catalog.CatalogId,
                    englishFiles.Length == 0
                        ? "a complete en-US source bundle is required"
                        : "exactly one en-US source bundle is allowed"));
                continue;
            }

            var catalogDiagnosticStart = diagnostics.Count;
            AdditionalLanguageFile? sourceFile = englishFiles.FirstOrDefault();
            sourceFile ??= catalogFiles.FirstOrDefault();
            if (sourceFile is null)
            {
                continue;
            }

            var englishSource = sourceFile.Document.File.Units.ToDictionary(
                static unit => unit.Id,
                static unit => unit.Source);
            var formattedUnits = catalog.Units
                                        .Select(unit => sourceFile.Document.File.Units
                                            .FirstOrDefault(sourceUnit => sourceUnit.Id == unit.Id)?
                                            .PlaceholderIndexes.Count > 0)
                                        .ToImmutableArray();
            var bundles = ImmutableArray.CreateBuilder<CompiledTranslationBundle>();
            var owners = new Dictionary<(string Language, LanguageFileSourceKind SourceKind), AdditionalLanguageFile>();
            foreach (var file in catalogFiles)
            {
                var language = GetLanguage(file);
                var ownerKey = (language, file.SourceKind);
                if (owners.TryGetValue(ownerKey, out var owner))
                {
                    diagnostics.Add(Mismatch(
                        file,
                        catalog.CatalogId,
                        $"language '{language}' has more than one translation source at the same priority " +
                        $"('{owner.SourceIdentity}' and '{file.SourceIdentity}')"));
                    continue;
                }
                owners.Add(ownerKey, file);

                var fileDiagnosticStart = diagnostics.Count;
                ValidateUnitContract(
                    catalog,
                    file,
                    requireComplete: file.SourceKind != LanguageFileSourceKind.ApplicationOverride,
                    diagnostics);
                ValidateSourceText(catalog, englishSource, file, diagnostics);
                var values = CompileValues(catalog, file, language, diagnostics);
                if (diagnostics.Count == fileDiagnosticStart)
                {
                    bundles.Add(new CompiledTranslationBundle(
                        language,
                        file.SourceKind,
                        file.SourceIdentity,
                        values));
                }
            }

            if (diagnostics.Count == catalogDiagnosticStart)
            {
                compiledCatalogs.Add(new CompiledLanguageCatalog(
                    catalog,
                    input.OwnsCatalog,
                    formattedUnits,
                    bundles.OrderBy(static bundle => bundle.Language, StringComparer.Ordinal)
                           .ThenBy(static bundle => bundle.SourceKind)
                           .ThenBy(static bundle => bundle.SourceIdentity, StringComparer.Ordinal)
                           .ToImmutableArray()));
            }
        }

        return new LanguageCatalogCompilationResult(
            compiledCatalogs.ToImmutable(),
            diagnostics.ToImmutable());
    }

    private static void ValidateUnitContract(
        LanguageCatalogInfo catalog,
        AdditionalLanguageFile file,
        bool requireComplete,
        ImmutableArray<Diagnostic>.Builder diagnostics)
    {
        var unitsById = file.Document.File.Units
                            .Where(static unit => !unit.IsObsolete)
                            .ToDictionary(static unit => unit.Id);
        foreach (var catalogUnit in catalog.Units)
        {
            if (!unitsById.TryGetValue(catalogUnit.Id, out var fileUnit))
            {
                if (requireComplete)
                {
                    diagnostics.Add(Mismatch(
                        file,
                        catalog.CatalogId,
                        $"unit ID '{catalogUnit.Id}' ('{catalogUnit.Name}') is missing"));
                }
                continue;
            }
            if (!string.Equals(fileUnit.Name, catalogUnit.Name, StringComparison.Ordinal))
            {
                diagnostics.Add(Mismatch(
                    file,
                    catalog.CatalogId,
                    $"unit ID '{catalogUnit.Id}' has name '{fileUnit.Name}' instead of '{catalogUnit.Name}'",
                    fileUnit.Line,
                    fileUnit.Column));
            }
        }

        var catalogIds = new HashSet<int>(catalog.Units.Select(static unit => unit.Id));
        foreach (var fileUnit in file.Document.File.Units)
        {
            if (!fileUnit.IsObsolete && !catalogIds.Contains(fileUnit.Id))
            {
                diagnostics.Add(Mismatch(
                    file,
                    catalog.CatalogId,
                    $"unit ID '{fileUnit.Id}' is not declared by the Catalog",
                    fileUnit.Line,
                    fileUnit.Column));
            }
        }
    }

    private static void ValidateSourceText(
        LanguageCatalogInfo catalog,
        IReadOnlyDictionary<int, string> englishSource,
        AdditionalLanguageFile file,
        ImmutableArray<Diagnostic>.Builder diagnostics)
    {
        foreach (var unit in file.Document.File.Units)
        {
            if (!unit.IsObsolete &&
                englishSource.TryGetValue(unit.Id, out var expected) &&
                !string.Equals(unit.Source, expected, StringComparison.Ordinal))
            {
                diagnostics.Add(Mismatch(
                    file,
                    catalog.CatalogId,
                    $"unit ID '{unit.Id}' source text differs from the en-US source bundle",
                    unit.Line,
                    unit.Column));
            }
        }
    }

    private static ImmutableArray<string?> CompileValues(
        LanguageCatalogInfo catalog,
        AdditionalLanguageFile file,
        string language,
        ImmutableArray<Diagnostic>.Builder diagnostics)
    {
        var units = file.Document.File.Units
                        .Where(static unit => !unit.IsObsolete)
                        .ToDictionary(static unit => unit.Id);
        var values = ImmutableArray.CreateBuilder<string?>(catalog.Units.Length);
        foreach (var catalogUnit in catalog.Units)
        {
            if (!units.TryGetValue(catalogUnit.Id, out var unit))
            {
                values.Add(null);
                continue;
            }

            if (language == "en-US")
            {
                values.Add(unit.Source);
                continue;
            }

            if (unit.Target is null ||
                unit.TargetState is not ("translated" or "reviewed" or "final"))
            {
                diagnostics.Add(InvalidTranslation(
                    file,
                    unit,
                    language,
                    "the target must be present in a publishable translated, reviewed, or final state"));
                values.Add(null);
                continue;
            }
            values.Add(unit.Target);
        }
        return values.ToImmutable();
    }

    private static string GetLanguage(AdditionalLanguageFile file)
    {
        return file.Document.TargetLanguage ?? file.Document.SourceLanguage;
    }

    private static Diagnostic Mismatch(
        AdditionalLanguageFile file,
        string catalogId,
        string reason,
        int line = 1,
        int column = 1)
    {
        return Diagnostic.Create(
            AtomUIDiagnosticDescriptors.LocalizationCatalogXliffMismatch,
            CreateLocation(file, line, column),
            file.Path,
            catalogId,
            reason);
    }

    private static Diagnostic InvalidTranslation(
        AdditionalLanguageFile file,
        AtomUI.Localization.Build.XliffUnitModel unit,
        string language,
        string reason)
    {
        return Diagnostic.Create(
            AtomUIDiagnosticDescriptors.LocalizationInvalidTranslation,
            CreateLocation(file, unit.Line, unit.Column),
            unit.Id,
            language,
            reason);
    }

    private static Location CreateLocation(
        AdditionalLanguageFile file,
        int oneBasedLine,
        int oneBasedColumn)
    {
        var text = file.Text;
        var lineIndex = Math.Max(0, Math.Min(oneBasedLine - 1, text.Lines.Count - 1));
        var line = text.Lines[lineIndex];
        var column = Math.Max(0, Math.Min(oneBasedColumn - 1, line.Span.Length));
        var position = line.Start + column;
        var linePosition = new LinePosition(lineIndex, column);
        return Location.Create(
            file.Path,
            new TextSpan(position, 0),
            new LinePositionSpan(linePosition, linePosition));
    }

    private static LanguageCatalogCompilationResult Empty()
    {
        return new LanguageCatalogCompilationResult(
            ImmutableArray<CompiledLanguageCatalog>.Empty,
            ImmutableArray<Diagnostic>.Empty);
    }

    private sealed class CatalogCompilationInput
    {
        internal CatalogCompilationInput(LanguageCatalogInfo catalog, bool ownsCatalog)
        {
            Catalog = catalog;
            OwnsCatalog = ownsCatalog;
        }

        internal LanguageCatalogInfo Catalog { get; }

        internal bool OwnsCatalog { get; }
    }
}
