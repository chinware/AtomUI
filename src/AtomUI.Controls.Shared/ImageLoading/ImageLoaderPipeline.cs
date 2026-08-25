using Avalonia.Media;

namespace AtomUI.Controls;

internal sealed class ImageLoaderPipeline : IDisposable
{
    private readonly object _epochGate = new();
    private readonly ImageLoadingOptions _options;
    private readonly ImageEncodedCache _encodedCache;
    private readonly ImageDecodedCache _decodedCache;
    private readonly ImageFileCache? _fileCache;
    private readonly ImageRequestScheduler _scheduler;
    private readonly ImageRequestCoordinator _coordinator;
    private readonly ImageSourceReaderRegistry _readers;
    private readonly ImageContentValidator _validator;
    private readonly ImageCodecRegistry _codecs;
    private readonly HttpImageTransport _transport;
    private readonly Dictionary<string, long> _partitionEpochs = [];
    private long _globalEpoch;
    private bool _disposed;

    internal ImageLoaderPipeline(
        ImageLoadingOptions options,
        IEnumerable<ImageCodec> codecs,
        HttpMessageHandler? httpMessageHandler = null)
    {
        _options = options;
        _encodedCache = new ImageEncodedCache(options.EncodedMemoryCacheBytes, options.EncodedMemoryCacheEntries);
        _decodedCache = new ImageDecodedCache(options.DecodedMemoryCacheBytes, options.DecodedMemoryCacheEntries);
        _fileCache = options.IsPersistentCacheEnabled
            ? new ImageFileCache(
                options.PersistentCacheDirectory!,
                options.PersistentCacheBytes,
                options.PersistentCacheEntries)
            : null;
        _scheduler = new ImageRequestScheduler(
            options.MaxConcurrentDownloads,
            options.MaxConcurrentLocalReads,
            options.MaxConcurrentDecodes);
        _coordinator = new ImageRequestCoordinator();
        _validator = new ImageContentValidator(options);
        _codecs = new ImageCodecRegistry(codecs);
        _transport = new HttpImageTransport(options, httpMessageHandler);
        _readers = new ImageSourceReaderRegistry(
        [
            new HttpImageSourceReader(_transport),
            new FileImageSourceReader(options),
            new AssetImageSourceReader(options),
            new StorageFileImageSourceReader(options),
            new BytesImageSourceReader(options),
            new StreamImageSourceReader(options),
            new BorrowedImageSourceReader()
        ]);
    }

    internal int ActiveReads => _scheduler.ActiveReads;
    internal int QueuedReads => _scheduler.QueuedReads;
    internal int ActiveDecodes => _scheduler.ActiveDecodes;
    internal int QueuedDecodes => _scheduler.QueuedDecodes;
    internal bool HasInFlightWork =>
        _coordinator.HasInFlightOperations ||
        ActiveReads != 0 ||
        QueuedReads != 0 ||
        ActiveDecodes != 0 ||
        QueuedDecodes != 0;
    internal int EncodedCacheEntries => _encodedCache.Count;
    internal long EncodedCacheBytes => _encodedCache.Bytes;
    internal int DecodedCacheEntries => _decodedCache.Count;
    internal long DecodedCacheBytes => _decodedCache.Bytes;

