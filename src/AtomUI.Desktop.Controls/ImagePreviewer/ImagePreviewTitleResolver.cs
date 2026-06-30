namespace AtomUI.Desktop.Controls;

public interface IImagePreviewTitleResolver
{
    string? ResolveTitle(ImagePreviewTitleResolveContext context);
}

public readonly record struct ImagePreviewTitleResolveContext(
    ImageSourceUri SourceUri,
    int CurrentIndex,
    int Count);

public sealed class DefaultImagePreviewTitleResolver : IImagePreviewTitleResolver
{
    public static DefaultImagePreviewTitleResolver Instance { get; } = new();

    public string? ResolveTitle(ImagePreviewTitleResolveContext context)
    {
        var sourceUri = context.SourceUri;
        return sourceUri.Kind switch
        {
            ImageSourceUriKind.LocalFile        => ResolveLocalFileName(sourceUri),
            ImageSourceUriKind.Remote           => ResolveUriFileName(sourceUri.Uri),
            ImageSourceUriKind.AvaloniaResource => ResolveUriFileName(sourceUri.Uri),
            _                                   => null
        };
    }

    private static string? ResolveLocalFileName(ImageSourceUri sourceUri)
    {
        return NormalizeFileName(Path.GetFileName(sourceUri.LocalPath));
    }

    private static string? ResolveUriFileName(Uri? uri)
    {
        if (uri is null)
        {
            return null;
        }

        var path = uri.AbsolutePath;
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        var segment = path.Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        return NormalizeFileName(segment is null ? null : Uri.UnescapeDataString(segment));
    }

    private static string? NormalizeFileName(string? fileName)
    {
        return string.IsNullOrWhiteSpace(fileName) ? null : fileName;
    }
}
