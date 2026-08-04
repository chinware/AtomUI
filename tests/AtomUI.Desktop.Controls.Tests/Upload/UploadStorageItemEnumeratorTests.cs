using Avalonia.Platform.Storage;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Upload;

public class UploadStorageItemEnumeratorTests
{
    [Fact]
    public async Task Storage_Source_Opens_Content_And_Releases_The_Item_Exactly_Once()
    {
        var storageFile = File("one.txt");
        var source = new UploadStorageFileSource(storageFile);

        await using var stream = await source.OpenReadAsync(TestContext.Current.CancellationToken);
        stream.ReadByte().ShouldBe(1);
        storageFile.OpenReadCount.ShouldBe(1);

        source.Dispose();
        source.Dispose();

        storageFile.DisposeCount.ShouldBe(1);
        await Should.ThrowAsync<ObjectDisposedException>(async () =>
            await source.OpenReadAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Reject_Mode_Rejects_And_Disposes_A_Directory()
    {
        var folder = Folder("root", File("inside.txt"));

        var result = await UploadStorageItemEnumerator.EnumerateAsync(
            [folder], UploadDirectoryDropMode.Reject, 32, 10_000, TestContext.Current.CancellationToken);

        result.Candidates.ShouldBeEmpty();
        result.RejectedItems.Single().Reason.ShouldBe(UploadRejectionReason.DirectoryNotAllowed);
        folder.DisposeCount.ShouldBe(1);
    }

    [Fact]
    public async Task Top_Level_Mode_Preserves_File_Order_And_Rejects_Nested_Folders()
    {
        var first = File("first.txt");
        var nested = Folder("nested", File("nested.txt"));
        var second = File("second.txt");
        var root = Folder("root", first, nested, second);

        var result = await UploadStorageItemEnumerator.EnumerateAsync(
            [root], UploadDirectoryDropMode.TopLevelFiles, 32, 10_000, TestContext.Current.CancellationToken);

        result.Candidates.Select(candidate => candidate.Name).ShouldBe(["first.txt", "second.txt"]);
        result.RejectedItems.Single().Name.ShouldBe("nested");
        nested.DisposeCount.ShouldBe(1);
        root.DisposeCount.ShouldBe(1);

        DisposeCandidates(result);
        first.DisposeCount.ShouldBe(1);
        second.DisposeCount.ShouldBe(1);
    }

    [Fact]
    public async Task Recursive_Mode_Uses_Stable_Depth_First_Order()
    {
        var first = File("first.txt");
        var nestedFile = File("nested.txt");
        var deepFile = File("deep.txt");
        var deep = Folder("deep", deepFile);
        var nested = Folder("nested", nestedFile, deep);
        var last = File("last.txt");
        var root = Folder("root", first, nested, last);

        var result = await UploadStorageItemEnumerator.EnumerateAsync(
            [root], UploadDirectoryDropMode.RecursiveFiles, 32, 10_000, TestContext.Current.CancellationToken);

        result.Candidates.Select(candidate => candidate.Name)
            .ShouldBe(["first.txt", "nested.txt", "deep.txt", "last.txt"]);
        result.RejectedItems.ShouldBeEmpty();
        root.DisposeCount.ShouldBe(1);
        nested.DisposeCount.ShouldBe(1);
        deep.DisposeCount.ShouldBe(1);

        DisposeCandidates(result);
    }

    [Fact]
    public async Task Recursive_Mode_Rejects_A_Folder_Beyond_Max_Depth_Without_Stopping_Siblings()
    {
        var tooDeepFile = File("too-deep.txt");
        var tooDeep = Folder("too-deep", tooDeepFile);
        var child = Folder("child", tooDeep);
        var sibling = File("sibling.txt");
        var root = Folder("root", child, sibling);

        var result = await UploadStorageItemEnumerator.EnumerateAsync(
            [root], UploadDirectoryDropMode.RecursiveFiles, 1, 10_000, TestContext.Current.CancellationToken);

        result.Candidates.Select(candidate => candidate.Name).ShouldBe(["sibling.txt"]);
        result.RejectedItems.Single().Reason.ShouldBe(UploadRejectionReason.DirectoryDepthExceeded);
        tooDeep.DisposeCount.ShouldBe(1);
        tooDeepFile.DisposeCount.ShouldBe(0);

        DisposeCandidates(result);
    }

    [Fact]
    public async Task Enumeration_Limit_Stops_The_Current_Tree_And_Disposes_The_Observed_Overflow_Item()
    {
        var first = File("first.txt");
        var overflow = File("overflow.txt");
        var root = Folder("root", first, overflow);

        var result = await UploadStorageItemEnumerator.EnumerateAsync(
            [root], UploadDirectoryDropMode.RecursiveFiles, 32, 1, TestContext.Current.CancellationToken);

        result.Candidates.Select(candidate => candidate.Name).ShouldBe(["first.txt"]);
        result.RejectedItems.Single().Reason.ShouldBe(UploadRejectionReason.EnumerationLimitExceeded);
        overflow.DisposeCount.ShouldBe(1);

        DisposeCandidates(result);
    }

    [Fact]
    public async Task Directory_Access_Failure_Rejects_Only_That_Branch()
    {
        var denied = new TestStorageFolder("denied", "file:///tmp/denied")
        {
            EnumerationException = new UnauthorizedAccessException("denied")
        };
        var sibling = File("sibling.txt");
        var root = Folder("root", denied, sibling);

        var result = await UploadStorageItemEnumerator.EnumerateAsync(
            [root], UploadDirectoryDropMode.RecursiveFiles, 32, 10_000, TestContext.Current.CancellationToken);

        result.Candidates.Select(candidate => candidate.Name).ShouldBe(["sibling.txt"]);
        var rejection = result.RejectedItems.Single();
        rejection.Reason.ShouldBe(UploadRejectionReason.AccessDenied);
        rejection.Exception.ShouldBeOfType<UnauthorizedAccessException>();
        denied.DisposeCount.ShouldBe(1);

        DisposeCandidates(result);
    }

    [Fact]
    public async Task Duplicate_Directory_Path_Is_Rejected_As_A_Cycle()
    {
        var repeated = Folder("root-again");
        var root = new TestStorageFolder("root", "file:///tmp/root", repeated);
        repeated = new TestStorageFolder("root-again", "file:///tmp/root");
        root = new TestStorageFolder("root", "file:///tmp/root", repeated);

        var result = await UploadStorageItemEnumerator.EnumerateAsync(
            [root], UploadDirectoryDropMode.RecursiveFiles, 32, 10_000, TestContext.Current.CancellationToken);

        result.Candidates.ShouldBeEmpty();
        result.RejectedItems.Single().Reason.ShouldBe(UploadRejectionReason.DirectoryCycleDetected);
        repeated.DisposeCount.ShouldBe(1);
        root.DisposeCount.ShouldBe(1);
    }

    [Fact]
    public async Task Cancellation_Between_Top_Level_Items_Disposes_The_Unprocessed_Remainder()
    {
        var first = File("first.txt");
        var second = File("second.txt");
        using var cancellation = new CancellationTokenSource();
        var storageItems = new CancellingStorageItemList(cancellation, first, second);

        await Should.ThrowAsync<OperationCanceledException>(() =>
            UploadStorageItemEnumerator.EnumerateAsync(
                storageItems,
                UploadDirectoryDropMode.Reject,
                32,
                10_000,
                cancellation.Token));

        first.DisposeCount.ShouldBe(1);
        second.DisposeCount.ShouldBe(1);
    }

    [Fact]
    public async Task File_Info_Construction_Failure_Releases_The_Transferred_Storage_Source()
    {
        var storageFile = File(" ");
        var result = await UploadStorageItemEnumerator.EnumerateAsync(
            [storageFile],
            UploadDirectoryDropMode.Reject,
            32,
            10_000,
            TestContext.Current.CancellationToken);
        var candidate = result.Candidates.Single();

        await Should.ThrowAsync<ArgumentException>(async () =>
            await candidate.CreateFileInfoAsync(TestContext.Current.CancellationToken));
        candidate.Dispose();

        storageFile.DisposeCount.ShouldBe(1);
    }

    private static TestStorageFile File(string name)
    {
        return new TestStorageFile(name, $"file:///tmp/{name}");
    }

    private static TestStorageFolder Folder(string name, params IStorageItem[] items)
    {
        return new TestStorageFolder(name, $"file:///tmp/{name}", items);
    }

    private static void DisposeCandidates(UploadStorageEnumerationResult result)
    {
        foreach (var candidate in result.Candidates)
        {
            candidate.Dispose();
        }
    }

    private sealed class CancellingStorageItemList(
        CancellationTokenSource cancellation,
        params IStorageItem[] items) : IReadOnlyList<IStorageItem>
    {
        public int Count => items.Length;

        public IStorageItem this[int index]
        {
            get
            {
                if (index == 0)
                {
                    cancellation.Cancel();
                }
                return items[index];
            }
        }

        public IEnumerator<IStorageItem> GetEnumerator() =>
            ((IEnumerable<IStorageItem>)items).GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
