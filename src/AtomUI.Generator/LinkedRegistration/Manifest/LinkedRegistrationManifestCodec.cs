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
                unit.FragmentMethod),
            LinkedControlMapManifestRecord controlMap => Create(
                LinkedRegistrationProtocol.ControlMapManifestKey,
                controlMap.PackageId,
                controlMap.MetadataName,
                controlMap.UnitId),
            LinkedUsageManifestRecord usage => Create(
                LinkedRegistrationProtocol.UsageManifestKey,
                usage.Kind.ToString(),
                usage.Identity,
                usage.Source,
                usage.Line.ToString(CultureInfo.InvariantCulture),
                usage.Column.ToString(CultureInfo.InvariantCulture)),
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
                LinkedRegistrationProtocol.UsageManifestKey => DecodeUsage(key, decoded),
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
        RequireFieldCount(key, fields, 7);
        RequireNonEmptyFields(key, fields, 0, 1, 3, 4);
        var packageSharedType = EmptyToNull(fields[5]);
        var packageSharedMethod = EmptyToNull(fields[6]);
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
            packageSharedType,
            packageSharedMethod);
    }

    private static LinkedUnitManifestRecord DecodeUnit(string key, string[] fields)
    {
        RequireFieldCount(key, fields, 4);
        RequireNonEmptyFields(key, fields, 0, 1, 2, 3);
        return new LinkedUnitManifestRecord(fields[0], fields[1], fields[2], fields[3]);
    }

    private static LinkedControlMapManifestRecord DecodeControlMap(string key, string[] fields)
    {
        RequireFieldCount(key, fields, 3);
        RequireNonEmptyFields(key, fields, 0, 1, 2);
        return new LinkedControlMapManifestRecord(fields[0], fields[1], fields[2]);
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
