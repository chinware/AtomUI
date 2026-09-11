using System.Collections.ObjectModel;
using Avalonia.Platform.Storage;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Upload;

public class UploadFileSelectionTests
{
    static UploadFileSelectionTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public async Task File_Picker_Receives_The_Exact_AllowedFileTypes_And_Uses_The_FilePicker_Source()
    {
        var png = new FilePickerFileType("PNG") { Patterns = ["*.png"] };
        var adapter = new TestStorageProviderAdapter
        {
            Files = [new TestStorageFile("image.png", "file:///image.png")]
        };
        var upload = CreateUpload(adapter);
        upload.AllowedFileTypes = [png];
        UploadInputBatchCompletedEventArgs? completed = null;
        upload.InputBatchCompleted += (_, args) => completed = args;

        await upload.SelectFilesAsync(TestContext.Current.CancellationToken);

        adapter.LastFileOptions.ShouldNotBeNull();
        adapter.LastFileOptions.FileTypeFilter.ShouldBeSameAs(upload.AllowedFileTypes);
        completed.ShouldNotBeNull();
        completed.Source.ShouldBe(UploadInputSource.FilePicker);
        upload.Files!.Select(item => item.Name).ShouldBe(["image.png"]);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Null_Or_Empty_AllowedFileTypes_Does_Not_Create_A_Synthetic_Filter(bool useEmptyList)
    {
        var adapter = new TestStorageProviderAdapter();
        var upload = CreateUpload(adapter);
        upload.AllowedFileTypes = useEmptyList ? [] : null;

        await upload.SelectFilesAsync(TestContext.Current.CancellationToken);

        adapter.LastFileOptions.ShouldNotBeNull();
        adapter.LastFileOptions.FileTypeFilter.ShouldBeSameAs(upload.AllowedFileTypes);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task File_And_Directory_Pickers_Use_The_Same_Multiple_Input_Option(bool isMultipleEnabled)
    {
        var adapter = new TestStorageProviderAdapter();
        var upload = CreateUpload(adapter);
        upload.IsMultipleEnabled = isMultipleEnabled;

        await upload.SelectFilesAsync(TestContext.Current.CancellationToken);
        await upload.SelectDirectoriesAsync(TestContext.Current.CancellationToken);

        adapter.LastFileOptions.ShouldNotBeNull();
        adapter.LastFileOptions.AllowMultiple.ShouldBe(isMultipleEnabled);
        adapter.LastFolderOptions.ShouldNotBeNull();
        adapter.LastFolderOptions.AllowMultiple.ShouldBe(isMultipleEnabled);
    }

    [Fact]
    public async Task Directory_Picker_Uses_TopLevelFiles_And_Preserves_Per_Item_Results()
    {
        var firstTopLevelFile = new TestStorageFile("first.txt", "file:///folder/first.txt");
        var secondTopLevelFile = new TestStorageFile("second.txt", "file:///folder/second.txt");
        var nestedFile = new TestStorageFile("nested.txt", "file:///folder/nested/nested.txt");
        var nestedFolder = new TestStorageFolder("nested", "file:///folder/nested/", nestedFile);
        var root = new TestStorageFolder(
            "folder",
            "file:///folder/",
            firstTopLevelFile,
            secondTopLevelFile,
            nestedFolder);
        var adapter = new TestStorageProviderAdapter { Folders = [root] };
        var upload = CreateUpload(adapter);
        UploadInputBatchCompletedEventArgs? completed = null;
        upload.InputBatchCompleted += (_, args) => completed = args;

        await upload.SelectDirectoriesAsync(TestContext.Current.CancellationToken);

        upload.Files!.Select(item => item.Name).ShouldBe(["first.txt", "second.txt"]);
        completed.ShouldNotBeNull();
        completed.Source.ShouldBe(UploadInputSource.DirectoryPicker);
        completed.RejectedItems.Single().Reason.ShouldBe(UploadRejectionReason.DirectoryNotAllowed);
        nestedFolder.DisposeCount.ShouldBe(1);
        nestedFile.DisposeCount.ShouldBe(0);
    }

    [Fact]
    public async Task Empty_Picker_Result_Completes_One_Empty_Batch()
    {
        var adapter = new TestStorageProviderAdapter();
        var upload = CreateUpload(adapter);
        var events = new List<UploadInputBatchCompletedEventArgs>();
        upload.InputBatchCompleted += (_, args) => events.Add(args);

        await upload.SelectFilesAsync(TestContext.Current.CancellationToken);

        events.Count.ShouldBe(1);
        events[0].Source.ShouldBe(UploadInputSource.FilePicker);
        events[0].AcceptedFiles.ShouldBeEmpty();
        events[0].RejectedItems.ShouldBeEmpty();
    }

    [Fact]
    public async Task Unavailable_Picker_Does_Not_Start_An_Input_Batch()
    {
        var adapter = new TestStorageProviderAdapter { CanOpenFiles = false };
        var upload = CreateUpload(adapter);
        var eventCount = 0;
        upload.InputBatchCompleted += (_, _) => eventCount++;

        await upload.SelectFilesAsync(TestContext.Current.CancellationToken);

        adapter.LastFileOptions.ShouldBeNull();
        eventCount.ShouldBe(0);
    }

    private static Desktop.Controls.Upload CreateUpload(TestStorageProviderAdapter adapter)
    {
        return new Desktop.Controls.Upload
        {
            AutoUpload = false,
            Files = new ObservableCollection<UploadFileItem>(),
            StorageProviderAdapter = adapter
        };
    }

    private sealed class TestStorageProviderAdapter : IUploadStorageProviderAdapter
    {
        public bool CanOpenFiles { get; init; } = true;
        public bool CanOpenFolders { get; init; } = true;
        internal IReadOnlyList<IStorageFile> Files { get; init; } = [];
        internal IReadOnlyList<IStorageFolder> Folders { get; init; } = [];
        internal FilePickerOpenOptions? LastFileOptions { get; private set; }
        internal FolderPickerOpenOptions? LastFolderOptions { get; private set; }

        public Task<IReadOnlyList<IStorageFile>> OpenFilesAsync(
            FilePickerOpenOptions options,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            LastFileOptions = options;
            return Task.FromResult(Files);
        }

        public Task<IReadOnlyList<IStorageFolder>> OpenFoldersAsync(
            FolderPickerOpenOptions options,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            LastFolderOptions = options;
            return Task.FromResult(Folders);
        }
    }
}
