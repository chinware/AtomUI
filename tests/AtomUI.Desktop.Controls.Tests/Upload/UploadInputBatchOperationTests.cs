using AtomUI.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Upload;

public class UploadInputBatchOperationTests
{
    [Fact]
    public void Storage_Ownership_Rejects_Duplicate_Adopt_And_Invalid_Release()
    {
        using var operation = new UploadInputBatchOperation(UploadInputSource.DragDrop);
        var item = new TestStorageFile("file.txt", "file:///file.txt");

        operation.AdoptStorageItem(item);
        Should.Throw<InvalidOperationException>(() => operation.AdoptStorageItem(item));
        operation.ReleaseStorageItem(item);
        item.DisposeCount.ShouldBe(1);
        Should.Throw<InvalidOperationException>(() => operation.ReleaseStorageItem(item));
    }

    [Fact]
    public void Promote_Transfers_Storage_Ownership_To_A_Source_Lease()
    {
        using var operation = new UploadInputBatchOperation(UploadInputSource.DragDrop);
        var item = new TestStorageFile("file.txt", "file:///file.txt");
        operation.AdoptStorageItem(item);

        var source = operation.PromoteStorageFile(item);
        var file = new UploadFileInfo("file.txt", source, item.Path);

        operation.OwnsFileSource(file).ShouldBeTrue();
        operation.TransferFileSourceToUpload(file);
        operation.OwnsFileSource(file).ShouldBeFalse();
        Should.Throw<InvalidOperationException>(() => operation.TransferFileSourceToUpload(file));

        source.Dispose();
        item.DisposeCount.ShouldBe(1);
    }

    [Fact]
    public void Dispose_Attempts_All_Resources_And_Aggregates_Cleanup_Failures()
    {
        var first = new ThrowingStorageFile("first.txt", "file:///first.txt");
        var second = new ThrowingStorageFile("second.txt", "file:///second.txt");
        var operation = new UploadInputBatchOperation(UploadInputSource.DragDrop);
        operation.AdoptStorageItem(first);
        operation.AdoptStorageItem(second);

        var error = Should.Throw<AggregateException>(() => operation.Dispose());

        error.InnerExceptions.Count.ShouldBe(2);
        first.DisposeCount.ShouldBe(1);
        second.DisposeCount.ShouldBe(1);
        operation.Dispose();
    }
}
