using System.Reactive.Disposables;
using AtomUI.Data;
using AtomUI.Icon;
using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Threading;

namespace AtomUI.Controls;

using FlyoutControl = Flyout;

public class DropdownButton : Button
{
   #region 公共属性定义

   public static readonly StyledProperty<FlyoutControl?> DropdownFlyoutProperty =
      AvaloniaProperty.Register<DropdownButton, FlyoutControl?>(nameof(DropdownFlyout));

   public static readonly StyledProperty<FlyoutTriggerType> TriggerTypeProperty =
      AvaloniaProperty.Register<DropdownButton, FlyoutTriggerType>(nameof(TriggerType), FlyoutTriggerType.Click);

   public static readonly StyledProperty<bool> IsShowArrowProperty =
      ArrowDecoratedBox.IsShowArrowProperty.AddOwner<DropdownButton>();

   public static readonly StyledProperty<bool> IsPointAtCenterProperty =
      MenuFlyout.IsPointAtCenterProperty.AddOwner<DropdownButton>();

   public static readonly StyledProperty<PlacementMode> PlacementProperty =
      Popup.PlacementProperty.AddOwner<DropdownButton>();

   public static readonly StyledProperty<PopupAnchor> PlacementAnchorProperty =
      Popup.PlacementAnchorProperty.AddOwner<DropdownButton>();

   public static readonly StyledProperty<PopupGravity> PlacementGravityProperty =
      Popup.PlacementGravityProperty.AddOwner<DropdownButton>();

   public static readonly StyledProperty<double> MarginToAnchorProperty =
      Popup.MarginToAnchorProperty.AddOwner<DropdownButton>();

   public static readonly StyledProperty<int> MouseEnterDelayProperty =
      AvaloniaProperty.Register<DropdownButton, int>(nameof(MouseEnterDelay), 100);

   public static readonly StyledProperty<int> MouseLeaveDelayProperty =
      AvaloniaProperty.Register<DropdownButton, int>(nameof(MouseLeaveDelay), 100);

   public FlyoutControl? DropdownFlyout
   {
      get => GetValue(DropdownFlyoutProperty);
      set => SetValue(DropdownFlyoutProperty, value);
   }

