namespace AtomUI.Controls;

internal static class ImageProgressDispatcher
{
    internal static void Report(
        IProgress<ImageLoadProgress>? progress,
        ImageLoadProgress value)
    {
        if (progress is null)
        {
            return;
        }
        try
        {
            progress.Report(value);
        }
        catch (Exception exception) when (ImageLoadEventDispatcher.IsNonFatal(exception))
        {
            // Progress observers are advisory and must not fail the image request.
        }
    }
}
