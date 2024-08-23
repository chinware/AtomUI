using System.Reactive.Disposables;
using AtomUI.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Metadata;
using Avalonia.Threading;

namespace AtomUI.Controls;

using FlyoutControl = Flyout;

public enum FlyoutTriggerType
{
   Hover,
   Click,
   Focus
}

public class FlyoutHost : Control
{
   #region 公共属性定义

    public static readonly StyledProperty<Control?> AnchorTargetProperty =
      AvaloniaProperty.Register<FlyoutHost, Control?>(nameof(AnchorTarget));

   /// <summary>
   /// Defines the <see cref="Flyout"/> property
   /// </summary>
   public static readonly StyledProperty<PopupFlyoutBase?> FlyoutProperty =
      AvaloniaProperty.Register<FlyoutHost, PopupFlyoutBase?>(nameof(Flyout));

   /// <summary>
   /// 触发方式
   /// </summary>
   public static readonly StyledProperty<FlyoutTriggerType> TriggerProperty =
      AvaloniaProperty.Register<FlyoutHost, FlyoutTriggerType>(nameof(Trigger), FlyoutTriggerType.Click);

   /// <summary>
   /// 是否显示指示箭头
   /// </summary>
   public static readonly StyledProperty<bool> IsShowArrowProperty =
      ArrowDecoratedBox.IsShowArrowProperty.AddOwner<FlyoutHost>();

   /// <summary>
   /// 箭头是否始终指向中心
   /// </summary>
   public static readonly StyledProperty<bool> IsPointAtCenterProperty =
      FlyoutControl.IsPointAtCenterProperty.AddOwner<FlyoutHost>();
   
   public static readonly StyledProperty<PlacementMode> PlacementProperty = 
      Popup.PlacementProperty.AddOwner<FlyoutHost>();
   
   public static readonly StyledProperty<PopupAnchor> PlacementAnchorProperty =
      Popup.PlacementAnchorProperty.AddOwner<FlyoutHost>();
   
   public static readonly StyledProperty<PopupGravity> PlacementGravityProperty =
      Popup.PlacementGravityProperty.AddOwner<FlyoutHost>();

   /// <summary>
   /// 距离 anchor 的边距，根据垂直和水平进行设置
   /// 但是对某些组合无效，比如跟随鼠标的情况
   /// 还有些 anchor 和 gravity 的组合也没有用 
   /// </summary>
   public static readonly StyledProperty<double> MarginToAnchorProperty =
      Popup.MarginToAnchorProperty.AddOwner<FlyoutHost>();

   public static readonly StyledProperty<int> MouseEnterDelayProperty =
      AvaloniaProperty.Register<FlyoutHost, int>(nameof(MouseEnterDelay), 100);
   
   public static readonly StyledProperty<int> MouseLeaveDelayProperty =
      AvaloniaProperty.Register<FlyoutHost, int>(nameof(MouseLeaveDelay), 100);

   /// <summary>
   /// 装饰的目标控件
   /// </summary>
   [Content]
   public Control? AnchorTarget
   {
      get => GetValue(AnchorTargetProperty);
      set => SetValue(AnchorTargetProperty, value);
   }
   
   public PopupFlyoutBase? Flyout
   {
      get => GetValue(FlyoutProperty);
      set => SetValue(FlyoutProperty, value);
   }

   public FlyoutTriggerType Trigger
   {
      get => GetValue(TriggerProperty);
      set => SetValue(TriggerProperty, value);
   }

   public bool IsShowArrow
   {
      get => GetValue(IsShowArrowProperty);
      set => SetValue(IsShowArrowProperty, value);
   }

   public bool IsPointAtCenter
   {
      get => GetValue(IsPointAtCenterProperty);
      set => SetValue(IsPointAtCenterProperty, value);
   }

   public PlacementMode Placement
   {
      get => GetValue(PlacementProperty);
      set => SetValue(PlacementProperty, value);
   }
   
   public PopupGravity PlacementGravity
   {
      get => GetValue(PlacementGravityProperty);
      set => SetValue(PlacementGravityProperty, value);
   }

   public PopupAnchor PlacementAnchor
   {
      get => GetValue(PlacementAnchorProperty);
      set => SetValue(PlacementAnchorProperty, value);
   }

   public double MarginToAnchor
   {
      get => GetValue(MarginToAnchorProperty);
      set => SetValue(MarginToAnchorProperty, value);
   }

   public int MouseEnterDelay
   {
      get => GetValue(MouseEnterDelayProperty);
      set => SetValue(MouseEnterDelayProperty, value);
   }
   
   public int MouseLeaveDelay
   {
      get => GetValue(MouseLeaveDelayProperty);
      set => SetValue(MouseLeaveDelayProperty, value);
   }

   #endregion

   private bool _initialized = false;
   private DispatcherTimer? _mouseEnterDelayTimer;
   private DispatcherTimer? _mouseLeaveDelayTimer;
   private CompositeDisposable? _subscriptions;

   static FlyoutHost()
   {
      PlacementProperty.OverrideDefaultValue<FlyoutHost>(PlacementMode.Top);
   }

