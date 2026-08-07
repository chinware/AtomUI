using AtomUI.Localization.Build;
using Microsoft.Build.Framework;

namespace AtomUI.Build.Tasks;

public sealed class ValidateLanguageFilesTask : AtomUILocalizationTask
{
    private const string InvalidXliffCode = "ATOMUILOC005";
    private const string CatalogMismatchCode = "ATOMUILOC006";
    private const string InvalidTranslationCode = "ATOMUILOC007";
    private const string InvalidPackageCode = "ATOMUILOC009";

    [Required]
    public ITaskItem[] LanguageFiles { get; set; } = Array.Empty<ITaskItem>();

    public string MinimumTargetState { get; set; } = "translated";

    public override bool Execute()
    {
        MinimumTargetState = MinimumTargetState.Trim();
        if (!XliffTranslationTarget.TryGetStateRank(MinimumTargetState, out _))
        {
            Error(
                InvalidPackageCode,
                string.Empty,
                1,
                1,
                "MinimumTargetState must be translated, reviewed, or final.");
            return false;
        }

        var files = ParseFiles();
        ValidateDuplicateSources(files);
        ValidateBundles(files);
        return !files.HadErrors && !HasLoggedErrors;
    }

    private bool HasLoggedErrors { get; set; }

    private ParsedFiles ParseFiles()
    {
        var result = new ParsedFiles();
        foreach (var item in LanguageFiles)
        {
            var path = item.ItemSpec;
            string content;
            try
            {
                content = File.ReadAllText(path);
            }
            catch (Exception exception) when (
                exception is IOException or UnauthorizedAccessException or ArgumentException)
            {
                Error(InvalidXliffCode, path, 1, 1, $"Cannot read XLIFF language document: {exception.Message}");
                result.HadErrors = true;
                continue;
            }

            var parsed = Xliff21Parser.Parse(content);
            if (parsed.Errors.Count > 0)
            {
                foreach (var error in parsed.Errors)
                {
                    Error(
                        InvalidXliffCode,
                        path,
                        error.Line,
                        error.Column,
                        $"XLIFF language document is invalid: {error.Message}");
                }
                result.HadErrors = true;
                continue;
            }

            result.Files.Add(new ParsedLanguageFile(
                path,
                GetMetadata(item, "AtomUILanguageModuleId", "Application"),
                GetMetadata(item, "AtomUILanguageSourceKind", "ModuleBuiltIn"),
                GetMetadata(
                    item,
                    "AtomUILanguageSourceIdentity",
                    GetMetadata(item, "AtomUILanguageModuleId", "Application")),
                parsed.Document!));
        }
        return result;
    }

    private void ValidateDuplicateSources(ParsedFiles parsed)
    {
        var bundleOwners = new Dictionary<BundleIdentity, ParsedLanguageFile>();
        var overrideOwners = new Dictionary<OverrideUnitIdentity, ParsedLanguageFile>();
        foreach (var file in parsed.Files)
        {
            var identity = new BundleIdentity(
                file.ModuleId,
                file.Document.File.Id,
                file.Document.TargetLanguage ?? file.Document.SourceLanguage,
                file.SourceKind);
            if (string.Equals(file.SourceKind, "ApplicationOverride", StringComparison.Ordinal))
            {
                ValidateOverrideUnitSources(file, identity, overrideOwners);
                continue;
            }

            if (!bundleOwners.TryGetValue(identity, out var owner))
            {
                bundleOwners.Add(identity, file);
                continue;
            }

            Error(
                CatalogMismatchCode,
                file.Path,
                1,
                1,
                $"Catalog '{identity.CatalogId}' language '{identity.Language}' has more than one " +
                $"translation source at the same priority ('{owner.SourceIdentity}' and " +
                 $"'{file.SourceIdentity}').");
        }
    }

    private void ValidateOverrideUnitSources(
        ParsedLanguageFile file,
        BundleIdentity identity,
        Dictionary<OverrideUnitIdentity, ParsedLanguageFile> owners)
    {
        foreach (var unit in file.Document.File.Units.Where(static unit =>
                     !unit.IsObsolete && XliffTranslationTarget.IsPublishable(unit)))
        {
            var unitIdentity = new OverrideUnitIdentity(identity, unit.Key);
            if (!owners.TryGetValue(unitIdentity, out var owner))
            {
                owners.Add(unitIdentity, file);
                continue;
            }

            Error(
                CatalogMismatchCode,
                file.Path,
                unit.Line,
                unit.Column,
                $"Catalog '{identity.CatalogId}' unit '{unit.Key}' ('{unit.Name ?? unit.Key}') language " +
                $"'{identity.Language}' has more than one translation source at the same priority " +
                $"('{owner.SourceIdentity}' and '{file.SourceIdentity}').");
        }
    }

    private void ValidateBundles(ParsedFiles parsed)
    {
        var sourceFiles = parsed.Files
                                .Where(static file => file.Document.TargetLanguage is null)
                                .GroupBy(static file => (file.ModuleId, file.Document.File.Id))
                                .ToDictionary(static group => group.Key, static group => group.First());
        foreach (var file in parsed.Files)
        {
            if (file.Document.TargetLanguage is null)
            {
                continue;
            }

            sourceFiles.TryGetValue((file.ModuleId, file.Document.File.Id), out var source);
            ValidateTarget(file, source);
        }
    }

