using System.Collections.Frozen;
using System.Security.Cryptography;
using System.Text;

namespace AtomUI.Controls;

internal readonly record struct ImageEncodedCacheKey(string Value, string PartitionHash);

internal readonly record struct ImageDecodedCacheKey(
    ImageEncodedCacheKey EncodedKey,
    int DecodePixelWidth,
    int DecodePixelHeight,
    string CodecScope);

internal readonly record struct ImageEncodedOperationKey(
    ImageEncodedCacheKey CacheKey,
    ImageCacheMode CacheMode,
    string ShareScope);

internal readonly record struct ImageDecodedOperationKey(
    ImageDecodedCacheKey CacheKey,
    ImageCacheMode CacheMode,
    string ShareScope);

internal sealed record NormalizedImageRequest(
    ImageLoadSource Source,
    FrozenDictionary<string, string> Headers,
    ImageCacheMode CacheMode,
    string? CachePartition,
    string PartitionHash,
    string? Variant,
    TimeSpan Timeout,
    ImageRequestPriority Priority,
    int DecodePixelWidth,
    int DecodePixelHeight,
    IProgress<ImageLoadProgress>? Progress,
    ImageLoadTimingTracker Timing,
    ImageEncodedCacheKey EncodedKey,
    ImageDecodedCacheKey DecodedKey,
    ImageEncodedOperationKey EncodedOperationKey,
    ImageDecodedOperationKey DecodedOperationKey,
    bool HasAuthentication,
    bool CanShare,
    bool CanPersist);

internal static class ImageCacheKey
{
    private static readonly FrozenSet<string> s_forbiddenHeaders = new HashSet<string>(
        StringComparer.OrdinalIgnoreCase)
    {
        "Host",
        "Content-Length",
        "Connection",
        "Transfer-Encoding",
        "Upgrade",
        "Range",
        "Proxy-Connection"
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    internal static NormalizedImageRequest Normalize(
        ImageLoadRequest request,
        ImageLoadingOptions options,
        bool forceReload)
    {
        ArgumentNullException.ThrowIfNull(request);
        var requestOptions = request.Options ?? new ImageRequestOptions();
        var headers = SnapshotHeaders(requestOptions.Headers);
        var hasAuthentication = headers.Keys.Any(options.AuthenticationHeaderNames.Contains);
        var partition = NormalizeOptionalValue(requestOptions.CachePartition, nameof(requestOptions.CachePartition));
        var variant = NormalizeOptionalValue(requestOptions.Variant, nameof(requestOptions.Variant));
        var cacheMode = forceReload ? ImageCacheMode.Reload : requestOptions.CacheMode;
        if (hasAuthentication && partition is null)
        {
            cacheMode = ImageCacheMode.NoStore;
        }

        var timeout = requestOptions.Timeout ?? options.DefaultRequestTimeout;
        if (timeout <= TimeSpan.Zero || timeout == Timeout.InfiniteTimeSpan)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "Image request timeout must be finite and positive.");
        }

        var partitionHash = partition is null ? string.Empty : Hash(partition);
        var timing = new ImageLoadTimingTracker();
        var progress = timing.Wrap(request.Progress);
        var headerHash = HashHeaders(headers);
        var shareScope = hasAuthentication && partition is null ? Guid.NewGuid().ToString("N") : string.Empty;
        var encodedIdentity = string.Join(
            ':',
            request.Source.Identity,
            Hash(variant ?? string.Empty),
            partitionHash,
            headerHash,
            "reader-v1");
        var encodedKey = new ImageEncodedCacheKey(Hash(encodedIdentity), partitionHash);
        var decodedKey = new ImageDecodedCacheKey(
            encodedKey,
            request.DecodePixelWidth,
            request.DecodePixelHeight,
            "codec-registry-v1");
        var encodedOperationKey = new ImageEncodedOperationKey(encodedKey, cacheMode, shareScope);
        var decodedOperationKey = new ImageDecodedOperationKey(decodedKey, cacheMode, shareScope);
        var canPersist = request.Source.Kind switch
        {
            ImageLoadSourceKind.Http or ImageLoadSourceKind.Asset or ImageLoadSourceKind.File => true,
            ImageLoadSourceKind.StorageFile or ImageLoadSourceKind.Bytes or ImageLoadSourceKind.Stream =>
                request.Source.CacheKey is not null && request.Source.Version is not null,
            _ => false
        };
        if (hasAuthentication && !options.AllowAuthenticatedPersistentCache)
        {
            canPersist = false;
        }

        return new NormalizedImageRequest(
            request.Source,
            headers,
            cacheMode,
            partition,
            partitionHash,
            variant,
            timeout,
            request.Priority,
            request.DecodePixelWidth,
            request.DecodePixelHeight,
            progress,
            timing,
            encodedKey,
            decodedKey,
            encodedOperationKey,
            decodedOperationKey,
            hasAuthentication,
            !(hasAuthentication && partition is null),
            canPersist);
    }

    internal static string NormalizeOrigin(Uri uri)
    {
        var builder = new UriBuilder(uri)
        {
            Scheme = uri.Scheme.ToLowerInvariant(),
            Host = uri.IdnHost.ToLowerInvariant(),
            Path = string.Empty,
            Query = string.Empty,
            Fragment = string.Empty
        };
        if ((builder.Scheme == "http" && builder.Port == 80) ||
            (builder.Scheme == "https" && builder.Port == 443))
        {
            builder.Port = -1;
        }
        return builder.Uri.GetLeftPart(UriPartial.Authority);
    }

    internal static string Hash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    internal static string HashBytes(ReadOnlySpan<byte> value)
    {
        var bytes = SHA256.HashData(value);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    internal static string HashSelectedHeaders(
        IReadOnlyDictionary<string, string> headers,
        IEnumerable<string> names)
    {
        var builder = new StringBuilder();
        foreach (var name in names
                     .Select(name => name.Trim())
                     .Where(name => name.Length > 0)
                     .Distinct(StringComparer.OrdinalIgnoreCase)
                     .OrderBy(name => name, StringComparer.OrdinalIgnoreCase))
        {
            builder.Append(name.ToLowerInvariant()).Append(':');
            builder.Append(headers.TryGetValue(name, out var value) ? Hash(value) : "<absent>");
            builder.Append('\n');
        }
        return Hash(builder.ToString());
    }

    private static FrozenDictionary<string, string> SnapshotHeaders(
        IReadOnlyDictionary<string, string>? headers)
    {
        if (headers is null || headers.Count == 0)
        {
            return FrozenDictionary<string, string>.Empty;
        }

        var snapshot = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in headers)
        {
            if (string.IsNullOrWhiteSpace(pair.Key) || s_forbiddenHeaders.Contains(pair.Key))
            {
                throw new ArgumentException($"HTTP header '{pair.Key}' is not allowed.", nameof(headers));
            }
            if (pair.Value.Contains('\r') || pair.Value.Contains('\n'))
            {
                throw new ArgumentException($"HTTP header '{pair.Key}' contains invalid characters.", nameof(headers));
            }
            snapshot[pair.Key] = pair.Value;
        }
        return snapshot.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }

    private static string HashHeaders(FrozenDictionary<string, string> headers)
    {
        if (headers.Count == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        foreach (var pair in headers.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            builder.Append(pair.Key.ToLowerInvariant())
                .Append(':')
                .Append(Hash(pair.Value))
                .Append('\n');
        }
        return Hash(builder.ToString());
    }

    private static string? NormalizeOptionalValue(string? value, string name)
    {
        if (value is null)
        {
            return null;
        }
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be empty or whitespace.", name);
        }
        return value;
    }
}
