using System.Reflection;
using AtomUI.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Controls.Shared.Tests.Net;

public class UploadSchedulerTests
{
    [Fact]
    public async Task Running_Task_Count_Increments_When_Task_Starts()
    {
        var transport = new BlockingUploadTransport();
        var scheduler = new FileUploadScheduler(transport);
        var task      = CreateUploadTask("running.txt");

        scheduler.EnqueueTask(task);
        await transport.Started.Task.WaitAsync(TimeSpan.FromSeconds(3));

        GetRunningTaskCount(scheduler).ShouldBe(1);
        task.Status.ShouldBe(FileUploadStatus.Uploading);

        await scheduler.CancelAllAsync();
    }

    [Fact]
    public async Task Running_Task_Count_Decrements_When_Upload_Completes()
    {
        var transport = new BlockingUploadTransport();
        var scheduler = new FileUploadScheduler(transport);
        var task      = CreateUploadTask("complete.txt");

        scheduler.EnqueueTask(task);
        await transport.Started.Task.WaitAsync(TimeSpan.FromSeconds(3));
        GetRunningTaskCount(scheduler).ShouldBe(1);

        transport.Complete(FileUploadResult.SuccessResult(new Uri("https://example.com/complete.txt"), 12, TimeSpan.FromMilliseconds(1)));
        await WaitUntilAsync(() => task.Status == FileUploadStatus.Success);

        GetRunningTaskCount(scheduler).ShouldBe(0);
    }

    [Fact]
    public async Task CancelAllAsync_Cancels_Running_Task()
    {
        var transport = new BlockingUploadTransport();
        var scheduler = new FileUploadScheduler(transport);
        var task      = CreateUploadTask("cancel.txt");

        scheduler.EnqueueTask(task);
        await transport.Started.Task.WaitAsync(TimeSpan.FromSeconds(3));
        task.Status.ShouldBe(FileUploadStatus.Uploading);

        await scheduler.CancelAllAsync();
        await WaitUntilAsync(() => task.Status == FileUploadStatus.Cancelled);

        GetRunningTaskCount(scheduler).ShouldBe(0);
    }

    [Fact]
    public async Task SetTransportAsync_Cancels_Running_Task_Before_Replacing_Transport()
    {
        var oldTransport = new BlockingUploadTransport();
        var newTransport = new BlockingUploadTransport();
        var scheduler    = new FileUploadScheduler(oldTransport);
        var task         = CreateUploadTask("replace.txt");

        scheduler.EnqueueTask(task);
        await oldTransport.Started.Task.WaitAsync(TimeSpan.FromSeconds(3));
        task.Status.ShouldBe(FileUploadStatus.Uploading);

        await scheduler.SetTransportAsync(newTransport);
        await WaitUntilAsync(() => task.Status == FileUploadStatus.Cancelled);

        scheduler.Transport.ShouldBeSameAs(newTransport);
        GetRunningTaskCount(scheduler).ShouldBe(0);
    }

    private static FileUploadTask CreateUploadTask(string name)
    {
        return new FileUploadTask
        {
            UploadFileInfo = new UploadFileInfo(name, new Uri($"file:///tmp/{name}"), 12)
        };
    }

    private static int GetRunningTaskCount(FileUploadScheduler scheduler)
    {
        var field = typeof(FileUploadScheduler).GetField("_runningTasks", BindingFlags.Instance | BindingFlags.NonPublic);
        field.ShouldNotBeNull();
        var runningTasks = field.GetValue(scheduler);
        runningTasks.ShouldNotBeNull();
        var countProperty = runningTasks.GetType().GetProperty("Count");
        countProperty.ShouldNotBeNull();
        return countProperty.GetValue(runningTasks).ShouldBeAssignableTo<int>();
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        while (!condition())
        {
            timeout.Token.ThrowIfCancellationRequested();
            await Task.Delay(10, timeout.Token);
        }
    }

    private sealed class BlockingUploadTransport : IFileUploadTransport
    {
        public TaskCompletionSource<UploadFileInfo> Started { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource<bool> CancellationObserved { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        private readonly TaskCompletionSource<FileUploadResult> _completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public void Complete(FileUploadResult result)
        {
            _completion.TrySetResult(result);
        }

        public async Task<FileUploadResult> UploadAsync(
            UploadFileInfo fileInfo,
            object? context = null,
            IProgress<FileUploadProgress>? progress = null,
            CancellationToken cancellationToken = default)
        {
            using var registration = cancellationToken.Register(static state =>
            {
                ((TaskCompletionSource<bool>)state!).TrySetResult(true);
            }, CancellationObserved);
            Started.TrySetResult(fileInfo);

            return await _completion.Task.WaitAsync(cancellationToken);
        }
    }
}
