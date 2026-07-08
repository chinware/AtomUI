using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

public abstract class AbstractImagePreviewer : TemplatedControl, IMotionAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<ImageSourceUri?> SourceUriProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, ImageSourceUri?>(nameof(SourceUri));

    public static readonly StyledProperty<IList<ImageSourceUri>?> SourceUrisProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, IList<ImageSourceUri>?>(nameof(SourceUris));

    public static readonly StyledProperty<ImageSourceUri?> FallbackSourceUriProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, ImageSourceUri?>(nameof(FallbackSourceUri));

    public static readonly StyledProperty<string?> PreviewTitleProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, string?>(nameof(PreviewTitle));

    public static readonly StyledProperty<PathIcon?> PreviewTitleIconProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, PathIcon?>(nameof(PreviewTitleIcon));

    public static readonly StyledProperty<IImagePreviewTitleResolver?> PreviewTitleResolverProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, IImagePreviewTitleResolver?>(
            nameof(PreviewTitleResolver),
            DefaultImagePreviewTitleResolver.Instance);

    public static readonly StyledProperty<object?> LoadingContentProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, object?>(nameof(LoadingContent));

    public static readonly StyledProperty<IDataTemplate?> LoadingContentTemplateProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, IDataTemplate?>(nameof(LoadingContentTemplate));

    public static readonly StyledProperty<object?> ErrorContentProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, object?>(nameof(ErrorContent));

    public static readonly StyledProperty<IDataTemplate?> ErrorContentTemplateProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, IDataTemplate?>(nameof(ErrorContentTemplate));

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, bool>(
            nameof(IsOpen),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractImagePreviewer>();

    public static readonly StyledProperty<double> CoverWidthProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, double>(nameof(CoverWidth), double.NaN);

    public static readonly StyledProperty<double> CoverHeightProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, double>(nameof(CoverHeight), double.NaN);

    public static readonly StyledProperty<int> CurrentIndexProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, int>(
            nameof(CurrentIndex),
            0,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<int> CoverIndexProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, int>(
            nameof(CoverIndex),
            0,
            coerce: CoerceNonNegativeValue);

    public static readonly StyledProperty<int> MaxConcurrentLoadsProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, int>(
            nameof(MaxConcurrentLoads),
            4,
            coerce: CoercePositiveValue);

    public static readonly StyledProperty<int> PreloadCountProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, int>(
            nameof(PreloadCount),
            1,
            coerce: CoerceNonNegativeValue);

    public ImageSourceUri? SourceUri
    {
        get => GetValue(SourceUriProperty);
        set => SetValue(SourceUriProperty, value);
    }

    public IList<ImageSourceUri>? SourceUris
    {
        get => GetValue(SourceUrisProperty);
        set => SetValue(SourceUrisProperty, value);
    }

    public ImageSourceUri? FallbackSourceUri
    {
        get => GetValue(FallbackSourceUriProperty);
        set => SetValue(FallbackSourceUriProperty, value);
    }

    public string? PreviewTitle
    {
        get => GetValue(PreviewTitleProperty);
        set => SetValue(PreviewTitleProperty, value);
    }

    public PathIcon? PreviewTitleIcon
    {
        get => GetValue(PreviewTitleIconProperty);
        set => SetValue(PreviewTitleIconProperty, value);
    }

    public IImagePreviewTitleResolver? PreviewTitleResolver
    {
        get => GetValue(PreviewTitleResolverProperty);
        set => SetValue(PreviewTitleResolverProperty, value);
    }

    [DependsOn(nameof(LoadingContentTemplate))]
    public object? LoadingContent
    {
        get => GetValue(LoadingContentProperty);
        set => SetValue(LoadingContentProperty, value);
    }

    public IDataTemplate? LoadingContentTemplate
    {
        get => GetValue(LoadingContentTemplateProperty);
        set => SetValue(LoadingContentTemplateProperty, value);
    }

    [DependsOn(nameof(ErrorContentTemplate))]
    public object? ErrorContent
    {
        get => GetValue(ErrorContentProperty);
        set => SetValue(ErrorContentProperty, value);
    }

    public IDataTemplate? ErrorContentTemplate
    {
        get => GetValue(ErrorContentTemplateProperty);
        set => SetValue(ErrorContentTemplateProperty, value);
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public double CoverWidth
    {
        get => GetValue(CoverWidthProperty);
        set => SetValue(CoverWidthProperty, value);
    }

    public double CoverHeight
    {
        get => GetValue(CoverHeightProperty);
        set => SetValue(CoverHeightProperty, value);
    }

    public int CurrentIndex
    {
        get => GetValue(CurrentIndexProperty);
        set => SetValue(CurrentIndexProperty, value);
    }

    public int CoverIndex
    {
        get => GetValue(CoverIndexProperty);
        set => SetValue(CoverIndexProperty, value);
    }

    public int MaxConcurrentLoads
    {
        get => GetValue(MaxConcurrentLoadsProperty);
        set => SetValue(MaxConcurrentLoadsProperty, value);
    }

    public int PreloadCount
    {
        get => GetValue(PreloadCountProperty);
        set => SetValue(PreloadCountProperty, value);
    }

    #endregion

    #region 预览窗口相关属性设置

    public static readonly StyledProperty<bool> IsImageMovableProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, bool>(nameof(IsImageMovable), true);

    public static readonly StyledProperty<double> ImageScaleStepProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, double>(nameof(ImageScaleStep), 0.5);

    public static readonly StyledProperty<double> ImageMinScaleProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, double>(nameof(ImageMinScale), 1.0);

    public static readonly StyledProperty<double> ImageMaxScaleProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, double>(nameof(ImageMaxScale), 50.0);

    public static readonly StyledProperty<bool> IsDialogModalProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, bool>(nameof (IsDialogModal), false);

    public static readonly StyledProperty<bool> IsDialogTopmostProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, bool>(nameof(IsDialogTopmost));

    public bool IsImageMovable
    {
        get => GetValue(IsImageMovableProperty);
        set => SetValue(IsImageMovableProperty, value);
    }

    public double ImageScaleStep
    {
        get => GetValue(ImageScaleStepProperty);
        set => SetValue(ImageScaleStepProperty, value);
    }

    public double ImageMinScale
    {
        get => GetValue(ImageMinScaleProperty);
        set => SetValue(ImageMinScaleProperty, value);
    }

    public double ImageMaxScale
    {
        get => GetValue(ImageMaxScaleProperty);
        set => SetValue(ImageMaxScaleProperty, value);
    }

    public bool IsDialogModal
    {
        get => GetValue(IsDialogModalProperty);
        set => SetValue(IsDialogModalProperty, value);
    }

    public bool IsDialogTopmost
    {
        get => GetValue(IsDialogTopmostProperty);
        set => SetValue(IsDialogTopmostProperty, value);
    }

    #endregion

    #region 公共事件定义

    public event EventHandler? DialogClosed;
    public event EventHandler? DialogOpened;
    public event EventHandler<CancelEventArgs>? DialogClosing;

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<AbstractImagePreviewer, IList<ImagePreviewItem>?> EffectiveItemsProperty =
        AvaloniaProperty.RegisterDirect<AbstractImagePreviewer, IList<ImagePreviewItem>?>(
            nameof(EffectiveItems),
            o => o.EffectiveItems,
            (o, v) => o.EffectiveItems = v);

    private IList<ImagePreviewItem>? _effectiveItems;

    internal IList<ImagePreviewItem>? EffectiveItems
    {
        get => _effectiveItems;
        set => SetAndRaise(EffectiveItemsProperty, ref _effectiveItems, value);
    }

    #endregion

    private bool _ignoreIsOpenChanged;
    private IImagePreviewerOpenState? _openState;
    private IDisposable? _modalSubscription;
    private bool _dialogOpening;
    private bool _dialogClosing;
    private readonly IImageSourceLoader _imageSourceLoader;
    private readonly ImagePreviewLoadScheduler _imageLoadScheduler;

    private protected IImageSourceLoader ImageSourceLoader => _imageSourceLoader;

    static AbstractImagePreviewer()
    {
        FocusableProperty.OverrideDefaultValue<AbstractImagePreviewer>(true);
        IsOpenProperty.Changed.AddClassHandler<AbstractImagePreviewer>((x, e) => x.HandleIsOpenChanged(e));
    }

    private static int CoercePositiveValue(AvaloniaObject sender, int value)
    {
        return Math.Max(1, value);
    }

    private static int CoerceNonNegativeValue(AvaloniaObject sender, int value)
    {
        return Math.Max(0, value);
    }

    public AbstractImagePreviewer()
        : this(new DefaultImageSourceLoader())
    {
    }

    internal AbstractImagePreviewer(IImageSourceLoader imageSourceLoader)
    {
        _imageSourceLoader   = imageSourceLoader;
        _imageLoadScheduler  = new ImagePreviewLoadScheduler(
            imageSourceLoader,
            () => MaxConcurrentLoads,
            HandleItemLoadSettled);
        this.RegisterTokenResourceScope(ImagePreviewerToken.ScopeProvider);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SourceUriProperty ||
            change.Property == SourceUrisProperty)
        {
            HandleSourceChanged();
        }
        else if (change.Property == FallbackSourceUriProperty)
        {
            HandleFallbackSourceChanged((ImageSourceUri?)change.OldValue);
        }
        else if (change.Property == CurrentIndexProperty ||
                 change.Property == PreloadCountProperty ||
                 change.Property == MaxConcurrentLoadsProperty)
        {
            if (IsOpen)
            {
                RequestPreviewLoads();
            }
        }
    }

    private protected virtual void HandleSourceChanged()
    {
        MaterializeEffectiveItemsFromSourceUris();
    }

    private protected virtual void HandleFallbackSourceChanged(ImageSourceUri? oldFallbackSourceUri)
    {
        if (ShouldMaterializeFallbackEffectiveSource(oldFallbackSourceUri))
        {
            MaterializeFallbackEffectiveSource();
        }
    }

    private protected void MaterializeEffectiveItemsFromSourceUris()
    {
        var sourceUris = ResolveSourceUris();
        if (sourceUris.Count == 0)
        {
            ClearEffectiveItems();
            return;
        }

        var items = CreateEffectiveItems(sourceUris);
        if (HasSameItems(EffectiveItems, items))
        {
            return;
        }

        SetEffectiveItems(items, items);
    }

    private protected void MaterializeFallbackEffectiveSource()
    {
        if (FallbackSourceUri != null)
        {
            var item = new ImagePreviewItem(FallbackSourceUri);
            SetEffectiveItems(new[] { item });
            RequestItemLoad(item, ImagePreviewLoadPriority.Cover);
        }
        else
        {
            ClearEffectiveItems();
        }
    }

    private protected void ClearEffectiveItems()
    {
        SetEffectiveItems(Array.Empty<ImagePreviewItem>());
    }

    private List<ImagePreviewItem> CreateEffectiveItems(IReadOnlyList<ImageSourceUri> sourceUris)
    {
        var reusableItems = CreateReusableItemMap(EffectiveItems);
        var items         = new List<ImagePreviewItem>(sourceUris.Count);
        foreach (var sourceUri in sourceUris)
        {
            items.Add(TryTakeReusableItem(reusableItems, sourceUri) ?? new ImagePreviewItem(sourceUri));
        }

        return items;
    }

    private static Dictionary<string, Queue<ImagePreviewItem>> CreateReusableItemMap(IList<ImagePreviewItem>? items)
    {
        var reusableItems = new Dictionary<string, Queue<ImagePreviewItem>>();
        if (items is null)
        {
            return reusableItems;
        }

        foreach (var item in items)
        {
            if (!reusableItems.TryGetValue(item.SourceUri.CacheKey, out var queue))
            {
                queue = new Queue<ImagePreviewItem>();
                reusableItems.Add(item.SourceUri.CacheKey, queue);
            }

            queue.Enqueue(item);
        }

        return reusableItems;
    }

    private static ImagePreviewItem? TryTakeReusableItem(Dictionary<string, Queue<ImagePreviewItem>> reusableItems,
                                                        ImageSourceUri sourceUri)
    {
        return reusableItems.TryGetValue(sourceUri.CacheKey, out var queue) && queue.Count > 0
            ? queue.Dequeue()
            : null;
    }

    private static bool HasSameItems(IList<ImagePreviewItem>? oldItems, IList<ImagePreviewItem> newItems)
    {
        if (oldItems is null || oldItems.Count != newItems.Count)
        {
            return false;
        }

        for (var i = 0; i < oldItems.Count; i++)
        {
            if (!ReferenceEquals(oldItems[i], newItems[i]))
            {
                return false;
            }
        }

        return true;
    }

    private void SetEffectiveItems(IList<ImagePreviewItem> effectiveItems,
                                   IReadOnlyCollection<ImagePreviewItem>? preservedItems = null)
    {
        var oldItems = EffectiveItems;
        if (ReferenceEquals(oldItems, effectiveItems))
        {
            return;
        }

        CancelImageLoads();
        UnsubscribeItems(oldItems);
        SetCurrentValue(EffectiveItemsProperty, effectiveItems);
        SubscribeItems(effectiveItems);
        if (preservedItems is null)
        {
            DisposeItems(oldItems);
        }
        else
        {
            DisposeItemsExcept(oldItems, preservedItems);
        }
    }

    private static void DisposeItems(IList<ImagePreviewItem>? items)
    {
        if (items != null)
        {
            foreach (var item in items)
            {
                item.Dispose();
            }
        }
    }

    private IReadOnlyList<ImageSourceUri> ResolveSourceUris()
    {
        if (SourceUris is { Count: > 0 })
        {
            return SourceUris.Where(uri => uri is not null).ToList();
        }

        return SourceUri is null ? [] : [SourceUri];
    }

    private bool ShouldMaterializeFallbackEffectiveSource(ImageSourceUri? oldFallbackSourceUri)
    {
        var sourceUris = ResolveSourceUris();
        if (sourceUris.Count == 0)
        {
            return true;
        }

        if (FallbackSourceUri is null)
        {
            return false;
        }

        if (EffectiveItems is not { Count: > 0 } items)
        {
            return false;
        }

        if (oldFallbackSourceUri is not null &&
            items.Count == 1 &&
            items[0].SourceUri.CacheKey == oldFallbackSourceUri.CacheKey)
        {
            return true;
        }

        return items.All(item => item.IsFailed);
    }

    private void CancelImageLoads()
    {
        _imageLoadScheduler.Reset();
    }

    internal void RequestPreviewLoads()
    {
        CancelImageLoads();
        if (EffectiveItems is not { Count: > 0 } effectiveItems)
        {
            return;
        }

        var currentIndex = ClampIndex(CurrentIndex, effectiveItems.Count);
        RequestItemLoad(effectiveItems[currentIndex], ImagePreviewLoadPriority.Current);
        RequestItemLoad(effectiveItems[ClampIndex(CoverIndex, effectiveItems.Count)], ImagePreviewLoadPriority.Cover);

        var preloadCount = PreloadCount;
        for (var offset = 1; offset <= preloadCount; offset++)
        {
            var previousIndex = currentIndex - offset;
            if (previousIndex >= 0)
            {
                RequestItemLoad(effectiveItems[previousIndex], ImagePreviewLoadPriority.Preload);
            }

            var nextIndex = currentIndex + offset;
            if (nextIndex < effectiveItems.Count)
            {
                RequestItemLoad(effectiveItems[nextIndex], ImagePreviewLoadPriority.Preload);
            }
        }
    }

    private protected void RequestItemLoad(ImagePreviewItem item, ImagePreviewLoadPriority priority)
    {
        _imageLoadScheduler.Enqueue(item, priority);
    }

    private protected virtual void RequestClosedStateLoads()
    {
    }

    private protected static int ClampIndex(int index, int count)
    {
        if (count <= 0)
        {
            return 0;
        }

        if (index < 0)
        {
            return 0;
        }

        return index >= count ? count - 1 : index;
    }

    private void SubscribeItems(IList<ImagePreviewItem>? items)
    {
        if (items == null)
        {
            return;
        }

        foreach (var item in items)
        {
            item.PropertyChanged += HandleEffectiveItemPropertyChanged;
        }
    }

    private void UnsubscribeItems(IList<ImagePreviewItem>? items)
    {
        if (items == null)
        {
            return;
        }

        foreach (var item in items)
        {
            item.PropertyChanged -= HandleEffectiveItemPropertyChanged;
        }
    }

    private void HandleEffectiveItemPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(ImagePreviewItem.State))
        {
            CompleteCurrentLoadBatchIfReady();
        }
    }

    private void HandleItemLoadSettled(ImagePreviewItem item)
    {
        CompleteCurrentLoadBatchIfReady();
    }

    private void CompleteCurrentLoadBatchIfReady()
    {
        if (EffectiveItems is not { Count: > 0 } items ||
            items.Any(item => item.IsLoading || item.State == ImagePreviewItemState.Pending))
        {
            return;
        }

        var loadedItems = items.Where(item => item.IsLoaded).ToList();
        if (loadedItems.Count == 0)
        {
            if (FallbackSourceUri is not null &&
                items.Any(item => item.SourceUri.CacheKey != FallbackSourceUri.CacheKey))
            {
                MaterializeFallbackEffectiveSource();
            }
            return;
        }

        if (loadedItems.Count == items.Count)
        {
            return;
        }

        UnsubscribeItems(items);
        SetCurrentValue(EffectiveItemsProperty, loadedItems);
        SubscribeItems(loadedItems);
        DisposeItemsExcept(items, loadedItems);
    }

    private static void DisposeItemsExcept(IList<ImagePreviewItem>? items,
                                           IReadOnlyCollection<ImagePreviewItem> preservedItems)
    {
        if (items is null)
        {
            return;
        }

        foreach (var item in items)
        {
            if (!preservedItems.Contains(item))
            {
                item.Dispose();
            }
        }
    }

    protected override void OnLoaded(RoutedEventArgs args)
    {
        base.OnLoaded(args);
        HandleLoadedFallbackSource();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        CancelImageLoads();
    }

    private protected virtual void HandleLoadedFallbackSource()
    {
        if (EffectiveItems == null || EffectiveItems?.Count == 0)
        {
            MaterializeFallbackEffectiveSource();
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        OpenDialog();
    }

    #region 预览窗口相关方法

    private void HandleIsOpenChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (!_ignoreIsOpenChanged)
        {
            if (e.GetNewValue<bool>())
            {
                OpenDialog();
            }
            else
            {
                CloseDialog();
            }
        }
    }

    public void OpenDialog()
    {
        if (_openState != null || _dialogOpening)
        {
            return;
        }

        PrepareDialogOpen();
        if (EffectiveItems is not { Count: > 0 })
        {
            return;
        }
        RequestPreviewLoads();

        _dialogOpening = true;
        var placementTarget = this;
        Debug.Assert(placementTarget != null);
        var topLevel = TopLevel.GetTopLevel(placementTarget);
        if (!RuntimePlatform.Features.SupportsNativeWindow)
        {
            OpenOverlayDialog(placementTarget, topLevel);
            return;
        }

        if (topLevel is null)
        {
            _dialogOpening = false;
            throw new InvalidOperationException("Unable to resolve TopLevel for ImagePreviewer.");
        }

        CompositeDisposable relayBindingDisposables = new CompositeDisposable();

        var previewDialog = new ImagePreviewerDialog(topLevel, this);
        RelayDialogHostBindings(relayBindingDisposables, previewDialog);

        Debug.Assert(previewDialog != null);
        var handlerCleanup = new CompositeDisposable(8);
        previewDialog.Topmost = IsDialogTopmost;
        ((ISetLogicalParent)previewDialog).SetParent(this);

        previewDialog
            .Bind(
                ThemeVariantScope.ActualThemeVariantProperty,
                this.GetBindingObservable(ThemeVariantScope.ActualThemeVariantProperty))
            .DisposeWith(handlerCleanup);

        SubscribeToEventHandler<ImagePreviewerDialog, EventHandler<TemplateAppliedEventArgs>>(previewDialog, RootTemplateApplied,
            (x, handler) => x.TemplateApplied += handler,
            (x, handler) => x.TemplateApplied -= handler).DisposeWith(handlerCleanup);

        SubscribeToEventHandler<Control, EventHandler<VisualTreeAttachmentEventArgs>>(placementTarget, TargetDetached,
            (x, handler) => x.DetachedFromVisualTree += handler,
            (x, handler) => x.DetachedFromVisualTree -= handler).DisposeWith(handlerCleanup);
        if (topLevel is Window window)
        {
            SubscribeToEventHandler<Window, EventHandler>(window, ParentClosed,
                (x, handler) => x.Closed += handler,
                (x, handler) => x.Closed -= handler).DisposeWith(handlerCleanup);
        }

        var cleanupPopup = Disposable.Create((previewDialog, handlerCleanup), state =>
        {
            previewDialog.Close(() =>
            {
                state.handlerCleanup.Dispose();
                ((ISetLogicalParent)state.previewDialog).SetParent(null);
                relayBindingDisposables.Dispose();
                _dialogClosing             = false;
                if (DataContext is IDialogAwareDataContext dialogAwareDataContext)
                {
                    dialogAwareDataContext.NotifyClosed();
                }
                DialogClosed?.Invoke(this, EventArgs.Empty);
            });
        });

        _openState = new DialogOpenState(topLevel, previewDialog, cleanupPopup);

        previewDialog.Focus();
        if (IsDialogModal)
        {
            if (topLevel is Window windowTopLevel)
            {
                previewDialog.ShowDialog(windowTopLevel);
            }
        }
        else
        {
            previewDialog.Show();
        }

        if (IsDialogModal)
        {
            var tcs = new TaskCompletionSource<object?>();
            var disposables = new CompositeDisposable(
            [
                Observable.FromEventPattern(
                              x => DialogClosed += x,
                              x => DialogClosed -= x)
                          .Take(1)
                          .Subscribe(_ =>
                          {
                              _modalSubscription?.Dispose();
                          }),
                Disposable.Create(() =>
                {
                    _modalSubscription = null;
                    tcs.SetResult(null);
                })
            ]);

            _modalSubscription = disposables;
        }

        using (BeginIgnoringIsOpen())
        {
            SetCurrentValue(IsOpenProperty, true);
        }
        DialogOpened?.Invoke(this, EventArgs.Empty);
        _dialogOpening = false;
    }

    private void OpenOverlayDialog(Control placementTarget, TopLevel? topLevel)
    {
        try
        {
            var overlayLayer = OverlayLayerResolver.ResolveOverlayLayer(placementTarget, topLevel, nameof(ImagePreviewer));
            topLevel ??= TopLevel.GetTopLevel(overlayLayer);
            if (topLevel is null)
            {
                _dialogOpening = false;
                throw new InvalidOperationException("Unable to resolve TopLevel for ImagePreviewer.");
            }

            CompositeDisposable relayBindingDisposables = new CompositeDisposable();
            var previewHost = new ImagePreviewerOverlayHost(topLevel, this);
            RelayOverlayHostBindings(relayBindingDisposables, previewHost);

            var handlerCleanup = new CompositeDisposable(8);
            ((ISetLogicalParent)previewHost).SetParent(this);

            previewHost
                .Bind(
                    ThemeVariantScope.ActualThemeVariantProperty,
                    this.GetBindingObservable(ThemeVariantScope.ActualThemeVariantProperty))
                .DisposeWith(handlerCleanup);

            SubscribeToEventHandler<Control, EventHandler<VisualTreeAttachmentEventArgs>>(placementTarget, TargetDetached,
                (x, handler) => x.DetachedFromVisualTree += handler,
                (x, handler) => x.DetachedFromVisualTree -= handler).DisposeWith(handlerCleanup);

            ConfigureOverlayHostBounds(previewHost, overlayLayer, topLevel);
            void HandleOverlayLayerSizeChanged(object? sender, SizeChangedEventArgs args)
            {
                ConfigureOverlayHostBounds(previewHost, overlayLayer, topLevel);
            }

            overlayLayer.SizeChanged += HandleOverlayLayerSizeChanged;
            overlayLayer.Children.Add(previewHost);

            var cleanupPopup = Disposable.Create((previewHost, overlayLayer, handlerCleanup), state =>
            {
                previewHost.Close(() =>
                {
                    overlayLayer.SizeChanged -= HandleOverlayLayerSizeChanged;
                    state.overlayLayer.Children.Remove(state.previewHost);
                    state.handlerCleanup.Dispose();
                    ((ISetLogicalParent)state.previewHost).SetParent(null);
                    relayBindingDisposables.Dispose();
                    _dialogClosing             = false;
                    if (DataContext is IDialogAwareDataContext dialogAwareDataContext)
                    {
                        dialogAwareDataContext.NotifyClosed();
                    }
                    DialogClosed?.Invoke(this, EventArgs.Empty);
                });
            });

            _openState = new OverlayOpenState(topLevel, previewHost, cleanupPopup);
            previewHost.Focus();

            using (BeginIgnoringIsOpen())
            {
                SetCurrentValue(IsOpenProperty, true);
            }
            DialogOpened?.Invoke(this, EventArgs.Empty);
            _dialogOpening = false;
        }
        catch
        {
            _dialogOpening = false;
            throw;
        }
    }

    private static void ConfigureOverlayHostBounds(Control previewHost, Control overlayLayer, TopLevel topLevel)
    {
        var size = overlayLayer.Bounds.Size;
        if (size.Width <= 0 || size.Height <= 0)
        {
            size = topLevel.ClientSize;
        }

        previewHost.Width  = size.Width;
        previewHost.Height = size.Height;
        Canvas.SetLeft(previewHost, 0);
        Canvas.SetTop(previewHost, 0);
    }

    private protected virtual void PrepareDialogOpen()
    {
        if (SourceUris is { Count: > 0 } || SourceUri is not null)
        {
            MaterializeEffectiveItemsFromSourceUris();
        }
        else
        {
            MaterializeFallbackEffectiveSource();
        }
    }

    protected virtual void CloseDialog()
    {
        if (_dialogClosing)
        {
            return;
        }

        var closingArgs = new CancelEventArgs();
        DialogClosing?.Invoke(this, closingArgs);
        if (closingArgs.Cancel)
        {
            return;
        }

        if (_openState is null)
        {
            using (BeginIgnoringIsOpen())
            {
                SetCurrentValue(IsOpenProperty, false);
            }

            return;
        }

        _openState.Dispose();
        _openState = null;

        _modalSubscription?.Dispose();
        _modalSubscription = null;
        CancelImageLoads();
        RequestClosedStateLoads();
        using (BeginIgnoringIsOpen())
        {
            SetCurrentValue(IsOpenProperty, false);
        }
    }

    private protected virtual void RelayDialogHostBindings(CompositeDisposable disposables, ImagePreviewerDialog dialogHost)
    {
        disposables.Add(BindUtils.RelayBind(this, IsImageMovableProperty, dialogHost, ImagePreviewerDialog.IsImageMovableProperty));
        disposables.Add(BindUtils.RelayBind(this, ImageScaleStepProperty, dialogHost, ImagePreviewerDialog.ScaleStepProperty));
        disposables.Add(BindUtils.RelayBind(this, ImageMinScaleProperty, dialogHost, ImagePreviewerDialog.MinScaleProperty));
        disposables.Add(BindUtils.RelayBind(this, ImageMaxScaleProperty, dialogHost, ImagePreviewerDialog.MaxScaleProperty));
        disposables.Add(BindUtils.RelayBind(this, EffectiveItemsProperty, dialogHost, ImagePreviewerDialog.ItemsSourceProperty));
        disposables.Add(BindUtils.RelayBind(this, LoadingContentProperty, dialogHost, ImagePreviewerDialog.LoadingContentProperty));
        disposables.Add(BindUtils.RelayBind(this, LoadingContentTemplateProperty, dialogHost, ImagePreviewerDialog.LoadingContentTemplateProperty));
        disposables.Add(BindUtils.RelayBind(this, ErrorContentProperty, dialogHost, ImagePreviewerDialog.ErrorContentProperty));
        disposables.Add(BindUtils.RelayBind(this, ErrorContentTemplateProperty, dialogHost, ImagePreviewerDialog.ErrorContentTemplateProperty));
        disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, dialogHost, ImagePreviewerDialog.IsMotionEnabledProperty));
        disposables.Add(BindUtils.RelayBind(this, IsDialogModalProperty, dialogHost, ImagePreviewerDialog.IsModalProperty));
        disposables.Add(BindUtils.RelayBind(this,
            CurrentIndexProperty,
            dialogHost,
            ImagePreviewerDialog.CurrentIndexProperty,
            BindingMode.TwoWay));
        disposables.Add(BindUtils.RelayBind(this, PreviewTitleProperty, dialogHost, Window.TitleProperty));
        disposables.Add(BindUtils.RelayBind(this, PreviewTitleIconProperty, dialogHost, ImagePreviewerDialog.TitleIconProperty));
        disposables.Add(BindUtils.RelayBind(this, PreviewTitleResolverProperty, dialogHost, ImagePreviewerDialog.PreviewTitleResolverProperty));
    }

    private void RelayOverlayHostBindings(CompositeDisposable disposables, ImagePreviewerOverlayHost overlayHost)
    {
        disposables.Add(BindUtils.RelayBind(this, IsImageMovableProperty, overlayHost, ImagePreviewerOverlayHost.IsImageMovableProperty));
        disposables.Add(BindUtils.RelayBind(this, ImageScaleStepProperty, overlayHost, ImagePreviewerOverlayHost.ScaleStepProperty));
        disposables.Add(BindUtils.RelayBind(this, ImageMinScaleProperty, overlayHost, ImagePreviewerOverlayHost.MinScaleProperty));
        disposables.Add(BindUtils.RelayBind(this, ImageMaxScaleProperty, overlayHost, ImagePreviewerOverlayHost.MaxScaleProperty));
        disposables.Add(BindUtils.RelayBind(this, EffectiveItemsProperty, overlayHost, ImagePreviewerOverlayHost.ItemsSourceProperty));
        disposables.Add(BindUtils.RelayBind(this, LoadingContentProperty, overlayHost, ImagePreviewerOverlayHost.LoadingContentProperty));
        disposables.Add(BindUtils.RelayBind(this, LoadingContentTemplateProperty, overlayHost, ImagePreviewerOverlayHost.LoadingContentTemplateProperty));
        disposables.Add(BindUtils.RelayBind(this, ErrorContentProperty, overlayHost, ImagePreviewerOverlayHost.ErrorContentProperty));
        disposables.Add(BindUtils.RelayBind(this, ErrorContentTemplateProperty, overlayHost, ImagePreviewerOverlayHost.ErrorContentTemplateProperty));
        disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, overlayHost, ImagePreviewerOverlayHost.IsMotionEnabledProperty));
        disposables.Add(BindUtils.RelayBind(this, IsDialogModalProperty, overlayHost, ImagePreviewerOverlayHost.IsModalProperty));
        disposables.Add(BindUtils.RelayBind(this,
            CurrentIndexProperty,
            overlayHost,
            ImagePreviewerOverlayHost.CurrentIndexProperty,
            BindingMode.TwoWay));
    }

    private void RootTemplateApplied(object? sender, TemplateAppliedEventArgs e)
    {
        if (_openState is not DialogOpenState dialogOpenState)
        {
            return;
        }

        var popupHost = dialogOpenState.DialogHost;
        popupHost.TemplateApplied -= RootTemplateApplied;
        dialogOpenState.SetPresenterSubscription(null);

        // If the Popup appears in a control template, then the child controls
        // that appear in the popup host need to have their TemplatedParent
        // properties set.
        if (TemplatedParent != null && popupHost.Presenter is Control presenter)
        {
            presenter.ApplyTemplate();

            var presenterSubscription = presenter.GetObservable(ContentPresenter.ChildProperty)
                                                 .Subscribe(SetTemplatedParentAndApplyChildTemplates);

            dialogOpenState.SetPresenterSubscription(presenterSubscription);
        }
    }

    private void SetTemplatedParentAndApplyChildTemplates(Control? control)
    {
        if (control != null)
        {
            TemplatedControlUtils.ApplyTemplatedParent(control, TemplatedParent);
        }
    }

    private void TargetDetached(object? sender, VisualTreeAttachmentEventArgs e)
    {
        CloseDialog();
    }

    private static IDisposable SubscribeToEventHandler<T, TEventHandler>(T target, TEventHandler handler, Action<T, TEventHandler> subscribe, Action<T, TEventHandler> unsubscribe)
    {
        subscribe(target, handler);

        return Disposable.Create((unsubscribe, target, handler), state => state.unsubscribe(state.target, state.handler));
    }

    private void ParentClosed(object? sender, EventArgs e)
    {
        CloseDialog();
    }

    internal void NotifyDialogHostCloseRequest()
    {
        CloseDialog();
    }

    private IgnoreIsOpenScope BeginIgnoringIsOpen()
    {
        return new IgnoreIsOpenScope(this);
    }

    private readonly struct IgnoreIsOpenScope : IDisposable
    {
        private readonly AbstractImagePreviewer _owner;

        public IgnoreIsOpenScope(AbstractImagePreviewer owner)
        {
            _owner                      = owner;
            _owner._ignoreIsOpenChanged = true;
        }

        public void Dispose()
        {
            _owner._ignoreIsOpenChanged = false;
        }
    }

    private interface IImagePreviewerOpenState : IDisposable
    {
        TopLevel TopLevel { get; }
    }

    private class DialogOpenState : IImagePreviewerOpenState
    {
        private readonly IDisposable _cleanup;
        private IDisposable? _presenterCleanup;

        public DialogOpenState(TopLevel topLevel, ImagePreviewerDialog previewDialog, IDisposable cleanup)
        {
            TopLevel   = topLevel;
            DialogHost = previewDialog;
            _cleanup   = cleanup;
        }

        public ImagePreviewerDialog DialogHost { get; }
        public TopLevel TopLevel { get; }

        public void SetPresenterSubscription(IDisposable? presenterCleanup)
        {
            _presenterCleanup?.Dispose();
            _presenterCleanup = presenterCleanup;
        }

        public void Dispose()
        {
            _presenterCleanup?.Dispose();
            _cleanup.Dispose();
        }
    }

    private class OverlayOpenState : IImagePreviewerOpenState
    {
        private readonly IDisposable _cleanup;

        public OverlayOpenState(TopLevel topLevel, ImagePreviewerOverlayHost previewHost, IDisposable cleanup)
        {
            TopLevel    = topLevel;
            PreviewHost = previewHost;
            _cleanup    = cleanup;
        }

        public ImagePreviewerOverlayHost PreviewHost { get; }
        public TopLevel TopLevel { get; }

        public void Dispose()
        {
            _cleanup.Dispose();
        }
    }

    #endregion
}
