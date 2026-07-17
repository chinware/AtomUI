using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    public void SuccessAutoRemoveDelay_Removes_Successful_File_After_Delay()
    {
        var upload = CreateUploadWithFiles(out var files);
        upload.SuccessAutoRemoveDelay = TimeSpan.FromMilliseconds(20);
        var item = CreateItem();
        files.Add(item);

        item.Status = FileUploadStatus.Success;

        WaitUntil(() => files.Count == 0);
    }

    [Fact]
    public void Status_Changing_Away_From_Success_Cancels_Auto_Remove()
    {
        var upload = CreateUploadWithFiles(out var files);
        upload.SuccessAutoRemoveDelay = TimeSpan.FromMilliseconds(20);
        var item = CreateItem();
        files.Add(item);

        item.Status = FileUploadStatus.Success;
        item.Status = FileUploadStatus.Pending;
        PumpDispatcherFor(TimeSpan.FromMilliseconds(60));

        files.Count.ShouldBe(1);
    }

    [Fact]
    public void Manual_Remove_Cancels_Pending_Auto_Remove()
    {
        var upload = CreateUploadWithFiles(out var files);
        upload.SuccessAutoRemoveDelay = TimeSpan.FromMilliseconds(50);
        var item = CreateItem();
        files.Add(item);

        item.Status = FileUploadStatus.Success;
        upload.RemoveFileAsync(item.Id, TestContext.Current.CancellationToken)
              .IsCompletedSuccessfully.ShouldBeTrue();
        PumpDispatcherFor(TimeSpan.FromMilliseconds(80));

        files.Count.ShouldBe(0);
    }

    [Fact]
    public void Replacing_Files_Cancels_Pending_Auto_Remove_For_Old_Items()
    {
        var upload = CreateUploadWithFiles(out var oldFiles);
        upload.SuccessAutoRemoveDelay = TimeSpan.FromMilliseconds(20);
        var item = CreateItem();
        oldFiles.Add(item);

        item.Status  = FileUploadStatus.Success;
        upload.Files = new ObservableCollection<UploadFileItem>();
        PumpDispatcherFor(TimeSpan.FromMilliseconds(60));

        GetPendingAutoRemoveCount(upload).ShouldBe(0);
    }

    [Fact]
    public void SetFormValue_Cancels_Pending_Auto_Remove_For_Replaced_Items()
    {
        var upload = CreateUploadWithFiles(out var files);
        upload.SuccessAutoRemoveDelay = TimeSpan.FromMilliseconds(20);
        var item = CreateItem();
        files.Add(item);

        item.Status = FileUploadStatus.Success;
        ((IFormItemAware)upload).SetFormValue(new[] { CreateItem() });
        PumpDispatcherFor(TimeSpan.FromMilliseconds(60));

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

    private static void WaitUntil(Func<bool> condition)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        while (!condition())
        {
            timeout.Token.ThrowIfCancellationRequested();
            Dispatcher.UIThread.RunJobs();
            Thread.Sleep(10);
        }
    }

    private static void PumpDispatcherFor(TimeSpan duration)
    {
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < duration)
        {
            Dispatcher.UIThread.RunJobs();
            Thread.Sleep(5);
        }

        Dispatcher.UIThread.RunJobs();
    }
}
