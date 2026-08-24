using Avalonia;
using Avalonia.Media;
using Avalonia.Svg;
using Avalonia.Threading;

namespace AtomUI.Controls;

internal sealed class AssetSvgImageCodec : ImageCodec
{
    internal override string Id => "atomui.asset-svg";

    internal override int Version => 1;

    internal override bool CanDecode(ImageProbeResult probe, ImageLoadSource source)
    {
        return probe.Format == ImageContentFormat.Svg && source.Kind == ImageLoadSourceKind.Asset;
    }

    internal override async Task<ImageDecodedCacheEntry> DecodeAsync(
        ImageEncodedContent content,
        ImageProbeResult probe,
        NormalizedImageRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            using var stream = new MemoryStream(content.Bytes, writable: false);
            var source = SvgSource.Load(stream, parameters: null);
            var image = await OwnedSvgImage.CreateAsync(source, cancellationToken);
            var width = Math.Max(1, (int)Math.Ceiling(image.Size.Width));
            var height = Math.Max(1, (int)Math.Ceiling(image.Size.Height));
            return new ImageDecodedCacheEntry(
                image,
                ownsImage: true,
                width,
                height,
                width,
                height,
                checked((long)width * height * 4),
                probe.MediaType,
                content.CacheSource);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            throw ImageSourceReadHelpers.Failure(
                ImageLoadErrorCode.DecodeFailed,
                "SVG image decoding failed.",
                request.Source.DisplayName,
                exception);
        }
    }

    private sealed class OwnedSvgImage : IImage, IDisposable
    {
        private readonly SvgImage _image;
        private readonly Size _size;
        private SvgSource? _source;

        private OwnedSvgImage(SvgSource source)
        {
            _source = source;
            _image = new SvgImage { Source = source };
            _size = _image.Size;
        }

        public Size Size => _size;

        internal static async Task<OwnedSvgImage> CreateAsync(
            SvgSource source,
            CancellationToken cancellationToken)
        {
            if (Dispatcher.UIThread.CheckAccess())
            {
                return new OwnedSvgImage(source);
            }
            return await Dispatcher.UIThread.InvokeAsync(
                () => new OwnedSvgImage(source),
                DispatcherPriority.Background,
                cancellationToken);
        }

        public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
        {
            if (Volatile.Read(ref _source) is not null)
            {
                ((IImage)_image).Draw(context, sourceRect, destRect);
            }
        }

        public void Dispose()
        {
            var source = Interlocked.Exchange(ref _source, null);
            if (source is null)
            {
                return;
            }
            if (Dispatcher.UIThread.CheckAccess())
            {
                ReleaseSource(source);
                return;
            }
            Dispatcher.UIThread.Post(
                () => ReleaseSource(source),
                DispatcherPriority.Send);
        }

        private void ReleaseSource(SvgSource source)
        {
            _image.Source = null!;
            source.Picture = null!;
        }
    }
}
