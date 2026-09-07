using Avalonia;
using Avalonia.Media;
using Avalonia.Svg;
using Avalonia.Threading;
using Svg;
using Svg.Model;
using AvaloniaSvgImage = Avalonia.Svg.SvgImage;

namespace AtomUI.Controls;

internal sealed class SvgImageCodec : ImageCodec
{
    internal override string Id => "atomui.svg";

    internal override int Version => 2;

    internal override bool IsDecodeSizeDependent => false;

    internal override bool CanDecode(ImageProbeResult probe, ImageSource source)
    {
        return probe.Format == ImageContentFormat.Svg && probe.SvgMetadata is not null;
    }

    internal override async Task<ImageDecodedCacheEntry> DecodeAsync(
        ImageEncodedContent content,
        ImageProbeResult probe,
        NormalizedImageRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        SvgSource? source = null;
        OwnedSvgImage? image = null;
        try
        {
            using var stream = new MemoryStream(content.Bytes, writable: false);
            source = SvgSource.Load(stream, CreateParameters());
            if (source.Picture is null)
            {
                throw ImageSourceReadHelpers.Failure(
                    ImageLoadErrorCode.DecodeFailed,
                    "SVG image decoding did not produce a drawable model.",
                    request.Source.DisplayName);
            }

            cancellationToken.ThrowIfCancellationRequested();
            image = await OwnedSvgImage.CreateAsync(source, cancellationToken).ConfigureAwait(false);
            source = null;
            cancellationToken.ThrowIfCancellationRequested();

            var width = Math.Max(1, (int)Math.Ceiling(image.Size.Width));
            var height = Math.Max(1, (int)Math.Ceiling(image.Size.Height));
            var entry = new ImageDecodedCacheEntry(
                image,
                ownsImage: true,
                width,
                height,
                width,
                height,
                probe.SvgMetadata!.EstimatedDecodedCost,
                probe.MediaType,
                content.Origin);
            image = null;
            return entry;
        }
        catch (ImageLoadFailureException)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw ImageSourceReadHelpers.Failure(
                ImageLoadErrorCode.DecodeFailed,
                "SVG image decoding failed.",
                request.Source.DisplayName,
                exception);
        }
        finally
        {
            image?.Dispose();
            if (source is not null)
            {
                source.Picture = null!;
            }
        }
    }

    internal static SvgParameters CreateParameters()
    {
        return new SvgParameters(
            Entities: null,
            Css: null,
            CurrentColor: null,
            LoadOptions: new SvgDocumentLoadOptions
            {
                ProcessingMode = SvgProcessingMode.SecureStatic,
                ExternalResources = SvgExternalResourcePolicy.SameDocumentAndDataOnly,
                PreserveUnknownElements = false,
                PreferSvg2Href = true
            });
    }

    private sealed class OwnedSvgImage : IImage, IDisposable
    {
        private readonly AvaloniaSvgImage _image;
        private readonly Size _size;
        private SvgSource? _source;

        private OwnedSvgImage(SvgSource source)
        {
            _source = source;
            _image = new AvaloniaSvgImage { Source = source };
            _size = _image.Size;
            if (!double.IsFinite(_size.Width) || !double.IsFinite(_size.Height) ||
                _size.Width <= 0 || _size.Height <= 0)
            {
                _image.Source = null!;
                source.Picture = null!;
                _source = null;
                throw new InvalidOperationException("SVG image has no finite drawable size.");
            }
        }

        public Size Size => _size;

        internal static async Task<OwnedSvgImage> CreateAsync(
            SvgSource source,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (Dispatcher.UIThread.CheckAccess())
            {
                return new OwnedSvgImage(source);
            }
            return await Dispatcher.UIThread.InvokeAsync(
                () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return new OwnedSvgImage(source);
                },
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
