using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public class LiteWindow : WindowBase, IHostedVisualTreeRoot, IDisposable
{
   static LiteWindow()
   {
      BackgroundProperty.OverrideDefaultValue(typeof(LiteWindow), Brushes.White);
   }
   
   public LiteWindow(TopLevel parent, IPopupImpl impl)
      : this(parent, impl, null) { }

   /// <summary>
   /// Initializes a new instance of the <see cref="PopupRoot"/> class.
   /// </summary>
   /// <param name="parent">The popup parent.</param>
   /// <param name="impl">The popup implementation.</param>
   /// <param name="dependencyResolver">
   /// The dependency resolver to use. If null the default dependency resolver will be used.
   /// </param>
   public LiteWindow(TopLevel parent, IPopupImpl impl, IAvaloniaDependencyResolver? dependencyResolver)
      : base(impl, dependencyResolver)
   {
      ParentTopLevel = parent;
      impl.SetWindowManagerAddShadowHint(false);
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

   /// <inheritdoc/>
   public void Dispose()
   {
      PlatformImpl?.Dispose();
   }

   public void SetChild(Control? control) => Content = control;

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
}