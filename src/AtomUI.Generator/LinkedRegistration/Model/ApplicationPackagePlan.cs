using System.Collections.Immutable;
using AtomUI.Generator.LinkedRegistration.Manifest;

namespace AtomUI.Generator.LinkedRegistration.Model;

internal sealed record ApplicationPackagePlan
{
    internal ApplicationPackagePlan(
        string packageId,
        bool useFullFallback,
        ImmutableArray<LinkedUnitManifestRecord> units,
        LinkedPackageManifestRecord package)
    {
        PackageId = packageId;
        UseFullFallback = useFullFallback;
        Units = units;
        Package = package;
    }

    internal string PackageId { get; }
    internal bool UseFullFallback { get; }
    internal ImmutableArray<LinkedUnitManifestRecord> Units { get; }
    internal LinkedPackageManifestRecord Package { get; }
}
