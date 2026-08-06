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

    [Required]
    public string PackageId { get; set; } = string.Empty;

    [Required]
    public ITaskItem[] LanguageFiles { get; set; } = Array.Empty<ITaskItem>();

    public ITaskItem[] PackageFiles { get; set; } = Array.Empty<ITaskItem>();

    [Required]
    public string OutputManifestPath { get; set; } = string.Empty;

    [Output]
    public ITaskItem[] PreparedLanguageFiles { get; private set; } = Array.Empty<ITaskItem>();

    public override bool Execute()
    {
        var succeeded = ValidatePackageContents();
        var validation = new ValidateLanguageFilesTask
        {
            BuildEngine = BuildEngine,
            HostObject = HostObject,
            LanguageFiles = LanguageFiles
        };
        succeeded &= validation.Execute();

        var entries = new List<LanguagePackageCatalogEntry>();
        string? language = null;
        foreach (var item in LanguageFiles)
        {
            var parsed = Xliff21Parser.Parse(File.ReadAllText(item.ItemSpec));
            if (parsed.Document is null)
            {
                continue;
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

            var moduleId = item.GetMetadata("AtomUILanguageModuleId").Trim();
            if (moduleId.Length == 0)
            {
                Error(item.ItemSpec, "AtomUILanguageModuleId is required.");
                succeeded = false;
                continue;
            }
            if (!int.TryParse(
                    item.GetMetadata("AtomUILanguageContractVersion"),
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out var contractVersion) ||
                contractVersion <= 0)
            {
                Error(item.ItemSpec, "AtomUILanguageContractVersion must be a positive integer.");
                succeeded = false;
                continue;
            }
            if (!LanguagePackagePath.TryNormalize(
                    item.GetMetadata("AtomUILanguagePackagePath"),
                    out var packagePath))
            {
                Error(item.ItemSpec, "AtomUILanguagePackagePath must be a normalized relative path.");
                succeeded = false;
                continue;
            }

            item.SetMetadata("AtomUILanguageSourceKind", "StaticLanguagePack");
            item.SetMetadata("AtomUILanguageSourceIdentity", PackageId);
            entries.Add(new LanguagePackageCatalogEntry(
                moduleId,
                document.File.Id,
                contractVersion,
                packagePath,
                LanguageSourceFingerprint.Compute(document)));
        }

        if (!succeeded || language is null)
        {
            return false;
        }

        var manifest = new LanguagePackageManifest(PackageId, language, entries);
        WriteFile(OutputManifestPath, LanguagePackageManifestWriter.Write(manifest));
        PreparedLanguageFiles = LanguageFiles.ToArray();
        return true;
    }

    private bool ValidatePackageContents()
    {
        var succeeded = true;
        foreach (var item in PackageFiles)
        {
            var extension = Path.GetExtension(item.ItemSpec);
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
        }
        return succeeded;
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
}
