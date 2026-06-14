using AtomUI.Controls;
using AtomUI.Controls.Commons;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ScenarioTabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUIGallery.ShowCases.Notification;

public partial class NotificationShowCase : GalleryReactiveUserControl<NotificationViewModel>
{
    public const string LanguageId = nameof(NotificationShowCase);

    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;
    private WindowNotificationManager? _basicManager;
    private WindowNotificationManager? _topLeftManager;
    private WindowNotificationManager? _topManager;
    private WindowNotificationManager? _topRightManager;
    private WindowNotificationManager? _bottomLeftManager;
    private WindowNotificationManager? _bottomManager;
    private WindowNotificationManager? _bottomRightManager;
    private bool _isPauseOnHover = true;

    public NotificationShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);
        AddHandler(AbstractOptionButtonGroup.OptionCheckedChangedEvent, HandleHoverOptionGroupCheckedChanged);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _scenarioController.Attach(DataContext);
    }

    private void HandleHoverOptionGroupCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        _isPauseOnHover = args.Index == 0;
        if (_basicManager is not null)
        {
            _basicManager.IsPauseOnHover = _isPauseOnHover;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _scenarioController.Detach();
        DisposeManager(ref _basicManager);
        DisposeManager(ref _topLeftManager);
        DisposeManager(ref _topManager);
        DisposeManager(ref _topRightManager);
        DisposeManager(ref _bottomLeftManager);
        DisposeManager(ref _bottomManager);
        DisposeManager(ref _bottomRightManager);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        _scenarioController.UpdateDataContext(DataContext);
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            ApiScenario         => new NotificationApiDataGrid(),
            DesignTokenScenario => new NotificationDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Notification scenario: {scenario}")
        };
    }

    private WindowNotificationManager? GetBasicManager()
    {
        var manager = GetManager(ref _basicManager, NotificationPosition.TopRight);
        if (manager is not null)
        {
            manager.IsPauseOnHover = _isPauseOnHover;
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
            Lang(NotificationShowCaseLangResourceKind.P2NotificationTitle, "Notification Title"),
            Lang(NotificationShowCaseLangResourceKind.P2NotificationHello, "Hello, AtomUI/Avalonia!")
        ));
    }

    private void ShowNeverCloseNotification(object? sender, RoutedEventArgs e)
    {
        GetBasicManager()?.Show(new AtomUINotification(
            expiration: TimeSpan.Zero,
            title: Lang(NotificationShowCaseLangResourceKind.P2NotificationTitle, "Notification Title"),
            content: Lang(NotificationShowCaseLangResourceKind.P2NotificationNeverCloseContent,
                "I will never close automatically. This is a purposely very very long description that has many many characters and words.")
        ));
    }

    private void ShowSuccessNotification(object? sender, RoutedEventArgs e)
    {
        GetBasicManager()?.Show(new AtomUINotification(
            type: NotificationType.Success,
            title: Lang(NotificationShowCaseLangResourceKind.P2NotificationTitle, "Notification Title"),
            content: Lang(NotificationShowCaseLangResourceKind.P2NotificationContent,
                "This is the content of the notification. This is the content of the notification. This is the content of the notification.")
        ));
    }

    private void ShowInfoNotification(object? sender, RoutedEventArgs e)
    {
        GetBasicManager()?.Show(new AtomUINotification(
            type: NotificationType.Information,
            title: Lang(NotificationShowCaseLangResourceKind.P2NotificationTitle, "Notification Title"),
            content: Lang(NotificationShowCaseLangResourceKind.P2NotificationContent,
                "This is the content of the notification. This is the content of the notification. This is the content of the notification.")
        ));
    }

    private void ShowWarningNotification(object? sender, RoutedEventArgs e)
    {
        GetBasicManager()?.Show(new AtomUINotification(
            type: NotificationType.Warning,
            title: Lang(NotificationShowCaseLangResourceKind.P2NotificationTitle, "Notification Title"),
            content: Lang(NotificationShowCaseLangResourceKind.P2NotificationContent,
                "This is the content of the notification. This is the content of the notification. This is the content of the notification.")
        ));
    }

    private void ShowErrorNotification(object? sender, RoutedEventArgs e)
    {
        GetBasicManager()?.Show(new AtomUINotification(
            type: NotificationType.Error,
            title: Lang(NotificationShowCaseLangResourceKind.P2NotificationTitle, "Notification Title"),
            content: Lang(NotificationShowCaseLangResourceKind.P2NotificationContent,
                "This is the content of the notification. This is the content of the notification. This is the content of the notification.")
        ));
    }

    private void ShowTopNotification(object? sender, RoutedEventArgs e)
    {
        GetManager(ref _topManager, NotificationPosition.TopCenter)?.Show(new AtomUINotification(
            Lang(NotificationShowCaseLangResourceKind.P2NotificationTopTitle, "Notification Top"),
            Lang(NotificationShowCaseLangResourceKind.P2NotificationHello, "Hello, AtomUI/Avalonia!")
        ));
    }

    private void ShowBottomNotification(object? sender, RoutedEventArgs e)
    {
        GetManager(ref _bottomManager, NotificationPosition.BottomCenter)?.Show(new AtomUINotification(
            Lang(NotificationShowCaseLangResourceKind.P2NotificationBottomTitle, "Notification Bottom"),
            Lang(NotificationShowCaseLangResourceKind.P2NotificationHello, "Hello, AtomUI/Avalonia!")
        ));
    }

    private void ShowTopLeftNotification(object? sender, RoutedEventArgs e)
    {
        GetManager(ref _topLeftManager, NotificationPosition.TopLeft)?.Show(new AtomUINotification(
            Lang(NotificationShowCaseLangResourceKind.P2NotificationTopLeftTitle, "Notification TopLeft"),
            Lang(NotificationShowCaseLangResourceKind.P2NotificationHello, "Hello, AtomUI/Avalonia!")
        ));
    }

    private void ShowTopRightNotification(object? sender, RoutedEventArgs e)
    {
        GetManager(ref _topRightManager, NotificationPosition.TopRight)?.Show(new AtomUINotification(
            Lang(NotificationShowCaseLangResourceKind.P2NotificationTopRightTitle, "Notification TopRight"),
            Lang(NotificationShowCaseLangResourceKind.P2NotificationHello, "Hello, AtomUI/Avalonia!")
        ));
    }

    private void ShowBottomLeftNotification(object? sender, RoutedEventArgs e)
    {
        GetManager(ref _bottomLeftManager, NotificationPosition.BottomLeft)?.Show(new AtomUINotification(
            Lang(NotificationShowCaseLangResourceKind.P2NotificationBottomLeftTitle, "Notification BottomLeft"),
            Lang(NotificationShowCaseLangResourceKind.P2NotificationHello, "Hello, AtomUI/Avalonia!")
        ));
    }

    private void ShowBottomRightNotification(object? sender, RoutedEventArgs e)
    {
        GetManager(ref _bottomRightManager, NotificationPosition.BottomRight)?.Show(new AtomUINotification(
            Lang(NotificationShowCaseLangResourceKind.P2NotificationBottomRightTitle, "Notification BottomRight"),
            Lang(NotificationShowCaseLangResourceKind.P2NotificationHello, "Hello, AtomUI/Avalonia!")
        ));
    }

    private void ShowCustomIconNotification(object? sender, RoutedEventArgs e)
    {
        GetBasicManager()?.Show(new AtomUINotification(
            Lang(NotificationShowCaseLangResourceKind.P2NotificationTitle, "Notification Title"),
            Lang(NotificationShowCaseLangResourceKind.P2NotificationContent,
                "This is the content of the notification. This is the content of the notification. This is the content of the notification."),
            icon: new SettingOutlined()
        ));
    }

    private void ShowProgressNotification(object? sender, RoutedEventArgs e)
    {
        GetBasicManager()?.Show(new AtomUINotification(
            type: NotificationType.Information,
            title: Lang(NotificationShowCaseLangResourceKind.P2NotificationTitle, "Notification Title"),
            content: Lang(NotificationShowCaseLangResourceKind.P2NotificationContent,
                "This is the content of the notification. This is the content of the notification. This is the content of the notification."),
            showProgress: true
        ));
    }

    private static string Lang(NotificationShowCaseLangResourceKind resourceKind, string fallback)
    {
        return LanguageResourceBinder.GetLangResource(resourceKind) ?? fallback;
    }
}
