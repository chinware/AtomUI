using AtomUI.Controls;

namespace AtomUI.Desktop.Controls.Tests.Upload;

internal sealed class UploadTestFileSource : IUploadFileSource
{
    private readonly byte[] _content;

    internal UploadTestFileSource(byte[]? content = null)
    {
        _content = content ?? [1, 2, 3];
    }

    public ValueTask<Stream> OpenReadAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult<Stream>(new MemoryStream(_content, writable: false));
    }
}
