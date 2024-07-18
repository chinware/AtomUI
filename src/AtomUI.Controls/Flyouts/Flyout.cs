using System.ComponentModel;
using System.Reactive.Disposables;
using AtomUI.Controls.MotionScene;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.MotionScene;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Threading;

namespace AtomUI.Controls;

using PopupControl = Popup;

public class Flyout : PopupFlyoutBase
{
   /// <summary>
   /// 是否显示指示箭头
   /// </summary>
   public static readonly StyledProperty<bool> IsShowArrowProperty =
      ArrowDecoratedBox.IsShowArrowProperty.AddOwner<PopupFlyoutBase>();

   private static readonly StyledProperty<bool> IsShowArrowEffectiveProperty =
      ArrowDecoratedBox.IsShowArrowProperty.AddOwner<PopupFlyoutBase>();

   public static readonly StyledProperty<BoxShadows> MaskShadowsProperty =
      Border.BoxShadowProperty.AddOwner<Flyout>();

   /// <summary>
   /// 箭头是否始终指向中心
   /// </summary>
   public static readonly StyledProperty<bool> IsPointAtCenterProperty =
      AvaloniaProperty.Register<PopupFlyoutBase, bool>(nameof(IsPointAtCenter), false);

   /// <summary>
   /// Defines the <see cref="Content"/> property
   /// </summary>
   public static readonly StyledProperty<object> ContentProperty =
      AvaloniaProperty.Register<Flyout, object>(nameof(Content));

   private Classes? _classes;

   /// <summary>
   /// Gets the Classes collection to apply to the FlyoutPresenter this Flyout is hosting
   /// </summary>
   public Classes FlyoutPresenterClasses => _classes ??= new Classes();

   /// <summary>
   /// Defines the <see cref="FlyoutPresenterTheme"/> property.
   /// </summary>
   public static readonly StyledProperty<ControlTheme?> FlyoutPresenterThemeProperty =
      AvaloniaProperty.Register<Flyout, ControlTheme?>(nameof(FlyoutPresenterTheme));

   public bool IsShowArrow
   {
      get => GetValue(IsShowArrowProperty);
      set => SetValue(IsShowArrowProperty, value);
   }

   /// <summary>
   /// 是否实际显示箭头
   /// </summary>
   private bool IsShowArrowEffective
   {
      get => GetValue(IsShowArrowEffectiveProperty);
      set => SetValue(IsShowArrowEffectiveProperty, value);
   }

   public bool IsPointAtCenter
   {
      get => GetValue(IsPointAtCenterProperty);
      set => SetValue(IsPointAtCenterProperty, value);
   }

   /// <summary>
   /// Gets or sets the <see cref="ControlTheme"/> that is applied to the container element generated for the flyout presenter.
   /// </summary>
   public ControlTheme? FlyoutPresenterTheme
   {
      get => GetValue(FlyoutPresenterThemeProperty);
      set => SetValue(FlyoutPresenterThemeProperty, value);
   }

   /// <summary>
   /// Gets or sets the content to display in this flyout
   /// </summary>
   [Content]
   public object Content
   {
      get => GetValue(ContentProperty);
      set => SetValue(ContentProperty, value);
   }

   public BoxShadows MaskShadows
   {
      get => GetValue(MaskShadowsProperty);
      set => SetValue(MaskShadowsProperty, value);
   }

   private TimeSpan _motionDuration;

   private static readonly DirectProperty<Flyout, TimeSpan> MotionDurationTokenProperty
      = AvaloniaProperty.RegisterDirect<Flyout, TimeSpan>(nameof(_motionDuration),
                                                          (o) => o._motionDuration,
                                                          (o, v) => o._motionDuration = v);

   private CompositeDisposable? _compositeDisposable;
   private bool _animating = false;

   // 当鼠标移走了，但是打开动画还没完成，我们需要记录下来这个信号
   internal bool RequestCloseWhereAnimationCompleted { get; set; } = false;
   private PopupPositionInfo? _popupPositionInfo; // 这个信息在隐藏动画的时候会用到

