using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Data;

internal class RenderScaleAwareThicknessConfigure
{
   private WeakReference<Control> _control;
   private Func<Thickness, Thickness>? _postProcessor;
   
   public RenderScaleAwareThicknessConfigure(Control target, Func<Thickness, Thickness>? postProcessor = null)
   {
      _control = new WeakReference<Control>(target);
      _postProcessor = postProcessor;
   }

   public IObservable<object?> Configure(IObservable<object?> observable)
   {
      return observable.Select(o =>
      {
         if (o is Thickness thickness) {
            var renderScaling = 1d;
            if (_control.TryGetTarget(out Control? target)) {
               var visualRoot = target.GetVisualRoot();
               if (visualRoot is not null) {
                  renderScaling = visualRoot.RenderScaling;
               }
            }
            var result = new Thickness(thickness.Left / renderScaling,
                                       thickness.Top / renderScaling,
                                       thickness.Right / renderScaling,
                                       thickness.Bottom / renderScaling);
            if (_postProcessor is not null) {
               return _postProcessor(result);
            }

            return result;
         }
         return o;
      });
   }

   public static implicit operator Func<IObservable<object?>, IObservable<object?>>(RenderScaleAwareThicknessConfigure configure)
   {
      return configure.Configure;
   }
}