using System.Runtime.CompilerServices;
using Avalonia.Platform.Storage;

namespace AtomUI.Desktop.Controls.Tests.Upload;

internal abstract class TestStorageItem : IStorageItem
{
    public string Name { get; }
    public Uri Path { get; }
    public int DisposeCount { get; private set; }
    public bool CanBookmark => false;

    protected TestStorageItem(string name, string path)
    {
        Name = name;
        Path = new Uri(path, UriKind.Absolute);
    }

    public virtual Task<StorageItemProperties> GetBasicPropertiesAsync()
    {
        return Task.FromResult(new StorageItemProperties());
    }

    public Task<string?> SaveBookmarkAsync() => Task.FromResult<string?>(null);

    public Task<IStorageFolder?> GetParentAsync() => Task.FromResult<IStorageFolder?>(null);

    public Task DeleteAsync() => Task.CompletedTask;

    public Task<IStorageItem?> MoveAsync(IStorageFolder destination) => Task.FromResult<IStorageItem?>(null);

    public void Dispose()
    {
        DisposeCount++;
    }
}

internal sealed class TestStorageFile : TestStorageItem, IStorageFile
{
    private readonly byte[] _content;
    private readonly StorageItemProperties _properties;

    public int OpenReadCount { get; private set; }
    public Exception? MetadataException { get; init; }
    public Exception? OpenReadException { get; init; }

    public TestStorageFile(
        string name,
        string path,
        byte[]? content = null,
        ulong? size = null,
        DateTimeOffset? dateCreated = null,
        DateTimeOffset? dateModified = null)
        : base(name, path)
    {
        _content = content ?? [1, 2, 3];
        _properties = new StorageItemProperties(size ?? (ulong)_content.Length, dateCreated, dateModified);
    }

    public override Task<StorageItemProperties> GetBasicPropertiesAsync()
    {
        return MetadataException is null
            ? Task.FromResult(_properties)
            : Task.FromException<StorageItemProperties>(MetadataException);
    }

    public Task<Stream> OpenReadAsync()
    {
        OpenReadCount++;
        return OpenReadException is null
            ? Task.FromResult<Stream>(new MemoryStream(_content, writable: false))
            : Task.FromException<Stream>(OpenReadException);
    }

    public Task<Stream> OpenWriteAsync()
    {
        return Task.FromResult<Stream>(new MemoryStream());
    }
}

internal sealed class ThrowingStorageFile : IStorageFile
{
    public string Name { get; }
    public Uri Path { get; }
    public bool CanBookmark => false;
    public int DisposeCount { get; private set; }

    internal ThrowingStorageFile(string name, string path)
    {
        Name = name;
        Path = new Uri(path, UriKind.Absolute);
    }

    public Task<StorageItemProperties> GetBasicPropertiesAsync() =>
        Task.FromResult(new StorageItemProperties(3));

    public Task<Stream> OpenReadAsync() =>
        Task.FromResult<Stream>(new MemoryStream([1, 2, 3], writable: false));

    public Task<Stream> OpenWriteAsync() =>
        Task.FromResult<Stream>(new MemoryStream());

    public Task<string?> SaveBookmarkAsync() => Task.FromResult<string?>(null);

    public Task<IStorageFolder?> GetParentAsync() => Task.FromResult<IStorageFolder?>(null);

    public Task DeleteAsync() => Task.CompletedTask;

    public Task<IStorageItem?> MoveAsync(IStorageFolder destination) =>
        Task.FromResult<IStorageItem?>(null);

    public void Dispose()
    {
        DisposeCount++;
        throw new InvalidOperationException($"Dispose failed for {Name}.");
    }
}

internal sealed class TestStorageFolder : TestStorageItem, IStorageFolder
{
    private readonly IReadOnlyList<IStorageItem> _items;

    public Exception? EnumerationException { get; init; }
    public TaskCompletionSource<bool>? EnumerationGate { get; init; }
    public TaskCompletionSource EnumerationStarted { get; } =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    public int? PauseAfterItemCount { get; init; }
    public TaskCompletionSource<bool>? EnumerationPauseGate { get; init; }
    public TaskCompletionSource EnumerationPaused { get; } =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public TestStorageFolder(string name, string path, params IStorageItem[] items)
        : base(name, path)
    {
        _items = items;
    }

    public IAsyncEnumerable<IStorageItem> GetItemsAsync() => EnumerateAsync();

    public Task<IStorageFolder?> GetFolderAsync(string name) => Task.FromResult<IStorageFolder?>(null);

    public Task<IStorageFile?> GetFileAsync(string name) => Task.FromResult<IStorageFile?>(null);

    public Task<IStorageFile?> CreateFileAsync(string name) => Task.FromResult<IStorageFile?>(null);

    public Task<IStorageFolder?> CreateFolderAsync(string name) => Task.FromResult<IStorageFolder?>(null);

    private async IAsyncEnumerable<IStorageItem> EnumerateAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        EnumerationStarted.TrySetResult();
        if (EnumerationGate is not null)
        {
            await EnumerationGate.Task.WaitAsync(cancellationToken);
        }

        if (EnumerationException is not null)
        {
            throw EnumerationException;
        }

        var yieldedCount = 0;
        foreach (var item in _items)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return item;
            yieldedCount++;
            if (PauseAfterItemCount == yieldedCount && EnumerationPauseGate is not null)
            {
                EnumerationPaused.TrySetResult();
                await EnumerationPauseGate.Task.WaitAsync(cancellationToken);
            }
        }
    }
}
