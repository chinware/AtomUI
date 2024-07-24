using System.Reactive.Disposables;
using AtomUI.Controls.MotionScene;
using AtomUI.MotionScene;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;

namespace AtomUI.Controls;

public class Popup : AbstractPopup
{
   public event EventHandler<PopupFlippedEventArgs>? PositionFlipped;
   
   public static readonly StyledProperty<BoxShadows> MaskShadowsProperty =
      Border.BoxShadowProperty.AddOwner<Popup>();

   public static readonly StyledProperty<double> MarginToAnchorProperty =
      AvaloniaProperty.Register<Popup, double>(nameof(MarginToAnchor));
   
   
   private static readonly StyledProperty<TimeSpan> MotionDurationProperty
      = AvaloniaProperty.Register<Popup, TimeSpan>(nameof(MotionDuration));
   
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

   public TimeSpan MotionDuration
   {
      get => GetValue(MotionDurationProperty);
      set => SetValue(MotionDurationProperty, value);
   }

   private bool _isFlipped = false;
   public bool IsFlipped
   {
      get => _isFlipped;
      private set => SetAndRaise(IsFlippedProperty, ref _isFlipped, value);
   }
   
   private PopupShadowLayer? _shadowLayer;
   private CompositeDisposable? _compositeDisposable;
   private bool _initialized;
   private ManagedPopupPositionerInfo? _managedPopupPositioner;
   protected bool _animating = false;

   static Popup()
   {
      AffectsMeasure<Popup>(PlacementProperty);
      AffectsMeasure<Popup>(PlacementAnchorProperty);
      AffectsMeasure<Popup>(PlacementGravityProperty);
   }

