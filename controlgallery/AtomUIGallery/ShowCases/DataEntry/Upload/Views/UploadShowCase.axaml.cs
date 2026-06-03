using System;
using System.Globalization;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Threading;
using System.Threading.Tasks;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Theme.Language;
using Avalonia;
using Avalonia.Controls;
using AtomUIGallery.Localization;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Upload;

public partial class UploadShowCase : ReactiveUserControl<UploadViewModel>
{
    public const string LanguageId = nameof(UploadShowCase);

    private const string BasicScenario       = "Basic";
    private const string PicturesScenario    = "Pictures";
    private const string ConstraintsScenario = "Constraints";

    private readonly Dictionary<string, Control> _scenarioCache = new(StringComparer.Ordinal);

    public UploadShowCase()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            if (DataContext is UploadViewModel vm)
            {
                RefreshLocalizedTaskLists(vm);

                var themeManager = Application.Current?.GetThemeManager();
                if (themeManager != null)
                {
                    EventHandler<LanguageVariantChangedEventArgs> handler = (_, _) => RefreshLocalizedTaskLists(vm);
                    themeManager.LanguageVariantChanged += handler;
                    Disposable.Create(() => themeManager.LanguageVariantChanged -= handler)
                              .DisposeWith(disposables);
                }

                Disposable.Create(() =>
                {
                    vm.DefaultTaskList                 = null;
                    vm.PicturesWallDefaultTaskList     = null;
                    vm.PictureListStyleDefaultTaskList = null;
                }).DisposeWith(disposables);
            }
        });
        ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;
        EnsureSelectedScenarioContent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        foreach (var content in _scenarioCache.Values)
        {
            content.DataContext = DataContext;
        }
    }

    private void HandleScenarioSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        EnsureSelectedScenarioContent();
    }

    private void EnsureSelectedScenarioContent()
    {
        if (ScenarioTabs.SelectedItem is not AtomUI.Desktop.Controls.TabItem tabItem ||
            tabItem.Tag is not string scenario)
        {
            return;
        }

        if (!_scenarioCache.TryGetValue(scenario, out var content))
        {
            content             = CreateScenarioContent(scenario);
            content.DataContext = DataContext;
            _scenarioCache.Add(scenario, content);
        }

        if (tabItem.Content != content)
        {
            tabItem.Content = content;
        }
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            BasicScenario       => new UploadBasicShowCase(),
            PicturesScenario    => new UploadPicturesShowCase(),
            ConstraintsScenario => new UploadConstraintsShowCase(),
            _                   => throw new InvalidOperationException($"Unknown Upload scenario: {scenario}")
        };
    }

    private void RefreshLocalizedTaskLists(UploadViewModel vm)
    {
        InitDefaultTaskList(vm);
        InitPictureWallTaskList(vm);
        InitPictureListTaskList(vm);
    }

    private void InitDefaultTaskList(UploadViewModel vm)
    {
        vm.DefaultTaskList =
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
}

public abstract class UploadScenarioShowCase : ReactiveUserControl<UploadViewModel>
{
    private WindowMessageManager? _messageManager;

    protected IDisposable AttachUpload(
        AtomUI.Desktop.Controls.Upload upload,
        EventHandler<UploadTaskAboutToSchedulingEventArgs>? aboutToScheduling = null)
    {
        upload.UploadTransport     = new UploadMockTransport();
        upload.UploadTaskFailed    += HandleUploadFailed;
        upload.UploadTaskCompleted += HandleUploadCompleted;
        if (aboutToScheduling is not null)
        {
            upload.UploadTaskAboutToScheduling += aboutToScheduling;
        }

        return Disposable.Create(() =>
        {
            upload.UploadTransport     = null;
            upload.UploadTaskFailed    -= HandleUploadFailed;
            upload.UploadTaskCompleted -= HandleUploadCompleted;
            if (aboutToScheduling is not null)
            {
                upload.UploadTaskAboutToScheduling -= aboutToScheduling;
            }
            upload.Reset();
        });
    }

    protected void HandleImageUploadAboutToScheduling(object? sender, UploadTaskAboutToSchedulingEventArgs e)
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

    protected void HandlePngUploadAboutToScheduling(object? sender, UploadTaskAboutToSchedulingEventArgs e)
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

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _messageManager?.Dispose();
        _messageManager = null;
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
