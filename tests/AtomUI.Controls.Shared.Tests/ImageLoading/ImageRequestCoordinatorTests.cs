using AtomUI.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Controls.Shared.Tests.ImageLoading;

public class ImageRequestCoordinatorTests
{
    [Fact]
    public async Task Decoded_Waiters_Share_One_Operation_And_Acquire_Leases_Before_Release()
    {
        using var coordinator = new ImageRequestCoordinator();
        var key = CreateDecodedOperationKey("shared");
        var started = NewSignal();
        var release = NewSignal();
        var image = new TestImage();
        var factoryCalls = 0;

        async Task<ImageDecodedCacheEntry> Factory(
            ImageRequestCoordinator.SharedOperationContext context,
            CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref factoryCalls);
            started.TrySetResult();
            await release.Task.WaitAsync(cancellationToken);
            var entry = CreateOperationEntry(image);
            entry.RetainOperation();
            return entry;
        }

        var firstTask = coordinator.GetDecodedAsync(
            key,
            ImageRequestPriority.Normal,
            Factory,
            entry => entry.AcquireResult(ImageCacheSource.Local),
            null,
            CancellationToken.None);
        await started.Task;
        var secondTask = coordinator.GetDecodedAsync(
            key,
            ImageRequestPriority.High,
            Factory,
            entry => entry.AcquireResult(ImageCacheSource.Local),
            null,
            CancellationToken.None);
        release.TrySetResult();

        using var first = await firstTask;
        using var second = await secondTask;

        factoryCalls.ShouldBe(1);
        first.Image.ShouldBeSameAs(image);
        second.Image.ShouldBeSameAs(image);
        image.DisposeCount.ShouldBe(0);
        first.Dispose();
        image.DisposeCount.ShouldBe(0);
        second.Dispose();
        image.DisposeCount.ShouldBe(1);
    }

    [Fact]
    public async Task Canceling_One_Waiter_Does_Not_Cancel_Shared_Work_For_Remaining_Waiter()
    {
        using var coordinator = new ImageRequestCoordinator();
        using var firstCancellation = new CancellationTokenSource();
        var key = CreateEncodedOperationKey("cancel-one");
        var started = NewSignal();
        var release = NewSignal();
        var underlyingCanceled = false;

        async Task<ImageEncodedContent> Factory(
            ImageRequestCoordinator.SharedOperationContext context,
            CancellationToken cancellationToken)
        {
            started.TrySetResult();
            try
            {
                await release.Task.WaitAsync(cancellationToken);
                return ImageLoadingTestSupport.CreateContent([1]);
            }
            catch (OperationCanceledException)
            {
                underlyingCanceled = true;
                throw;
            }
        }

        var first = coordinator.GetEncodedAsync(
            key,
            ImageRequestPriority.Normal,
            Factory,
            null,
            firstCancellation.Token);
        await started.Task;
        var second = coordinator.GetEncodedAsync(
            key,
            ImageRequestPriority.Normal,
            Factory,
            null,
            CancellationToken.None);

        firstCancellation.Cancel();
        await Should.ThrowAsync<OperationCanceledException>(async () => await first);
        underlyingCanceled.ShouldBeFalse();

        release.TrySetResult();
        (await second).Bytes.ShouldBe([1]);
        underlyingCanceled.ShouldBeFalse();
    }

    [Fact]
    public async Task Last_Waiter_Cancellation_Cancels_Underlying_Operation()
    {
        using var coordinator = new ImageRequestCoordinator();
        using var cancellation = new CancellationTokenSource();
        var started = NewSignal();
        var underlyingCanceled = NewSignal();

        var task = coordinator.GetEncodedAsync(
            CreateEncodedOperationKey("cancel-last"),
            ImageRequestPriority.Normal,
            async (_, token) =>
            {
                started.TrySetResult();
                try
                {
                    await Task.Delay(Timeout.InfiniteTimeSpan, token);
                    throw new InvalidOperationException();
                }
                catch (OperationCanceledException)
                {
                    underlyingCanceled.TrySetResult();
                    throw;
                }
            },
            null,
            cancellation.Token);
        await started.Task;

        cancellation.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(async () => await task);
        await underlyingCanceled.Task;
    }

    [Fact]
    public async Task Completed_Operation_Is_Not_Reused_By_A_New_Waiter()
    {
        using var coordinator = new ImageRequestCoordinator();
        var calls = 0;
        var key = CreateEncodedOperationKey("completed");

        Task<ImageEncodedContent> Factory(
            ImageRequestCoordinator.SharedOperationContext context,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(ImageLoadingTestSupport.CreateContent([
                checked((byte)Interlocked.Increment(ref calls))
            ]));
        }

        var first = await coordinator.GetEncodedAsync(
            key,
            ImageRequestPriority.Normal,
            Factory,
            null,
            CancellationToken.None);
        var second = await coordinator.GetEncodedAsync(
            key,
            ImageRequestPriority.Normal,
            Factory,
            null,
            CancellationToken.None);

        first.Bytes.ShouldBe([1]);
        second.Bytes.ShouldBe([2]);
        calls.ShouldBe(2);
    }

    [Fact]
    public async Task Shared_Operation_Priority_Tracks_Highest_Active_Waiter()
    {
        using var coordinator = new ImageRequestCoordinator();
        using var criticalCancellation = new CancellationTokenSource();
        var contextSignal = new TaskCompletionSource<ImageRequestCoordinator.SharedOperationContext>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var release = NewSignal();
        var key = CreateEncodedOperationKey("priority");

        async Task<ImageEncodedContent> Factory(
            ImageRequestCoordinator.SharedOperationContext context,
            CancellationToken cancellationToken)
        {
            contextSignal.TrySetResult(context);
            await release.Task.WaitAsync(cancellationToken);
            return ImageLoadingTestSupport.CreateContent([1]);
        }

        var preload = coordinator.GetEncodedAsync(
            key,
            ImageRequestPriority.Preload,
            Factory,
            null,
            CancellationToken.None);
        var context = await contextSignal.Task;
        context.Priority.ShouldBe(ImageRequestPriority.Preload);
        var critical = coordinator.GetEncodedAsync(
            key,
            ImageRequestPriority.Critical,
            Factory,
            null,
            criticalCancellation.Token);
        context.Priority.ShouldBe(ImageRequestPriority.Critical);

        criticalCancellation.Cancel();
        await Should.ThrowAsync<OperationCanceledException>(async () => await critical);
        context.Priority.ShouldBe(ImageRequestPriority.Preload);

        release.TrySetResult();
        await preload;
    }

    private static ImageDecodedCacheEntry CreateOperationEntry(TestImage image)
    {
        return new ImageDecodedCacheEntry(
            image,
            ownsImage: true,
            10,
            10,
            10,
            10,
            400,
            "image/png");
    }

    private static ImageEncodedOperationKey CreateEncodedOperationKey(string value)
    {
        return new ImageEncodedOperationKey(
            new ImageEncodedCacheKey(value, string.Empty),
            ImageCacheMode.Default,
            string.Empty);
    }

    private static ImageDecodedOperationKey CreateDecodedOperationKey(string value)
    {
        return new ImageDecodedOperationKey(
            new ImageDecodedCacheKey(
                new ImageEncodedCacheKey(value, string.Empty),
                10,
                10,
                "codec"),
            ImageCacheMode.Default,
            string.Empty);
    }

    private static TaskCompletionSource NewSignal() =>
        new(TaskCreationOptions.RunContinuationsAsynchronously);
}
