using System.Globalization;
using System.Text;

namespace AtomUI.Generator.LinkedRegistration.Manifest;

internal static class LinkedRegistrationManifestCodec
{
    internal static LinkedRegistrationManifestEnvelope Encode(
        LinkedRegistrationManifestRecord record)
    {
        if (record is null)
        {
            throw new ArgumentNullException(nameof(record));
        }

        return record switch
        {
            LinkedPackageManifestRecord package => Create(
                LinkedRegistrationProtocol.PackageManifestKey,
                package.PackageId,
                package.AssemblyName,
                package.Granularity,
                package.EntryMethodMetadataNames,
                package.FullFragmentType,
                package.FullFragmentMethod,
                package.PackageSharedFragmentType ?? string.Empty,
                package.PackageSharedFragmentMethod ?? string.Empty),
            LinkedUnitManifestRecord unit => Create(
                LinkedRegistrationProtocol.UnitManifestKey,
                unit.PackageId,
                unit.UnitId,
                unit.FragmentType,
                unit.FragmentMethod,
                unit.OrderKey.ToString(CultureInfo.InvariantCulture)),
            LinkedControlMapManifestRecord controlMap => Create(
                LinkedRegistrationProtocol.ControlMapManifestKey,
                controlMap.PackageId,
                controlMap.MetadataName,
                controlMap.UnitId),
            LinkedUnitEdgeManifestRecord edge => Create(
                LinkedRegistrationProtocol.UnitEdgeManifestKey,
                edge.PackageId,
                edge.SourceUnitId,
                edge.TargetUnitId,
                edge.EvidenceKind.ToString()),
            LinkedRootUnitManifestRecord rootUnit => Create(
                LinkedRegistrationProtocol.RootUnitManifestKey,
                rootUnit.PackageId,
                rootUnit.UnitId),
            LinkedUsageManifestRecord usage => Create(
                LinkedRegistrationProtocol.UsageManifestKey,
                usage.Kind.ToString(),
                usage.Identity,
                usage.Source,
                usage.Line.ToString(CultureInfo.InvariantCulture),
                usage.Column.ToString(CultureInfo.InvariantCulture)),
            LinkedFallbackManifestRecord fallback => Create(
                LinkedRegistrationProtocol.FallbackManifestKey,
                fallback.PackageId,
                fallback.Reason,
                fallback.Source,
                fallback.Line.ToString(CultureInfo.InvariantCulture),
                fallback.Column.ToString(CultureInfo.InvariantCulture)),
            _ => throw new ArgumentOutOfRangeException(nameof(record), record.GetType().FullName)
        };
    }

    internal static bool TryDecode(
        string key,
        string value,
        out LinkedRegistrationManifestRecord? record,
        out string error)
    {
        record = null;
        error = string.Empty;
        if (!TryGetMajorVersion(key, out var majorVersion))
        {
            error = $"Manifest key '{key}' is not an AtomUI linked-registration record.";
            return false;
        }
        if (majorVersion != LinkedRegistrationProtocol.ProtocolMajorVersion)
        {
            error = $"AtomUI linked-registration manifest major version '{majorVersion}' is not supported.";
            return false;
        }

        var fields = value.Split('|');
        if (fields.Length == 0 ||
            !int.TryParse(fields[0], NumberStyles.None, CultureInfo.InvariantCulture, out var valueVersion) ||
            valueVersion != LinkedRegistrationProtocol.ProtocolMajorVersion)
        {
            error = $"Manifest '{key}' does not contain protocol major version '{LinkedRegistrationProtocol.ProtocolMajorVersion}'.";
            return false;
        }

        var decoded = new string[fields.Length - 1];
        for (var index = 1; index < fields.Length; index++)
        {
            try
            {
                decoded[index - 1] = Uri.UnescapeDataString(fields[index]);
            }
            catch (Exception exception)
            {
                error = $"Manifest '{key}' contains an invalid escaped field: {exception.Message}";
                return false;
            }
        }

        try
        {
            record = key switch
            {
                LinkedRegistrationProtocol.PackageManifestKey => DecodePackage(key, decoded),
                LinkedRegistrationProtocol.UnitManifestKey => DecodeUnit(key, decoded),
                LinkedRegistrationProtocol.ControlMapManifestKey => DecodeControlMap(key, decoded),
                LinkedRegistrationProtocol.UnitEdgeManifestKey => DecodeUnitEdge(key, decoded),
                LinkedRegistrationProtocol.RootUnitManifestKey => DecodeRootUnit(key, decoded),
                LinkedRegistrationProtocol.UsageManifestKey => DecodeUsage(key, decoded),
                LinkedRegistrationProtocol.FallbackManifestKey => DecodeFallback(key, decoded),
                _ => throw new FormatException(
                    $"Manifest key '{key}' is not supported by protocol version {LinkedRegistrationProtocol.ProtocolMajorVersion}.")
            };
            return true;
        }
        catch (FormatException exception)
        {
            error = exception.Message;
            record = null;
            return false;
        }
    }

