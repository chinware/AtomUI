using System.Collections.Immutable;
using System.Text;
using AtomUI.Generator.Diagnostics;
using AtomUI.Generator.LinkedRegistration.Manifest;
using AtomUI.Generator.LinkedRegistration.Model;
using AtomUI.LinkedRegistration.Protocol;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AtomUI.Generator.LinkedRegistration;

internal sealed class LinkedRegistrationManifestCatalog
{
    private const string AssemblyMetadataAttribute =
        "System.Reflection.AssemblyMetadataAttribute";
    private const string SidecarMetadata =
        "build_metadata.AdditionalFiles.AtomUILinkedSidecar";
    private readonly Action<Diagnostic> _reportDiagnostic;
    private readonly Dictionary<IAssemblySymbol, string> _packageByAssembly =
        new(SymbolEqualityComparer.Default);
    private readonly List<LinkedUsageInfo> _referencedUsagesToValidate = [];
    private readonly HashSet<string> _referencedUsageValidationKeys = new(StringComparer.Ordinal);
    private readonly HashSet<string> _referencedUsageKeys = new(StringComparer.Ordinal);
    private readonly HashSet<LinkedUnitEdgeManifestRecord> _unitEdgeSet = [];
    private readonly HashSet<LinkedFallbackManifestRecord> _fallbackSet = [];
    private readonly HashSet<string> _sidecarAssemblies = new(StringComparer.Ordinal);
    private long _totalSidecarBytes;
    private bool _fineAnalysisDisabled;

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
    internal List<LinkedUnitEdgeManifestRecord> UnitEdges { get; } = [];
    internal Dictionary<string, HashSet<string>> RootUnitsByPackage { get; } =
        new(StringComparer.Ordinal);
    internal List<LinkedFallbackManifestRecord> Fallbacks { get; } = [];
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
        return Create(
            compilation,
            ImmutableArray<AdditionalText>.Empty,
            optionsProvider: null,
            reportDiagnostic);
    }

    internal static LinkedRegistrationManifestCatalog Create(
        Compilation compilation,
        ImmutableArray<AdditionalText> additionalTexts,
        AnalyzerConfigOptionsProvider? optionsProvider,
        Action<Diagnostic> reportDiagnostic)
    {
        var catalog = new LinkedRegistrationManifestCatalog(reportDiagnostic);
        if (optionsProvider is not null)
        {
            foreach (var text in additionalTexts.OrderBy(
                         static item => item.Path,
                         StringComparer.Ordinal))
            {
                if (optionsProvider.GetOptions(text).TryGetValue(SidecarMetadata, out var enabled) &&
                    string.Equals(enabled, "true", StringComparison.OrdinalIgnoreCase))
                {
                    catalog.ReadSidecar(text);
                }
            }
        }
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

    private void ReadSidecar(AdditionalText text)
    {
        var sourceText = text.GetText();
        if (sourceText is null || sourceText.Length == 0)
        {
            ReportIncompatibleInput(text.Path, "sidecar is empty");
            return;
        }
        if (sourceText.Length > LinkedRegistrationAnalysisBudget.MaxSidecarBytes)
        {
            DisableFineAnalysis(text.Path);
            return;
        }

        var content = sourceText.ToString();
        var bytes = Encoding.UTF8.GetBytes(content);
        if (bytes.Length > LinkedRegistrationAnalysisBudget.MaxSidecarBytes ||
            _totalSidecarBytes + bytes.Length > LinkedRegistrationAnalysisBudget.MaxTotalSidecarBytes)
        {
            DisableFineAnalysis(text.Path);
            return;
        }
        _totalSidecarBytes += bytes.Length;
        var error = string.Empty;
        if (string.IsNullOrWhiteSpace(content) ||
            !LinkedRegistrationSidecarCodec.TryRead(
                bytes,
                out var sidecar,
                out error))
        {
            ReportIncompatibleInput(text.Path, error.Length == 0 ? "sidecar is empty" : error);
            return;
        }
        if (!_sidecarAssemblies.Add(sidecar!.Assembly.Name))
        {
            ReportInvalidManifest(
                sidecar.Assembly.Name,
                "more than one linked-registration Sidecar declares the same assembly");
            return;
        }

        if (_fineAnalysisDisabled || ExceedsStructuralBudget(sidecar!))
        {
            ReadBudgetFallbackSidecar(sidecar!, text.Path);
            return;
        }

        var isControlPackage = sidecar.Packages.Length != 0;
        var ownPackageIds = new HashSet<string>(
            sidecar.Packages.Select(static package => package.Id),
            StringComparer.Ordinal);
        foreach (var package in sidecar.Packages)
        {
            AddRecord(
                new LinkedPackageManifestRecord(
                    package.Id,
                    package.AssemblyName,
                    package.Granularity,
                    string.Join(";", package.EntryMethods),
                    package.FullFragment.Type,
                    package.FullFragment.Method,
                    package.SharedFragment?.Type,
                    package.SharedFragment?.Method),
                includeUsages: false,
                validateUsage: false);
            foreach (var unit in package.Units)
            {
                AddRecord(
                    new LinkedUnitManifestRecord(
                        package.Id,
                        unit.Id,
                        unit.FragmentType,
                        unit.FragmentMethod,
                        unit.OrderKey),
                    includeUsages: false,
                    validateUsage: false);
                foreach (var control in unit.Controls)
                {
                    AddRecord(
                        new LinkedControlMapManifestRecord(package.Id, control, unit.Id),
                        includeUsages: false,
                        validateUsage: false);
                }
            }
            foreach (var edge in package.UnitEdges)
            {
                if (!Enum.TryParse(
                        edge.EvidenceKind,
                        ignoreCase: false,
                        out LinkedUnitEdgeEvidenceKind evidenceKind))
                {
                    ReportIncompatibleInput(
                        text.Path,
                        $"unknown UnitEdge evidence kind '{edge.EvidenceKind}'");
                    continue;
                }
                AddRecord(
                    new LinkedUnitEdgeManifestRecord(
                        package.Id,
                        edge.SourceUnitId,
                        edge.TargetUnitId,
                        evidenceKind),
                    includeUsages: false,
                    validateUsage: false);
            }
            foreach (var rootUnit in package.RootUnits)
            {
                AddRecord(
                    new LinkedRootUnitManifestRecord(package.Id, rootUnit),
                    includeUsages: false,
                    validateUsage: false);
            }
        }
        foreach (var usage in sidecar.Usages)
        {
            if (!Enum.TryParse(usage.Kind, ignoreCase: false, out LinkedUsageKind kind))
            {
                ReportIncompatibleInput(text.Path, $"unknown Usage kind '{usage.Kind}'");
                continue;
            }
            var includeUsage = !isControlPackage ||
                               (kind == LinkedUsageKind.PackageRoot &&
                                ownPackageIds.Contains(usage.Identity));
            AddRecord(
                new LinkedUsageManifestRecord(
                    kind,
                    usage.Identity,
                    usage.Source,
                    usage.Line,
                    usage.Column),
                includeUsage,
                validateUsage: true);
        }
        foreach (var fallback in sidecar.Fallbacks)
        {
            AddRecord(
                new LinkedFallbackManifestRecord(
                    fallback.PackageId,
                    fallback.Reason,
                    fallback.Source,
                    fallback.Line,
                    fallback.Column),
                includeUsages: false,
                validateUsage: false);
        }
    }

    private bool ExceedsStructuralBudget(LinkedRegistrationSidecar sidecar)
    {
        long unitCount = 0;
        long edgeCount = 0;
        long rootCount = 0;
        long controlCount = 0;
        foreach (var package in sidecar.Packages)
        {
            unitCount += package.Units.Length;
            edgeCount += package.UnitEdges.Length;
            rootCount += package.RootUnits.Length;
            foreach (var unit in package.Units)
            {
                controlCount += unit.Controls.Length;
            }
        }

        return Packages.Count + (long)sidecar.Packages.Length > LinkedRegistrationAnalysisBudget.MaxPackages ||
               Units.Count + unitCount > LinkedRegistrationAnalysisBudget.MaxRegistrationUnits ||
               UnitEdges.Count + edgeCount > LinkedRegistrationAnalysisBudget.MaxUnitEdges ||
               RootUnitsByPackage.Sum(static item => item.Value.Count) + rootCount >
               LinkedRegistrationAnalysisBudget.MaxRootUnits ||
               ControlMaps.Count + controlCount > LinkedRegistrationAnalysisBudget.MaxControlMappings ||
               ReferencedUsages.Count + (long)sidecar.Usages.Length >
               LinkedRegistrationAnalysisBudget.MaxUsages ||
               Fallbacks.Count + (long)sidecar.Fallbacks.Length >
               LinkedRegistrationAnalysisBudget.MaxFallbacks;
    }

    private void ReadBudgetFallbackSidecar(LinkedRegistrationSidecar sidecar, string source)
    {
        foreach (var package in sidecar.Packages)
        {
            AddRecord(
                new LinkedPackageManifestRecord(
                    package.Id,
                    package.AssemblyName,
                    package.Granularity,
                    string.Join(";", package.EntryMethods),
                    package.FullFragment.Type,
                    package.FullFragment.Method,
                    package.SharedFragment?.Type,
                    package.SharedFragment?.Method),
                includeUsages: false,
                validateUsage: false);
        }
        foreach (var usage in sidecar.Usages)
        {
            if (!Enum.TryParse(usage.Kind, ignoreCase: false, out LinkedUsageKind kind) ||
                kind is not (LinkedUsageKind.Entry or LinkedUsageKind.PackageRoot))
            {
                continue;
            }
            AddRecord(
                new LinkedUsageManifestRecord(
                    kind,
                    usage.Identity,
                    usage.Source,
                    usage.Line,
                    usage.Column),
                includeUsages: true,
                validateUsage: true);
        }
        DisableFineAnalysis(source);
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
                if (_sidecarAssemblies.Contains(assembly.Identity.Name))
                {
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
            if (packageRecords.Length != 0 && record is not LinkedPackageManifestRecord)
            {
                continue;
            }
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
        if (includeUsages && packageRecords.Length != 0)
        {
            foreach (var package in packageRecords)
            {
                AddRecord(
                    new LinkedFallbackManifestRecord(
                        package.PackageId,
                        "MissingSidecar",
                        assembly.Identity.Name,
                        0,
                        0),
                    includeUsages: false,
                    validateUsage: false);
            }
        }
        var assemblyPackages = Packages.Values.Where(package => string.Equals(
                                                 package.AssemblyName,
                                                 assembly.Identity.Name,
                                                 StringComparison.Ordinal))
                                             .Select(static package => package.PackageId)
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
        if (_fineAnalysisDisabled && record is not LinkedPackageManifestRecord &&
            record is not LinkedUsageManifestRecord
            {
                Kind: LinkedUsageKind.Entry or LinkedUsageKind.PackageRoot
            })
        {
            return;
        }

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
                if (Units.Count >= LinkedRegistrationAnalysisBudget.MaxRegistrationUnits)
                {
                    DisableFineAnalysis(unit.PackageId);
                    break;
                }
                if (TryAddRecord(Units, unit.UnitId, unit, "Registration Unit"))
                {
                    PackageByUnit[unit.UnitId] = unit.PackageId;
                }
                break;
            case LinkedControlMapManifestRecord controlMap:
                if (ControlMaps.Count >= LinkedRegistrationAnalysisBudget.MaxControlMappings)
                {
                    DisableFineAnalysis(controlMap.PackageId);
                    break;
                }
                TryAddRecord(ControlMaps, controlMap.MetadataName, controlMap, "ControlMap");
                break;
            case LinkedUnitEdgeManifestRecord edge:
                if (UnitEdges.Count >= LinkedRegistrationAnalysisBudget.MaxUnitEdges)
                {
                    DisableFineAnalysis(edge.PackageId);
                    break;
                }
                if (_unitEdgeSet.Add(edge))
                {
                    UnitEdges.Add(edge);
                }
                break;
            case LinkedRootUnitManifestRecord rootUnit:
                if (RootUnitsByPackage.Sum(static item => item.Value.Count) >=
                    LinkedRegistrationAnalysisBudget.MaxRootUnits)
                {
                    DisableFineAnalysis(rootUnit.PackageId);
                    break;
                }
                if (!RootUnitsByPackage.TryGetValue(rootUnit.PackageId, out var rootUnits))
                {
                    rootUnits = new HashSet<string>(StringComparer.Ordinal);
                    RootUnitsByPackage.Add(rootUnit.PackageId, rootUnits);
                }
                rootUnits.Add(rootUnit.UnitId);
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
                var usageKey = GetUsageKey(usageInfo);
                if (validateUsage && _referencedUsageValidationKeys.Add(usageKey))
                {
                    _referencedUsagesToValidate.Add(usageInfo);
                }
                if (includeUsages && _referencedUsageKeys.Add(usageKey))
                {
                    if (ReferencedUsages.Count >= LinkedRegistrationAnalysisBudget.MaxUsages)
                    {
                        DisableFineAnalysis(usage.Identity);
                        break;
                    }
                    ReferencedUsages.Add(usageInfo);
                }
                break;
            case LinkedFallbackManifestRecord fallback:
                if (_fallbackSet.Add(fallback))
                {
                    if (Fallbacks.Count >= LinkedRegistrationAnalysisBudget.MaxFallbacks)
                    {
                        DisableFineAnalysis(fallback.PackageId);
                        break;
                    }
                    Fallbacks.Add(fallback);
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
        foreach (var edge in UnitEdges)
        {
            if (!Units.TryGetValue(edge.SourceUnitId, out var sourceUnit) ||
                !Units.TryGetValue(edge.TargetUnitId, out var targetUnit) ||
                !string.Equals(sourceUnit.PackageId, edge.PackageId, StringComparison.Ordinal) ||
                !string.Equals(targetUnit.PackageId, edge.PackageId, StringComparison.Ordinal))
            {
                ReportInvalidManifest(
                    edge.PackageId,
                    $"UnitEdge '{edge.SourceUnitId}' -> '{edge.TargetUnitId}' does not resolve within its Package");
            }
        }
        foreach (var packageRoots in RootUnitsByPackage)
        {
            foreach (var unitId in packageRoots.Value)
            {
                if (!Units.TryGetValue(unitId, out var unit) ||
                    !string.Equals(unit.PackageId, packageRoots.Key, StringComparison.Ordinal))
                {
                    ReportInvalidManifest(
                        packageRoots.Key,
                        $"Package root references unknown Registration Unit '{unitId}'");
                }
            }
        }
        foreach (var fallback in Fallbacks)
        {
            if (fallback.PackageId.Length != 0 && !Packages.ContainsKey(fallback.PackageId))
            {
                ReportInvalidManifest(
                    fallback.PackageId,
                    $"Fallback '{fallback.Reason}' references an unknown Package");
            }
        }
    }

    private static string GetUsageKey(LinkedUsageInfo usage)
    {
        return string.Join(
            "\u001f",
            usage.Kind,
            usage.Identity,
            usage.Source,
            usage.Line.ToString(System.Globalization.CultureInfo.InvariantCulture),
            usage.Column.ToString(System.Globalization.CultureInfo.InvariantCulture));
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

    internal void AddAnalysisBudgetFallback(string source)
    {
        DisableFineAnalysis(source);
    }

    private void DisableFineAnalysis(string source)
    {
        _fineAnalysisDisabled = true;
        var fallback = new LinkedFallbackManifestRecord(
            string.Empty,
            "AnalysisBudgetExceeded",
            source,
            0,
            0);
        if (_fallbackSet.Add(fallback))
        {
            Fallbacks.Add(fallback);
        }
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
               key == LinkedRegistrationProtocol.UnitEdgeManifestKey ||
               key == LinkedRegistrationProtocol.RootUnitManifestKey ||
               key == LinkedRegistrationProtocol.UsageManifestKey ||
               key == LinkedRegistrationProtocol.FallbackManifestKey;
    }
}
