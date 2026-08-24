namespace AtomUI.Controls;

internal sealed class FileImageSourceReader : ImageSourceReader
{
    private readonly ImageLoadingOptions _options;

    internal FileImageSourceReader(ImageLoadingOptions options)
    {
        _options = options;
    }

    internal override ImageLoadSourceKind Kind => ImageLoadSourceKind.File;

    internal override async Task<ImageSourceReadResult> ReadAsync(
        NormalizedImageRequest request,
        ImageEncodedContent? staleContent,
        IProgress<ImageLoadProgress>? progress,
        CancellationToken cancellationToken)
    {
        if (OperatingSystem.IsBrowser())
        {
            throw ImageSourceReadHelpers.Failure(
                ImageLoadErrorCode.AccessDenied,
                "Direct file access is not available on this platform.",
                request.Source.DisplayName);
        }

        var path = (string)request.Source.Value;
        try
        {
            var info = new FileInfo(path);
            if (!info.Exists)
            {
                throw ImageSourceReadHelpers.Failure(
                    ImageLoadErrorCode.NotFound,
                    "Image file was not found.",
                    request.Source.DisplayName);
            }
            var version = $"{info.Length}:{info.LastWriteTimeUtc.Ticks}";
            if (request.CacheMode != ImageCacheMode.Reload &&
                staleContent is not null &&
                staleContent.SourceVersion == version)
            {
                return new ImageSourceReadResult(staleContent.WithCacheSource(ImageCacheSource.EncodedMemory));
            }
            await using var stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                64 * 1024,
                FileOptions.Asynchronous | FileOptions.SequentialScan);
            progress?.Report(ImageLoadProgress.Create(ImageLoadStage.Reading));
            var bytes = await ImageSourceReadHelpers.ReadAllBytesAsync(
                stream,
                _options.MaxResponseBytes,
                ImageLoadStage.Reading,
                progress,
                info.Length,
                cancellationToken).ConfigureAwait(false);
            return new ImageSourceReadResult(new ImageEncodedContent(
                bytes,
                null,
                ImageCacheSource.Local,
                DateTimeOffset.UtcNow,
                FreshUntil: DateTimeOffset.MaxValue,
                SourceVersion: version));
        }
        catch (ImageLoadFailureException)
        {
            throw;
        }
        catch (UnauthorizedAccessException exception)
        {
            throw ImageSourceReadHelpers.Failure(
                ImageLoadErrorCode.AccessDenied,
                "Access to the image file was denied.",
                request.Source.DisplayName,
                exception);
        }
        catch (IOException exception)
        {
            throw ImageSourceReadHelpers.Failure(
                ImageLoadErrorCode.InvalidSource,
                "The image file could not be read.",
                request.Source.DisplayName,
                exception);
        }
    }
}
