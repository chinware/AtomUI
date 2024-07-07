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
   private GlobalTokenBinder _globalTokenBinder;
   private CompositeDisposable? _compositeDisposable;
   private bool _initialized;

   private PlacementMode? _originPlacementMode;
   private PopupAnchor? _originPlacementAnchor;
   private PopupGravity? _originPlacementGravity;
   private double _originOffsetX;
   private double _originOffsetY;
   private bool _ignoreSyncOriginValues = false;

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
      _originOffsetX = HorizontalOffset;
      _originOffsetY = VerticalOffset;
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      if (!_ignoreSyncOriginValues) {
         if (e.Property == HorizontalOffsetProperty) {
            _originOffsetX = e.GetNewValue<double>();
         } else if (e.Property == VerticalOffsetProperty) {
            _originOffsetY = e.GetNewValue<double>();
         } else if (e.Property == PlacementProperty) {
            _originPlacementMode = e.GetNewValue<PlacementMode>();
         } else if (e.Property == PlacementAnchorProperty) {
            _originPlacementAnchor = e.GetNewValue<PopupAnchor>();
         } else if (e.Property == PlacementGravityProperty) {
            _originPlacementGravity = e.GetNewValue<PopupGravity>();
         }
      }
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

   internal (bool, bool) CalculateFlipInfo(Rect bounds, Size translatedSize, Rect anchorRect, PopupAnchor anchor,
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

   private Rect GetBounds(Rect anchorRect, Rect parentGeometry, IReadOnlyList<ManagedPopupPositionerScreenInfo> screens)
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

   private IReadOnlyList<ManagedPopupPositionerScreenInfo> GetScreenInfos(TopLevel topLevel)
   {
      if (topLevel is WindowBase window) {
         var windowImpl = window.PlatformImpl!;
         return windowImpl.Screen.AllScreens
                   .Select(s => new ManagedPopupPositionerScreenInfo(s.Bounds.ToRect(1), s.WorkingArea.ToRect(1)))
                   .ToArray();
      }
      return Array.Empty<ManagedPopupPositionerScreenInfo>();
   }

   private Rect GetParentClientAreaScreenGeometry(TopLevel topLevel)
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
          PopupUtils.IsCanonicalAnchorType(placement, PlacementAnchor, PlacementGravity)) {
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
   
   protected internal override void NotifyPopupRootAboutToShow(PopupRoot popupRoot)
   {
      base.NotifyPopupRootAboutToShow(popupRoot);
      using var ignoreSyncOriginHandling = IgnoreSyncOriginValueHandling();
      var offsetX = _originOffsetX;
      var offsetY = _originOffsetY;
      var marginToAnchorOffset = CalculateMarginToAnchorOffset(Placement);
      offsetX += marginToAnchorOffset.X;
      offsetY += marginToAnchorOffset.Y;
      HorizontalOffset = offsetX;
      VerticalOffset = offsetY;

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
            var flipAnchorAndGravity = GetAnchorAndGravity(flipPlacement);
            var flipOffset = CalculateMarginToAnchorOffset(flipPlacement);
            
            _originPlacementMode = Placement;
            _originPlacementAnchor = PlacementAnchor;
            _originPlacementGravity = PlacementGravity;
            
            Placement = flipPlacement;
            PlacementAnchor = flipAnchorAndGravity.Item1;
            PlacementGravity = flipAnchorAndGravity.Item2;
            HorizontalOffset = flipOffset.X;
            VerticalOffset = flipOffset.Y;
            IsFlipped = true;
            
         } else {
            IsFlipped = false;
           
            if (_originPlacementMode.HasValue) {
               Placement = _originPlacementMode.Value;
            }

            if (_originPlacementAnchor.HasValue) {
               PlacementAnchor = _originPlacementAnchor.Value;
            }

            if (_originPlacementGravity.HasValue) {
               PlacementGravity = _originPlacementGravity.Value;
            }

            _originPlacementMode = null;
            _originPlacementAnchor = null;
            _originPlacementGravity = null;
         }
      }
   }

   internal PopupPositionInfo CalculatePositionInfo(Control placementTarget, Control popupContent)
   {
      using var ignoreSyncOriginHandling = IgnoreSyncOriginValueHandling();
      var offsetX = _originOffsetX;
      var offsetY = _originOffsetY + 0.5; // TODO 不知道为什么会出现 0.5 的误差
      
      var marginToAnchorOffset = CalculateMarginToAnchorOffset(Placement);
      offsetX += marginToAnchorOffset.X;
      offsetY += marginToAnchorOffset.Y;
      Point offset = default;
      PopupPositionerParameters parameters = new PopupPositionerParameters();
      var parentTopLevel = TopLevel.GetTopLevel(placementTarget)!;
      
      // Popup.Child can't be null here, it was set in ShowAtCore.
      if (popupContent.DesiredSize == default) {
         // Popup may not have been shown yet. Measure content
         parameters.Size = LayoutHelper.MeasureChild(popupContent, Size.Infinity, new Thickness());
      } else {
         parameters.Size = popupContent.DesiredSize;
      }
      
      if (Placement != PlacementMode.Center &&
          Placement != PlacementMode.Pointer) {
         offset = new Point(offsetX, offsetY);
      }

      ConfigurePosition(ref parameters, parentTopLevel,
                        placementTarget,
                        Placement,
                        offset,
                        PlacementAnchor,
                        PlacementGravity,
                        PopupPositionerConstraintAdjustment.All,
                        PlacementRect ?? new Rect(default, placementTarget.Bounds.Size),
                        FlowDirection);
      
      var positionInfo = new PopupPositionInfo();
      positionInfo.EffectivePlacement = Placement;
      positionInfo.EffectivePlacementAnchor = PlacementAnchor;
      positionInfo.EffectivePlacementGravity = PlacementGravity;
      positionInfo.Size = parameters.Size;
      positionInfo.Offset = parameters.Offset;
      
      var scaling = parentTopLevel.RenderScaling;
      var parentGeometry = GetParentClientAreaScreenGeometry(parentTopLevel);
      var screens = GetScreenInfos(parentTopLevel);
      
      if (Placement != PlacementMode.Center &&
          Placement != PlacementMode.Pointer) {
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
            var flipPlacement = GetFlipPlacement(Placement);
            var flipAnchorAndGravity = GetAnchorAndGravity(flipPlacement);
            var flipOffset = CalculateMarginToAnchorOffset(flipPlacement);
            
            positionInfo.EffectivePlacement = flipPlacement;
            positionInfo.EffectivePlacementAnchor = flipAnchorAndGravity.Item1;
            positionInfo.EffectivePlacementGravity = flipAnchorAndGravity.Item2;
            positionInfo.Offset = flipOffset;
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
         parameters.Offset * scaling,
         parentGeometry,
         screens);

      positionInfo.Offset = rect.Position;
      positionInfo.Size = rect.Size;
      positionInfo.Scaling = scaling;
      
      return positionInfo;
   }
   
   internal static (PopupAnchor, PopupGravity) GetAnchorAndGravity(PlacementMode placement)
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
   
   private protected IDisposable IgnoreSyncOriginValueHandling()
   {
      return new IgnoreSyncOriginValueDisposable(this);
   }
   
   private readonly struct IgnoreSyncOriginValueDisposable : IDisposable
   {
      private readonly Popup _popup;

      public IgnoreSyncOriginValueDisposable(Popup popup)
      {
         _popup = popup;
         _popup._ignoreSyncOriginValues = true;
      }
            
      public void Dispose()
      {
         _popup._ignoreSyncOriginValues = false;
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