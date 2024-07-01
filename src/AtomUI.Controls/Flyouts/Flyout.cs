using System.ComponentModel;
using System.Reactive.Disposables;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Layout;
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

   static Flyout()
   {
      IsShowArrowProperty.OverrideDefaultValue<Flyout>(true);
   }

   private CompositeDisposable? _compositeDisposable;
   private FlyoutPresenter? _presenter;

   private void HandlePopupPropertyChanged(AvaloniaPropertyChangedEventArgs args)
   {
      SetupArrowPosition(Popup.Placement, Popup.PlacementAnchor, Popup.PlacementGravity);
   }

   private void SetupArrowPosition(PlacementMode placement, PopupAnchor? anchor, PopupGravity? gravity)
   {
      var arrowPosition = CalculateArrowPosition(placement, anchor, gravity);
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

   private ArrowPosition? CalculateArrowPosition(PlacementMode placement, PopupAnchor? anchor, PopupGravity? gravity)
   {
      if (!CanEnabledArrow(placement, anchor, gravity)) {
         return null;
      }
      
      if (placement != PlacementMode.AnchorAndGravity) {
         var ret = PopupControl.GetAnchorAndGravity(placement);
         anchor = ret.Item1;
         gravity = ret.Item2;
      }

      ArrowPosition? arrowPosition;
      switch (anchor, gravity) {
         case (PopupAnchor.Bottom, PopupGravity.Bottom):
            arrowPosition = ArrowPosition.Top;
            break;
         case (PopupAnchor.Right, PopupGravity.Right):
            arrowPosition = ArrowPosition.Left;
            break;
         case (PopupAnchor.Left, PopupGravity.Left):
            arrowPosition = ArrowPosition.Right;
            break;
         case (PopupAnchor.Top, PopupGravity.Top):
            arrowPosition = ArrowPosition.Bottom;
            break;
         case (PopupAnchor.TopRight, PopupGravity.TopLeft):
            arrowPosition = ArrowPosition.BottomEdgeAlignedRight;
            break;
         case (PopupAnchor.TopLeft, PopupGravity.TopRight):
            arrowPosition = ArrowPosition.BottomEdgeAlignedLeft;
            break;
         case (PopupAnchor.BottomLeft, PopupGravity.BottomRight):
            arrowPosition = ArrowPosition.TopEdgeAlignedLeft;
            break;
         case (PopupAnchor.BottomRight, PopupGravity.BottomLeft):
            arrowPosition = ArrowPosition.TopEdgeAlignedRight;
            break;
         case (PopupAnchor.TopLeft, PopupGravity.BottomLeft):
            arrowPosition = ArrowPosition.RightEdgeAlignedTop;
            break;
         case (PopupAnchor.BottomLeft, PopupGravity.TopLeft):
            arrowPosition = ArrowPosition.RightEdgeAlignedBottom;
            break;
         case (PopupAnchor.TopRight, PopupGravity.BottomRight):
            arrowPosition = ArrowPosition.LeftEdgeAlignedTop;
            break;
         case (PopupAnchor.BottomRight, PopupGravity.TopRight):
            arrowPosition = ArrowPosition.LeftEdgeAlignedBottom;
            break;
         default:
            arrowPosition = null;
            break;
      }

      return arrowPosition;
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

   /// <summary>
   /// 判断是否可以启用箭头，有些组合是不能启用箭头绘制的，因为没有意义
   /// </summary>
   /// <param name="placement"></param>
   /// <param name="anchor"></param>
   /// <param name="gravity"></param>
   /// <returns></returns>
   private bool CanEnabledArrow(PlacementMode placement, PopupAnchor? anchor, PopupGravity? gravity)
   {
      if (placement == PlacementMode.Center ||
          placement == PlacementMode.Pointer) {
         return false;
      }

      return PopupControl.IsCanonicalAnchorType(placement, anchor, gravity);
   }

   private Point CalculatePopupPositionDelta(Control anchorTarget, PlacementMode placement, PopupAnchor? anchor = null,
                                             PopupGravity? gravity = null)
   {
      var offsetX = 0d;
      var offsetY = 0d;
      if (IsShowArrow && IsPointAtCenter) {
         if (CanEnabledArrow(placement, anchor, gravity)) {
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
         IsShowArrowEffective = CanEnabledArrow(Placement, PlacementAnchor, PlacementGravity);
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
      Popup.VerticalOffset = offsetX;
      Popup.HorizontalOffset = offsetY;
   }

   protected override bool ShowAtCore(Control placementTarget, bool showAtPointer = false)
   {
      CalculateShowArrowEffective();
      return base.ShowAtCore(placementTarget, showAtPointer);
   }
}