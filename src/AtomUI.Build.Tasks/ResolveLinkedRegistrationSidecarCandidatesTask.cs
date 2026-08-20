using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using AtomUI.LinkedRegistration.Protocol;
using Microsoft.Build.Framework;

namespace AtomUI.Build.Tasks;

/// <summary>
/// Resolves all formal linked-registration Sidecar candidates before the compiler sees them.
/// Metadata extraction is only allowed for references that have no formal Sidecar identity.
/// </summary>
public sealed class ResolveLinkedRegistrationSidecarCandidatesTask : ITask
{
    private const string DiagnosticCode = "ATOMUILINK005";
    private const string SidecarSourceMetadata = "AtomUILinkedSidecarSource";
    private const string SidecarAssemblyMetadata = "AtomUILinkedSidecarAssembly";
    private const string SidecarHashMetadata = "AtomUILinkedSidecarContractHash";
    private const string ReferenceAssemblyMetadata = "AtomUILinkedAssemblyName";

    public ITaskItem[] SidecarCandidates { get; set; } = Array.Empty<ITaskItem>();

    [Required]
    public ITaskItem[] ReferencePaths { get; set; } = Array.Empty<ITaskItem>();

    [Output]
    public ITaskItem[] CanonicalSidecars { get; private set; } = Array.Empty<ITaskItem>();

    [Output]
    public ITaskItem[] ExtractableReferences { get; private set; } = Array.Empty<ITaskItem>();

    public IBuildEngine BuildEngine { get; set; } = null!;

    public ITaskHost HostObject { get; set; } = null!;

    public bool Execute()
    {
        var referenceAssemblies = ReadReferenceAssemblies(ReferencePaths);
        var referencedAssemblyNames = new HashSet<string>(
            referenceAssemblies.Select(static reference => reference.Name),
            StringComparer.Ordinal);
        var candidatesByAssembly = new Dictionary<string, List<SidecarCandidate>>(StringComparer.Ordinal);
        var succeeded = true;

        foreach (var item in SidecarCandidates)
        {
            var path = GetItemPath(item);
            if (!File.Exists(path))
            {
                continue;
            }

            if (!TryReadCandidate(item, path, out var candidate))
            {
                succeeded = false;
                continue;
            }

            if (!referencedAssemblyNames.Contains(candidate!.AssemblyName))
            {
                continue;
            }

            if (!candidatesByAssembly.TryGetValue(candidate!.AssemblyName, out var group))
            {
                group = [];
                candidatesByAssembly.Add(candidate.AssemblyName, group);
            }

            group.Add(candidate);
        }

        var canonical = new List<SidecarCandidate>();
        var formalAssemblies = new HashSet<string>(StringComparer.Ordinal);
        foreach (var group in candidatesByAssembly.OrderBy(
                     static entry => entry.Key,
                     StringComparer.Ordinal))
        {
            var selected = SelectCanonical(group.Value, ref succeeded);
            if (selected is null)
            {
                continue;
            }

            canonical.Add(selected);
            formalAssemblies.Add(selected.AssemblyName);
        }

        if (!succeeded)
        {
            CanonicalSidecars = Array.Empty<ITaskItem>();
            ExtractableReferences = Array.Empty<ITaskItem>();
            return false;
        }

        var extractable = referenceAssemblies
            .Where(static entry => entry.Name.StartsWith("AtomUI.", StringComparison.Ordinal))
            .Where(entry => !formalAssemblies.Contains(entry.Name))
            .OrderBy(static entry => entry.Name, StringComparer.Ordinal)
            .ThenBy(static entry => entry.Path, StringComparer.Ordinal)
            .GroupBy(static entry => entry.Name, StringComparer.Ordinal)
            .Select(static group => group.First())
            .ToArray();

        CanonicalSidecars = canonical
            .OrderBy(static candidate => candidate.AssemblyName, StringComparer.Ordinal)
            .ThenBy(static candidate => SourcePriority(candidate.Source), Comparer<int>.Default)
            .ThenBy(static candidate => candidate.Path, StringComparer.Ordinal)
            .Select(static candidate =>
            {
                var item = new GeneratedTaskItem(candidate.Path);
                item.SetMetadata(SidecarSourceMetadata, candidate.Source);
                item.SetMetadata(SidecarAssemblyMetadata, candidate.AssemblyName);
                item.SetMetadata(SidecarHashMetadata, candidate.ContractHash);
                return (ITaskItem)item;
            })
            .ToArray();

        ExtractableReferences = extractable
            .Select(static reference =>
            {
                var item = new GeneratedTaskItem(reference.Path);
                item.SetMetadata(ReferenceAssemblyMetadata, reference.Name);
                return (ITaskItem)item;
            })
            .ToArray();

        return succeeded;
    }

