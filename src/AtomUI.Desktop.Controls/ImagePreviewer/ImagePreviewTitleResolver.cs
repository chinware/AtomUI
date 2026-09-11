namespace AtomUI.Desktop.Controls;

public interface IImagePreviewTitleResolver
{
    string? ResolveTitle(in ImagePreviewTitleResolveContext context);
}

public readonly record struct ImagePreviewTitleResolveContext(
    ImagePreviewItem Item,
    int CurrentIndex,
    int Count);

public sealed class DefaultImagePreviewTitleResolver : IImagePreviewTitleResolver
{
    public static DefaultImagePreviewTitleResolver Instance { get; } = new();

    public string? ResolveTitle(in ImagePreviewTitleResolveContext context)
    {
        return string.IsNullOrWhiteSpace(context.Item.Source.DisplayName)
            ? null
            : context.Item.Source.DisplayName;
    }
}
