using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Upload;

public partial class UploadShowCase : GalleryReactiveUserControl<UploadViewModel>
{
    public const string LanguageId = nameof(UploadShowCase);

    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;
    private WindowMessageManager? _messageManager;

    public UploadShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);

        this.WhenActivated(disposables =>
        {
            if (DataContext is UploadViewModel viewModel)
            {
                RefreshLocalizedTaskLists(viewModel);

                var themeManager = Application.Current?.GetThemeManager();
                if (themeManager != null)
                {
                    EventHandler<LanguageVariantChangedEventArgs> handler = (_, _) => RefreshLocalizedTaskLists(viewModel);
                    themeManager.LanguageVariantChanged += handler;
                    Disposable.Create(() => themeManager.LanguageVariantChanged -= handler)
                              .DisposeWith(disposables);
                }

                Disposable.Create(() =>
                {
                    viewModel.DefaultTaskList                 = null;
                    viewModel.PicturesWallDefaultTaskList     = null;
                    viewModel.PictureListStyleDefaultTaskList = null;
                }).DisposeWith(disposables);
            }
        });
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _scenarioController.Attach(DataContext);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _scenarioController.Detach();
        _messageManager?.Dispose();
        _messageManager = null;
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
            ApiScenario         => new UploadApiDataGrid(),
            DesignTokenScenario => new UploadDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Upload scenario: {scenario}")
        };
    }

    private void RefreshLocalizedTaskLists(UploadViewModel viewModel)
    {
        InitDefaultTaskList(viewModel);
        InitPictureWallTaskList(viewModel);
        InitPictureListTaskList(viewModel);
    }

    private void InitDefaultTaskList(UploadViewModel viewModel)
    {
        viewModel.DefaultTaskList =
        [
            new UploadTaskInfo()
            {
                TaskId      = Guid.NewGuid(),
                FileName    = "xxx.png",
                IsImageFile = true,
                Status      = FileUploadStatus.Uploading,
                Progress    = 33
            },
            new UploadTaskInfo()
            {
                TaskId      = Guid.NewGuid(),
                FileName    = "yyy.png",
                IsImageFile = true,
                Status      = FileUploadStatus.Success,
                Progress    = 100
            },
            new UploadTaskInfo()
            {
                TaskId       = Guid.NewGuid(),
                FileName     = "zzz.png",
                IsImageFile  = true,
                Status       = FileUploadStatus.Failed,
                ErrorMessage = UploadShowCaseLanguage.Get(
                    UploadShowCaseLangResourceKind.P2ErrorServer500,
                    "Server Error 500")
            },
        ];
    }

    private void InitPictureWallTaskList(UploadViewModel uploadViewModel)
    {
        uploadViewModel.PicturesWallDefaultTaskList = [
            new UploadTaskInfo()
            {
                TaskId      = Guid.NewGuid(),
                FileName    = "image.png",
                IsImageFile = true,
                Status      = FileUploadStatus.Success,
                FilePath    = new Uri("avares://AtomUIGallery/Assets/ImagePreviewerShowCase/1.png")
            },
            new UploadTaskInfo()
            {
                TaskId      = Guid.NewGuid(),
                FileName    = "image.png",
                IsImageFile = true,
                Status      = FileUploadStatus.Success,
                FilePath    = new Uri("avares://AtomUIGallery/Assets/ImagePreviewerShowCase/1.png")
            },
            new UploadTaskInfo()
            {
                TaskId      = Guid.NewGuid(),
                FileName    = "image.png",
                IsImageFile = true,
                Status      = FileUploadStatus.Success,
                FilePath    = new Uri("avares://AtomUIGallery/Assets/ImagePreviewerShowCase/1.png")
            },
            new UploadTaskInfo()
            {
                TaskId      = Guid.NewGuid(),
                FileName    = "image.png",
                IsImageFile = true,
                Status      = FileUploadStatus.Success,
                FilePath    = new Uri("avares://AtomUIGallery/Assets/ImagePreviewerShowCase/1.png")
            },
            new UploadTaskInfo()
            {
                TaskId      = Guid.NewGuid(),
                FileName    = "image.png",
                IsImageFile = true,
                Status      = FileUploadStatus.Uploading,
                Progress    = 50
            },
            new UploadTaskInfo()
            {
                TaskId       = Guid.NewGuid(),
                FileName     = "image.png",
                IsImageFile  = true,
                Status       = FileUploadStatus.Failed,
                ErrorMessage = UploadShowCaseLanguage.Get(
                    UploadShowCaseLangResourceKind.P2ErrorUpload,
                    "Upload error!")
            },
        ];
    }

    private void InitPictureListTaskList(UploadViewModel uploadViewModel)
    {
        uploadViewModel.PictureListStyleDefaultTaskList =
        [
            new UploadTaskInfo()
            {
                TaskId      = Guid.NewGuid(),
                FileName    = "xxx.png",
                IsImageFile = true,
                Status      = FileUploadStatus.Uploading,
                Progress    = 33
            },
            new UploadTaskInfo()
            {
                TaskId      = Guid.NewGuid(),
                FileName    = "yyy.png",
                IsImageFile = true,
                Status      = FileUploadStatus.Success,
                FilePath    = new Uri("avares://AtomUIGallery/Assets/ImagePreviewerShowCase/1.png")
            },
            new UploadTaskInfo()
            {
                TaskId       = Guid.NewGuid(),
                FileName     = "zzz.png",
                IsImageFile  = true,
                Status       = FileUploadStatus.Failed,
                ErrorMessage = UploadShowCaseLanguage.Get(
                    UploadShowCaseLangResourceKind.P2ErrorUpload,
                    "Upload error!")
            },
        ];
    }

    private void HandleImageUploadAboutToScheduling(object? sender, UploadTaskAboutToSchedulingEventArgs e)
    {
        var fileInfo          = e.UploadFileInfo;
        var ext               = Path.GetExtension(fileInfo.FilePath.LocalPath);
        var isAllowedFileType = ext is ".jpeg" or ".jpg" or ".png";
        if (!isAllowedFileType)
        {
            e.Result       = UploadPredicateResult.CancelWithInTaskList;
            e.CancelReason = UploadShowCaseLanguage.Get(
                UploadShowCaseLangResourceKind.P2CancelJpgPngOnly,
                "You can only upload JPG/PNG file!");
            return;
        }

        var isLt2M = (double)fileInfo.Size / 1024 / 1024 < 2;
        if (!isLt2M)
        {
            e.Result       = UploadPredicateResult.CancelWithInTaskList;
            e.CancelReason = UploadShowCaseLanguage.Get(
                UploadShowCaseLangResourceKind.P2CancelImageSize,
                "Image must be smaller than 2MB!");
        }
    }

    private void HandlePngUploadAboutToScheduling(object? sender, UploadTaskAboutToSchedulingEventArgs e)
    {
        var fileInfo = e.UploadFileInfo;
        var ext      = Path.GetExtension(fileInfo.FilePath.LocalPath);
        if (ext != ".png")
        {
            e.Result       = UploadPredicateResult.Cancel;
            e.CancelReason = UploadShowCaseLanguage.Get(
                UploadShowCaseLangResourceKind.P2CancelPngOnly,
                "You can only upload PNG file!");
        }
    }

    private void HandleUploadFailed(object? sender, UploadTaskFailedEventArgs e)
    {
        var errorMsg = e.Result.UserFriendlyMessage;
        GetMessageManager()?.Show(new AtomUIMessage(
            type: MessageType.Error,
            content: $"{errorMsg}"
        ));
    }

    private void HandleUploadCompleted(object? sender, UploadTaskCompletedEventArgs e)
    {
        GetMessageManager()?.Show(new AtomUIMessage(
            type: MessageType.Success,
            content: UploadShowCaseLanguage.Format(
                UploadShowCaseLangResourceKind.P2UploadSuccessFormat,
                "{0} uploaded successfully!",
                e.UploadFileInfo.Name)
        ));
    }

    private WindowMessageManager? GetMessageManager()
    {
        if (_messageManager is not null)
        {
            return _messageManager;
        }

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            return null;
        }

        _messageManager = new WindowMessageManager(topLevel)
        {
            MaxItems = 10
        };
        return _messageManager;
    }
}