   public sealed override void ApplyTemplate()
   {
      base.ApplyTemplate();
      if (!_initialized) {
         if (AnchorTarget is not null) {
            ((ISetLogicalParent)AnchorTarget).SetParent(this);
            VisualChildren.Add(AnchorTarget);
         }
         TokenResourceBinder.CreateGlobalTokenBinding(this, MarginToAnchorProperty, GlobalTokenResourceKey.MarginXXS);
         _initialized = true;
      }
   }

   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      SetupTriggerHandler();
      SetupFlyoutProperties();
   }

   protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnDetachedFromVisualTree(e);
      StopMouseLeaveTimer();
      StopMouseEnterTimer();
      _subscriptions?.Dispose();
   }

   private void SetupFlyoutProperties()
   {
      if (Flyout is not null) {
         BindUtils.RelayBind(this, PlacementProperty, Flyout);
         BindUtils.RelayBind(this, PlacementAnchorProperty, Flyout);
         BindUtils.RelayBind(this, PlacementGravityProperty, Flyout);
         BindUtils.RelayBind(this, IsShowArrowProperty, Flyout);
         BindUtils.RelayBind(this, IsPointAtCenterProperty, Flyout);
         BindUtils.RelayBind(this, MarginToAnchorProperty, Flyout);
      }
   }

   private void SetupTriggerHandler()
   {
      if (AnchorTarget is null) {
         return;
      }

      _subscriptions = new CompositeDisposable();
      if (Trigger == FlyoutTriggerType.Hover) {
         IsPointerOverProperty.Changed.Subscribe(args =>
         {
            if (args.Sender == AnchorTarget) {
               HandleAnchorTargetHover(args);
            }
         });
      } else if (Trigger == FlyoutTriggerType.Focus) {
         _subscriptions.Add(IsFocusedProperty.Changed.Subscribe(args =>
         {
            if (args.Sender == AnchorTarget) {
               HandleAnchorTargetFocus(args);
            }
         }));
      } else if (Trigger == FlyoutTriggerType.Click) {
         var inputManager = AvaloniaLocator.Current.GetService<IInputManager>()!;
         _subscriptions.Add(inputManager.Process.Subscribe(HandleAnchorTargetClick));
      }
   }

   private void HandleAnchorTargetHover(AvaloniaPropertyChangedEventArgs<bool> e)
   {
      if (Flyout is not null) {
         if (e.GetNewValue<bool>()) {
            ShowFlyout();
         } else {
            HideFlyout();
         }
      }
   }

   private void HandleAnchorTargetFocus(AvaloniaPropertyChangedEventArgs<bool> e)
   {
      if (Flyout is not null) {
         if (e.GetNewValue<bool>()) {
            ShowFlyout();
         } else {
            HideFlyout();
         }
      }
   }

   private void HandleAnchorTargetClick(RawInputEventArgs args)
   {
      if (args is RawPointerEventArgs pointerEventArgs) {
         if (AnchorTarget is not null && pointerEventArgs.Type == RawPointerEventType.LeftButtonUp) {

            if (Flyout is null) {
               return;
            }

            if (!Flyout.IsOpen) {
               var pos = AnchorTarget.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(AnchorTarget)!);
               if (!pos.HasValue) {
                  return;
               }
               var bounds = new Rect(pos.Value, AnchorTarget.Bounds.Size);
               if (bounds.Contains(pointerEventArgs.Position)) {
                  ShowFlyout();
               }
            } else {
               if (Flyout is IPopupHostProvider popupHostProvider) {
                  if (popupHostProvider.PopupHost != pointerEventArgs.Root) {
                     HideFlyout();
                  }
               }
            }
         }
      }
   }

   public void ShowFlyout()
   {
      if (Flyout is null || AnchorTarget is null) {
         return;
      }
      StopMouseEnterTimer();
      StopMouseLeaveTimer();
      Flyout.Hide();
      if (MouseEnterDelay == 0) {
         Flyout.ShowAt(AnchorTarget);
      } else {
         StartMouseEnterTimer();
      }
   }

   public void HideFlyout()
   {
      if (Flyout is null) {
         return;
      }
      StopMouseEnterTimer();
      
      if (MouseLeaveDelay == 0) {
         Flyout.Hide();
      } else {
         StartMouseLeaveTimer();
      }
   }

   private void StartMouseEnterTimer()
   {
      _mouseEnterDelayTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(MouseEnterDelay), Tag = this };
      _mouseEnterDelayTimer.Tick += (sender, args) =>
      {
         if (_mouseEnterDelayTimer != null) {
            StopMouseEnterTimer();
            if (Flyout is null || AnchorTarget is null) {
               return;
            }
            Flyout.ShowAt(AnchorTarget);
         }
      };
      _mouseEnterDelayTimer.Start();
   }
   
   private void StopMouseEnterTimer()
   {
      _mouseEnterDelayTimer?.Stop();
      _mouseEnterDelayTimer = null;
   }

   private void StopMouseLeaveTimer()
   {
      _mouseLeaveDelayTimer?.Stop();
      _mouseLeaveDelayTimer = null;
   }
   
   private void StartMouseLeaveTimer()
   {
      _mouseLeaveDelayTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(MouseLeaveDelay), Tag = this };
      _mouseLeaveDelayTimer.Tick += (sender, args) =>
      {
         if (_mouseLeaveDelayTimer != null) {
            StopMouseLeaveTimer();
            if (Flyout is null) {
               return;
            }
            Flyout.Hide();
         }
      };
      _mouseLeaveDelayTimer.Start();
   }
}