    private static LinkedRegistrationManifestEnvelope Create(
        string key,
        params string[] fields)
    {
        var value = new StringBuilder();
        value.Append(LinkedRegistrationProtocol.ProtocolMajorVersion);
        foreach (var field in fields)
        {
            value.Append('|').Append(Uri.EscapeDataString(field));
        }
        return new LinkedRegistrationManifestEnvelope(key, value.ToString());
    }

    private static LinkedPackageManifestRecord DecodePackage(string key, string[] fields)
    {
        if (fields.Length == 7)
        {
            RequireNonEmptyFields(key, fields, 0, 1, 3, 4);
            var legacySharedType = EmptyToNull(fields[5]);
            var legacySharedMethod = EmptyToNull(fields[6]);
            if ((legacySharedType is null) != (legacySharedMethod is null))
            {
                throw new FormatException(
                    $"Manifest '{key}' must provide both PackageShared fragment type and method, or neither.");
            }
            return new LinkedPackageManifestRecord(
                fields[0],
                fields[1],
                string.Empty,
                fields[2],
                fields[3],
                fields[4],
                legacySharedType,
                legacySharedMethod);
        }
        RequireFieldCount(key, fields, 8);
        RequireNonEmptyFields(key, fields, 0, 1, 2, 4, 5);
        if (!string.Equals(fields[2], "Package", StringComparison.Ordinal) &&
            !string.Equals(fields[2], "Directory", StringComparison.Ordinal))
        {
            throw new FormatException(
                $"Manifest '{key}' contains invalid registration granularity '{fields[2]}'.");
        }
        var packageSharedType = EmptyToNull(fields[6]);
        var packageSharedMethod = EmptyToNull(fields[7]);
        if ((packageSharedType is null) != (packageSharedMethod is null))
        {
            throw new FormatException(
                $"Manifest '{key}' must provide both PackageShared fragment type and method, or neither.");
        }

        return new LinkedPackageManifestRecord(
            fields[0],
            fields[1],
            fields[2],
            fields[3],
            fields[4],
            fields[5],
            packageSharedType,
            packageSharedMethod);
    }

    private static LinkedUnitManifestRecord DecodeUnit(string key, string[] fields)
    {
        if (fields.Length == 4)
        {
            RequireNonEmptyFields(key, fields, 0, 1, 2, 3);
            return new LinkedUnitManifestRecord(fields[0], fields[1], fields[2], fields[3], 0);
        }
        RequireFieldCount(key, fields, 5);
        RequireNonEmptyFields(key, fields, 0, 1, 2, 3);
        if (!int.TryParse(fields[4], NumberStyles.None, CultureInfo.InvariantCulture, out var orderKey) ||
            orderKey < 0)
        {
            throw new FormatException($"Manifest '{key}' contains an invalid Unit order key.");
        }
        return new LinkedUnitManifestRecord(fields[0], fields[1], fields[2], fields[3], orderKey);
    }

