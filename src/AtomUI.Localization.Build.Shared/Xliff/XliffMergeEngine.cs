namespace AtomUI.Localization.Build;

internal static class XliffMergeEngine
{
    internal static XliffDocumentModel Merge(
        XliffDocumentModel source,
        XliffDocumentModel? existingTarget,
        string targetLanguage)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        if (targetLanguage is null)
        {
            throw new ArgumentNullException(nameof(targetLanguage));
        }
        if (!Bcp47LanguageTagParser.TryParse(targetLanguage, out var canonicalTargetLanguage) ||
            !string.Equals(targetLanguage, canonicalTargetLanguage, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "The target language must be a canonical BCP 47 language tag.",
                nameof(targetLanguage));
        }
        if (source.TargetLanguage is not null)
        {
            throw new ArgumentException("The source template cannot declare a target language.", nameof(source));
        }

        if (existingTarget is not null)
        {
            if (!string.Equals(source.File.Id, existingTarget.File.Id, StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    "The existing target Catalog does not match the source Catalog.",
                    nameof(existingTarget));
            }
            if (!string.Equals(
                    existingTarget.TargetLanguage,
                    canonicalTargetLanguage,
                    StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    "The existing target language does not match the requested target language.",
                    nameof(existingTarget));
            }
        }

        var existingUnits = existingTarget?.File.Units.ToDictionary(static unit => unit.Id) ??
                            new Dictionary<int, XliffUnitModel>();
        var sourceIds = new HashSet<int>();
        var mergedUnits = new List<XliffUnitModel>();
        foreach (var sourceUnit in source.File.Units.OrderBy(static unit => unit.Id))
        {
            sourceIds.Add(sourceUnit.Id);
            existingUnits.TryGetValue(sourceUnit.Id, out var existingUnit);
            var sourceChanged = existingUnit is not null &&
                                !string.Equals(
                                    sourceUnit.Source,
                                    existingUnit.Source,
                                    StringComparison.Ordinal);
            mergedUnits.Add(new XliffUnitModel(
                sourceUnit.Id,
                sourceUnit.Name,
                sourceUnit.Source,
                existingUnit?.Target ?? string.Empty,
                existingUnit is null || sourceChanged ? "initial" : existingUnit.TargetState,
                MergeNotes(sourceUnit.Notes, existingUnit?.Notes),
                sourceUnit.PlaceholderIndexes,
                sourceUnit.Line,
                sourceUnit.Column));
        }

        foreach (var existingUnit in existingUnits.Values
                                                  .Where(unit => !sourceIds.Contains(unit.Id))
                                                  .OrderBy(static unit => unit.Id))
        {
            mergedUnits.Add(new XliffUnitModel(
                existingUnit.Id,
                existingUnit.Name,
                existingUnit.Source,
                existingUnit.Target,
                existingUnit.TargetState,
                existingUnit.Notes,
                existingUnit.PlaceholderIndexes,
                existingUnit.Line,
                existingUnit.Column,
                isObsolete: true));
        }

        return new XliffDocumentModel(
            source.SourceLanguage,
            canonicalTargetLanguage,
            new XliffFileModel(
                source.File.Id,
                mergedUnits.OrderBy(static unit => unit.Id).ToArray()));
    }

    private static IReadOnlyList<string> MergeNotes(
        IReadOnlyList<string> sourceNotes,
        IReadOnlyList<string>? existingNotes)
    {
        if (existingNotes is null || existingNotes.Count == 0)
        {
            return sourceNotes.ToArray();
        }

        return sourceNotes.Concat(existingNotes)
                          .Distinct(StringComparer.Ordinal)
                          .ToArray();
    }
}
