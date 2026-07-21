using System.Reactive.Disposables;
using System.Reactive.Linq;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal sealed class OverlayDialogPresenter : ContentControl,
                                               IDialogPresenter
{
    internal static readonly StyledProperty<bool> IsModalProperty =
        Dialog.IsModalProperty.AddOwner<OverlayDialogPresenter>();

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        Dialog.IsMotionEnabledProperty.AddOwner<OverlayDialogPresenter>();

    internal static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        AvaloniaProperty.Register<OverlayDialogPresenter, TimeSpan>(nameof(MotionDuration));

    internal bool IsModal
    {
        get => GetValue(IsModalProperty);
        set => SetValue(IsModalProperty, value);
    }

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    internal TimeSpan MotionDuration
    {
        get => GetValue(MotionDurationProperty);
        set => SetValue(MotionDurationProperty, value);
    }

    private readonly Dialog _dialog;
    private readonly Control _placementTarget;
    private readonly DialogSurface _surface;
    private readonly MatrixTransform _surfacePositionTransform = new();
    private readonly CompositeDisposable _bindings = new();
    private DialogOverlayLayer? _dialogLayer;
    private Window? _ownerWindow;
    private MotionActor? _maskMotionActor;
    private MotionActor? _surfaceMotionActor;
    private OverlayDialogMask? _dialogMask;
    private CancellationTokenSource? _openingMotionCancellationSource;
    private Task? _showTask;
    private Task? _closeTask;
    private Task? _disposeTask;
    private Point? _dragPointerOffset;
    private Point _surfacePosition;

    internal DialogSurface Surface => _surface;

    public event EventHandler<DialogPresenterCloseRequestedEventArgs>? CloseRequested;

    public IInputElement FocusScope => _surface;

    internal OverlayDialogPresenter(Dialog dialog, Control placementTarget)
    {
        _dialog = dialog;
        _placementTarget = placementTarget;
        _surface = new DialogSurface(dialog)
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            RenderTransform = _surfacePositionTransform
        };
        _surface.Classes.Add("overlay-hosted");
        Content = _surface;

        _bindings.Add(Bind(IsModalProperty, dialog.GetObservable(Dialog.IsModalProperty)));
        _bindings.Add(Bind(IsMotionEnabledProperty, dialog.GetObservable(Dialog.IsMotionEnabledProperty)));
        _bindings.Add(dialog.GetObservable(Dialog.HostWidthProperty).Subscribe(_ => UpdateCurrentLayerBounds()));
        _bindings.Add(dialog.GetObservable(Dialog.HostHeightProperty).Subscribe(_ => UpdateCurrentLayerBounds()));
        _bindings.Add(dialog.GetObservable(Dialog.HostMinWidthProperty).Subscribe(_ => UpdateCurrentLayerBounds()));
        _bindings.Add(dialog.GetObservable(Dialog.HostMinHeightProperty).Subscribe(_ => UpdateCurrentLayerBounds()));
        _bindings.Add(dialog.GetObservable(Dialog.HostMaxWidthProperty).Subscribe(_ => UpdateCurrentLayerBounds()));
        _bindings.Add(dialog.GetObservable(Dialog.HostMaxHeightProperty).Subscribe(_ => UpdateCurrentLayerBounds()));
        _surface.CloseRequested += HandleSurfaceCloseRequested;
        _surface.HostCloseRequested += HandleHostCloseRequested;
        _surface.MaximizeRequested += HandleMaximizeRequested;
        _surface.RestoreRequested += HandleRestoreRequested;
        _surface.ResizeRequested += HandleResizeRequested;
        _surface.HeaderPointerPressed += HandleHeaderPointerPressed;
        _surface.HeaderPointerMoved += HandleHeaderPointerMoved;
        _surface.HeaderPointerReleased += HandleHeaderPointerReleased;
        _surface.SizeChanged += HandleSurfaceSizeChanged;
        AddHandler(PointerPressedEvent, HandlePresenterPointerPressed, RoutingStrategies.Tunnel, true);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ReleaseDialogMask();
        _maskMotionActor = e.NameScope.Find<MotionActor>("PART_MaskMotionActor");
        _surfaceMotionActor = e.NameScope.Find<MotionActor>("PART_SurfaceMotionActor");
        _dialogMask = e.NameScope.Find<OverlayDialogMask>("PART_DialogMask");
        if (_dialogMask is not null)
        {
            _dialogMask.PointerPressed += HandleMaskPointerPressed;
        }
    }

    public ValueTask ShowAsync(CancellationToken cancellationToken)
    {
        _showTask ??= ShowCoreAsync(cancellationToken);
        return new ValueTask(_showTask);
    }

    private async Task ShowCoreAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_dialogLayer is not null)
        {
            return;
        }

        ((ISetInheritanceParent)this).SetParent(
            ((ILogical)_dialog).IsAttachedToLogicalTree ? _dialog : _placementTarget);
        _dialogLayer = DialogOverlayLayer.GetOrCreate(_placementTarget);
        _ownerWindow = TopLevel.GetTopLevel(_placementTarget) as Window;
        _dialogLayer.Add(this);
        AttachOwnerGeometryBindings();
        UpdateLayerBounds(_dialogLayer.AvailableSize);
        ApplyTemplate();
        _surface.ApplyTemplate();
        UpdateLayerBounds(_dialogLayer.AvailableSize);
        if (_surfaceMotionActor is not null)
        {
            _surfaceMotionActor.Opacity = IsMotionEnabled ? 0 : 1;
        }

        if (_maskMotionActor is not null)
        {
            _maskMotionActor.Opacity = IsMotionEnabled && IsModal ? 0 : 1;
        }

        await Dispatcher.UIThread.InvokeAsync(
            () => UpdateLayerBounds(_dialogLayer?.AvailableSize ?? default),
            DispatcherPriority.Loaded);

        if (IsMotionEnabled && _surfaceMotionActor is not null)
        {
            _openingMotionCancellationSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var motionTasks = new List<Task>
            {
                CreateSurfaceMotion(isOpening: true)
                    .RunAsync(
                        _surfaceMotionActor,
                        cancellationToken: _openingMotionCancellationSource.Token)
            };
            if (IsModal && _maskMotionActor is not null)
            {
                motionTasks.Add(new FadeInMotion(MotionDuration)
                    .RunAsync(
                        _maskMotionActor,
                        cancellationToken: _openingMotionCancellationSource.Token));
            }

            await Task.WhenAll(motionTasks);
        }
    }

    public ValueTask CloseAsync()
    {
        _closeTask ??= CloseCoreAsync();
        return new ValueTask(_closeTask);
    }

    private async Task CloseCoreAsync()
    {
        _openingMotionCancellationSource?.Cancel();
        if (_showTask is not null)
        {
            try
            {
                await _showTask;
            }
            catch
            {
                // Show failures are reported by the session; close still owns complete teardown.
            }
        }

        if (IsMotionEnabled && _surfaceMotionActor is not null && _dialogLayer is not null)
        {
            var motionTasks = new List<Task>
            {
                CreateSurfaceMotion(isOpening: false).RunAsync(_surfaceMotionActor)
            };
            if (IsModal && _maskMotionActor is not null)
            {
                motionTasks.Add(new FadeOutMotion(MotionDuration, new CubicEaseIn())
                    .RunAsync(_maskMotionActor));
            }

            await Task.WhenAll(motionTasks);
        }

        _surface.DisconnectCompositionChildren();
        _surface.Dispose();
        RemoveFromDialogLayer();
    }

    public ValueTask DisposeAsync()
    {
        _disposeTask ??= DisposeCoreAsync();
        return new ValueTask(_disposeTask);
    }

    private async Task DisposeCoreAsync()
    {
        Exception? firstException = null;
        try
        {
            await CloseAsync();
        }
        catch (Exception ex)
        {
            firstException = ex;
        }

        _surface.CloseRequested -= HandleSurfaceCloseRequested;
        _surface.HostCloseRequested -= HandleHostCloseRequested;
        _surface.MaximizeRequested -= HandleMaximizeRequested;
        _surface.RestoreRequested -= HandleRestoreRequested;
        _surface.ResizeRequested -= HandleResizeRequested;
        _surface.HeaderPointerPressed -= HandleHeaderPointerPressed;
        _surface.HeaderPointerMoved -= HandleHeaderPointerMoved;
        _surface.HeaderPointerReleased -= HandleHeaderPointerReleased;
        _surface.SizeChanged -= HandleSurfaceSizeChanged;
        RemoveHandler(PointerPressedEvent, HandlePresenterPointerPressed);
        try
        {
            _bindings.Dispose();
        }
        catch (Exception ex)
        {
            firstException ??= ex;
        }

        _openingMotionCancellationSource?.Dispose();
        _openingMotionCancellationSource = null;
        try
        {
            _surface.DisconnectCompositionChildren();
        }
        catch (Exception ex)
        {
            firstException ??= ex;
        }

        try
        {
            _surface.Dispose();
        }
        catch (Exception ex)
        {
            firstException ??= ex;
        }

        try
        {
            RemoveFromDialogLayer();
        }
        catch (Exception ex)
        {
            firstException ??= ex;
        }

        ReleaseDialogMask();
        Content = null;
        _ownerWindow = null;
        _maskMotionActor = null;
        _surfaceMotionActor = null;
        try
        {
            ((ISetInheritanceParent)this).SetParent(null);
        }
        catch (Exception ex)
        {
            firstException ??= ex;
        }

        if (firstException is not null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(firstException).Throw();
        }
    }

    private void RemoveFromDialogLayer()
    {
        if (_dialogLayer is not { } dialogLayer)
        {
            return;
        }

        dialogLayer.Remove(this);
        _dialogLayer = null;
    }

    internal void UpdateLayerBounds(Size layerSize)
    {
        if (layerSize.Width <= 0 || layerSize.Height <= 0)
        {
            return;
        }

        var visibleFrameBounds = ResolveOwnerBounds(layerSize);
        ApplyMaskBounds(layerSize);
        if (_surface.IsDialogMaximized)
        {
            ApplyMaximizedBounds(ResolveDialogBodyOwnerBounds(visibleFrameBounds));
            return;
        }

        var ownerBounds = ResolveDialogBodyOwnerBounds(visibleFrameBounds);
        ApplyNormalSurfaceSizeConstraints(ownerBounds);
        UpdateSurfacePlacement(ownerBounds);
    }

    private void ApplyNormalSurfaceSizeConstraints(Rect ownerBounds)
    {
        var minWidth = Math.Min(_dialog.HostMinWidth, ownerBounds.Width);
        var minHeight = Math.Min(_dialog.HostMinHeight, ownerBounds.Height);
        var maxWidth = ResolveMaximum(_dialog.HostMaxWidth, minWidth, ownerBounds.Width);
        var maxHeight = ResolveMaximum(_dialog.HostMaxHeight, minHeight, ownerBounds.Height);

        _surface.MinWidth = minWidth;
        _surface.MinHeight = minHeight;
        _surface.MaxWidth = maxWidth;
        _surface.MaxHeight = maxHeight;
        _surface.Width = ResolveExplicitSize(_dialog.HostWidth, minWidth, maxWidth);
        _surface.Height = ResolveExplicitSize(_dialog.HostHeight, minHeight, maxHeight);
    }

    private void ApplyMaximizedBounds(Rect ownerBounds)
    {
        _surface.MinWidth = 0;
        _surface.MinHeight = 0;
        _surface.MaxWidth = double.PositiveInfinity;
        _surface.MaxHeight = double.PositiveInfinity;
        _surface.Width = ownerBounds.Width;
        _surface.Height = ownerBounds.Height;
        SetSurfacePosition(ownerBounds.Position);
        _surface.CornerRadius = default;
    }

    private void UpdateCurrentLayerBounds()
    {
        var layerSize = _dialogLayer?.AvailableSize ?? Bounds.Size;
        UpdateLayerBounds(layerSize);
    }

    private void AttachOwnerGeometryBindings()
    {
        if (_ownerWindow is not { } window)
        {
            return;
        }

        _bindings.Add(window.GetObservable(Window.FrameShadowThicknessProperty)
                            .CombineLatest(
                                window.GetObservable(Window.VisibleFrameBorderThicknessProperty),
                                static (frameShadowThickness, visibleFrameBorderThickness) =>
                                    (frameShadowThickness, visibleFrameBorderThickness))
                            .DistinctUntilChanged()
                            .Subscribe(_ => UpdateCurrentLayerBounds()));
    }

    private void UpdateSurfacePlacement(Rect ownerBounds)
    {
        if (_surface.IsDialogMaximized)
        {
            return;
        }

        _surface.Measure(ownerBounds.Size);
        var surfaceSize = _surface.DesiredSize;
        var offset = _dialog.CalculatePlacementOffset(surfaceSize, ownerBounds.Size);
        var position = ConstrainAndRoundSurfacePosition(
            ownerBounds,
            surfaceSize,
            new Point(ownerBounds.X + offset.X, ownerBounds.Y + offset.Y));
        SetSurfacePosition(position);
    }

    private Rect ResolveOwnerBounds(Size layerSize)
    {
        return _ownerWindow is { } window
            ? WindowVisualLayerClip.CalculateClipBounds(layerSize, window.FrameShadowThickness)
            : new Rect(default, layerSize);
    }

    private Rect ResolveDialogBodyOwnerBounds(Rect visibleFrameBounds)
    {
        return _ownerWindow is { } window
            ? visibleFrameBounds.Deflate(window.VisibleFrameBorderThickness)
            : visibleFrameBounds;
    }

    private void ApplyMaskBounds(Size layerSize)
    {
        if (_maskMotionActor is null)
        {
            return;
        }

        _maskMotionActor.HorizontalAlignment = HorizontalAlignment.Left;
        _maskMotionActor.VerticalAlignment = VerticalAlignment.Top;
        _maskMotionActor.Width = layerSize.Width;
        _maskMotionActor.Height = layerSize.Height;
        _maskMotionActor.Margin = default;
    }

    private static Point ConstrainSurfacePosition(Rect ownerBounds, Size surfaceSize, Point position)
    {
        return new Point(
            Math.Clamp(
                position.X,
                ownerBounds.X,
                Math.Max(ownerBounds.X, ownerBounds.Right - surfaceSize.Width)),
            Math.Clamp(
                position.Y,
                ownerBounds.Y,
                Math.Max(ownerBounds.Y, ownerBounds.Bottom - surfaceSize.Height)));
    }

    private Point ConstrainAndRoundSurfacePosition(Rect ownerBounds, Size surfaceSize, Point position)
    {
        var constrained = ConstrainSurfacePosition(ownerBounds, surfaceSize, position);
        if (!_surface.UseLayoutRounding)
        {
            return constrained;
        }

        var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1;
        var rounded = LayoutHelper.RoundLayoutPoint(constrained, scaling);
        return ConstrainSurfacePosition(ownerBounds, surfaceSize, rounded);
    }

    private void SetSurfacePosition(Point position)
    {
        if (_surfacePosition == position)
        {
            return;
        }

        _surfacePosition = position;
        _surfacePositionTransform.Matrix = Matrix.CreateTranslation(position.X, position.Y);
    }

    private static double ResolveMaximum(double value, double min, double available)
    {
        var maximum = double.IsNaN(value) || double.IsInfinity(value)
            ? available
            : Math.Min(value, available);
        return Math.Max(min, maximum);
    }

    private static double ResolveExplicitSize(double value, double min, double max)
    {
        return double.IsNaN(value) ? double.NaN : Math.Clamp(value, min, max);
    }

    private AbstractMotion CreateSurfaceMotion(bool isOpening)
    {
        if (!_dialog.UsesPlacementTargetAsMotionAnchor)
        {
            return isOpening
                ? new FadeInMotion(MotionDuration, new CircularEaseOut())
                : new FadeOutMotion(MotionDuration, new CubicEaseIn());
        }

        var origin = ResolveMotionOrigin();
        return isOpening
            ? new DialogZoomInMotion(origin, MotionDuration)
            : new DialogZoomOutMotion(origin, MotionDuration);
    }

    private RelativePoint ResolveMotionOrigin()
    {
        var targetCenter = new Point(
            _placementTarget.Bounds.Width / 2,
            _placementTarget.Bounds.Height / 2);
        var presenterPoint = _placementTarget.TranslatePoint(targetCenter, this) ??
                             new Point(Bounds.Width / 2, Bounds.Height / 2);
        return new RelativePoint(presenterPoint, RelativeUnit.Absolute);
    }

    private void HandleSurfaceCloseRequested(object? sender, DialogSurfaceCloseRequestedEventArgs e)
    {
        CloseRequested?.Invoke(this,
            new DialogPresenterCloseRequestedEventArgs(e.Reason, e.Result, e.SourceButton));
    }

    private void HandleSurfaceSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        var visibleFrameBounds = ResolveOwnerBounds(_dialogLayer?.AvailableSize ?? Bounds.Size);
        UpdateSurfacePlacement(ResolveDialogBodyOwnerBounds(visibleFrameBounds));
    }

    private void HandleHostCloseRequested(object? sender, EventArgs e)
    {
        CloseRequested?.Invoke(this,
            new DialogPresenterCloseRequestedEventArgs(
                DialogCloseReason.HostCloseRequest,
                _dialog.Result));
    }

    private void HandleMaximizeRequested(object? sender, EventArgs e)
    {
        _surface.IsDialogMaximized = true;
        var visibleFrameBounds = ResolveOwnerBounds(_dialogLayer?.AvailableSize ?? Bounds.Size);
        ApplyMaximizedBounds(ResolveDialogBodyOwnerBounds(visibleFrameBounds));
    }

    private void HandleRestoreRequested(object? sender, EventArgs e)
    {
        _surface.IsDialogMaximized = false;
        _surface.ClearValue(TemplatedControl.CornerRadiusProperty);
        UpdateLayerBounds(_dialogLayer?.AvailableSize ?? Bounds.Size);
    }

    private void HandleResizeRequested(object? sender, OverlayDialogResizeEventArgs e)
    {
        if (_surface.IsDialogMaximized)
        {
            return;
        }

        var width = _surface.Bounds.Width;
        var height = _surface.Bounds.Height;
        if ((e.Location & ResizeHandleLocation.East) != 0)
        {
            width = Math.Clamp(width + e.DeltaOffsetX, _surface.MinWidth, _surface.MaxWidth);
        }
        else if ((e.Location & ResizeHandleLocation.West) != 0)
        {
            var nextWidth = Math.Clamp(width - e.DeltaOffsetX, _surface.MinWidth, _surface.MaxWidth);
            _dialog.SetCurrentValue(Dialog.OffsetXProperty, _dialog.OffsetX + width - nextWidth);
            width = nextWidth;
        }

        if ((e.Location & ResizeHandleLocation.South) != 0)
        {
            height = Math.Clamp(height + e.DeltaOffsetY, _surface.MinHeight, _surface.MaxHeight);
        }
        else if ((e.Location & ResizeHandleLocation.North) != 0)
        {
            var nextHeight = Math.Clamp(height - e.DeltaOffsetY, _surface.MinHeight, _surface.MaxHeight);
            _dialog.SetCurrentValue(Dialog.OffsetYProperty, _dialog.OffsetY + height - nextHeight);
            height = nextHeight;
        }

        _surface.Width = width;
        _surface.Height = height;
        var visibleFrameBounds = ResolveOwnerBounds(_dialogLayer?.AvailableSize ?? Bounds.Size);
        var ownerBounds = ResolveDialogBodyOwnerBounds(visibleFrameBounds);
        var surfaceSize = new Size(width, height);
        var offset = _dialog.CalculatePlacementOffset(surfaceSize, ownerBounds.Size);
        var position = ConstrainAndRoundSurfacePosition(
            ownerBounds,
            surfaceSize,
            new Point(ownerBounds.X + offset.X, ownerBounds.Y + offset.Y));
        SetSurfacePosition(position);
    }

    private void HandleHeaderPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!_dialog.IsDragMovable ||
            _surface.IsDialogMaximized ||
            !e.Properties.IsLeftButtonPressed ||
            e.Source is Visual source &&
            source.FindAncestorOfType<DialogCaptionButton>(true) is not null)
        {
            return;
        }

        _dragPointerOffset = e.GetPosition(_surface);
        e.Pointer.Capture(_surface.Header);
        e.PreventGestureRecognition();
        e.Handled = true;
    }

    private void HandleHeaderPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_dragPointerOffset is null || !e.Properties.IsLeftButtonPressed)
        {
            return;
        }

        MoveSurface(e.GetPosition(this));
        e.Handled = true;
    }

    private void MoveSurface(Point pointerPosition)
    {
        var surfaceSize = _surface.Bounds.Size == default
            ? new Size(_surface.Width, _surface.Height)
            : _surface.Bounds.Size;
        var pointerOffset = _dragPointerOffset!.Value;
        var visibleFrameBounds = ResolveOwnerBounds(_dialogLayer?.AvailableSize ?? Bounds.Size);
        var position = ConstrainAndRoundSurfacePosition(
            ResolveDialogBodyOwnerBounds(visibleFrameBounds),
            surfaceSize,
            pointerPosition - new Vector(pointerOffset.X, pointerOffset.Y));

        _dialog.SetCurrentValue(Dialog.OffsetXProperty, _dialog.OffsetX + position.X - _surfacePosition.X);
        _dialog.SetCurrentValue(Dialog.OffsetYProperty, _dialog.OffsetY + position.Y - _surfacePosition.Y);
        SetSurfacePosition(position);
    }

    private void HandleHeaderPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_dragPointerOffset is null)
        {
            return;
        }

        _dragPointerOffset = null;
        e.Pointer.Capture(null);
        e.Handled = true;
    }

    private void HandleMaskPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.Properties.IsLeftButtonPressed ||
            e.Source is not Visual source ||
            _dialogMask is not { } dialogMask ||
            (!ReferenceEquals(source, dialogMask) &&
             !dialogMask.IsVisualAncestorOf(source)) ||
            _dialogLayer?.IsTopmost(this) != true)
        {
            return;
        }

        e.Handled = true;
        HandleHostCloseRequested(this, EventArgs.Empty);
    }

    private void ReleaseDialogMask()
    {
        if (_dialogMask is not null)
        {
            _dialogMask.PointerPressed -= HandleMaskPointerPressed;
            _dialogMask = null;
        }
    }

    private void HandlePresenterPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (ReferenceEquals(e.Source, _dialogMask))
        {
            return;
        }

        _dialogLayer?.Activate(this);
        _surface.Focus(NavigationMethod.Pointer);
    }

    internal bool TryInvokeStandardButton(Key key)
    {
        return _surface.TryInvokeStandardButton(key);
    }
}