    internal async Task<ImageLoadResult> LoadAsync(
        NormalizedImageRequest request,
        CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ImageProgressDispatcher.Report(request.Progress, ImageLoadProgress.Create(ImageLoadStage.Resolving));
        if (request.Source.Kind == ImageLoadSourceKind.Image)
        {
            return await LoadBorrowedImageAsync(request, cancellationToken).ConfigureAwait(false);
        }

        if (CanReadDecodedCache(request.CacheMode))
        {
            foreach (var candidate in _codecs.CreateDecodedKeyCandidates(request))
            {
                if (!_decodedCache.TryAcquireResult(
                        candidate,
                        ImageCacheSource.DecodedMemory,
                        request.Timing.Snapshot(),
                        out var cachedResult))
                {
                    continue;
                }
                ImageProgressDispatcher.Report(request.Progress, ImageLoadProgress.Create(ImageLoadStage.CacheLookup));
                return cachedResult!;
            }
        }

        var epoch = GetEpoch(request.PartitionHash);
        var validated = await _coordinator.GetEncodedAsync(
            request.EncodedOperationKey,
            request.Priority,
            (context, operationCancellation) => LoadEncodedCoreAsync(
                request,
                context,
                operationCancellation),
            request.Progress,
            cancellationToken).ConfigureAwait(false);
        var encoded = validated.Content;
        if (encoded.NoStore)
        {
            _decodedCache.RemoveByEncodedKey(request.EncodedKey);
        }

        var codec = _codecs.Select(validated.Probe, request.Source);
        var decodedKey = codec.CreateDecodedCacheKey(request);
        if (CanReadDecodedCache(request.CacheMode) &&
            _decodedCache.TryAcquireResult(
                decodedKey,
                ImageCacheSource.DecodedMemory,
                request.Timing.Snapshot(),
                out var exactCachedResult))
        {
            ImageProgressDispatcher.Report(request.Progress, ImageLoadProgress.Create(ImageLoadStage.CacheLookup));
            return exactCachedResult!;
        }

        return await _coordinator.GetDecodedAsync(
            ImageCacheKey.CreateDecodedOperationKey(request, decodedKey),
            request.Priority,
            (context, operationCancellation) => DecodeCoreAsync(
                request,
                validated,
                codec,
                decodedKey,
                epoch,
                context,
                operationCancellation),
            entry => entry.AcquireResult(entry.OriginCacheSource, request.Timing.Snapshot()),
            request.Progress,
            cancellationToken).ConfigureAwait(false);
    }

    internal async ValueTask ClearCacheAsync(
        ImageCacheClearRequest request,
        CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(request);
        var partitionHash = request.CachePartition is null
            ? null
            : ImageCacheKey.Hash(request.CachePartition);
        IncrementEpoch(partitionHash);
        if (request.CancelInFlight)
        {
            _coordinator.Cancel(partitionHash);
        }
        if (request.ClearDecodedMemory)
        {
            _decodedCache.Clear(partitionHash);
        }
        if (request.ClearEncodedMemory)
        {
            _encodedCache.Clear(partitionHash);
        }
        if (request.ClearPersistent && _fileCache is not null)
        {
            await _fileCache.ClearAsync(partitionHash, cancellationToken).ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _coordinator.Dispose();
        _scheduler.Dispose();
        _decodedCache.Dispose();
        _encodedCache.Dispose();
        _fileCache?.Dispose();
        _transport.Dispose();
    }

    private async Task<ImageDecodedCacheEntry> DecodeCoreAsync(
        NormalizedImageRequest request,
        ImageValidatedContent validated,
        ImageCodec codec,
        ImageDecodedCacheKey decodedKey,
        (long Global, long Partition) epoch,
        ImageRequestCoordinator.SharedOperationContext context,
        CancellationToken cancellationToken)
    {
        if (CanReadDecodedCache(request.CacheMode) &&
            _decodedCache.TryRetain(decodedKey, out var cachedEntry))
        {
            return cachedEntry!;
        }

        var encoded = validated.Content;
        var probe = validated.Probe;
        context.Report(ImageLoadProgress.Create(ImageLoadStage.Queued));
        var decoded = await _scheduler.ScheduleDecodeAsync(
            async token =>
            {
                context.Report(ImageLoadProgress.Create(ImageLoadStage.Decoding));
                return await codec.DecodeAsync(encoded, probe, request, token).ConfigureAwait(false);
            },
            () => context.Priority,
            cancellationToken,
            static entry => entry.Discard()).ConfigureAwait(false);
        decoded.RetainOperation();
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (CanWriteCache(request, encoded) && IsEpochCurrent(request.PartitionHash, epoch))
            {
                _decodedCache.TryAdd(decodedKey, decoded);
            }
            return decoded;
        }
        catch
        {
            decoded.ReleaseOperation();
            throw;
        }
    }

