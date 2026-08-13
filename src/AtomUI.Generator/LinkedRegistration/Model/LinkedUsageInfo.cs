using AtomUI.Generator.LinkedRegistration.Manifest;
using Microsoft.CodeAnalysis;

namespace AtomUI.Generator.LinkedRegistration.Model;

internal sealed class LinkedUsageInfo
{
    internal LinkedUsageInfo(
        LinkedUsageKind kind,
        string identity,
        string source,
        int line,
        int column,
        string packageId,
        Location? location)
    {
        Kind = kind;
        Identity = identity;
        Source = source;
        Line = line;
        Column = column;
        PackageId = packageId;
        Location = location;
    }

    internal LinkedUsageKind Kind { get; }
    internal string Identity { get; }
    internal string Source { get; }
    internal int Line { get; }
    internal int Column { get; }
    internal string PackageId { get; }
    internal Location? Location { get; }

    internal LinkedUsageManifestRecord ToManifestRecord()
    {
        return new LinkedUsageManifestRecord(Kind, Identity, Source, Line, Column);
    }
}
