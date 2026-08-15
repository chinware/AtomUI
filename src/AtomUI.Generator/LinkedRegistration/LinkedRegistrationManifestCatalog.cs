using AtomUI.Generator.Diagnostics;
using AtomUI.Generator.LinkedRegistration.Manifest;
using AtomUI.Generator.LinkedRegistration.Model;
using Microsoft.CodeAnalysis;

namespace AtomUI.Generator.LinkedRegistration;

internal sealed class LinkedRegistrationManifestCatalog
{
    private const string AssemblyMetadataAttribute =
        "System.Reflection.AssemblyMetadataAttribute";
    private readonly Action<Diagnostic> _reportDiagnostic;
    private readonly Dictionary<IAssemblySymbol, string> _packageByAssembly =
        new(SymbolEqualityComparer.Default);
    private readonly List<LinkedUsageInfo> _referencedUsagesToValidate = [];

    private LinkedRegistrationManifestCatalog(Action<Diagnostic> reportDiagnostic)
    {
        _reportDiagnostic = reportDiagnostic;
    }

    internal Dictionary<string, LinkedPackageManifestRecord> Packages { get; } =
        new(StringComparer.Ordinal);
    internal Dictionary<string, HashSet<string>> PackageIdsByEntry { get; } =
        new(StringComparer.Ordinal);
    internal Dictionary<string, LinkedControlMapManifestRecord> ControlMaps { get; } =
        new(StringComparer.Ordinal);
    internal Dictionary<string, LinkedUnitManifestRecord> Units { get; } =
        new(StringComparer.Ordinal);
    internal Dictionary<string, string> PackageByUnit { get; } =
        new(StringComparer.Ordinal);
    internal List<LinkedUsageInfo> ReferencedUsages { get; } = [];
    internal Dictionary<string, HashSet<string>> XmlNamespaces { get; } =
        new(StringComparer.Ordinal);
    internal Dictionary<string, HashSet<string>> XmlNamespacePackages { get; } =
        new(StringComparer.Ordinal);
    internal HashSet<string> PlanOwners { get; } = new(StringComparer.Ordinal);
    internal bool HasErrors { get; private set; }

    internal static LinkedRegistrationManifestCatalog Create(
        Compilation compilation,
        Action<Diagnostic> reportDiagnostic)
    {
        var catalog = new LinkedRegistrationManifestCatalog(reportDiagnostic);
        foreach (var assembly in compilation.SourceModule.ReferencedAssemblySymbols.OrderBy(
                     static item => item.Identity.Name,
                     StringComparer.Ordinal))
        {
            catalog.ReadAssembly(assembly, includeUsages: true);
        }
        catalog.ReadAssembly(compilation.Assembly, includeUsages: false);
        catalog.ValidateRelationships();
        return catalog;
    }

    internal bool TryGetPackageId(IAssemblySymbol assembly, out string packageId)
    {
        return _packageByAssembly.TryGetValue(assembly, out packageId!);
    }

