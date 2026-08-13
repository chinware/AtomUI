namespace AtomUI.Generator.LinkedRegistration.Manifest;

internal abstract record LinkedRegistrationManifestRecord;

internal sealed record LinkedPackageManifestRecord : LinkedRegistrationManifestRecord
{
    internal LinkedPackageManifestRecord(
        string packageId,
        string assemblyName,
        string entryMethodMetadataNames,
        string fullFragmentType,
        string fullFragmentMethod,
        string? packageSharedFragmentType,
        string? packageSharedFragmentMethod)
    {
        PackageId = packageId;
        AssemblyName = assemblyName;
        EntryMethodMetadataNames = entryMethodMetadataNames;
        FullFragmentType = fullFragmentType;
        FullFragmentMethod = fullFragmentMethod;
        PackageSharedFragmentType = packageSharedFragmentType;
        PackageSharedFragmentMethod = packageSharedFragmentMethod;
    }

    internal string PackageId { get; }
    internal string AssemblyName { get; }
    internal string EntryMethodMetadataNames { get; }
    internal string FullFragmentType { get; }
    internal string FullFragmentMethod { get; }
    internal string? PackageSharedFragmentType { get; }
    internal string? PackageSharedFragmentMethod { get; }
}

internal sealed record LinkedUnitManifestRecord : LinkedRegistrationManifestRecord
{
    internal LinkedUnitManifestRecord(
        string packageId,
        string unitId,
        string fragmentType,
        string fragmentMethod)
    {
        PackageId = packageId;
        UnitId = unitId;
        FragmentType = fragmentType;
        FragmentMethod = fragmentMethod;
    }

    internal string PackageId { get; }
    internal string UnitId { get; }
    internal string FragmentType { get; }
    internal string FragmentMethod { get; }
}

internal sealed record LinkedControlMapManifestRecord : LinkedRegistrationManifestRecord
{
    internal LinkedControlMapManifestRecord(
        string packageId,
        string metadataName,
        string unitId)
    {
        PackageId = packageId;
        MetadataName = metadataName;
        UnitId = unitId;
    }

    internal string PackageId { get; }
    internal string MetadataName { get; }
    internal string UnitId { get; }
}

internal enum LinkedUsageKind
{
    Control,
    UnitRoot,
    PackageRoot,
    Entry
}

internal sealed record LinkedUsageManifestRecord : LinkedRegistrationManifestRecord
{
    internal LinkedUsageManifestRecord(
        LinkedUsageKind kind,
        string identity,
        string source,
        int line,
        int column)
    {
        Kind = kind;
        Identity = identity;
        Source = source;
        Line = line;
        Column = column;
    }

    internal LinkedUsageKind Kind { get; }
    internal string Identity { get; }
    internal string Source { get; }
    internal int Line { get; }
    internal int Column { get; }
}

internal sealed record LinkedRegistrationManifestEnvelope
{
    internal LinkedRegistrationManifestEnvelope(string key, string value)
    {
        Key = key;
        Value = value;
    }

    internal string Key { get; }
    internal string Value { get; }
}
