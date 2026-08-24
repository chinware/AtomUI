using System.Collections.Frozen;

namespace AtomUI.Controls;

public sealed class ImageLoadingOptionsBuilder
{
    private readonly HashSet<string> _authenticationHeaderNames =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "Authorization",
            "Proxy-Authorization",
            "Cookie"
        };
    private readonly HashSet<string> _allowedHttpOrigins = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _credentialForwardingOrigins = new(StringComparer.OrdinalIgnoreCase);

    public int MaxConcurrentDownloads { get; set; } = OperatingSystem.IsBrowser() ? 4 : 6;

    public int MaxConcurrentDecodes { get; set; } = OperatingSystem.IsBrowser()
        ? 2
        : Math.Min(4, Math.Max(1, Environment.ProcessorCount - 1));

    public long EncodedMemoryCacheBytes { get; set; } = OperatingSystem.IsBrowser()
        ? 32L * 1024 * 1024
        : 128L * 1024 * 1024;

    public int EncodedMemoryCacheEntries { get; set; } = OperatingSystem.IsBrowser() ? 128 : 256;

    public long DecodedMemoryCacheBytes { get; set; } = OperatingSystem.IsBrowser()
        ? 64L * 1024 * 1024
        : 256L * 1024 * 1024;

    public int DecodedMemoryCacheEntries { get; set; } = OperatingSystem.IsBrowser() ? 128 : 256;

    public long MaxResponseBytes { get; set; } = OperatingSystem.IsBrowser()
        ? 16L * 1024 * 1024
        : 32L * 1024 * 1024;

    public int MaxImageWidth { get; set; } = OperatingSystem.IsBrowser() ? 8_192 : 16_384;

    public int MaxImageHeight { get; set; } = OperatingSystem.IsBrowser() ? 8_192 : 16_384;

    public long MaxImagePixelCount { get; set; } = OperatingSystem.IsBrowser() ? 32_000_000 : 64_000_000;

    public long MaxDecodedImageBytes { get; set; } = OperatingSystem.IsBrowser()
        ? 128L * 1024 * 1024
        : 256L * 1024 * 1024;

    public TimeSpan DefaultRequestTimeout { get; set; } = TimeSpan.FromSeconds(30);

    public int MaxRedirects { get; set; } = 8;

    public bool IsPersistentCacheEnabled { get; set; }

    public string? PersistentCacheDirectory { get; set; }

    public long PersistentCacheBytes { get; set; } = 512L * 1024 * 1024;

    public int PersistentCacheEntries { get; set; } = 2048;

    public bool AllowAuthenticatedPersistentCache { get; set; }

    public void AddAuthenticationHeaderName(string name)
    {
        ValidateHeaderName(name);
        _authenticationHeaderNames.Add(name);
    }

    public void AllowHttpOrigin(string origin)
    {
        _allowedHttpOrigins.Add(NormalizeOrigin(origin));
    }

    public void AllowCredentialForwardingOrigin(string origin)
    {
        _credentialForwardingOrigins.Add(NormalizeOrigin(origin));
    }

    internal ImageLoadingOptions Build(string applicationId)
    {
        Validate();
        var persistentCacheDirectory = PersistentCacheDirectory;
        if (IsPersistentCacheEnabled && string.IsNullOrWhiteSpace(persistentCacheDirectory))
        {
            var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (string.IsNullOrWhiteSpace(root))
            {
                root = Path.GetTempPath();
            }
            persistentCacheDirectory = Path.Combine(
                root,
                "AtomUI",
                "ImageCache",
                ImageCacheKey.Hash(applicationId)[..16]);
        }
        return new ImageLoadingOptions(
            MaxConcurrentDownloads,
            MaxConcurrentDecodes,
            EncodedMemoryCacheBytes,
            EncodedMemoryCacheEntries,
            DecodedMemoryCacheBytes,
            DecodedMemoryCacheEntries,
            MaxResponseBytes,
            MaxImageWidth,
            MaxImageHeight,
            MaxImagePixelCount,
            MaxDecodedImageBytes,
            DefaultRequestTimeout,
            MaxRedirects,
            IsPersistentCacheEnabled,
            persistentCacheDirectory,
            PersistentCacheBytes,
            PersistentCacheEntries,
            AllowAuthenticatedPersistentCache,
            _authenticationHeaderNames.ToFrozenSet(StringComparer.OrdinalIgnoreCase),
            _allowedHttpOrigins.ToFrozenSet(StringComparer.OrdinalIgnoreCase),
            _credentialForwardingOrigins.ToFrozenSet(StringComparer.OrdinalIgnoreCase));
    }

    private void Validate()
    {
        var downloadCap = OperatingSystem.IsBrowser() ? 8 : 32;
        var decodeCap = OperatingSystem.IsBrowser() ? 4 : 8;
        ValidateRange(MaxConcurrentDownloads, 1, downloadCap, nameof(MaxConcurrentDownloads));
        ValidateRange(MaxConcurrentDecodes, 1, decodeCap, nameof(MaxConcurrentDecodes));
        ValidatePositive(EncodedMemoryCacheBytes, nameof(EncodedMemoryCacheBytes));
        ValidatePositive(EncodedMemoryCacheEntries, nameof(EncodedMemoryCacheEntries));
        ValidatePositive(DecodedMemoryCacheBytes, nameof(DecodedMemoryCacheBytes));
        ValidatePositive(DecodedMemoryCacheEntries, nameof(DecodedMemoryCacheEntries));
        ValidatePositive(MaxResponseBytes, nameof(MaxResponseBytes));
        ValidatePositive(MaxImageWidth, nameof(MaxImageWidth));
        ValidatePositive(MaxImageHeight, nameof(MaxImageHeight));
        ValidatePositive(MaxImagePixelCount, nameof(MaxImagePixelCount));
        ValidatePositive(MaxDecodedImageBytes, nameof(MaxDecodedImageBytes));
        ValidatePositive(PersistentCacheBytes, nameof(PersistentCacheBytes));
        ValidatePositive(PersistentCacheEntries, nameof(PersistentCacheEntries));
        ValidateRange(MaxRedirects, 0, 32, nameof(MaxRedirects));
        if (DefaultRequestTimeout <= TimeSpan.Zero || DefaultRequestTimeout == Timeout.InfiniteTimeSpan)
        {
            throw new InvalidOperationException("DefaultRequestTimeout must be a finite positive duration.");
        }
        if (OperatingSystem.IsBrowser() && IsPersistentCacheEnabled)
        {
            throw new PlatformNotSupportedException("Persistent image cache is not available in Browser applications.");
        }
    }

    private static void ValidateHeaderName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        const string separators = "()<>@,;:/[]?={}\t";
        if (name.Any(character =>
                character <= 32 || character >= 127 || character == '\\' || character == '"' ||
                separators.Contains(character)))
        {
            throw new ArgumentException("Invalid HTTP header name.", nameof(name));
        }
    }

    private static string NormalizeOrigin(string origin)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(origin);
        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) ||
            !string.IsNullOrEmpty(uri.UserInfo) ||
            uri.AbsolutePath != "/" ||
            !string.IsNullOrEmpty(uri.Query) ||
            !string.IsNullOrEmpty(uri.Fragment))
        {
            throw new ArgumentException("Origin must contain only an HTTP scheme, host, and optional port.", nameof(origin));
        }

        return ImageCacheKey.NormalizeOrigin(uri);
    }

    private static void ValidatePositive(long value, string name)
    {
        if (value <= 0)
        {
            throw new InvalidOperationException($"{name} must be greater than zero.");
        }
    }

    private static void ValidateRange(int value, int minimum, int maximum, string name)
    {
        if (value < minimum || value > maximum)
        {
            throw new InvalidOperationException($"{name} must be between {minimum} and {maximum}.");
        }
    }
}
