using Avalonia.Controls;

namespace AtomUI.Native;

internal interface INativeWindowSizingHook : IDisposable
{
    event EventHandler? UserResizeCompleted;

    bool IsUserResizeInProgress { get; }

    void BeginUserResize();

    void CompleteUserResize();

    void CancelUserResize();
}

internal static class NativeWindowSizing
{
    public static INativeWindowSizingHook? TryAttachCsdSizingHook(Window window, Func<bool> isEnabled)
    {
        return OperatingSystem.IsWindows()
            ? new WindowsCsdSizingHook(window, isEnabled)
            : null;
    }
}
