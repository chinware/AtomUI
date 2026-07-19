namespace AtomUI.Theme;

public sealed class ThemeChangeFailedEventArgs : EventArgs
{
    public ThemeChangeFailedEventArgs(
        ThemeRequest request,
        IReadOnlyList<ThemeDiagnostic> diagnostics,
        Exception? exception)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(diagnostics);

        Request     = request;
        Diagnostics = Array.AsReadOnly(diagnostics.ToArray());
        Exception   = exception;
    }

    public ThemeRequest Request { get; }
    public IReadOnlyList<ThemeDiagnostic> Diagnostics { get; }
    public Exception? Exception { get; }
}
