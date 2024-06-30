using System.Reactive.Disposables;
using System.Reflection;
using AtomUI.Data;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls;

public class Popup : AbstractPopup
{
   public static readonly StyledProperty<BoxShadows> MaskShadowsProperty =
      Border.BoxShadowProperty.AddOwner<Popup>();

   public static readonly StyledProperty<double> MarginToAnchorProperty =
      AvaloniaProperty.Register<Popup, double>(nameof(MarginToAnchor));
   
   public BoxShadows MaskShadows
   {
      get => GetValue(MaskShadowsProperty);
      set => SetValue(MaskShadowsProperty, value);
   }

   public double MarginToAnchor
   {
      get => GetValue(MarginToAnchorProperty);
      set => SetValue(MarginToAnchorProperty, value);
   }
   
   private static readonly MethodInfo ConfigurePositionMethodInfo;
   private PopupShadowLayer? _shadowLayer;
   private GlobalTokenBinder _globalTokenBinder;
   private CompositeDisposable? _compositeDisposable;
   private bool _initialized;

   static Popup()
   {
      var type = typeof(IPopupPositioner).Assembly.GetType("Avalonia.Controls.Primitives.PopupPositioning.PopupPositionerExtensions")!;
      ConfigurePositionMethodInfo = type.GetMethod("ConfigurePosition", BindingFlags.Public | BindingFlags.Static)!;
   }

