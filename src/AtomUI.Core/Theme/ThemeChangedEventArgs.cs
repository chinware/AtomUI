namespace AtomUI.Theme;

public sealed class ThemeChangedEventArgs : EventArgs
{
    public ThemeChangedEventArgs(
        ThemeRequest request,
        ThemeState state,
        IReadOnlyList<ThemeDiagnostic> publishDiagnostics)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(publishDiagnostics);

        Request            = request;
        State              = state;
        PublishDiagnostics = Array.AsReadOnly(publishDiagnostics.ToArray());
    }

    internal ThemeChangedEventArgs(
        ThemeRequest request,
        ThemeState state,
        List<ThemeDiagnostic> publishDiagnostics)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(publishDiagnostics);

        Request            = request;
        State              = state;
        PublishDiagnostics = publishDiagnostics.AsReadOnly();
    }

    public ThemeRequest Request { get; }
    public ThemeState State { get; }
    public IReadOnlyList<ThemeDiagnostic> PublishDiagnostics { get; }
}
