using AtomUI.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Controls.Shared.Tests.ImageLoading;

public class ImageLoaderPipelineTests
{
    [Fact]
    public async Task Same_Decoded_Request_Shares_Read_And_Decode_And_Returns_Independent_Leases()
    {
        var streamStarted = NewSignal();
        var releaseStream = NewSignal();
        var streamCalls = 0;
        var codec = new TestCodec();
        var source = ImageLoadSource.FromStream(
            async token =>
            {
                Interlocked.Increment(ref streamCalls);
                streamStarted.TrySetResult();
                await releaseStream.Task.WaitAsync(token);
                return new MemoryStream(ImageLoadingTestSupport.CreatePngHeader());
            },
            "shared",
            "v1",
            "shared.png");
        using var loader = CreateLoader(codec);
        var request = new ImageLoadRequest(source) { DecodePixelWidth = 32, DecodePixelHeight = 32 };
        var cancellationToken = TestContext.Current.CancellationToken;

        var firstTask = loader.LoadAsync(request, cancellationToken).AsTask();
        await streamStarted.Task;
        var secondTask = loader.LoadAsync(request, cancellationToken).AsTask();
        releaseStream.TrySetResult();
        using var first = await firstTask;
        using var second = await secondTask;

        streamCalls.ShouldBe(1);
        codec.DecodeCalls.ShouldBe(1);
        first.Image.ShouldBeSameAs(second.Image);
        first.IsSuccess.ShouldBeTrue();
        second.IsSuccess.ShouldBeTrue();
        first.Dispose();
        second.Dispose();
        codec.Images.Single().DisposeCount.ShouldBe(0);
        loader.Dispose();
        codec.Images.Single().DisposeCount.ShouldBe(1);
    }

    [Fact]
    public async Task Different_Decode_Sizes_Share_Encoded_Read_But_Not_Decode()
    {
        var streamStarted = NewSignal();
        var releaseStream = NewSignal();
        var streamCalls = 0;
        var codec = new TestCodec();
        var source = ImageLoadSource.FromStream(
            async token =>
            {
                Interlocked.Increment(ref streamCalls);
                streamStarted.TrySetResult();
                await releaseStream.Task.WaitAsync(token);
                return new MemoryStream(ImageLoadingTestSupport.CreatePngHeader(64, 64));
            },
            "sized",
            "v1");
        using var loader = CreateLoader(codec);
        var cancellationToken = TestContext.Current.CancellationToken;

        var firstTask = loader.LoadAsync(new ImageLoadRequest(source)
        {
            DecodePixelWidth = 16,
            DecodePixelHeight = 16
        }, cancellationToken).AsTask();
        await streamStarted.Task;
        var secondTask = loader.LoadAsync(new ImageLoadRequest(source)
        {
            DecodePixelWidth = 32,
            DecodePixelHeight = 32
        }, cancellationToken).AsTask();
        releaseStream.TrySetResult();
        using var first = await firstTask;
        using var second = await secondTask;

        streamCalls.ShouldBe(1);
        codec.DecodeCalls.ShouldBe(2);
        first.Image.ShouldNotBeSameAs(second.Image);
    }

    [Fact]
    public async Task Sequential_Request_Hits_Decoded_Memory_Cache()
    {
        var codec = new TestCodec();
        var source = ImageLoadSource.FromBytes(
            ImageLoadingTestSupport.CreatePngHeader(),
            "cache",
            "v1");
        using var loader = CreateLoader(codec);
        var request = new ImageLoadRequest(source) { DecodePixelWidth = 16, DecodePixelHeight = 16 };
        var cancellationToken = TestContext.Current.CancellationToken;

        using var first = await loader.LoadAsync(request, cancellationToken);
        using var second = await loader.LoadAsync(request, cancellationToken);

        first.CacheSource.ShouldBe(ImageCacheSource.Local);
        second.CacheSource.ShouldBe(ImageCacheSource.DecodedMemory);
        codec.DecodeCalls.ShouldBe(1);
    }

