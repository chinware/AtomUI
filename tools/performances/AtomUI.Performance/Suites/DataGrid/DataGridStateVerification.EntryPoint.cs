using AtomUI.DataGridPerformanceSupport;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunDataGridStateVerification() => DataGridStateVerifier.Run();
}
