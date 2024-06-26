using System.Reflection;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Rendering;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Input;

class TransformTrackingHelper : IDisposable
{
   private Visual? _visual;
   private bool _queuedForUpdate;
   private readonly EventHandler<AvaloniaPropertyChangedEventArgs> _propertyChangedHandler;
   private readonly List<Visual> _propertyChangedSubscriptions = new List<Visual>();
   
   private static readonly PropertyInfo IsAttachedToVisualTreeInfo;
   private static readonly PropertyInfo VisualRootInfo;
   private static readonly PropertyInfo AfterRenderInfo;
   private static readonly PropertyInfo IsEffectiveValueChangeInfo;

   static TransformTrackingHelper()
   {
      IsAttachedToVisualTreeInfo = typeof(Visual).GetPropertyInfoOrThrow("IsAttachedToVisualTree");
      VisualRootInfo = typeof(Visual).GetPropertyInfoOrThrow("VisualRoot");
      AfterRenderInfo = typeof(DispatcherPriority).GetPropertyInfoOrThrow("AfterRender");
      IsEffectiveValueChangeInfo =
         typeof(AvaloniaPropertyChangedEventArgs).GetPropertyInfoOrThrow("IsEffectiveValueChange");
   }

   private static bool IsAttachedToVisualTree(Visual visual)
   {
      return (bool)IsAttachedToVisualTreeInfo.GetValue(visual)!;
   }

   private static IRenderRoot? VisualRoot(Visual visual)
   {
      return VisualRootInfo.GetValue(visual) as IRenderRoot;
   }

   private static DispatcherPriority AfterRenderDispatcherPriority()
   {
      return (DispatcherPriority)AfterRenderInfo.GetValue(null)!;
   }

   private static bool IsEffectiveValueChange(AvaloniaPropertyChangedEventArgs args)
   {
      return (bool)IsEffectiveValueChangeInfo.GetValue(args)!;
   }
   
   public TransformTrackingHelper()
   {
      _propertyChangedHandler = PropertyChangedHandler;
   }

   public void SetVisual(Visual? visual)
   {
      Dispose();
      _visual = visual;
      if (visual != null) {
         visual.AttachedToVisualTree += OnAttachedToVisualTree;
         visual.DetachedFromVisualTree -= OnDetachedFromVisualTree;
         if (IsAttachedToVisualTree(visual)) {
            SubscribeToParents();
         }
         UpdateMatrix();
      }
   }

   public Matrix? Matrix { get; private set; }
   public event Action? MatrixChanged;

   public void Dispose()
   {
      if (_visual == null) return;
      UnsubscribeFromParents();
      _visual.AttachedToVisualTree -= OnAttachedToVisualTree;
      _visual.DetachedFromVisualTree -= OnDetachedFromVisualTree;
      _visual = null;
   }

   private void SubscribeToParents()
   {
      var visual = _visual;
      // ReSharper disable once ConditionIsAlwaysTrueOrFalse
      // false positive
      while (visual != null) {
         if (visual is Visual v) {
            v.PropertyChanged += _propertyChangedHandler;
            _propertyChangedSubscriptions.Add(v);
         }

         visual = visual.GetVisualParent();
      }
   }

   private void UnsubscribeFromParents()
   {
      foreach (var v in _propertyChangedSubscriptions) v.PropertyChanged -= _propertyChangedHandler;
      _propertyChangedSubscriptions.Clear();
   }

   void UpdateMatrix()
   {
      Matrix? matrix = null;
      if (_visual != null && VisualRoot(_visual) != null) matrix = _visual.TransformToVisual((Visual)VisualRoot(_visual)!);
      if (Matrix != matrix) {
         Matrix = matrix;
         MatrixChanged?.Invoke();
      }
   }

   private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs visualTreeAttachmentEventArgs)
   {
      SubscribeToParents();
      UpdateMatrix();
   }

   private void EnqueueForUpdate()
   {
      if (_queuedForUpdate) return;
      _queuedForUpdate = true;
      Dispatcher.UIThread.Post(UpdateMatrix, AfterRenderDispatcherPriority());
   }

   private void PropertyChangedHandler(object? sender, AvaloniaPropertyChangedEventArgs e)
   {
      if (IsEffectiveValueChange(e) && e.Property == Visual.BoundsProperty) EnqueueForUpdate();
   }

   private void OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs visualTreeAttachmentEventArgs)
   {
      UnsubscribeFromParents();
      UpdateMatrix();
   }

   public static IDisposable Track(Visual visual, Action<Visual, Matrix?> cb)
   {
      var rv = new TransformTrackingHelper();
      rv.MatrixChanged += () => cb(visual, rv.Matrix);
      rv.SetVisual(visual);
      return rv;
   }
}