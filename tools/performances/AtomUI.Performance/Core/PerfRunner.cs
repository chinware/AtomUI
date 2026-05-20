using System.Diagnostics;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static readonly Size MeasureSize = new(1280, 4096);
    private static readonly Rect ArrangeRect = new(0, 0, 1280, 4096);

    private static void RunWarmup(PerfScenario scenario, int count)
        {
            if (count <= 0)
            {
                return;
            }

            using var _ = RealizeScenario(scenario, count);
        }

        private static PerfResult MeasureScenario(PerfScenario scenario, int count)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            AddOnDecoratedBoxPerfProbe.Reset();
            var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
            var stopwatch       = Stopwatch.StartNew();

            using var realized = RealizeScenario(scenario, count);

            stopwatch.Stop();
            var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
            var probeSnapshot  = AddOnDecoratedBoxPerfProbe.Snapshot();
            var treeStats      = TreeStats.Collect(realized.RootControls);

            return new PerfResult(
                scenario.Name,
                count,
                stopwatch.Elapsed,
                allocatedBytes,
                treeStats,
                probeSnapshot);
        }

        private static RealizedScenario RealizeScenario(PerfScenario scenario, int count)
        {
            var rootControls = new List<Control>(count);
            var panel        = new StackPanel
            {
                Spacing = 8
            };

            for (var i = 0; i < count; i++)
            {
                var control = scenario.Create(i);
                rootControls.Add(control);
                panel.Children.Add(control);
            }

            var window = new Avalonia.Controls.Window
            {
                Width       = MeasureSize.Width,
                Height      = 900,
                Content     = panel,
                ShowInTaskbar = false
            };

            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.Measure(MeasureSize);
            window.Arrange(ArrangeRect);
            window.UpdateLayout();
            Dispatcher.UIThread.RunJobs();

            return new RealizedScenario(window, rootControls);
        }

        private static RealizedScenario RealizeControl(Control control)
        {
            return RealizeScenario(new PerfScenario("Accessory.Verify", _ => control), 1);
        }

        private static void RefreshLayout(Avalonia.Controls.Window window)
        {
            Dispatcher.UIThread.RunJobs();
            window.Measure(MeasureSize);
            window.Arrange(ArrangeRect);
            window.UpdateLayout();
            Dispatcher.UIThread.RunJobs();
        }
}
