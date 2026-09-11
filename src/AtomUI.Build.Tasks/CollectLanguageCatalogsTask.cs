using AtomUI.Build.Tasks.LocalizationBuild;
using Microsoft.Build.Framework;

namespace AtomUI.Build.Tasks;

public sealed class CollectLanguageCatalogsTask : AtomUILocalizationTask
{
    [Required]
    public ITaskItem[] CatalogFiles { get; set; } = Array.Empty<ITaskItem>();

    [Output]
    public ITaskItem[] Catalogs { get; private set; } = Array.Empty<ITaskItem>();

    public override bool Execute()
    {
        var succeeded = true;
        var paths = new HashSet<string>(StringComparer.Ordinal);
        var owners = new Dictionary<CatalogIdentity, string>();
        var catalogs = new List<(CatalogIdentity Identity, ITaskItem Item)>();
        foreach (var input in CatalogFiles)
        {
            var path = Path.GetFullPath(input.ItemSpec);
            if (!paths.Add(path))
            {
                continue;
            }

            var parsed = Parse(path, ref succeeded);
            if (parsed is null)
            {
                continue;
            }
            if (parsed.TargetLanguage is not null)
            {
                LogError(
                    "ATOMUILOC005",
                    path,
                    1,
                    1,
                    "A Catalog template must be an en-US XLIFF document without trgLang.");
                succeeded = false;
                continue;
            }

            var moduleId = input.GetMetadata("AtomUILanguageModuleId").Trim();
            if (moduleId.Length == 0)
            {
                LogError(
                    "ATOMUILOC006",
                    path,
                    1,
                    1,
                    "A Catalog template requires AtomUILanguageModuleId.");
                succeeded = false;
                continue;
            }

            var identity = new CatalogIdentity(moduleId, parsed.File.Id);
            if (owners.TryGetValue(identity, out var owner))
            {
                LogError(
                    "ATOMUILOC006",
                    path,
                    1,
                    1,
                    $"Catalog '{parsed.File.Id}' in module '{moduleId}' has more than one template " +
                    $"('{owner}' and '{path}').");
                succeeded = false;
                continue;
            }
            owners.Add(identity, path);

            var output = new GeneratedTaskItem(path);
            output.SetMetadata("AtomUILanguageModuleId", moduleId);
            output.SetMetadata("AtomUILanguageCatalogId", parsed.File.Id);
            output.SetMetadata(
                "AtomUILanguageSourceFingerprint",
                LanguageSourceFingerprint.Compute(parsed));
            catalogs.Add((identity, output));
        }

        Catalogs = catalogs.OrderBy(static entry => entry.Identity.ModuleId, StringComparer.Ordinal)
                           .ThenBy(static entry => entry.Identity.CatalogId, StringComparer.Ordinal)
                           .Select(static entry => entry.Item)
                           .ToArray();
        return succeeded;
    }

    private XliffDocumentModel? Parse(string path, ref bool succeeded)
    {
        XliffParseResult result;
        try
        {
            result = Xliff21Parser.Parse(File.ReadAllText(path));
        }
        catch (Exception exception) when (
            exception is IOException or UnauthorizedAccessException or ArgumentException)
        {
            LogError("ATOMUILOC005", path, 1, 1, $"Cannot read Catalog template: {exception.Message}");
            succeeded = false;
            return null;
        }

        if (result.Errors.Count == 0)
        {
            return result.Document;
        }
        foreach (var error in result.Errors)
        {
            LogError("ATOMUILOC005", path, error.Line, error.Column, error.Message);
        }
        succeeded = false;
        return null;
    }

    private readonly struct CatalogIdentity : IEquatable<CatalogIdentity>
    {
        internal CatalogIdentity(string moduleId, string catalogId)
        {
            ModuleId = moduleId;
            CatalogId = catalogId;
        }

        internal string ModuleId { get; }

        internal string CatalogId { get; }

        public bool Equals(CatalogIdentity other)
        {
            return string.Equals(ModuleId, other.ModuleId, StringComparison.Ordinal) &&
                   string.Equals(CatalogId, other.CatalogId, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj) => obj is CatalogIdentity other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (StringComparer.Ordinal.GetHashCode(ModuleId) * 397) ^
                       StringComparer.Ordinal.GetHashCode(CatalogId);
            }
        }
    }
}
