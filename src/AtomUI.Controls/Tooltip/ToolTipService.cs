using System.Reactive.Disposables;
using AtomUI.Input;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Rendering;
using Avalonia.Threading;

namespace AtomUI.Controls;

/// <summary>
/// Handles <see cref="ToolTip"/> interaction with controls.
/// </summary>
internal sealed class ToolTipService : IDisposable
{
   private readonly IDisposable _subscriptions;

   private Control? _tipControl;
   private long _lastTipCloseTime;
   private DispatcherTimer? _timer;
   private ulong _lastTipEventTime;
   private ulong _lastWindowEventTime;
   
   static ToolTipService()
   {
   }

   public ToolTipService()
   {
      _subscriptions = new CompositeDisposable(
         InputManagerEx.SubscribeRawPointerEvent(null, HandleInputManagerOnProcess),
         ToolTip.ServiceEnabledProperty.Changed.Subscribe(ServiceEnabledChanged),
         ToolTip.TipProperty.Changed.Subscribe(TipChanged));
   }

   public void Dispose()
   {
      StopTimer();
      _subscriptions.Dispose();
   }

   private void HandleInputManagerOnProcess(RawPointerEventArgs pointerEvent)
   {
      var root = pointerEvent.GetPropertyOrThrow<IInputRoot>("Root");
      var timestamp = pointerEvent.GetPropertyOrThrow<ulong>("Timestamp");
      var visualRoot = _tipControl?.GetPropertyOrThrow<IRenderRoot?>("VisualRoot");
      bool isTooltipEvent = false;
      if (_tipControl?.GetValue(ToolTip.ToolTipProperty) is { } currentTip && root == currentTip.PopupHost) {
         isTooltipEvent = true;
         _lastTipEventTime = timestamp;
      } else if (root == _tipControl?.GetPropertyOrThrow<IRenderRoot?>("VisualRoot")) {
         _lastWindowEventTime = timestamp;
      }

      var eventType = pointerEvent.GetPropertyOrThrow<RawPointerEventType>("Type");
      switch (eventType) {
         case RawPointerEventType.Move:
            var inputHitTestResult =
               pointerEvent.GetPropertyOrThrow<(IInputElement? element, IInputElement? firstEnabledAncestor)>("InputHitTestResult");
            Update(root!, inputHitTestResult.element as Visual);
            break;
         case RawPointerEventType.LeaveWindow
            when (root == visualRoot && _lastTipEventTime != timestamp) ||
                 (isTooltipEvent && _lastWindowEventTime != timestamp):
            ClearTip();
            _tipControl = null;
            break;
         case RawPointerEventType.LeftButtonDown:
         case RawPointerEventType.RightButtonDown:
         case RawPointerEventType.MiddleButtonDown:
         case RawPointerEventType.XButton1Down:
         case RawPointerEventType.XButton2Down:
            ClearTip();
            break;
      }

      void ClearTip()
      {
         StopTimer();
         _tipControl?.ClearValue(ToolTip.IsOpenProperty);
      }
   }

   public void Update(IInputRoot root, Visual? candidateToolTipHost)
   {
      var currentToolTip = _tipControl?.GetValue(ToolTip.ToolTipProperty);

      var visualRoot = currentToolTip?.GetPropertyOrThrow<IRenderRoot?>("VisualRoot");
      if (root == visualRoot) {
         // Don't update while the pointer is over a tooltip
         return;
      }

      while (candidateToolTipHost != null) {
         if (candidateToolTipHost ==
             currentToolTip) // when OverlayPopupHost is in use, the tooltip is in the same window as the host control
            return;

         if (candidateToolTipHost is Control control) {
            if (!ToolTip.GetServiceEnabled(control)) {
               return;
            }

            if (ToolTip.GetTip(control) != null &&
                (control.IsEffectivelyEnabled || ToolTip.GetShowOnDisabled(control))) {
               break;
            }
         }
         var candidateToolTipHostVisualRoot = candidateToolTipHost?.GetPropertyOrThrow<Visual?>("VisualParent");
         candidateToolTipHost = candidateToolTipHostVisualRoot;
      }

      var newControl = candidateToolTipHost as Control;

      if (newControl == _tipControl) {
         return;
      }

      OnTipControlChanged(_tipControl, newControl);
      _tipControl = newControl;
   }

   private void ServiceEnabledChanged(AvaloniaPropertyChangedEventArgs<bool> args)
   {
      if (args.Sender == _tipControl && !ToolTip.GetServiceEnabled(_tipControl)) {
         StopTimer();
      }
   }

   /// <summary>
   /// called when the <see cref="ToolTip.TipProperty"/> property changes on a control.
   /// </summary>
   /// <param name="e">The event args.</param>
   private void TipChanged(AvaloniaPropertyChangedEventArgs e)
   {
      var control = (Control)e.Sender;

      if (ToolTip.GetIsOpen(control) && e.NewValue != e.OldValue && !(e.NewValue is ToolTip)) {
         if (e.NewValue is null) {
            Close(control);
         } else {
            if (control.GetValue(ToolTip.ToolTipProperty) is { } tip) {
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

      if (oldValue != null && ToolTip.GetIsOpen(oldValue)) {
         Close(oldValue);
         closedPreviousTip = true;
      }

      if (newValue != null && !ToolTip.GetIsOpen(newValue)) {
         var betweenShowDelay = ToolTip.GetBetweenShowDelay(newValue);

         int showDelay;

         if (betweenShowDelay >= 0 && (closedPreviousTip || (DateTime.UtcNow.Ticks - _lastTipCloseTime) <=
                betweenShowDelay * TimeSpan.TicksPerMillisecond)) {
            showDelay = 0;
         } else {
            showDelay = ToolTip.GetShowDelay(newValue);
         }

         if (showDelay == 0) {
            Open(newValue);
         } else {
            StartShowTimer(showDelay, newValue);
         }
      }
   }

   private void ToolTipClosed(object? sender, EventArgs e)
   {
      _lastTipCloseTime = DateTime.UtcNow.Ticks;
      if (sender is ToolTip toolTip) {
         toolTip.Closed -= ToolTipClosed;
         toolTip.PointerExited -= ToolTipPointerExited;
      }
   }

   private void ToolTipPointerExited(object? sender, PointerEventArgs e)
   {
      // The pointer has exited the tooltip. Close the tooltip unless the current tooltip source is still the
      // adorned control.
      if (sender is ToolTip { AdornedControl: { } control } && control != _tipControl) {
         Close(control);
      }
   }

   private void StartShowTimer(int showDelay, Control control)
   {
      _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(showDelay), Tag = (this, control) };
      _timer.Tick += (o, e) =>
      {
         if (_timer != null) Open(control);
      };
      _timer.Start();
   }

   private void Open(Control control)
   {
      StopTimer();
      var isAttachedToVisualTree = control.GetPropertyOrThrow<bool>("IsAttachedToVisualTree");
      if (isAttachedToVisualTree) {
         ToolTip.SetIsOpen(control, true);

         if (control.GetValue(ToolTip.ToolTipProperty) is { } tooltip) {
            tooltip.Closed += ToolTipClosed;
            tooltip.PointerExited += ToolTipPointerExited;
         }
      }
   }

   private void Close(Control control)
   {
      ToolTip.SetIsOpen(control, false);
   }

   private void StopTimer()
   {
      _timer?.Stop();
      _timer = null;
   }
}