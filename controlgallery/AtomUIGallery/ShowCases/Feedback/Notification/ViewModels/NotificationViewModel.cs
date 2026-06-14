using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Notification;

public class NotificationViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public static EntityKey ID = "Notification";

    public IScreen HostScreen { get; }
    public ViewModelActivator Activator { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<NotificationApiRow>? _apiRows;
    private ObservableCollection<NotificationDesignTokenRow>? _designTokenRows;

    public ObservableCollection<NotificationApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<NotificationDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public NotificationViewModel(IScreen screen)
    {
        Activator  = new ViewModelActivator();
        HostScreen = screen;
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new NotificationApiRow("Notification.Title", Lang(NotificationShowCaseLangResourceKind.ApiPropertyNotificationTitle), "string", "cyan", "required"),
            new NotificationApiRow("Notification.Content", Lang(NotificationShowCaseLangResourceKind.ApiPropertyNotificationContent), "object?", "cyan", "required"),
            new NotificationApiRow("Notification.Type", Lang(NotificationShowCaseLangResourceKind.ApiPropertyNotificationType), "NotificationType", "blue", "Information"),
            new NotificationApiRow("Notification.Icon", Lang(NotificationShowCaseLangResourceKind.ApiPropertyNotificationIcon), "PathIcon?", "cyan", "null"),
            new NotificationApiRow("Notification.Expiration", Lang(NotificationShowCaseLangResourceKind.ApiPropertyNotificationExpiration), "TimeSpan", "cyan", "5s"),
            new NotificationApiRow("Notification.ShowProgress", Lang(NotificationShowCaseLangResourceKind.ApiPropertyNotificationShowProgress), "bool", "purple", "false"),
            new NotificationApiRow("Notification.OnClick", Lang(NotificationShowCaseLangResourceKind.ApiPropertyNotificationOnClick), "Action?", "cyan", "null"),
            new NotificationApiRow("Notification.OnClose", Lang(NotificationShowCaseLangResourceKind.ApiPropertyNotificationOnClose), "Action?", "cyan", "null"),
            new NotificationApiRow("WindowNotificationManager.Position", Lang(NotificationShowCaseLangResourceKind.ApiPropertyManagerPosition), "NotificationPosition", "blue", "TopRight"),
            new NotificationApiRow("WindowNotificationManager.MaxItems", Lang(NotificationShowCaseLangResourceKind.ApiPropertyManagerMaxItems), "int", "purple", "5"),
            new NotificationApiRow("WindowNotificationManager.IsPauseOnHover", Lang(NotificationShowCaseLangResourceKind.ApiPropertyManagerIsPauseOnHover), "bool", "purple", "true"),
            new NotificationApiRow("WindowNotificationManager.IsMotionEnabled", Lang(NotificationShowCaseLangResourceKind.ApiPropertyManagerIsMotionEnabled), "bool", "purple", "true"),
            new NotificationApiRow("INotificationManager.Show", Lang(NotificationShowCaseLangResourceKind.ApiMethodManagerShow), "void", "cyan", "-")
        ];
    }

    public void EnsureDesignTokenRows()
    {
        if (DesignTokenRows is not null)
        {
            return;
        }

        DesignTokenRows =
        [
            new NotificationDesignTokenRow("NotificationBg", Lang(NotificationShowCaseLangResourceKind.TokenNameNotificationBg), Lang(NotificationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(NotificationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new NotificationDesignTokenRow("NotificationPadding", Lang(NotificationShowCaseLangResourceKind.TokenNameNotificationPadding), Lang(NotificationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(NotificationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new NotificationDesignTokenRow("NotificationIconSize", Lang(NotificationShowCaseLangResourceKind.TokenNameNotificationIconSize), Lang(NotificationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(NotificationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new NotificationDesignTokenRow("NotificationIconMargin", Lang(NotificationShowCaseLangResourceKind.TokenNameNotificationIconMargin), Lang(NotificationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(NotificationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new NotificationDesignTokenRow("NotificationCloseButtonSize", Lang(NotificationShowCaseLangResourceKind.TokenNameNotificationCloseButtonSize), Lang(NotificationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(NotificationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new NotificationDesignTokenRow("NotificationCloseButtonPadding", Lang(NotificationShowCaseLangResourceKind.TokenNameNotificationCloseButtonPadding), Lang(NotificationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(NotificationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new NotificationDesignTokenRow("NotificationProgressHeight", Lang(NotificationShowCaseLangResourceKind.TokenNameNotificationProgressHeight), Lang(NotificationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(NotificationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new NotificationDesignTokenRow("NotificationProgressBg", Lang(NotificationShowCaseLangResourceKind.TokenNameNotificationProgressBg), Lang(NotificationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(NotificationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new NotificationDesignTokenRow("NotificationProgressMargin", Lang(NotificationShowCaseLangResourceKind.TokenNameNotificationProgressMargin), Lang(NotificationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(NotificationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new NotificationDesignTokenRow("NotificationWidth", Lang(NotificationShowCaseLangResourceKind.TokenNameNotificationWidth), Lang(NotificationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(NotificationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new NotificationDesignTokenRow("NotificationTopMargin", Lang(NotificationShowCaseLangResourceKind.TokenNameNotificationTopMargin), Lang(NotificationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(NotificationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new NotificationDesignTokenRow("NotificationBottomMargin", Lang(NotificationShowCaseLangResourceKind.TokenNameNotificationBottomMargin), Lang(NotificationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(NotificationShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(NotificationShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(NotificationShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            NotificationShowCaseLangResourceKind.ApiPropertyNotificationTitle               => en_US.ApiPropertyNotificationTitle,
            NotificationShowCaseLangResourceKind.ApiPropertyNotificationContent             => en_US.ApiPropertyNotificationContent,
            NotificationShowCaseLangResourceKind.ApiPropertyNotificationType                => en_US.ApiPropertyNotificationType,
            NotificationShowCaseLangResourceKind.ApiPropertyNotificationIcon                => en_US.ApiPropertyNotificationIcon,
            NotificationShowCaseLangResourceKind.ApiPropertyNotificationExpiration          => en_US.ApiPropertyNotificationExpiration,
            NotificationShowCaseLangResourceKind.ApiPropertyNotificationShowProgress        => en_US.ApiPropertyNotificationShowProgress,
            NotificationShowCaseLangResourceKind.ApiPropertyNotificationOnClick             => en_US.ApiPropertyNotificationOnClick,
            NotificationShowCaseLangResourceKind.ApiPropertyNotificationOnClose             => en_US.ApiPropertyNotificationOnClose,
            NotificationShowCaseLangResourceKind.ApiPropertyManagerPosition                 => en_US.ApiPropertyManagerPosition,
            NotificationShowCaseLangResourceKind.ApiPropertyManagerMaxItems                 => en_US.ApiPropertyManagerMaxItems,
            NotificationShowCaseLangResourceKind.ApiPropertyManagerIsPauseOnHover           => en_US.ApiPropertyManagerIsPauseOnHover,
            NotificationShowCaseLangResourceKind.ApiPropertyManagerIsMotionEnabled          => en_US.ApiPropertyManagerIsMotionEnabled,
            NotificationShowCaseLangResourceKind.ApiMethodManagerShow                       => en_US.ApiMethodManagerShow,
            NotificationShowCaseLangResourceKind.TokenNameNotificationBg                    => en_US.TokenNameNotificationBg,
            NotificationShowCaseLangResourceKind.TokenNameNotificationPadding               => en_US.TokenNameNotificationPadding,
            NotificationShowCaseLangResourceKind.TokenNameNotificationIconSize              => en_US.TokenNameNotificationIconSize,
            NotificationShowCaseLangResourceKind.TokenNameNotificationIconMargin            => en_US.TokenNameNotificationIconMargin,
            NotificationShowCaseLangResourceKind.TokenNameNotificationCloseButtonSize       => en_US.TokenNameNotificationCloseButtonSize,
            NotificationShowCaseLangResourceKind.TokenNameNotificationCloseButtonPadding    => en_US.TokenNameNotificationCloseButtonPadding,
            NotificationShowCaseLangResourceKind.TokenNameNotificationProgressHeight        => en_US.TokenNameNotificationProgressHeight,
            NotificationShowCaseLangResourceKind.TokenNameNotificationProgressBg            => en_US.TokenNameNotificationProgressBg,
            NotificationShowCaseLangResourceKind.TokenNameNotificationProgressMargin        => en_US.TokenNameNotificationProgressMargin,
            NotificationShowCaseLangResourceKind.TokenNameNotificationWidth                 => en_US.TokenNameNotificationWidth,
            NotificationShowCaseLangResourceKind.TokenNameNotificationTopMargin             => en_US.TokenNameNotificationTopMargin,
            NotificationShowCaseLangResourceKind.TokenNameNotificationBottomMargin          => en_US.TokenNameNotificationBottomMargin,
            NotificationShowCaseLangResourceKind.TokenScopeComponent                        => en_US.TokenScopeComponent,
            NotificationShowCaseLangResourceKind.TokenStatusStable                          => en_US.TokenStatusStable,
            _                                                                               => kind.ToString()
        };
    }
}

public sealed record NotificationApiRow(
    string Member,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record NotificationDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
