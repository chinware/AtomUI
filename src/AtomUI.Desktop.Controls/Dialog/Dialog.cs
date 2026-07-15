using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Reflection;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Metadata;
using Avalonia.Input;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

public partial class Dialog : TemplatedControl,
                              IDialogHostProvider,
                              IMotionAwareControl,
                              IDialog
{
    #region 公共属性定义
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<Dialog, string?>(nameof(Title));

    public static readonly StyledProperty<PathIcon?> TitleIconProperty =
        AvaloniaProperty.Register<Dialog, PathIcon?>(nameof(TitleIcon));

    public static readonly StyledProperty<object?> ContentProperty = ContentPresenter.ContentProperty.AddOwner<Dialog>();

    public static readonly StyledProperty<IDataTemplate?> ContentTemplateProperty =
        ContentPresenter.ContentTemplateProperty.AddOwner<Dialog>();

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<Dialog, bool>(
            nameof(IsOpen),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<bool> IsModalProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof(IsModal), true);

    public static readonly StyledProperty<bool> IsResizableProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof(IsResizable));

    public static readonly StyledProperty<bool> IsClosableProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof(IsClosable), true);

    public static readonly StyledProperty<bool> IsMaximizableProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof(IsMaximizable));

    public static readonly StyledProperty<bool> IsMinimizableProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof(IsMinimizable), true);

    public static readonly StyledProperty<bool> IsDragMovableProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof(IsDragMovable), true);

    public static readonly StyledProperty<Control?> PlacementTargetProperty =
        AvaloniaProperty.Register<Dialog, Control?>(nameof(PlacementTarget));

    public static readonly StyledProperty<DialogHorizontalAnchor> HorizontalStartupLocationProperty =
        AvaloniaProperty.Register<Dialog, DialogHorizontalAnchor>(nameof(HorizontalStartupLocation),
            DialogHorizontalAnchor.Custom);

    public static readonly StyledProperty<DialogVerticalAnchor> VerticalStartupLocationProperty =
        AvaloniaProperty.Register<Dialog, DialogVerticalAnchor>(nameof(VerticalStartupLocation),
            DialogVerticalAnchor.Custom);

    public static readonly StyledProperty<Dimension?> HorizontalOffsetProperty =
        AvaloniaProperty.Register<Dialog, Dimension?>(nameof(HorizontalOffset));

    public static readonly StyledProperty<Dimension?> VerticalOffsetProperty =
        AvaloniaProperty.Register<Dialog, Dimension?>(nameof(VerticalOffset));

    public static readonly StyledProperty<bool> IsTopmostProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof(IsTopmost));

    public static readonly StyledProperty<object?> ResultProperty =
        AvaloniaProperty.Register<Dialog, object?>(nameof(Result));

    public static readonly StyledProperty<DialogStandardButtons> StandardButtonsProperty =
        DialogButtonBox.StandardButtonsProperty.AddOwner<Dialog>();

    public static readonly StyledProperty<DialogStandardButton> DefaultStandardButtonProperty =
        DialogButtonBox.DefaultStandardButtonProperty.AddOwner<Dialog>();

    public static readonly StyledProperty<DialogStandardButton> EscapeStandardButtonProperty =
        DialogButtonBox.EscapeStandardButtonProperty.AddOwner<Dialog>();

    public static readonly StyledProperty<bool> IsFooterVisibleProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof(IsFooterVisible), true);

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Dialog>();

    public static readonly StyledProperty<DialogHostType> DialogHostTypeProperty =
        AvaloniaProperty.Register<Dialog, DialogHostType>(nameof(DialogHostType), DialogHostType.Overlay);

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof(IsLoading));

    public static readonly StyledProperty<bool> IsConfirmLoadingProperty =
        AvaloniaProperty.Register<Dialog, bool>(nameof(IsConfirmLoading));

    // Dialog 控件自身（TemplatedControl）从 Layoutable 继承的 Width / MinWidth 等尺寸属性
    // 在这里语义上是死属性 —— Dialog 仅作为状态持有者，不直接参与可视布局。下面 6 个
    // Host* 属性用于声明"希望 host（DialogHost / OverlayDialogHost）采用的尺寸约束"，
    // 所有 host 的 sizing 逻辑都从这 6 个读取。
    public static readonly StyledProperty<double> HostWidthProperty =
        AvaloniaProperty.Register<Dialog, double>(nameof(HostWidth), double.NaN);

    public static readonly StyledProperty<double> HostHeightProperty =
        AvaloniaProperty.Register<Dialog, double>(nameof(HostHeight), double.NaN);

    public static readonly StyledProperty<double> HostMinWidthProperty =
        AvaloniaProperty.Register<Dialog, double>(nameof(HostMinWidth), 0d);

    public static readonly StyledProperty<double> HostMinHeightProperty =
        AvaloniaProperty.Register<Dialog, double>(nameof(HostMinHeight), 0d);

    public static readonly StyledProperty<double> HostMaxWidthProperty =
        AvaloniaProperty.Register<Dialog, double>(nameof(HostMaxWidth), double.PositiveInfinity);

    public static readonly StyledProperty<double> HostMaxHeightProperty =
        AvaloniaProperty.Register<Dialog, double>(nameof(HostMaxHeight), double.PositiveInfinity);

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public PathIcon? TitleIcon
    {
        get => GetValue(TitleIconProperty);
        set => SetValue(TitleIconProperty, value);
    }

    [Content]
    [DependsOn(nameof(ContentTemplate))]
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public IDataTemplate? ContentTemplate
    {
        get => GetValue(ContentTemplateProperty);
        set => SetValue(ContentTemplateProperty, value);
    }

    public IAvaloniaDependencyResolver? DependencyResolver { get; set; }

    internal DialogMotionAnchorMode MotionAnchorMode { get; set; }

    internal bool UsesPlacementTargetAsMotionAnchor =>
        MotionAnchorMode switch
        {
            DialogMotionAnchorMode.ExplicitPlacementTarget => PlacementTarget is not null,
            DialogMotionAnchorMode.FallbackPlacementTarget => false,
            _ => PlacementTarget is not null
        };

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

    [ResolveByName]
    public Control? PlacementTarget
    {
        get => GetValue(PlacementTargetProperty);
        set => SetValue(PlacementTargetProperty, value);
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

    public bool IsTopmost
    {
        get => GetValue(IsTopmostProperty);
        set => SetValue(IsTopmostProperty, value);
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

    public DialogStandardButton EscapeStandardButton
    {
        get => GetValue(EscapeStandardButtonProperty);
        set => SetValue(EscapeStandardButtonProperty, value);
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

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public bool IsConfirmLoading
    {
        get => GetValue(IsConfirmLoadingProperty);
        set => SetValue(IsConfirmLoadingProperty, value);
    }

    public double HostWidth
    {
        get => GetValue(HostWidthProperty);
        set => SetValue(HostWidthProperty, value);
    }

    public double HostHeight
    {
        get => GetValue(HostHeightProperty);
        set => SetValue(HostHeightProperty, value);
    }

    public double HostMinWidth
    {
        get => GetValue(HostMinWidthProperty);
        set => SetValue(HostMinWidthProperty, value);
    }

    public double HostMinHeight
    {
        get => GetValue(HostMinHeightProperty);
        set => SetValue(HostMinHeightProperty, value);
    }

    public double HostMaxWidth
    {
        get => GetValue(HostMaxWidthProperty);
        set => SetValue(HostMaxWidthProperty, value);
    }

    public double HostMaxHeight
    {
        get => GetValue(HostMaxHeightProperty);
        set => SetValue(HostMaxHeightProperty, value);
    }

    public IDialogHost? Host => _openState?.DialogHost;
    IDialogHost? IDialogHostProvider.DialogHost => Host;

    public AvaloniaList<DialogButton> CustomButtons { get; } = new();
    public Action<IReadOnlyList<DialogButton>>? ButtonsConfigure { get; set; }

    #endregion

    #region 公共事件定义

    public event EventHandler? Closed;
    public event EventHandler? Opened;
    public event EventHandler<CancelEventArgs>? Closing;
    public event EventHandler? Accepted;
    public event EventHandler? Rejected;
    public event EventHandler<DialogFinishedEventArgs>? Finished;
    public event EventHandler<DialogButtonClickedEventArgs>? ButtonClicked;

    event Action<IDialogHost?>? IDialogHostProvider.DialogHostChanged
    {
        add => _dialogHostChangedHandler += value;
        remove => _dialogHostChangedHandler -= value;
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<Dialog, double> OffsetXProperty =
        AvaloniaProperty.RegisterDirect<Dialog, double>(
            nameof(OffsetX),
            o => o.OffsetX,
            (o, v) => o.OffsetX = v);

    internal static readonly DirectProperty<Dialog, double> OffsetYProperty =
        AvaloniaProperty.RegisterDirect<Dialog, double>(
            nameof(OffsetY),
            o => o.OffsetY,
            (o, v) => o.OffsetY = v);

    internal static readonly DirectProperty<Dialog, bool> EffectiveMinimizableProperty =
        AvaloniaProperty.RegisterDirect<Dialog, bool>(
            nameof(EffectiveMinimizable),
            o => o.EffectiveMinimizable,
            (o, v) => o.EffectiveMinimizable = v);

    private double _offsetX;

    internal double OffsetX
    {
        get => _offsetX;
        set => SetAndRaise(OffsetXProperty, ref _offsetX, value);
    }

    private double _offsetY;

    internal double OffsetY
    {
        get => _offsetY;
        set => SetAndRaise(OffsetYProperty, ref _offsetY, value);
    }

    private bool _effectiveMinimizable;

    internal bool EffectiveMinimizable
    {
        get => _effectiveMinimizable;
        set => SetAndRaise(EffectiveMinimizableProperty, ref _effectiveMinimizable, value);
    }

    #endregion

    private bool _ignoreIsOpenChanged;
    private DialogOpenState? _openState;
    private Action<IDialogHost?>? _dialogHostChangedHandler;
    private CancellationTokenSource? _frameCancellationTokenSource;
    private DispatcherFrame? _synchronousOpenFrame;
    private bool _opening;
    private bool _closing;
    private Func<DialogClosingContext, ValueTask<bool>>? BeforeCloseAsync { get; set; }
    private CancellationToken _openCancellationToken;
    private IReadOnlyList<DialogButton> _synchronizedButtons = Array.Empty<DialogButton>();

    static Dialog()
    {
        IsHitTestVisibleProperty.OverrideDefaultValue<Dialog>(false);
        IsOpenProperty.Changed.AddClassHandler<Dialog>((x, e) => x.HandleIsOpenChanged((AvaloniaPropertyChangedEventArgs<bool>)e));
    }

    public Dialog()
    {
        CustomButtons.CollectionChanged += HandleCustomButtonsChanged;
        SetCurrentValue(EffectiveMinimizableProperty, IsMinimizable);
    }

    private void HandleIsOpenChanged(AvaloniaPropertyChangedEventArgs<bool> e)
    {
        if (_ignoreIsOpenChanged)
        {
            return;
        }

        if (e.NewValue.Value)
        {
            Dispatcher.InvokeAsync(() => OpenAsync());
        }
        else
        {
            RequestClose(DialogCloseRequest.OpenStateChanged(Result));
        }
    }

    public object? Open()
    {
        if (_openState != null || _opening || _closing)
        {
            return null;
        }

        _frameCancellationTokenSource?.Cancel();
        _frameCancellationTokenSource?.Dispose();
        _frameCancellationTokenSource = new CancellationTokenSource();
        var frame = new DispatcherFrame();
        _synchronousOpenFrame = frame;
        _frameCancellationTokenSource.Token.Register(() => frame.Continue = false);
        DialogInputCaptureTracker.ReleaseCurrentMouseCapture();
        Dispatcher.InvokeAsync(async () => await OpenAsync(_frameCancellationTokenSource.Token));
        try
        {
            Dispatcher.PushFrame(frame);
        }
        finally
        {
            if (ReferenceEquals(_synchronousOpenFrame, frame))
            {
                _synchronousOpenFrame = null;
            }
        }
        return Result;
    }

    public async Task OpenAsync(CancellationToken cancellationToken = default)
    {
        if (_openState != null || _opening || _closing)
        {
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();
        _openCancellationToken = cancellationToken;
        _opening = true;

        try
        {
            var placementTarget = ResolvePlacementTarget();
            var topLevel        = TopLevel.GetTopLevel(placementTarget);
            if (topLevel is null)
            {
                throw new InvalidOperationException("Unable to resolve TopLevel for Dialog.");
            }

            var disposables          = new CompositeDisposable();
            var ownershipTransferred = false;
            try
            {
                var effectiveHostType = ResolveDialogHostType(DialogHostType);
                var dialogHost = effectiveHostType == DialogHostType.Window
                    ? CreateDialogHost(topLevel, this)
                    : CreateOverlayDialogHost(placementTarget, this);

                dialogHost.BindDialog(this, disposables);
                dialogHost.CustomButtons.AddRange(CustomButtons);

                dialogHost.Content         = Content;
                dialogHost.ContentTemplate = ContentTemplate;
                dialogHost.UpdateSizing();
                dialogHost.Topmost         = IsTopmost;

                SubscribeToEventHandler<IDialogHost, EventHandler<TemplateAppliedEventArgs>>(dialogHost, RootTemplateApplied,
                    (x, handler) => x.TemplateApplied += handler,
                    (x, handler) => x.TemplateApplied -= handler).DisposeWith(disposables);

                SubscribeToEventHandler<Control, EventHandler<VisualTreeAttachmentEventArgs>>(placementTarget, TargetDetached,
                    (x, handler) => x.DetachedFromVisualTree += handler,
                    (x, handler) => x.DetachedFromVisualTree -= handler).DisposeWith(disposables);

                if (topLevel is Window parentWindow)
                {
                    SubscribeToEventHandler<Window, EventHandler>(parentWindow, ParentClosed,
                    (x, handler) => x.Closed += handler,
                    (x, handler) => x.Closed -= handler).DisposeWith(disposables);
                }

                dialogHost.AttachPlacement(placementTarget);

                var openState = new DialogOpenState(dialogHost,
                    disposables);
                _openState           = openState;
                ownershipTransferred = true;
                _dialogHostChangedHandler?.Invoke(dialogHost);

                using (BeginIgnoringIsOpen())
                {
                    SetCurrentValue(IsOpenProperty, true);
                }

                if (dialogHost is DialogHost windowDialog &&
                    IsModal &&
                    topLevel is Window ownerWindow &&
                    RuntimePlatform.Features.SupportsWindowModalDialog)
                {
                    // Window 宿主 + modal：必须用 ShowDialog() 拿 OS 级 modal 语义
                    // （父窗禁用、焦点限制、macOS 下表现为 sheet）。
                    // 仅用 Show() 再 await ClosedTask 只是应用层等待，父窗仍然可交互。
                    Opened?.Invoke(this, EventArgs.Empty);
                    await windowDialog.ShowDialog(ownerWindow).WaitAsync(cancellationToken);
                }
                else
                {
                    dialogHost.Show();
                    Opened?.Invoke(this, EventArgs.Empty);

                    if (IsModal)
                    {
                        await openState.ClosedTask.WaitAsync(cancellationToken);
                    }
                }
            }
            catch when (!ownershipTransferred)
            {
                // openState 还没接管 disposables；这里手动释放避免事件订阅 / binding 残留。
                // ownershipTransferred 之后抛异常的分支不走这里，由 _openState.Dispose() 负责清理。
                disposables.Dispose();
                throw;
            }
        }
        finally
        {
            _opening = false;
        }
    }

    private protected virtual IDialogHost CreateDialogHost(TopLevel topLevel, Dialog dialog)
    {
        return new DialogHost(topLevel, dialog);
    }

    private protected virtual IDialogHost CreateOverlayDialogHost(Control placementTarget, Dialog dialog)
    {
        return new OverlayDialogHost(placementTarget, dialog, DependencyResolver);
    }

    private static DialogHostType ResolveDialogHostType(DialogHostType requestedHostType)
    {
        if (requestedHostType == DialogHostType.Window && !RuntimePlatform.Features.SupportsNativeWindow)
        {
            return DialogHostType.Overlay;
        }

        return requestedHostType;
    }

    public void Accept()
    {
        RequestClose(DialogCloseRequest.Accepted(null));
    }

    public void Reject()
    {
        RequestClose(DialogCloseRequest.Rejected(null));
    }

    public void Done(object? dialogResult)
    {
        RequestClose(DialogCloseRequest.Programmatic(dialogResult));
    }

    public void Done()
    {
        RequestClose(DialogCloseRequest.Programmatic(Result));
    }

    private void RequestClose(DialogCloseRequest request)
    {
        if (IsConfirmLoading || _closing)
        {
            return;
        }

        _closing = true;
        var closeCompletionPending = false;
        var previousResult         = Result;

        try
        {
            SetCurrentValue(ResultProperty, request.Result);

            if (!NotifyClosing())
            {
                CancelCloseRequest(request, previousResult);
                return;
            }

            if (BeforeCloseAsync is null)
            {
                CommitClose(request.Result, ref closeCompletionPending);
                return;
            }

            var beforeCloseTask = CreateBeforeCloseTask(request);
            if (beforeCloseTask.IsCompleted)
            {
                if (!ReadBeforeCloseResult(beforeCloseTask))
                {
                    CancelCloseRequest(request, previousResult);
                    return;
                }

                CommitClose(request.Result, ref closeCompletionPending);
                return;
            }

            closeCompletionPending = true;
            _ = ContinueCloseAsync(request, previousResult, beforeCloseTask);
        }
        finally
        {
            if (!closeCompletionPending)
            {
                _closing = false;
            }
        }
    }

    private bool NotifyClosing()
    {
        var closingArgs = new CancelEventArgs();
        Closing?.Invoke(this, closingArgs);
        return !closingArgs.Cancel;
    }

    private async Task ContinueCloseAsync(
        DialogCloseRequest request,
        object? previousResult,
        ValueTask<bool> beforeCloseTask)
    {
        var closeCompletionPending = false;
        try
        {
            if (!await ReadBeforeCloseResultAsync(beforeCloseTask))
            {
                CancelCloseRequest(request, previousResult);
                return;
            }

            CommitClose(request.Result, ref closeCompletionPending);
        }
        finally
        {
            if (!closeCompletionPending)
            {
                _closing = false;
            }
        }
    }

    private ValueTask<bool> CreateBeforeCloseTask(DialogCloseRequest request)
    {
        var context = new DialogClosingContext(
            this,
            request.Result,
            request.Reason,
            request.SourceButton,
            _openCancellationToken);
        try
        {
            return BeforeCloseAsync?.Invoke(context) ?? ValueTask.FromResult(true);
        }
        catch (Exception ex)
        {
            TraceBeforeCloseException(ex);
            return ValueTask.FromResult(false);
        }
    }

    private static bool ReadBeforeCloseResult(ValueTask<bool> beforeCloseTask)
    {
        try
        {
            return beforeCloseTask.GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            TraceBeforeCloseException(ex);
            return false;
        }
    }

    private static async ValueTask<bool> ReadBeforeCloseResultAsync(ValueTask<bool> beforeCloseTask)
    {
        try
        {
            return await beforeCloseTask;
        }
        catch (Exception ex)
        {
            TraceBeforeCloseException(ex);
            return false;
        }
    }

    private static void TraceBeforeCloseException(Exception exception)
    {
        Debug.WriteLine($"Dialog before-close callback failed: {exception}");
    }

    private void CommitClose(object? result, ref bool closeCompletionPending)
    {
        if (result is DialogCode code)
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

        Finished?.Invoke(this, new DialogFinishedEventArgs(result));
        if (DataContext is IDialogAwareDataContext dialogAwareDataContext)
        {
            dialogAwareDataContext.NotifyClosed();
        }

        var openState = _openState;
        _openState    = null;
        _dialogHostChangedHandler?.Invoke(null);

        using (BeginIgnoringIsOpen())
        {
            SetCurrentValue(IsOpenProperty, false);
        }

        StopSynchronousOpenFrame();
        if (openState is not null)
        {
            var closeCompletedSynchronously = false;
            openState.Close(() =>
            {
                closeCompletedSynchronously = true;
                CompleteClose(openState, result);
            });
            closeCompletionPending = !closeCompletedSynchronously;
        }
        else
        {
            CompleteClose(null, result);
        }
    }

    private void RestoreResult(object? previousResult)
    {
        SetCurrentValue(ResultProperty, previousResult);
    }

    private void CancelCloseRequest(DialogCloseRequest request, object? previousResult)
    {
        RestoreResult(previousResult);
        if (request.RestoreIsOpenOnCancel)
        {
            using (BeginIgnoringIsOpen())
            {
                SetCurrentValue(IsOpenProperty, true);
            }
        }
    }

    private void StopSynchronousOpenFrame()
    {
        if (_synchronousOpenFrame is { } frame)
        {
            frame.Continue = false;
        }
    }

    private void CompleteClose(DialogOpenState? openState, object? result)
    {
        openState?.SetClosed(result);
        _synchronizedButtons = Array.Empty<DialogButton>();
        Closed?.Invoke(this, EventArgs.Empty);
        _frameCancellationTokenSource?.Cancel();
        _frameCancellationTokenSource?.Dispose();
        _frameCancellationTokenSource = null;
        _openCancellationToken        = default;
        _closing = false;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (_openState is not null)
        {
            if (change.Property == HostWidthProperty ||
                change.Property == HostMinWidthProperty ||
                change.Property == HostMaxWidthProperty ||
                change.Property == HostHeightProperty ||
                change.Property == HostMinHeightProperty ||
                change.Property == HostMaxHeightProperty)
            {
                _openState.DialogHost.UpdateSizing();
            }
            else if (change.Property == HorizontalStartupLocationProperty ||
                     change.Property == VerticalStartupLocationProperty ||
                     change.Property == HorizontalOffsetProperty ||
                     change.Property == VerticalOffsetProperty ||
                     change.Property == OffsetXProperty ||
                     change.Property == OffsetYProperty)
            {
                _openState.DialogHost.UpdatePlacement();
            }
            else if (change.Property == ContentTemplateProperty)
            {
                _openState.DialogHost.ContentTemplate = change.GetNewValue<IDataTemplate?>();
            }
            else if (change.Property == ContentProperty)
            {
                var hostedContent = change.GetNewValue<object?>() as Control;
                _openState.DialogHost.Content = hostedContent;
            }
            else if (change.Property == IsTopmostProperty)
            {
                _openState.DialogHost.Topmost = change.GetNewValue<bool>();
            }
        }

        if (change.Property == IsModalProperty || change.Property == IsMinimizableProperty)
        {
            SetCurrentValue(EffectiveMinimizableProperty, !IsModal && IsMinimizable);
        }
        else if (change.Property == DataContextProperty)
        {
            if (change.OldValue is IDialogAwareDataContext oldDataContext)
            {
                oldDataContext.NotifyDetachedFromDialog();
            }

            if (change.NewValue is IDialogAwareDataContext newDataContext)
            {
                newDataContext.NotifyAttachedToDialog(this);
            }
        }
    }

    private static IDisposable SubscribeToEventHandler<T, TEventHandler>(
        T target,
        TEventHandler handler,
        Action<T, TEventHandler> subscribe,
        Action<T, TEventHandler> unsubscribe)
    {
        subscribe(target, handler);
        return Disposable.Create((unsubscribe, target, handler),
            state => state.unsubscribe(state.target, state.handler));
    }

    private void RootTemplateApplied(object? sender, TemplateAppliedEventArgs e)
    {
        SetTemplatedParentAndApplyChildTemplates(Content as Control);
    }

    private void SetTemplatedParentAndApplyChildTemplates(Control? control)
    {
        if (control is null)
        {
            return;
        }

        control.SetTemplatedParent(this);
        control.ApplyTemplate();
    }

    private void ParentClosed(object? sender, EventArgs e)
    {
        RequestClose(new DialogCloseRequest(null, DialogCloseReason.OwnerClosed, null, false));
    }

    private void TargetDetached(object? sender, VisualTreeAttachmentEventArgs e)
    {
        RequestClose(new DialogCloseRequest(Result, DialogCloseReason.PlacementTargetDetached, null, false));
    }

    internal void NotifyDialogHostCloseRequest()
    {
        RequestClose(new DialogCloseRequest(Result, DialogCloseReason.HostCloseRequest, null, false));
    }

    internal Point CalculatePlacementOffset(Size hostSize, Size ownerSize)
    {
        var x = CalculateHorizontalPlacementOffset(hostSize, ownerSize) + OffsetX;
        var y = CalculateVerticalPlacementOffset(hostSize, ownerSize) + OffsetY;
        return new Point(x, y);
    }

    private double CalculateHorizontalPlacementOffset(Size hostSize, Size ownerSize)
    {
        return HorizontalStartupLocation switch
        {
            DialogHorizontalAnchor.Left => 0,
            DialogHorizontalAnchor.Center => Math.Max((ownerSize.Width - hostSize.Width) / 2, 0),
            DialogHorizontalAnchor.Right => Math.Max(ownerSize.Width - hostSize.Width, 0),
            _ => HorizontalOffset?.Resolve(ownerSize.Width) ?? 0
        };
    }

    private double CalculateVerticalPlacementOffset(Size hostSize, Size ownerSize)
    {
        return VerticalStartupLocation switch
        {
            DialogVerticalAnchor.Top => 0,
            DialogVerticalAnchor.Center => Math.Max((ownerSize.Height - hostSize.Height) / 2, 0),
            DialogVerticalAnchor.Bottom => Math.Max(ownerSize.Height - hostSize.Height, 0),
            _ => VerticalOffset?.Resolve(ownerSize.Height) ?? 0
        };
    }

    private Control ResolvePlacementTarget()
    {
        return OverlayLayerResolver.ResolvePlacementTarget(this, PlacementTarget, nameof(Dialog));
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
            _owner = owner;
            _owner._ignoreIsOpenChanged = true;
        }

        public void Dispose()
        {
            _owner._ignoreIsOpenChanged = false;
        }
    }

    private sealed record DialogCloseRequest(
        object? Result,
        DialogCloseReason Reason,
        DialogButton? SourceButton,
        bool RestoreIsOpenOnCancel)
    {
        public static DialogCloseRequest Accepted(DialogButton? sourceButton)
        {
            return new DialogCloseRequest(DialogCode.Accepted, DialogCloseReason.Accepted, sourceButton, false);
        }

        public static DialogCloseRequest Rejected(DialogButton? sourceButton)
        {
            return new DialogCloseRequest(DialogCode.Rejected, DialogCloseReason.Rejected, sourceButton, false);
        }

        public static DialogCloseRequest Programmatic(object? result)
        {
            return new DialogCloseRequest(result, DialogCloseReason.Programmatic, null, false);
        }

        public static DialogCloseRequest OpenStateChanged(object? result)
        {
            return new DialogCloseRequest(result, DialogCloseReason.Programmatic, null, true);
        }
    }

    private class DialogOpenState : IDisposable
    {
        private readonly IDisposable _cleanup;
        private readonly TaskCompletionSource<object?> _closedTaskSource =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public DialogOpenState(IDialogHost dialogHost,
                               IDisposable cleanup)
        {
            DialogHost          = dialogHost;
            _cleanup            = cleanup;
        }

        public IDialogHost DialogHost { get; }
        public Task<object?> ClosedTask => _closedTaskSource.Task;

        public void SetClosed(object? result)
        {
            _closedTaskSource.TrySetResult(result);
        }

        public void Close(Action? closedCallback = null)
        {
            DialogHost.Close(() =>
            {
                _cleanup.Dispose();
                DialogHost.Content = null;
                closedCallback?.Invoke();
            });
        }

        public void Dispose()
        {
            Close();
        }
    }

    private void HandleCustomButtonsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var targetButtons = _openState?.DialogHost.CustomButtons;

        if (targetButtons is null)
        {
            return;
        }

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                DialogButtonCollectionUtils.AddRange(targetButtons, e.NewItems!);
                break;
            case NotifyCollectionChangedAction.Remove:
                DialogButtonCollectionUtils.RemoveAll(targetButtons, e.OldItems!);
                break;
            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Move:
            case NotifyCollectionChangedAction.Reset:
                throw new NotSupportedException();
        }
    }

    internal void NotifyDialogButtonBoxClicked(DialogButton button)
    {
        var buttonClickedArgs = new DialogButtonClickedEventArgs(button);
        ButtonClicked?.Invoke(this, buttonClickedArgs);
        if (buttonClickedArgs.Handled)
        {
            return;
        }

        if (button.Role == DialogButtonRole.AcceptRole ||
            button.Role == DialogButtonRole.YesRole ||
            button.Role == DialogButtonRole.ApplyRole ||
            button.Role == DialogButtonRole.ResetRole)
        {
            RequestClose(DialogCloseRequest.Accepted(button));
        }
        else if (button.Role == DialogButtonRole.RejectRole ||
                 button.Role == DialogButtonRole.NoRole)
        {
            RequestClose(DialogCloseRequest.Rejected(button));
        }
    }

    internal void NotifyDialogButtonSynchronized(IReadOnlyList<DialogButton> buttons)
    {
        _synchronizedButtons = buttons;
        ButtonsConfigure?.Invoke(buttons);
    }

    internal bool TryHandleStandardButtonKey(Key key)
    {
        var standardButton = ResolveStandardButtonForKey(key);

        if (standardButton == DialogStandardButton.NoButton)
        {
            return false;
        }

        var button = _synchronizedButtons.FirstOrDefault(x =>
            x.StandardButtonType == standardButton &&
            x.IsEffectivelyEnabled);
        if (button is null)
        {
            return false;
        }

        NotifyDialogButtonBoxClicked(button);
        return true;
    }

    private DialogStandardButton ResolveStandardButtonForKey(Key key)
    {
        return key switch
        {
            Key.Enter  => DefaultStandardButton,
            Key.Escape => ResolveEscapeStandardButton(),
            _          => DialogStandardButton.NoButton
        };
    }

    private DialogStandardButton ResolveEscapeStandardButton()
    {
        if (IsSet(EscapeStandardButtonProperty))
        {
            return EscapeStandardButton;
        }

        return StandardButtons.HasFlag(DialogStandardButton.Cancel)
            ? DialogStandardButton.Cancel
            : DialogStandardButton.NoButton;
    }
}
