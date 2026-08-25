namespace AtomUI.Controls;

internal sealed record SvgImageLoadingOptions(
    long MaxDocumentBytes,
    long MaxXmlCharacters,
    int MaxElementCount,
    int MaxAttributeCount,
    int MaxElementDepth,
    int MaxReferenceDepth,
    long MaxPathDataCharacters,
    int MaxEmbeddedImageCount,
    long MaxEmbeddedImageBytes);
