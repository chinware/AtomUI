using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.PopupConfirm;

public class PopupConfirmViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public static EntityKey ID = "PopupConfirm";

    public IScreen HostScreen { get; }
    public ViewModelActivator Activator { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<PopupConfirmApiRow>? _apiRows;
    private ObservableCollection<PopupConfirmDesignTokenRow>? _designTokenRows;

    public ObservableCollection<PopupConfirmApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<PopupConfirmDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public PopupConfirmViewModel(IScreen screen)
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
            new PopupConfirmApiRow("PopupConfirm.Title", Lang(PopupConfirmShowCaseLangResourceKind.ApiPropertyPopupConfirmTitle), "string", "cyan", "null"),
            new PopupConfirmApiRow("PopupConfirm.ConfirmContent", Lang(PopupConfirmShowCaseLangResourceKind.ApiPropertyPopupConfirmConfirmContent), "object?", "cyan", "null"),
            new PopupConfirmApiRow("PopupConfirm.ConfirmContentTemplate", Lang(PopupConfirmShowCaseLangResourceKind.ApiPropertyPopupConfirmConfirmContentTemplate), "IDataTemplate?", "cyan", "null"),
            new PopupConfirmApiRow("PopupConfirm.OkText", Lang(PopupConfirmShowCaseLangResourceKind.ApiPropertyPopupConfirmOkText), "string", "cyan", "null"),
            new PopupConfirmApiRow("PopupConfirm.CancelText", Lang(PopupConfirmShowCaseLangResourceKind.ApiPropertyPopupConfirmCancelText), "string", "cyan", "null"),
            new PopupConfirmApiRow("PopupConfirm.OkButtonType", Lang(PopupConfirmShowCaseLangResourceKind.ApiPropertyPopupConfirmOkButtonType), "ButtonType", "blue", "Primary"),
            new PopupConfirmApiRow("PopupConfirm.IsShowCancelButton", Lang(PopupConfirmShowCaseLangResourceKind.ApiPropertyPopupConfirmIsShowCancelButton), "bool", "purple", "true"),
            new PopupConfirmApiRow("PopupConfirm.Icon", Lang(PopupConfirmShowCaseLangResourceKind.ApiPropertyPopupConfirmIcon), "PathIcon?", "cyan", "null"),
            new PopupConfirmApiRow("PopupConfirm.ConfirmStatus", Lang(PopupConfirmShowCaseLangResourceKind.ApiPropertyPopupConfirmConfirmStatus), "PopupConfirmStatus", "blue", "Warning"),
            new PopupConfirmApiRow("PopupConfirm.Cancelled", Lang(PopupConfirmShowCaseLangResourceKind.ApiEventPopupConfirmCancelled), "RoutedEvent", "cyan", "-"),
            new PopupConfirmApiRow("PopupConfirm.Confirmed", Lang(PopupConfirmShowCaseLangResourceKind.ApiEventPopupConfirmConfirmed), "RoutedEvent", "cyan", "-"),
            new PopupConfirmApiRow("PopupConfirm.PopupClick", Lang(PopupConfirmShowCaseLangResourceKind.ApiEventPopupConfirmPopupClick), "RoutedEvent", "cyan", "-")
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
            new PopupConfirmDesignTokenRow("PopupMinWidth", Lang(PopupConfirmShowCaseLangResourceKind.TokenNamePopupMinWidth), Lang(PopupConfirmShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(PopupConfirmShowCaseLangResourceKind.TokenStatusStable), "success"),
            new PopupConfirmDesignTokenRow("PopupMinHeight", Lang(PopupConfirmShowCaseLangResourceKind.TokenNamePopupMinHeight), Lang(PopupConfirmShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(PopupConfirmShowCaseLangResourceKind.TokenStatusStable), "success"),
            new PopupConfirmDesignTokenRow("ButtonSpacing", Lang(PopupConfirmShowCaseLangResourceKind.TokenNameButtonSpacing), Lang(PopupConfirmShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(PopupConfirmShowCaseLangResourceKind.TokenStatusStable), "success"),
            new PopupConfirmDesignTokenRow("IconMargin", Lang(PopupConfirmShowCaseLangResourceKind.TokenNameIconMargin), Lang(PopupConfirmShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(PopupConfirmShowCaseLangResourceKind.TokenStatusStable), "success"),
            new PopupConfirmDesignTokenRow("ContentContainerMargin", Lang(PopupConfirmShowCaseLangResourceKind.TokenNameContentContainerMargin), Lang(PopupConfirmShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(PopupConfirmShowCaseLangResourceKind.TokenStatusStable), "success"),
            new PopupConfirmDesignTokenRow("ButtonContainerMargin", Lang(PopupConfirmShowCaseLangResourceKind.TokenNameButtonContainerMargin), Lang(PopupConfirmShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(PopupConfirmShowCaseLangResourceKind.TokenStatusStable), "success"),
            new PopupConfirmDesignTokenRow("TitleMargin", Lang(PopupConfirmShowCaseLangResourceKind.TokenNameTitleMargin), Lang(PopupConfirmShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(PopupConfirmShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(PopupConfirmShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(PopupConfirmShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            PopupConfirmShowCaseLangResourceKind.ApiPropertyPopupConfirmTitle                  => en_US.ApiPropertyPopupConfirmTitle,
            PopupConfirmShowCaseLangResourceKind.ApiPropertyPopupConfirmConfirmContent         => en_US.ApiPropertyPopupConfirmConfirmContent,
            PopupConfirmShowCaseLangResourceKind.ApiPropertyPopupConfirmConfirmContentTemplate => en_US.ApiPropertyPopupConfirmConfirmContentTemplate,
            PopupConfirmShowCaseLangResourceKind.ApiPropertyPopupConfirmOkText                 => en_US.ApiPropertyPopupConfirmOkText,
            PopupConfirmShowCaseLangResourceKind.ApiPropertyPopupConfirmCancelText             => en_US.ApiPropertyPopupConfirmCancelText,
            PopupConfirmShowCaseLangResourceKind.ApiPropertyPopupConfirmOkButtonType           => en_US.ApiPropertyPopupConfirmOkButtonType,
            PopupConfirmShowCaseLangResourceKind.ApiPropertyPopupConfirmIsShowCancelButton     => en_US.ApiPropertyPopupConfirmIsShowCancelButton,
            PopupConfirmShowCaseLangResourceKind.ApiPropertyPopupConfirmIcon                   => en_US.ApiPropertyPopupConfirmIcon,
            PopupConfirmShowCaseLangResourceKind.ApiPropertyPopupConfirmConfirmStatus          => en_US.ApiPropertyPopupConfirmConfirmStatus,
            PopupConfirmShowCaseLangResourceKind.ApiEventPopupConfirmCancelled                 => en_US.ApiEventPopupConfirmCancelled,
            PopupConfirmShowCaseLangResourceKind.ApiEventPopupConfirmConfirmed                 => en_US.ApiEventPopupConfirmConfirmed,
            PopupConfirmShowCaseLangResourceKind.ApiEventPopupConfirmPopupClick                => en_US.ApiEventPopupConfirmPopupClick,
            PopupConfirmShowCaseLangResourceKind.TokenNamePopupMinWidth                        => en_US.TokenNamePopupMinWidth,
            PopupConfirmShowCaseLangResourceKind.TokenNamePopupMinHeight                       => en_US.TokenNamePopupMinHeight,
            PopupConfirmShowCaseLangResourceKind.TokenNameButtonSpacing                        => en_US.TokenNameButtonSpacing,
            PopupConfirmShowCaseLangResourceKind.TokenNameIconMargin                           => en_US.TokenNameIconMargin,
            PopupConfirmShowCaseLangResourceKind.TokenNameContentContainerMargin               => en_US.TokenNameContentContainerMargin,
            PopupConfirmShowCaseLangResourceKind.TokenNameButtonContainerMargin                => en_US.TokenNameButtonContainerMargin,
            PopupConfirmShowCaseLangResourceKind.TokenNameTitleMargin                          => en_US.TokenNameTitleMargin,
            PopupConfirmShowCaseLangResourceKind.TokenScopeComponent                           => en_US.TokenScopeComponent,
            PopupConfirmShowCaseLangResourceKind.TokenStatusStable                             => en_US.TokenStatusStable,
            _                                                                                  => kind.ToString()
        };
    }
}

public sealed record PopupConfirmApiRow(
    string Member,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record PopupConfirmDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
