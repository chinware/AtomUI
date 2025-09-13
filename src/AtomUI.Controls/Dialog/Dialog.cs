using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using AtomUI.Controls.DialogPositioning;
using AtomUI.Controls.MessageBox;
using AtomUI.Controls.Primitives;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.IconPkg;
using AtomUI.Input;
using AtomUI.Reflection;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public class Dialog : Control, 
                      IDialogHostProvider, 
                      IControlSharedTokenResourcesHost,
                      IMotionAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<Dialog, string?>(nameof (Title));
    
    public static readonly StyledProperty<Icon?> TitleIconProperty =
        AvaloniaProperty.Register<Dialog, Icon?>(nameof (TitleIcon));

    public static readonly StyledProperty<Control?> ChildProperty =
        AvaloniaProperty.Register<Dialog, Control?>(nameof(Child));

    public static readonly StyledProperty<bool> InheritsTransformProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof(InheritsTransform));
    
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof(IsOpen));
    
    public static readonly StyledProperty<bool> IsModalProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof (IsModal), true);
    
    public static readonly StyledProperty<bool> IsResizableProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof (IsResizable), false);
        
    public static readonly StyledProperty<bool> IsClosableProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof (IsClosable), true);
    
    public static readonly StyledProperty<bool> IsMaximizableProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof (IsMaximizable), false);

    public static readonly StyledProperty<bool> IsMinimizableProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof (IsMinimizable), true);
    
    public static readonly StyledProperty<bool> IsDragMovableProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof (IsDragMovable), false);
    
    public static readonly StyledProperty<Rect?> PlacementRectProperty =
        AvaloniaProperty.Register<Dialog, Rect?>(nameof(PlacementRect));
    
    public static readonly StyledProperty<Control?> PlacementTargetProperty =
        AvaloniaProperty.Register<Dialog, Control?>(nameof(PlacementTarget));
    
    public static readonly StyledProperty<CustomDialogPlacementCallback?> CustomPopupPlacementCallbackProperty =
        AvaloniaProperty.Register<Dialog, CustomDialogPlacementCallback?>(nameof(CustomDialogPlacementCallback));
    
    public static readonly StyledProperty<bool> OverlayDismissEventPassThroughProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof(OverlayDismissEventPassThrough), false);
    
    public static readonly StyledProperty<IInputElement?> OverlayInputPassThroughElementProperty =
        AvaloniaProperty.Register<Dialog, IInputElement?>(nameof(OverlayInputPassThroughElement));
    
    public static readonly StyledProperty<bool> IsLightDismissEnabledProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof(IsLightDismissEnabled), false);
    
    public static readonly StyledProperty<DialogHorizontalAnchor> HorizontalStartupLocationProperty =
        AvaloniaProperty.Register<Dialog, DialogHorizontalAnchor>(nameof(HorizontalStartupLocation), DialogHorizontalAnchor.Center);
    
    public static readonly StyledProperty<DialogVerticalAnchor> VerticalStartupLocationProperty =
        AvaloniaProperty.Register<Dialog, DialogVerticalAnchor>(nameof(VerticalStartupLocation), DialogVerticalAnchor.Center);
    
    public static readonly StyledProperty<Dimension?> HorizontalOffsetProperty =
        AvaloniaProperty.Register<Dialog, Dimension?>(nameof(HorizontalOffset));
    
    public static readonly StyledProperty<Dimension?> VerticalOffsetProperty =
        AvaloniaProperty.Register<Dialog, Dimension?>(nameof(VerticalOffset));
    
    public static readonly StyledProperty<bool> TopmostProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof(Topmost));
    
    public static readonly StyledProperty<object?> ResultProperty =
        AvaloniaProperty.Register<Dialog, object?>(nameof(Result));
    
    public static readonly StyledProperty<DialogStandardButtons> StandardButtonsProperty =
        DialogButtonBox.StandardButtonsProperty.AddOwner<Dialog>();
    
    public static readonly StyledProperty<DialogStandardButton> DefaultStandardButtonProperty =
        DialogButtonBox.DefaultStandardButtonProperty.AddOwner<Dialog>();
    
    public static readonly StyledProperty<bool> IsFooterVisibleProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof(IsFooterVisible), true);
        
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Dialog>();
    
    public static readonly StyledProperty<DialogHostType> DialogHostTypeProperty =
        AvaloniaProperty.Register<Dialog, DialogHostType>(nameof(DialogHostType), DialogHostType.Overlay);
    
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    public Icon? TitleIcon
    {
        get => GetValue(TitleIconProperty);
        set => SetValue(TitleIconProperty, value);
    }
    
    [Content]
    public Control? Child
    {
        get => GetValue(ChildProperty);
        set => SetValue(ChildProperty, value);
    }
    
    public IAvaloniaDependencyResolver? DependencyResolver { get; set; }
    
    public bool InheritsTransform
    {
        get => GetValue(InheritsTransformProperty);
        set => SetValue(InheritsTransformProperty, value);
    }
    
    public bool IsLightDismissEnabled
    {
        get => GetValue(IsLightDismissEnabledProperty);
        set => SetValue(IsLightDismissEnabledProperty, value);
    }
    
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }
    
    public bool IsModal
    {
        get => GetValue(IsModalProperty);
        set => SetValue(IsModalProperty, value);
    }
    
    public bool IsResizable
    {
        get => GetValue(IsResizableProperty);
        set => SetValue(IsResizableProperty, value);
    }
    
    public bool IsClosable
    {
        get => GetValue(IsClosableProperty);
        set => SetValue(IsClosableProperty, value);
    }
    
    public bool IsMaximizable
    {
        get => GetValue(IsMaximizableProperty);
        set => SetValue(IsMaximizableProperty, value);
    }
    
    // 仅对 Window 类型的 Dialog 有效
    public bool IsMinimizable
    {
        get => GetValue(IsMinimizableProperty);
        set => SetValue(IsMinimizableProperty, value);
    }
    
    public bool IsDragMovable
    {
        get => GetValue(IsDragMovableProperty);
        set => SetValue(IsDragMovableProperty, value);
    }
    
    public Rect? PlacementRect
    {
        get => GetValue(PlacementRectProperty);
        set => SetValue(PlacementRectProperty, value);
    }
    
    [ResolveByName]
    public Control? PlacementTarget
    {
        get => GetValue(PlacementTargetProperty);
        set => SetValue(PlacementTargetProperty, value);
    }
    
    public CustomDialogPlacementCallback? CustomDialogPlacementCallback
    {
        get => GetValue(CustomPopupPlacementCallbackProperty);
        set => SetValue(CustomPopupPlacementCallbackProperty, value);
    }
    
    public bool OverlayDismissEventPassThrough
    {
        get => GetValue(OverlayDismissEventPassThroughProperty);
        set => SetValue(OverlayDismissEventPassThroughProperty, value);
    }
    
    public IInputElement? OverlayInputPassThroughElement
    {
        get => GetValue(OverlayInputPassThroughElementProperty);
        set => SetValue(OverlayInputPassThroughElementProperty, value);
    }
    
    public DialogHorizontalAnchor HorizontalStartupLocation
    {
        get => GetValue(HorizontalStartupLocationProperty);
        set => SetValue(HorizontalStartupLocationProperty, value);
    }
    
    public DialogVerticalAnchor VerticalStartupLocation
    {
        get => GetValue(VerticalStartupLocationProperty);
        set => SetValue(VerticalStartupLocationProperty, value);
    }
    
    public Dimension? HorizontalOffset
    {
        get => GetValue(HorizontalOffsetProperty);
        set => SetValue(HorizontalOffsetProperty, value);
    }
    
    public Dimension? VerticalOffset
    {
        get => GetValue(VerticalOffsetProperty);
        set => SetValue(VerticalOffsetProperty, value);
    }
    
    public bool Topmost
    {
        get => GetValue(TopmostProperty);
        set => SetValue(TopmostProperty, value);
    }
    
    public object? Result
    {
        get => GetValue(ResultProperty);
        set => SetValue(ResultProperty, value);
    }
    
    public DialogHostType DialogHostType
    {
        get => GetValue(DialogHostTypeProperty);
        set => SetValue(DialogHostTypeProperty, value);
    }
    
    public Task<object?>? ResultTask
    {
        get;
        private set;
    }
    
    public DialogStandardButtons StandardButtons
    {
        get => GetValue(StandardButtonsProperty);
        set => SetValue(StandardButtonsProperty, value);
    }
    
    public DialogStandardButton DefaultStandardButton
    {
        get => GetValue(DefaultStandardButtonProperty);
        set => SetValue(DefaultStandardButtonProperty, value);
    }
    
    public bool IsFooterVisible
    {
        get => GetValue(IsFooterVisibleProperty);
        set => SetValue(IsFooterVisibleProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public IDialogHost? Host => _openState?.DialogHost;
    IDialogHost? IDialogHostProvider.DialogHost => Host;
    
    public AvaloniaList<Button> CustomButtons { get; } = new ();
    
    #endregion

    #region 公共事件定义

    public event EventHandler? Closed;
    public event EventHandler? Opened;
    internal event EventHandler<CancelEventArgs>? Closing;
    
    public event EventHandler? Accepted;
    public event EventHandler? Rejected;
    public event EventHandler<DialogFinishedEventArgs>? Finished;
    
    event Action<IDialogHost?>? IDialogHostProvider.DialogHostChanged 
    { 
        add => _dialogHostChangedHandler += value; 
        remove => _dialogHostChangedHandler -= value;
    }
    #endregion
    
    #region 内部属性定义
    
    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => DialogToken.ID;
    
    #endregion
    
    private bool _ignoreIsOpenChanged;
    private DialogOpenState? _openState;
    private Action<IDialogHost?>? _dialogHostChangedHandler;
    private CompositeDisposable? _tokenBindingDisposables;
    private IDisposable? _modalSubscription;
    private CancellationTokenSource? _frameCancellationTokenSource;

    static Dialog()
    {
        IsHitTestVisibleProperty.OverrideDefaultValue<Dialog>(false);
        ChildProperty.Changed.AddClassHandler<Dialog>((x, e) => x.HandleChildChanged(e));
        IsOpenProperty.Changed.AddClassHandler<Dialog>((x, e) => x.HandleIsOpenChanged((AvaloniaPropertyChangedEventArgs<bool>)e));
    }

    public Dialog()
    {
        this.RegisterResources();
        CustomButtons.CollectionChanged += new NotifyCollectionChangedEventHandler(HandleCustomButtonsChanged);
    }

    private void HandleIsOpenChanged(AvaloniaPropertyChangedEventArgs<bool> e)
    {
        if (!_ignoreIsOpenChanged)
        {
            if (e.NewValue.Value)
            {
                OpenAsync();
            }
            else
            {
                Done(Result);
            }
        }
    }

    public object? Open()
    {
        if (_openState != null)
        {
            return null;
        }
        _frameCancellationTokenSource?.Cancel();
        _frameCancellationTokenSource = new CancellationTokenSource();
        var token = _frameCancellationTokenSource.Token;
        var frame = new DispatcherFrame();
        token.Register(() => frame.Continue = false);
        var resultTask = OpenAsync();
        Dispatcher.UIThread.PushFrame(frame);
        return resultTask?.Result;
    }

    public Task<object?>? OpenAsync()
    {
        if (_openState != null)
        {
            return null;
        }
        var placementTarget = PlacementTarget ?? this.FindLogicalAncestorOfType<Control>();
        Debug.Assert(placementTarget != null);
        var topLevel = TopLevel.GetTopLevel(placementTarget);
        Debug.Assert(topLevel != null);
        IDialogHost?        dialogHost              = null;
        CompositeDisposable relayBindingDisposables = new CompositeDisposable();
        DialogHost?         windowDialogHost        = null;
        OverlayDialogHost?  overlayDialogHost       = null;
        if (DialogHostType == DialogHostType.Window)
        {
            windowDialogHost = new DialogHost(topLevel, this);
            relayBindingDisposables.Add(BindUtils.RelayBind(this, TitleProperty, windowDialogHost, DialogHost.TitleProperty));
            relayBindingDisposables.Add(BindUtils.RelayBind(this, TitleIconProperty, windowDialogHost, DialogHost.IconProperty));
            relayBindingDisposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, windowDialogHost, DialogHost.IsMotionEnabledProperty));
            relayBindingDisposables.Add(BindUtils.RelayBind(this, IsResizableProperty, windowDialogHost, DialogHost.CanResizeProperty));
            relayBindingDisposables.Add(BindUtils.RelayBind(this, IsMaximizableProperty, windowDialogHost, DialogHost.CanMaximizeProperty));
            relayBindingDisposables.Add(BindUtils.RelayBind(this, IsMinimizableProperty, windowDialogHost, DialogHost.CanMinimizeProperty));
            relayBindingDisposables.Add(BindUtils.RelayBind(this, IsDragMovableProperty, windowDialogHost, DialogHost.IsMoveEnabledProperty));
            relayBindingDisposables.Add(BindUtils.RelayBind(this, StandardButtonsProperty, windowDialogHost, DialogHost.StandardButtonsProperty));
            relayBindingDisposables.Add(BindUtils.RelayBind(this, DefaultStandardButtonProperty, windowDialogHost, DialogHost.DefaultStandardButtonProperty));
            relayBindingDisposables.Add(BindUtils.RelayBind(this, IsClosableProperty, windowDialogHost, DialogHost.IsCloseCaptionButtonEnabledProperty));
            relayBindingDisposables.Add(BindUtils.RelayBind(this, IsFooterVisibleProperty, windowDialogHost, DialogHost.IsFooterVisibleProperty));
            windowDialogHost.CustomButtons.AddRange(CustomButtons);
            dialogHost                     = windowDialogHost;
        }
        else
        {
            var dialogLayer = DialogLayer.GetDialogLayer(placementTarget);
            if (dialogLayer != null)
            {
                overlayDialogHost = new OverlayDialogHost(dialogLayer, this);
                relayBindingDisposables.Add(BindUtils.RelayBind(this, TitleProperty, overlayDialogHost, OverlayDialogHost.TitleProperty));
                relayBindingDisposables.Add(BindUtils.RelayBind(this, TitleIconProperty, overlayDialogHost, OverlayDialogHost.TitleIconProperty));
                relayBindingDisposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, overlayDialogHost, OverlayDialogHost.IsMotionEnabledProperty));
                relayBindingDisposables.Add(BindUtils.RelayBind(this, IsModalProperty, overlayDialogHost, OverlayDialogHost.IsModalProperty));
                relayBindingDisposables.Add(BindUtils.RelayBind(this, IsResizableProperty, overlayDialogHost, OverlayDialogHost.IsResizableProperty));
                relayBindingDisposables.Add(BindUtils.RelayBind(this, IsClosableProperty, overlayDialogHost, OverlayDialogHost.IsClosableProperty));
                relayBindingDisposables.Add(BindUtils.RelayBind(this, IsMaximizableProperty, overlayDialogHost, OverlayDialogHost.IsMaximizableProperty));
                relayBindingDisposables.Add(BindUtils.RelayBind(this, IsDragMovableProperty, overlayDialogHost, OverlayDialogHost.IsDragMovableProperty));
                relayBindingDisposables.Add(BindUtils.RelayBind(this, StandardButtonsProperty, overlayDialogHost, OverlayDialogHost.StandardButtonsProperty));
                relayBindingDisposables.Add(BindUtils.RelayBind(this, DefaultStandardButtonProperty, overlayDialogHost, OverlayDialogHost.DefaultStandardButtonProperty));
                relayBindingDisposables.Add(BindUtils.RelayBind(this, IsFooterVisibleProperty, overlayDialogHost, OverlayDialogHost.IsFooterVisibleProperty));
                overlayDialogHost.CustomButtons.AddRange(CustomButtons);
                dialogHost = overlayDialogHost;
            }
        }
        
        Debug.Assert(dialogHost != null);
        var handlerCleanup = new CompositeDisposable(7);
        UpdateHostSizing(dialogHost, topLevel, placementTarget);
        dialogHost.Topmost = Topmost;
        dialogHost.SetChild(Child);
        ((ISetLogicalParent)dialogHost).SetParent(this);
        if (InheritsTransform)
        {
            TransformTrackingHelper.Track(placementTarget, PlacementTargetTransformChanged)
                                   .DisposeWith(handlerCleanup);
        }
        else
        {
            dialogHost.Transform = null;
        }
        if (dialogHost is DialogHost topLevelDialog)
        {
            topLevelDialog
                .Bind(
                    ThemeVariantScope.ActualThemeVariantProperty,
                    this.GetBindingObservable(ThemeVariantScope.ActualThemeVariantProperty))
                .DisposeWith(handlerCleanup);
        }
        UpdateHostPosition(dialogHost, placementTarget);
        
        SubscribeToEventHandler<IDialogHost, EventHandler<TemplateAppliedEventArgs>>(dialogHost, RootTemplateApplied,
            (x, handler) => x.TemplateApplied += handler,
            (x, handler) => x.TemplateApplied -= handler).DisposeWith(handlerCleanup);
        
        SubscribeToEventHandler<Control, EventHandler<VisualTreeAttachmentEventArgs>>(placementTarget, TargetDetached,
            (x, handler) => x.DetachedFromVisualTree += handler,
            (x, handler) => x.DetachedFromVisualTree -= handler).DisposeWith(handlerCleanup);
        if (topLevel is DialogHost parentDialogHost)
        {
            if (parentDialogHost.Parent is Dialog dialog)
            {
                SubscribeToEventHandler<Dialog, EventHandler>(dialog, ParentClosed,
                    (x, handler) => x.Closed += handler,
                    (x, handler) => x.Closed -= handler).DisposeWith(handlerCleanup);
            }
        }
        else if (topLevel is Window window)
        {
            SubscribeToEventHandler<Window, EventHandler>(window, ParentClosed,
                (x, handler) => x.Closed += handler,
                (x, handler) => x.Closed -= handler).DisposeWith(handlerCleanup);
        } 
        var inputManager = AvaloniaLocator.Current.GetService<IInputManager>();
        inputManager?.Process.Subscribe(ListenForNonClientClick).DisposeWith(handlerCleanup);
        var cleanupPopup = Disposable.Create((dialogHost, handlerCleanup), state =>
        {
            state.handlerCleanup.Dispose();

            state.dialogHost.SetChild(null);

            if (DialogHostType == DialogHostType.Overlay)
            {
                overlayDialogHost?.Hide();
            }
            else
            {
                windowDialogHost?.Close();
            }
            
            ((ISetLogicalParent)state.dialogHost).SetParent(null);
            state.dialogHost.Dispose();
            relayBindingDisposables.Dispose();
        });
        
        if (IsLightDismissEnabled)
        {
            var dismissLayer = LightDismissOverlayLayer.GetLightDismissOverlayLayer(placementTarget);

            if (dismissLayer != null)
            {
                dismissLayer.IsVisible               = true;
                dismissLayer.InputPassThroughElement = OverlayInputPassThroughElement;
                    
                Disposable.Create(() =>
                {
                    dismissLayer.IsVisible               = false;
                    dismissLayer.InputPassThroughElement = null;
                }).DisposeWith(handlerCleanup);
                
                SubscribeToEventHandler<LightDismissOverlayLayer, EventHandler<PointerPressedEventArgs>>(
                    dismissLayer,
                    PointerPressedDismissOverlay,
                    (x, handler) => x.PointerPressed += handler,
                    (x, handler) => x.PointerPressed -= handler).DisposeWith(handlerCleanup);
            }
        }
        
        _openState = new DialogOpenState(placementTarget, topLevel, dialogHost, cleanupPopup);

        if (dialogHost is DialogHost windowDialog)
        {
            windowDialog.Focus();
            if (IsModal)
            {
                if (topLevel is Window windowTopLevel)
                {
                    windowDialog.ShowDialog(windowTopLevel);
                }
            }
            else
            {
                windowDialog.Show();
            }
       
        }
        else
        {
            dialogHost.Show();
        }
        
        if (IsModal)
        {
            var tcs = new TaskCompletionSource<object?>();

            var disposables = new CompositeDisposable(
            [
                Observable.FromEventPattern(
                              x => Closed += x,
                              x => Closed -= x)
                          .Take(1)
                          .Subscribe(_ =>
                          {
                              _modalSubscription?.Dispose();
                          }),
                Disposable.Create(() =>
                {
                    _modalSubscription = null;
                    // owner!.Activate();
                    tcs.SetResult(Result);
                })
            ]);

            _modalSubscription = disposables;
            ResultTask             = tcs.Task;
        }
        
        using (BeginIgnoringIsOpen())
        {
            SetCurrentValue(IsOpenProperty, true);
        }

        Opened?.Invoke(this, EventArgs.Empty);
        _dialogHostChangedHandler?.Invoke(Host);
        return ResultTask;
    }

    public void Accept()
    {
        Result = DialogCode.Accepted;
        NotifyClose();
    }

    public void Reject()
    {
        Result = DialogCode.Rejected;
        NotifyClose();
    }

    public void Done(object? dialogResult)
    {
        Result = dialogResult;
        NotifyClose();
    }

    public void Done()
    {
        NotifyClose();
    }

    protected virtual void NotifyClose()
    {
        var closingArgs = new CancelEventArgs();
        Closing?.Invoke(this, closingArgs);
        if (closingArgs.Cancel)
        {
            return;
        }

        if (Result is DialogCode code)
        {
            if (code == DialogCode.Accepted)
            {
                Accepted?.Invoke(this, EventArgs.Empty);
            }
            else if (code == DialogCode.Rejected)
            {
                Rejected?.Invoke(this, EventArgs.Empty);
            }
        }
        Finished?.Invoke(this, new DialogFinishedEventArgs(Result));
        
        if (_openState is null)
        {
            using (BeginIgnoringIsOpen())
            {
                SetCurrentValue(IsOpenProperty, false);
            }

            return;
        }
        _frameCancellationTokenSource?.Cancel();
        _frameCancellationTokenSource = null;
        _openState.Dispose();
        _openState = null;
        
        _dialogHostChangedHandler?.Invoke(null);
        _modalSubscription?.Dispose();
        _modalSubscription = null;
        using (BeginIgnoringIsOpen())
        {
            SetCurrentValue(IsOpenProperty, false);
        }

        Closed?.Invoke(this, EventArgs.Empty);
    }
    
    protected override Size MeasureCore(Size availableSize)
    {
        return new Size();
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _tokenBindingDisposables?.Dispose();
        _tokenBindingDisposables = new CompositeDisposable();
        _tokenBindingDisposables.Add(TokenResourceBinder.CreateTokenBinding(this, IsMotionEnabledProperty, SharedTokenKey.EnableMotion));
        _tokenBindingDisposables.Add(TokenResourceBinder.CreateTokenBinding(this, MinHeightProperty, DialogTokenKey.MinHeight));
        _tokenBindingDisposables.Add(TokenResourceBinder.CreateTokenBinding(this, MinWidthProperty, DialogTokenKey.MinWidth));
        NotifyCreateTokenBindings(_tokenBindingDisposables);
    }

    protected virtual void NotifyCreateTokenBindings(CompositeDisposable compositeDisposables)
    {
        
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        Done(null);
        _tokenBindingDisposables?.Dispose();
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (_openState is not null)
        {
            if (change.Property == WidthProperty ||
                change.Property == MinWidthProperty ||
                change.Property == MaxWidthProperty ||
                change.Property == HeightProperty ||
                change.Property == MinHeightProperty ||
                change.Property == MaxHeightProperty)
            {
                UpdateHostSizing(_openState.DialogHost, _openState.TopLevel, _openState.PlacementTarget);
            }
            else if (change.Property == PlacementTargetProperty ||
                     change.Property == HorizontalAlignmentProperty ||
                     change.Property == VerticalAlignmentProperty ||
                     change.Property == HorizontalOffsetProperty ||
                     change.Property == VerticalOffsetProperty ||
                     change.Property == PlacementRectProperty)
            {
                if (change.Property == PlacementTargetProperty)
                {
                    var newTarget = change.GetNewValue<Control?>() ?? this.FindLogicalAncestorOfType<Control>();

                    if (newTarget is null || newTarget.GetVisualRoot() != _openState.TopLevel)
                    {
                        Done();
                        return;
                    }

                    _openState.PlacementTarget = newTarget;
                }

                UpdateHostPosition(_openState.DialogHost, _openState.PlacementTarget);
            }
            else if (change.Property == TopmostProperty)
            {
                _openState.DialogHost.Topmost = change.GetNewValue<bool>();
            }
        }
    }
    
    internal void SetDialogParent(Control? newParent)
    {
        if (Parent != null && Parent != newParent)
        {
            ((ISetLogicalParent)this).SetParent(null);
        }
    
        if (Parent == null || PlacementTarget != newParent)
        {
            ((ISetLogicalParent)this).SetParent(newParent);
            this.SetTemplatedParent(newParent?.TemplatedParent);
        }
    }
    
    private static IDisposable SubscribeToEventHandler<T, TEventHandler>(T target, TEventHandler handler, Action<T, TEventHandler> subscribe, Action<T, TEventHandler> unsubscribe)
    {
        subscribe(target, handler);

        return Disposable.Create((unsubscribe, target, handler), state => state.unsubscribe(state.target, state.handler));
    }
    
    private void HandleChildChanged(AvaloniaPropertyChangedEventArgs e)
    {
        LogicalChildren.Clear();

        ((ISetLogicalParent?)e.OldValue)?.SetParent(null);

        if (e.NewValue != null)
        {
            ((ISetLogicalParent)e.NewValue).SetParent(this);
            LogicalChildren.Add((ILogical)e.NewValue);
        }
    }
    
    private void ListenForNonClientClick(RawInputEventArgs e)
    {
        var mouse = e as RawPointerEventArgs;

        if (IsLightDismissEnabled && mouse?.Type == RawPointerEventType.NonClientLeftButtonDown)
        {
            NotifyClose();
        }
    }
    
    private void PointerPressedDismissOverlay(object? sender, PointerPressedEventArgs e)
    {
        if (IsLightDismissEnabled && e.Source is Visual v && !IsChildOrThis(v))
        {
            if (OverlayDismissEventPassThrough)
            {
                PassThroughEvent(e);
            }

            // Ensure the popup is closed if it was not closed by a pass-through event handler
            if (IsOpen)
            {
                NotifyClose();
            }
        }
    }
    
    private static void PassThroughEvent(PointerPressedEventArgs e)
    {
        if (e.Source is LightDismissOverlayLayer layer &&
            layer.GetVisualRoot() is InputElement root)
        {
            var p   = e.GetCurrentPoint(root);
            var hit = root.InputHitTest(p.Position, x => x != layer);

            if (hit != null)
            {
                e.Pointer.Capture(hit);
                hit.RaiseEvent(e);
                e.Handled = true;
            }
        }
    }
    
    private void RootTemplateApplied(object? sender, TemplateAppliedEventArgs e)
    {
        if (_openState is null)
        {
            return;
        }

        var popupHost = _openState.DialogHost;

        popupHost.TemplateApplied -= RootTemplateApplied;

        _openState.SetPresenterSubscription(null);

        // If the Popup appears in a control template, then the child controls
        // that appear in the popup host need to have their TemplatedParent
        // properties set.
        if (TemplatedParent != null && popupHost.Presenter is Control presenter)
        {
            presenter.ApplyTemplate();

            var presenterSubscription = presenter.GetObservable(ContentPresenter.ChildProperty)
                                                 .Subscribe(SetTemplatedParentAndApplyChildTemplates);

            _openState.SetPresenterSubscription(presenterSubscription);
        }
    }

    
    private void SetTemplatedParentAndApplyChildTemplates(Control? control)
    {
        if (control != null)
        {
            TemplatedControlUtils.ApplyTemplatedParent(control, TemplatedParent);
        }
    }
    
    private bool IsChildOrThis(Visual child)
    {
        if (_openState is null)
        {
            return false;
        }

        var dialogHost = _openState.DialogHost;

        Visual? root = child.GetVisualRoot() as Visual;
            
        while (root is IHostedVisualTreeRoot hostedRoot)
        {
            if (root == dialogHost)
            {
                return true;
            }

            root = hostedRoot.Host?.GetVisualRoot() as Visual;
        }

        return false;
    }
    
    public bool IsInsideDialog(Visual visual)
    {
        if (_openState is null)
        {
            return false;
        }

        var dialogHost = _openState.DialogHost;
        return ((Visual)dialogHost).IsVisualAncestorOf(visual);
    }
    
    public bool IsPointerOverDialog => ((IInputElement?)_openState?.DialogHost)?.IsPointerOver ?? false;
    
    private void ParentClosed(object? sender, EventArgs e)
    {
        Done(null);
    }
    
    private void PlacementTargetTransformChanged(Visual v, Matrix? matrix)
    {
        if (_openState is not null)
        {
            UpdateHostSizing(_openState.DialogHost, _openState.TopLevel, _openState.PlacementTarget);
        }
    }
    
    private void UpdateHostPosition(IDialogHost dialogHost, Control placementTarget)
    {
        dialogHost.ConfigurePosition(new DialogPositionRequest(
            placementTarget,
            HorizontalOffset ?? new Dimension(0),
            VerticalOffset ?? new Dimension(0),
            PlacementRect ?? new Rect(default, placementTarget.Bounds.Size),
            CustomDialogPlacementCallback));
    }
    
    private void UpdateHostSizing(IDialogHost dialogHost, TopLevel topLevel, Control placementTarget)
    {
        var scaleX = 1.0;
        var scaleY = 1.0;

        if (InheritsTransform && placementTarget.TransformToVisual(topLevel) is { } m)
        {
            scaleX = Math.Sqrt(m.M11 * m.M11 + m.M12 * m.M12);
            scaleY = Math.Sqrt(m.M11 * m.M11 + m.M12 * m.M12);

            // Ideally we'd only assign a ScaleTransform here when the scale != 1, but there's
            // an issue with LayoutTransformControl in that it sets its LayoutTransform property
            // with LocalValue priority in ArrangeOverride in certain cases when LayoutTransform
            // is null, which breaks TemplateBindings to this property. Offending commit/line:
            //
            // https://github.com/AvaloniaUI/Avalonia/commit/6fbe1c2180ef45a940e193f1b4637e64eaab80ed#diff-5344e793df13f462126a8153ef46c44194f244b6890f25501709bae51df97f82R54
            dialogHost.Transform = new ScaleTransform(scaleX, scaleY);
        }
        else
        {
            dialogHost.Transform = null;
        }

        dialogHost.Width     = Width * scaleX;
        dialogHost.MinWidth  = MinWidth * scaleX;
        dialogHost.MaxWidth  = MaxWidth * scaleX;
        dialogHost.Height    = Height * scaleY;
        dialogHost.MinHeight = MinHeight * scaleY;
        dialogHost.MaxHeight = MaxHeight * scaleY;
    }
    
    private void TargetDetached(object? sender, VisualTreeAttachmentEventArgs e)
    {
        Done();
    }

    internal void NotifyDialogHostMeasured(Size size, Rect bounds)
    {
        Size boundSize = bounds.Size;
        if (HorizontalOffset == null && HorizontalStartupLocation != DialogHorizontalAnchor.Custom)
        {
            if (HorizontalStartupLocation == DialogHorizontalAnchor.Left)
            {
                SetCurrentValue(HorizontalOffsetProperty, new Dimension(0));
            }
            else if (HorizontalStartupLocation == DialogHorizontalAnchor.Right)
            {
                SetCurrentValue(HorizontalOffsetProperty, new Dimension(boundSize.Width - size.Width));
            }
            else if (HorizontalStartupLocation == DialogHorizontalAnchor.Center)
            {
                SetCurrentValue(HorizontalOffsetProperty, new Dimension((boundSize.Width - size.Width) / 2));
            }
        }
        
        if (VerticalOffset == null && VerticalStartupLocation != DialogVerticalAnchor.Custom)
        {
            if (VerticalStartupLocation == DialogVerticalAnchor.Top)
            {
                SetCurrentValue(VerticalOffsetProperty, new Dimension(0));
            }
            else if (VerticalStartupLocation == DialogVerticalAnchor.Bottom)
            {
                SetCurrentValue(VerticalOffsetProperty, new Dimension(boundSize.Height - size.Height));
            }
            else if (VerticalStartupLocation == DialogVerticalAnchor.Center)
            {
                SetCurrentValue(VerticalOffsetProperty, new Dimension((boundSize.Height - size.Height) / 2));
            }
        }
    }

    internal void NotifyDialogHostCloseRequest()
    {
        Done();
    }
    
    private IgnoreIsOpenScope BeginIgnoringIsOpen()
    {
        return new IgnoreIsOpenScope(this);
    }
    
    private readonly struct IgnoreIsOpenScope : IDisposable
    {
        private readonly Dialog _owner;

        public IgnoreIsOpenScope(Dialog owner)
        {
            _owner                      = owner;
            _owner._ignoreIsOpenChanged = true;
        }

        public void Dispose()
        {
            _owner._ignoreIsOpenChanged = false;
        }
    }

    private class DialogOpenState : IDisposable
    {
        private readonly IDisposable _cleanup;
        private IDisposable? _presenterCleanup;
        private Control _placementTarget;
        
        public DialogOpenState(Control placementTarget, TopLevel topLevel, IDialogHost dialogHost, IDisposable cleanup)
        {
            PlacementTarget = placementTarget;
            TopLevel        = topLevel;
            DialogHost      = dialogHost;
            _cleanup        = cleanup;
        }
        
        public Rect LastPlacementTargetBounds { get; set; }
        public IDialogHost DialogHost { get; }
        public TopLevel TopLevel { get; }
        
        public Control PlacementTarget
        {
            get => _placementTarget;
            [MemberNotNull(nameof(_placementTarget))]
            set
            {
                _placementTarget          = value;
                LastPlacementTargetBounds = value.Bounds;
            }
        }
        
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
    
    private void HandleCustomButtonsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (!IsOpen || _openState == null)
        {
            return;
        }

        AvaloniaList<Button>? targetButtons = null;
        if (DialogHostType == DialogHostType.Overlay)
        {
            if (_openState.DialogHost is OverlayDialogHost overlayDialogHost)
            {
                targetButtons = overlayDialogHost.CustomButtons;
            }
            else if (_openState.DialogHost is DialogHost dialogHost)
            {
                targetButtons = dialogHost.CustomButtons;
            }
        }
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                var newItems = e.NewItems!.OfType<Button>();
                targetButtons?.AddRange(newItems);
                break;
            case NotifyCollectionChangedAction.Remove:
                var oldItems = e.OldItems!.OfType<Button>();
                targetButtons?.RemoveAll(oldItems);
                break;
            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Move:
            case NotifyCollectionChangedAction.Reset:
                throw new NotSupportedException();
        }
    }
}