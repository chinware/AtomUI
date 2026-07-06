using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text.RegularExpressions;
using AtomUI.Controls;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

public partial class Upload : ContentControl,
                              IMotionAwareControl,
                              IFormItemAware
{
    #region 公共属性定义

    public static readonly StyledProperty<IList<UploadFileItem>?> FilesProperty =
        AvaloniaProperty.Register<Upload, IList<UploadFileItem>?>(
            nameof(Files),
            defaultBindingMode: BindingMode.TwoWay,
            enableDataValidation: true);

    public static readonly StyledProperty<IReadOnlyList<string>?> AcceptsProperty =
        AvaloniaProperty.Register<Upload, IReadOnlyList<string>?>(nameof(Accepts));

    public static readonly StyledProperty<object?> ExtraContextProperty =
        AvaloniaProperty.Register<Upload, object?>(nameof(ExtraContext));

    public static readonly StyledProperty<int> MaxCountProperty =
        AvaloniaProperty.Register<Upload, int>(nameof(MaxCount), int.MaxValue);

    public static readonly StyledProperty<int> MaxConcurrentTasksProperty =
        AvaloniaProperty.Register<Upload, int>(nameof(MaxConcurrentTasks), 3);

    public static readonly StyledProperty<bool> AutoUploadProperty =
        AvaloniaProperty.Register<Upload, bool>(nameof(AutoUpload), true);

    public static readonly StyledProperty<UploadListType> ListTypeProperty =
        AvaloniaProperty.Register<Upload, UploadListType>(nameof(ListType));

    public static readonly StyledProperty<bool> IsMultipleEnabledProperty =
        AvaloniaProperty.Register<Upload, bool>(nameof(IsMultipleEnabled));

    public static readonly StyledProperty<bool> IsOpenFileDialogOnClickProperty =
        AvaloniaProperty.Register<Upload, bool>(nameof(IsOpenFileDialogOnClick), true);

    public static readonly StyledProperty<bool> IsShowUploadListProperty =
        AvaloniaProperty.Register<Upload, bool>(nameof(IsShowUploadList), true);

    public static readonly StyledProperty<double> ListMaxHeightProperty =
        AvaloniaProperty.Register<Upload, double>(
            nameof(ListMaxHeight),
            double.PositiveInfinity);

    public static readonly StyledProperty<ScrollBarVisibility> ListScrollBarVisibilityProperty =
        AvaloniaProperty.Register<Upload, ScrollBarVisibility>(
            nameof(ListScrollBarVisibility),
            ScrollBarVisibility.Disabled);

    public static readonly StyledProperty<TimeSpan?> SuccessAutoRemoveDelayProperty =
        AvaloniaProperty.Register<Upload, TimeSpan?>(nameof(SuccessAutoRemoveDelay));

    public static readonly StyledProperty<string?> PendingTextProperty =
        AvaloniaProperty.Register<Upload, string?>(nameof(PendingText));

    public static readonly StyledProperty<UploadFileValueMode> FileValueModeProperty =
        AvaloniaProperty.Register<Upload, UploadFileValueMode>(
            nameof(FileValueMode),
            UploadFileValueMode.SuccessfulFiles);

    public static readonly StyledProperty<object?> TriggerContentProperty =
        AvaloniaProperty.Register<Upload, object?>(nameof(TriggerContent));

    public static readonly StyledProperty<IDataTemplate?> TriggerContentTemplateProperty =
        AvaloniaProperty.Register<Upload, IDataTemplate?>(nameof(TriggerContentTemplate));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Upload>();

    public static readonly DirectProperty<Upload, IFileUploadTransport?> UploadTransportProperty =
        AvaloniaProperty.RegisterDirect<Upload, IFileUploadTransport?>(
            nameof(UploadTransport),
            o => o.UploadTransport,
            (o, v) => o.UploadTransport = v);

    public static readonly DirectProperty<Upload, bool> IsTaskRunningProperty =
        AvaloniaProperty.RegisterDirect<Upload, bool>(
            nameof(IsTaskRunning),
            o => o.IsTaskRunning,
            (o, v) => o.IsTaskRunning = v);

    public IList<UploadFileItem>? Files
    {
        get => GetValue(FilesProperty);
        set => SetValue(FilesProperty, value);
    }

    public IReadOnlyList<string>? Accepts
    {
        get => GetValue(AcceptsProperty);
        set => SetValue(AcceptsProperty, value);
    }

    public object? ExtraContext
    {
        get => GetValue(ExtraContextProperty);
        set => SetValue(ExtraContextProperty, value);
    }

    public int MaxCount
    {
        get => GetValue(MaxCountProperty);
        set => SetValue(MaxCountProperty, value);
    }

    public int MaxConcurrentTasks
    {
        get => GetValue(MaxConcurrentTasksProperty);
        set => SetValue(MaxConcurrentTasksProperty, value);
    }

    public bool AutoUpload
    {
        get => GetValue(AutoUploadProperty);
        set => SetValue(AutoUploadProperty, value);
    }

    public UploadListType ListType
    {
        get => GetValue(ListTypeProperty);
        set => SetValue(ListTypeProperty, value);
    }

    public bool IsMultipleEnabled
    {
        get => GetValue(IsMultipleEnabledProperty);
        set => SetValue(IsMultipleEnabledProperty, value);
    }

    public bool IsOpenFileDialogOnClick
    {
        get => GetValue(IsOpenFileDialogOnClickProperty);
        set => SetValue(IsOpenFileDialogOnClickProperty, value);
    }

    public bool IsShowUploadList
    {
        get => GetValue(IsShowUploadListProperty);
        set => SetValue(IsShowUploadListProperty, value);
    }

    public double ListMaxHeight
    {
        get => GetValue(ListMaxHeightProperty);
        set => SetValue(ListMaxHeightProperty, value);
    }

    public ScrollBarVisibility ListScrollBarVisibility
    {
        get => GetValue(ListScrollBarVisibilityProperty);
        set => SetValue(ListScrollBarVisibilityProperty, value);
    }

    public TimeSpan? SuccessAutoRemoveDelay
    {
        get => GetValue(SuccessAutoRemoveDelayProperty);
        set => SetValue(SuccessAutoRemoveDelayProperty, value);
    }

    public string? PendingText
    {
        get => GetValue(PendingTextProperty);
        set => SetValue(PendingTextProperty, value);
    }

    public UploadFileValueMode FileValueMode
    {
        get => GetValue(FileValueModeProperty);
        set => SetValue(FileValueModeProperty, value);
    }

    [DependsOn(nameof(TriggerContentTemplate))]
    public object? TriggerContent
    {
        get => GetValue(TriggerContentProperty);
        set => SetValue(TriggerContentProperty, value);
    }

    public IDataTemplate? TriggerContentTemplate
    {
        get => GetValue(TriggerContentTemplateProperty);
        set => SetValue(TriggerContentTemplateProperty, value);
    }

    private IFileUploadTransport? _uploadTransport;

    public IFileUploadTransport? UploadTransport
    {
        get => _uploadTransport;
        set => SetAndRaise(UploadTransportProperty, ref _uploadTransport, value);
    }

    private bool _isTaskRunning;

    public bool IsTaskRunning
    {
        get => _isTaskRunning;
        set => SetAndRaise(IsTaskRunningProperty, ref _isTaskRunning, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    #region 公共事件定义

    public event EventHandler<UploadTaskCreatedEventArgs>? UploadTaskCreated;
    public event EventHandler<UploadTaskAboutToSchedulingEventArgs>? UploadTaskAboutToScheduling;
    public event EventHandler<UploadTaskProgressEventArgs>? UploadTaskProgress;
    public event EventHandler<UploadTaskCompletedEventArgs>? UploadTaskCompleted;
    public event EventHandler<UploadTaskCancelledEventArgs>? UploadTaskCancelled;
    public event EventHandler<UploadTaskFailedEventArgs>? UploadTaskFailed;
    public event EventHandler<UploadTaskRemovedEventArgs>? UploadTaskRemoved;

    #endregion

    #region 公共回调函数定义

    public Func<UploadFileInfo, bool>? IsImageFilePredicate { get; set; }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<Upload, IList<UploadFileItem>> EffectiveFilesProperty =
        AvaloniaProperty.RegisterDirect<Upload, IList<UploadFileItem>>(
            nameof(EffectiveFiles),
            o => o.EffectiveFiles);

    internal static readonly DirectProperty<Upload, IList<object?>> EffectivePictureItemsProperty =
        AvaloniaProperty.RegisterDirect<Upload, IList<object?>>(
            nameof(EffectivePictureItems),
            o => o.EffectivePictureItems);

    internal IList<UploadFileItem> EffectiveFiles
    {
        get => _effectiveFiles;
        private set => SetAndRaise(EffectiveFilesProperty, ref _effectiveFiles, value);
    }

    internal IList<object?> EffectivePictureItems => _effectivePictureItems;

    #endregion

    private static readonly Regex ImageExtensionRegex =
        new(@"\.(webp|svg|png|gif|jpg|jpeg|jfif|bmp|dpg|ico|heic|heif)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private readonly AvaloniaList<UploadFileItem> _ownedFiles = new();
    private readonly AvaloniaList<object?> _effectivePictureItems = new();
    private readonly UploadAppendContentItem _appendContentItem = new();
    private readonly HashSet<UploadFileItem> _observedFileItems = new();
    private readonly Dictionary<Guid, CancellationTokenSource> _successAutoRemoveDelays = new();
    private readonly UploadQueue _uploadQueue;
    private IList<UploadFileItem> _effectiveFiles = null!;
    private INotifyCollectionChanged? _attachedFileCollection;
    private EventHandler? _formValueChanged;

    static Upload()
    {
        AbstractUploadListItem.TaskRemoveRequestEvent.AddClassHandler<Upload>(
            (upload, args) => upload.HandleTaskRemoveRequest(args));
    }

    public Upload()
    {
        this.RegisterTokenResourceScope(UploadToken.ScopeProvider);
        _effectiveFiles = _ownedFiles;
        _uploadQueue = new UploadQueue(this, UploadTransport, MaxConcurrentTasks);
        SyncAppendContentItem();
        AttachFileCollection(EffectiveFiles);
        SyncEffectivePictureItems();
    }

    public async Task EnqueueFilesAsync(IEnumerable<UploadFileInfo> files, CancellationToken cancellationToken = default)
    {
        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await EnqueueFileAsync(file, cancellationToken);
        }
    }

    public async Task RemoveFileAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var files = EffectiveFiles;
        var item = FindFile(id);
        if (item is null)
        {
            return;
        }

        await _uploadQueue.CancelAsync(id, cancellationToken);
        CancelSuccessAutoRemove(id);
        DetachFileItem(item);
        files.Remove(item);
        if (files is not INotifyCollectionChanged)
        {
            RemoveEffectivePictureItem(item);
        }
        NotifyFormValueChanged(files);
        UploadTaskRemoved?.Invoke(this, new UploadTaskRemovedEventArgs(id, CreateUploadFileInfo(item)));
    }

    public async Task ResetAsync(CancellationToken cancellationToken = default)
    {
        await _uploadQueue.CancelAllAsync(cancellationToken);
        ClearEffectiveFiles();
    }

    public void Reset()
    {
        Dispatcher.InvokeAsync(async () => await ResetAsync());
    }

    private async void HandleTaskRemoveRequest(TaskRemoveRequestEventArgs args)
    {
        if (args.Handled)
        {
            return;
        }

        args.Handled = true;
        await RemoveFileAsync(args.TaskId);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        Dispatcher.InvokeAsync(async () => await _uploadQueue.CancelAllAsync());
        CancelAllSuccessAutoRemove();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == FilesProperty)
        {
            var typedChange = (AvaloniaPropertyChangedEventArgs<IList<UploadFileItem>?>)change;
            HandleFilesChanged(typedChange.OldValue.Value, typedChange.NewValue.Value);
        }
        else if (change.Property == TriggerContentProperty ||
                 change.Property == TriggerContentTemplateProperty ||
                 change.Property == HorizontalContentAlignmentProperty ||
                 change.Property == VerticalContentAlignmentProperty)
        {
            SyncAppendContentItem();
        }
        else if (change.Property == UploadTransportProperty)
        {
            Dispatcher.InvokeAsync(async () => await _uploadQueue.SetTransportAsync(UploadTransport));
        }
        else if (change.Property == MaxConcurrentTasksProperty)
        {
            Dispatcher.InvokeAsync(async () => await _uploadQueue.SetMaxConcurrentTasksAsync(MaxConcurrentTasks));
        }
    }

    #region 实现 FormItem 接口

    event EventHandler? IFormItemAware.ValueChanged
    {
        add => _formValueChanged += value;
        remove => _formValueChanged -= value;
    }

    void IFormItemAware.SetFormValue(object? value) => NotifySetFormValue(value);

    object? IFormItemAware.GetFormValue() => NotifyGetFormValue();

    void IFormItemAware.ClearFormValue() => NotifyClearFormValue();

    void IFormItemAware.NotifyValidateStatus(FormValidateStatus status) => NotifyValidateStatus(status);

    protected virtual void NotifyFormValueChanged(object? value)
    {
        _formValueChanged?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void NotifySetFormValue(object? value)
    {
        CancelAllSuccessAutoRemove();
        CancelAllUploadQueue();

        var files = EffectiveFiles;
        foreach (var item in files.ToArray())
        {
            DetachFileItem(item);
        }
        files.Clear();

        if (value is IEnumerable<UploadFileItem> items)
        {
            foreach (var item in items)
            {
                AttachFileItem(item);
                files.Add(item);
            }
        }

        SyncEffectivePictureItems();
        NotifyFormValueChanged(files);
    }

    protected virtual object? NotifyGetFormValue()
    {
        return FileValueMode switch
        {
            UploadFileValueMode.AllFiles => EffectiveFiles.ToArray(),
            UploadFileValueMode.Results => EffectiveFiles
                                           .Where(file => file.Status == FileUploadStatus.Success)
                                           .Select(file => file.Result)
                                           .Where(result => result is not null)
                                           .ToArray(),
            _ => EffectiveFiles
                 .Where(file => file.Status == FileUploadStatus.Success)
                 .ToArray()
        };
    }

    protected virtual void NotifyClearFormValue()
    {
        if (Dispatcher.CheckAccess())
        {
            ClearEffectiveFiles();
            Dispatcher.InvokeAsync(async () => await _uploadQueue.CancelAllAsync());
        }
        else
        {
            Dispatcher.InvokeAsync(ClearEffectiveFiles);
            Dispatcher.InvokeAsync(async () => await _uploadQueue.CancelAllAsync());
        }
    }

    protected virtual void NotifyValidateStatus(FormValidateStatus status)
    {
    }

    #endregion

    internal void NotifyUploadProgress(UploadFileItem item, UploadFileInfo fileInfo, double progress)
    {
        RunOnUiThread(() =>
        {
            item.Status   = FileUploadStatus.Uploading;
            item.Progress = progress;
            SetCurrentValue(IsTaskRunningProperty, true);
            UploadTaskProgress?.Invoke(this, new UploadTaskProgressEventArgs(item.Id, fileInfo, progress));
        });
    }

    internal void NotifyUploadCompleted(UploadFileItem item, UploadFileInfo fileInfo, FileUploadResult result)
    {
        RunOnUiThread(() =>
        {
            item.Status = FileUploadStatus.Success;
            item.Result = result;
            UploadTaskCompleted?.Invoke(this, new UploadTaskCompletedEventArgs(item.Id, fileInfo, result));
            ScheduleSuccessAutoRemove(item);
            UpdateTaskRunning();
        });
    }

    internal void NotifyUploadFailed(UploadFileItem item, UploadFileInfo fileInfo, FileUploadResult result)
    {
        RunOnUiThread(() =>
        {
            item.Status       = FileUploadStatus.Failed;
            item.ErrorMessage = result.UserFriendlyMessage;
            item.Result       = result;
            UploadTaskFailed?.Invoke(this, new UploadTaskFailedEventArgs(item.Id, fileInfo, result));
            UpdateTaskRunning();
        });
    }

    internal void NotifyUploadCancelled(UploadFileItem item, UploadFileInfo fileInfo, FileUploadResult result)
    {
        RunOnUiThread(() =>
        {
            item.Status       = FileUploadStatus.Cancelled;
            item.ErrorMessage = result.UserFriendlyMessage;
            item.Result       = result;
            UploadTaskCancelled?.Invoke(this, new UploadTaskCancelledEventArgs(item.Id, fileInfo, result));
            UpdateTaskRunning();
        });
    }

    private async Task EnqueueFileAsync(UploadFileInfo file, CancellationToken cancellationToken)
    {
        var files = EffectiveFiles;
        if (files.Count >= MaxCount)
        {
            if (MaxCount != 1)
            {
                return;
            }
            await ResetAsync(cancellationToken);
        }

        var item = new UploadFileItem
        {
            Name        = file.Name,
            Path        = file.FilePath,
            Size        = file.Size,
            PendingText = PendingText,
            IsImageFile = IsImageFile(file),
            UserData    = ExtraContext
        };

        AttachFileItem(item);
        files.Add(item);
        if (files is not INotifyCollectionChanged)
        {
            InsertEffectivePictureItem(item, files.IndexOf(item));
        }
        NotifyFormValueChanged(files);
        UploadTaskCreated?.Invoke(this, new UploadTaskCreatedEventArgs(item.Id, file));

        var aboutToSchedulingEvent = new UploadTaskAboutToSchedulingEventArgs(item.Id, file);
        UploadTaskAboutToScheduling?.Invoke(this, aboutToSchedulingEvent);
        if (aboutToSchedulingEvent.Result == UploadPredicateResult.Cancel)
        {
            item.Status       = FileUploadStatus.Failed;
            item.ErrorMessage = aboutToSchedulingEvent.CancelReason;
            return;
        }

        if (AutoUpload && aboutToSchedulingEvent.Result == UploadPredicateResult.Schedule)
        {
            _uploadQueue.Enqueue(item, file);
        }
        else if (aboutToSchedulingEvent.Result == UploadPredicateResult.CancelWithInTaskList)
        {
            item.Status       = FileUploadStatus.Failed;
            item.ErrorMessage = aboutToSchedulingEvent.CancelReason;
        }
    }

    private void HandleFilesChanged(IList<UploadFileItem>? oldFiles, IList<UploadFileItem>? newFiles)
    {
        CancelAllSuccessAutoRemove();
        CancelAllUploadQueue();
        DetachFileCollection(oldFiles ?? _ownedFiles);
        EffectiveFiles = newFiles ?? _ownedFiles;
        AttachFileCollection(EffectiveFiles);
        SyncEffectivePictureItems();
        NotifyFormValueChanged(EffectiveFiles);
    }

    private void AttachFileCollection(IList<UploadFileItem> files)
    {
        if (files is INotifyCollectionChanged notifyCollectionChanged)
        {
            _attachedFileCollection = notifyCollectionChanged;
            notifyCollectionChanged.CollectionChanged += HandleFilesCollectionChanged;
        }

        foreach (var item in files)
        {
            AttachFileItem(item);
        }
    }

    private void DetachFileCollection(IList<UploadFileItem> files)
    {
        if (_attachedFileCollection is not null)
        {
            _attachedFileCollection.CollectionChanged -= HandleFilesCollectionChanged;
            _attachedFileCollection = null;
        }

        foreach (var item in files)
        {
            DetachFileItem(item);
        }
    }

    private void HandleFilesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (var item in e.OldItems.OfType<UploadFileItem>())
            {
                DetachFileItem(item);
                CancelSuccessAutoRemove(item.Id);
                CancelUploadQueue(item.Id);
            }
        }

        if (e.NewItems is not null)
        {
            foreach (var item in e.NewItems.OfType<UploadFileItem>())
            {
                AttachFileItem(item);
            }
        }

        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            CancelAllSuccessAutoRemove();
            CancelAllUploadQueue();
            foreach (var item in _observedFileItems.ToArray())
            {
                DetachFileItem(item);
            }
            foreach (var item in EffectiveFiles)
            {
                AttachFileItem(item);
            }
        }

        SyncEffectivePictureItems(e);
        NotifyFormValueChanged(EffectiveFiles);
    }

    private void AttachFileItem(UploadFileItem item)
    {
        if (_observedFileItems.Add(item))
        {
            item.PropertyChanged += HandleFileItemPropertyChanged;
        }
    }

    private void DetachFileItem(UploadFileItem item)
    {
        if (_observedFileItems.Remove(item))
        {
            item.PropertyChanged -= HandleFileItemPropertyChanged;
        }
    }

    private void HandleFileItemPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender is not UploadFileItem item || e.Property != UploadFileItem.StatusProperty)
        {
            return;
        }

        if (item.Status == FileUploadStatus.Success)
        {
            ScheduleSuccessAutoRemove(item);
        }
        else
        {
            CancelSuccessAutoRemove(item.Id);
        }

        UpdateTaskRunning();
    }

    private UploadFileItem? FindFile(Guid id)
    {
        return EffectiveFiles.FirstOrDefault(file => file.Id == id);
    }

    private void ClearEffectiveFiles()
    {
        CancelAllSuccessAutoRemove();
        foreach (var item in EffectiveFiles.ToArray())
        {
            DetachFileItem(item);
        }
        EffectiveFiles.Clear();
        SyncEffectivePictureItems();
        NotifyFormValueChanged(EffectiveFiles);
    }

    private void SyncAppendContentItem()
    {
        _appendContentItem.Content                    = TriggerContent;
        _appendContentItem.ContentTemplate            = TriggerContentTemplate;
        _appendContentItem.HorizontalContentAlignment = HorizontalContentAlignment;
        _appendContentItem.VerticalContentAlignment   = VerticalContentAlignment;
        _appendContentItem.IsVisible                  = TriggerContent is not null;
    }

    private void SyncEffectivePictureItems()
    {
        _effectivePictureItems.Clear();
        foreach (var item in EffectiveFiles)
        {
            _effectivePictureItems.Add(item);
        }
        _effectivePictureItems.Add(_appendContentItem);
    }

    private void SyncEffectivePictureItems(NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                InsertEffectivePictureItems(e.NewItems, e.NewStartingIndex);
                break;
            case NotifyCollectionChangedAction.Remove:
                RemoveEffectivePictureItems(e.OldItems);
                break;
            case NotifyCollectionChangedAction.Replace:
                RemoveEffectivePictureItems(e.OldItems);
                InsertEffectivePictureItems(e.NewItems, e.NewStartingIndex);
                break;
            case NotifyCollectionChangedAction.Move:
                RemoveEffectivePictureItems(e.OldItems);
                InsertEffectivePictureItems(e.NewItems, e.NewStartingIndex);
                break;
            default:
                SyncEffectivePictureItems();
                break;
        }
    }

    private void InsertEffectivePictureItems(IList? items, int index)
    {
        if (items is null)
        {
            return;
        }

        var insertIndex = index;
        foreach (var item in items.OfType<UploadFileItem>())
        {
            if (insertIndex < 0)
            {
                insertIndex = EffectiveFiles.IndexOf(item);
            }
            InsertEffectivePictureItem(item, insertIndex);
            insertIndex++;
        }
    }

    private void InsertEffectivePictureItem(UploadFileItem item, int index)
    {
        if (_effectivePictureItems.Contains(item))
        {
            return;
        }

        var appendIndex = EnsureAppendContentItem();
        var insertIndex = Math.Clamp(index, 0, appendIndex);
        _effectivePictureItems.Insert(insertIndex, item);
    }

    private void RemoveEffectivePictureItems(IList? items)
    {
        if (items is null)
        {
            return;
        }

        foreach (var item in items.OfType<UploadFileItem>())
        {
            RemoveEffectivePictureItem(item);
        }
    }

    private void RemoveEffectivePictureItem(UploadFileItem item)
    {
        _effectivePictureItems.Remove(item);
        EnsureAppendContentItem();
    }

    private int EnsureAppendContentItem()
    {
        var appendIndex = _effectivePictureItems.IndexOf(_appendContentItem);
        if (appendIndex >= 0)
        {
            return appendIndex;
        }

        _effectivePictureItems.Add(_appendContentItem);
        return _effectivePictureItems.Count - 1;
    }

    private bool IsImageFile(UploadFileInfo file)
    {
        if (IsImageFilePredicate is not null)
        {
            return IsImageFilePredicate.Invoke(file);
        }

        var extension = System.IO.Path.GetExtension(file.FilePath.LocalPath);
        return !string.IsNullOrEmpty(extension) && ImageExtensionRegex.IsMatch(extension);
    }

    private static UploadFileInfo CreateUploadFileInfo(UploadFileItem item)
    {
        return new UploadFileInfo(
            item.Name ?? string.Empty,
            item.Path ?? new Uri("file:///", UriKind.Absolute),
            item.Size);
    }

    private void UpdateTaskRunning()
    {
        var isTaskRunning = EffectiveFiles.Any(file => file.Status == FileUploadStatus.Uploading);
        if (IsTaskRunning != isTaskRunning)
        {
            SetCurrentValue(IsTaskRunningProperty, isTaskRunning);
        }
    }

    private void CancelUploadQueue(Guid id)
    {
        var task = _uploadQueue.CancelAsync(id);
        if (!task.IsCompletedSuccessfully)
        {
            _ = ObserveUploadQueueTaskAsync(task);
        }
    }

    private void CancelAllUploadQueue()
    {
        var task = _uploadQueue.CancelAllAsync();
        if (!task.IsCompletedSuccessfully)
        {
            _ = ObserveUploadQueueTaskAsync(task);
        }
    }

    private static async Task ObserveUploadQueueTaskAsync(Task task)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Upload queue cancellation failed: {ex.Message}");
        }
    }

    private void RunOnUiThread(Action action)
    {
        if (Dispatcher.CheckAccess())
        {
            action();
        }
        else
        {
            Dispatcher.Post(action);
        }
    }
}
