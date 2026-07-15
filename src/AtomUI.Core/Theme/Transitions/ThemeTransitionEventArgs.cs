using AtomUI.Theme.Compilation;

namespace AtomUI.Theme.Transitions;

internal sealed class ThemeTransitionEventArgs : EventArgs
{
    internal ThemeTransitionEventArgs(
        ThemeRequest request,
        ITheme? oldTheme,
        ITheme newTheme,
        ThemeSnapshot newSnapshot)
    {
        Request     = request;
        OldTheme    = oldTheme;
        NewTheme    = newTheme;
        NewSnapshot = newSnapshot;
    }

    public ThemeRequest Request { get; }
    public ITheme? OldTheme { get; }
    public ITheme NewTheme { get; }
    public ThemeSnapshot NewSnapshot { get; }
}
