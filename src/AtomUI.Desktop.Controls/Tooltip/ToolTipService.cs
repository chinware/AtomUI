using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

public sealed class ToolTipService : IDisposable
{
    private readonly IDisposable _subscriptions;

    private Control? _tipControl;
    private long _lastTipCloseTime;
    private DispatcherTimer? _timer;
    private ulong _lastTipEventTime;
    private ulong _lastWindowEventTime;

    public ToolTipService(IInputManager inputManager)
    {
        _subscriptions = new CompositeDisposable([
            inputManager.Process.Subscribe(InputManager_OnProcess),
            ToolTip.ServiceEnabledProperty.Changed.Subscribe(ServiceEnabledChanged),
            ToolTip.TipProperty.Changed.Subscribe(TipChanged)]);
    }

    public void Dispose()
    {
        StopTimer();
        _subscriptions.Dispose();
    }

    private void InputManager_OnProcess(RawInputEventArgs e)
    {
        if (e is RawPointerEventArgs pointerEvent)
        {
            bool isTooltipEvent = false;
            if (_tipControl?.GetValue(ToolTip.ToolTipProperty) is { } currentTip
                && e.Root == currentTip.GetVisualRoot() as IInputRoot)
            {
                isTooltipEvent = true;
                _lastTipEventTime = pointerEvent.Timestamp;
            }
            else if (e.Root.GetRootElement() == _tipControl?.GetVisualRoot())
            {
                _lastWindowEventTime = pointerEvent.Timestamp;
            }

            switch (pointerEvent.Type)
            {
                case RawPointerEventType.Move:
                    Update(pointerEvent.Root, pointerEvent.GetInputHitTestResult().element as Visual);
                    break;
                case RawPointerEventType.LeaveWindow
                    when (e.Root.GetRootElement() == _tipControl?.GetVisualRoot() &&
                          _lastTipEventTime != e.Timestamp) ||
                         (isTooltipEvent && _lastWindowEventTime != e.Timestamp):
                    ClearTip();
                    _tipControl = null;
                    break;
                case RawPointerEventType.LeftButtonDown:
                case RawPointerEventType.RightButtonDown:
                case RawPointerEventType.MiddleButtonDown:
                case RawPointerEventType.XButton1Down:
                case RawPointerEventType.XButton2Down:
                    if (_tipControl is not null && ToolTip.GetIsCustomShowAndHide(_tipControl))
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

        if (root == currentToolTip?.GetVisualRoot() as IInputRoot)
        {
            return;
        }

        while (candidateToolTipHost != null)
        {
            if (candidateToolTipHost == currentToolTip)
            {
                return;
            }

            if (candidateToolTipHost is Control control)
            {
                if (!ToolTip.GetServiceEnabled(control) || ToolTip.GetIsCustomShowAndHide(control))
                {
                    return;
                }

                if (ToolTip.GetTip(control) != null &&
                    (control.IsEffectivelyEnabled || ToolTip.GetShowOnDisabled(control)))
                {
                    break;
                }
            }

            candidateToolTipHost = candidateToolTipHost?.GetVisualParent();
        }

        var newControl = candidateToolTipHost as Control;

        if (newControl == _tipControl)
        {
            return;
        }

        HandleTipControlChanged(_tipControl, newControl);
        _tipControl = newControl;
    }

    private void ServiceEnabledChanged(AvaloniaPropertyChangedEventArgs<bool> args)
    {
        if (args.Sender == _tipControl && !ToolTip.GetServiceEnabled(_tipControl))
        {
            StopTimer();
        }
    }

    private void TipChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var control = (Control)e.Sender;

        if (ToolTip.GetIsOpen(control) && e.NewValue != e.OldValue && e.NewValue is not ToolTip)
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

    private void HandleTipControlChanged(Control? oldValue, Control? newValue)
    {
        StopTimer();

        var closedPreviousTip = false;

        if (oldValue != null && ToolTip.GetIsOpen(oldValue))
        {
            Close(oldValue);
            closedPreviousTip = true;
        }

        if (newValue != null && !ToolTip.GetIsOpen(newValue))
        {
            var betweenShowDelay = ToolTip.GetBetweenShowDelay(newValue);

            int showDelay;

            if (betweenShowDelay >= 0 &&
                (closedPreviousTip ||
                 (DateTime.UtcNow.Ticks - _lastTipCloseTime) <=
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
            toolTip.Closed -= ToolTipClosed;
            toolTip.PointerExited -= ToolTipPointerExited;
        }
    }

    private void ToolTipPointerExited(object? sender, PointerEventArgs e)
    {
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

            if (ToolTip.GetIsOpen(control) && control.GetValue(ToolTip.ToolTipProperty) is { } tooltip)
            {
                tooltip.Closed += ToolTipClosed;
                tooltip.PointerExited += ToolTipPointerExited;
            }
        }
    }

    private void Close(Control control)
    {
        if (!ToolTip.GetIsCustomShowAndHide(control))
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
