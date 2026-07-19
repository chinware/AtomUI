using System.Reactive.Disposables;
using System.Reactive.Linq;
using AtomUI.Controls;
using AtomUI.Native;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

internal sealed class WindowDialogPresenter : IDialogPresenter
{
    private readonly Dialog _dialog;
    private readonly TopLevel _owner;
    private readonly DialogSurface _surface;
    private readonly CompositeDisposable _bindings = new();
    private readonly TaskCompletionSource _openedSource = new();
    private readonly TaskCompletionSource _closedSource =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private CancellationTokenSource? _openingCancellationSource;
    private Task? _modalTask;
    private Task? _showTask;
    private Task? _closeTask;
    private Task? _disposeTask;

    internal DialogWindow HostWindow { get; }

    public event EventHandler<DialogPresenterCloseRequestedEventArgs>? CloseRequested;

    public IInputElement FocusScope => _surface;

    internal WindowDialogPresenter(Dialog dialog, TopLevel owner)
    {
        _dialog = dialog;
        _owner = owner;
        _surface = new DialogSurface(dialog)
        {
            IsHeaderVisible = false,
            IsResizable = false,
            TitleIcon = null
        };
        HostWindow = new DialogWindow
        {
            Content = _surface,
            WindowStartupLocation = WindowStartupLocation.Manual
        };

        ((ISetLogicalParent)HostWindow).SetParent(_dialog);
        BindDialogProperties();
        HostWindow.CloseRequested += HandleWindowCloseRequested;
        HostWindow.Opened += HandleWindowOpened;
        HostWindow.Closed += HandleWindowClosed;
        HostWindow.KeyDown += HandleWindowKeyDown;
        _surface.CloseRequested += HandleSurfaceCloseRequested;
    }

    private void BindDialogProperties()
    {
        _bindings.Add(HostWindow.Bind(Window.TitleProperty, _dialog.GetObservable(Dialog.TitleProperty)));
        _bindings.Add(HostWindow.Bind(Window.LogoProperty, _dialog.GetObservable(Dialog.TitleIconProperty)));
        _bindings.Add(HostWindow.Bind(Window.CanResizeProperty, _dialog.GetObservable(Dialog.IsResizableProperty)));
        _bindings.Add(HostWindow.Bind(Window.CanMinimizeProperty,
            _dialog.GetObservable(Dialog.EffectiveMinimizableProperty)));
        _bindings.Add(HostWindow.Bind(Window.CanMaximizeProperty,
            _dialog.GetObservable(Dialog.IsMaximizableProperty)));
        _bindings.Add(HostWindow.Bind(Window.IsCloseCaptionButtonVisibleProperty,
            _dialog.GetObservable(Dialog.IsClosableProperty)));
        _bindings.Add(HostWindow.Bind(Window.IsMoveEnabledProperty,
            _dialog.GetObservable(Dialog.IsDragMovableProperty)));
        _bindings.Add(HostWindow.Bind(Window.TopmostProperty,
            _dialog.GetObservable(Dialog.IsTopmostProperty)));
        _bindings.Add(Observable.Merge(
                _dialog.GetObservable(Dialog.HostWidthProperty),
                _dialog.GetObservable(Dialog.HostHeightProperty),
                _dialog.GetObservable(Dialog.HostMinWidthProperty),
                _dialog.GetObservable(Dialog.HostMinHeightProperty),
                _dialog.GetObservable(Dialog.HostMaxWidthProperty),
                _dialog.GetObservable(Dialog.HostMaxHeightProperty))
            .Subscribe(_ => UpdateSizing()));
    }

    public ValueTask ShowAsync(CancellationToken cancellationToken)
    {
        _showTask ??= ShowCoreAsync(cancellationToken);
        return new ValueTask(_showTask);
    }

    private async Task ShowCoreAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _openingCancellationSource =
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var openingCancellationToken = _openingCancellationSource.Token;
        // Native Window content is not attached yet, so inherit Dialog resources for natural measurement.
        ((ISetInheritanceParent)_surface).SetParent(_dialog);
        UpdateSizing();
        if (_dialog.IsModal &&
            _owner is Window ownerWindow &&
            RuntimePlatform.Features.SupportsWindowModalDialog)
        {
            _modalTask = HostWindow.ShowDialog(ownerWindow);
        }
        else
        {
            HostWindow.Show();
        }

