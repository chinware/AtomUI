using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunTimelineStateVerification()
    {
        var failures = new List<string>();
        VerifyTimelinePendingLifecycle(failures);
        VerifyTimelinePendingIcon(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Timeline state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Timeline state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyTimelinePendingLifecycle(ICollection<string> failures)
    {
        var timeline = new Timeline
        {
            Pending = "Recording..."
        };
        timeline.Items.Add(new TimelineItem { Content = "First" });
        timeline.Items.Add(new TimelineItem { Content = "Second" });

        using var realized = RealizeControl(timeline);
        Expect(GetPendingTimelineItems(timeline).Count == 1,
            $"Timeline should create one pending item, actual {GetPendingTimelineItems(timeline).Count}.",
            failures);
        Expect(timeline.Items.Count == 3,
            $"Timeline should include two user items plus one pending item, actual {timeline.Items.Count}.",
            failures);

        timeline.Pending = null;
        RefreshLayout(realized.Window);
        Expect(GetPendingTimelineItems(timeline).Count == 0,
            $"Timeline should remove pending item when Pending is cleared, actual {GetPendingTimelineItems(timeline).Count}.",
            failures);
        Expect(timeline.Items.Count == 2,
            $"Timeline should restore user item count after Pending is cleared, actual {timeline.Items.Count}.",
            failures);

        timeline.Pending = "Again";
        RefreshLayout(realized.Window);
        var pendingItems = GetPendingTimelineItems(timeline);
        Expect(pendingItems.Count == 1,
            $"Timeline should recreate one pending item when Pending is set again, actual {pendingItems.Count}.",
            failures);
        Expect(Equals(pendingItems.FirstOrDefault()?.Content, "Again"),
            $"Timeline recreated pending item should use latest Pending content, actual {pendingItems.FirstOrDefault()?.Content ?? "<null>"}.",
            failures);
    }

    private static void VerifyTimelinePendingIcon(ICollection<string> failures)
    {
        var timeline = new Timeline
        {
            Pending = "Recording..."
        };
        timeline.Items.Add(new TimelineItem { Content = "First" });

        using var realized = RealizeControl(timeline);
        var pending = GetPendingTimelineItems(timeline).SingleOrDefault();
        Expect(pending?.IndicatorIcon is LoadingOutlined,
            $"Timeline default pending item should use LoadingOutlined, actual {pending?.IndicatorIcon?.GetType().Name ?? "<null>"}.",
            failures);

        var customIcon = new ClockCircleOutlined();
        timeline.PendingIcon = customIcon;
        RefreshLayout(realized.Window);

        var pendingItems = GetPendingTimelineItems(timeline);
        pending = pendingItems.SingleOrDefault();
        Expect(pendingItems.Count == 1,
            $"Timeline should keep one pending item after PendingIcon changes, actual {pendingItems.Count}.",
            failures);
        Expect(ReferenceEquals(pending?.IndicatorIcon, customIcon),
            $"Timeline pending item should use custom PendingIcon, actual {pending?.IndicatorIcon?.GetType().Name ?? "<null>"}.",
            failures);
    }

    private static IReadOnlyList<TimelineItem> GetPendingTimelineItems(Timeline timeline)
    {
        return timeline.Items
                       .OfType<TimelineItem>()
                       .Where(IsPendingTimelineItem)
                       .ToList();
    }

    private static bool IsPendingTimelineItem(TimelineItem item)
    {
        return GetPrivateField(item, "AtomUI.Controls.Commons.AbstractTimelineItem", "_isPending") is true;
    }
}
