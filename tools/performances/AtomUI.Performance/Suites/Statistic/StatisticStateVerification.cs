using System.Reflection;
using AtomUI.Desktop.Controls;
using Avalonia.Threading;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunStatisticStateVerification()
    {
        var failures = new List<string>();
        VerifyStatisticGeneratedContent(failures);
        VerifyStatisticCountUpDataContextCleanup(failures);
        VerifyTimerStatisticLifecycle(failures);
        VerifyTimerStatisticCountdownFinish(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Statistic state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Statistic state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyStatisticGeneratedContent(ICollection<string> failures)
    {
        var statistic = new Statistic
        {
            Value = 112893
        };

        using var realized = RealizeControl(statistic);
        Expect(Equals(statistic.Content, "112,893"),
            $"Statistic generated Content should be 112,893, actual {statistic.Content ?? "<null>"}.",
            failures);

        statistic.Value = 42;
        RefreshLayout(realized.Window);
        Expect(Equals(statistic.Content, "42"),
            $"Statistic generated Content should update when Value changes, actual {statistic.Content ?? "<null>"}.",
            failures);

        statistic.Value = null;
        RefreshLayout(realized.Window);
        Expect(statistic.Content is null,
            $"Statistic generated Content should clear when Value is null, actual {statistic.Content}.",
            failures);
    }

    private static void VerifyStatisticCountUpDataContextCleanup(ICollection<string> failures)
    {
        var first = new StatisticCountUp
        {
            EndValue = 10
        };
        var second = new StatisticCountUp
        {
            EndValue = 20
        };
        var statistic = new Statistic
        {
            Value   = 112893,
            Content = first
        };

        using var realized = RealizeControl(statistic);
        Expect(ReferenceEquals(first.DataContext, statistic),
            "StatisticCountUp Content should receive the owning Statistic as DataContext.",
            failures);

        statistic.Content = second;
        RefreshLayout(realized.Window);
        Expect(first.DataContext is null,
            "Old StatisticCountUp DataContext should be cleared when Content is replaced.",
            failures);
        Expect(ReferenceEquals(second.DataContext, statistic),
            "New StatisticCountUp Content should receive the owning Statistic as DataContext.",
            failures);

        statistic.Value = 24;
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(statistic.Content, second),
            "Explicit Statistic.Content should not be overwritten by generated Value content.",
            failures);
    }

    private static void VerifyTimerStatisticLifecycle(ICollection<string> failures)
    {
        var timerStatistic = new TimerStatistic
        {
            Value = DateTime.Now.AddSeconds(30)
        };

        Expect(GetPrivateField(timerStatistic, "AtomUI.Desktop.Controls.TimerStatistic", "_timer") is null,
            "TimerStatistic should not create a DispatcherTimer before visual attachment.",
            failures);

        using (RealizeControl(timerStatistic))
        {
            Expect(GetPrivateField(timerStatistic, "AtomUI.Desktop.Controls.TimerStatistic", "_timer") is DispatcherTimer,
                "TimerStatistic should create a DispatcherTimer after visual attachment.",
                failures);
        }

        Expect(GetPrivateField(timerStatistic, "AtomUI.Desktop.Controls.TimerStatistic", "_timer") is null,
            "TimerStatistic should release the DispatcherTimer after visual detach.",
            failures);
    }

    private static void VerifyTimerStatisticCountdownFinish(ICollection<string> failures)
    {
        var timerStatistic = new TimerStatistic
        {
            Value           = DateTime.Now.AddMilliseconds(25),
            RefreshDuration = TimeSpan.FromMilliseconds(5)
        };
        var finishCount = 0;
        timerStatistic.CountdownFinished += (_, _) => finishCount++;

        using var realized = RealizeControl(timerStatistic);
        Thread.Sleep(35);
        InvokeTimerTick(timerStatistic);
        InvokeTimerTick(timerStatistic);
        RefreshLayout(realized.Window);

        Expect(finishCount == 1,
            $"TimerStatistic countdown should finish exactly once, actual {finishCount}.",
            failures);
        Expect(GetNonPublicProperty<TimeSpan>(timerStatistic, "RemainingTime") == TimeSpan.Zero,
            $"TimerStatistic RemainingTime should stop at zero, actual {GetNonPublicProperty<TimeSpan>(timerStatistic, "RemainingTime")}.",
            failures);
    }

    private static T? GetNonPublicProperty<T>(object target, string propertyName)
    {
        var property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
        return property is null ? default : (T?)property.GetValue(target);
    }

    private static void InvokeTimerTick(TimerStatistic timerStatistic)
    {
        timerStatistic.GetType()
                      .GetMethod("HandleTickElapsed", BindingFlags.Instance | BindingFlags.NonPublic)
                      ?.Invoke(timerStatistic, [timerStatistic, EventArgs.Empty]);
        Dispatcher.UIThread.RunJobs();
    }
}
