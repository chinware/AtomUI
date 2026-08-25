namespace AtomUI.Controls;

internal sealed record SvgContentMetadata(
    int ElementCount,
    int AttributeCount,
    int MaxElementDepth,
    long PathDataCharacters,
    int EmbeddedImageCount,
    long EmbeddedImageBytes,
    long EmbeddedImageDecodedBytes,
    int MaxReferenceDepth,
    double? IntrinsicWidth,
    double? IntrinsicHeight,
    double? ViewBoxWidth,
    double? ViewBoxHeight,
    long EstimatedDecodedCost);
