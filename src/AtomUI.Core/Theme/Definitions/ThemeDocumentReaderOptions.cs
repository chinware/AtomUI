namespace AtomUI.Theme.Definitions;

internal sealed class ThemeDocumentReaderOptions
{
    internal const long DefaultMaxDocumentBytes = 4L * 1024 * 1024;
    internal const int DefaultMaxElements = 65536;

    public long MaxDocumentBytes { get; init; } = DefaultMaxDocumentBytes;
    public int MaxElements { get; init; } = DefaultMaxElements;
}
