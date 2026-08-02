namespace AtomUI.Theme;

internal static class ThemeEventDispatcher
{
    private const string ObserverFailureCode = "ATMTHM7001";

    internal static void Dispatch<TEventArgs>(
        EventHandler<TEventArgs>? handlers,
        object sender,
        TEventArgs args,
        List<ThemeDiagnostic> publishDiagnostics,
        string source)
        where TEventArgs : EventArgs
    {
        if (handlers is null)
        {
            return;
        }

        foreach (var invocation in handlers.GetInvocationList())
        {
            if (invocation is not EventHandler<TEventArgs> handler)
            {
                continue;
            }

            try
            {
                handler(sender, args);
            }
            catch (Exception exception)
            {
                AddObserverFailure(publishDiagnostics, sender, source, exception);
            }
        }
    }

    internal static void Dispatch(
        EventHandler? handlers,
        object sender,
        EventArgs args,
        List<ThemeDiagnostic> publishDiagnostics,
        string source)
    {
        if (handlers is null)
        {
            return;
        }

        foreach (var invocation in handlers.GetInvocationList())
        {
            if (invocation is not EventHandler handler)
            {
                continue;
            }

            try
            {
                handler(sender, args);
            }
            catch (Exception exception)
            {
                AddObserverFailure(publishDiagnostics, sender, source, exception);
            }
        }
    }

    private static void AddObserverFailure(
        List<ThemeDiagnostic> publishDiagnostics,
        object sender,
        string source,
        Exception exception)
    {
        ThemeRuntimeLogger.LogPublishFailure(sender, source, exception);
        publishDiagnostics.Add(new ThemeDiagnostic(
            ObserverFailureCode,
            ThemeDiagnosticSeverity.Warning,
            source,
            "$",
            $"Theme observer failed: {exception.GetBaseException().Message}"));
    }
}
