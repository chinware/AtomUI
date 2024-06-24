using System.Reactive.Disposables;
using System.Reflection;
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
   
   private static readonly PropertyInfo RootInfo;
   private static readonly PropertyInfo TimestampInfo;
   private static readonly PropertyInfo VisualRootInfo;
   private static readonly PropertyInfo EventTypeInfo;
   private static readonly PropertyInfo VisualParentInfo;
   private static readonly PropertyInfo IsAttachedToVisualTreeInfo;
   
   static ToolTipService()
   {
      RootInfo = typeof(RawPointerEventArgs).GetPropertyInfoOrThrow("Root");
      TimestampInfo = typeof(RawPointerEventArgs).GetPropertyInfoOrThrow("Timestamp");
      VisualRootInfo = typeof(Control).GetPropertyInfoOrThrow("VisualRoot");
      EventTypeInfo = typeof(RawPointerEventArgs).GetPropertyInfoOrThrow("Type");
      VisualParentInfo = typeof(Visual).GetPropertyInfoOrThrow("VisualParent");
      IsAttachedToVisualTreeInfo = typeof(Control).GetPropertyInfoOrThrow("IsAttachedToVisualTree");
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
   
   public static IInputRoot? Root(RawPointerEventArgs e) => RootInfo.GetValue(e) as IInputRoot;
   public static ulong Timestamp(RawPointerEventArgs e) => (ulong)TimestampInfo.GetValue(e)!;
   public static IRenderRoot? VisualRoot(Control? control) => control is not null ? VisualRootInfo.GetValue(control) as IRenderRoot : null;
   public static RawPointerEventType EventType(RawPointerEventArgs e) => (RawPointerEventType)EventTypeInfo.GetValue(e)!;
   public static Visual? VisualParent(Visual? visual) => VisualParentInfo.GetValue(visual) as Visual;
   public static bool IsAttachedToVisualTree(Control control) => (bool)IsAttachedToVisualTreeInfo.GetValue(control)!;
   
   private void HandleInputManagerOnProcess(RawPointerEventArgs pointerEvent)
   {
      bool isTooltipEvent = false;
      if (_tipControl?.GetValue(ToolTip.ToolTipProperty) is { } currentTip && Root(pointerEvent) == currentTip.PopupHost) {
         isTooltipEvent = true;
         _lastTipEventTime = Timestamp(pointerEvent);
      } else if (Root(pointerEvent) == VisualRoot(_tipControl)) {
         _lastWindowEventTime = Timestamp(pointerEvent);
      }

      var eventType = EventType(pointerEvent);
      switch (eventType) {
         case RawPointerEventType.Move:
            var inputHitTestResult =
               pointerEvent.GetPropertyOrThrow<(IInputElement? element, IInputElement? firstEnabledAncestor)>("InputHitTestResult");
            Update(Root(pointerEvent)!, inputHitTestResult.element as Visual);
            break;
         case RawPointerEventType.LeaveWindow
            when (Root(pointerEvent) == VisualRoot(_tipControl) && _lastTipEventTime != Timestamp(pointerEvent)) ||
                 (isTooltipEvent && _lastWindowEventTime != Timestamp(pointerEvent)):
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

      var visualRoot = VisualRoot(currentToolTip);
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

         var candidateToolTipHostVisualRoot = VisualParent(candidateToolTipHost);
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
      var isAttachedToVisualTree = IsAttachedToVisualTree(control);
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