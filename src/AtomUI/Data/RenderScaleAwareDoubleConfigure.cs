using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Data;

internal class RenderScaleAwareDoubleConfigure
{
   private WeakReference<Control> _control;
   private Func<double, double>? _postProcessor;
   
   public RenderScaleAwareDoubleConfigure(Control target, Func<double, double>? postProcessor = null)
   {
      _control = new WeakReference<Control>(target);
      _postProcessor = postProcessor;
   }

   public object? Configure(object? o)
   {
      if (o is double value) {
         var renderScaling = 1d;
         if (_control.TryGetTarget(out Control? target)) {
            var visualRoot = target.GetVisualRoot();
            if (visualRoot is not null) {
               renderScaling = visualRoot.RenderScaling;
            }
         }

         value /= renderScaling;
         if (_postProcessor is not null) {
            return _postProcessor(value);
         }

         return value;
      }
      return o;
   }

   public static implicit operator Func<object?, object?>(RenderScaleAwareDoubleConfigure configure)
   {
      return configure.Configure;
   }
}