   public Popup()
   {
      IsLightDismissEnabled = false;
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         BindUtils.CreateGlobalTokenBinding(this, MaskShadowsProperty, GlobalResourceKey.BoxShadowsSecondary);
         BindUtils.CreateGlobalTokenBinding(this, MotionDurationProperty, GlobalResourceKey.MotionDurationMid);
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
      var placementTarget = GetEffectivePlacementTarget();
      if (placementTarget is not null) {
         var toplevel = TopLevel.GetTopLevel(placementTarget);
         if (toplevel is null) {
            throw new InvalidOperationException(
               "Unable to create shadow layer, top level for PlacementTarget is null.");
         }
         
         // 目前我们只支持 WindowBase Popup
         _managedPopupPositioner = new ManagedPopupPositionerInfo((toplevel as WindowBase)!.PlatformImpl!);
         
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

   /// <summary>
   /// 在这里处理翻转问题
   /// </summary>
   /// <param name="popupHost"></param>
   /// <param name="placementTarget"></param>
   protected internal override void NotifyAboutToUpdateHostPosition(IPopupHost popupHost, Control placementTarget)
   {
      base.NotifyAboutToUpdateHostPosition(popupHost, placementTarget);
   
      var offsetX = HorizontalOffset;
      var offsetY = VerticalOffset;
      var marginToAnchorOffset = PopupUtils.CalculateMarginToAnchorOffset(Placement, MarginToAnchor, PlacementAnchor, PlacementGravity);
      offsetX += marginToAnchorOffset.X;
      offsetY += marginToAnchorOffset.Y;
      HorizontalOffset = offsetX;
      VerticalOffset = offsetY;

      var direction = PopupUtils.GetDirection(Placement);
      var topLevel = TopLevel.GetTopLevel(placementTarget)!;
      
      if (Placement != PlacementMode.Center &&
          Placement != PlacementMode.Pointer) {
         // 计算是否 flip
         PopupPositionerParameters parameters = new PopupPositionerParameters();
         var offset = new Point(HorizontalOffset, VerticalOffset);
         parameters.ConfigurePosition(topLevel,
                                      placementTarget,
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
   
   // TODO Review 需要评估这里等待的变成模式是否正确高效
   public async Task OpenAnimationAsync()
   {
      // AbstractPopup is currently open
      if (_openState != null || _animating) {
         return;
      }

      _animating = true;
      PrepareOpenState();
      if (_openState == null) {
         return;
      }
      
      // 获取 popup 的具体位置，这个就是非常准确的位置，还有大小
      // TODO 暂时只支持 WindowBase popup
      var popupRoot = (_openState.PopupHost as PopupRoot)!;
      popupRoot.Show();
      popupRoot.Hide();
      var popupOffset = popupRoot.PlatformImpl!.Position;
      var offset = new Point(popupOffset.X, popupOffset.Y);
      var topLevel = _openState.TopLevel;
      var scaling = topLevel.RenderScaling;
      
      // 调度动画
      var director = Director.Instance;
      var motion = new ZoomBigInMotion();
      motion.ConfigureOpacity(MotionDuration);
      motion.ConfigureRenderTransform(MotionDuration);
      
      var motionActor =
         new PopupMotionActor(MaskShadows,offset, scaling, Child ?? popupRoot, motion);
      motionActor.DispatchInSceneLayer = true;
      motionActor.SceneParent = topLevel;
      var cts = new CancellationTokenSource();
      var cancelToken = cts.Token;
      
      motionActor.Completed += (sender, args) =>
      {
         OpenOverride();
         if (_openState?.PopupHost is WindowBase window) {
            window.PlatformImpl!.SetTopmost(true);
         }
         _animating = false;
         cts.Cancel();
         if (RequestCloseWhereAnimationCompleted) {
            RequestCloseWhereAnimationCompleted = false;
            Dispatcher.UIThread.InvokeAsync(async () => { await CloseAnimationAsync(); });
         }
      };
      director?.Schedule(motionActor);
      while (!cancelToken.IsCancellationRequested) {
         await Task.Delay(TimeSpan.FromMilliseconds(10), cancelToken);
      }
   }

   public async Task CloseAnimationAsync()
   {
      if (_animating) {
         RequestCloseWhereAnimationCompleted = true;
         return;
      }
      if (_openState is null) {
         return;
      }

      _animating = true;
  
      var director = Director.Instance;
      var motion = new ZoomBigOutMotion();
      motion.ConfigureOpacity(MotionDuration);
      motion.ConfigureRenderTransform(MotionDuration);
      
      var popupRoot = (_openState.PopupHost as PopupRoot)!;
      var popupOffset = popupRoot.PlatformImpl!.Position;
      var offset = new Point(popupOffset.X, popupOffset.Y);
      var scaling = _openState.TopLevel.RenderScaling;
      
      var motionActor = new PopupMotionActor(MaskShadows, offset, scaling, Child ?? popupRoot, motion);
      motionActor.DispatchInSceneLayer = true;
      motionActor.SceneParent = _openState.TopLevel;
      
      var cts = new CancellationTokenSource();
      var cancelToken = cts.Token;
      
      motionActor.SceneShowed += (sender, args) =>
      {
         popupRoot.Opacity = 0;
         HideShadowLayer();
      };

      motionActor.Completed += (sender, args) =>
      {
         _animating = false;
         Close();
         cts.Cancel();
      };

      director?.Schedule(motionActor);
      while (!cancelToken.IsCancellationRequested) {
         await Task.Delay(TimeSpan.FromMilliseconds(10), cancelToken);
      }
   }
}

internal class ManagedPopupPositionerInfo
{
   private readonly IWindowBaseImpl _parent;
   public ManagedPopupPositionerInfo(IWindowBaseImpl parent)
   {
      _parent = parent;
   }

   public IReadOnlyList<ManagedPopupPositionerScreenInfo> Screens =>
      _parent.Screen.AllScreens
             .Select(s => new ManagedPopupPositionerScreenInfo(s.Bounds.ToRect(1), s.WorkingArea.ToRect(1)))
             .ToArray();
   public Rect ParentClientAreaScreenGeometry {  
      get
      {
         // Popup positioner operates with abstract coordinates, but in our case they are pixel ones
         var point = _parent.PointToScreen(default);
         var size = _parent.ClientSize * Scaling;
         return new Rect(point.X, point.Y, size.Width, size.Height);
      }
   }
   public double Scaling => _parent.DesktopScaling;
}

public class PopupFlippedEventArgs : EventArgs
{
   public bool Flipped { get; set; }
   public PopupFlippedEventArgs(bool flipped)
   {
      Flipped = flipped;
   }
}