    private void ValidateTarget(
        ParsedLanguageFile target,
        ParsedLanguageFile? source)
    {
        var targetUnits = target.Document.File.Units
                                .Where(static unit => !unit.IsObsolete)
                                .ToDictionary(static unit => unit.Key, StringComparer.Ordinal);
        IReadOnlyDictionary<string, XliffUnitModel>? sourceUnits = source?.Document.File.Units
            .Where(static unit => !unit.IsObsolete)
            .ToDictionary(static unit => unit.Key, StringComparer.Ordinal);
        var isPartial = string.Equals(
            target.SourceKind,
            "ApplicationOverride",
            StringComparison.Ordinal);

        if (sourceUnits is not null)
        {
            foreach (var sourceUnit in sourceUnits.Values)
            {
                if (!targetUnits.TryGetValue(sourceUnit.Key, out var targetUnit))
                {
                    if (!isPartial)
                    {
                        Error(
                            InvalidTranslationCode,
                            target.Path,
                            1,
                            1,
                            $"Translation unit '{sourceUnit.Key}' ('{sourceUnit.Name ?? sourceUnit.Key}') is missing from " +
                            $"the complete '{target.Document.TargetLanguage}' bundle.");
                    }
                    continue;
                }

                if (!string.Equals(sourceUnit.Source, targetUnit.Source, StringComparison.Ordinal))
                {
                    Error(
                        CatalogMismatchCode,
                        target.Path,
                        targetUnit.Line,
                        targetUnit.Column,
                        $"Translation unit '{targetUnit.Key}' does not match the en-US Catalog source.");
                }
            }

            foreach (var targetUnit in targetUnits.Values)
            {
                if (!sourceUnits.ContainsKey(targetUnit.Key))
                {
                    Error(
                        CatalogMismatchCode,
                        target.Path,
                        targetUnit.Line,
                        targetUnit.Column,
                        $"Translation unit '{targetUnit.Key}' is not declared by the en-US Catalog source.");
                }
            }
        }

        foreach (var unit in targetUnits.Values)
        {
            if (!XliffTranslationTarget.IsPublishable(unit))
            {
                Error(
                    InvalidTranslationCode,
                    target.Path,
                    unit.Line,
                    unit.Column,
                    $"Translation unit '{unit.Key}' ('{unit.Name ?? unit.Key}') must contain a target " +
                    "in translated, reviewed, or final state; an empty target is valid only when the source is empty.");
                continue;
            }

            if (!XliffTranslationTarget.MeetsMinimumState(unit, MinimumTargetState))
            {
                Error(
                    InvalidTranslationCode,
                    target.Path,
                    unit.Line,
                    unit.Column,
                    $"Translation unit '{unit.Key}' ('{unit.Name ?? unit.Key}') target state " +
                    $"'{unit.TargetState}' does not meet the required minimum state '{MinimumTargetState}'.");
            }
        }
    }

    private void Error(
        string code,
        string file,
        int line,
        int column,
        string message)
    {
        HasLoggedErrors = true;
        LogError(code, file, line, column, message);
    }

    private static string GetMetadata(ITaskItem item, string name, string fallback)
    {
        var value = item.GetMetadata(name);
        return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
    }

    private sealed class ParsedFiles
    {
        internal List<ParsedLanguageFile> Files { get; } = [];

        internal bool HadErrors { get; set; }
    }

    private sealed class ParsedLanguageFile
    {
        internal ParsedLanguageFile(
            string path,
            string moduleId,
            string sourceKind,
            string sourceIdentity,
            XliffDocumentModel document)
        {
            Path = path;
            ModuleId = moduleId;
            SourceKind = sourceKind;
            SourceIdentity = sourceIdentity;
            Document = document;
        }

        internal string Path { get; }

        internal string ModuleId { get; }

        internal string SourceKind { get; }

        internal string SourceIdentity { get; }

        internal XliffDocumentModel Document { get; }
    }

    private readonly struct BundleIdentity : IEquatable<BundleIdentity>
    {
        internal BundleIdentity(string moduleId, string catalogId, string language, string sourceKind)
        {
            ModuleId = moduleId;
            CatalogId = catalogId;
            Language = language;
            SourceKind = sourceKind;
        }

        internal string ModuleId { get; }

        internal string CatalogId { get; }

        internal string Language { get; }

        internal string SourceKind { get; }

        public bool Equals(BundleIdentity other)
        {
            return string.Equals(ModuleId, other.ModuleId, StringComparison.Ordinal) &&
                   string.Equals(CatalogId, other.CatalogId, StringComparison.Ordinal) &&
                   string.Equals(Language, other.Language, StringComparison.Ordinal) &&
                   string.Equals(SourceKind, other.SourceKind, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj) => obj is BundleIdentity other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = StringComparer.Ordinal.GetHashCode(ModuleId);
                hash = (hash * 397) ^ StringComparer.Ordinal.GetHashCode(CatalogId);
                hash = (hash * 397) ^ StringComparer.Ordinal.GetHashCode(Language);
                return (hash * 397) ^ StringComparer.Ordinal.GetHashCode(SourceKind);
            }
        }
    }

    private readonly struct OverrideUnitIdentity : IEquatable<OverrideUnitIdentity>
    {
        internal OverrideUnitIdentity(BundleIdentity bundle, string key)
        {
            Bundle = bundle;
            Key = key;
        }

        private BundleIdentity Bundle { get; }

        private string Key { get; }

        public bool Equals(OverrideUnitIdentity other)
        {
            return Bundle.Equals(other.Bundle) &&
                   string.Equals(Key, other.Key, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj) => obj is OverrideUnitIdentity other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (Bundle.GetHashCode() * 397) ^ StringComparer.Ordinal.GetHashCode(Key);
            }
        }
    }
}