   public FlyoutTriggerType TriggerType
   {
      get => GetValue(TriggerTypeProperty);
      set => SetValue(TriggerTypeProperty, value);
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

   private DispatcherTimer? _mouseEnterDelayTimer;
   private DispatcherTimer? _mouseLeaveDelayTimer;
   private CompositeDisposable? _subscriptions;
   private PathIcon? _openIndicatorIcon;

   static DropdownButton()
   {
      PlacementProperty.OverrideDefaultValue<DropdownButton>(PlacementMode.BottomEdgeAlignedLeft);
      IsShowArrowProperty.OverrideDefaultValue<DropdownButton>(false);
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      _openIndicatorIcon = new PathIcon()
      {
         Kind = "DownOutlined"
      };
      BindUtils.RelayBind(this, IconSizeProperty, _openIndicatorIcon, PathIcon.WidthProperty);
      BindUtils.RelayBind(this, IconSizeProperty, _openIndicatorIcon, PathIcon.HeightProperty);
      ExtraContent = _openIndicatorIcon;
      
      base.OnApplyTemplate(e);
      TokenResourceBinder.CreateGlobalTokenBinding(this, MarginToAnchorProperty, GlobalTokenResourceKey.MarginXXS);
      SetupTriggerHandler();
   }

   protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnDetachedFromVisualTree(e);
      StopMouseLeaveTimer();
      StopMouseEnterTimer();
      _subscriptions?.Dispose();
   }

   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      SetupTriggerHandler();
   }

   private void SetupFlyoutProperties()
   {
      if (DropdownFlyout is not null) {
         BindUtils.RelayBind(this, PlacementProperty, DropdownFlyout);
         BindUtils.RelayBind(this, PlacementAnchorProperty, DropdownFlyout);
         BindUtils.RelayBind(this, PlacementGravityProperty, DropdownFlyout);
         BindUtils.RelayBind(this, IsShowArrowProperty, DropdownFlyout);
         BindUtils.RelayBind(this, IsPointAtCenterProperty, DropdownFlyout);
         BindUtils.RelayBind(this, MarginToAnchorProperty, DropdownFlyout);
      }
   }
   
   private void SetupTriggerHandler()
   {
      _subscriptions = new CompositeDisposable();
      if (TriggerType == FlyoutTriggerType.Hover) {
         _subscriptions.Add(IsPointerOverProperty.Changed.Subscribe(args =>
         {
            if (args.Sender == this) {
               HandleAnchorTargetHover(args);
            }
         }));
      } else if (TriggerType == FlyoutTriggerType.Focus) {
         _subscriptions.Add(IsFocusedProperty.Changed.Subscribe(args =>
         {
            if (args.Sender == this) {
               HandleAnchorTargetFocus(args);
            }
         }));
      } else if (TriggerType == FlyoutTriggerType.Click) {
         var inputManager = AvaloniaLocator.Current.GetService<IInputManager>()!;
         _subscriptions.Add(inputManager.Process.Subscribe(HandleAnchorTargetClick));
      }
   }

   private void HandleAnchorTargetHover(AvaloniaPropertyChangedEventArgs<bool> e)
   {
      if (DropdownFlyout is not null) {
         if (e.GetNewValue<bool>()) {
            ShowFlyout();
         } else {
            HideFlyout();
         }
      }
   }

   private void HandleAnchorTargetFocus(AvaloniaPropertyChangedEventArgs<bool> e)
   {
      if (DropdownFlyout is not null) {
         if (e.GetNewValue<bool>()) {
            if (!DropdownFlyout.IsOpen) {
               ShowFlyout();
            }
         } else {
            HideFlyout();
         }
      }
   }

   private void HandleAnchorTargetClick(RawInputEventArgs args)
   {
      if (args is RawPointerEventArgs pointerEventArgs) {
         if (pointerEventArgs.Type == RawPointerEventType.LeftButtonUp) {
            if (DropdownFlyout is null) {
               return;
            }
      
            if (!DropdownFlyout.IsOpen) {
               var pos = this.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!);
               if (!pos.HasValue) {
                  return;
               }
      
               var bounds = new Rect(pos.Value, Bounds.Size);
               if (bounds.Contains(pointerEventArgs.Position)) {
                  ShowFlyout();
               }
            } else {
               if (DropdownFlyout is IPopupHostProvider popupHostProvider) {
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
      if (DropdownFlyout is null) {
         return;
      }

      StopMouseEnterTimer();
      StopMouseLeaveTimer();
      DropdownFlyout.Hide();
      if (MouseEnterDelay == 0) {
         DropdownFlyout.ShowAt(this);
      } else {
         StartMouseEnterTimer();
      }
   }

   public void HideFlyout()
   {
      if (DropdownFlyout is null) {
         return;
      }
      
      StopMouseEnterTimer();
      
      if (MouseLeaveDelay == 0) {
         DropdownFlyout.Hide();
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
            if (DropdownFlyout is null) {
               return;
            }

            DropdownFlyout.ShowAt(this);
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
            if (DropdownFlyout is null) {
               return;
            }

            DropdownFlyout.Hide();
         }
      };
      _mouseLeaveDelayTimer.Start();
   }

   protected override void NotifyIconBrushCalculated(in TokenResourceKey normalFilledBrushKey,
                                                    in TokenResourceKey selectedFilledBrushKey,
                                                    in TokenResourceKey activeFilledBrushKey,
                                                    in TokenResourceKey disabledFilledBrushKey)
   {
      if (_openIndicatorIcon is not null) {
         TokenResourceBinder.CreateGlobalTokenBinding(_openIndicatorIcon, PathIcon.NormalFilledBrushProperty, normalFilledBrushKey);
         TokenResourceBinder.CreateGlobalTokenBinding(_openIndicatorIcon, PathIcon.SelectedFilledBrushProperty, selectedFilledBrushKey);
         TokenResourceBinder.CreateGlobalTokenBinding(_openIndicatorIcon, PathIcon.ActiveFilledBrushProperty, activeFilledBrushKey);
         TokenResourceBinder.CreateGlobalTokenBinding(_openIndicatorIcon, PathIcon.DisabledFilledBrushProperty, disabledFilledBrushKey);
      }
   }

   protected override void ApplyIconModeStyleConfig()
   {
      if (_openIndicatorIcon is not null) {
         if (_styleState.HasFlag(ControlStyleState.Enabled)) {
            if (_styleState.HasFlag(ControlStyleState.Sunken)) {
               _openIndicatorIcon.IconMode = IconMode.Selected;
            } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _openIndicatorIcon.IconMode = IconMode.Active;
            } else {
               _openIndicatorIcon.IconMode = IconMode.Normal;
            }
         } else {
            _openIndicatorIcon.IconMode = IconMode.Disabled;
         }
      }
      base.ApplyIconModeStyleConfig();
   }


}