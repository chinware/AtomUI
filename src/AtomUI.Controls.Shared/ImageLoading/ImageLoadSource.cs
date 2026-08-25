using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Avalonia.Media;
using Avalonia.Platform.Storage;

namespace AtomUI.Controls;

[TypeConverter(typeof(ImageLoadSourceConverter))]
public sealed class ImageLoadSource
{
    private static readonly ConditionalWeakTable<object, IdentityHolder> s_objectIdentities = new();
    private static long s_nextObjectIdentity;

    private ImageLoadSource(
        ImageLoadSourceKind kind,
        object value,
        string identity,
        string? displayName,
        string? cacheKey = null,
        string? version = null)
    {
        Kind = kind;
        Value = value;
        Identity = identity;
        DisplayName = displayName;
        CacheKey = cacheKey;
        Version = version;
    }

    public ImageLoadSourceKind Kind { get; }

    public string? DisplayName { get; }

    internal object Value { get; }

    internal string Identity { get; }

    internal string? CacheKey { get; }

    internal string? Version { get; }

    public static ImageLoadSource Parse(string source)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);
        if (!TryParse(source, out var result))
        {
            throw new FormatException($"'{source}' is not a supported image source.");
        }
        return result!;
    }

    public static bool TryParse(string? source, out ImageLoadSource? result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(source))
        {
            return false;
        }

        if (Path.IsPathFullyQualified(source))
        {
            result = FromFile(source);
            return true;
        }

        if (!Uri.TryCreate(source, UriKind.Absolute, out var uri))
        {
            return false;
        }

        try
        {
            result = FromUri(uri);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    public static ImageLoadSource FromUri(string uri)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        if (!Uri.TryCreate(uri, UriKind.Absolute, out var parsed))
        {
            throw new FormatException($"'{uri}' is not an absolute URI.");
        }
        return FromUri(parsed);
    }

    public static ImageLoadSource FromUri(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);
        if (!uri.IsAbsoluteUri)
        {
            throw new ArgumentException("Image URI must be absolute.", nameof(uri));
        }

        return uri.Scheme.ToLowerInvariant() switch
        {
            "http" or "https" => FromHttpUri(uri),
            "avares" => FromAsset(uri),
            "file" => FromFile(uri.LocalPath),
            _ => throw new ArgumentException($"URI scheme '{uri.Scheme}' is not supported.", nameof(uri))
        };
    }

    public static ImageLoadSource FromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var fullPath = Path.GetFullPath(path);
        return new ImageLoadSource(
            ImageLoadSourceKind.File,
            fullPath,
            $"file:{NormalizeFileIdentity(fullPath)}",
            Path.GetFileName(fullPath));
    }

    public static ImageLoadSource FromAsset(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);
        if (!uri.IsAbsoluteUri || !uri.Scheme.Equals("avares", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Asset source must be an absolute avares URI.", nameof(uri));
        }
        if (string.IsNullOrWhiteSpace(uri.Host) ||
            !string.IsNullOrEmpty(uri.UserInfo) ||
            !string.IsNullOrEmpty(uri.Query) ||
            !string.IsNullOrEmpty(uri.Fragment))
        {
            throw new ArgumentException(
                "Asset source must contain an assembly host and resource path without credentials, query, or fragment.",
                nameof(uri));
        }

        var normalized = new Uri(uri.GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped));
        return new ImageLoadSource(
            ImageLoadSourceKind.Asset,
            normalized,
            $"asset:{normalized.AbsoluteUri}",
            Path.GetFileName(normalized.AbsolutePath));
    }

    public static ImageLoadSource FromStorageFile(
        IStorageFile storageFile,
        string? cacheKey = null,
        string? version = null)
    {
        ArgumentNullException.ThrowIfNull(storageFile);
        ValidateOptionalKey(cacheKey, nameof(cacheKey));
        ValidateOptionalKey(version, nameof(version));
        var identity = cacheKey is not null && version is not null
            ? $"storage:key:{cacheKey}:{version}"
            : $"storage:object:{GetObjectIdentity(storageFile)}";
        return new ImageLoadSource(
            ImageLoadSourceKind.StorageFile,
            storageFile,
            identity,
            storageFile.Name,
            cacheKey,
            version);
    }

    public static ImageLoadSource FromBytes(
        ReadOnlyMemory<byte> bytes,
        string? cacheKey = null,
        string? version = null)
    {
        ValidateOptionalKey(cacheKey, nameof(cacheKey));
        ValidateOptionalKey(version, nameof(version));
        var copy = bytes.ToArray();
        var identity = cacheKey is not null && version is not null
            ? $"bytes:key:{cacheKey}:{version}"
            : $"bytes:object:{GetObjectIdentity(copy)}";
        return new ImageLoadSource(
            ImageLoadSourceKind.Bytes,
            copy,
            identity,
            cacheKey,
            cacheKey,
            version);
    }

    public static ImageLoadSource FromStream(
        Func<CancellationToken, ValueTask<Stream>> openStream,
        string? cacheKey = null,
        string? version = null,
        string? displayName = null)
    {
        ArgumentNullException.ThrowIfNull(openStream);
        ValidateOptionalKey(cacheKey, nameof(cacheKey));
        ValidateOptionalKey(version, nameof(version));
        var identity = cacheKey is not null && version is not null
            ? $"stream:key:{cacheKey}:{version}"
            : $"stream:object:{GetObjectIdentity(openStream)}";
        return new ImageLoadSource(
            ImageLoadSourceKind.Stream,
            openStream,
            identity,
            displayName ?? cacheKey,
            cacheKey,
            version);
    }

    public static ImageLoadSource FromImage(IImage image, string? cacheKey = null)
    {
        ArgumentNullException.ThrowIfNull(image);
        ValidateOptionalKey(cacheKey, nameof(cacheKey));
        return new ImageLoadSource(
            ImageLoadSourceKind.Image,
            image,
            $"image:object:{GetObjectIdentity(image)}:{cacheKey}",
            cacheKey,
            cacheKey);
    }

    public override string ToString() => DisplayName ?? Identity;

    private static ImageLoadSource FromHttpUri(Uri uri)
    {
        if (!string.IsNullOrEmpty(uri.UserInfo) || string.IsNullOrWhiteSpace(uri.Host))
        {
            throw new ArgumentException("HTTP image URI must have a host and cannot contain credentials.", nameof(uri));
        }

        var builder = new UriBuilder(uri)
        {
            Scheme = uri.Scheme.ToLowerInvariant(),
            Host = new IdnMapping().GetAscii(uri.Host).ToLowerInvariant(),
            Fragment = string.Empty
        };
        if ((builder.Scheme == "http" && builder.Port == 80) ||
            (builder.Scheme == "https" && builder.Port == 443))
        {
            builder.Port = -1;
        }

        var normalized = builder.Uri;
        return new ImageLoadSource(
            ImageLoadSourceKind.Http,
            normalized,
            $"http:{normalized.AbsoluteUri}",
            ResolveUriDisplayName(normalized));
    }

    private static string? ResolveUriDisplayName(Uri uri)
    {
        var segments = uri.AbsolutePath
                          .Split('/', StringSplitOptions.RemoveEmptyEntries)
                          .Select(Uri.UnescapeDataString)
                          .ToArray();
        if (segments.Length == 0)
        {
            return null;
        }

        return segments.Length >= 3 &&
               IsPositiveIntegerSegment(segments[^1]) &&
               IsPositiveIntegerSegment(segments[^2])
            ? segments[^3]
            : segments[^1];
    }

    private static bool IsPositiveIntegerSegment(string segment)
    {
        return segment.Length > 0 && segment.All(char.IsDigit);
    }

    private static string NormalizeFileIdentity(string path)
    {
        return OperatingSystem.IsWindows() ? path.ToUpperInvariant() : path;
    }

    private static void ValidateOptionalKey(string? value, string parameterName)
    {
        if (value is not null && string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be empty or whitespace.", parameterName);
        }
    }

    private static long GetObjectIdentity(object value)
    {
        return s_objectIdentities.GetValue(
            value,
            static _ => new IdentityHolder(Interlocked.Increment(ref s_nextObjectIdentity))).Value;
    }

    private sealed record IdentityHolder(long Value);
}
