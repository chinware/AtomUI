using Avalonia.Media;

namespace AtomUI.Controls;

internal sealed class ImageDecodedCacheEntry
{
    private readonly object _gate = new();
    private IImage? _image;
    private int _leaseCount;
    private int _operationReferences;
    private bool _hasCacheMembership;
    private readonly bool _ownsImage;

    internal ImageDecodedCacheEntry(
        IImage image,
        bool ownsImage,
        int originalPixelWidth,
        int originalPixelHeight,
        int decodedPixelWidth,
        int decodedPixelHeight,
        long decodedBytes,
        string? mediaType,
        ImageCacheSource originCacheSource = ImageCacheSource.Local)
    {
        _image = image ?? throw new ArgumentNullException(nameof(image));
        _ownsImage = ownsImage;
        OriginalPixelWidth = originalPixelWidth;
        OriginalPixelHeight = originalPixelHeight;
        DecodedPixelWidth = decodedPixelWidth;
        DecodedPixelHeight = decodedPixelHeight;
        DecodedBytes = decodedBytes;
        MediaType = mediaType;
        OriginCacheSource = originCacheSource;
    }

    internal int OriginalPixelWidth { get; }

    internal int OriginalPixelHeight { get; }

    internal int DecodedPixelWidth { get; }

    internal int DecodedPixelHeight { get; }

    internal long DecodedBytes { get; }

    internal string? MediaType { get; }

    internal ImageCacheSource OriginCacheSource { get; }

    internal void AddCacheMembership()
    {
        lock (_gate)
        {
            ThrowIfReleased();
            if (_hasCacheMembership)
            {
                throw new InvalidOperationException("Decoded cache entry already has cache membership.");
            }
            _hasCacheMembership = true;
        }
    }

    internal void RetainOperation()
    {
        lock (_gate)
        {
            ThrowIfReleased();
            _operationReferences++;
        }
    }

    internal ImageLoadResult AcquireResult(
        ImageCacheSource cacheSource,
        IReadOnlyDictionary<ImageLoadStage, TimeSpan>? stageDurations = null)
    {
        IImage image;
        lock (_gate)
        {
            ThrowIfReleased();
            _leaseCount++;
            image = _image!;
        }
        return new ImageLoadResult(
            image,
            new ImageResultLease(ReleaseLease),
            OriginalPixelWidth,
            OriginalPixelHeight,
            DecodedPixelWidth,
            DecodedPixelHeight,
            MediaType,
            cacheSource,
            stageDurations);
    }

    internal void ReleaseOperation()
    {
        IImage? dispose = null;
        lock (_gate)
        {
            if (_operationReferences == 0)
            {
                return;
            }
            _operationReferences--;
            dispose = TakeDisposableImageCore();
        }
        DisposeImage(dispose);
    }

    internal void RemoveCacheMembership()
    {
        IImage? dispose = null;
        lock (_gate)
        {
            if (!_hasCacheMembership)
            {
                return;
            }
            _hasCacheMembership = false;
            dispose = TakeDisposableImageCore();
        }
        DisposeImage(dispose);
    }

    private void ReleaseLease()
    {
        IImage? dispose = null;
        lock (_gate)
        {
            if (_leaseCount == 0)
            {
                return;
            }
            _leaseCount--;
            dispose = TakeDisposableImageCore();
        }
        DisposeImage(dispose);
    }

    private IImage? TakeDisposableImageCore()
    {
        if (_image is null || _hasCacheMembership || _operationReferences != 0 || _leaseCount != 0)
        {
            return null;
        }
        var image = _image;
        _image = null;
        return _ownsImage ? image : null;
    }

    private void ThrowIfReleased()
    {
        if (_image is null)
        {
            throw new ObjectDisposedException(nameof(ImageDecodedCacheEntry));
        }
    }

    private static void DisposeImage(IImage? image)
    {
        (image as IDisposable)?.Dispose();
    }
}
