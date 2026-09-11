namespace AtomUI.Controls;

internal sealed class FileImageSourceReader : ImageSourceReader
{
    private readonly ImageLoadingOptions _options;

    internal FileImageSourceReader(ImageLoadingOptions options)
    {
        _options = options;
    }

    internal override ImageSourceKind Kind => ImageSourceKind.File;

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

        var source = (FileImageSource)request.Source;
        var path = source.Path;
        try
        {
            await using var stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite | FileShare.Delete,
                64 * 1024,
                FileOptions.Asynchronous | FileOptions.SequentialScan);
            var creation = File.GetCreationTimeUtc(stream.SafeFileHandle).Ticks;
            var lastWrite = File.GetLastWriteTimeUtc(stream.SafeFileHandle).Ticks;
            var version = new ImageSourceVersion($"{creation}:{stream.Length}:{lastWrite}");
            if (source.Validation == ImageFileValidationMode.Metadata &&
                request.CacheRead != ImageCacheReadPolicy.RefreshSource &&
                staleContent is not null &&
                staleContent.SourceVersion == version)
            {
                return new ImageSourceReadResult(staleContent, SourceValidation: ImageSourceValidation.Current);
            }
            ImageProgressDispatcher.Report(progress, ImageLoadProgress.Create(ImageLoadStage.Reading));
            var bytes = await ImageSourceReadHelpers.ReadAllBytesAsync(
                stream,
                _options.MaxResponseBytes,
                ImageLoadStage.Reading,
                progress,
                stream.Length,
                cancellationToken).ConfigureAwait(false);
            return new ImageSourceReadResult(new ImageEncodedContent(
                bytes,
                null,
                ImageLoadOrigin.Local,
                DateTimeOffset.UtcNow,
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
        catch (FileNotFoundException exception)
        {
            throw ImageSourceReadHelpers.Failure(
                ImageLoadErrorCode.NotFound,
                "Image file was not found.",
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
