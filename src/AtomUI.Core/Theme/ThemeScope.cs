using AtomUI.Theme.Compilation;
using Avalonia;

namespace AtomUI.Theme;

internal static class ThemeScope
{
    public static readonly AttachedProperty<ThemeSnapshot?> SnapshotProperty =
        AvaloniaProperty.RegisterAttached<ThemeConfigProvider, StyledElement, ThemeSnapshot?>(
            "Snapshot",
            inherits: true);
}
