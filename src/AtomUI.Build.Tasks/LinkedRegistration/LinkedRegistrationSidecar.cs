using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;

namespace AtomUI.LinkedRegistration.Protocol;

[DataContract]
internal sealed class LinkedRegistrationSidecar
{
    internal const int SupportedProtocolMajor = 1;
    internal const int SupportedProtocolMinor = 0;

    [DataMember(Name = "protocolMajor", Order = 0, IsRequired = true)]
    public int ProtocolMajor { get; set; } = SupportedProtocolMajor;

    [DataMember(Name = "protocolMinor", Order = 1, IsRequired = true)]
    public int ProtocolMinor { get; set; } = SupportedProtocolMinor;

    [DataMember(Name = "producer", Order = 2, IsRequired = true)]
    public string Producer { get; set; } = string.Empty;

    [DataMember(Name = "assembly", Order = 3, IsRequired = true)]
    public LinkedSidecarAssembly Assembly { get; set; } = new();

    [DataMember(Name = "packages", Order = 4, IsRequired = true)]
    public LinkedSidecarPackage[] Packages { get; set; } = [];

    [DataMember(Name = "usages", Order = 5, IsRequired = true)]
    public LinkedSidecarUsage[] Usages { get; set; } = [];

    [DataMember(Name = "fallbacks", Order = 6, IsRequired = true)]
    public LinkedSidecarFallback[] Fallbacks { get; set; } = [];
}

[DataContract]
internal sealed class LinkedSidecarAssembly
{
    [DataMember(Name = "name", Order = 0, IsRequired = true)]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "contractHash", Order = 1, IsRequired = true)]
    public string ContractHash { get; set; } = string.Empty;

    [DataMember(Name = "targetFramework", Order = 2, IsRequired = true)]
    public string TargetFramework { get; set; } = string.Empty;
}

[DataContract]
internal sealed class LinkedSidecarPackage
{
    [DataMember(Name = "id", Order = 0, IsRequired = true)]
    public string Id { get; set; } = string.Empty;

    [DataMember(Name = "assemblyName", Order = 1, IsRequired = true)]
    public string AssemblyName { get; set; } = string.Empty;

    [DataMember(Name = "granularity", Order = 2, IsRequired = true)]
    public string Granularity { get; set; } = string.Empty;

    [DataMember(Name = "entryMethods", Order = 3, IsRequired = true)]
    public string[] EntryMethods { get; set; } = [];

    [DataMember(Name = "fullFragment", Order = 4, IsRequired = true)]
    public LinkedSidecarFragment FullFragment { get; set; } = new();

    [DataMember(Name = "sharedFragment", Order = 5)]
    public LinkedSidecarFragment? SharedFragment { get; set; }

    [DataMember(Name = "units", Order = 6, IsRequired = true)]
    public LinkedSidecarUnit[] Units { get; set; } = [];

    [DataMember(Name = "unitEdges", Order = 7, IsRequired = true)]
    public LinkedSidecarUnitEdge[] UnitEdges { get; set; } = [];

    [DataMember(Name = "rootUnits", Order = 8, IsRequired = true)]
    public string[] RootUnits { get; set; } = [];
}

[DataContract]
internal sealed class LinkedSidecarFragment
{
    [DataMember(Name = "type", Order = 0, IsRequired = true)]
    public string Type { get; set; } = string.Empty;

    [DataMember(Name = "method", Order = 1, IsRequired = true)]
    public string Method { get; set; } = string.Empty;
}

[DataContract]
internal sealed class LinkedSidecarUnit
{
    [DataMember(Name = "id", Order = 0, IsRequired = true)]
    public string Id { get; set; } = string.Empty;

    [DataMember(Name = "fragmentType", Order = 1, IsRequired = true)]
    public string FragmentType { get; set; } = string.Empty;

    [DataMember(Name = "fragmentMethod", Order = 2, IsRequired = true)]
    public string FragmentMethod { get; set; } = string.Empty;

    [DataMember(Name = "orderKey", Order = 3, IsRequired = true)]
    public int OrderKey { get; set; }

    [DataMember(Name = "controls", Order = 4, IsRequired = true)]
    public string[] Controls { get; set; } = [];
}

[DataContract]
internal sealed class LinkedSidecarUnitEdge
{
    [DataMember(Name = "sourceUnitId", Order = 0, IsRequired = true)]
    public string SourceUnitId { get; set; } = string.Empty;

    [DataMember(Name = "targetUnitId", Order = 1, IsRequired = true)]
    public string TargetUnitId { get; set; } = string.Empty;