    [Fact]
    public async Task Cache_Clear_Epoch_Prevents_InFlight_Result_From_Repopulating_Decoded_Cache()
    {
        var decodeStarted = NewSignal();
        var releaseDecode = NewSignal();
        var codec = new TestCodec(decodeStarted, releaseDecode);
        var source = ImageLoadSource.FromBytes(
            ImageLoadingTestSupport.CreatePngHeader(),
            "epoch",
            "v1");
        using var loader = CreateLoader(codec);
        var request = new ImageLoadRequest(source) { DecodePixelWidth = 16, DecodePixelHeight = 16 };
        var cancellationToken = TestContext.Current.CancellationToken;

        var firstTask = loader.LoadAsync(request, cancellationToken).AsTask();
        await decodeStarted.Task;
        await loader.ClearCacheAsync(new ImageCacheClearRequest
        {
            CancelInFlight = false
        }, cancellationToken);
        releaseDecode.TrySetResult();
        using var first = await firstTask;
        using var second = await loader.LoadAsync(request, cancellationToken);

        codec.DecodeCalls.ShouldBe(2);
        second.CacheSource.ShouldNotBe(ImageCacheSource.DecodedMemory);
    }

    [Fact]
    public async Task Borrowed_Image_Result_Is_Not_Disposed_By_Loader_Or_Lease()
    {
        var borrowed = new TestImage(20, 30);
        using var loader = CreateLoader(new TestCodec());

        using var result = await loader.LoadAsync(
            new ImageLoadRequest(ImageLoadSource.FromImage(borrowed)),
            TestContext.Current.CancellationToken);

        result.IsSuccess.ShouldBeTrue();
        result.Image.ShouldBeSameAs(borrowed);
        result.DecodedPixelWidth.ShouldBe(20);
        result.DecodedPixelHeight.ShouldBe(30);
        result.Dispose();
        loader.Dispose();
        borrowed.DisposeCount.ShouldBe(0);
    }

    private static ImageLoader CreateLoader(ImageCodec codec)
    {
        return new ImageLoader(ImageLoadingTestSupport.CreateOptions(), [codec]);
    }

    private static TaskCompletionSource NewSignal() =>
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    private sealed class TestCodec : ImageCodec
    {
        private readonly TaskCompletionSource? _decodeStarted;
        private readonly TaskCompletionSource? _releaseDecode;
        private int _decodeCalls;

        internal TestCodec(
            TaskCompletionSource? decodeStarted = null,
            TaskCompletionSource? releaseDecode = null)
        {
            _decodeStarted = decodeStarted;
            _releaseDecode = releaseDecode;
        }

        internal override string Id => "test.raster";

        internal override int Version => 1;

        internal int DecodeCalls => Volatile.Read(ref _decodeCalls);

        internal List<TestImage> Images { get; } = [];

        internal override bool CanDecode(ImageProbeResult probe, ImageLoadSource source) => true;

        internal override async Task<ImageDecodedCacheEntry> DecodeAsync(
            ImageEncodedContent content,
            ImageProbeResult probe,
            NormalizedImageRequest request,
            CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _decodeCalls);
            _decodeStarted?.TrySetResult();
            if (_releaseDecode is not null)
            {
                await _releaseDecode.Task.WaitAsync(cancellationToken);
            }
            var width = request.DecodePixelWidth > 0 ? request.DecodePixelWidth : probe.PixelWidth;
            var height = request.DecodePixelHeight > 0 ? request.DecodePixelHeight : probe.PixelHeight;
            var image = new TestImage(width, height);
            lock (Images)
            {
                Images.Add(image);
            }
            return new ImageDecodedCacheEntry(
                image,
                ownsImage: true,
                probe.PixelWidth,
                probe.PixelHeight,
                width,
                height,
                checked((long)width * height * 4),
                probe.MediaType,
                content.CacheSource);
        }
    }
}
