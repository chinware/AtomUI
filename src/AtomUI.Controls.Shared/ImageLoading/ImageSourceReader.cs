using Avalonia.Media;

namespace AtomUI.Controls;

internal sealed record ImageSourceReadResult(
    ImageEncodedContent? EncodedContent = null,
    IImage? BorrowedImage = null);

internal abstract class ImageSourceReader
{
    internal abstract ImageLoadSourceKind Kind { get; }

    internal abstract Task<ImageSourceReadResult> ReadAsync(
        NormalizedImageRequest request,
        ImageEncodedContent? staleContent,
        IProgress<ImageLoadProgress>? progress,
        CancellationToken cancellationToken);
}

internal sealed class ImageSourceReaderRegistry
{
    private readonly IReadOnlyDictionary<ImageLoadSourceKind, ImageSourceReader> _readers;

    internal ImageSourceReaderRegistry(IEnumerable<ImageSourceReader> readers)
    {
        var dictionary = new Dictionary<ImageLoadSourceKind, ImageSourceReader>();
        foreach (var reader in readers)
        {
            if (!dictionary.TryAdd(reader.Kind, reader))
            {
                throw new InvalidOperationException($"Image source reader '{reader.Kind}' is registered more than once.");
            }
        }
        foreach (var kind in Enum.GetValues<ImageLoadSourceKind>())
        {
            if (!dictionary.ContainsKey(kind))
            {
                throw new InvalidOperationException($"Image source reader '{kind}' is not registered.");
            }
        }
        _readers = dictionary;
    }

    internal ImageSourceReader Get(ImageLoadSourceKind kind) => _readers[kind];
}

internal sealed class ImageLoadFailureException : Exception
{
    internal ImageLoadFailureException(ImageLoadError error)
        : base(error.Message, error.Exception)
    {
        Error = error;
    }

    internal ImageLoadError Error { get; }
}

internal static class ImageSourceReadHelpers
{
    internal static async Task<byte[]> ReadAllBytesAsync(
        Stream stream,
        long maxBytes,
        ImageLoadStage stage,
        IProgress<ImageLoadProgress>? progress,
        long? totalBytes,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanRead)
        {
            throw Failure(ImageLoadErrorCode.InvalidSource, "Image stream is not readable.");
        }
        if (totalBytes > maxBytes)
        {
            throw Failure(ImageLoadErrorCode.ResponseTooLarge, "Image content exceeds the configured size limit.");
        }

        var initialCapacity = totalBytes is > 0 and <= int.MaxValue ? (int)totalBytes.Value : 0;
        using var buffer = new MemoryStream(initialCapacity);
        var chunk = new byte[64 * 1024];
        long received = 0;
        var lastReport = DateTimeOffset.MinValue;
        long lastReportedBytes = 0;
        while (true)
        {
            var read = await stream.ReadAsync(chunk, cancellationToken).ConfigureAwait(false);
            if (read == 0)
            {
                break;
            }
            received += read;
            if (received > maxBytes)
            {
                throw Failure(ImageLoadErrorCode.ResponseTooLarge, "Image content exceeds the configured size limit.");
            }
            buffer.Write(chunk, 0, read);

            var now = DateTimeOffset.UtcNow;
            if (received - lastReportedBytes >= 64 * 1024 || now - lastReport >= TimeSpan.FromMilliseconds(50))
            {
                progress?.Report(ImageLoadProgress.Create(stage, received, totalBytes));
                lastReport = now;
                lastReportedBytes = received;
            }
        }
        progress?.Report(ImageLoadProgress.Create(stage, received, totalBytes));
        return buffer.ToArray();
    }

    internal static ImageLoadFailureException Failure(
        ImageLoadErrorCode code,
        string message,
        string? sourceDisplayName = null,
        Exception? exception = null,
        int? httpStatus = null)
    {
        return new ImageLoadFailureException(
            new ImageLoadError(code, message, httpStatus, sourceDisplayName, exception));
    }
}
