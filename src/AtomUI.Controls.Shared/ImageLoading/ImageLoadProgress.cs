namespace AtomUI.Controls;

public readonly record struct ImageLoadProgress(
    ImageLoadStage Stage,
    long BytesReceived,
    long? TotalBytes,
    double? Fraction)
{
    internal static ImageLoadProgress Create(
        ImageLoadStage stage,
        long bytesReceived = 0,
        long? totalBytes = null)
    {
        double? fraction = totalBytes > 0
            ? Math.Clamp((double)bytesReceived / totalBytes.Value, 0, 1)
            : null;
        return new ImageLoadProgress(stage, bytesReceived, totalBytes, fraction);
    }
}
