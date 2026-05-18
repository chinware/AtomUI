using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateNotificationScenarios()
    {
        return
        [
            new PerfScenario("NotificationCard.Information.NoMotion", _ => CreateNotificationCard(NotificationType.Information, isShowProgress: false)),
            new PerfScenario("NotificationCard.Success.NoMotion", _ => CreateNotificationCard(NotificationType.Success, isShowProgress: false)),
            new PerfScenario("NotificationCard.Warning.NoMotion", _ => CreateNotificationCard(NotificationType.Warning, isShowProgress: false)),
            new PerfScenario("NotificationCard.Error.NoMotion", _ => CreateNotificationCard(NotificationType.Error, isShowProgress: false)),
            new PerfScenario("NotificationCard.Progress.NoMotion", _ => CreateNotificationCard(NotificationType.Information, isShowProgress: true)),
            new PerfScenario("WindowNotificationManager.Empty.Closed", _ => CreateWindowNotificationManager()),
            new PerfScenario("WindowNotificationManager.Show.Single.NoMotion", _ => new NotificationManagerShowHost(1)),
            new PerfScenario("WindowNotificationManager.Show.MaxItems.NoMotion", _ => new NotificationManagerShowHost(12))
        ];
    }

    private static NotificationCard CreateNotificationCard(NotificationType type, bool isShowProgress)
    {
        var manager = CreateWindowNotificationManager();
        return new NotificationCard(manager)
        {
            Title            = "Notification Title",
            Content          = "Hello, AtomUI/Avalonia!",
            NotificationType = type,
            Expiration       = isShowProgress ? TimeSpan.FromSeconds(5) : null,
            IsShowProgress   = isShowProgress,
            IsMotionEnabled  = false
        };
    }

    private static WindowNotificationManager CreateWindowNotificationManager()
    {
        return new WindowNotificationManager(null)
        {
            IsMotionEnabled = false,
            MaxItems        = 10
        };
    }

    private sealed class NotificationManagerShowHost : Border
    {
        private readonly int _notificationCount;
        private WindowNotificationManager? _notificationManager;
        private bool _isShown;

        public NotificationManagerShowHost(int notificationCount)
        {
            _notificationCount = notificationCount;
            _notificationManager = new WindowNotificationManager(null)
            {
                IsMotionEnabled = false,
                MaxItems        = 5
            };
            Child = _notificationManager;
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            Dispatcher.UIThread.Post(ShowNotifications, DispatcherPriority.Loaded);
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            _notificationManager?.Dispose();
            _notificationManager = null;
        }

        private void ShowNotifications()
        {
            if (_isShown || _notificationManager is null)
            {
                return;
            }

            _isShown = true;
            for (var i = 0; i < _notificationCount; i++)
            {
                _notificationManager.Show(new Notification(
                    type: i % 2 == 0 ? NotificationType.Information : NotificationType.Success,
                    title: $"Notification {i}",
                    content: "Hello, AtomUI/Avalonia!",
                    expiration: TimeSpan.Zero));
            }
        }
    }
}
