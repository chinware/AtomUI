using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace AtomUI.Generator.Localization.Catalog;

internal sealed class LanguageCatalogInfo
{
    internal LanguageCatalogInfo(
        string moduleId,
        string metadataName,
        string @namespace,
        string typeName,
        int contractVersion,
        ImmutableArray<LanguageCatalogUnitInfo> units,
        Location location)
    {
        ModuleId = moduleId;
        MetadataName = metadataName;
        Namespace = @namespace;
        TypeName = typeName;
        ContractVersion = contractVersion;
        Units = units;
        Location = location;
    }

    internal string ModuleId { get; }

    internal string MetadataName { get; }

    internal string Namespace { get; }

    internal string TypeName { get; }

    internal int ContractVersion { get; }

    internal ImmutableArray<LanguageCatalogUnitInfo> Units { get; }

    internal Location Location { get; }

    internal string CatalogId => $"{ModuleId}:{MetadataName}";
}

internal sealed class LanguageCatalogUnitInfo
{
    internal LanguageCatalogUnitInfo(string key, Location location)
    {
        Key = key;
        Location = location;
    }

    internal string Key { get; }

    internal Location Location { get; }
}

internal sealed class LanguageCatalogParseResult
{
    internal LanguageCatalogParseResult(
        LanguageCatalogInfo? catalog,
        ImmutableArray<Diagnostic> diagnostics)
    {
        Catalog = catalog;
        Diagnostics = diagnostics;
    }

    internal LanguageCatalogInfo? Catalog { get; }

    internal ImmutableArray<Diagnostic> Diagnostics { get; }
}
