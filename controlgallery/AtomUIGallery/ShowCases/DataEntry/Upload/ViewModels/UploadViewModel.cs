using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Upload;

public class UploadViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Upload";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    public IFileUploadTransport UploadTransport { get; } = new UploadMockTransport();

    private ObservableCollection<UploadApiRow>? _apiRows;
    private ObservableCollection<UploadDesignTokenRow>? _designTokenRows;

    public ObservableCollection<UploadApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<UploadDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    private ObservableCollection<UploadFileItem>? _defaultFiles;

    public ObservableCollection<UploadFileItem>? DefaultFiles
    {
        get => _defaultFiles;
        set => this.RaiseAndSetIfChanged(ref _defaultFiles, value);
    }

    private ObservableCollection<UploadFileItem>? _picturesWallFiles;

    public ObservableCollection<UploadFileItem>? PicturesWallFiles
    {
        get => _picturesWallFiles;
        set => this.RaiseAndSetIfChanged(ref _picturesWallFiles, value);
    }

    private ObservableCollection<UploadFileItem>? _pictureCircleFiles;

    public ObservableCollection<UploadFileItem>? PictureCircleFiles
    {
        get => _pictureCircleFiles;
        set => this.RaiseAndSetIfChanged(ref _pictureCircleFiles, value);
    }

    private ObservableCollection<UploadFileItem>? _pictureListStyleFiles;

    public ObservableCollection<UploadFileItem>? PictureListStyleFiles
    {
        get => _pictureListStyleFiles;
        set => this.RaiseAndSetIfChanged(ref _pictureListStyleFiles, value);
    }

    private ObservableCollection<UploadFileItem>? _scrollableUploadFiles;

    public ObservableCollection<UploadFileItem>? ScrollableUploadFiles
    {
        get => _scrollableUploadFiles;
        set => this.RaiseAndSetIfChanged(ref _scrollableUploadFiles, value);
    }

    public UploadViewModel(IScreen screen)
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
            new UploadApiRow("Files", Lang(UploadShowCaseLangResourceKind.ApiPropertyFiles), "IList<UploadFileItem>?", "cyan", "null"),
            new UploadApiRow("Accepts", Lang(UploadShowCaseLangResourceKind.ApiPropertyAccepts), "IReadOnlyList<string>?", "cyan", "null"),
            new UploadApiRow("ExtraContext", Lang(UploadShowCaseLangResourceKind.ApiPropertyExtraContext), "object?", "cyan", "null"),
            new UploadApiRow("MaxCount", Lang(UploadShowCaseLangResourceKind.ApiPropertyMaxCount), "int", "cyan", "int.MaxValue"),
            new UploadApiRow("MaxConcurrentTasks", Lang(UploadShowCaseLangResourceKind.ApiPropertyMaxConcurrentTasks), "int", "cyan", "3"),
            new UploadApiRow("AutoUpload", Lang(UploadShowCaseLangResourceKind.ApiPropertyAutoUpload), "bool", "green", "true"),
            new UploadApiRow("UploadTransport", Lang(UploadShowCaseLangResourceKind.ApiPropertyUploadTransport), "IFileUploadTransport?", "cyan", "null"),
            new UploadApiRow("ListType", Lang(UploadShowCaseLangResourceKind.ApiPropertyListType), "UploadListType", "blue", "Text"),
            new UploadApiRow("IsMultipleEnabled", Lang(UploadShowCaseLangResourceKind.ApiPropertyIsMultipleEnabled), "bool", "green", "false"),
            new UploadApiRow("IsOpenFileDialogOnClick", Lang(UploadShowCaseLangResourceKind.ApiPropertyIsOpenFileDialogOnClick), "bool", "green", "true"),
            new UploadApiRow("IsShowUploadList", Lang(UploadShowCaseLangResourceKind.ApiPropertyIsShowUploadList), "bool", "green", "true"),
            new UploadApiRow("ListMaxHeight", Lang(UploadShowCaseLangResourceKind.ApiPropertyListMaxHeight), "double", "cyan", "Infinity"),
            new UploadApiRow("ListScrollBarVisibility", Lang(UploadShowCaseLangResourceKind.ApiPropertyListScrollBarVisibility), "ScrollBarVisibility", "blue", "Disabled"),
            new UploadApiRow("SuccessAutoRemoveDelay", Lang(UploadShowCaseLangResourceKind.ApiPropertySuccessAutoRemoveDelay), "TimeSpan?", "cyan", "null"),
            new UploadApiRow("PendingText", Lang(UploadShowCaseLangResourceKind.ApiPropertyPendingText), "string?", "cyan", "null"),
            new UploadApiRow("FileValueMode", Lang(UploadShowCaseLangResourceKind.ApiPropertyFileValueMode), "UploadFileValueMode", "blue", "SuccessfulFiles"),
            new UploadApiRow("TriggerContent", Lang(UploadShowCaseLangResourceKind.ApiPropertyTriggerContent), "object?", "cyan", "null"),
            new UploadApiRow("UploadTrigger.SourceKind", Lang(UploadShowCaseLangResourceKind.ApiPropertyUploadTriggerSourceKind), "UploadSourceKind", "blue", "Files")
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
            new UploadDesignTokenRow("ActionsColor", Lang(UploadShowCaseLangResourceKind.TokenNameActionsColor), Lang(UploadShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(UploadShowCaseLangResourceKind.TokenStatusStable), "success"),
            new UploadDesignTokenRow("PictureCardSize", Lang(UploadShowCaseLangResourceKind.TokenNamePictureCardSize), Lang(UploadShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(UploadShowCaseLangResourceKind.TokenStatusStable), "success"),
            new UploadDesignTokenRow("TextListItemMargin", Lang(UploadShowCaseLangResourceKind.TokenNameTextListItemMargin), Lang(UploadShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(UploadShowCaseLangResourceKind.TokenStatusStable), "success"),
            new UploadDesignTokenRow("TextListNamePadding", Lang(UploadShowCaseLangResourceKind.TokenNameTextListNamePadding), Lang(UploadShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(UploadShowCaseLangResourceKind.TokenStatusStable), "success"),
            new UploadDesignTokenRow("TextListProgressPadding", Lang(UploadShowCaseLangResourceKind.TokenNameTextListProgressPadding), Lang(UploadShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(UploadShowCaseLangResourceKind.TokenStatusStable), "success"),
            new UploadDesignTokenRow("UploadThumbnailSize", Lang(UploadShowCaseLangResourceKind.TokenNameUploadThumbnailSize), Lang(UploadShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(UploadShowCaseLangResourceKind.TokenStatusStable), "success"),
            new UploadDesignTokenRow("DragIconSize", Lang(UploadShowCaseLangResourceKind.TokenNameDragIconSize), Lang(UploadShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(UploadShowCaseLangResourceKind.TokenStatusStable), "success"),
            new UploadDesignTokenRow("DragIconMargin", Lang(UploadShowCaseLangResourceKind.TokenNameDragIconMargin), Lang(UploadShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(UploadShowCaseLangResourceKind.TokenStatusStable), "success"),
            new UploadDesignTokenRow("DragHeaderMargin", Lang(UploadShowCaseLangResourceKind.TokenNameDragHeaderMargin), Lang(UploadShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(UploadShowCaseLangResourceKind.TokenStatusStable), "success"),
            new UploadDesignTokenRow("PictureListItemMargin", Lang(UploadShowCaseLangResourceKind.TokenNamePictureListItemMargin), Lang(UploadShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(UploadShowCaseLangResourceKind.TokenStatusStable), "success"),
            new UploadDesignTokenRow("PictureListPreviewerSize", Lang(UploadShowCaseLangResourceKind.TokenNamePictureListPreviewerSize), Lang(UploadShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(UploadShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(UploadShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(UploadShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            UploadShowCaseLangResourceKind.ApiPropertyFiles                      => en_US.ApiPropertyFiles,
            UploadShowCaseLangResourceKind.ApiPropertyAccepts                    => en_US.ApiPropertyAccepts,
            UploadShowCaseLangResourceKind.ApiPropertyExtraContext               => en_US.ApiPropertyExtraContext,
            UploadShowCaseLangResourceKind.ApiPropertyMaxCount                   => en_US.ApiPropertyMaxCount,
            UploadShowCaseLangResourceKind.ApiPropertyMaxConcurrentTasks         => en_US.ApiPropertyMaxConcurrentTasks,
            UploadShowCaseLangResourceKind.ApiPropertyAutoUpload                 => en_US.ApiPropertyAutoUpload,
            UploadShowCaseLangResourceKind.ApiPropertyUploadTransport            => en_US.ApiPropertyUploadTransport,
            UploadShowCaseLangResourceKind.ApiPropertyListType                   => en_US.ApiPropertyListType,
            UploadShowCaseLangResourceKind.ApiPropertyIsMultipleEnabled          => en_US.ApiPropertyIsMultipleEnabled,
            UploadShowCaseLangResourceKind.ApiPropertyIsOpenFileDialogOnClick    => en_US.ApiPropertyIsOpenFileDialogOnClick,
            UploadShowCaseLangResourceKind.ApiPropertyIsShowUploadList           => en_US.ApiPropertyIsShowUploadList,
            UploadShowCaseLangResourceKind.ApiPropertyListMaxHeight              => en_US.ApiPropertyListMaxHeight,
            UploadShowCaseLangResourceKind.ApiPropertyListScrollBarVisibility    => en_US.ApiPropertyListScrollBarVisibility,
            UploadShowCaseLangResourceKind.ApiPropertySuccessAutoRemoveDelay     => en_US.ApiPropertySuccessAutoRemoveDelay,
            UploadShowCaseLangResourceKind.ApiPropertyPendingText                => en_US.ApiPropertyPendingText,
            UploadShowCaseLangResourceKind.ApiPropertyFileValueMode              => en_US.ApiPropertyFileValueMode,
            UploadShowCaseLangResourceKind.ApiPropertyTriggerContent             => en_US.ApiPropertyTriggerContent,
            UploadShowCaseLangResourceKind.ApiPropertyUploadTriggerSourceKind    => en_US.ApiPropertyUploadTriggerSourceKind,
            UploadShowCaseLangResourceKind.TokenNameActionsColor                 => en_US.TokenNameActionsColor,
            UploadShowCaseLangResourceKind.TokenNamePictureCardSize              => en_US.TokenNamePictureCardSize,
            UploadShowCaseLangResourceKind.TokenNameTextListItemMargin           => en_US.TokenNameTextListItemMargin,
            UploadShowCaseLangResourceKind.TokenNameTextListNamePadding          => en_US.TokenNameTextListNamePadding,
            UploadShowCaseLangResourceKind.TokenNameTextListProgressPadding      => en_US.TokenNameTextListProgressPadding,
            UploadShowCaseLangResourceKind.TokenNameUploadThumbnailSize          => en_US.TokenNameUploadThumbnailSize,
            UploadShowCaseLangResourceKind.TokenNameDragIconSize                 => en_US.TokenNameDragIconSize,
            UploadShowCaseLangResourceKind.TokenNameDragIconMargin               => en_US.TokenNameDragIconMargin,
            UploadShowCaseLangResourceKind.TokenNameDragHeaderMargin             => en_US.TokenNameDragHeaderMargin,
            UploadShowCaseLangResourceKind.TokenNamePictureListItemMargin        => en_US.TokenNamePictureListItemMargin,
            UploadShowCaseLangResourceKind.TokenNamePictureListPreviewerSize     => en_US.TokenNamePictureListPreviewerSize,
            UploadShowCaseLangResourceKind.TokenScopeComponent                   => en_US.TokenScopeComponent,
            UploadShowCaseLangResourceKind.TokenStatusStable                     => en_US.TokenStatusStable,
            _                                                                    => kind.ToString()
        };
    }
}

public sealed record UploadApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record UploadDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
