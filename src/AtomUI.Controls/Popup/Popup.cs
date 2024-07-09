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

   public event EventHandler<PopupFlippedEventArgs>? PositionFlipped;
   
   public static readonly StyledProperty<BoxShadows> MaskShadowsProperty =
      Border.BoxShadowProperty.AddOwner<Popup>();

   public static readonly StyledProperty<double> MarginToAnchorProperty =
      AvaloniaProperty.Register<Popup, double>(nameof(MarginToAnchor));
   
   public static readonly DirectProperty<Popup, bool> IsFlippedProperty =
      AvaloniaProperty.RegisterDirect<Popup, bool>(nameof(IsFlipped),
                                                   o => o.IsFlipped,
                                                   (o, v) => o.IsFlipped = v);
   
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

   private bool _isFlipped = false;
   public bool IsFlipped
   {
      get => _isFlipped;
      private set => SetAndRaise(IsFlippedProperty, ref _isFlipped, value);
   }
   
   private static readonly MethodInfo ConfigurePositionMethodInfo;
   private PopupShadowLayer? _shadowLayer;
   private readonly GlobalTokenBinder _globalTokenBinder;
   private CompositeDisposable? _compositeDisposable;
   private bool _initialized;

   static Popup()
   {
      var type = typeof(IPopupPositioner).Assembly.GetType("Avalonia.Controls.Primitives.PopupPositioning.PopupPositionerExtensions")!;
      ConfigurePositionMethodInfo = type.GetMethod("ConfigurePosition", BindingFlags.Public | BindingFlags.Static)!;
      AffectsMeasure<Popup>(PlacementProperty);
      AffectsMeasure<Popup>(PlacementAnchorProperty);
      AffectsMeasure<Popup>(PlacementGravityProperty);
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
         _compositeDisposable?.Add(BindUtils.RelayBind(this, OpacityProperty, _shadowLayer!));
         _compositeDisposable?.Add(BindUtils.RelayBind(this, OpacityProperty, (popupHost as Control)!));
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
      var bounds = GetBounds(anchorRect);
      return CalculateFlipInfo(bounds, translatedSize, anchorRect, anchor, gravity, offset);
   }

   internal static (bool, bool) CalculateFlipInfo(Rect bounds, Size translatedSize, Rect anchorRect, PopupAnchor anchor,
                                                 PopupGravity gravity,
                                                 Point offset)
   {
      var result = (false, false);

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
         new Rect(PopupUtils.Gravitate(PopupUtils.GetAnchorPoint(anchorRect, a), translatedSize, g) + offset, translatedSize);

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
   
   private Rect GetBounds(Rect anchorRect)
   {
      // 暂时只支持窗口的方式
      if (_managedPopupPositioner is null) {
         throw new InvalidOperationException("ManagedPopupPositioner is null");
      }

      var parentGeometry = _managedPopupPositioner.ParentClientAreaScreenGeometry;
      var screens = _managedPopupPositioner.Screens;
      return GetBounds(anchorRect, parentGeometry, screens);
   }

   private static Rect GetBounds(Rect anchorRect, Rect parentGeometry, IReadOnlyList<ManagedPopupPositionerScreenInfo> screens)
   {
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

   private static IReadOnlyList<ManagedPopupPositionerScreenInfo> GetScreenInfos(TopLevel topLevel)
   {
      if (topLevel is WindowBase window) {
         var windowImpl = window.PlatformImpl!;
         return windowImpl.Screen.AllScreens
                   .Select(s => new ManagedPopupPositionerScreenInfo(s.Bounds.ToRect(1), s.WorkingArea.ToRect(1)))
                   .ToArray();
      }
      return Array.Empty<ManagedPopupPositionerScreenInfo>();
   }

   private static Rect GetParentClientAreaScreenGeometry(TopLevel topLevel)
   {
      // Popup positioner operates with abstract coordinates, but in our case they are pixel ones
      var point = topLevel.PointToScreen(default);
      var size = topLevel.ClientSize * topLevel.RenderScaling;
      return new Rect(point.X, point.Y, size.Width, size.Height);
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
   
   protected internal override void NotifyPopupRootAboutToShow(PopupRoot popupRoot)
   {
      base.NotifyPopupRootAboutToShow(popupRoot);
      var offsetX = HorizontalOffset;
      var offsetY = VerticalOffset;
      var marginToAnchorOffset = PopupUtils.CalculateMarginToAnchorOffset(Placement, MarginToAnchor, PlacementAnchor, PlacementGravity);
      offsetX += marginToAnchorOffset.X;
      offsetY += marginToAnchorOffset.Y;
      HorizontalOffset = offsetX;
      VerticalOffset = offsetY;

      var direction = PopupUtils.GetDirection(Placement);
      
      if (Placement != PlacementMode.Center &&
          Placement != PlacementMode.Pointer) {
         // 计算是否 flip
         PopupPositionerParameters parameters = new PopupPositionerParameters();
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
                                          offset * scaling);
         if (flipInfo.Item1 || flipInfo.Item2) {
            var flipPlacement = GetFlipPlacement(Placement);
            var flipAnchorAndGravity = PopupUtils.GetAnchorAndGravity(flipPlacement);
            var flipOffset = PopupUtils.CalculateMarginToAnchorOffset(flipPlacement, MarginToAnchor, PlacementAnchor, PlacementGravity);
            
            Placement = flipPlacement;
            PlacementAnchor = flipAnchorAndGravity.Item1;
            PlacementGravity = flipAnchorAndGravity.Item2;

            // 这里有个问题，目前需要重新看看，就是 X 轴 和 Y 轴会不会同时被反转呢？
            
            if (direction == Direction.Top || direction == Direction.Bottom) {
               VerticalOffset = flipOffset.Y;
            } else {
               HorizontalOffset = flipOffset.X;
            }
            IsFlipped = true;
            
         } else {
            IsFlipped = false;
         }
         PositionFlipped?.Invoke(this, new PopupFlippedEventArgs(IsFlipped));
      }
   }
   
   internal static PopupPositionInfo CalculatePositionInfo(Control placementTarget, 
                                                           double marginToAnchor,
                                                           Control popupContent, 
                                                           Point offset,
                                                           PlacementMode placement,
                                                           PopupAnchor placementAnchor,
                                                           PopupGravity placementGravity,
                                                           Rect? placementRect,
                                                           FlowDirection flowDirection)
   {
      var offsetX = offset.X;
      var offsetY = offset.Y;
      
      var marginToAnchorOffset = PopupUtils.CalculateMarginToAnchorOffset(placement, marginToAnchor, placementAnchor, placementGravity);
      offsetX += marginToAnchorOffset.X;
      offsetY += marginToAnchorOffset.Y;

      var direction = PopupUtils.GetDirection(placement);
  
      PopupPositionerParameters parameters = new PopupPositionerParameters();
      var parentTopLevel = TopLevel.GetTopLevel(placementTarget)!;
      
      // Popup.Child can't be null here, it was set in ShowAtCore.
      if (popupContent.DesiredSize == default) {
         // Popup may not have been shown yet. Measure content
         parameters.Size = LayoutHelper.MeasureChild(popupContent, Size.Infinity, new Thickness());
      } else {
         parameters.Size = popupContent.DesiredSize;
      }
      
      if (placement != PlacementMode.Center &&
          placement != PlacementMode.Pointer) {
         offset = new Point(offsetX, offsetY);
      }
      
      ConfigurePosition(ref parameters, parentTopLevel,
                        placementTarget,
                        placement,
                        offset,
                        placementAnchor,
                        placementGravity,
                        PopupPositionerConstraintAdjustment.All,
                        placementRect ?? new Rect(default, placementTarget.Bounds.Size),
                        flowDirection);
      
      var positionInfo = new PopupPositionInfo();
      positionInfo.EffectivePlacement = placement;
      positionInfo.EffectivePlacementAnchor = placementAnchor;
      positionInfo.EffectivePlacementGravity = placementGravity;
      positionInfo.Size = parameters.Size;
      positionInfo.Offset = parameters.Offset;
      
      var scaling = parentTopLevel.RenderScaling;
      var parentGeometry = GetParentClientAreaScreenGeometry(parentTopLevel);
      var screens = GetScreenInfos(parentTopLevel);
      
      if (placement != PlacementMode.Center &&
          placement != PlacementMode.Pointer) {
         // 计算是否 flip
         var anchorRect = new Rect(
            parameters.AnchorRectangle.TopLeft * scaling,
            parameters.AnchorRectangle.Size * scaling);
         
         var parentOffsetPoint = parentTopLevel.PointToScreen(default);
      
         anchorRect = anchorRect.Translate(new Point(parentOffsetPoint.X, parentOffsetPoint.Y));
   
         var bounds = GetBounds(anchorRect, parentGeometry,screens);
         var flipInfo = CalculateFlipInfo(bounds, 
                                          parameters.Size * scaling,
                                          anchorRect,
                                          parameters.Anchor,
                                          parameters.Gravity,
                                          offset * scaling);
         if (flipInfo.Item1 || flipInfo.Item2) {
            var flipPlacement = GetFlipPlacement(placement);
            var flipAnchorAndGravity = PopupUtils.GetAnchorAndGravity(flipPlacement);
            var flipOffset = PopupUtils.CalculateMarginToAnchorOffset(flipPlacement, marginToAnchor, placementAnchor, placementGravity);
            positionInfo.EffectivePlacement = flipPlacement;
            positionInfo.EffectivePlacementAnchor = flipAnchorAndGravity.Item1;
            positionInfo.EffectivePlacementGravity = flipAnchorAndGravity.Item2;
                        
            if (direction == Direction.Top || direction == Direction.Bottom) {
               positionInfo.Offset = positionInfo.Offset.WithY(flipOffset.Y);
            } else {
               positionInfo.Offset = positionInfo.Offset.WithX(flipOffset.X);
            }
            
            positionInfo.IsFlipped = true;
         } else {
            positionInfo.IsFlipped = false;
         }
      }
      
      var rect = PopupUtils.Calculate(
         parameters.Size * scaling,
         new Rect(
            parameters.AnchorRectangle.TopLeft * scaling,
            parameters.AnchorRectangle.Size * scaling),
         parameters.Anchor,
         parameters.Gravity,
         parameters.ConstraintAdjustment,
         positionInfo.Offset * scaling,
         parentGeometry,
         screens);

      positionInfo.Offset = new Point(Math.Round(rect.Position.X), Math.Floor(rect.Position.Y + 0.5));
      positionInfo.Size = rect.Size;
      positionInfo.Scaling = scaling;
      
      return positionInfo;
   }

   protected static PlacementMode GetFlipPlacement(PlacementMode placement)
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

   public void HideShadowLayer()
   {
      if (_shadowLayer is not null) {
         _shadowLayer.Opacity = 0;
      }
   }
}

internal class PopupPositionInfo
{
   public Point Offset { get; set; }
   public bool IsFlipped { get; set; }
   public Size Size { get; set; }
   public double Scaling { get; set; }
   public PlacementMode EffectivePlacement { get; set; }
   public PopupAnchor EffectivePlacementAnchor { get; set; }
   public PopupGravity EffectivePlacementGravity { get; set; }
}

public class PopupFlippedEventArgs : EventArgs
{
   public bool Flipped { get; set; }
   public PopupFlippedEventArgs(bool flipped)
   {
      Flipped = flipped;
   }
}