    [DataMember(Name = "evidenceKind", Order = 2, IsRequired = true)]
    public string EvidenceKind { get; set; } = string.Empty;
}

[DataContract]
internal sealed class LinkedSidecarUsage
{
    [DataMember(Name = "kind", Order = 0, IsRequired = true)]
    public string Kind { get; set; } = string.Empty;

    [DataMember(Name = "identity", Order = 1, IsRequired = true)]
    public string Identity { get; set; } = string.Empty;

    [DataMember(Name = "source", Order = 2, IsRequired = true)]
    public string Source { get; set; } = string.Empty;

    [DataMember(Name = "line", Order = 3, IsRequired = true)]
    public int Line { get; set; }

    [DataMember(Name = "column", Order = 4, IsRequired = true)]
    public int Column { get; set; }
}

[DataContract]
internal sealed class LinkedSidecarFallback
{
    [DataMember(Name = "packageId", Order = 0, IsRequired = true)]
    public string PackageId { get; set; } = string.Empty;

    [DataMember(Name = "reason", Order = 1, IsRequired = true)]
    public string Reason { get; set; } = string.Empty;

    [DataMember(Name = "source", Order = 2, IsRequired = true)]
    public string Source { get; set; } = string.Empty;

    [DataMember(Name = "line", Order = 3, IsRequired = true)]
    public int Line { get; set; }

    [DataMember(Name = "column", Order = 4, IsRequired = true)]
    public int Column { get; set; }
}

internal static class LinkedRegistrationSidecarCodec
{
    private static readonly DataContractJsonSerializer s_serializer = new(
        typeof(LinkedRegistrationSidecar),
        new DataContractJsonSerializerSettings
        {
            EmitTypeInformation = EmitTypeInformation.Never,
            SerializeReadOnlyTypes = false,
            UseSimpleDictionaryFormat = true
        });

    internal static byte[] Write(LinkedRegistrationSidecar sidecar)
    {
        Canonicalize(sidecar);
        sidecar.Assembly.ContractHash = string.Empty;
        sidecar.Assembly.ContractHash = ComputeHash(SerializeCore(sidecar));
        return SerializeCore(sidecar);
    }

    internal static bool TryRead(
        byte[] bytes,
        out LinkedRegistrationSidecar? sidecar,
        out string error)
    {
        sidecar = null;
        error = string.Empty;
        try
        {
            using var stream = new MemoryStream(bytes, writable: false);
            sidecar = s_serializer.ReadObject(stream) as LinkedRegistrationSidecar;
            if (sidecar is null)
            {
                error = "Sidecar root is missing.";
                return false;
            }
            if (sidecar.ProtocolMajor != LinkedRegistrationSidecar.SupportedProtocolMajor)
            {
                error = $"Sidecar protocol major '{sidecar.ProtocolMajor}' is not supported.";
                sidecar = null;
                return false;
            }
            if (!ValidateRequiredFields(sidecar, out error))
            {
                sidecar = null;
                return false;
            }

            var expectedHash = sidecar.Assembly.ContractHash;
            Canonicalize(sidecar);
            sidecar.Assembly.ContractHash = string.Empty;
            var actualHash = ComputeHash(SerializeCore(sidecar));
            sidecar.Assembly.ContractHash = expectedHash;
            if (!string.Equals(expectedHash, actualHash, StringComparison.Ordinal))
            {
                error = $"Sidecar contract hash mismatch; expected '{expectedHash}', calculated '{actualHash}'.";
                sidecar = null;
                return false;
            }
            return true;
        }
        catch (Exception exception) when (
            exception is SerializationException or InvalidDataContractException or
            IOException or ArgumentException)
        {
            error = exception.Message;
            sidecar = null;
            return false;
        }
    }

