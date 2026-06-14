using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Message;

public class MessageViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public static EntityKey ID = "Message";

    public IScreen HostScreen { get; }
    public ViewModelActivator Activator { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<MessageApiRow>? _apiRows;
    private ObservableCollection<MessageDesignTokenRow>? _designTokenRows;

    public ObservableCollection<MessageApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<MessageDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public MessageViewModel(IScreen screen)
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
            new MessageApiRow("Message.Content", Lang(MessageShowCaseLangResourceKind.ApiPropertyMessageContent), "string", "cyan", "required"),
            new MessageApiRow("Message.Type", Lang(MessageShowCaseLangResourceKind.ApiPropertyMessageType), "MessageType", "blue", "Information"),
            new MessageApiRow("Message.Icon", Lang(MessageShowCaseLangResourceKind.ApiPropertyMessageIcon), "PathIcon?", "cyan", "null"),
            new MessageApiRow("Message.Expiration", Lang(MessageShowCaseLangResourceKind.ApiPropertyMessageExpiration), "TimeSpan", "cyan", "5s"),
            new MessageApiRow("Message.OnClose", Lang(MessageShowCaseLangResourceKind.ApiPropertyMessageOnClose), "Action?", "cyan", "null"),
            new MessageApiRow("WindowMessageManager.Position", Lang(MessageShowCaseLangResourceKind.ApiPropertyManagerPosition), "NotificationPosition", "blue", "TopRight"),
            new MessageApiRow("WindowMessageManager.MaxItems", Lang(MessageShowCaseLangResourceKind.ApiPropertyManagerMaxItems), "int", "purple", "5"),
            new MessageApiRow("WindowMessageManager.IsMotionEnabled", Lang(MessageShowCaseLangResourceKind.ApiPropertyManagerIsMotionEnabled), "bool", "purple", "true"),
            new MessageApiRow("IMessageManager.Show", Lang(MessageShowCaseLangResourceKind.ApiMethodManagerShow), "void", "cyan", "-")
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
            new MessageDesignTokenRow("ContentBg", Lang(MessageShowCaseLangResourceKind.TokenNameMessageContentBg), Lang(MessageShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(MessageShowCaseLangResourceKind.TokenStatusStable), "success"),
            new MessageDesignTokenRow("ContentPadding", Lang(MessageShowCaseLangResourceKind.TokenNameMessageContentPadding), Lang(MessageShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(MessageShowCaseLangResourceKind.TokenStatusStable), "success"),
            new MessageDesignTokenRow("CardHeight", Lang(MessageShowCaseLangResourceKind.TokenNameMessageCardHeight), Lang(MessageShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(MessageShowCaseLangResourceKind.TokenStatusStable), "success"),
            new MessageDesignTokenRow("MessageIconSize", Lang(MessageShowCaseLangResourceKind.TokenNameMessageIconSize), Lang(MessageShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(MessageShowCaseLangResourceKind.TokenStatusStable), "success"),
            new MessageDesignTokenRow("MessageIconMargin", Lang(MessageShowCaseLangResourceKind.TokenNameMessageIconMargin), Lang(MessageShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(MessageShowCaseLangResourceKind.TokenStatusStable), "success"),
            new MessageDesignTokenRow("MessageTopMargin", Lang(MessageShowCaseLangResourceKind.TokenNameMessageTopMargin), Lang(MessageShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(MessageShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(MessageShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(MessageShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            MessageShowCaseLangResourceKind.ApiPropertyMessageContent          => en_US.ApiPropertyMessageContent,
            MessageShowCaseLangResourceKind.ApiPropertyMessageType             => en_US.ApiPropertyMessageType,
            MessageShowCaseLangResourceKind.ApiPropertyMessageIcon             => en_US.ApiPropertyMessageIcon,
            MessageShowCaseLangResourceKind.ApiPropertyMessageExpiration       => en_US.ApiPropertyMessageExpiration,
            MessageShowCaseLangResourceKind.ApiPropertyMessageOnClose          => en_US.ApiPropertyMessageOnClose,
            MessageShowCaseLangResourceKind.ApiPropertyManagerPosition         => en_US.ApiPropertyManagerPosition,
            MessageShowCaseLangResourceKind.ApiPropertyManagerMaxItems         => en_US.ApiPropertyManagerMaxItems,
            MessageShowCaseLangResourceKind.ApiPropertyManagerIsMotionEnabled  => en_US.ApiPropertyManagerIsMotionEnabled,
            MessageShowCaseLangResourceKind.ApiMethodManagerShow               => en_US.ApiMethodManagerShow,
            MessageShowCaseLangResourceKind.TokenNameMessageContentBg          => en_US.TokenNameMessageContentBg,
            MessageShowCaseLangResourceKind.TokenNameMessageContentPadding     => en_US.TokenNameMessageContentPadding,
            MessageShowCaseLangResourceKind.TokenNameMessageCardHeight         => en_US.TokenNameMessageCardHeight,
            MessageShowCaseLangResourceKind.TokenNameMessageIconSize           => en_US.TokenNameMessageIconSize,
            MessageShowCaseLangResourceKind.TokenNameMessageIconMargin         => en_US.TokenNameMessageIconMargin,
            MessageShowCaseLangResourceKind.TokenNameMessageTopMargin          => en_US.TokenNameMessageTopMargin,
            MessageShowCaseLangResourceKind.TokenScopeComponent                => en_US.TokenScopeComponent,
            MessageShowCaseLangResourceKind.TokenStatusStable                  => en_US.TokenStatusStable,
            _                                                                  => kind.ToString()
        };
    }
}

public sealed record MessageApiRow(
    string Member,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record MessageDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
