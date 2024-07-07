using System.Reflection;
using AtomUI.Platform.Windows;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.VisualTree;

namespace AtomUI.MotionScene;

public class SceneLayer : WindowBase, IHostedVisualTreeRoot, IDisposable
{
   private IManagedPopupPositionerPopup? _managedPopupPositionerPopup;
   private static readonly FieldInfo ManagedPopupPositionerPopupInfo;
   private Canvas _layout;
   
   static SceneLayer()
   {
      BackgroundProperty.OverrideDefaultValue(typeof(SceneLayer), Brushes.Transparent);
      ManagedPopupPositionerPopupInfo = typeof(ManagedPopupPositioner).GetField("_popup",
         BindingFlags.Instance | BindingFlags.NonPublic)!;
      
   }

   public SceneLayer(TopLevel parent, IPopupImpl impl)
      : this(parent, impl, null) { }
   
   /// <summary>
   /// 初始化一个新的动画顶层窗口
   /// </summary>
   /// <param name="parent"></param>
   /// <param name="impl"></param>
   /// <param name="dependencyResolver">The dependency resolver to use. If null the default dependency resolver will be used.</param>
   public SceneLayer(TopLevel parent, IPopupImpl impl, IAvaloniaDependencyResolver? dependencyResolver)
      : base(impl, dependencyResolver)
   {
      ParentTopLevel = parent;
      impl.SetWindowManagerAddShadowHint(false);
      if (this is WindowBase window) {
         window.SetTransparentForMouseEvents(true);
      }
      
      if (PlatformImpl?.PopupPositioner is ManagedPopupPositioner managedPopupPositioner) {
         _managedPopupPositionerPopup =
            ManagedPopupPositionerPopupInfo.GetValue(managedPopupPositioner) as IManagedPopupPositionerPopup;
      }

      _layout = new Canvas();
      Content = _layout;
#if DEBUG
      if (this is TopLevel topLevel) {
         topLevel.AttachDevTools();
      }
#endif
      Focusable = true;
   }
   
   /// <summary>
   /// Gets the platform-specific window implementation.
   /// </summary>
   public new IPopupImpl? PlatformImpl => (IPopupImpl?)base.PlatformImpl;

   /// <summary>
   /// Gets the control that is hosting the popup root.
   /// </summary>
   Visual? IHostedVisualTreeRoot.Host
   {
      get
      {
         // If the parent is attached to a visual tree, then return that. However the parent
         // will possibly be a standalone Popup (i.e. a Popup not attached to a visual tree,
         // created by e.g. a ContextMenu): if this is the case, return the ParentTopLevel
         // if set. This helps to allow the focus manager to restore the focus to the outer
         // scope when the popup is closed.
         var parentVisual = Parent as Visual;
         if (parentVisual?.GetVisualRoot() != null) return parentVisual;
         return ParentTopLevel ?? parentVisual;
      }
   }

   public TopLevel ParentTopLevel { get; }

   public void Dispose()
   {
      PlatformImpl?.Dispose();
   }

   public void SetMotionTarget(Control motionTarget)
   {
      _layout.Children.Add(motionTarget);
   }

   // 这个地方我们可以需要定制
   protected override Size MeasureOverride(Size availableSize)
   {
      var maxAutoSize = PlatformImpl?.MaxAutoSizeHint ?? Size.Infinity;
      var constraint = availableSize;

      if (double.IsInfinity(constraint.Width)) {
         constraint = constraint.WithWidth(maxAutoSize.Width);
      }

      if (double.IsInfinity(constraint.Height)) {
         constraint = constraint.WithHeight(maxAutoSize.Height);
      }

      var measured = base.MeasureOverride(constraint);
      var width = measured.Width;
      var height = measured.Height;
      var widthCache = Width;
      var heightCache = Height;

      if (!double.IsNaN(widthCache)) {
         width = widthCache;
      }

      width = Math.Min(width, MaxWidth);
      width = Math.Max(width, MinWidth);

      if (!double.IsNaN(heightCache)) {
         height = heightCache;
      }

      height = Math.Min(height, MaxHeight);
      height = Math.Max(height, MinHeight);

      return new Size(width, height);
   }

   protected override Size ArrangeSetBounds(Size size)
   {
      return ClientSize;
   }

   public void MoveAndResize(Point point, Size size)
   {
      Width = size.Width;
      Height = size.Height;
      _managedPopupPositionerPopup?.MoveAndResize(point, size);
   }

   protected override void OnOpened(EventArgs e)
   {
      base.OnOpened(e);
      foreach (var child in _layout.Children) {
         if (child is INotifyCaptureGhostBitmap captureGhost) {
            captureGhost.NotifyCaptureGhostBitmap();
         }
      }
   }

   public override void Render(DrawingContext context)
   {
      //context.FillRectangle(new SolidColorBrush(Colors.Coral, 0.2), new Rect(new Point(0, 0), DesiredSize));
   }
}