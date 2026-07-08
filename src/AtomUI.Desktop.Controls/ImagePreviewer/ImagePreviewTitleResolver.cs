namespace AtomUI.Desktop.Controls;

public interface IImagePreviewTitleResolver
{
    string? ResolveTitle(ImagePreviewTitleResolveContext context);
}

public readonly record struct ImagePreviewTitleResolveContext(
    IImagePreviewSource Source,
    int CurrentIndex,
    int Count);

public sealed class DefaultImagePreviewTitleResolver : IImagePreviewTitleResolver
{
    public static DefaultImagePreviewTitleResolver Instance { get; } = new();

    public string? ResolveTitle(ImagePreviewTitleResolveContext context)
    {
        return context.Source is UriImagePreviewSource uriSource
            ? ResolveUriSourceTitle(uriSource.SourceUri)
            : ResolveDisplayNameSourceTitle(context.Source);
    }

    private static string? ResolveUriSourceTitle(ImageSourceUri sourceUri)
    {
        return sourceUri.Kind switch
        {
            ImageSourceUriKind.LocalFile        => ResolveLocalFileName(sourceUri),
            ImageSourceUriKind.Remote           => ResolveUriFileName(sourceUri.Uri),
            ImageSourceUriKind.AvaloniaResource => ResolveUriFileName(sourceUri.Uri),
            _                                   => null
        };
    }

    private static string? ResolveDisplayNameSourceTitle(IImagePreviewSource source)
    {
        return NormalizeFileName(source.DisplayName);
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

        var segments = uri.AbsolutePath
                          .Split('/', StringSplitOptions.RemoveEmptyEntries)
                          .Select(Uri.UnescapeDataString)
                          .ToArray();
        if (segments.Length == 0)
        {
            return null;
        }

        return NormalizeFileName(ResolveMeaningfulUriSegment(segments));
    }

    private static string? ResolveMeaningfulUriSegment(string[] segments)
    {
        if (HasTrailingDimensionPair(segments))
        {
            return segments[^3];
        }

        return segments[^1];
    }

    private static bool HasTrailingDimensionPair(string[] segments)
    {
        return segments.Length >= 3 &&
               IsPositiveIntegerSegment(segments[^1]) &&
               IsPositiveIntegerSegment(segments[^2]);
    }

    private static bool IsPositiveIntegerSegment(string segment)
    {
        if (string.IsNullOrWhiteSpace(segment))
        {
            return false;
        }

        foreach (var character in segment)
        {
            if (!char.IsDigit(character))
            {
                return false;
            }
        }

        return true;
    }

    private static string? NormalizeFileName(string? fileName)
    {
        return string.IsNullOrWhiteSpace(fileName) ? null : fileName;
    }
}