    private Task<ImageLoadResult> LoadBorrowedImageAsync(
        NormalizedImageRequest request,
        CancellationToken cancellationToken)
    {
        return _coordinator.GetDecodedAsync(
            ImageCacheKey.CreateBorrowedDecodedOperationKey(request),
            request.Priority,
            (context, operationCancellation) => LoadBorrowedImageCoreAsync(
                request,
                context,
                operationCancellation),
            entry => entry.AcquireResult(entry.OriginCacheSource, request.Timing.Snapshot()),
            request.Progress,
            cancellationToken);
    }

    private async Task<ImageDecodedCacheEntry> LoadBorrowedImageCoreAsync(
        NormalizedImageRequest request,
        ImageRequestCoordinator.SharedOperationContext context,
        CancellationToken cancellationToken)
    {
        context.Report(ImageLoadProgress.Create(ImageLoadStage.Reading));
        var readResult = await _readers.Get(request.Source.Kind)
            .ReadAsync(request, null, null, cancellationToken)
            .ConfigureAwait(false);
        var borrowed = readResult.BorrowedImage!;
        var width = Math.Max(1, (int)Math.Ceiling(borrowed.Size.Width));
        var height = Math.Max(1, (int)Math.Ceiling(borrowed.Size.Height));
        var entry = new ImageDecodedCacheEntry(
            borrowed,
            ownsImage: false,
            width,
            height,
            width,
            height,
            checked((long)width * height * 4),
            null,
            ImageCacheSource.Local);
        entry.RetainOperation();
        return entry;
    }

    private async Task<ImageValidatedContent> LoadEncodedCoreAsync(
        NormalizedImageRequest request,
        ImageRequestCoordinator.SharedOperationContext context,
        CancellationToken cancellationToken)
    {
        var epoch = GetEpoch(request.PartitionHash);
        context.Report(ImageLoadProgress.Create(ImageLoadStage.CacheLookup));
        ImageEncodedContent? cached = null;
        if (CanReadEncodedCache(request.CacheMode) && _encodedCache.TryGet(request.EncodedKey, out var memoryContent))
        {
            if (memoryContent!.SecurityPolicyVersion != ImageSecurityPolicy.Version)
            {
                _encodedCache.Remove(request.EncodedKey);
            }
            else if (MatchesVary(request, memoryContent))
            {
                cached = memoryContent;
            }
            else
            {
                _encodedCache.Remove(request.EncodedKey);
            }
            if (cached is not null &&
                (CanUseWithoutSourceProbe(request, cached) ||
                 CanUseCacheOnlyWithoutSourceProbe(request)))
            {
                return ValidateContent(cached, request.Source, cancellationToken);
            }
        }

        if (cached is null && CanReadEncodedCache(request.CacheMode) && _fileCache is not null && request.CanPersist)
        {
            cached = await _fileCache.TryGetAsync(request.EncodedKey, cancellationToken).ConfigureAwait(false);
            if (cached is not null && !MatchesVary(request, cached))
            {
                await _fileCache.RemoveAsync(request.EncodedKey, cancellationToken).ConfigureAwait(false);
                cached = null;
            }
            if (cached is not null &&
                (CanUseWithoutSourceProbe(request, cached) ||
                 CanUseCacheOnlyWithoutSourceProbe(request)))
            {
                var validated = ValidateContent(cached, request.Source, cancellationToken);
                _encodedCache.Set(request.EncodedKey, validated.Content);
                return validated;
            }
        }

        if (request.CacheMode == ImageCacheMode.CacheOnly &&
            request.Source.Kind is not ImageLoadSourceKind.Bytes)
        {
            throw ImageSourceReadHelpers.Failure(
                ImageLoadErrorCode.CacheMiss,
                "The requested image is not available in cache.",
                request.Source.DisplayName);
        }

        context.Report(ImageLoadProgress.Create(ImageLoadStage.Queued));
        var result = await _scheduler.ScheduleReadAsync(
            request.Source.Kind,
            token => _readers.Get(request.Source.Kind).ReadAsync(
                request,
                cached,
                new CallbackProgress<ImageLoadProgress>(context.Report),
                token),
            () => context.Priority,
            cancellationToken).ConfigureAwait(false);
        var content = result.EncodedContent ?? throw new InvalidOperationException("Source reader did not return encoded content.");
        context.Report(ImageLoadProgress.Create(ImageLoadStage.Validating, content.Bytes.LongLength, content.Bytes.LongLength));
        var validatedContent = ValidateContent(content, request.Source, cancellationToken);
        content = validatedContent.Content;

        if (content.NoStore)
        {
            _encodedCache.Remove(request.EncodedKey);
            if (_fileCache is not null)
            {
                await _fileCache.RemoveAsync(request.EncodedKey, cancellationToken).ConfigureAwait(false);
            }
            return validatedContent;
        }
        if (CanWriteCache(request, content) && IsEpochCurrent(request.PartitionHash, epoch))
        {
            _encodedCache.Set(request.EncodedKey, content);
            if (_fileCache is not null && request.CanPersist)
            {
                await _fileCache.SetAsync(request.EncodedKey, content, cancellationToken).ConfigureAwait(false);
            }
        }
        return validatedContent;
    }

