using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Avalonia.Media;
using Avalonia.Platform.Storage;

namespace AtomUI.Controls;

[TypeConverter(typeof(ImageSourceConverter))]
public abstract class ImageSource
{
    private static readonly ConditionalWeakTable<object, IdentityHolder> s_objectIdentities = new();
    private static long s_nextObjectIdentity;

    internal ImageSource(string? displayName)
    {
        DisplayName = displayName;
    }

    public string? DisplayName { get; }

    internal abstract ImageSourceKind Kind { get; }

    internal abstract string CacheIdentity { get; }

    internal virtual ImageSourceVersion? SourceRevision => null;

    internal virtual bool CanPersistSourceSnapshot => false;

    public static ImageSource Parse(string source)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);
        if (!TryParse(source, out var result))
        {
            throw new FormatException($"'{source}' is not a supported image source.");
        }
        return result!;
    }

    public static bool TryParse(string? source, out ImageSource? result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(source))
        {
            return false;
        }
        if (Path.IsPathFullyQualified(source))
        {
            result = new FileImageSource(source);
            return true;
        }
        if (!Uri.TryCreate(source, UriKind.Absolute, out var uri))
        {
            return false;
        }
        try
        {
            result = uri.Scheme.ToLowerInvariant() switch
            {
                "http" or "https" => new HttpImageSource(uri),
                "avares" => new AssetImageSource(uri),
                "file" => new FileImageSource(uri.LocalPath),
                _ => null
            };
            return result is not null;
        }
        catch (ArgumentException)
        {
            result = null;
            return false;
        }
    }

    public override string ToString() => DisplayName ?? CacheIdentity;

    internal static long GetObjectIdentity(object value)
    {
        return s_objectIdentities.GetValue(
            value,
            static _ => new IdentityHolder(Interlocked.Increment(ref s_nextObjectIdentity))).Value;
    }

    internal static void ValidateOptionalValue(string? value, string parameterName)
    {
        if (value is not null && string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be empty or whitespace.", parameterName);
        }
    }

    internal static string? ResolveUriDisplayName(Uri uri)
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
               segments[^1].Length > 0 && segments[^1].All(char.IsDigit) &&
               segments[^2].Length > 0 && segments[^2].All(char.IsDigit)
            ? segments[^3]
            : segments[^1];
    }

    private sealed record IdentityHolder(long Value);
}

public sealed class HttpImageSource : ImageSource
{
    public HttpImageSource(Uri uri)
        : this(Normalize(uri), true)
    {
    }

    private HttpImageSource(Uri uri, bool _)
        : base(ResolveUriDisplayName(uri))
    {
        Uri = uri;
    }

    public Uri Uri { get; }

    internal override ImageSourceKind Kind => ImageSourceKind.Http;

    internal override string CacheIdentity => $"http:{Uri.AbsoluteUri}";

    internal override bool CanPersistSourceSnapshot => true;

    private static Uri Normalize(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);
        if (!uri.IsAbsoluteUri ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) ||
            !string.IsNullOrEmpty(uri.UserInfo) ||
            string.IsNullOrWhiteSpace(uri.Host))
        {
            throw new ArgumentException(
                "HTTP image source must be an absolute HTTP or HTTPS URI with a host and no credentials.",
                nameof(uri));
        }
        var builder = new UriBuilder(uri)
        {
            Scheme = uri.Scheme.ToLowerInvariant(),
            Host = new IdnMapping().GetAscii(uri.Host).ToLowerInvariant(),
            Fragment = string.Empty
        };
        if ((builder.Scheme == Uri.UriSchemeHttp && builder.Port == 80) ||
            (builder.Scheme == Uri.UriSchemeHttps && builder.Port == 443))
        {
            builder.Port = -1;
        }
        return builder.Uri;
    }
}

public sealed class FileImageSource : ImageSource
{
    public FileImageSource(
        string path,
        ImageFileValidationMode validation = ImageFileValidationMode.Metadata)
        : base(GetDisplayName(path))
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        Path = System.IO.Path.GetFullPath(path);
        Validation = validation;
    }

    public string Path { get; }

    public ImageFileValidationMode Validation { get; }

    internal override ImageSourceKind Kind => ImageSourceKind.File;

    internal override string CacheIdentity => $"file:{NormalizePath(Path)}:{Validation}";

    internal override bool CanPersistSourceSnapshot => true;

    private static string GetDisplayName(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return System.IO.Path.GetFileName(System.IO.Path.GetFullPath(path));
    }

    private static string NormalizePath(string path) =>
        OperatingSystem.IsWindows() ? path.ToUpperInvariant() : path;
}

