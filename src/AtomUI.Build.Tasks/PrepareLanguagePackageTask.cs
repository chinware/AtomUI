using System.Globalization;
using System.Text;
using AtomUI.Localization.Build;
using Microsoft.Build.Framework;

namespace AtomUI.Build.Tasks;

public sealed class PrepareLanguagePackageTask : AtomUILocalizationTask
{
    private static readonly HashSet<string> s_runtimeExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".dll",
        ".exe",
        ".so",
        ".dylib",
        ".cs",
        ".fs",
        ".vb",
        ".atomlang"
    };

    private static readonly HashSet<string> s_scriptExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".ps1",
        ".sh",
        ".cmd",
        ".bat"
    };

    private static readonly HashSet<string> s_buildLogicExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".props",
        ".targets"
    };

    private static readonly HashSet<string> s_buildLogicDirectories = new(StringComparer.OrdinalIgnoreCase)
    {
        "build",
        "buildMultiTargeting",
        "buildTransitive"
    };

    [Required]
    public string PackageId { get; set; } = string.Empty;

    [Required]
    public ITaskItem[] LanguageFiles { get; set; } = Array.Empty<ITaskItem>();

    public ITaskItem[] SourceLanguageFiles { get; set; } = Array.Empty<ITaskItem>();

    public ITaskItem[] PackageFiles { get; set; } = Array.Empty<ITaskItem>();

    [Required]
    public string OutputManifestPath { get; set; } = string.Empty;

    public string? ExpectedLanguage { get; set; }

    public string MinimumTargetState { get; set; } = "translated";

    public bool RequireVerifiedContract { get; set; }

    [Output]
    public ITaskItem[] PreparedLanguageFiles { get; private set; } = Array.Empty<ITaskItem>();

    public override bool Execute()
    {
        var succeeded = ValidatePackageContents();
        var targetModuleIds = LanguageFiles
            .Select(static item => item.GetMetadata("AtomUILanguageModuleId").Trim())
            .ToArray();
        foreach (var moduleId in targetModuleIds)
        {
            if (moduleId.Length == 0)
            {
                Error(string.Empty, "AtomUILanguageModuleId is required.");
                succeeded = false;
            }
        }

        var distinctModuleIds = targetModuleIds
            .Where(static moduleId => moduleId.Length > 0)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        if (distinctModuleIds.Length > 1)
        {
            Error(string.Empty, "A module language package must target exactly one AtomUILanguageModuleId.");
            succeeded = false;
        }

        var targetModuleId = distinctModuleIds.Length == 1 ? distinctModuleIds[0] : string.Empty;
        var relevantSourceFiles = SourceLanguageFiles
            .Where(item => string.Equals(
                item.GetMetadata("AtomUILanguageModuleId").Trim(),
                targetModuleId,
                StringComparison.Ordinal))
            .ToArray();
        var validation = new ValidateLanguageFilesTask
        {
            BuildEngine = BuildEngine,
            HostObject = HostObject,
            LanguageFiles = relevantSourceFiles.Concat(LanguageFiles).ToArray(),
            MinimumTargetState = MinimumTargetState
        };
        succeeded &= validation.Execute();

        if (!succeeded || LanguageFiles.Length == 0 || targetModuleId.Length == 0)
        {
            return false;
        }

        var targets = new List<ParsedTargetLanguageFile>();
        string? language = null;
        foreach (var item in LanguageFiles)
        {
            var parsed = Xliff21Parser.Parse(File.ReadAllText(item.ItemSpec));
            if (parsed.Document is null)
            {
                return false;
            }
            var document = parsed.Document;
            if (document.TargetLanguage is null)
            {
                Error(item.ItemSpec, "A static language package cannot contain an en-US source document.");
                succeeded = false;
                continue;
            }
            if (language is null)
            {
                language = document.TargetLanguage;
            }
            else if (!string.Equals(language, document.TargetLanguage, StringComparison.Ordinal))
            {
                Error(item.ItemSpec, "A static language package must contain exactly one target language.");
                succeeded = false;
            }

            if (!LanguagePackagePath.TryNormalize(
                    item.GetMetadata("AtomUILanguagePackagePath"),
                    out var packagePath))
            {
                Error(item.ItemSpec, "AtomUILanguagePackagePath must be a normalized relative path.");
                succeeded = false;
                continue;
            }
            targets.Add(new ParsedTargetLanguageFile(item, document, packagePath));
        }

        if (!succeeded || language is null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(ExpectedLanguage))
        {
            var expectedText = ExpectedLanguage!;
            if (!Bcp47LanguageTagParser.TryParse(expectedText, out var expectedLanguage) ||
                !string.Equals(expectedText, expectedLanguage, StringComparison.Ordinal))
            {
                Error(string.Empty, "ExpectedLanguage must be a canonical BCP 47 language tag.");
                return false;
            }
            if (!string.Equals(language, expectedLanguage, StringComparison.Ordinal))
            {
                Error(string.Empty, $"The package target language is '{language}', but ExpectedLanguage is '{expectedLanguage}'.");
                return false;
            }
        }

        var entries = relevantSourceFiles.Length == 0
            ? PrepareDeferredEntries(targetModuleId, targets)
            : PrepareVerifiedEntries(targetModuleId, targets, relevantSourceFiles);
        if (entries is null)
        {
            return false;
        }

        var manifest = new LanguagePackageManifest(PackageId, language, entries);
        WriteFile(OutputManifestPath, LanguagePackageManifestWriter.Write(manifest));
        PreparedLanguageFiles = LanguageFiles.ToArray();
        return true;
    }

    private IReadOnlyList<LanguagePackageCatalogEntry>? PrepareDeferredEntries(
        string moduleId,
        IReadOnlyList<ParsedTargetLanguageFile> targets)
    {
        if (RequireVerifiedContract)
        {
            Error(
                string.Empty,
                $"Language package '{PackageId}' requires a verified language contract for module " +
                $"'{moduleId}', but no authoritative en-US source assets were found.");
            return null;
        }

        var entries = new List<LanguagePackageCatalogEntry>(targets.Count);
        foreach (var target in targets)
        {
            if (!string.IsNullOrWhiteSpace(target.Item.GetMetadata("AtomUILanguageContractVersion")))
            {
                Error(
                    target.Item.ItemSpec,
                    "A deferred language package must not declare AtomUILanguageContractVersion without " +
                    "an authoritative source contract.");
                return null;
            }

            var sourceFingerprint = LanguageSourceFingerprint.Compute(target.Document);
            EnrichPreparedItem(
                target.Item,
                LanguagePackageContractValidation.Deferred,
                contractVersion: null,
                target.PackagePath,
                sourceFingerprint);
            entries.Add(new LanguagePackageCatalogEntry(
                moduleId,
                target.Document.File.Id,
                LanguagePackageContractValidation.Deferred,
                contractVersion: null,
                target.PackagePath,
                sourceFingerprint));
        }

        LogWarning(
            "ATOMUILOC010",
            string.Empty,
            1,
            1,
            $"Language package '{PackageId}' is being packed without the authoritative en-US contract " +
            $"for module '{moduleId}'. Contract validation is deferred to consuming applications. Add an " +
            "authoring-only component PackageReference with PrivateAssets=all to enable verified validation.");
        return entries;
    }

    private IReadOnlyList<LanguagePackageCatalogEntry>? PrepareVerifiedEntries(
        string moduleId,
        IReadOnlyList<ParsedTargetLanguageFile> targets,
        IReadOnlyList<ITaskItem> sourceItems)
    {
        var sources = new Dictionary<string, ParsedSourceLanguageFile>(StringComparer.Ordinal);
        foreach (var item in sourceItems)
        {
            var parsed = Xliff21Parser.Parse(File.ReadAllText(item.ItemSpec));
            if (parsed.Document is null || parsed.Document.TargetLanguage is not null)
            {
                CatalogError(item.ItemSpec, "An authoritative language contract must be an en-US source XLIFF.");
                return null;
            }
            if (!int.TryParse(
                    item.GetMetadata("AtomUILanguageContractVersion"),
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out var contractVersion) ||
                contractVersion <= 0)
            {
                Error(item.ItemSpec, "An authoritative language contract requires a positive ContractVersion.");
                return null;
            }
            if (sources.ContainsKey(parsed.Document.File.Id))
            {
                CatalogError(
                    item.ItemSpec,
                    $"Authoritative Catalog '{parsed.Document.File.Id}' is declared more than once.");
                return null;
            }
            sources.Add(
                parsed.Document.File.Id,
                new ParsedSourceLanguageFile(item, parsed.Document, contractVersion));
        }

        var targetsByCatalog = targets.ToDictionary(
            static target => target.Document.File.Id,
            StringComparer.Ordinal);
        foreach (var source in sources.Values)
        {
            if (!targetsByCatalog.ContainsKey(source.Document.File.Id))
            {
                CatalogError(
                    source.Item.ItemSpec,
                    $"Authoritative Catalog '{source.Document.File.Id}' does not contain a target XLIFF " +
                    $"in language package '{PackageId}'.");
                return null;
            }
        }
        foreach (var target in targets)
        {
            if (!sources.ContainsKey(target.Document.File.Id))
            {
                CatalogError(
                    target.Item.ItemSpec,
                    $"Target Catalog '{target.Document.File.Id}' is not declared by the authoritative " +
                    $"en-US contract for module '{moduleId}'.");
                return null;
            }
        }

        var entries = new List<LanguagePackageCatalogEntry>(targets.Count);
        foreach (var target in targets)
        {
            var source = sources[target.Document.File.Id];
            var declaredContractVersion = target.Item.GetMetadata("AtomUILanguageContractVersion").Trim();
            if (declaredContractVersion.Length > 0 &&
                (!int.TryParse(
                     declaredContractVersion,
                     NumberStyles.None,
                     CultureInfo.InvariantCulture,
                     out var parsedContractVersion) ||
                 parsedContractVersion != source.ContractVersion))
            {
                CatalogError(
                    target.Item.ItemSpec,
                    $"Target Catalog '{target.Document.File.Id}' ContractVersion '{declaredContractVersion}' " +
                    $"does not match authoritative ContractVersion '{source.ContractVersion}'.");
                return null;
            }

            var sourceFingerprint = LanguageSourceFingerprint.Compute(source.Document);
            var targetFingerprint = LanguageSourceFingerprint.Compute(target.Document);
            if (!string.Equals(sourceFingerprint, targetFingerprint, StringComparison.Ordinal))
            {
                CatalogError(
                    target.Item.ItemSpec,
                    $"Target Catalog '{target.Document.File.Id}' source fingerprint does not match the " +
                    "authoritative en-US contract.");
                return null;
            }

            EnrichPreparedItem(
                target.Item,
                LanguagePackageContractValidation.Verified,
                source.ContractVersion,
                target.PackagePath,
                sourceFingerprint);
            entries.Add(new LanguagePackageCatalogEntry(
                moduleId,
                target.Document.File.Id,
                LanguagePackageContractValidation.Verified,
                source.ContractVersion,
                target.PackagePath,
                sourceFingerprint));
        }
        return entries;
    }

    private void EnrichPreparedItem(
        ITaskItem item,
        LanguagePackageContractValidation contractValidation,
        int? contractVersion,
        string packagePath,
        string sourceFingerprint)
    {
        item.SetMetadata("AtomUILanguageSourceKind", "StaticLanguagePack");
        item.SetMetadata("AtomUILanguageSourceIdentity", PackageId);
        item.SetMetadata("AtomUILanguageContractValidation", contractValidation.ToString());
        if (contractVersion is { } version)
        {
            item.SetMetadata(
                "AtomUILanguageContractVersion",
                version.ToString(CultureInfo.InvariantCulture));
        }
        else
        {
            item.RemoveMetadata("AtomUILanguageContractVersion");
        }
        item.SetMetadata("AtomUILanguagePackagePath", packagePath);
        item.SetMetadata("AtomUILanguageSourceFingerprint", sourceFingerprint);
    }

    private void CatalogError(string file, string message)
    {
        LogError("ATOMUILOC006", file, 1, 1, message);
    }

    private bool ValidatePackageContents()
    {
        var succeeded = true;
        foreach (var item in PackageFiles)
        {
            foreach (var packagePath in GetPackagePaths(item))
            {
                var extension = GetEffectiveExtension(packagePath, item.ItemSpec);
                if (IsBuildLogicFile(packagePath, extension))
                {
                    Error(item.ItemSpec, "Static language packages cannot contain executable build logic.");
                    succeeded = false;
                    break;
                }

                if (s_scriptExtensions.Contains(extension))
                {
                    Error(item.ItemSpec, "Static language packages cannot contain scripts.");
                    succeeded = false;
                    break;
                }

                if (!s_runtimeExtensions.Contains(extension))
                {
                    continue;
                }

                Error(
                    item.ItemSpec,
                    extension.Equals(".atomlang", StringComparison.OrdinalIgnoreCase)
                        ? "Static language packages cannot contain .atomlang payloads."
                        : extension is ".cs" or ".fs" or ".vb"
                            ? "Static language packages cannot contain runtime source code."
                        : "Static language packages cannot contain a runtime assembly or native library.");
                succeeded = false;
                break;
            }
        }
        return succeeded;
    }

    private static IEnumerable<string> GetPackagePaths(ITaskItem item)
    {
        var packagePath = item.GetMetadata("PackagePath");
        var hasPackagePath = false;
        foreach (var path in packagePath.Split([';'], StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmedPath = path.Trim();
            if (trimmedPath.Length > 0)
            {
                hasPackagePath = true;
                yield return trimmedPath;
            }
        }

        if (!hasPackagePath)
        {
            yield return item.ItemSpec;
        }
    }

    private static string GetEffectiveExtension(string packagePath, string sourcePath)
    {
        var extension = Path.GetExtension(packagePath);
        return extension.Length == 0 ? Path.GetExtension(sourcePath) : extension;
    }

    private static bool IsBuildLogicFile(string path, string extension)
    {
        if (!s_buildLogicExtensions.Contains(extension))
        {
            return false;
        }

        return path.Replace('\\', '/')
                   .Split('/')
                   .Any(s_buildLogicDirectories.Contains);
    }

    private void Error(string file, string message)
    {
        LogError("ATOMUILOC009", file, 1, 1, message);
    }

    private static void WriteFile(string path, string content)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(path, content, new UTF8Encoding(false));
    }

    private sealed class ParsedTargetLanguageFile
    {
        internal ParsedTargetLanguageFile(
            ITaskItem item,
            XliffDocumentModel document,
            string packagePath)
        {
            Item = item;
            Document = document;
            PackagePath = packagePath;
        }

        internal ITaskItem Item { get; }

        internal XliffDocumentModel Document { get; }

        internal string PackagePath { get; }
    }

    private sealed class ParsedSourceLanguageFile
    {
        internal ParsedSourceLanguageFile(
            ITaskItem item,
            XliffDocumentModel document,
            int contractVersion)
        {
            Item = item;
            Document = document;
            ContractVersion = contractVersion;
        }

        internal ITaskItem Item { get; }

        internal XliffDocumentModel Document { get; }

        internal int ContractVersion { get; }
    }
}