    private ImageValidatedContent ValidateContent(
        ImageEncodedContent content,
        ImageLoadSource source,
        CancellationToken cancellationToken)
    {
        var probe = _validator.Validate(content, source, cancellationToken);
        return new ImageValidatedContent(content.MarkValidated(), probe);
    }

    private static bool CanReadDecodedCache(ImageCacheMode mode) =>
        mode is ImageCacheMode.Default or ImageCacheMode.CacheOnly;

    private static bool CanReadEncodedCache(ImageCacheMode mode) =>
        mode is ImageCacheMode.Default or ImageCacheMode.Reload or ImageCacheMode.CacheOnly;

    private static bool CanWriteCache(NormalizedImageRequest request, ImageEncodedContent content) =>
        request.CacheMode is (ImageCacheMode.Default or ImageCacheMode.Reload) && !content.NoStore;

    private static bool CanUseWithoutSourceProbe(
        NormalizedImageRequest request,
        ImageEncodedContent content)
    {
        if (request.CacheMode == ImageCacheMode.Reload)
        {
            return false;
        }
        if (content.IsRemote)
        {
            return content.IsFresh(DateTimeOffset.UtcNow);
        }
        return request.Source.Kind switch
        {
            ImageLoadSourceKind.Asset => true,
            ImageLoadSourceKind.Bytes => true,
            ImageLoadSourceKind.StorageFile or ImageLoadSourceKind.Stream =>
                request.Source.Version is not null && request.Source.Version == content.SourceVersion,
            _ => false
        };
    }

    private static bool CanUseCacheOnlyWithoutSourceProbe(NormalizedImageRequest request) =>
        request.CacheMode == ImageCacheMode.CacheOnly &&
        request.Source.Kind is ImageLoadSourceKind.Asset or
            ImageLoadSourceKind.File or
            ImageLoadSourceKind.StorageFile or
            ImageLoadSourceKind.Stream;

    private static bool MatchesVary(
        NormalizedImageRequest request,
        ImageEncodedContent content)
    {
        return content.VaryHeaders is not { Length: > 0 } varyHeaders ||
               string.Equals(
                   content.VaryDigest,
                   ImageCacheKey.HashSelectedHeaders(request.Headers, varyHeaders),
                   StringComparison.Ordinal);
    }

    private (long Global, long Partition) GetEpoch(string partitionHash)
    {
        lock (_epochGate)
        {
            _partitionEpochs.TryGetValue(partitionHash, out var partitionEpoch);
            return (_globalEpoch, partitionEpoch);
        }
    }

    private bool IsEpochCurrent(string partitionHash, (long Global, long Partition) epoch)
    {
        return GetEpoch(partitionHash) == epoch;
    }

    private void IncrementEpoch(string? partitionHash)
    {
        lock (_epochGate)
        {
            if (partitionHash is null)
            {
                _globalEpoch++;
                _partitionEpochs.Clear();
            }
            else
            {
                _partitionEpochs.TryGetValue(partitionHash, out var current);
                _partitionEpochs[partitionHash] = current + 1;
            }
        }
    }
}
