namespace AtomUI.Desktop.Controls;

public sealed class StreamImagePreviewSource : IImagePreviewSource, IImagePreviewSourceIdentity
{
    private readonly Func<CancellationToken, ValueTask<Stream>> _openReadAsync;
    private readonly object? _identity;

    public StreamImagePreviewSource(
        Func<CancellationToken, ValueTask<Stream>> openReadAsync,
        string? displayName = null,
        string? contentType = null,
        object? identity = null)
    {
        _openReadAsync = openReadAsync ?? throw new ArgumentNullException(nameof(openReadAsync));
        DisplayName    = displayName;
        ContentType    = contentType;
        _identity      = identity;
    }

    public object Identity => _identity ?? this;

    public string? DisplayName { get; }

    public string? ContentType { get; }

    public async ValueTask<Stream> OpenReadAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var stream = await _openReadAsync(cancellationToken).ConfigureAwait(false);
        if (cancellationToken.IsCancellationRequested)
        {
            if (stream is not null)
            {
                await stream.DisposeAsync().ConfigureAwait(false);
            }

            cancellationToken.ThrowIfCancellationRequested();
        }

        return stream ?? throw new InvalidOperationException("Image preview source returned a null stream.");
    }
}
