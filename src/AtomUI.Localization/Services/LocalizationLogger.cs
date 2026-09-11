using Avalonia.Logging;

namespace AtomUI.Localization;

internal static class LocalizationLogger
{
    private const string LogArea = "Localization";

    internal static void LogPublishFailure(
        object source,
        string boundary,
        Exception exception)
    {
        var logger = Logger.TryGet(LogEventLevel.Warning, LogArea);
        logger?.Log(
            source,
            $"Localization publish boundary '{boundary}' failed: {exception}");
    }
}