    private bool TryReadCandidate(
        ITaskItem item,
        string path,
        out SidecarCandidate? candidate)
    {
        candidate = null;
        try
        {
            var bytes = File.ReadAllBytes(path);
            if (!LinkedRegistrationSidecarCodec.TryRead(bytes, out var sidecar, out var error) ||
                sidecar is null)
            {
                LogError(path, error.Length == 0 ? "the Sidecar is invalid" : error);
                return false;
            }

            var source = item.GetMetadata(SidecarSourceMetadata).Trim();
            if (!IsKnownSource(source))
            {
                LogError(
                    path,
                    $"candidate source metadata '{SidecarSourceMetadata}' must be Package, ProjectCompanion, or MetadataExtraction");
                return false;
            }

            candidate = new SidecarCandidate(
                path,
                sidecar.Assembly.Name,
                sidecar.Assembly.ContractHash,
                source);
            return true;
        }
        catch (Exception exception) when (
            exception is IOException or UnauthorizedAccessException or ArgumentException or
            BadImageFormatException)
        {
            LogError(path, exception.Message);
            return false;
        }
    }

    private SidecarCandidate? SelectCanonical(
        IReadOnlyList<SidecarCandidate> group,
        ref bool succeeded)
    {
        var hashes = group.Select(static candidate => candidate.ContractHash)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        if (hashes.Length > 1)
        {
            succeeded = false;
            var details = string.Join(
                "; ",
                group.OrderBy(static candidate => candidate.Path, StringComparer.Ordinal)
                    .Select(static candidate =>
                        $"{candidate.Source} '{candidate.Path}' ({candidate.ContractHash})"));
            LogError(
                group[0].AssemblyName,
                $"assembly '{group[0].AssemblyName}' has conflicting linked-registration Sidecars: {details}");
            return null;
        }

        return group.OrderBy(static candidate => SourcePriority(candidate.Source))
            .ThenBy(static candidate => candidate.Path, StringComparer.Ordinal)
            .First();
    }

    private static IReadOnlyCollection<ReferenceAssembly> ReadReferenceAssemblies(
        IEnumerable<ITaskItem> items)
    {
        var assemblies = new List<ReferenceAssembly>();
        foreach (var item in items)
        {
            var path = GetItemPath(item);
            if (!TryReadAssemblyName(path, out var name))
            {
                continue;
            }

            assemblies.Add(new ReferenceAssembly(name!, path));
        }

        return assemblies;
    }

    private static string GetItemPath(ITaskItem item)
    {
        var fullPath = item.GetMetadata("FullPath");
        return Path.GetFullPath(
            string.IsNullOrWhiteSpace(fullPath) ? item.ItemSpec : fullPath);
    }

    private static bool TryReadAssemblyName(string path, out string? assemblyName)
    {
        assemblyName = null;
        try
        {
            using var stream = File.OpenRead(path);
            using var peReader = new PEReader(stream);
            if (!peReader.HasMetadata)
            {
                return false;
            }

            var reader = peReader.GetMetadataReader();
            if (!reader.IsAssembly)
            {
                return false;
            }

            assemblyName = reader.GetString(reader.GetAssemblyDefinition().Name);
            return !string.IsNullOrWhiteSpace(assemblyName);
        }
        catch (Exception exception) when (
            exception is IOException or UnauthorizedAccessException or ArgumentException or
            BadImageFormatException)
        {
            return false;
        }
    }

    private void LogError(string source, string detail)
    {
        BuildEngine.LogErrorEvent(new BuildErrorEventArgs(
            subcategory: "LinkedRegistration",
            DiagnosticCode,
            source,
            lineNumber: 0,
            columnNumber: 0,
            endLineNumber: 0,
            endColumnNumber: 0,
            $"Cannot resolve linked-registration Sidecar candidates: {detail}",
            helpKeyword: null,
            senderName: nameof(ResolveLinkedRegistrationSidecarCandidatesTask)));
    }

    private static int SourcePriority(string source)
    {
        return source switch
        {
            "ProjectCompanion" => 0,
            "Package" => 1,
            "MetadataExtraction" => 2,
            _ => 3
        };
    }

    private static bool IsKnownSource(string source)
    {
        return source is "ProjectCompanion" or "Package" or "MetadataExtraction";
    }

    private sealed class SidecarCandidate
    {
        internal SidecarCandidate(
            string path,
            string assemblyName,
            string contractHash,
            string source)
        {
            Path = path;
            AssemblyName = assemblyName;
            ContractHash = contractHash;
            Source = source;
        }

        internal string Path { get; }

        internal string AssemblyName { get; }

        internal string ContractHash { get; }

        internal string Source { get; }
    }

    private sealed class ReferenceAssembly
    {
        internal ReferenceAssembly(string name, string path)
        {
            Name = name;
            Path = path;
        }

        internal string Name { get; }

        internal string Path { get; }
    }
}
