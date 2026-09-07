using System.Buffers.Binary;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.ImageLoading;

public class ImageLoaderDisposeTests
{
    public ImageLoaderDisposeTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public async Task Dispose_On_UI_Thread_Does_Not_Block_An_InFlight_UI_Dispatch()
    {
        var decodeStarted = NewSignal();
        var releaseDecode = NewSignal();
        var options = new ImageLoadingOptionsBuilder
        {
            IsPersistentCacheEnabled = false,
            MaxConcurrentDownloads = 1,
            MaxConcurrentLocalReads = 1,
            MaxConcurrentDecodes = 1
        }.Build("AtomUI.Desktop.Controls.Tests");
        using var loader = new ImageLoader(
            options,
            [new DispatcherBlockingCodec(decodeStarted, releaseDecode)]);

        var loadTask = loader.LoadAsync(
            new ImageLoadRequest(new BytesImageSource(CreatePngHeader())),
            TestContext.Current.CancellationToken).AsTask();
        await decodeStarted.Task.WaitAsync(
            TimeSpan.FromSeconds(2),
            TestContext.Current.CancellationToken);

        var disposeTask = Dispatcher.UIThread.InvokeAsync(() =>
        {
            releaseDecode.TrySetResult();
            loader.Dispose();
        }).GetTask();

        await disposeTask.WaitAsync(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);
        await Should.ThrowAsync<ObjectDisposedException>(async () => await loadTask);
    }

    private static byte[] CreatePngHeader()
    {
        var bytes = new byte[45];
        new byte[] { 0x89, (byte)'P', (byte)'N', (byte)'G', 0x0d, 0x0a, 0x1a, 0x0a }
            .CopyTo(bytes, 0);
        BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(8, 4), 13);
        "IHDR"u8.CopyTo(bytes.AsSpan(12));
        BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(16, 4), 2);
        BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(20, 4), 2);
        bytes[24] = 8;
        bytes[25] = 6;
        "IEND"u8.CopyTo(bytes.AsSpan(37));
        return bytes;
    }

    private static TaskCompletionSource NewSignal() =>
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    private sealed class DispatcherBlockingCodec : ImageCodec
    {
        private readonly TaskCompletionSource _decodeStarted;
        private readonly TaskCompletionSource _releaseDecode;

        internal DispatcherBlockingCodec(
            TaskCompletionSource decodeStarted,
            TaskCompletionSource releaseDecode)
        {
            _decodeStarted = decodeStarted;
            _releaseDecode = releaseDecode;
        }

        internal override string Id => "dispose-ui-dispatch";

        internal override int Version => 1;

        internal override bool CanDecode(ImageProbeResult probe, ImageSource source) => true;

        internal override async Task<ImageDecodedCacheEntry> DecodeAsync(
            ImageEncodedContent content,
            ImageProbeResult probe,
            NormalizedImageRequest request,
            CancellationToken cancellationToken)
        {
            _decodeStarted.TrySetResult();
            await _releaseDecode.Task.WaitAsync(cancellationToken);
            await Dispatcher.UIThread.InvokeAsync(
                static () => { },
                DispatcherPriority.Background,
                cancellationToken);
            return new ImageDecodedCacheEntry(
                new DispatcherTestImage(),
                ownsImage: true,
                probe.PixelWidth,
                probe.PixelHeight,
                probe.PixelWidth,
                probe.PixelHeight,
                checked((long)probe.PixelWidth * probe.PixelHeight * 4),
                probe.MediaType,
                content.Origin);
        }
    }

    private sealed class DispatcherTestImage : IImage, IDisposable
    {
        public Size Size => new(2, 2);

        public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
        {
        }

        public void Dispose()
        {
        }
    }
}
