using System.Collections.Immutable;
using AtomUI.Generator.Localization.Xliff;

namespace AtomUI.Generator.Localization.Catalog;

internal static class TranslationBundleCompiler
{
    internal static CompiledLanguageCatalog Compile(ValidatedCatalogInput input)
    {
        var catalog = input.WorkItem.Catalog;
        var sourceUnits = input.SourceFile.Document.File.Units
                                  .Where(static unit => !unit.IsObsolete)
                                  .ToDictionary(static unit => unit.Key, StringComparer.Ordinal);
        var formattedUnits = catalog.Units
                                    .Select(unit => sourceUnits[unit.Key].PlaceholderIndexes.Count > 0)
                                    .ToImmutableArray();
        var bundles = input.WorkItem.Inputs
                           .OrderBy(static resolution => GetLanguage(resolution.Input), StringComparer.Ordinal)
                           .ThenBy(static resolution => resolution.Input.SourceKind)
                           .ThenBy(static resolution => resolution.Input.SourceIdentity, StringComparer.Ordinal)
                           .ThenBy(static resolution => resolution.Input.Path, StringComparer.Ordinal)
                           .Select(resolution => CompileBundle(catalog, resolution.Input))
                           .OrderBy(static bundle => bundle.Language, StringComparer.Ordinal)
                           .ThenBy(static bundle => bundle.SourceKind)
                           .ThenBy(static bundle => bundle.SourceIdentity, StringComparer.Ordinal)
                           .ThenBy(static bundle => GetFirstPopulatedSlot(bundle.Values))
                           .ToImmutableArray();

        return new CompiledLanguageCatalog(
            catalog,
            input.WorkItem.OwnsCatalog,
            formattedUnits,
            bundles);
    }

    private static CompiledTranslationBundle CompileBundle(
        LanguageCatalogInfo catalog,
        LanguageFileInput input)
    {
        var language = GetLanguage(input);
        var unitsByKey = input.Document.File.Units
                              .Where(static unit => !unit.IsObsolete)
                              .ToDictionary(static unit => unit.Key, StringComparer.Ordinal);
        var values = ImmutableArray.CreateBuilder<string?>(catalog.Units.Length);
        foreach (var catalogUnit in catalog.Units)
        {
            if (!unitsByKey.TryGetValue(catalogUnit.Key, out var unit))
            {
                values.Add(null);
                continue;
            }

            values.Add(language == "en-US" ? unit.Source : unit.Target);
        }

        return new CompiledTranslationBundle(
            language,
            input.SourceKind,
            input.SourceIdentity,
            values.ToImmutable());
    }

    private static int GetFirstPopulatedSlot(ImmutableArray<string?> values)
    {
        for (var index = 0; index < values.Length; index++)
        {
            if (values[index] is not null)
            {
                return index;
            }
        }

        return int.MaxValue;
    }

    private static string GetLanguage(LanguageFileInput input)
    {
        return input.Document.TargetLanguage ?? input.Document.SourceLanguage;
    }
}
