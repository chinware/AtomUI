using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Notification;

public partial class NotificationShowCase : ReactiveUserControl<NotificationViewModel>
{
    public const string LanguageId = nameof(NotificationShowCase);

    private WindowNotificationManager? _basicManager;
    private WindowNotificationManager? _topLeftManager;
    private WindowNotificationManager? _topManager;
    private WindowNotificationManager? _topRightManager;
    private WindowNotificationManager? _bottomLeftManager;
    private WindowNotificationManager? _bottomManager;
    private WindowNotificationManager? _bottomRightManager;

    public NotificationShowCase()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        HoverOptionGroup.OptionCheckedChanged -= HandleHoverOptionGroupCheckedChanged;
        HoverOptionGroup.OptionCheckedChanged += HandleHoverOptionGroupCheckedChanged;
    }

    private void HandleHoverOptionGroupCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (_basicManager is not null)
        {
            _basicManager.IsPauseOnHover = args.Index == 0;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        HoverOptionGroup.OptionCheckedChanged -= HandleHoverOptionGroupCheckedChanged;
        DisposeManager(ref _basicManager);
        DisposeManager(ref _topLeftManager);
        DisposeManager(ref _topManager);
        DisposeManager(ref _topRightManager);
        DisposeManager(ref _bottomLeftManager);
        DisposeManager(ref _bottomManager);
        DisposeManager(ref _bottomRightManager);
    }

    private WindowNotificationManager? GetBasicManager()
    {
        var manager = GetManager(ref _basicManager, NotificationPosition.TopRight);
        if (manager is not null)
        {
            manager.IsPauseOnHover = HoverOptionGroup.SelectedIndex != 1;
        }
        return manager;
    }

    private WindowNotificationManager? GetManager(ref WindowNotificationManager? manager, NotificationPosition position)
    {
        if (manager is not null)
        {
            return manager;
        }

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            return null;
        }

        manager = new WindowNotificationManager(topLevel)
        {
            MaxItems = 3,
            Position = position
        };
        return manager;
    }

    private static void DisposeManager(ref WindowNotificationManager? manager)
    {
        manager?.Dispose();
        manager = null;
    }

    private void ShowSimpleNotification(object? sender, RoutedEventArgs e)
    {
        GetBasicManager()?.Show(new AtomUINotification(
            "Notification Title",
            "Hello, AtomUI/Avalonia!"
        ));
    }

    private void ShowNeverCloseNotification(object? sender, RoutedEventArgs e)
    {
        GetBasicManager()?.Show(new AtomUINotification(
            expiration: TimeSpan.Zero,
            title: "Notification Title",
            content:
            "I will never close automatically. This is a purposely very very long description that has many many characters and words."
        ));
    }

    private void ShowSuccessNotification(object? sender, RoutedEventArgs e)
    {
        GetBasicManager()?.Show(new AtomUINotification(
            type: NotificationType.Success,
            title: "Notification Title",
            content:
            "This is the content of the notification. This is the content of the notification. This is the content of the notification."
        ));
    }

    private void ShowInfoNotification(object? sender, RoutedEventArgs e)
    {
        GetBasicManager()?.Show(new AtomUINotification(
            type: NotificationType.Information,
            title: "Notification Title",
            content:
            "This is the content of the notification. This is the content of the notification. This is the content of the notification."
        ));
    }

    private void ShowWarningNotification(object? sender, RoutedEventArgs e)
    {
        GetBasicManager()?.Show(new AtomUINotification(
            type: NotificationType.Warning,
            title: "Notification Title",
            content:
            "This is the content of the notification. This is the content of the notification. This is the content of the notification."
        ));
    }

    private void ShowErrorNotification(object? sender, RoutedEventArgs e)
    {
        GetBasicManager()?.Show(new AtomUINotification(
            type: NotificationType.Error,
            title: "Notification Title",
            content:
            "This is the content of the notification. This is the content of the notification. This is the content of the notification."
        ));
    }

    private void ShowTopNotification(object? sender, RoutedEventArgs e)
    {
        GetManager(ref _topManager, NotificationPosition.TopCenter)?.Show(new AtomUINotification(
            "Notification Top",
            "Hello, AtomUI/Avalonia!"
        ));
    }

    private void ShowBottomNotification(object? sender, RoutedEventArgs e)
    {
        GetManager(ref _bottomManager, NotificationPosition.BottomCenter)?.Show(new AtomUINotification(
            "Notification Bottom",
            "Hello, AtomUI/Avalonia!"
        ));
    }

    private void ShowTopLeftNotification(object? sender, RoutedEventArgs e)
    {
        GetManager(ref _topLeftManager, NotificationPosition.TopLeft)?.Show(new AtomUINotification(
            "Notification TopLeft",
            "Hello, AtomUI/Avalonia!"
        ));
    }

    private void ShowTopRightNotification(object? sender, RoutedEventArgs e)
    {
        GetManager(ref _topRightManager, NotificationPosition.TopRight)?.Show(new AtomUINotification(
            "Notification TopRight",
            "Hello, AtomUI/Avalonia!"
        ));
    }

    private void ShowBottomLeftNotification(object? sender, RoutedEventArgs e)
    {
        GetManager(ref _bottomLeftManager, NotificationPosition.BottomLeft)?.Show(new AtomUINotification(
            "Notification BottomLeft",
            "Hello, AtomUI/Avalonia!"
        ));
    }

    private void ShowBottomRightNotification(object? sender, RoutedEventArgs e)
    {
        GetManager(ref _bottomRightManager, NotificationPosition.BottomRight)?.Show(new AtomUINotification(
            "Notification BottomRight",
            "Hello, AtomUI/Avalonia!"
        ));
    }

    private void ShowCustomIconNotification(object? sender, RoutedEventArgs e)
    {
        GetBasicManager()?.Show(new AtomUINotification(
            "Notification Title",
            "This is the content of the notification. This is the content of the notification. This is the content of the notification.",
            icon: new SettingOutlined()
        ));
    }

    private void ShowProgressNotification(object? sender, RoutedEventArgs e)
    {
        GetBasicManager()?.Show(new AtomUINotification(
            type: NotificationType.Information,
            title: "Notification Title",
            content:
            "This is the content of the notification. This is the content of the notification. This is the content of the notification.",
            showProgress: true
        ));
    }
}