internal static class UploadShowCaseLanguage
{
    public static string Get(UploadShowCaseLangResourceKind resourceKind, string fallback)
    {
        return LanguageResourceBinder.GetLangResource(resourceKind) ?? fallback;
    }

    public static string Format(UploadShowCaseLangResourceKind resourceKind, string fallback, params object?[] args)
    {
        return string.Format(CultureInfo.CurrentCulture, Get(resourceKind, fallback), args);
    }
}

public class UploadMockTransport : IFileUploadTransport
{
    public async Task<FileUploadResult> UploadAsync(
        UploadFileInfo fileInfo,
        object? context = null,
        IProgress<FileUploadProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var totalBytes  = fileInfo.Size;
        var bytesSent   = 0L;
        var elapsedTime = TimeSpan.Zero;
        try
        {
            var random        = Random.Shared.Next(1, 10);
            var isServerError = random % 3 == 0;
            var cycle         = 0;
            while (!cancellationToken.IsCancellationRequested && bytesSent < totalBytes)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                var delay = TimeSpan.FromMilliseconds(Random.Shared.Next(300, 1000));
                await Task.Delay(delay, cancellationToken);
                elapsedTime += delay;
                bytesSent += (long)(totalBytes *
                                    ((double)Random.Shared.NextInt64((long)totalBytes / 20, (long)totalBytes / 10) /
                                     totalBytes));
                bytesSent = Math.Min(bytesSent, totalBytes);
                var uploadProgress = new FileUploadProgress()
                {
                    TotalBytes = fileInfo.Size,
                    BytesSent  = bytesSent,
                };
                progress?.Report(uploadProgress);
                if (isServerError && cycle > 2)
                {
                    return FileUploadResult.FailureResult(FileUploadErrorCode.ServerError, "Max number of elements reached for this resource!");
                }

                ++cycle;
            }

            return FileUploadResult.SuccessResult(
                fileInfo.FilePath,
                fileInfo.Size,
                elapsedTime,
                "Success");
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return FileUploadResult.FailureResult(FileUploadErrorCode.Unknown, "Upload failed: " + ex.Message);
        }
    }
}