    private void ReadAssembly(IAssemblySymbol assembly, bool includeUsages)
    {
        var records = new List<LinkedRegistrationManifestRecord>();
        var xmlNamespaces = new HashSet<string>(StringComparer.Ordinal);
        foreach (var attribute in assembly.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() == AssemblyMetadataAttribute &&
                attribute.ConstructorArguments.Length == 2 &&
                attribute.ConstructorArguments[0].Value is string key &&
                attribute.ConstructorArguments[1].Value is string value &&
                key.StartsWith(LinkedRegistrationProtocol.MetadataPrefix, StringComparison.Ordinal))
            {
                if (key == LinkedRegistrationProtocol.PlanMarkerKey)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        ReportIncompatibleInput(
                            assembly.Identity.Name,
                            "application plan marker owner is empty");
                    }
                    else
                    {
                        PlanOwners.Add(value.Trim());
                    }
                    continue;
                }
                if (!IsSupportedManifestKey(key))
                {
                    ReportIncompatibleInput(assembly.Identity.Name, $"unsupported manifest key '{key}'");
                    continue;
                }
                if (!LinkedRegistrationManifestCodec.TryDecode(key, value, out var record, out var error))
                {
                    ReportInvalidManifest(assembly.Identity.Name, error);
                    continue;
                }
                if (record is not null)
                {
                    records.Add(record);
                }
                continue;
            }

            if (attribute.AttributeClass?.ToDisplayString() ==
                "Avalonia.Metadata.XmlnsDefinitionAttribute" &&
                attribute.ConstructorArguments.Length >= 2 &&
                attribute.ConstructorArguments[0].Value is string xmlNamespace &&
                attribute.ConstructorArguments[1].Value is string clrNamespace)
            {
                if (!XmlNamespaces.TryGetValue(xmlNamespace, out var namespaces))
                {
                    namespaces = new HashSet<string>(StringComparer.Ordinal);
                    XmlNamespaces.Add(xmlNamespace, namespaces);
                }
                namespaces.Add(clrNamespace);
                xmlNamespaces.Add(xmlNamespace);
            }
        }

        var packageRecords = records.OfType<LinkedPackageManifestRecord>().Distinct().ToArray();
        var isControlPackage = packageRecords.Length != 0;
        var controlPackageId = packageRecords.Length == 1
            ? packageRecords[0].PackageId
            : null;
        foreach (var record in records)
        {
            var includeReferencedUsage = includeUsages &&
                                         (!isControlPackage ||
                                          record is LinkedUsageManifestRecord
                                          {
                                              Kind: LinkedUsageKind.PackageRoot
                                          } packageRoot &&
                                          string.Equals(
                                              packageRoot.Identity,
                                              controlPackageId,
                                              StringComparison.Ordinal));
            AddRecord(
                record,
                includeReferencedUsage,
                validateUsage: includeUsages);
        }
        var assemblyPackages = packageRecords.Select(static package => package.PackageId)
                                             .Distinct(StringComparer.Ordinal)
                                             .ToArray();
        if (assemblyPackages.Length == 1 && Packages.ContainsKey(assemblyPackages[0]))
        {
            _packageByAssembly[assembly] = assemblyPackages[0];
            foreach (var xmlNamespace in xmlNamespaces)
            {
                if (!XmlNamespacePackages.TryGetValue(xmlNamespace, out var packages))
                {
                    packages = new HashSet<string>(StringComparer.Ordinal);
                    XmlNamespacePackages.Add(xmlNamespace, packages);
                }
                packages.Add(assemblyPackages[0]);
            }
        }
        else if (assemblyPackages.Length > 1)
        {
            ReportInvalidManifest(
                assembly.Identity.Name,
                "an assembly cannot declare more than one Package identity");
        }
    }

    private void AddRecord(
        LinkedRegistrationManifestRecord? record,
        bool includeUsages,
        bool validateUsage)
    {
        switch (record)
        {
            case LinkedPackageManifestRecord package:
                if (!TryAddRecord(Packages, package.PackageId, package, "Package"))
                {
                    break;
                }
                foreach (var entry in SplitEntryMethodMetadataNames(package.EntryMethodMetadataNames))
                {
                    if (!PackageIdsByEntry.TryGetValue(entry, out var packageIds))
                    {
                        packageIds = new HashSet<string>(StringComparer.Ordinal);
                        PackageIdsByEntry.Add(entry, packageIds);
                    }
                    packageIds.Add(package.PackageId);
                }
                break;
            case LinkedUnitManifestRecord unit:
                if (TryAddRecord(Units, unit.UnitId, unit, "Registration Unit"))
                {
                    PackageByUnit[unit.UnitId] = unit.PackageId;
                }
                break;
            case LinkedControlMapManifestRecord controlMap:
                TryAddRecord(ControlMaps, controlMap.MetadataName, controlMap, "ControlMap");
                break;
            case LinkedUsageManifestRecord usage:
                var usageInfo = new LinkedUsageInfo(
                    usage.Kind,
                    usage.Identity,
                    usage.Source,
                    usage.Line,
                    usage.Column,
                    string.Empty,
                    null);
                if (validateUsage && !ContainsUsage(_referencedUsagesToValidate, usageInfo))
                {
                    _referencedUsagesToValidate.Add(usageInfo);
                }
                if (includeUsages && !ContainsUsage(ReferencedUsages, usageInfo))
                {
                    ReferencedUsages.Add(usageInfo);
                }
                break;
        }
    }

    private void ValidateRelationships()
    {
        foreach (var unit in Units.Values)
        {
            if (!Packages.ContainsKey(unit.PackageId))
            {
                ReportInvalidManifest(
                    unit.PackageId,
                    $"Registration Unit '{unit.UnitId}' references an unknown Package");
            }
        }
        foreach (var controlMap in ControlMaps.Values)
        {
            if (!Packages.ContainsKey(controlMap.PackageId))
            {
                ReportInvalidManifest(
                    controlMap.PackageId,
                    $"ControlMap '{controlMap.MetadataName}' references an unknown Package");
            }
            if (!Units.TryGetValue(controlMap.UnitId, out var unit))
            {
                ReportInvalidManifest(
                    controlMap.PackageId,
                    $"ControlMap '{controlMap.MetadataName}' references an unknown Registration Unit '{controlMap.UnitId}'");
            }
            else if (!string.Equals(unit.PackageId, controlMap.PackageId, StringComparison.Ordinal))
            {
                ReportInvalidManifest(
                    controlMap.PackageId,
                    $"ControlMap '{controlMap.MetadataName}' conflicts with Registration Unit '{controlMap.UnitId}'");
            }
        }
        foreach (var usage in _referencedUsagesToValidate)
        {
            var valid = usage.Kind switch
            {
                LinkedUsageKind.Control => ControlMaps.ContainsKey(usage.Identity),
                LinkedUsageKind.UnitRoot => Units.ContainsKey(usage.Identity),
                LinkedUsageKind.PackageRoot or LinkedUsageKind.Entry =>
                    Packages.ContainsKey(usage.Identity),
                _ => false
            };
            if (!valid)
            {
                ReportInvalidManifest(
                    usage.Identity,
                    $"Usage '{usage.Kind}' references an unknown identity '{usage.Identity}'");
            }
        }
    }

    private static bool ContainsUsage(
        IEnumerable<LinkedUsageInfo> usages,
        LinkedUsageInfo candidate)
    {
        return usages.Any(existing =>
            existing.Kind == candidate.Kind &&
            existing.Identity == candidate.Identity &&
            existing.Source == candidate.Source &&
            existing.Line == candidate.Line &&
            existing.Column == candidate.Column);
    }

    private static IEnumerable<string> SplitEntryMethodMetadataNames(string entries)
    {
        return entries.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(static entry => entry.Trim())
                      .Where(static entry => entry.Length != 0)
                      .Distinct(StringComparer.Ordinal)
                      .OrderBy(static entry => entry, StringComparer.Ordinal);
    }

    private bool TryAddRecord<TRecord>(
        Dictionary<string, TRecord> records,
        string identity,
        TRecord record,
        string kind)
        where TRecord : LinkedRegistrationManifestRecord
    {
        if (!records.TryGetValue(identity, out var existing))
        {
            records.Add(identity, record);
            return true;
        }
        if (EqualityComparer<TRecord>.Default.Equals(existing, record))
        {
            return true;
        }
        ReportInvalidManifest(identity, $"conflicting duplicate {kind} record");
        return false;
    }

    private void ReportInvalidManifest(string packageId, string detail)
    {
        HasErrors = true;
        _reportDiagnostic(Diagnostic.Create(
            AtomUIDiagnosticDescriptors.LinkedPackageDefinitionInvalid,
            Location.None,
            packageId,
            detail));
    }

    private void ReportIncompatibleInput(string input, string detail)
    {
        HasErrors = true;
        _reportDiagnostic(Diagnostic.Create(
            AtomUIDiagnosticDescriptors.LinkedManifestVersionMismatch,
            Location.None,
            input,
            detail));
    }

    private static bool IsSupportedManifestKey(string key)
    {
        return key == LinkedRegistrationProtocol.PackageManifestKey ||
               key == LinkedRegistrationProtocol.UnitManifestKey ||
               key == LinkedRegistrationProtocol.ControlMapManifestKey ||
               key == LinkedRegistrationProtocol.UsageManifestKey;
    }
}
