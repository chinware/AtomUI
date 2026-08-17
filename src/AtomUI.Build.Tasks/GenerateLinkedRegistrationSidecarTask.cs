using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using AtomUI.Generator.LinkedRegistration;
using AtomUI.Generator.LinkedRegistration.Manifest;
using AtomUI.LinkedRegistration.Protocol;
using Microsoft.Build.Framework;

namespace AtomUI.Build.Tasks;

public sealed class GenerateLinkedRegistrationSidecarTask : ITask
{
    private const string DiagnosticCode = "ATOMUILINK006";
    private const string AssemblyMetadataNamespace = "System.Reflection";
    private const string AssemblyMetadataName = "AssemblyMetadataAttribute";

    [Required]
    public string AssemblyPath { get; set; } = string.Empty;

    [Required]
    public string OutputPath { get; set; } = string.Empty;

    public string TargetFramework { get; set; } = string.Empty;

    [Output]
    public string SidecarPath { get; private set; } = string.Empty;

    [Output]
    public bool WroteFile { get; private set; }

    public IBuildEngine BuildEngine { get; set; } = null!;

    public ITaskHost HostObject { get; set; } = null!;

    public bool Execute()
    {
        try
        {
            using var stream = File.OpenRead(AssemblyPath);
            using var peReader = new PEReader(stream);
            if (!peReader.HasMetadata)
            {
                return LogError("The file does not contain managed metadata.");
            }
            var reader = peReader.GetMetadataReader();
            if (!reader.IsAssembly)
            {
                return LogError("The file is not a managed assembly.");
            }

            if (!TryReadRecords(reader, out var records, out var analysisBudgetExceeded, out var error))
            {
                return LogError(error);
            }
            if (analysisBudgetExceeded)
            {
                records = records.Where(static record =>
                        record is LinkedPackageManifestRecord ||
                        record is LinkedUsageManifestRecord
                        {
                            Kind: LinkedUsageKind.Entry or LinkedUsageKind.PackageRoot
                        })
                    .ToList();
            }
            var assemblyName = reader.GetString(reader.GetAssemblyDefinition().Name);
            if (!TryCreateSidecar(assemblyName, records, out var sidecar, out error))
            {
                return LogError(error);
            }
            sidecar.Assembly.TargetFramework = TargetFramework.Trim();
            var bytes = LinkedRegistrationSidecarCodec.Write(sidecar);
            if (analysisBudgetExceeded ||
                bytes.Length > LinkedRegistrationAnalysisBudget.MaxSidecarBytes)
            {
                sidecar = CreateBudgetFallbackSidecar(sidecar);
                bytes = LinkedRegistrationSidecarCodec.Write(sidecar);
                if (bytes.Length > LinkedRegistrationAnalysisBudget.MaxSidecarBytes)
                {
                    return LogError(
                        $"the budget fallback Sidecar is {bytes.Length} bytes, exceeding the {LinkedRegistrationAnalysisBudget.MaxSidecarBytes} byte limit");
                }
            }
            var fullOutputPath = Path.GetFullPath(OutputPath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullOutputPath)!);
            if (!File.Exists(fullOutputPath) || !File.ReadAllBytes(fullOutputPath).SequenceEqual(bytes))
            {
                File.WriteAllBytes(fullOutputPath, bytes);
                WroteFile = true;
            }
            SidecarPath = fullOutputPath;
            return true;
        }
        catch (Exception exception) when (
            exception is IOException or UnauthorizedAccessException or ArgumentException or
            BadImageFormatException or InvalidOperationException)
        {
            return LogError(exception.Message);
        }
    }

    private static bool TryReadRecords(
        MetadataReader reader,
        out List<LinkedRegistrationManifestRecord> records,
        out bool analysisBudgetExceeded,
        out string error)
    {
        records = [];
        analysisBudgetExceeded = false;
        error = string.Empty;
        foreach (var attributeHandle in reader.GetAssemblyDefinition().GetCustomAttributes())
        {
            var attribute = reader.GetCustomAttribute(attributeHandle);
            if (!IsAssemblyMetadataAttribute(reader, attribute.Constructor) ||
                !TryReadMetadataValue(reader, attribute, out var key, out var value) ||
                key is null || value is null ||
                !key.StartsWith(LinkedRegistrationProtocol.MetadataPrefix, StringComparison.Ordinal) ||
                key == LinkedRegistrationProtocol.PlanMarkerKey)
            {
                continue;
            }
            if (!LinkedRegistrationManifestCodec.TryDecode(key, value, out var record, out error))
            {
                return false;
            }
            if (record is not null)
            {
                if (records.Count >= LinkedRegistrationAnalysisBudget.MaxManifestRecords &&
                    record is not LinkedPackageManifestRecord &&
                    record is not LinkedUsageManifestRecord
                    {
                        Kind: LinkedUsageKind.Entry or LinkedUsageKind.PackageRoot
                    })
                {
                    analysisBudgetExceeded = true;
                }
                else
                {
                    records.Add(record);
                }
            }
        }
        return true;
    }

    private static LinkedRegistrationSidecar CreateBudgetFallbackSidecar(
        LinkedRegistrationSidecar sidecar)
    {
        var fallbackPackages = sidecar.Packages.Select(static package => package.Id)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static packageId => packageId, StringComparer.Ordinal)
            .ToArray();
        var fallbacks = fallbackPackages.Length == 0
            ? new[]
            {
                new LinkedSidecarFallback
                {
                    PackageId = string.Empty,
                    Reason = "AnalysisBudgetExceeded",
                    Source = "<sidecar>",
                    Line = 0,
                    Column = 0
                }
            }
            : fallbackPackages.Select(static packageId => new LinkedSidecarFallback
            {
                PackageId = packageId,
                Reason = "AnalysisBudgetExceeded",
                Source = "<sidecar>",
                Line = 0,
                Column = 0
            }).ToArray();
        var usages = sidecar.Usages
            .Where(static usage => usage.Kind == nameof(LinkedUsageKind.Entry) ||
                                  usage.Kind == nameof(LinkedUsageKind.PackageRoot))
            .GroupBy(static usage => usage.Kind + "\u001f" + usage.Identity, StringComparer.Ordinal)
            .Select(static group => group.First())
            .ToArray();
        return new LinkedRegistrationSidecar
        {
            ProtocolMajor = sidecar.ProtocolMajor,
            ProtocolMinor = sidecar.ProtocolMinor,
            Producer = sidecar.Producer,
            Assembly = sidecar.Assembly,
            Packages = sidecar.Packages.Select(static package => new LinkedSidecarPackage
            {
                Id = package.Id,
                AssemblyName = package.AssemblyName,
                Granularity = "Package",
                EntryMethods = package.EntryMethods,
                FullFragment = package.FullFragment,
                SharedFragment = package.SharedFragment,
                Units = [],
                UnitEdges = [],
                RootUnits = []
            }).ToArray(),
            Usages = usages,
            Fallbacks = fallbacks
        };
    }

    private static bool TryCreateSidecar(
        string assemblyName,
        IReadOnlyList<LinkedRegistrationManifestRecord> records,
        out LinkedRegistrationSidecar sidecar,
        out string error)
    {
        error = string.Empty;
        var packageRecords = records.OfType<LinkedPackageManifestRecord>()
            .Distinct()
            .OrderBy(static package => package.PackageId, StringComparer.Ordinal)
            .ToArray();
        var unitRecords = records.OfType<LinkedUnitManifestRecord>().Distinct().ToArray();
        var controlRecords = records.OfType<LinkedControlMapManifestRecord>().Distinct().ToArray();
        var edgeRecords = records.OfType<LinkedUnitEdgeManifestRecord>().Distinct().ToArray();
        var rootRecords = records.OfType<LinkedRootUnitManifestRecord>().Distinct().ToArray();

        var packages = new List<LinkedSidecarPackage>(packageRecords.Length);
        foreach (var package in packageRecords)
        {
            if (!string.Equals(package.AssemblyName, assemblyName, StringComparison.Ordinal))
            {
                sidecar = new LinkedRegistrationSidecar();
                error = $"Package '{package.PackageId}' declares assembly '{package.AssemblyName}', but the compiled assembly is '{assemblyName}'.";
                return false;
            }
            var units = unitRecords.Where(unit => string.Equals(
                    unit.PackageId,
                    package.PackageId,
                    StringComparison.Ordinal))
                .OrderBy(static unit => unit.OrderKey)
                .ThenBy(static unit => unit.UnitId, StringComparer.Ordinal)
                .Select(unit => new LinkedSidecarUnit
                {
                    Id = unit.UnitId,
                    FragmentType = unit.FragmentType,
                    FragmentMethod = unit.FragmentMethod,
                    OrderKey = unit.OrderKey,
                    Controls = controlRecords.Where(control => string.Equals(
                            control.PackageId,
                            package.PackageId,
                            StringComparison.Ordinal) &&
                        string.Equals(control.UnitId, unit.UnitId, StringComparison.Ordinal))
                        .Select(static control => control.MetadataName)
                        .Distinct(StringComparer.Ordinal)
                        .OrderBy(static control => control, StringComparer.Ordinal)
                        .ToArray()
                })
                .ToArray();
            var unitIds = new HashSet<string>(units.Select(static unit => unit.Id), StringComparer.Ordinal);
            var packageEdges = edgeRecords.Where(edge => string.Equals(
                    edge.PackageId,
                    package.PackageId,
                    StringComparison.Ordinal))
                .ToArray();
            if (packageEdges.Any(edge =>
                    !unitIds.Contains(edge.SourceUnitId) || !unitIds.Contains(edge.TargetUnitId)))
            {
                sidecar = new LinkedRegistrationSidecar();
                error = $"Package '{package.PackageId}' contains a UnitEdge that references an unknown Unit.";
                return false;
            }
            var rootUnits = rootRecords.Where(root => string.Equals(
                    root.PackageId,
                    package.PackageId,
                    StringComparison.Ordinal))
                .Select(static root => root.UnitId)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(static unitId => unitId, StringComparer.Ordinal)
                .ToArray();
            if (rootUnits.Any(unitId => !unitIds.Contains(unitId)))
            {
                sidecar = new LinkedRegistrationSidecar();
                error = $"Package '{package.PackageId}' contains a Package Core root that references an unknown Unit.";
                return false;
            }

            packages.Add(new LinkedSidecarPackage
            {
                Id = package.PackageId,
                AssemblyName = package.AssemblyName,
                Granularity = package.Granularity.Length == 0
                    ? (units.Length > 1 ? "Directory" : "Package")
                    : package.Granularity,
                EntryMethods = SplitEntries(package.EntryMethodMetadataNames),
                FullFragment = new LinkedSidecarFragment
                {
                    Type = package.FullFragmentType,
                    Method = package.FullFragmentMethod
                },
                SharedFragment = package.PackageSharedFragmentType is null
                    ? null
                    : new LinkedSidecarFragment
                    {
                        Type = package.PackageSharedFragmentType,
                        Method = package.PackageSharedFragmentMethod!
                    },
                Units = units,
                UnitEdges = packageEdges.Select(static edge => new LinkedSidecarUnitEdge
                    {
                        SourceUnitId = edge.SourceUnitId,
                        TargetUnitId = edge.TargetUnitId,
                        EvidenceKind = edge.EvidenceKind.ToString()
                    })
                    .ToArray(),
                RootUnits = rootUnits
            });
        }

        sidecar = new LinkedRegistrationSidecar
        {
            Producer = "AtomUI.Generator.LinkedPublish/6.0",
            Assembly = new LinkedSidecarAssembly
            {
                Name = assemblyName
            },
            Packages = packages.ToArray(),
            Usages = records.OfType<LinkedUsageManifestRecord>()
                .Select(static usage => new LinkedSidecarUsage
                {
                    Kind = usage.Kind.ToString(),
                    Identity = usage.Identity,
                    Source = NormalizeSource(usage.Source),
                    Line = usage.Line,
                    Column = usage.Column
                })
                .ToArray(),
            Fallbacks = records.OfType<LinkedFallbackManifestRecord>()
                .Select(static fallback => new LinkedSidecarFallback
                {
                    PackageId = fallback.PackageId,
                    Reason = fallback.Reason,
                    Source = NormalizeSource(fallback.Source),
                    Line = fallback.Line,
                    Column = fallback.Column
                })
                .ToArray()
        };
        return true;
    }

    private static string[] SplitEntries(string entries)
    {
        return entries.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(static entry => entry.Trim())
            .Where(static entry => entry.Length != 0)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static entry => entry, StringComparer.Ordinal)
            .ToArray();
    }

    private static string NormalizeSource(string source)
    {
        var normalized = source.Replace('\\', '/');
        return Path.IsPathRooted(normalized) ? Path.GetFileName(normalized) : normalized;
    }

    private static bool IsAssemblyMetadataAttribute(MetadataReader reader, EntityHandle constructor)
    {
        EntityHandle declaringType;
        switch (constructor.Kind)
        {
            case HandleKind.MemberReference:
                declaringType = reader.GetMemberReference((MemberReferenceHandle)constructor).Parent;
                break;
            case HandleKind.MethodDefinition:
                declaringType = reader.GetMethodDefinition((MethodDefinitionHandle)constructor).GetDeclaringType();
                break;
            default:
                return false;
        }
        return IsType(reader, declaringType, AssemblyMetadataNamespace, AssemblyMetadataName);
    }

    private static bool IsType(
        MetadataReader reader,
        EntityHandle handle,
        string expectedNamespace,
        string expectedName)
    {
        StringHandle namespaceHandle;
        StringHandle nameHandle;
        switch (handle.Kind)
        {
            case HandleKind.TypeReference:
                var typeReference = reader.GetTypeReference((TypeReferenceHandle)handle);
                namespaceHandle = typeReference.Namespace;
                nameHandle = typeReference.Name;
                break;
            case HandleKind.TypeDefinition:
                var typeDefinition = reader.GetTypeDefinition((TypeDefinitionHandle)handle);
                namespaceHandle = typeDefinition.Namespace;
                nameHandle = typeDefinition.Name;
                break;
            default:
                return false;
        }
        return reader.StringComparer.Equals(namespaceHandle, expectedNamespace) &&
               reader.StringComparer.Equals(nameHandle, expectedName);
    }

    private static bool TryReadMetadataValue(
        MetadataReader reader,
        CustomAttribute attribute,
        out string? key,
        out string? value)
    {
        key = null;
        value = null;
        try
        {
            var valueReader = reader.GetBlobReader(attribute.Value);
            if (valueReader.ReadUInt16() != 1)
            {
                return false;
            }
            key = valueReader.ReadSerializedString();
            value = valueReader.ReadSerializedString();
            return key is not null && value is not null;
        }
        catch (BadImageFormatException)
        {
            return false;
        }
    }

    private bool LogError(string detail)
    {
        BuildEngine.LogErrorEvent(new BuildErrorEventArgs(
            subcategory: "LinkedRegistration",
            DiagnosticCode,
            AssemblyPath,
            lineNumber: 0,
            columnNumber: 0,
            endLineNumber: 0,
            endColumnNumber: 0,
            $"Cannot generate linked-registration Sidecar for '{AssemblyPath}': {detail}",
            helpKeyword: null,
            senderName: nameof(GenerateLinkedRegistrationSidecarTask)));
        return false;
    }
}
