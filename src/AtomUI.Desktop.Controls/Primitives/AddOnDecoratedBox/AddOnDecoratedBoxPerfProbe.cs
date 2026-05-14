using System.Threading;

namespace AtomUI.Desktop.Controls;

internal static class AddOnDecoratedBoxPerfProbe
{
    private static long _updateIconStatusColorsCalls;
    private static long _applyIconBrushCalls;
    private static long _applyIconBrushScannedVisuals;
    private static long _applyIconBrushMatchedIcons;

    public static bool IsEnabled { get; set; }

    public static void Reset()
    {
        Interlocked.Exchange(ref _updateIconStatusColorsCalls, 0);
        Interlocked.Exchange(ref _applyIconBrushCalls, 0);
        Interlocked.Exchange(ref _applyIconBrushScannedVisuals, 0);
        Interlocked.Exchange(ref _applyIconBrushMatchedIcons, 0);
    }

    public static AddOnDecoratedBoxPerfSnapshot Snapshot()
    {
        return new AddOnDecoratedBoxPerfSnapshot(
            Interlocked.Read(ref _updateIconStatusColorsCalls),
            Interlocked.Read(ref _applyIconBrushCalls),
            Interlocked.Read(ref _applyIconBrushScannedVisuals),
            Interlocked.Read(ref _applyIconBrushMatchedIcons));
    }

    internal static void RecordUpdateIconStatusColors()
    {
        if (!IsEnabled)
        {
            return;
        }

        Interlocked.Increment(ref _updateIconStatusColorsCalls);
    }

    internal static void RecordApplyIconBrush(int scannedVisuals, int matchedIcons)
    {
        if (!IsEnabled)
        {
            return;
        }

        Interlocked.Increment(ref _applyIconBrushCalls);
        Interlocked.Add(ref _applyIconBrushScannedVisuals, scannedVisuals);
        Interlocked.Add(ref _applyIconBrushMatchedIcons, matchedIcons);
    }
}

internal readonly record struct AddOnDecoratedBoxPerfSnapshot(
    long UpdateIconStatusColorsCalls,
    long ApplyIconBrushCalls,
    long ApplyIconBrushScannedVisuals,
    long ApplyIconBrushMatchedIcons);
