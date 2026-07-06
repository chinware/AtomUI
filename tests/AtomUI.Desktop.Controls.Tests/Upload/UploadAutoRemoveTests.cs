using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using Avalonia.Threading;
using AtomUI.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Upload;

public class UploadAutoRemoveTests
{
    static UploadAutoRemoveTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public async Task SuccessAutoRemoveDelay_Removes_Successful_File_After_Delay()
    {
        var upload = CreateUploadWithFiles(out var files);
        upload.SuccessAutoRemoveDelay = TimeSpan.FromMilliseconds(20);
        var item = CreateItem();
        files.Add(item);

        item.Status = FileUploadStatus.Success;

        await WaitUntilAsync(() => files.Count == 0);
    }

    [Fact]
    public async Task Status_Changing_Away_From_Success_Cancels_Auto_Remove()
    {
        var upload = CreateUploadWithFiles(out var files);
        upload.SuccessAutoRemoveDelay = TimeSpan.FromMilliseconds(20);
        var item = CreateItem();
        files.Add(item);

        item.Status = FileUploadStatus.Success;
        item.Status = FileUploadStatus.Pending;
        await Task.Delay(60, TestContext.Current.CancellationToken);
        Dispatcher.UIThread.RunJobs();

        files.Count.ShouldBe(1);
    }

    [Fact]
    public async Task Manual_Remove_Cancels_Pending_Auto_Remove()
    {
        var upload = CreateUploadWithFiles(out var files);
        upload.SuccessAutoRemoveDelay = TimeSpan.FromMilliseconds(50);
        var item = CreateItem();
        files.Add(item);

        item.Status = FileUploadStatus.Success;
        await upload.RemoveFileAsync(item.Id, TestContext.Current.CancellationToken);
        await Task.Delay(80, TestContext.Current.CancellationToken);
        Dispatcher.UIThread.RunJobs();

        files.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Replacing_Files_Cancels_Pending_Auto_Remove_For_Old_Items()
    {
        var upload = CreateUploadWithFiles(out var oldFiles);
        upload.SuccessAutoRemoveDelay = TimeSpan.FromMilliseconds(20);
        var item = CreateItem();
        oldFiles.Add(item);

        item.Status  = FileUploadStatus.Success;
        upload.Files = new ObservableCollection<UploadFileItem>();
        await Task.Delay(60, TestContext.Current.CancellationToken);
        Dispatcher.UIThread.RunJobs();

        GetPendingAutoRemoveCount(upload).ShouldBe(0);
    }

    [Fact]
    public async Task SetFormValue_Cancels_Pending_Auto_Remove_For_Replaced_Items()
    {
        var upload = CreateUploadWithFiles(out var files);
        upload.SuccessAutoRemoveDelay = TimeSpan.FromMilliseconds(20);
        var item = CreateItem();
        files.Add(item);

        item.Status = FileUploadStatus.Success;
        ((IFormItemAware)upload).SetFormValue(new[] { CreateItem() });
        await Task.Delay(60, TestContext.Current.CancellationToken);
        Dispatcher.UIThread.RunJobs();

        GetPendingAutoRemoveCount(upload).ShouldBe(0);
    }

    private static Desktop.Controls.Upload CreateUploadWithFiles(out IList<UploadFileItem> files)
    {
        files = new ObservableCollection<UploadFileItem>();
        return new Desktop.Controls.Upload
        {
            AutoUpload = false,
            Files      = files
        };
    }

    private static UploadFileItem CreateItem()
    {
        return new UploadFileItem
        {
            Name = "avatar.png",
            Path = new Uri("file:///tmp/avatar.png"),
            Size = 12
        };
    }

    private static int GetPendingAutoRemoveCount(Desktop.Controls.Upload upload)
    {
        var field = typeof(Desktop.Controls.Upload).GetField(
            "_successAutoRemoveDelays",
            BindingFlags.Instance | BindingFlags.NonPublic);
        field.ShouldNotBeNull();
        var delays = field.GetValue(upload);
        delays.ShouldNotBeNull();
        return ((IDictionary)delays).Count;
    }

    private static Task WaitUntilAsync(Func<bool> condition)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        while (!condition())
        {
            timeout.Token.ThrowIfCancellationRequested();
            Dispatcher.UIThread.RunJobs();
            Thread.Sleep(10);
        }

        return Task.CompletedTask;
    }
}