   static Flyout()
   {
      IsShowArrowProperty.OverrideDefaultValue<Flyout>(true);
   }

   public Flyout()
   {
      BindUtils.CreateGlobalTokenBinding(this, MotionDurationTokenProperty, GlobalResourceKey.MotionDurationMid);
      BindUtils.CreateGlobalTokenBinding(this, MaskShadowsProperty, GlobalResourceKey.BoxShadowsSecondary);
   }

   private void HandlePopupPropertyChanged(AvaloniaPropertyChangedEventArgs args)
   {
      SetupArrowPosition(AtomPopup);
   }

   private void SetupArrowPosition(Popup popup, FlyoutPresenter? flyoutPresenter = null)
   {
      if (flyoutPresenter is null) {
         var child = popup.Child;
         if (child is FlyoutPresenter childPresenter) {
            flyoutPresenter = childPresenter;
         }
      }

      var placement = popup.Placement;
      var anchor = popup.PlacementAnchor;
      var gravity = popup.PlacementGravity;

      if (flyoutPresenter is not null) {
         var arrowPosition = PopupUtils.CalculateArrowPosition(placement, anchor, gravity);
         if (arrowPosition.HasValue) {
            flyoutPresenter.ArrowPosition = arrowPosition.Value;
         }
      }
   }

