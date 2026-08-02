using Avalonia.Logging;

namespace AtomUI.Theme;

internal static class ThemeRuntimeLogger
{
    internal static void LogPublishFailure(
        object source,
        string boundary,
        Exception exception)
    {
        var logger = Logger.TryGet(LogEventLevel.Warning, AtomUILogArea.Theme);
        logger?.Log(
            source,
            $"Theme publish boundary '{boundary}' failed: {exception}");
    }
}

internal static class ThemePublishBoundary
{
    internal static void Dispatch(
        Action action,
        object source,
        List<ThemeDiagnostic> diagnostics,
        string boundary)
    {
        try
        {
            action();
        }
        catch (Exception exception)
        {
            ThemeRuntimeLogger.LogPublishFailure(source, boundary, exception);
            diagnostics.Add(new ThemeDiagnostic(
                "ATMTHM7003",
                ThemeDiagnosticSeverity.Warning,
                boundary,
                "$",
                $"Theme publish boundary failed: {exception.GetBaseException().Message}"));
        }
    }
}
