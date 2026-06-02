using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Notification;

[LanguageProvider(LanguageCode.en_US, NotificationShowCase.LanguageId)]
internal class en_US : LanguageProvider
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

    protected override Type GetResourceKindType() => typeof(NotificationShowCaseLangResourceKind);
}
