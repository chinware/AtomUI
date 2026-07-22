using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Notification;

[LanguageProvider(LanguageCode.en_US, NotificationShowCase.LanguageId)]
internal partial class en_US
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The simplest usage for Notification.";
    public const string DurationTitle = "Duration after which the notification box is closed";
    public const string DurationDescription = "Duration can be used to specify how long the notification stays open. After the duration time elapses, the notification closes automatically. If not specified, default value is 4.5 seconds. If you set the value to TimeSpan.Zero, the notification box will never close automatically.";
    public const string WithIconTitle = "Notification with icon";
    public const string WithIconDescription = "A notification box with a icon at the left side.";
    public const string PlacementTitle = "Placement";
    public const string PlacementDescription = "A notification box can appear from the top bottom topLeft topRight bottomLeft or bottomRight of the viewport via placement.";
    public const string CustomizedIconTitle = "Customized Icon";
    public const string CustomizedIconDescription = "The icon can be customized to any icon node.";
    public const string ProgressTitle = "Show with progress";
    public const string ProgressDescription = "Show progress bar for auto-closing notification.";
    public const string ComponentCategory = "Feedback";
    public const string ComponentStatusStable = "Stable";
    public const string PageSubtitle = "Desktop notifications for longer feedback and non-blocking updates.";
    public const string PageDescription = "Notification displays rich feedback cards near the edge of the window. It supports semantic types, custom icons, placement, manual duration, hover pause, and auto-close progress.";
    public const string ScenarioExamples = "Examples";
    public const string ApiMethodManagerShow = "Shows an INotification instance and applies optional style classes.";
    public const string P2ContentShowNotification = "Show Notification";
    public const string P2ContentOpenTheNotificationBox = "Open the notification box";
    public const string P2ContentSuccess = "Success";
    public const string P2ContentInfo = "Info";
    public const string P2ContentWarning = "Warning";
    public const string P2ContentError = "Error";
    public const string P2ContentTop = "Top";
    public const string P2ContentBottom = "Bottom";
    public const string P2ContentTopleft = "TopLeft";
    public const string P2ContentTopright = "TopRight";
    public const string P2ContentBottomleft = "BottomLeft";
    public const string P2ContentBottomright = "BottomRight";
    public const string P2ContentPauseOnHover = "Pause on hover";
    public const string P2ContentDonTPauseOnHover = "Don't pause on hover";
    public const string P2NotificationTitle = "Notification Title";
    public const string P2NotificationTopTitle = "Notification Top";
    public const string P2NotificationBottomTitle = "Notification Bottom";
    public const string P2NotificationTopLeftTitle = "Notification TopLeft";
    public const string P2NotificationTopRightTitle = "Notification TopRight";
    public const string P2NotificationBottomLeftTitle = "Notification BottomLeft";
    public const string P2NotificationBottomRightTitle = "Notification BottomRight";
    public const string P2NotificationHello = "Hello, AtomUI/Avalonia!";
    public const string P2NotificationContent = "This is the content of the notification. This is the content of the notification. This is the content of the notification.";
    public const string P2NotificationNeverCloseContent = "I will never close automatically. This is a purposely very very long description that has many many characters and words.";

}
