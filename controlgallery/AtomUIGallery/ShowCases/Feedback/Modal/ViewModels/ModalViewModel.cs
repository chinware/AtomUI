using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Modal;

public class ModalViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Modal";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<ModalApiRow>? _apiRows;
    private ObservableCollection<ModalDesignTokenRow>? _designTokenRows;

    public ObservableCollection<ModalApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<ModalDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    private bool _isBasicModalOpened;

    public bool IsBasicModalOpened
    {
        get => _isBasicModalOpened;
        set => this.RaiseAndSetIfChanged(ref _isBasicModalOpened, value);
    }

    private bool _isBasicWindowModalOpened;

    public bool IsBasicWindowModalOpened
    {
        get => _isBasicWindowModalOpened;
        set => this.RaiseAndSetIfChanged(ref _isBasicWindowModalOpened, value);
    }

    private DialogHostType _messageBoxStyleCaseHostType;

    public DialogHostType MessageBoxStyleCaseHostType
    {
        get => _messageBoxStyleCaseHostType;
        set => this.RaiseAndSetIfChanged(ref _messageBoxStyleCaseHostType, value);
    }

    private bool _isConfirmMsgBoxOpened;

    public bool IsConfirmMsgBoxOpened
    {
        get => _isConfirmMsgBoxOpened;
        set => this.RaiseAndSetIfChanged(ref _isConfirmMsgBoxOpened, value);
    }

    private bool _isInformationMsgBoxOpened;

    public bool IsInformationMsgBoxOpened
    {
        get => _isInformationMsgBoxOpened;
        set => this.RaiseAndSetIfChanged(ref _isInformationMsgBoxOpened, value);
    }

    private bool _isSuccessMsgBoxOpened;

    public bool IsSuccessMsgBoxOpened
    {
        get => _isSuccessMsgBoxOpened;
        set => this.RaiseAndSetIfChanged(ref _isSuccessMsgBoxOpened, value);
    }

    private bool _isErrorMsgBoxOpened;

    public bool IsErrorMsgBoxOpened
    {
        get => _isErrorMsgBoxOpened;
        set => this.RaiseAndSetIfChanged(ref _isErrorMsgBoxOpened, value);
    }

    private bool _isWarningMsgBoxOpened;

    public bool IsWarningMsgBoxOpened
    {
        get => _isWarningMsgBoxOpened;
        set => this.RaiseAndSetIfChanged(ref _isWarningMsgBoxOpened, value);
    }

    private bool _isLoadingMsgBoxOpened;

    public bool IsLoadingMsgBoxOpened
    {
        get => _isLoadingMsgBoxOpened;
        set => this.RaiseAndSetIfChanged(ref _isLoadingMsgBoxOpened, value);
    }

    private bool _isAsyncDialogOpened;

    public bool IsAsyncDialogOpened
    {
        get => _isAsyncDialogOpened;
        set => this.RaiseAndSetIfChanged(ref _isAsyncDialogOpened, value);
    }

    private bool _isCustomFooterDialogOpened;

    public bool IsCustomFooterDialogOpened
    {
        get => _isCustomFooterDialogOpened;
        set => this.RaiseAndSetIfChanged(ref _isCustomFooterDialogOpened, value);
    }

    private bool _isCustomFooterMsgBoxOpened;

    public bool IsCustomFooterMsgBoxOpened
    {
        get => _isCustomFooterMsgBoxOpened;
        set => this.RaiseAndSetIfChanged(ref _isCustomFooterMsgBoxOpened, value);
    }

    private bool _isDraggableMsgBoxOpened;

    public bool IsDraggableMsgBoxOpened
    {
        get => _isDraggableMsgBoxOpened;
        set => this.RaiseAndSetIfChanged(ref _isDraggableMsgBoxOpened, value);
    }

    private bool _isDelayedCloseMsgBoxOpened;

    public bool IsDelayedCloseMsgBoxOpened
    {
        get => _isDelayedCloseMsgBoxOpened;
        set => this.RaiseAndSetIfChanged(ref _isDelayedCloseMsgBoxOpened, value);
    }

    private int _countdownSeconds;

    public int CountdownSeconds
    {
        get => _countdownSeconds;
        set => this.RaiseAndSetIfChanged(ref _countdownSeconds, value);
    }

    private bool _isConfigureButtonsDialogOpened;

    public bool IsConfigureButtonsDialogOpened
    {
        get => _isConfigureButtonsDialogOpened;
        set => this.RaiseAndSetIfChanged(ref _isConfigureButtonsDialogOpened, value);
    }

    public ModalViewModel(IScreen screen)
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
            new ModalApiRow("Dialog.Content", Lang(ModalShowCaseLangResourceKind.ApiPropertyDialogContent), "object?", "cyan", "null"),
            new ModalApiRow("Dialog.IsOpen", Lang(ModalShowCaseLangResourceKind.ApiPropertyDialogIsOpen), "bool", "purple", "false"),
            new ModalApiRow("Dialog.IsModal", Lang(ModalShowCaseLangResourceKind.ApiPropertyDialogIsModal), "bool", "purple", "true"),
            new ModalApiRow("Dialog.DialogHostType", Lang(ModalShowCaseLangResourceKind.ApiPropertyDialogDialogHostType), "DialogHostType", "blue", "Overlay"),
            new ModalApiRow("Dialog.StandardButtons", Lang(ModalShowCaseLangResourceKind.ApiPropertyDialogStandardButtons), "DialogStandardButtons", "blue", "NoButton"),
            new ModalApiRow("Dialog.DefaultStandardButton", Lang(ModalShowCaseLangResourceKind.ApiPropertyDialogDefaultStandardButton), "DialogStandardButton", "blue", "NoButton"),
            new ModalApiRow("Dialog.IsLoading", Lang(ModalShowCaseLangResourceKind.ApiPropertyDialogIsLoading), "bool", "purple", "false"),
            new ModalApiRow("Dialog.IsConfirmLoading", Lang(ModalShowCaseLangResourceKind.ApiPropertyDialogIsConfirmLoading), "bool", "purple", "false"),
            new ModalApiRow("Dialog.HostWidth", Lang(ModalShowCaseLangResourceKind.ApiPropertyDialogHostWidth), "double", "cyan", "NaN"),
            new ModalApiRow("Dialog.BeforeCloseAsync / DialogOptions.BeforeCloseAsync", Lang(ModalShowCaseLangResourceKind.ApiPropertyDialogOptionsBeforeCloseAsync), "Func<DialogClosingContext, ValueTask<bool>>?", "cyan", "null"),
            new ModalApiRow("Dialog.ShowDialogModalAsync", Lang(ModalShowCaseLangResourceKind.ApiMethodDialogShowDialogModalAsync), "Task<object?>", "cyan", "-"),
            new ModalApiRow("MessageBox.Style", Lang(ModalShowCaseLangResourceKind.ApiPropertyMessageBoxStyle), "MessageBoxStyle", "blue", "Information"),
            new ModalApiRow("MessageBox.OkButtonStyle", Lang(ModalShowCaseLangResourceKind.ApiPropertyMessageBoxOkButtonStyle), "MessageBoxOkButtonStyle", "blue", "Primary")
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
            new ModalDesignTokenRow("Dialog.HeaderBg", Lang(ModalShowCaseLangResourceKind.TokenNameDialogHeaderBg), Lang(ModalShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ModalShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ModalDesignTokenRow("Dialog.HeaderPadding", Lang(ModalShowCaseLangResourceKind.TokenNameDialogHeaderPadding), Lang(ModalShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ModalShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ModalDesignTokenRow("Dialog.ContentBg", Lang(ModalShowCaseLangResourceKind.TokenNameDialogContentBg), Lang(ModalShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ModalShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ModalDesignTokenRow("Dialog.ContentPadding", Lang(ModalShowCaseLangResourceKind.TokenNameDialogContentPadding), Lang(ModalShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ModalShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ModalDesignTokenRow("Dialog.FooterPadding", Lang(ModalShowCaseLangResourceKind.TokenNameDialogFooterPadding), Lang(ModalShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ModalShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ModalDesignTokenRow("Dialog.CloseBtnSize", Lang(ModalShowCaseLangResourceKind.TokenNameDialogCloseBtnSize), Lang(ModalShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ModalShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ModalDesignTokenRow("Dialog.ButtonGroupSpacing", Lang(ModalShowCaseLangResourceKind.TokenNameDialogButtonGroupSpacing), Lang(ModalShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ModalShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ModalDesignTokenRow("MessageBox.StyleIconSize", Lang(ModalShowCaseLangResourceKind.TokenNameMessageBoxStyleIconSize), Lang(ModalShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ModalShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ModalDesignTokenRow("MessageBox.MinWidth", Lang(ModalShowCaseLangResourceKind.TokenNameMessageBoxMinWidth), Lang(ModalShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ModalShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(ModalShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(ModalShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            ModalShowCaseLangResourceKind.ApiPropertyDialogContent                 => en_US.ApiPropertyDialogContent,
            ModalShowCaseLangResourceKind.ApiPropertyDialogIsOpen                  => en_US.ApiPropertyDialogIsOpen,
            ModalShowCaseLangResourceKind.ApiPropertyDialogIsModal                 => en_US.ApiPropertyDialogIsModal,
            ModalShowCaseLangResourceKind.ApiPropertyDialogDialogHostType          => en_US.ApiPropertyDialogDialogHostType,
            ModalShowCaseLangResourceKind.ApiPropertyDialogStandardButtons         => en_US.ApiPropertyDialogStandardButtons,
            ModalShowCaseLangResourceKind.ApiPropertyDialogDefaultStandardButton   => en_US.ApiPropertyDialogDefaultStandardButton,
            ModalShowCaseLangResourceKind.ApiPropertyDialogIsLoading               => en_US.ApiPropertyDialogIsLoading,
            ModalShowCaseLangResourceKind.ApiPropertyDialogIsConfirmLoading        => en_US.ApiPropertyDialogIsConfirmLoading,
            ModalShowCaseLangResourceKind.ApiPropertyDialogHostWidth               => en_US.ApiPropertyDialogHostWidth,
            ModalShowCaseLangResourceKind.ApiPropertyDialogOptionsBeforeCloseAsync => en_US.ApiPropertyDialogOptionsBeforeCloseAsync,
            ModalShowCaseLangResourceKind.ApiMethodDialogShowDialogModalAsync      => en_US.ApiMethodDialogShowDialogModalAsync,
            ModalShowCaseLangResourceKind.ApiPropertyMessageBoxStyle               => en_US.ApiPropertyMessageBoxStyle,
            ModalShowCaseLangResourceKind.ApiPropertyMessageBoxOkButtonStyle       => en_US.ApiPropertyMessageBoxOkButtonStyle,
            ModalShowCaseLangResourceKind.TokenNameDialogHeaderBg                  => en_US.TokenNameDialogHeaderBg,
            ModalShowCaseLangResourceKind.TokenNameDialogHeaderPadding             => en_US.TokenNameDialogHeaderPadding,
            ModalShowCaseLangResourceKind.TokenNameDialogContentBg                 => en_US.TokenNameDialogContentBg,
            ModalShowCaseLangResourceKind.TokenNameDialogContentPadding            => en_US.TokenNameDialogContentPadding,
            ModalShowCaseLangResourceKind.TokenNameDialogFooterPadding             => en_US.TokenNameDialogFooterPadding,
            ModalShowCaseLangResourceKind.TokenNameDialogCloseBtnSize              => en_US.TokenNameDialogCloseBtnSize,
            ModalShowCaseLangResourceKind.TokenNameDialogButtonGroupSpacing        => en_US.TokenNameDialogButtonGroupSpacing,
            ModalShowCaseLangResourceKind.TokenNameMessageBoxStyleIconSize         => en_US.TokenNameMessageBoxStyleIconSize,
            ModalShowCaseLangResourceKind.TokenNameMessageBoxMinWidth              => en_US.TokenNameMessageBoxMinWidth,
            ModalShowCaseLangResourceKind.TokenScopeComponent                      => en_US.TokenScopeComponent,
            ModalShowCaseLangResourceKind.TokenStatusStable                        => en_US.TokenStatusStable,
            _                                                                      => kind.ToString()
        };
    }
}

public sealed record ModalApiRow(
    string Member,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record ModalDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
