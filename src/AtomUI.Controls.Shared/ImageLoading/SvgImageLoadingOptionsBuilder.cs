namespace AtomUI.Controls;

public sealed class SvgImageLoadingOptionsBuilder
{
    public SvgConformanceMode ConformanceMode { get; set; } = SvgConformanceMode.Compatible;

    public long MaxDocumentBytes { get; set; } = 4L * 1024 * 1024;

    public long MaxXmlCharacters { get; set; } = 8_000_000;

    public int MaxElementCount { get; set; } = 20_000;

    public int MaxAttributeCount { get; set; } = 100_000;

    public int MaxElementDepth { get; set; } = 256;

    public int MaxReferenceDepth { get; set; } = 64;

    public long MaxPathDataCharacters { get; set; } = 2_000_000;

    public int MaxEmbeddedImageCount { get; set; } = 16;

    public long MaxEmbeddedImageBytes { get; set; } = 8L * 1024 * 1024;

    internal SvgImageLoadingOptions Build()
    {
        if (!Enum.IsDefined(ConformanceMode))
        {
            throw new InvalidOperationException($"{nameof(ConformanceMode)} must be a defined value.");
        }
        ValidatePositive(MaxDocumentBytes, nameof(MaxDocumentBytes));
        ValidatePositive(MaxXmlCharacters, nameof(MaxXmlCharacters));
        ValidatePositive(MaxElementCount, nameof(MaxElementCount));
        ValidatePositive(MaxAttributeCount, nameof(MaxAttributeCount));
        ValidatePositive(MaxElementDepth, nameof(MaxElementDepth));
        ValidatePositive(MaxReferenceDepth, nameof(MaxReferenceDepth));
        ValidatePositive(MaxPathDataCharacters, nameof(MaxPathDataCharacters));
        ValidatePositive(MaxEmbeddedImageCount, nameof(MaxEmbeddedImageCount));
        ValidatePositive(MaxEmbeddedImageBytes, nameof(MaxEmbeddedImageBytes));

        return new SvgImageLoadingOptions(
            ConformanceMode,
            MaxDocumentBytes,
            MaxXmlCharacters,
            MaxElementCount,
            MaxAttributeCount,
            MaxElementDepth,
            MaxReferenceDepth,
            MaxPathDataCharacters,
            MaxEmbeddedImageCount,
            MaxEmbeddedImageBytes);
    }

    private static void ValidatePositive(long value, string name)
    {
        if (value <= 0)
        {
            throw new InvalidOperationException($"{name} must be greater than zero.");
        }
    }
}
