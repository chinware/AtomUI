using System.Reactive.Disposables;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

/// <summary>
/// Handles <see cref="ToolTip" /> interaction with controls.
/// </summary>
internal sealed class ToolTipService : IDisposable
{
    private readonly IDisposable _subscriptions;
    private long _lastTipCloseTime;
    private ulong _lastTipEventTime;
    private ulong _lastWindowEventTime;
    private DispatcherTimer? _timer;

    private Control? _tipControl;

    public ToolTipService()
    {
        var inputManager = AvaloniaLocator.Current.GetService<IInputManager>()!;
        _subscriptions = new CompositeDisposable(
            inputManager.Process.Subscribe(HandleInputManagerOnProcess),
            ToolTip.ServiceEnabledProperty.Changed.Subscribe(ServiceEnabledChanged),
            ToolTip.TipProperty.Changed.Subscribe(TipChanged));
    }

    public void Dispose()
    {
        StopTimer();
        _subscriptions.Dispose();
    }

    private void HandleInputManagerOnProcess(RawInputEventArgs args)
    {
        if (args is RawPointerEventArgs pointerEventArgs)
        {
            var isTooltipEvent = false;
            if (_tipControl?.GetValue(ToolTip.ToolTipProperty) is { } currentTip &&
                pointerEventArgs.Root == currentTip.PopupHost)
            {
                isTooltipEvent    = true;
                _lastTipEventTime = pointerEventArgs.Timestamp;
            }
            else if (pointerEventArgs.Root == _tipControl?.GetVisualRoot())
            {
                _lastWindowEventTime = pointerEventArgs.Timestamp;
            }

            var eventType = pointerEventArgs.Type;
            switch (eventType)
            {
                case RawPointerEventType.Move:
                    var inputHitTestResult =
                        pointerEventArgs
                            .GetPropertyOrThrow<(IInputElement? element, IInputElement? firstEnabledAncestor)>(
                                "InputHitTestResult");
                    Update(pointerEventArgs.Root, inputHitTestResult.element as Visual);
                    break;
                case RawPointerEventType.LeaveWindow
                    when (pointerEventArgs.Root == _tipControl?.GetVisualRoot() &&
                          _lastTipEventTime != pointerEventArgs.Timestamp) ||
                         (isTooltipEvent && _lastWindowEventTime != pointerEventArgs.Timestamp):
                    ClearTip();
                    _tipControl = null;
                    break;
                case RawPointerEventType.LeftButtonDown:
                case RawPointerEventType.RightButtonDown:
                case RawPointerEventType.MiddleButtonDown:
                case RawPointerEventType.XButton1Down:
                case RawPointerEventType.XButton2Down:
                    if (_tipControl is not null && ToolTip.GetIsCustomHide(_tipControl))
                    {
                        break;
                    }

                    ClearTip();
                    break;
            }

            void ClearTip()
            {
                StopTimer();
                _tipControl?.ClearValue(ToolTip.IsOpenProperty);
            }
        }
    }

    public void Update(IInputRoot root, Visual? candidateToolTipHost)
    {
        var currentToolTip = _tipControl?.GetValue(ToolTip.ToolTipProperty);
        if (root == currentToolTip?.GetVisualRoot())

            // Don't update while the pointer is over a tooltip
        {
            return;
        }

        while (candidateToolTipHost != null)
        {
            if (candidateToolTipHost ==
                currentToolTip) // when OverlayPopupHost is in use, the tooltip is in the same window as the host control
            {
                return;
            }

            if (candidateToolTipHost is Control control)
            {
                if (!ToolTip.GetServiceEnabled(control))
                {
                    return;
                }

                if (ToolTip.GetTip(control) != null &&
                    (control.IsEffectivelyEnabled || ToolTip.GetShowOnDisabled(control)))
                {
                    break;
                }
            }

            candidateToolTipHost = candidateToolTipHost.GetVisualParent();
        }

        var newControl = candidateToolTipHost as Control;

        if (newControl == _tipControl)
        {
            return;
        }

        OnTipControlChanged(_tipControl, newControl);
        _tipControl = newControl;
    }

    private void ServiceEnabledChanged(AvaloniaPropertyChangedEventArgs<bool> args)
    {
        if (args.Sender == _tipControl && !ToolTip.GetServiceEnabled(_tipControl))
        {
            StopTimer();
        }
    }

    /// <summary>
    /// called when the <see cref="ToolTip.TipProperty" /> property changes on a control.
    /// </summary>
    /// <param name="e">The event args.</param>
    private void TipChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var control = (Control)e.Sender;

        if (ToolTip.GetIsOpen(control) && e.NewValue != e.OldValue && !(e.NewValue is ToolTip))
        {
            if (e.NewValue is null)
            {
                Close(control);
            }
            else
            {
                if (control.GetValue(ToolTip.ToolTipProperty) is { } tip)
                {
                    tip.Content = e.NewValue;
                }
            }
        }
    }

    private void OnTipControlChanged(Control? oldValue, Control? newValue)
    {
        StopTimer();

        var closedPreviousTip =
            false; // avoid race conditions by remembering whether we closed a tooltip in the current call.

        if (oldValue != null && ToolTip.GetIsOpen(oldValue))
        {
            Close(oldValue);
            closedPreviousTip = true;
        }

        if (newValue != null && !ToolTip.GetIsOpen(newValue))
        {
            var betweenShowDelay = ToolTip.GetBetweenShowDelay(newValue);

            int showDelay;

            if (betweenShowDelay >= 0 && (closedPreviousTip || DateTime.UtcNow.Ticks - _lastTipCloseTime <=
                    betweenShowDelay * TimeSpan.TicksPerMillisecond))
            {
                showDelay = 0;
            }
            else
            {
                showDelay = ToolTip.GetShowDelay(newValue);
            }

            if (showDelay == 0)
            {
                Open(newValue);
            }
            else
            {
                StartShowTimer(showDelay, newValue);
            }
        }
    }

    private void ToolTipClosed(object? sender, EventArgs e)
    {
        _lastTipCloseTime = DateTime.UtcNow.Ticks;
        if (sender is ToolTip toolTip)
        {
            toolTip.Closed        -= ToolTipClosed;
            toolTip.PointerExited -= ToolTipPointerExited;
        }
    }

    private void ToolTipPointerExited(object? sender, PointerEventArgs e)
    {
        // The pointer has exited the tooltip. Close the tooltip unless the current tooltip source is still the
        // adorned control.
        if (sender is ToolTip { AdornedControl: { } control } && control != _tipControl)
        {
            Close(control);
        }
    }

    private void StartShowTimer(int showDelay, Control control)
    {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(showDelay), Tag = (this, control) };
        _timer.Tick += (o, e) =>
        {
            if (_timer != null)
            {
                Open(control);
            }
        };
        _timer.Start();
    }

    private void Open(Control control)
    {
        StopTimer();
        if (control.IsAttachedToVisualTree())
        {
            ToolTip.SetIsOpen(control, true);

            if (control.GetValue(ToolTip.ToolTipProperty) is { } tooltip)
            {
                tooltip.Closed        += ToolTipClosed;
                tooltip.PointerExited += ToolTipPointerExited;
            }
        }
    }

    private void Close(Control control)
    {
        if (!ToolTip.GetIsCustomHide(control))
        {
            ToolTip.SetIsOpen(control, false);
        }
    }

    private void StopTimer()
    {
        _timer?.Stop();
        _timer = null;
    }
}