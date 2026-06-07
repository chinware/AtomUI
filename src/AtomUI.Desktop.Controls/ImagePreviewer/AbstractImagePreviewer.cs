using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls;

public abstract class AbstractImagePreviewer : TemplatedControl, IMotionAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<IList<string>?> ItemsSourceProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, IList<string>?>(nameof(ItemsSource));

    public static readonly StyledProperty<string?> FallbackImageSrcProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, string?>(nameof(FallbackImageSrc));

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, bool>(nameof(IsOpen));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractImagePreviewer>();

    public static readonly StyledProperty<double> CoverWidthProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, double>(nameof(CoverWidth), double.NaN);

    public static readonly StyledProperty<double> CoverHeightProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, double>(nameof(CoverHeight), double.NaN);

    public static readonly StyledProperty<int> CurrentIndexProperty =
        AvaloniaProperty.Register<AbstractImagePreviewer, int>(nameof(CurrentIndex), 0);

    public IList<string>? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public string? FallbackImageSrc
    {
        get => GetValue(FallbackImageSrcProperty);
        set => SetValue(FallbackImageSrcProperty, value);
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

    internal static readonly DirectProperty<AbstractImagePreviewer, IList<PreviewImageSource>?> EffectiveSourcesProperty =
        AvaloniaProperty.RegisterDirect<AbstractImagePreviewer, IList<PreviewImageSource>?>(
            nameof(EffectiveSources),
            o => o.EffectiveSources,
            (o, v) => o.EffectiveSources = v);

    private IList<PreviewImageSource>? _effectiveSources;

    internal IList<PreviewImageSource>? EffectiveSources
    {
        get => _effectiveSources;
        set => SetAndRaise(EffectiveSourcesProperty, ref _effectiveSources, value);
    }

    #endregion

    private bool _ignoreIsOpenChanged;
    private IImagePreviewerOpenState? _openState;
    private IDisposable? _modalSubscription;
    private bool _dialogOpening;
    private bool _dialogClosing;

    static AbstractImagePreviewer()
    {
        FocusableProperty.OverrideDefaultValue<AbstractImagePreviewer>(true);
        IsOpenProperty.Changed.AddClassHandler<AbstractImagePreviewer>((x, e) => x.HandleIsOpenChanged(e));
    }

    public AbstractImagePreviewer()
    {
        this.RegisterTokenResourceScope(ImagePreviewerToken.ScopeProvider);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ItemsSourceProperty)
        {
            HandleSourceChanged();
        }
    }

    internal PreviewImageSource LoadImageSource(string filePath)
    {
        using var stream  = AssetsLoader.OpenStream(filePath);
        if (IsSvgImage(filePath))
        {
            return PreviewImageSource.CreateSvg(stream);
        }
        return PreviewImageSource.CreateBitmap(stream);
    }

    private protected virtual void HandleSourceChanged()
    {
        MaterializeEffectiveSourcesFromItemsSource();
    }

    private protected void MaterializeEffectiveSourcesFromItemsSource()
    {
        var itemsSource = ItemsSource;
        if (itemsSource == null || itemsSource.Count == 0)
        {
            ClearEffectiveSources();
            return;
        }

        var effectiveSources = new List<PreviewImageSource>(itemsSource.Count);
        foreach (var source in itemsSource)
        {
            try
            {
                effectiveSources.Add(LoadImageSource(source));
            }
            catch (Exception)
            {
                // TODO 这个错误直接抛出还是忽略
            }
        }

        SetEffectiveSources(effectiveSources);
    }

    private protected void MaterializeFallbackEffectiveSource()
    {
        if (FallbackImageSrc != null)
        {
            try
            {
                SetEffectiveSources(new[] { LoadImageSource(FallbackImageSrc) });
            }
            catch (Exception)
            {
                ClearEffectiveSources();
            }
        }
        else
        {
            ClearEffectiveSources();
        }
    }

    private protected void ClearEffectiveSources()
    {
        SetEffectiveSources(Array.Empty<PreviewImageSource>());
    }

    private void SetEffectiveSources(IList<PreviewImageSource> effectiveSources)
    {
        var oldSources = EffectiveSources;
        if (ReferenceEquals(oldSources, effectiveSources))
        {
            return;
        }

        SetCurrentValue(EffectiveSourcesProperty, effectiveSources);
        DisposeSources(oldSources);
    }

    private static void DisposeSources(IList<PreviewImageSource>? sources)
    {
        if (sources != null)
        {
            foreach (var source in sources)
            {
                source.Dispose();
            }
        }
    }

    private bool IsSvgImage(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return false;
        }

        return string.Equals(Path.GetExtension(filePath), ".svg", StringComparison.OrdinalIgnoreCase);
    }

    protected override void OnLoaded(RoutedEventArgs args)
    {
        base.OnLoaded(args);
        HandleLoadedFallbackSource();
    }

    private protected virtual void HandleLoadedFallbackSource()
    {
        if (EffectiveSources == null || EffectiveSources?.Count == 0)
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
        disposables.Add(BindUtils.RelayBind(this, EffectiveSourcesProperty, dialogHost, ImagePreviewerDialog.ItemsSourceProperty));
        disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, dialogHost, ImagePreviewerDialog.IsMotionEnabledProperty));
        disposables.Add(BindUtils.RelayBind(this, IsDialogModalProperty, dialogHost, ImagePreviewerDialog.IsModalProperty));
        disposables.Add(BindUtils.RelayBind(this, CurrentIndexProperty, dialogHost, ImagePreviewerDialog.CurrentIndexProperty));
    }

    private void RelayOverlayHostBindings(CompositeDisposable disposables, ImagePreviewerOverlayHost overlayHost)
    {
        disposables.Add(BindUtils.RelayBind(this, IsImageMovableProperty, overlayHost, ImagePreviewerOverlayHost.IsImageMovableProperty));
        disposables.Add(BindUtils.RelayBind(this, ImageScaleStepProperty, overlayHost, ImagePreviewerOverlayHost.ScaleStepProperty));
        disposables.Add(BindUtils.RelayBind(this, ImageMinScaleProperty, overlayHost, ImagePreviewerOverlayHost.MinScaleProperty));
        disposables.Add(BindUtils.RelayBind(this, ImageMaxScaleProperty, overlayHost, ImagePreviewerOverlayHost.MaxScaleProperty));
        disposables.Add(BindUtils.RelayBind(this, EffectiveSourcesProperty, overlayHost, ImagePreviewerOverlayHost.ItemsSourceProperty));
        disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, overlayHost, ImagePreviewerOverlayHost.IsMotionEnabledProperty));
        disposables.Add(BindUtils.RelayBind(this, IsDialogModalProperty, overlayHost, ImagePreviewerOverlayHost.IsModalProperty));
        disposables.Add(BindUtils.RelayBind(this, CurrentIndexProperty, overlayHost, ImagePreviewerOverlayHost.CurrentIndexProperty));
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
