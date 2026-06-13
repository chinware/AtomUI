using System.Collections.Generic;
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

    private List<UploadTaskInfo>? _defaultTaskList;

    public List<UploadTaskInfo>? DefaultTaskList
    {
        get => _defaultTaskList;
        set => this.RaiseAndSetIfChanged(ref _defaultTaskList, value);
    }

    private List<UploadTaskInfo>? _picturesWallDefaultTaskList;

    public List<UploadTaskInfo>? PicturesWallDefaultTaskList
    {
        get => _picturesWallDefaultTaskList;
        set => this.RaiseAndSetIfChanged(ref _picturesWallDefaultTaskList, value);
    }

    private List<UploadTaskInfo>? _pictureListStyleDefaultTaskList;

    public List<UploadTaskInfo>? PictureListStyleDefaultTaskList
    {
        get => _pictureListStyleDefaultTaskList;
        set => this.RaiseAndSetIfChanged(ref _pictureListStyleDefaultTaskList, value);
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
            new UploadApiRow("Accepts", Lang(UploadShowCaseLangResourceKind.ApiPropertyAccepts), "IReadOnlyList<string>?", "cyan", "null"),
            new UploadApiRow("ExtraContext", Lang(UploadShowCaseLangResourceKind.ApiPropertyExtraContext), "object?", "cyan", "null"),
            new UploadApiRow("MaxCount", Lang(UploadShowCaseLangResourceKind.ApiPropertyMaxCount), "int", "cyan", "int.MaxValue"),
            new UploadApiRow("IsUploadDirectoryEnabled", Lang(UploadShowCaseLangResourceKind.ApiPropertyIsUploadDirectoryEnabled), "bool", "green", "false"),
            new UploadApiRow("ListType", Lang(UploadShowCaseLangResourceKind.ApiPropertyListType), "UploadListType", "blue", "Text"),
            new UploadApiRow("IsMultipleEnabled", Lang(UploadShowCaseLangResourceKind.ApiPropertyIsMultipleEnabled), "bool", "green", "false"),
            new UploadApiRow("IsOpenFileDialogOnClick", Lang(UploadShowCaseLangResourceKind.ApiPropertyIsOpenFileDialogOnClick), "bool", "green", "true"),
            new UploadApiRow("IsShowUploadList", Lang(UploadShowCaseLangResourceKind.ApiPropertyIsShowUploadList), "bool", "green", "true"),
            new UploadApiRow("IsShowUploadTrigger", Lang(UploadShowCaseLangResourceKind.ApiPropertyIsShowUploadTrigger), "bool", "green", "true"),
            new UploadApiRow("DefaultTaskList", Lang(UploadShowCaseLangResourceKind.ApiPropertyDefaultTaskList), "IList<UploadTaskInfo>?", "cyan", "null"),
            new UploadApiRow("MaxConcurrentTasks", Lang(UploadShowCaseLangResourceKind.ApiPropertyMaxConcurrentTasks), "int", "cyan", "3"),
            new UploadApiRow("UploadTransport", Lang(UploadShowCaseLangResourceKind.ApiPropertyUploadTransport), "IFileUploadTransport?", "cyan", "null")
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
            UploadShowCaseLangResourceKind.ApiPropertyAccepts                    => en_US.ApiPropertyAccepts,
            UploadShowCaseLangResourceKind.ApiPropertyExtraContext               => en_US.ApiPropertyExtraContext,
            UploadShowCaseLangResourceKind.ApiPropertyMaxCount                   => en_US.ApiPropertyMaxCount,
            UploadShowCaseLangResourceKind.ApiPropertyIsUploadDirectoryEnabled   => en_US.ApiPropertyIsUploadDirectoryEnabled,
            UploadShowCaseLangResourceKind.ApiPropertyListType                   => en_US.ApiPropertyListType,
            UploadShowCaseLangResourceKind.ApiPropertyIsMultipleEnabled          => en_US.ApiPropertyIsMultipleEnabled,
            UploadShowCaseLangResourceKind.ApiPropertyIsOpenFileDialogOnClick    => en_US.ApiPropertyIsOpenFileDialogOnClick,
            UploadShowCaseLangResourceKind.ApiPropertyIsShowUploadList           => en_US.ApiPropertyIsShowUploadList,
            UploadShowCaseLangResourceKind.ApiPropertyIsShowUploadTrigger        => en_US.ApiPropertyIsShowUploadTrigger,
            UploadShowCaseLangResourceKind.ApiPropertyDefaultTaskList            => en_US.ApiPropertyDefaultTaskList,
            UploadShowCaseLangResourceKind.ApiPropertyMaxConcurrentTasks         => en_US.ApiPropertyMaxConcurrentTasks,
            UploadShowCaseLangResourceKind.ApiPropertyUploadTransport            => en_US.ApiPropertyUploadTransport,
            UploadShowCaseLangResourceKind.TokenNameActionsColor                 => en_US.TokenNameActionsColor,
            UploadShowCaseLangResourceKind.TokenNamePictureCardSize              => en_US.TokenNamePictureCardSize,
            UploadShowCaseLangResourceKind.TokenNameTextListItemMargin           => en_US.TokenNameTextListItemMargin,
            UploadShowCaseLangResourceKind.TokenNameTextListNamePadding          => en_US.TokenNameTextListNamePadding,
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
