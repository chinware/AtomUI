namespace AtomUI.Desktop.Controls;

public interface IImagePreviewSource
{
    string? DisplayName { get; }

    string? ContentType { get; }

    ValueTask<Stream> OpenReadAsync(CancellationToken cancellationToken);
}

public interface IImagePreviewSourceIdentity
{
    object Identity { get; }
}
