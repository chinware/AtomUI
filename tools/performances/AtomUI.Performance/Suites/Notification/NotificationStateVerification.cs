using AtomUI;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using System.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunNotificationStateVerification()
    {
        var failures = new List<string>();
        VerifyNotificationCardTypeState(failures);
        VerifyNotificationCardCustomIcon(failures);
        VerifyNotificationProgressBarLifecycle(failures);
        VerifyNotificationManagerPreTemplateShow(failures);
        VerifyNotificationManagerTimerLifecycle(failures);
        VerifyNotificationManagerMaxItemsConverges(failures);
        VerifyNotificationManagerDisposeWithoutTopLevel(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Notification state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Notification state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyNotificationCardTypeState(ICollection<string> failures)
    {
        var card = CreateNotificationCard(NotificationType.Information, isShowProgress: false);
        using var realized = RealizeControl(card);

        Expect(HasPseudoClass(card, StdPseudoClass.Information),
            "Information NotificationCard should set :information.",
            failures);
        Expect(!HasPseudoClass(card, StdPseudoClass.Error),
            "Information NotificationCard should not set :error.",
            failures);

        card.NotificationType = NotificationType.Error;
        RefreshLayout(realized.Window);
        Expect(!HasPseudoClass(card, StdPseudoClass.Information),
            "NotificationCard should clear :information when NotificationType changes.",
            failures);
        Expect(HasPseudoClass(card, StdPseudoClass.Error),
            "NotificationCard should set :error when NotificationType changes to Error.",
            failures);
    }

    private static void VerifyNotificationCardCustomIcon(ICollection<string> failures)
    {
        var customIcon = new GithubOutlined();
        var card = new NotificationCard(CreateWindowNotificationManager())
        {
            Icon               = customIcon,
            Title              = "Custom icon",
            Content            = "Custom icon content",
            NotificationType   = NotificationType.Information,
            IsMotionEnabled    = false
        };
        using var realized = RealizeControl(card);

        Expect(ReferenceEquals(customIcon, card.Icon),
            "NotificationCard should keep the caller-provided custom icon.",
            failures);

        card.NotificationType = NotificationType.Error;
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(customIcon, card.Icon),
            "NotificationCard should not replace a custom icon when NotificationType changes.",
            failures);
    }

    private static void VerifyNotificationProgressBarLifecycle(ICollection<string> failures)
    {
        var normalCard = CreateNotificationCard(NotificationType.Information, isShowProgress: false);
        normalCard.Expiration = TimeSpan.FromSeconds(5);
        using (RealizeControl(normalCard))
        {
            Expect(FindVisualByName<NotificationProgressBar>(normalCard, "ProgressBar") == null,
                "NotificationCard without progress should not create NotificationProgressBar.",
                failures);
        }

        var progressCard = CreateNotificationCard(NotificationType.Information, isShowProgress: true);
        progressCard.Expiration = TimeSpan.FromSeconds(5);
        using var realized = RealizeControl(progressCard);
        Expect(FindVisualByName<NotificationProgressBar>(progressCard, "ProgressBar") != null,
            "NotificationCard with progress should create NotificationProgressBar.",
            failures);

        progressCard.IsShowProgress = false;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<NotificationProgressBar>(progressCard, "ProgressBar") == null,
            "NotificationCard should remove NotificationProgressBar when progress is disabled.",
            failures);

        progressCard.IsShowProgress = true;
        progressCard.Expiration     = TimeSpan.FromSeconds(5);
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<NotificationProgressBar>(progressCard, "ProgressBar") != null,
            "NotificationCard should recreate NotificationProgressBar when progress is enabled again.",
            failures);

        progressCard.Expiration = null;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<NotificationProgressBar>(progressCard, "ProgressBar") == null,
            "NotificationCard should remove NotificationProgressBar when expiration is cleared.",
            failures);
    }

    private static void VerifyNotificationManagerPreTemplateShow(ICollection<string> failures)
    {
        var manager = CreateWindowNotificationManager();
        manager.IsMotionEnabled = false;

        manager.Show(new Notification(
            type: NotificationType.Success,
            title: "Immediate notification",
            content: "Immediate content",
            expiration: TimeSpan.Zero));

        using var realized = RealizeControl(manager);
        RefreshLayout(realized.Window);

        Expect(CountNotificationCards(manager) == 1,
            "WindowNotificationManager should preserve a notification shown before visual realization.",
            failures);
    }

    private static void VerifyNotificationManagerTimerLifecycle(ICollection<string> failures)
    {
        var manager = CreateWindowNotificationManager();
        manager.IsMotionEnabled = false;
        using var realized = RealizeControl(manager);

        manager.Show(new Notification(
            type: NotificationType.Information,
            title: "Never close",
            content: "No timer needed",
            expiration: TimeSpan.Zero));
        RefreshLayout(realized.Window);
        Expect(GetNotificationExpiredTimer(manager) is not { IsEnabled: true },
            "WindowNotificationManager should not run the expiration timer for never-close notifications.",
            failures);

        manager.Show(new Notification(
            type: NotificationType.Information,
            title: "Auto close",
            content: "Timer needed",
            expiration: TimeSpan.FromSeconds(30)));
        RefreshLayout(realized.Window);
        Expect(GetNotificationExpiredTimer(manager) is { IsEnabled: true },
            "WindowNotificationManager should start the expiration timer when an expiring notification exists.",
            failures);

        foreach (var card in manager.GetSelfAndVisualDescendants().OfType<NotificationCard>().ToList())
        {
            card.Close();
        }
        RefreshLayout(realized.Window);
        Expect(GetNotificationExpiredTimer(manager) is not { IsEnabled: true },
            "WindowNotificationManager should stop the expiration timer after expiring notifications close.",
            failures);
    }

    private static void VerifyNotificationManagerMaxItemsConverges(ICollection<string> failures)
    {
        var manager = CreateWindowNotificationManager();
        manager.IsMotionEnabled = false;
        manager.MaxItems        = 3;
        using var realized = RealizeControl(manager);

        for (var i = 0; i < 8; i++)
        {
            manager.Show(new Notification(
                type: NotificationType.Information,
                title: $"Notification {i}",
                content: "MaxItems content",
                expiration: TimeSpan.Zero));
        }

        RefreshLayout(realized.Window);
        RefreshLayout(realized.Window);
        Expect(CountNotificationCards(manager) == 3,
            $"WindowNotificationManager MaxItems should converge to 3 cards, actual {CountNotificationCards(manager)}.",
            failures);
    }

    private static void VerifyNotificationManagerDisposeWithoutTopLevel(ICollection<string> failures)
    {
        var pendingManager = CreateWindowNotificationManager();
        pendingManager.Show(new Notification(
            type: NotificationType.Information,
            title: "Pending notification",
            content: "Pending content",
            expiration: TimeSpan.Zero));
        pendingManager.Dispose();
        Expect(GetPendingNotificationCount(pendingManager) == 0,
            "WindowNotificationManager.Dispose should clear pending notifications when no TopLevel is installed.",
            failures);

        var manager = CreateWindowNotificationManager();
        manager.IsMotionEnabled = false;
        using var realized = RealizeControl(manager);

        manager.Show(new Notification(
            type: NotificationType.Success,
            title: "Dispose notification",
            content: "Dispose content",
            expiration: TimeSpan.FromSeconds(30)));
        RefreshLayout(realized.Window);
        Expect(GetNotificationExpiredTimer(manager) != null,
            "WindowNotificationManager should have an expiration timer before dispose.",
            failures);

        manager.Dispose();
        Expect(GetNotificationExpiredTimer(manager) == null,
            "WindowNotificationManager.Dispose should release the expiration timer even without a TopLevel.",
            failures);
        Expect(GetNotificationCleanupTimer(manager) == null,
            "WindowNotificationManager.Dispose should release the cleanup timer even without a TopLevel.",
            failures);
    }

    private static int CountNotificationCards(Control root)
    {
        return root.GetSelfAndVisualDescendants().OfType<NotificationCard>().Count();
    }

    private static DispatcherTimer? GetNotificationExpiredTimer(WindowNotificationManager manager)
    {
        return GetPrivateField(manager, "AtomUI.Desktop.Controls.WindowNotificationManager", "_cardExpiredTimer") as DispatcherTimer;
    }

    private static DispatcherTimer? GetNotificationCleanupTimer(WindowNotificationManager manager)
    {
        return GetPrivateField(manager, "AtomUI.Desktop.Controls.WindowNotificationManager", "_cleanupTimer") as DispatcherTimer;
    }

    private static int GetPendingNotificationCount(WindowNotificationManager manager)
    {
        return GetPrivateField(manager, "AtomUI.Desktop.Controls.WindowNotificationManager", "_pendingNotifications") is ICollection pendingNotifications
            ? pendingNotifications.Count
            : 0;
    }
}
