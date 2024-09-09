using AtomUI.Data;
using AtomUI.Icon;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

public class DropdownButton : Button
{
   #region 公共属性定义

   public static readonly StyledProperty<MenuFlyout?> DropdownFlyoutProperty =
      AvaloniaProperty.Register<DropdownButton, MenuFlyout?>(nameof(DropdownFlyout));

   public static readonly StyledProperty<FlyoutTriggerType> TriggerTypeProperty =
      FlyoutStateHelper.TriggerTypeProperty.AddOwner<DropdownButton>();

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
      FlyoutStateHelper.MouseEnterDelayProperty.AddOwner<DropdownButton>();

   public static readonly StyledProperty<int> MouseLeaveDelayProperty =
      FlyoutStateHelper.MouseLeaveDelayProperty.AddOwner<DropdownButton>();
   
   public static readonly StyledProperty<bool> IsShowIndicatorProperty =
      AvaloniaProperty.Register<DropdownButton, bool>(nameof(IsShowIndicator), true);

   public static readonly RoutedEvent<FlyoutMenuItemClickedEventArgs> MenuItemClickedEvent =
      RoutedEvent.Register<DropdownButton, FlyoutMenuItemClickedEventArgs>(
         nameof(MenuItemClicked),
         RoutingStrategies.Bubble);

   public MenuFlyout? DropdownFlyout
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
   
   public bool IsShowIndicator
   {
      get => GetValue(IsShowIndicatorProperty);
      set => SetValue(IsShowIndicatorProperty, value);
   }

   public event EventHandler<FlyoutMenuItemClickedEventArgs>? MenuItemClicked
   {
      add => AddHandler(MenuItemClickedEvent, value);
      remove => RemoveHandler(MenuItemClickedEvent, value);
   }

   #endregion
   
   private PathIcon? _openIndicatorIcon;
   private MenuFlyoutPresenter? _menuFlyoutPresenter;
   private FlyoutStateHelper _flyoutStateHelper;

   static DropdownButton()
   {
      PlacementProperty.OverrideDefaultValue<DropdownButton>(PlacementMode.BottomEdgeAlignedLeft);
      IsShowArrowProperty.OverrideDefaultValue<DropdownButton>(false);
   }
   
   public DropdownButton()
   {
      _flyoutStateHelper = new FlyoutStateHelper()
      {
         AnchorTarget = this
      };
   }
   
   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      BindUtils.RelayBind(this, DropdownFlyoutProperty, _flyoutStateHelper, FlyoutStateHelper.FlyoutProperty);
      BindUtils.RelayBind(this, MouseEnterDelayProperty, _flyoutStateHelper, FlyoutStateHelper.MouseEnterDelayProperty);
      BindUtils.RelayBind(this, MouseLeaveDelayProperty, _flyoutStateHelper, FlyoutStateHelper.MouseLeaveDelayProperty);
      BindUtils.RelayBind(this, TriggerTypeProperty, _flyoutStateHelper, FlyoutStateHelper.TriggerTypeProperty);
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      _openIndicatorIcon = new PathIcon()
      {
         Kind = "DownOutlined"
      };
      BindUtils.RelayBind(this, IconSizeProperty, _openIndicatorIcon, PathIcon.WidthProperty);
      BindUtils.RelayBind(this, IconSizeProperty, _openIndicatorIcon, PathIcon.HeightProperty);
      
      base.OnApplyTemplate(e);
      TokenResourceBinder.CreateGlobalTokenBinding(this, MarginToAnchorProperty, GlobalTokenResourceKey.MarginXXS);
      SetupFlyoutProperties();
      if (IsShowIndicator) {
         RightExtraContent = _openIndicatorIcon;
      }
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      if (VisualRoot is not null) {
         if (e.Property == IsShowIndicatorProperty) {
            if (IsShowIndicator) {
               RightExtraContent = _openIndicatorIcon;
            } else {
               RightExtraContent = null;
            }
         }
      }
   }

   protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnDetachedFromVisualTree(e);
      _flyoutStateHelper.NotifyDetachedFromVisualTree();
   }

   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      _flyoutStateHelper.NotifyAttachedToVisualTree();
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

         DropdownFlyout.Opened += HandleFlyoutOpened;
         DropdownFlyout.Closed += HandleFlyoutClosed;
      }
   }

   private void HandleFlyoutOpened(object? sender, EventArgs e)
   {
      if (DropdownFlyout is IPopupHostProvider popupHostProvider) {
         var host = popupHostProvider.PopupHost;
         if (host is PopupRoot popupRoot) {
            if (popupRoot.Parent is Popup popup) {
               if (popup.Child is MenuFlyoutPresenter menuFlyoutPresenter) {
                  _menuFlyoutPresenter = menuFlyoutPresenter;
                  menuFlyoutPresenter.MenuItemClicked += HandleMenuItemClicked;
               }
            }
         }
      }
   }

   private void HandleFlyoutClosed(object? sender, EventArgs e)
   {
      if (_menuFlyoutPresenter is not null) {
         _menuFlyoutPresenter.MenuItemClicked -= HandleMenuItemClicked;
         _menuFlyoutPresenter = null;
      }
   }

   private void HandleMenuItemClicked(object? sender, FlyoutMenuItemClickedEventArgs args)
   {
      var eventArgs = new FlyoutMenuItemClickedEventArgs(MenuItemClickedEvent, args.Item);
      RaiseEvent(eventArgs);
   }
   
   protected override void NotifyIconBrushCalculated(in TokenResourceKey normalFilledBrushKey,
                                                     in TokenResourceKey selectedFilledBrushKey,
                                                     in TokenResourceKey activeFilledBrushKey,
                                                     in TokenResourceKey disabledFilledBrushKey)
   {
      if (_openIndicatorIcon is not null) {
         TokenResourceBinder.CreateGlobalTokenBinding(_openIndicatorIcon, PathIcon.NormalFilledBrushProperty,
                                                      normalFilledBrushKey);
         TokenResourceBinder.CreateGlobalTokenBinding(_openIndicatorIcon, PathIcon.SelectedFilledBrushProperty,
                                                      selectedFilledBrushKey);
         TokenResourceBinder.CreateGlobalTokenBinding(_openIndicatorIcon, PathIcon.ActiveFilledBrushProperty,
                                                      activeFilledBrushKey);
         TokenResourceBinder.CreateGlobalTokenBinding(_openIndicatorIcon, PathIcon.DisabledFilledBrushProperty,
                                                      disabledFilledBrushKey);
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