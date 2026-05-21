namespace AtomUI.Performance;

// Compatibility shim: the original AddOnDecoratedBoxPerfProbe / Snapshot were removed during the
// Avalonia 12 migration of AddOnDecoratedBox. Until the AddOn perf suite is rewritten against the
// new accessory architecture, these no-op stubs keep the harness building so other suites
// (TextBlock, etc.) can run.
internal sealed record AddOnDecoratedBoxPerfSnapshot(
    int UpdateIconStatusColorsCalls = 0,
    int ApplyIconBrushCalls = 0,
    int ApplyIconBrushScannedVisuals = 0,
    int ApplyIconBrushMatchedIcons = 0)
{
    public static AddOnDecoratedBoxPerfSnapshot Empty { get; } = new();
}

internal static class AddOnDecoratedBoxPerfProbe
{
    public static bool IsEnabled { get; set; }

    public static void Reset()
    {
    }

    public static AddOnDecoratedBoxPerfSnapshot Snapshot() => AddOnDecoratedBoxPerfSnapshot.Empty;
}
