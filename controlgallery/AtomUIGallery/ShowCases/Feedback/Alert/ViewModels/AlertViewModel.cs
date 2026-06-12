using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Alert;

public class AlertViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Alert";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private ObservableCollection<AlertApiRow>? _apiRows;
    private ObservableCollection<AlertDesignTokenRow>? _designTokenRows;

    public ObservableCollection<AlertApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<AlertDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public AlertViewModel(IScreen screen)
    {
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
            new AlertApiRow("Type", Lang(AlertShowCaseLangResourceKind.ApiPropertyType), "AlertType", "purple", "Success"),
            new AlertApiRow("Message", Lang(AlertShowCaseLangResourceKind.ApiPropertyMessage), "string", "cyan", "null"),
            new AlertApiRow("Description", Lang(AlertShowCaseLangResourceKind.ApiPropertyDescription), "string?", "cyan", "null"),
            new AlertApiRow("IsShowIcon", Lang(AlertShowCaseLangResourceKind.ApiPropertyIsShowIcon), "bool", "green", "false"),
            new AlertApiRow("IsMessageMarqueeEnabled", Lang(AlertShowCaseLangResourceKind.ApiPropertyIsMessageMarqueeEnabled), "bool", "green", "false"),
            new AlertApiRow("IsClosable", Lang(AlertShowCaseLangResourceKind.ApiPropertyIsClosable), "bool", "green", "false"),
            new AlertApiRow("CloseIcon", Lang(AlertShowCaseLangResourceKind.ApiPropertyCloseIcon), "PathIcon?", "cyan", "null"),
            new AlertApiRow("ExtraAction", Lang(AlertShowCaseLangResourceKind.ApiPropertyExtraAction), "Control?", "cyan", "null"),
            new AlertApiRow("CloseRequest", Lang(AlertShowCaseLangResourceKind.ApiEventCloseRequest), "event EventHandler?", "orange", "-")
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
            new AlertDesignTokenRow("DefaultPadding", Lang(AlertShowCaseLangResourceKind.TokenNameDefaultPadding), Lang(AlertShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(AlertShowCaseLangResourceKind.TokenStatusStable), "success"),
            new AlertDesignTokenRow("WithDescriptionPadding", Lang(AlertShowCaseLangResourceKind.TokenNameWithDescriptionPadding), Lang(AlertShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(AlertShowCaseLangResourceKind.TokenStatusStable), "success"),
            new AlertDesignTokenRow("MessageWithDescriptionMargin", Lang(AlertShowCaseLangResourceKind.TokenNameMessageWithDescriptionMargin), Lang(AlertShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(AlertShowCaseLangResourceKind.TokenStatusStable), "success"),
            new AlertDesignTokenRow("IconDefaultMargin", Lang(AlertShowCaseLangResourceKind.TokenNameIconDefaultMargin), Lang(AlertShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(AlertShowCaseLangResourceKind.TokenStatusStable), "success"),
            new AlertDesignTokenRow("IconWithDescriptionMargin", Lang(AlertShowCaseLangResourceKind.TokenNameIconWithDescriptionMargin), Lang(AlertShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(AlertShowCaseLangResourceKind.TokenStatusStable), "success"),
            new AlertDesignTokenRow("IconSize", Lang(AlertShowCaseLangResourceKind.TokenNameIconSize), Lang(AlertShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(AlertShowCaseLangResourceKind.TokenStatusStable), "success"),
            new AlertDesignTokenRow("WithDescriptionIconSize", Lang(AlertShowCaseLangResourceKind.TokenNameWithDescriptionIconSize), Lang(AlertShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(AlertShowCaseLangResourceKind.TokenStatusStable), "success"),
            new AlertDesignTokenRow("CloseIconSize", Lang(AlertShowCaseLangResourceKind.TokenNameCloseIconSize), Lang(AlertShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(AlertShowCaseLangResourceKind.TokenStatusStable), "success"),
            new AlertDesignTokenRow("ExtraElementMargin", Lang(AlertShowCaseLangResourceKind.TokenNameExtraElementMargin), Lang(AlertShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(AlertShowCaseLangResourceKind.TokenStatusStable), "success"),
            new AlertDesignTokenRow("DescriptionLabelMargin", Lang(AlertShowCaseLangResourceKind.TokenNameDescriptionLabelMargin), Lang(AlertShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(AlertShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(AlertShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(AlertShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            AlertShowCaseLangResourceKind.ApiPropertyType                    => en_US.ApiPropertyType,
            AlertShowCaseLangResourceKind.ApiPropertyMessage                 => en_US.ApiPropertyMessage,
            AlertShowCaseLangResourceKind.ApiPropertyDescription             => en_US.ApiPropertyDescription,
            AlertShowCaseLangResourceKind.ApiPropertyIsShowIcon              => en_US.ApiPropertyIsShowIcon,
            AlertShowCaseLangResourceKind.ApiPropertyIsMessageMarqueeEnabled => en_US.ApiPropertyIsMessageMarqueeEnabled,
            AlertShowCaseLangResourceKind.ApiPropertyIsClosable              => en_US.ApiPropertyIsClosable,
            AlertShowCaseLangResourceKind.ApiPropertyCloseIcon               => en_US.ApiPropertyCloseIcon,
            AlertShowCaseLangResourceKind.ApiPropertyExtraAction             => en_US.ApiPropertyExtraAction,
            AlertShowCaseLangResourceKind.ApiEventCloseRequest               => en_US.ApiEventCloseRequest,
            AlertShowCaseLangResourceKind.TokenNameDefaultPadding            => en_US.TokenNameDefaultPadding,
            AlertShowCaseLangResourceKind.TokenNameWithDescriptionPadding    => en_US.TokenNameWithDescriptionPadding,
            AlertShowCaseLangResourceKind.TokenNameMessageWithDescriptionMargin => en_US.TokenNameMessageWithDescriptionMargin,
            AlertShowCaseLangResourceKind.TokenNameIconDefaultMargin         => en_US.TokenNameIconDefaultMargin,
            AlertShowCaseLangResourceKind.TokenNameIconWithDescriptionMargin => en_US.TokenNameIconWithDescriptionMargin,
            AlertShowCaseLangResourceKind.TokenNameIconSize                  => en_US.TokenNameIconSize,
            AlertShowCaseLangResourceKind.TokenNameWithDescriptionIconSize   => en_US.TokenNameWithDescriptionIconSize,
            AlertShowCaseLangResourceKind.TokenNameCloseIconSize             => en_US.TokenNameCloseIconSize,
            AlertShowCaseLangResourceKind.TokenNameExtraElementMargin        => en_US.TokenNameExtraElementMargin,
            AlertShowCaseLangResourceKind.TokenNameDescriptionLabelMargin    => en_US.TokenNameDescriptionLabelMargin,
            AlertShowCaseLangResourceKind.TokenScopeComponent                => en_US.TokenScopeComponent,
            AlertShowCaseLangResourceKind.TokenStatusStable                  => en_US.TokenStatusStable,
            _                                                                => kind.ToString()
        };
    }
}

public sealed record AlertApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record AlertDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
