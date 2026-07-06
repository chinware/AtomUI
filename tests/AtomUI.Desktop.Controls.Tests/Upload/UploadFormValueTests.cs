using AtomUI.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Upload;

public class UploadFormValueTests
{
    static UploadFormValueTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void GetFormValue_Returns_Successful_Files_By_Default()
    {
        var success = CreateItem("success.txt", FileUploadStatus.Success);
        var pending = CreateItem("pending.txt", FileUploadStatus.Pending);
        var upload = new Desktop.Controls.Upload
        {
            Files = new List<UploadFileItem> { success, pending }
        };

        var value = ((IFormItemAware)upload).GetFormValue();

        var files = Assert.IsAssignableFrom<UploadFileItem[]>(value);
        files.Length.ShouldBe(1);
        files[0].ShouldBeSameAs(success);
    }

    [Fact]
    public void GetFormValue_Returns_Results_When_Configured()
    {
        var result = FileUploadResult.SuccessResult(new Uri("https://example.com/a.txt"), 12, TimeSpan.FromMilliseconds(1));
        var success = CreateItem("success.txt", FileUploadStatus.Success);
        success.Result = result;
        var upload = new Desktop.Controls.Upload
        {
            FileValueMode = UploadFileValueMode.Results,
            Files         = new List<UploadFileItem> { success, CreateItem("pending.txt", FileUploadStatus.Pending) }
        };

        var value = ((IFormItemAware)upload).GetFormValue();

        var results = Assert.IsAssignableFrom<FileUploadResult?[]>(value);
        results.Length.ShouldBe(1);
        results[0].ShouldBeSameAs(result);
    }

    [Fact]
    public void SetFormValue_Updates_Bound_Collection_Without_Replacing_It()
    {
        var files = new List<UploadFileItem> { CreateItem("old.txt", FileUploadStatus.Success) };
        var upload = new Desktop.Controls.Upload
        {
            Files = files
        };
        var item = CreateItem("new.txt", FileUploadStatus.Pending);

        ((IFormItemAware)upload).SetFormValue(new[] { item });

        upload.Files.ShouldBeSameAs(files);
        files.Count.ShouldBe(1);
        files[0].ShouldBeSameAs(item);
    }

    [Fact]
    public async Task ClearFormValue_Clears_Bound_Collection()
    {
        var files = new List<UploadFileItem> { CreateItem("old.txt", FileUploadStatus.Success) };
        var upload = new Desktop.Controls.Upload
        {
            Files = files
        };

        ((IFormItemAware)upload).ClearFormValue();
        await WaitUntilAsync(() => files.Count == 0);

        upload.Files.ShouldBeSameAs(files);
    }

    private static UploadFileItem CreateItem(string name, FileUploadStatus status)
    {
        return new UploadFileItem
        {
            Name   = name,
            Path   = new Uri($"file:///tmp/{name}"),
            Size   = 12,
            Status = status
        };
    }

    private static Task WaitUntilAsync(Func<bool> condition)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        while (!condition())
        {
            timeout.Token.ThrowIfCancellationRequested();
            Avalonia.Threading.Dispatcher.UIThread.RunJobs();
            Thread.Sleep(10);
        }

        return Task.CompletedTask;
    }
}