    private static void Canonicalize(LinkedRegistrationSidecar sidecar)
    {
        sidecar.Producer ??= string.Empty;
        sidecar.Assembly ??= new LinkedSidecarAssembly();
        sidecar.Assembly.Name ??= string.Empty;
        sidecar.Assembly.TargetFramework ??= string.Empty;
        sidecar.Packages = (sidecar.Packages ?? []).OrderBy(
                static package => package.Id,
                StringComparer.Ordinal)
            .ThenBy(static package => package.AssemblyName, StringComparer.Ordinal)
            .ToArray();
        foreach (var package in sidecar.Packages)
        {
            package.EntryMethods = (package.EntryMethods ?? []).Where(static item => item.Length != 0)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(static item => item, StringComparer.Ordinal)
                .ToArray();
            package.Units = (package.Units ?? []).OrderBy(static unit => unit.OrderKey)
                .ThenBy(static unit => unit.Id, StringComparer.Ordinal)
                .ToArray();
            foreach (var unit in package.Units)
            {
                unit.Controls = (unit.Controls ?? []).Distinct(StringComparer.Ordinal)
                    .OrderBy(static control => control, StringComparer.Ordinal)
                    .ToArray();
            }
            package.UnitEdges = (package.UnitEdges ?? []).GroupBy(
                    static edge => string.Join("\u001f", edge.SourceUnitId, edge.TargetUnitId, edge.EvidenceKind),
                    StringComparer.Ordinal)
                .Select(static group => group.First())
                .OrderBy(static edge => edge.SourceUnitId, StringComparer.Ordinal)
                .ThenBy(static edge => edge.TargetUnitId, StringComparer.Ordinal)
                .ThenBy(static edge => edge.EvidenceKind, StringComparer.Ordinal)
                .ToArray();
            package.RootUnits = (package.RootUnits ?? []).Distinct(StringComparer.Ordinal)
                .OrderBy(static unitId => unitId, StringComparer.Ordinal)
                .ToArray();
        }
        sidecar.Usages = (sidecar.Usages ?? []).GroupBy(
                static usage => string.Join(
                    "\u001f",
                    usage.Kind,
                    usage.Identity,
                    usage.Source,
                    usage.Line.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    usage.Column.ToString(System.Globalization.CultureInfo.InvariantCulture)),
                StringComparer.Ordinal)
            .Select(static group => group.First())
            .OrderBy(static usage => usage.Source, StringComparer.Ordinal)
            .ThenBy(static usage => usage.Line)
            .ThenBy(static usage => usage.Column)
            .ThenBy(static usage => usage.Kind, StringComparer.Ordinal)
            .ThenBy(static usage => usage.Identity, StringComparer.Ordinal)
            .ToArray();
        sidecar.Fallbacks = (sidecar.Fallbacks ?? []).GroupBy(
                static fallback => string.Join(
                    "\u001f",
                    fallback.PackageId,
                    fallback.Reason,
                    fallback.Source,
                    fallback.Line.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    fallback.Column.ToString(System.Globalization.CultureInfo.InvariantCulture)),
                StringComparer.Ordinal)
            .Select(static group => group.First())
            .OrderBy(static fallback => fallback.PackageId, StringComparer.Ordinal)
            .ThenBy(static fallback => fallback.Source, StringComparer.Ordinal)
            .ThenBy(static fallback => fallback.Line)
            .ThenBy(static fallback => fallback.Column)
            .ThenBy(static fallback => fallback.Reason, StringComparer.Ordinal)
            .ToArray();
    }

    private static bool ValidateRequiredFields(
        LinkedRegistrationSidecar sidecar,
        out string error)
    {
        error = string.Empty;
        if (sidecar.Assembly is null || string.IsNullOrWhiteSpace(sidecar.Assembly.Name) ||
            string.IsNullOrWhiteSpace(sidecar.Assembly.ContractHash) ||
            sidecar.Packages is null || sidecar.Usages is null || sidecar.Fallbacks is null)
        {
            error = "Sidecar is missing required top-level fields.";
            return false;
        }
        foreach (var package in sidecar.Packages)
        {
            if (string.IsNullOrWhiteSpace(package.Id) ||
                string.IsNullOrWhiteSpace(package.AssemblyName) ||
                (package.Granularity != "Package" && package.Granularity != "Directory") ||
                package.FullFragment is null ||
                string.IsNullOrWhiteSpace(package.FullFragment.Type) ||
                string.IsNullOrWhiteSpace(package.FullFragment.Method) ||
                package.EntryMethods is null || package.Units is null ||
                package.UnitEdges is null || package.RootUnits is null)
            {
                error = $"Package '{package.Id}' contains invalid required fields.";
                return false;
            }
        }
        return true;
    }

    private static byte[] SerializeCore(LinkedRegistrationSidecar sidecar)
    {
        using var stream = new MemoryStream();
        s_serializer.WriteObject(stream, sidecar);
        return stream.ToArray();
    }

    private static string ComputeHash(byte[] bytes)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(bytes);
        var builder = new StringBuilder("sha256:", 7 + hash.Length * 2);
        foreach (var value in hash)
        {
            builder.Append(value.ToString("x2", System.Globalization.CultureInfo.InvariantCulture));
        }
        return builder.ToString();
    }
}