   public Popup()
   {
      IsLightDismissEnabled = false;
      _globalTokenBinder = new GlobalTokenBinder();
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _globalTokenBinder.AddGlobalBinding(this, MaskShadowsProperty, GlobalResourceKey.BoxShadowsSecondary);
         _initialized = true;
      }
   }

   protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnDetachedFromLogicalTree(e);
      _compositeDisposable?.Dispose();
   }

   protected override void NotifyPopupHostCreated(IPopupHost popupHost)
   {
      base.NotifyPopupHostCreated(popupHost);

      if (PlacementTarget is not null) {
         var toplevel = TopLevel.GetTopLevel(PlacementTarget);
         if (toplevel is null) {
            throw new InvalidOperationException(
               "Unable to create shadow layer, top level for PlacementTarget is null.");
         }

         _compositeDisposable = new CompositeDisposable();
         _shadowLayer = new PopupShadowLayer(toplevel);
         _compositeDisposable?.Add(BindUtils.RelayBind(this, MaskShadowsProperty, _shadowLayer!));
         _shadowLayer.AttachToTarget(this);
      }
   }

   protected override void NotifyClosed()
   {
      base.NotifyClosed();
      _compositeDisposable?.Dispose();
      _shadowLayer = null;
   }

   internal (bool, bool) CalculateFlipInfo(Size translatedSize, Rect anchorRect, PopupAnchor anchor,
                                           PopupGravity gravity,
                                           Point offset)
   {
      var result = (false, false);
      var bounds = GetBounds(anchorRect);
      offset *= _managedPopupPositioner!.Scaling;

      bool FitsInBounds(Rect rc, PopupAnchor edge = PopupAnchor.AllMask)
      {
         if (edge.HasFlag(PopupAnchor.Left) && rc.X < bounds.X ||
             edge.HasFlag(PopupAnchor.Top) && rc.Y < bounds.Y ||
             edge.HasFlag(PopupAnchor.Right) && rc.Right > bounds.Right ||
             edge.HasFlag(PopupAnchor.Bottom) && rc.Bottom > bounds.Bottom) {
            return false;
         }

         return true;
      }

      Rect GetUnconstrained(PopupAnchor a, PopupGravity g) =>
         new Rect(Gravitate(GetAnchorPoint(anchorRect, a), translatedSize, g) + offset, translatedSize);

      var geo = GetUnconstrained(anchor, gravity);
      // If flipping geometry and anchor is allowed and helps, use the flipped one,
      // otherwise leave it as is
      if (!FitsInBounds(geo, PopupAnchor.HorizontalMask)) {
         result.Item1 = true;
      }

      if (!FitsInBounds(geo, PopupAnchor.VerticalMask)) {
         result.Item2 = true;
      }

      return result;
   }

   private static Point Gravitate(Point anchorPoint, Size size, PopupGravity gravity)
   {
      double x, y;
      if (gravity.HasFlag(PopupGravity.Left)) {
         x = -size.Width;
      } else if (gravity.HasFlag(PopupGravity.Right)) {
         x = 0;
      } else {
         x = -size.Width / 2;
      }

      if (gravity.HasFlag(PopupGravity.Top)) {
         y = -size.Height;
      } else if (gravity.HasFlag(PopupGravity.Bottom)) {
         y = 0;
      } else {
         y = -size.Height / 2;
      }

      return anchorPoint + new Point(x, y);
   }

   private static Point GetAnchorPoint(Rect anchorRect, PopupAnchor edge)
   {
      double x, y;
      if (edge.HasFlag(PopupAnchor.Left)) {
         x = anchorRect.X;
      } else if (edge.HasFlag(PopupAnchor.Right)) {
         x = anchorRect.Right;
      } else {
         x = anchorRect.X + anchorRect.Width / 2;
      }

      if (edge.HasFlag(PopupAnchor.Top)) {
         y = anchorRect.Y;
      } else if (edge.HasFlag(PopupAnchor.Bottom)) {
         y = anchorRect.Bottom;
      } else {
         y = anchorRect.Y + anchorRect.Height / 2;
      }

      return new Point(x, y);
   }

   private Rect GetBounds(Rect anchorRect)
   {
      // 暂时只支持窗口的方式
      if (_managedPopupPositioner is null) {
         throw new InvalidOperationException("ManagedPopupPositioner is null");
      }

      var parentGeometry = _managedPopupPositioner.ParentClientAreaScreenGeometry;
      var screens = _managedPopupPositioner.Screens;

      var targetScreen = screens.FirstOrDefault(s => s.Bounds.ContainsExclusive(anchorRect.TopLeft))
                         ?? screens.FirstOrDefault(s => s.Bounds.Intersects(anchorRect))
                         ?? screens.FirstOrDefault(s => s.Bounds.ContainsExclusive(parentGeometry.TopLeft))
                         ?? screens.FirstOrDefault(s => s.Bounds.Intersects(parentGeometry))
                         ?? screens.FirstOrDefault();

      if (targetScreen != null &&
          (targetScreen.WorkingArea.Width == 0 && targetScreen.WorkingArea.Height == 0)) {
         return targetScreen.Bounds;
      }

      return targetScreen?.WorkingArea
             ?? new Rect(0, 0, double.MaxValue, double.MaxValue);
   }

   public static void ConfigurePosition(ref PopupPositionerParameters positionerParameters,
                                        TopLevel topLevel,
                                        Visual target, PlacementMode placement, Point offset,
                                        PopupAnchor anchor, PopupGravity gravity,
                                        PopupPositionerConstraintAdjustment constraintAdjustment, Rect? rect,
                                        FlowDirection flowDirection)
   {

      var arguments = new object?[]
      { 
         positionerParameters,
         topLevel,
         target,
         placement,
         offset,
         anchor,
         gravity,
         constraintAdjustment,
         rect,
         flowDirection
      };
      ConfigurePositionMethodInfo.Invoke(null, arguments);
      positionerParameters = (PopupPositionerParameters)arguments[0]!;
   }

   internal static Direction GetDirection(PlacementMode placement)
   {
      return placement switch
      {
         PlacementMode.Left => Direction.Left,
         PlacementMode.LeftEdgeAlignedBottom => Direction.Left,
         PlacementMode.LeftEdgeAlignedTop => Direction.Left,

         PlacementMode.Top => Direction.Top,
         PlacementMode.TopEdgeAlignedLeft => Direction.Top,
         PlacementMode.TopEdgeAlignedRight => Direction.Top,

         PlacementMode.Right => Direction.Right,
         PlacementMode.RightEdgeAlignedBottom => Direction.Right,
         PlacementMode.RightEdgeAlignedTop => Direction.Right,

         PlacementMode.Bottom => Direction.Bottom,
         PlacementMode.BottomEdgeAlignedLeft => Direction.Bottom,
         PlacementMode.BottomEdgeAlignedRight => Direction.Bottom,
         _ => throw new ArgumentOutOfRangeException(nameof(placement), placement, "Invalid value for PlacementMode")
      };
   }
   
   private Point CalculateMarginToAnchorOffset(PlacementMode placement)
   {
      var offsetX = 0d;
      var offsetY = 0d;
      if (placement != PlacementMode.Center && 
          placement != PlacementMode.Pointer &&
          IsCanonicalAnchorType(placement, PlacementAnchor, PlacementGravity)) {
         var direction = GetDirection(placement);
         if (direction == Direction.Bottom) {
            offsetY += MarginToAnchor;
         } else if (direction == Direction.Top) {
            offsetY += -MarginToAnchor;
         } else if (direction == Direction.Left) {
            offsetX += -MarginToAnchor;
         } else {
            offsetX += MarginToAnchor;
         }
      } else if (placement == PlacementMode.Pointer) {
         offsetX += MarginToAnchor;
         offsetY += MarginToAnchor;
      }
      return new Point(offsetX, offsetY);
   }
   
   /// <summary>
   /// 是否为标准的 anchor 类型
   /// </summary>
   /// <param name="placement"></param>
   /// <param name="anchor"></param>
   /// <param name="gravity"></param>
   /// <returns></returns>
   internal static bool IsCanonicalAnchorType(PlacementMode placement, PopupAnchor? anchor, PopupGravity? gravity)
   {
      if (placement == PlacementMode.AnchorAndGravity) {
         switch (anchor, gravity) {
            case (PopupAnchor.Bottom, PopupGravity.Bottom):
            case (PopupAnchor.Right, PopupGravity.Right):
            case (PopupAnchor.Left, PopupGravity.Left):
            case (PopupAnchor.Top, PopupGravity.Top):
            case (PopupAnchor.TopRight, PopupGravity.TopLeft):
            case (PopupAnchor.TopLeft, PopupGravity.TopRight):
            case (PopupAnchor.BottomLeft, PopupGravity.BottomRight):
            case (PopupAnchor.BottomRight, PopupGravity.BottomLeft):
            case (PopupAnchor.TopLeft, PopupGravity.BottomLeft):
            case (PopupAnchor.BottomLeft, PopupGravity.TopLeft):
            case (PopupAnchor.TopRight, PopupGravity.BottomRight):
            case (PopupAnchor.BottomRight, PopupGravity.TopRight):
               break;
            default:
               return false;
         }
      }

      return true;
   }
   
   protected internal override void NotifyAboutToUpdateHostPosition(IPopupHost popupHost, Control placementTarget)
   {
      base.NotifyAboutToUpdateHostPosition(popupHost, placementTarget);
      var offsetX = HorizontalOffset;
      var offsetY = VerticalOffset;
      var marginToAnchorOffset = CalculateMarginToAnchorOffset(Placement);
      offsetX += marginToAnchorOffset.X;
      offsetY += marginToAnchorOffset.Y;
      HorizontalOffset = offsetX;
      VerticalOffset = offsetY;

      if (Placement != PlacementMode.Center &&
          Placement != PlacementMode.Pointer) {
         // 计算是否 flip
         PopupPositionerParameters parameters = new PopupPositionerParameters();
         if (popupHost is PopupRoot popupRoot) {
            var offset = new Point(HorizontalOffset, VerticalOffset);
            ConfigurePosition(ref parameters, popupRoot.ParentTopLevel,
                              PlacementTarget!,
                              Placement,
                              offset,
                              PlacementAnchor,
                              PlacementGravity,
                              PopupPositionerConstraintAdjustment.All,
                              null,
                              FlowDirection);
            
            Size popupSize;
            // Popup.Child can't be null here, it was set in ShowAtCore.
            if (Child!.DesiredSize == default) {
               // Popup may not have been shown yet. Measure content
               popupSize = LayoutHelper.MeasureChild(Child, Size.Infinity, new Thickness());
            } else {
               popupSize = Child.DesiredSize;
            }
            var scaling = _managedPopupPositioner!.Scaling;
            var anchorRect = new Rect(
               parameters.AnchorRectangle.TopLeft * scaling,
               parameters.AnchorRectangle.Size * scaling);
            anchorRect = anchorRect.Translate(_managedPopupPositioner.ParentClientAreaScreenGeometry.TopLeft);
            
            var flipInfo = CalculateFlipInfo(popupSize * scaling,
                                             anchorRect,
                                             parameters.Anchor,
                                             parameters.Gravity,
                                             offset);
            if (flipInfo.Item1 || flipInfo.Item2) {
               var flipPlacement = GetFlipPlacement(Placement);
               var flipAnchorAndGravity = GetAnchorAndGravity(flipPlacement);
               var flipOffset = CalculateMarginToAnchorOffset(flipPlacement);
               Placement = flipPlacement;
               PlacementAnchor = flipAnchorAndGravity.Item1;
               PlacementGravity = flipAnchorAndGravity.Item2;
               HorizontalOffset = flipOffset.X;
               VerticalOffset = flipOffset.Y;
            }
         }
      }
   }
   
   private (PopupAnchor, PopupGravity) GetAnchorAndGravity(PlacementMode placement)
   {
      return placement switch
      {
         PlacementMode.Bottom => (PopupAnchor.Bottom, PopupGravity.Bottom),
         PlacementMode.Right => (PopupAnchor.Right, PopupGravity.Right),
         PlacementMode.Left => (PopupAnchor.Left, PopupGravity.Left),
         PlacementMode.Top => (PopupAnchor.Top, PopupGravity.Top),
         PlacementMode.TopEdgeAlignedRight => (PopupAnchor.TopRight, PopupGravity.TopLeft),
         PlacementMode.TopEdgeAlignedLeft => (PopupAnchor.TopLeft, PopupGravity.TopRight),
         PlacementMode.BottomEdgeAlignedLeft => (PopupAnchor.BottomLeft, PopupGravity.BottomRight),
         PlacementMode.BottomEdgeAlignedRight => (PopupAnchor.BottomRight, PopupGravity.BottomLeft),
         PlacementMode.LeftEdgeAlignedTop => (PopupAnchor.TopLeft, PopupGravity.BottomLeft),
         PlacementMode.LeftEdgeAlignedBottom => (PopupAnchor.BottomLeft, PopupGravity.TopLeft),
         PlacementMode.RightEdgeAlignedTop => (PopupAnchor.TopRight, PopupGravity.BottomRight),
         PlacementMode.RightEdgeAlignedBottom => (PopupAnchor.BottomRight, PopupGravity.TopRight),
         _ => throw new ArgumentOutOfRangeException(nameof(placement), placement, "Invalid value for PlacementMode")
      };
   }
   
   protected PlacementMode GetFlipPlacement(PlacementMode placement)
   {
      return placement switch
      {
         PlacementMode.Left => PlacementMode.Right,
         PlacementMode.LeftEdgeAlignedTop => PlacementMode.RightEdgeAlignedTop,
         PlacementMode.LeftEdgeAlignedBottom => PlacementMode.RightEdgeAlignedBottom,

         PlacementMode.Top => PlacementMode.Bottom,
         PlacementMode.TopEdgeAlignedLeft => PlacementMode.BottomEdgeAlignedLeft,
         PlacementMode.TopEdgeAlignedRight => PlacementMode.BottomEdgeAlignedRight,

         PlacementMode.Right => PlacementMode.Left,
         PlacementMode.RightEdgeAlignedTop => PlacementMode.LeftEdgeAlignedTop,
         PlacementMode.RightEdgeAlignedBottom => PlacementMode.LeftEdgeAlignedBottom,

         PlacementMode.Bottom => PlacementMode.Top,
         PlacementMode.BottomEdgeAlignedLeft => PlacementMode.TopEdgeAlignedLeft,
         PlacementMode.BottomEdgeAlignedRight => PlacementMode.TopEdgeAlignedRight,
         
         _ => throw new ArgumentOutOfRangeException(nameof(placement), placement, "Invalid value for PlacementMode")
      };
   }
}