    private static LinkedControlMapManifestRecord DecodeControlMap(string key, string[] fields)
    {
        RequireFieldCount(key, fields, 3);
        RequireNonEmptyFields(key, fields, 0, 1, 2);
        return new LinkedControlMapManifestRecord(fields[0], fields[1], fields[2]);
    }

    private static LinkedUnitEdgeManifestRecord DecodeUnitEdge(string key, string[] fields)
    {
        RequireFieldCount(key, fields, 4);
        RequireNonEmptyFields(key, fields, 0, 1, 2, 3);
        if (!Enum.TryParse(fields[3], ignoreCase: false, out LinkedUnitEdgeEvidenceKind evidenceKind))
        {
            throw new FormatException(
                $"Manifest '{key}' contains unknown UnitEdge evidence kind '{fields[3]}'.");
        }
        return new LinkedUnitEdgeManifestRecord(fields[0], fields[1], fields[2], evidenceKind);
    }

    private static LinkedRootUnitManifestRecord DecodeRootUnit(string key, string[] fields)
    {
        RequireFieldCount(key, fields, 2);
        RequireNonEmptyFields(key, fields, 0, 1);
        return new LinkedRootUnitManifestRecord(fields[0], fields[1]);
    }

    private static LinkedUsageManifestRecord DecodeUsage(string key, string[] fields)
    {
        RequireFieldCount(key, fields, 5);
        RequireNonEmptyFields(key, fields, 1, 2);
        if (!Enum.TryParse(fields[0], ignoreCase: false, out LinkedUsageKind kind))
        {
            throw new FormatException($"Manifest '{key}' contains unknown usage kind '{fields[0]}'.");
        }
        if (!int.TryParse(fields[3], NumberStyles.None, CultureInfo.InvariantCulture, out var line) || line < 0 ||
            !int.TryParse(fields[4], NumberStyles.None, CultureInfo.InvariantCulture, out var column) || column < 0)
        {
            throw new FormatException($"Manifest '{key}' contains an invalid source location.");
        }
        return new LinkedUsageManifestRecord(kind, fields[1], fields[2], line, column);
    }

    private static LinkedFallbackManifestRecord DecodeFallback(string key, string[] fields)
    {
        RequireFieldCount(key, fields, 5);
        RequireNonEmptyFields(key, fields, 1, 2);
        if (!int.TryParse(fields[3], NumberStyles.None, CultureInfo.InvariantCulture, out var line) || line < 0 ||
            !int.TryParse(fields[4], NumberStyles.None, CultureInfo.InvariantCulture, out var column) || column < 0)
        {
            throw new FormatException($"Manifest '{key}' contains an invalid source location.");
        }
        return new LinkedFallbackManifestRecord(fields[0], fields[1], fields[2], line, column);
    }

    private static void RequireFieldCount(string key, string[] fields, int expected)
    {
        if (fields.Length != expected)
        {
            throw new FormatException(
                $"Manifest '{key}' field count is {fields.Length}; expected {expected}.");
        }
    }

    private static void RequireNonEmptyFields(
        string key,
        string[] fields,
        params int[] indexes)
    {
        foreach (var index in indexes)
        {
            if (string.IsNullOrWhiteSpace(fields[index]))
            {
                throw new FormatException(
                    $"Manifest '{key}' contains an empty required field at index {index}.");
            }
        }
    }

    private static bool TryGetMajorVersion(string key, out int majorVersion)
    {
        majorVersion = 0;
        if (!key.StartsWith(LinkedRegistrationProtocol.MetadataPrefix, StringComparison.Ordinal))
        {
            return false;
        }

        var marker = key.LastIndexOf(".v", StringComparison.Ordinal);
        return marker >= 0 &&
               int.TryParse(
                   key.Substring(marker + 2),
                   NumberStyles.None,
                   CultureInfo.InvariantCulture,
                   out majorVersion);
    }

    private static string? EmptyToNull(string value)
    {
        return value.Length == 0 ? null : value;
    }
}
