using AtomUI.LinkedRegistration.Protocol;
using Microsoft.Build.Framework;

namespace AtomUI.Build.Tasks;

public sealed class DiscoverLinkedRegistrationConsumerReferencesTask : ITask
{
    private const string DiagnosticCode = "ATOMUILINK006";

    [Required]
    public ITaskItem[] ReferencePaths { get; set; } = [];

    [Required]
    public ITaskItem[] CatalogSidecars { get; set; } = [];

    [Output]
    public ITaskItem[] ConsumerReferences { get; private set; } = [];

    public IBuildEngine BuildEngine { get; set; } = null!;

    public ITaskHost HostObject { get; set; } = null!;

    public bool Execute()
    {
        var sidecarPaths = CatalogSidecars.Select(GetItemPath).ToArray();
        if (!LinkedRegistrationPackageCatalogReader.TryRead(
                sidecarPaths,
                out var catalog,
                out var error))
        {
            return LogError(error);
        }
        var packageAssemblies = new HashSet<string>(
            catalog.Packages.Select(static package => package.AssemblyName),
            StringComparer.Ordinal);
        if (packageAssemblies.Count == 0)
        {
            return true;
        }

        var consumers = new List<ITaskItem>();
        foreach (var item in ReferencePaths)
        {
            var path = GetItemPath(item);
            if (!LinkedRegistrationAssemblyUsageExtractor.TryReadAssemblyReferences(
                    path,
                    out var assemblyName,
                    out var referencedAssemblies,
                    out _))
            {
                continue;
            }
            if (packageAssemblies.Contains(assemblyName) ||
                catalog.SidecarAssemblyNames.Contains(assemblyName) ||
                !referencedAssemblies.Any(packageAssemblies.Contains))
            {
                continue;
            }

            var consumer = new GeneratedTaskItem(path);
            consumer.SetMetadata("AtomUILinkedAssemblyName", assemblyName);
            consumers.Add(consumer);
        }

        ConsumerReferences = consumers.OrderBy(static item => item.GetMetadata("AtomUILinkedAssemblyName"), StringComparer.Ordinal)
            .ThenBy(static item => item.ItemSpec, StringComparer.Ordinal)
            .GroupBy(static item => item.GetMetadata("AtomUILinkedAssemblyName"), StringComparer.Ordinal)
            .Select(static group => group.First())
            .ToArray();
        return true;
    }

    private static string GetItemPath(ITaskItem item)
    {
        var fullPath = item.GetMetadata("FullPath");
        return Path.GetFullPath(string.IsNullOrWhiteSpace(fullPath) ? item.ItemSpec : fullPath);
    }

    private bool LogError(string detail)
    {
        BuildEngine.LogErrorEvent(new BuildErrorEventArgs(
            subcategory: "LinkedRegistration",
            DiagnosticCode,
            file: null,
            lineNumber: 0,
            columnNumber: 0,
            endLineNumber: 0,
            endColumnNumber: 0,
            $"Cannot discover linked-registration consumer references: {detail}",
            helpKeyword: null,
            senderName: nameof(DiscoverLinkedRegistrationConsumerReferencesTask)));
        return false;
    }
}
