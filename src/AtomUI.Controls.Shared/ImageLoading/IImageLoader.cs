namespace AtomUI.Controls;

public interface IImageLoader
{
    ValueTask<ImageLoadResult> LoadAsync(
        ImageLoadRequest request,
        CancellationToken cancellationToken = default);

    ValueTask ClearCacheAsync(
        ImageCacheClearRequest request,
        CancellationToken cancellationToken = default);

    ImageLoaderSnapshot Snapshot { get; }

    event EventHandler<ImageLoaderEventArgs>? LoadEvent;
}
