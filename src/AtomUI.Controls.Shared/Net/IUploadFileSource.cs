namespace AtomUI.Controls;

public interface IUploadFileSource
{
    ValueTask<Stream> OpenReadAsync(CancellationToken cancellationToken = default);
}