   protected override Control CreatePresenter()
   {
      var presenter = new FlyoutPresenter
      {
         [!Border.ChildProperty] = this[!ContentProperty]
      };
      BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, presenter, IsShowArrowProperty);
      SetupArrowPosition(AtomPopup, presenter);
      return presenter;
   }

   protected internal override void NotifyPopupCreated(Popup popup)
   {
      base.NotifyPopupCreated(popup);
      BindUtils.RelayBind(this, PlacementProperty, popup);
      BindUtils.RelayBind(this, PlacementAnchorProperty, popup);
      BindUtils.RelayBind(this, PlacementGravityProperty, popup);
      BindUtils.RelayBind(this, MaskShadowsProperty, popup);
      SetupArrowPosition(popup);
   }

   protected override void OnOpening(CancelEventArgs args)
   {
      if (Popup.Child is { } presenter) {
         if (_classes != null) {
            SetPresenterClasses(presenter, FlyoutPresenterClasses);
         }

         if (FlyoutPresenterTheme is { } theme) {
            presenter.SetValue(Control.ThemeProperty, theme);
         }
      }

      base.OnOpening(args);
      if (!args.Cancel) {
         _compositeDisposable = new CompositeDisposable();
         _compositeDisposable.Add(PopupControl.IsFlippedProperty.Changed.Subscribe(HandlePopupPropertyChanged));
      }
   }

   protected override void OnClosed()
   {
      base.OnClosed();
      _compositeDisposable?.Dispose();
   }

   private Point CalculatePopupPositionDelta(Control anchorTarget,
                                             Control? flyoutPresenter,
                                             PlacementMode placement,
                                             PopupAnchor? anchor = null,
                                             PopupGravity? gravity = null)
   {
      var offsetX = 0d;
      var offsetY = 0d;
      if (IsShowArrow && IsPointAtCenter) {
         if (PopupUtils.CanEnabledArrow(placement, anchor, gravity)) {
            if (flyoutPresenter is ArrowDecoratedBox arrowDecoratedBox) {
               var arrowVertexPoint = arrowDecoratedBox.ArrowVertexPoint;

               var anchorSize = anchorTarget.Bounds.Size;
               var centerX = anchorSize.Width / 2;
               var centerY = anchorSize.Height / 2;
               // 这里计算不需要全局坐标
               if (placement == PlacementMode.TopEdgeAlignedLeft ||
                   placement == PlacementMode.BottomEdgeAlignedLeft) {
                  offsetX += centerX - arrowVertexPoint.Item1;
               } else if (placement == PlacementMode.TopEdgeAlignedRight ||
                          placement == PlacementMode.BottomEdgeAlignedRight) {
                  offsetX -= centerX - arrowVertexPoint.Item2;
               } else if (placement == PlacementMode.RightEdgeAlignedTop ||
                          placement == PlacementMode.LeftEdgeAlignedTop) {
                  offsetY += centerY - arrowVertexPoint.Item1;
               } else if (placement == PlacementMode.RightEdgeAlignedBottom ||
                          placement == PlacementMode.LeftEdgeAlignedBottom) {
                  offsetY -= centerY - arrowVertexPoint.Item2;
               }
            }
         }
      }

      return new Point(offsetX, offsetY);
   }

   // 因为在某些 placement 下箭头是不能显示的
   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      if (e.Property == IsShowArrowProperty ||
          e.Property == PlacementProperty ||
          e.Property == PlacementAnchorProperty ||
          e.Property == PlacementGravityProperty) {
         CalculateShowArrowEffective();
      }
   }

   private void CalculateShowArrowEffective()
   {
      if (IsShowArrow == false) {
         IsShowArrowEffective = false;
      } else {
         IsShowArrowEffective = PopupUtils.CanEnabledArrow(Placement, PlacementAnchor, PlacementGravity);
      }
   }

   protected internal override void NotifyPositionPopup(bool showAtPointer)
   {
      if (Popup.Child!.DesiredSize == default) {
         LayoutHelper.MeasureChild(Popup.Child, Size.Infinity, new Thickness());
      }

      Popup.PlacementAnchor = PlacementAnchor;
      Popup.PlacementGravity = PlacementGravity;

      if (showAtPointer) {
         Popup.Placement = PlacementMode.Pointer;
      } else {
         Popup.Placement = Placement;
         Popup.PlacementConstraintAdjustment = PlacementConstraintAdjustment;
      }

      var pointAtCenterOffset =
         CalculatePopupPositionDelta(Target!, Popup.Child, Popup.Placement, Popup.PlacementAnchor,
                                     Popup.PlacementGravity);

      var offsetX = HorizontalOffset;
      var offsetY = VerticalOffset;
      if (IsPointAtCenter) {
         offsetX += pointAtCenterOffset.X;
         offsetY += pointAtCenterOffset.Y;
      }

      // 更新弹出信息是否指向中点
      Popup.HorizontalOffset = offsetX;
      Popup.VerticalOffset = offsetY;
   }

   protected override bool ShowAtCore(Control placementTarget, bool showAtPointer = false)
   {
      if (_animating) {
         return false;
      }

      RequestCloseWhereAnimationCompleted = false;
      CalculateShowArrowEffective();
      var presenter = CreatePresenter();
      bool result = default;
      if (presenter is FlyoutPresenter flyoutPresenter) {
         _animating = true;
         if (flyoutPresenter.Child is not null) {
            var placementToplevel = TopLevel.GetTopLevel(placementTarget);
            UIStructureUtils.ClearLogicalParentRecursive(flyoutPresenter, null);
            UIStructureUtils.ClearVisualParentRecursive(flyoutPresenter, null);
            UIStructureUtils.SetLogicalParent(flyoutPresenter, placementToplevel);

            _popupPositionInfo = PopupControl.CalculatePositionInfo(placementTarget,
                                                                    MarginToAnchor,
                                                                    presenter,
                                                                    new Point(HorizontalOffset, VerticalOffset),
                                                                    Placement,
                                                                    Popup.PlacementAnchor,
                                                                    Popup.PlacementGravity,
                                                                    null,
                                                                    Popup.FlowDirection);
            // 重新设置箭头位置
            // 因为可能有 flip 的情况
            var arrowPosition = PopupUtils.CalculateArrowPosition(_popupPositionInfo.EffectivePlacement,
                                                                  _popupPositionInfo.EffectivePlacementAnchor,
                                                                  _popupPositionInfo.EffectivePlacementGravity);
            if (arrowPosition.HasValue) {
               flyoutPresenter.ArrowPosition = arrowPosition.Value;
            }

            // 获取是否在指向中点
            var pointAtCenterOffset = CalculatePopupPositionDelta(placementTarget,
                                                                  presenter,
                                                                  _popupPositionInfo.EffectivePlacement,
                                                                  _popupPositionInfo.EffectivePlacementAnchor,
                                                                  _popupPositionInfo.EffectivePlacementGravity);
            if (IsPointAtCenter) {
               _popupPositionInfo.Offset = new Point(
                  Math.Floor(_popupPositionInfo.Offset.X + pointAtCenterOffset.X * _popupPositionInfo.Scaling),
                  Math.Floor(_popupPositionInfo.Offset.Y + pointAtCenterOffset.Y * _popupPositionInfo.Scaling));
            }

            PlayShowMotion(_popupPositionInfo, placementTarget, flyoutPresenter, showAtPointer);
         }

         result = true;
      } else {
         result = base.ShowAtCore(placementTarget, showAtPointer);
      }

      return result;
   }

   protected override bool HideCore(bool canCancel = true)
   {
      // 在这里我们需要自己实现是否能关闭的逻辑了
      if (!IsOpen || _animating) {
         return false;
      }

      if (canCancel) {
         if (CancelClosing()) {
            return false;
         }
      }

      // 后期加上是否有动画的开关
      PlayHideMotion();
      return true;
   }

   private bool CancelClosing()
   {
      var eventArgs = new CancelEventArgs();
      OnClosing(eventArgs);
      return eventArgs.Cancel;
   }

   private void PlayShowMotion(PopupPositionInfo positionInfo, Control placementTarget, FlyoutPresenter flyoutPresenter,
                               bool showAtPointer)
   {
      var director = Director.Instance;
      var motion = new ZoomBigInMotion();
      motion.ConfigureOpacity(_motionDuration);
      motion.ConfigureRenderTransform(_motionDuration);
      var topLevel = TopLevel.GetTopLevel(placementTarget);

      var motionActor =
         new PopupMotionActor(MaskShadows, positionInfo.Offset, positionInfo.Scaling, flyoutPresenter, motion);
      motionActor.DispatchInSceneLayer = true;
      motionActor.SceneParent = topLevel;
      motionActor.Completed += (sender, args) =>
      {
         if (flyoutPresenter.Child is not null) {
            var child = flyoutPresenter.Child;
            UIStructureUtils.ClearLogicalParentRecursive(child, null);
            UIStructureUtils.ClearVisualParentRecursive(child, null);
         }

         base.ShowAtCore(placementTarget, showAtPointer);
         if (Popup.Host is WindowBase window) {
            window.PlatformImpl!.SetTopmost(true);
         }

         _animating = false;
         if (RequestCloseWhereAnimationCompleted) {
            Dispatcher.UIThread.Post(() => { Hide(); });
         }
      };

      director?.Schedule(motionActor);
   }

   private void PlayHideMotion()
   {
      var popup = AtomPopup;
      var placementToplevel = TopLevel.GetTopLevel(popup.PlacementTarget);
      if (_popupPositionInfo is null ||
          popup.Child is null ||
          placementToplevel is null) {
         // 没有动画位置信息，直接关闭
         base.HideCore(false);
         return;
      }

      _animating = true;
      var director = Director.Instance;
      var motion = new ZoomBigOutMotion();
      motion.ConfigureOpacity(_motionDuration);
      motion.ConfigureRenderTransform(_motionDuration);

      UIStructureUtils.SetVisualParent(popup.Child, null);
      UIStructureUtils.SetVisualParent(popup.Child, null);

      var motionActor = new PopupMotionActor(MaskShadows, _popupPositionInfo.Offset, _popupPositionInfo.Scaling,
                                             popup.Child, motion);
      motionActor.DispatchInSceneLayer = true;
      motionActor.SceneParent = placementToplevel;

      motionActor.SceneShowed += (sender, args) =>
      {
         if (popup.Host is WindowBase window) {
            window.Opacity = 0;
            popup.HideShadowLayer();
         }
      };

      motionActor.Completed += (sender, args) =>
      {
         base.HideCore(false);
         _animating = false;
      };

      director?.Schedule(motionActor);
   }
}