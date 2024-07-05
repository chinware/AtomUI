using System.ComponentModel;
using System.Reactive.Disposables;
using AtomUI.Controls.MotionScene;
using AtomUI.Data;
using AtomUI.MotionScene;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Styling;

namespace AtomUI.Controls;

using PopupControl = Popup;

public class Flyout : PopupFlyoutBase
{
   /// <summary>
   /// 是否显示指示箭头
   /// </summary>
   public static readonly StyledProperty<bool> IsShowArrowProperty =
      ArrowDecoratedBox.IsShowArrowProperty.AddOwner<PopupFlyoutBase>();

   public static readonly StyledProperty<bool> IsShowArrowEffectiveProperty =
      ArrowDecoratedBox.IsShowArrowProperty.AddOwner<PopupFlyoutBase>();

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
   public bool IsShowArrowEffective
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
   
   private TimeSpan _motionDuration;
   private static readonly DirectProperty<Flyout, TimeSpan> MotionDurationTokenProperty
      = AvaloniaProperty.RegisterDirect<Flyout, TimeSpan>(nameof(_motionDuration),
                                                          (o) => o._motionDuration,
                                                          (o, v) => o._motionDuration = v);

   private CompositeDisposable? _compositeDisposable;
   private FlyoutPresenter? _presenter;
   private bool _animating = false;
   private GlobalTokenBinder _globalTokenBinder;
   
   static Flyout()
   {
      IsShowArrowProperty.OverrideDefaultValue<Flyout>(true);
   }

   public Flyout()
   {
      _globalTokenBinder = new GlobalTokenBinder();
      _globalTokenBinder.AddGlobalBinding(this, MotionDurationTokenProperty, GlobalResourceKey.MotionDurationMid);
   }

   private void HandlePopupPropertyChanged(AvaloniaPropertyChangedEventArgs args)
   {
      SetupArrowPosition(Popup.Placement, Popup.PlacementAnchor, Popup.PlacementGravity);
   }

   private void SetupArrowPosition(PlacementMode placement, PopupAnchor? anchor, PopupGravity? gravity)
   {
      var arrowPosition = PopupUtils.CalculateArrowPosition(placement, anchor, gravity);
      if (_presenter is not null && arrowPosition is not null) {
         _presenter.ArrowPosition = arrowPosition.Value;
      }
   }

   protected override Control CreatePresenter()
   {
      _presenter = new FlyoutPresenter
      {
         [!BorderedStyleControl.ChildProperty] = this[!ContentProperty]
      };
      BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, _presenter, IsShowArrowProperty);
      SetupArrowPosition(Placement, PlacementAnchor, PlacementGravity);
      return _presenter;
   }
   
   protected internal override void NotifyPopupCreated(Popup popup) 
   {
      base.NotifyPopupCreated(popup);
      BindUtils.RelayBind(this, PlacementProperty, popup);
      BindUtils.RelayBind(this, PlacementAnchorProperty, popup);
      BindUtils.RelayBind(this, PlacementGravityProperty, popup);
      BindUtils.RelayBind(this, HorizontalOffsetProperty, popup);
      BindUtils.RelayBind(this, VerticalOffsetProperty, popup);
      SetupArrowPosition(popup.Placement, popup.PlacementAnchor, popup.PlacementGravity);
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

   private Point CalculatePopupPositionDelta(Control anchorTarget, PlacementMode placement, PopupAnchor? anchor = null,
                                             PopupGravity? gravity = null)
   {
      var offsetX = 0d;
      var offsetY = 0d;
      if (IsShowArrow && IsPointAtCenter) {
         if (PopupUtils.CanEnabledArrow(placement, anchor, gravity)) {
            if (Popup.Child is ArrowDecoratedBox arrowDecoratedBox) {
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
         SetupArrowPosition(Placement, PlacementAnchor, PlacementGravity);
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
      Size sz;
      // Popup.Child can't be null here, it was set in ShowAtCore.
      if (Popup.Child!.DesiredSize == default) {
         // Popup may not have been shown yet. Measure content
         sz = LayoutHelper.MeasureChild(Popup.Child, Size.Infinity, new Thickness());
      } else {
         sz = Popup.Child.DesiredSize;
      }

      Popup.PlacementAnchor = PlacementAnchor;
      Popup.PlacementGravity = PlacementGravity;

      if (showAtPointer) {
         Popup.Placement = PlacementMode.Pointer;
      } else {
         Popup.Placement = Placement;
         Popup.PlacementConstraintAdjustment = PlacementConstraintAdjustment;
      }

      var offsetX = HorizontalOffset;
      var offsetY = VerticalOffset;

      var offset = CalculatePopupPositionDelta(Target!, Placement, PlacementAnchor, PlacementGravity);
      offsetX += offset.X;
      offsetY += offset.Y;
      Popup.HorizontalOffset = offsetX;
      Popup.VerticalOffset = offsetY;
   }

   protected override bool ShowAtCore(Control placementTarget, bool showAtPointer = false)
   {
      if (_animating) {
         return false;
      }
      CalculateShowArrowEffective();
      //var presenter = CreatePresenter();
      // if (presenter is FlyoutPresenter flyoutPresenter) {
      //    // 为了获取 token 资源
      //    ((ISetLogicalParent)flyoutPresenter).SetParent(placementTarget);
      //    AtomPopup.CalculatePositionInfo(placementTarget, presenter);
      //    ((ISetLogicalParent)flyoutPresenter).SetParent(null);
      // }

      var result = base.ShowAtCore(placementTarget, showAtPointer);
     // PlayShowUpMotion(placementTarget);
      return result;
   }

   private void PlayShowUpMotion(Control placementTarget)
   {
      if (Popup.Host is PopupRoot popupRoot) {
         if (popupRoot.Content is Control content) {
            Popup.Opacity = 0;
            var director = Director.Instance;
            var motion = new ZoomBigInMotion();
            motion.ConfigureOpacity(_motionDuration);
            motion.ConfigureRenderTransform(_motionDuration);
            BoxShadows boxShadows = default;
            if (Popup is Popup shadowAwarePopup) {
               boxShadows = shadowAwarePopup.MaskShadows;
            }

            var topLevel = TopLevel.GetTopLevel(placementTarget);
            var motionActor = new PopupMotionActor(boxShadows, popupRoot, content, motion);
            motionActor.DispatchInSceneLayer = true;
            motionActor.SceneParent = topLevel;
            motionActor.Completed += (sender, args) =>
            {
               _animating = false;
               popupRoot.PlatformImpl?.SetTopmost(true);
               Popup.Opacity = 1;
            };
            
            director?.Schedule(motionActor);
            _animating = true;
         }
      }
   }
}