        // Show attaches the Window ContentPresenter and replaces the inheritance parent before first render.
        ((ISetInheritanceParent)_surface).SetParent(_dialog);
        await _openedSource.Task.WaitAsync(openingCancellationToken);
        ApplyStartupPlacement();
        openingCancellationToken.ThrowIfCancellationRequested();
    }

    private void ApplyStartupPlacement()
    {
        var ownerBounds = GetOwnerBounds(_owner);
        var offset = _dialog.CalculatePlacementOffset(HostWindow.ClientSize, ownerBounds.Size);
        var point = ownerBounds.TopLeft + offset;
        HostWindow.Position = new PixelPoint(
            (int)Math.Round(point.X * HostWindow.DesktopScaling),
            (int)Math.Round(point.Y * HostWindow.DesktopScaling));
    }

    private static Rect GetOwnerBounds(TopLevel owner)
    {
        if (owner is Avalonia.Controls.Window ownerWindow)
        {
            return new Rect(
                ownerWindow.Position.X / ownerWindow.DesktopScaling,
                ownerWindow.Position.Y / ownerWindow.DesktopScaling,
                ownerWindow.ClientSize.Width,
                ownerWindow.ClientSize.Height);
        }

        return new Rect(default, owner.ClientSize);
    }

    private void UpdateSizing()
    {
        HostWindow.MinWidth = _dialog.HostMinWidth;
        HostWindow.MinHeight = _dialog.HostMinHeight;
        HostWindow.MaxWidth = _dialog.HostMaxWidth;
        HostWindow.MaxHeight = _dialog.HostMaxHeight;
        var measuredSize = MeasureWindowSize();
        HostWindow.SizeToContent = SizeToContent.Manual;
        HostWindow.ApplyRequestedSize(
            double.IsNaN(_dialog.HostWidth) ? measuredSize.Width : _dialog.HostWidth,
            double.IsNaN(_dialog.HostHeight) ? measuredSize.Height : _dialog.HostHeight);
    }

    private Size MeasureWindowSize()
    {
        HostWindow.ApplyStyling();
        HostWindow.ApplyTemplate();
        _surface.ApplyStyling();
        _surface.ApplyTemplate();

        var padding = HostWindow.Padding;
        var shadow = HostWindow.FrameShadowThickness;
        var titleBarHeight = HostWindow.IsTitleBarVisible ? HostWindow.TitleBarHeight : 0;
        var horizontalChrome = padding.Left + padding.Right + shadow.Left + shadow.Right;
        var verticalChrome = padding.Top + padding.Bottom + shadow.Top + shadow.Bottom + titleBarHeight;
        var ownerSize = GetOwnerBounds(_owner).Size;
        var availableWindowSize = new Size(
            ResolveAvailableMeasureSize(ownerSize.Width, _dialog.HostMaxWidth),
            ResolveAvailableMeasureSize(ownerSize.Height, _dialog.HostMaxHeight));
        var availableSurfaceSize = new Size(
            Math.Max(0, availableWindowSize.Width - horizontalChrome),
            Math.Max(0, availableWindowSize.Height - verticalChrome));

        _surface.Measure(availableSurfaceSize);
        var naturalSurfaceSize = _surface.DesiredSize;
        var windowWidth = double.IsNaN(_dialog.HostWidth)
            ? ResolveMeasuredSize(
                naturalSurfaceSize.Width + horizontalChrome,
                _dialog.HostMinWidth,
                _dialog.HostMaxWidth)
            : _dialog.HostWidth;

        var constrainedSurfaceWidth = Math.Max(0, windowWidth - horizontalChrome);
        _surface.Measure(new Size(constrainedSurfaceWidth, availableSurfaceSize.Height));
        var windowHeight = double.IsNaN(_dialog.HostHeight)
            ? ResolveMeasuredSize(
                _surface.DesiredSize.Height + verticalChrome,
                _dialog.HostMinHeight,
                _dialog.HostMaxHeight)
            : _dialog.HostHeight;
        return new Size(windowWidth, windowHeight);
    }

    private static double ResolveAvailableMeasureSize(double ownerSize, double maxSize)
    {
        if (!double.IsNaN(maxSize) && !double.IsInfinity(maxSize))
        {
            return maxSize;
        }

        return ownerSize > 0 ? ownerSize : double.PositiveInfinity;
    }

    private static double ResolveMeasuredSize(double measuredSize, double minSize, double maxSize)
    {
        var size = double.IsFinite(measuredSize) ? measuredSize : 0;
        size = Math.Max(size, minSize);
        if (!double.IsNaN(maxSize) && !double.IsInfinity(maxSize))
        {
            size = Math.Min(size, maxSize);
        }

        return size;
    }

    public ValueTask CloseAsync()
    {
        _closeTask ??= CloseCoreAsync();
        return new ValueTask(_closeTask);
    }

    private async Task CloseCoreAsync()
    {
        _openingCancellationSource?.Cancel();
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

        Exception? firstException = null;
        await InvokeOnUIThreadAsync(() =>
        {
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
        });

        var wasVisible = false;
        var nativeCloseFailed = false;
        await InvokeOnUIThreadAsync(() =>
        {
            try
            {
                wasVisible = HostWindow.IsVisible;
                if (wasVisible)
                {
                    HostWindow.CloseFromPresenter();
                }
            }
            catch (Exception ex)
            {
                nativeCloseFailed = true;
                firstException ??= ex;
            }
        });
        if (wasVisible && !nativeCloseFailed)
        {
            await _closedSource.Task;
        }
        else if (!wasVisible)
        {
            _closedSource.TrySetResult();
        }

        if (_modalTask is not null)
        {
            try
            {
                await _modalTask;
            }
            catch (Exception ex)
            {
                firstException ??= ex;
            }
        }

        if (firstException is not null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(firstException).Throw();
        }
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

        await InvokeOnUIThreadAsync(() =>
        {
            HostWindow.CloseRequested -= HandleWindowCloseRequested;
            HostWindow.Opened -= HandleWindowOpened;
            HostWindow.Closed -= HandleWindowClosed;
            HostWindow.KeyDown -= HandleWindowKeyDown;
            _surface.CloseRequested -= HandleSurfaceCloseRequested;
            try
            {
                _bindings.Dispose();
            }
            catch (Exception ex)
            {
                firstException ??= ex;
            }

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

            _openingCancellationSource?.Dispose();
            _openingCancellationSource = null;
            HostWindow.Content = null;
            ((ISetInheritanceParent)_surface).SetParent(null);
            try
            {
                ((ISetLogicalParent)HostWindow).SetParent(null);
            }
            catch (Exception ex)
            {
                firstException ??= ex;
            }
        });

        if (firstException is not null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(firstException).Throw();
        }
    }

    private void HandleWindowCloseRequested(object? sender, EventArgs e)
    {
        CloseRequested?.Invoke(this,
            new DialogPresenterCloseRequestedEventArgs(DialogCloseReason.HostCloseRequest));
    }

    private void HandleWindowOpened(object? sender, EventArgs e)
    {
        _openedSource.TrySetResult();
    }

    private void HandleWindowClosed(object? sender, EventArgs e)
    {
        _closedSource.TrySetResult();
    }

    private void HandleSurfaceCloseRequested(object? sender, DialogSurfaceCloseRequestedEventArgs e)
    {
        CloseRequested?.Invoke(this,
            new DialogPresenterCloseRequestedEventArgs(e.Reason, e.Result, e.SourceButton));
    }

    private void HandleWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (!e.Handled && _surface.TryInvokeStandardButton(e.Key))
        {
            e.Handled = true;
        }
    }

    private static async Task InvokeOnUIThreadAsync(Action action)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            action();
            return;
        }

        await Dispatcher.UIThread.InvokeAsync(action);
    }
}
