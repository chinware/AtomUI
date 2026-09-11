using AtomUI.Theme.Schema;

namespace AtomUI.Desktop.Controls;

internal static class DesktopControlRegistrationSelector
{
    private static readonly HashSet<string> s_browserExcludedControlIds = new(StringComparer.Ordinal)
    {
        "AdornerLayer",
        "OtpLineEdit",
        "OtpLineEditCell",
        "SplitView",
        "TreeViewFlyoutPresenter",
        "Window",
        "WindowTitleBar"
    };

    internal static bool IsBrowserControlSupported(ControlTokenIdentity identity)
    {
        return !s_browserExcludedControlIds.Contains(identity.Id);
    }
}