public sealed class AssetImageSource : ImageSource
{
    public AssetImageSource(Uri uri)
        : this(Normalize(uri), true)
    {
    }

    private AssetImageSource(Uri uri, bool _)
        : base(ResolveUriDisplayName(uri))
    {
        Uri = uri;
    }

    public Uri Uri { get; }

    internal override ImageSourceKind Kind => ImageSourceKind.Asset;

    internal override string CacheIdentity => $"asset:{Uri.AbsoluteUri}";

    internal override ImageSourceVersion? SourceRevision => new("application-resource");

    internal override bool CanPersistSourceSnapshot => true;

    private static Uri Normalize(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);
        if (!uri.IsAbsoluteUri ||
            !uri.Scheme.Equals("avares", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(uri.Host) ||
            !string.IsNullOrEmpty(uri.UserInfo) ||
            !string.IsNullOrEmpty(uri.Query) ||
            !string.IsNullOrEmpty(uri.Fragment))
        {
            throw new ArgumentException(
                "Asset source must be an absolute avares URI with an assembly host and no credentials, query, or fragment.",
                nameof(uri));
        }
        return new Uri(uri.GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped));
    }
}

public sealed class StorageFileImageSource : ImageSource
{
    public StorageFileImageSource(IStorageFile file, string? revision = null)
        : base((file ?? throw new ArgumentNullException(nameof(file))).Name)
    {
        ValidateOptionalValue(revision, nameof(revision));
        File = file;
        Revision = revision;
    }

    public IStorageFile File { get; }

    public string? Revision { get; }

    internal override ImageSourceKind Kind => ImageSourceKind.StorageFile;

    internal override string CacheIdentity => Revision is not null
        ? $"storage:uri:{File.Path.AbsoluteUri}"
        : $"storage:object:{GetObjectIdentity(File)}";

    internal override ImageSourceVersion? SourceRevision => Revision is null ? null : new(Revision);

    internal override bool CanPersistSourceSnapshot => Revision is not null;
}

public sealed class BytesImageSource : ImageSource
{
    private readonly byte[] _bytes;
    private readonly string _digest;

    public BytesImageSource(ReadOnlyMemory<byte> bytes, string? displayName = null)
        : base(displayName)
    {
        _bytes = bytes.ToArray();
        _digest = Convert.ToHexString(SHA256.HashData(_bytes)).ToLowerInvariant();
    }

    public ReadOnlyMemory<byte> Bytes => _bytes.ToArray();

    internal ReadOnlyMemory<byte> Content => _bytes;

    internal override ImageSourceKind Kind => ImageSourceKind.Bytes;

    internal override string CacheIdentity => $"bytes:{_digest}";

    internal override ImageSourceVersion? SourceRevision => new(_digest);
}

public sealed class StreamImageSource : ImageSource
{
    public StreamImageSource(
        Func<CancellationToken, ValueTask<Stream>> openStream,
        string? identity = null,
        string? revision = null,
        string? displayName = null)
        : base(displayName ?? identity)
    {
        OpenStream = openStream ?? throw new ArgumentNullException(nameof(openStream));
        ValidateOptionalValue(identity, nameof(identity));
        ValidateOptionalValue(revision, nameof(revision));
        Identity = identity;
        Revision = revision;
    }

    public Func<CancellationToken, ValueTask<Stream>> OpenStream { get; }

    public string? Identity { get; }

    public string? Revision { get; }

    internal override ImageSourceKind Kind => ImageSourceKind.Stream;

    internal override string CacheIdentity => Identity is not null
        ? $"stream:key:{Identity}"
        : $"stream:object:{GetObjectIdentity(OpenStream)}";

    internal override ImageSourceVersion? SourceRevision => Revision is null ? null : new(Revision);

    internal override bool CanPersistSourceSnapshot => Identity is not null && Revision is not null;
}

public sealed class BorrowedImageSource : ImageSource
{
    public BorrowedImageSource(IImage image, string? displayName = null)
        : base(displayName)
    {
        Image = image ?? throw new ArgumentNullException(nameof(image));
    }

    public IImage Image { get; }

    internal override ImageSourceKind Kind => ImageSourceKind.Borrowed;

    internal override string CacheIdentity => $"borrowed:object:{GetObjectIdentity(Image)}";
}
