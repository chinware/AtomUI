namespace AtomUI.Controls;

public sealed class UploadFileInfo
{
    public string Name { get; }
    public Uri? Path { get; }
    public long? Size { get; }
    public string? ContentType { get; }
    public DateTimeOffset? DateCreated { get; }
    public DateTimeOffset? DateModified { get; }
    public IUploadFileSource Source { get; }

    public UploadFileInfo(
        string name,
        IUploadFileSource source,
        Uri? path = null,
        long? size = null,
        string? contentType = null,
        DateTimeOffset? dateCreated = null,
        DateTimeOffset? dateModified = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(source);

        Name         = name;
        Source       = source;
        Path         = path;
        Size         = size;
        ContentType  = contentType;
        DateCreated  = dateCreated;
        DateModified = dateModified;
    }
}
