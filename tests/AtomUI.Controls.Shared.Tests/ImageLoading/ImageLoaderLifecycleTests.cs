using AtomUI.Controls;
using Avalonia;
using Shouldly;
using Xunit;

namespace AtomUI.Controls.Shared.Tests.ImageLoading;

public class ImageLoaderLifecycleTests
{
    [Fact]
    public void Attach_Publishes_One_Application_Loader_And_Detach_Removes_It()
    {
        var application = new Application();
        using var loader = CreateLoader();
        using var competingLoader = CreateLoader();

        application.TryGetImageLoader().ShouldBeNull();
        Should.Throw<InvalidOperationException>(() => application.GetImageLoader())
              .Message.ShouldContain("UseImageLoading");

        loader.Attach(application);

        application.GetImageLoader().ShouldBeSameAs(loader);
        application.TryGetImageLoader().ShouldBeSameAs(loader);
        Should.Throw<InvalidOperationException>(() => competingLoader.Attach(application))
              .Message.ShouldContain("already attached");

        loader.Detach(application);

        application.TryGetImageLoader().ShouldBeNull();
        competingLoader.Attach(application);
        application.GetImageLoader().ShouldBeSameAs(competingLoader);
    }

    [Fact]
    public async Task Dispose_Stops_Publishing_The_Attached_Loader()
    {
        var application = new Application();
        var loader = CreateLoader();
        loader.Attach(application);

        loader.Dispose();

        application.TryGetImageLoader().ShouldBeNull();
        await Should.ThrowAsync<ObjectDisposedException>(() =>
            loader.LoadAsync(
                new ImageLoadRequest(ImageLoadSource.FromBytes(new byte[] { 1 })))
                  .AsTask());
    }

    [Fact]
    public async Task Waiter_Timeout_Is_Typed_And_Does_Not_Cancel_A_Shared_Request_With_Other_Waiters()
    {
        var readStarted = NewSignal();
        var releaseRead = NewSignal();
        var underlyingCanceled = 0;
        var source = ImageLoadSource.FromStream(
            async token =>
            {
                using var registration = token.Register(() => Interlocked.Exchange(ref underlyingCanceled, 1));
                readStarted.TrySetResult();
                await releaseRead.Task.WaitAsync(token);
                return new MemoryStream(ImageLoadingTestSupport.CreatePngHeader());
            },
            "waiter-timeout",
            "v1");
        using var loader = CreateLoader();
        var cancellationToken = TestContext.Current.CancellationToken;

        var survivingWaiter = loader.LoadAsync(
            new ImageLoadRequest(source)
            {
                Options = new ImageRequestOptions { Timeout = TimeSpan.FromSeconds(5) }
            },
            cancellationToken).AsTask();
        await readStarted.Task.WaitAsync(cancellationToken);
        var timedOutWaiter = loader.LoadAsync(
            new ImageLoadRequest(source)
            {
                Options = new ImageRequestOptions { Timeout = TimeSpan.FromMilliseconds(50) }
            },
            cancellationToken).AsTask();

        using var timedOutResult = await timedOutWaiter.WaitAsync(TimeSpan.FromSeconds(5), cancellationToken);

        timedOutResult.IsSuccess.ShouldBeFalse();
        timedOutResult.Error.ShouldNotBeNull().Code.ShouldBe(ImageLoadErrorCode.Timeout);
        Volatile.Read(ref underlyingCanceled).ShouldBe(0);
        survivingWaiter.IsCompleted.ShouldBeFalse();

        releaseRead.TrySetResult();
        using var survivingResult = await survivingWaiter.WaitAsync(cancellationToken);

        survivingResult.IsSuccess.ShouldBeTrue();
        Volatile.Read(ref underlyingCanceled).ShouldBe(0);
    }

    private static ImageLoader CreateLoader()
    {
        return new ImageLoader(
            ImageLoadingTestSupport.CreateOptions(),
            [new PassThroughCodec()]);
    }

    private static TaskCompletionSource NewSignal() =>
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    private sealed class PassThroughCodec : ImageCodec
    {
        internal override string Id => "lifecycle-test";

        internal override int Version => 1;

        internal override bool CanDecode(ImageProbeResult probe, ImageLoadSource source) => true;

        internal override Task<ImageDecodedCacheEntry> DecodeAsync(
            ImageEncodedContent content,
            ImageProbeResult probe,
            NormalizedImageRequest request,
            CancellationToken cancellationToken)
        {
            var width = request.DecodePixelWidth > 0 ? request.DecodePixelWidth : probe.PixelWidth;
            var height = request.DecodePixelHeight > 0 ? request.DecodePixelHeight : probe.PixelHeight;
            return Task.FromResult(new ImageDecodedCacheEntry(
                new TestImage(width, height),
                ownsImage: true,
                probe.PixelWidth,
                probe.PixelHeight,
                width,
                height,
                checked((long)width * height * 4),
                probe.MediaType,
                content.CacheSource));
        }
    }
}
