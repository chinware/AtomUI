using System.Text;

namespace AtomUI.Localization;

internal static class LanguageSnapshotBuilder
{
    private static readonly TranslationSourceKind[] s_sourcePriorities =
    [
        TranslationSourceKind.ApplicationOverride,
        TranslationSourceKind.StaticLanguagePack,
        TranslationSourceKind.ModuleBuiltIn
    ];

    internal static LanguageSnapshot Build(
        LanguageCatalogRegistry registry,
        LanguageTag requestedLanguage,
        LanguageDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(definition);
        if (requestedLanguage == default)
        {
            throw new ArgumentException("A valid requested language is required.", nameof(requestedLanguage));
        }
        if (definition.Tag != requestedLanguage)
        {
            throw new LanguageConfigurationException(
                $"Language definition '{definition.Tag.Value}' cannot build Snapshot '{requestedLanguage.Value}'.");
        }

        var candidates = LanguageFallbackResolver.Resolve(
            requestedLanguage,
            definition.FormattingCulture);
        var requiresTranslatedCoverage = !string.Equals(
            definition.FormattingCulture.TwoLetterISOLanguageName,
            "en",
            StringComparison.OrdinalIgnoreCase);
        var entries = new LanguageSnapshotEntry[registry.Catalogs.Count][];

        for (var catalogSlot = 0; catalogSlot < registry.Catalogs.Count; catalogSlot++)
        {
            var catalog = registry.Catalogs[catalogSlot];
            var bundles = registry.GetBundles(catalogSlot);
            var catalogEntries = new LanguageSnapshotEntry[catalog.Units.Count];
            for (var unitSlot = 0; unitSlot < catalog.Units.Count; unitSlot++)
            {
                if (!TryResolveValue(
                        bundles,
                        candidates,
                        unitSlot,
                        out var text,
                        out var resolvedLanguage,
                        out var sourceIdentity))
                {
                    var unit = catalog.Units[unitSlot];
                    throw new LanguageCatalogException(
                        $"Catalog '{catalog.CatalogId}' unit '{unit.Name}' ({unit.Id}) has no value for " +
                        $"requested language '{requestedLanguage.Value}' or required en-US source.");
                }

                var unitDescriptor = catalog.Units[unitSlot];
                if (requiresTranslatedCoverage && resolvedLanguage == LanguageTags.EnUS)
                {
                    throw new LanguageCoverageException(
                        $"Supported language '{requestedLanguage.Value}' resolves Catalog '{catalog.CatalogId}' " +
                        $"unit '{unitDescriptor.Name}' ({unitDescriptor.Id}) only through final en-US fallback. " +
                        "Add a translation for the exact language or a valid parent candidate.");
                }

                CompositeFormat? compositeFormat = null;
                if (unitDescriptor.IsFormatted)
                {
                    try
                    {
                        compositeFormat = CompositeFormat.Parse(text);
                    }
                    catch (FormatException exception)
                    {
                        throw new LanguageCatalogException(
                            $"Compiled translation from '{sourceIdentity}' for Catalog '{catalog.CatalogId}' unit " +
                            $"'{unitDescriptor.Name}' ({unitDescriptor.Id}) and language " +
                            $"'{resolvedLanguage.Value}' has an invalid CompositeFormat.",
                            exception);
                    }
                }

                catalogEntries[unitSlot] = new LanguageSnapshotEntry(
                    text,
                    resolvedLanguage,
                    compositeFormat);
            }

            entries[catalogSlot] = catalogEntries;
        }

        return new LanguageSnapshot(requestedLanguage, entries);
    }

    private static bool TryResolveValue(
        IReadOnlyList<TranslationBundleDescriptor> bundles,
        IReadOnlyList<LanguageTag> candidates,
        int unitSlot,
        out string text,
        out LanguageTag resolvedLanguage,
        out string sourceIdentity)
    {
        foreach (var sourceKind in s_sourcePriorities)
        {
            foreach (var candidate in candidates)
            {
                if (candidate == LanguageTags.EnUS)
                {
                    continue;
                }

                if (TryFindBundleValue(
                        bundles,
                        candidate,
                        sourceKind,
                        unitSlot,
                        out text,
                        out sourceIdentity))
                {
                    resolvedLanguage = candidate;
                    return true;
                }
            }
        }

        foreach (var sourceKind in s_sourcePriorities)
        {
            if (TryFindBundleValue(
                    bundles,
                    LanguageTags.EnUS,
                    sourceKind,
                    unitSlot,
                    out text,
                    out sourceIdentity))
            {
                resolvedLanguage = LanguageTags.EnUS;
                return true;
            }
        }

        text = string.Empty;
        resolvedLanguage = default;
        sourceIdentity = string.Empty;
        return false;
    }

    private static bool TryFindBundleValue(
        IReadOnlyList<TranslationBundleDescriptor> bundles,
        LanguageTag language,
        TranslationSourceKind sourceKind,
        int unitSlot,
        out string text,
        out string sourceIdentity)
    {
        foreach (var bundle in bundles)
        {
            if (bundle.Language == language &&
                bundle.SourceKind == sourceKind &&
                bundle.Values[unitSlot] is { } value)
            {
                text = value;
                sourceIdentity = bundle.SourceIdentity;
                return true;
            }
        }

        text = string.Empty;
        sourceIdentity = string.Empty;
        return false;
    }
}
