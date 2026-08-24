using Avalonia.Media.Imaging;

namespace AtomUI.Controls;

internal sealed class RasterImageCodec : ImageCodec
{
    internal override string Id => "atomui.raster";

    internal override int Version => 1;

    internal override bool CanDecode(ImageProbeResult probe, ImageLoadSource source)
    {
        return probe.Format != ImageContentFormat.Svg;
    }

    internal override Task<ImageDecodedCacheEntry> DecodeAsync(
        ImageEncodedContent content,
        ImageProbeResult probe,
        NormalizedImageRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            using var stream = new MemoryStream(content.Bytes, writable: false);
            var (targetWidth, targetHeight) = CalculateTargetSize(
                probe.PixelWidth,
                probe.PixelHeight,
                request.DecodePixelWidth,
                request.DecodePixelHeight);
            Bitmap bitmap;
            if (targetWidth >= probe.PixelWidth && targetHeight >= probe.PixelHeight)
            {
                bitmap = new Bitmap(stream);
            }
            else if ((double)targetWidth / probe.PixelWidth <= (double)targetHeight / probe.PixelHeight)
            {
                bitmap = Bitmap.DecodeToWidth(stream, targetWidth, BitmapInterpolationMode.HighQuality);
            }
            else
            {
                bitmap = Bitmap.DecodeToHeight(stream, targetHeight, BitmapInterpolationMode.HighQuality);
            }
            var decodedWidth = bitmap.PixelSize.Width;
            var decodedHeight = bitmap.PixelSize.Height;
            var decodedBytes = checked((long)decodedWidth * decodedHeight * 4);
            return Task.FromResult(new ImageDecodedCacheEntry(
                bitmap,
                ownsImage: true,
                probe.PixelWidth,
                probe.PixelHeight,
                decodedWidth,
                decodedHeight,
                decodedBytes,
                probe.MediaType,
                content.CacheSource));
        }
        catch (ImageLoadFailureException)
        {
            throw;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            throw ImageSourceReadHelpers.Failure(
                ImageLoadErrorCode.DecodeFailed,
                "Image decoding failed.",
                request.Source.DisplayName,
                exception);
        }
    }

    private static (int Width, int Height) CalculateTargetSize(
        int originalWidth,
        int originalHeight,
        int maximumWidth,
        int maximumHeight)
    {
        if (maximumWidth == 0 && maximumHeight == 0)
        {
            return (originalWidth, originalHeight);
        }
        var widthScale = maximumWidth > 0 ? (double)maximumWidth / originalWidth : double.PositiveInfinity;
        var heightScale = maximumHeight > 0 ? (double)maximumHeight / originalHeight : double.PositiveInfinity;
        var scale = Math.Min(1, Math.Min(widthScale, heightScale));
        return (
            Math.Max(1, (int)Math.Round(originalWidth * scale)),
            Math.Max(1, (int)Math.Round(originalHeight * scale)));
    }
}
