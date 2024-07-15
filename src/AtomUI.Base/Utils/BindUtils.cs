using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;

namespace AtomUI.Utils;

public static class BindUtils
{
   public static IDisposable RelayBind(AvaloniaObject source, AvaloniaProperty sourceProperty, AvaloniaObject target,
                                       AvaloniaProperty? targetProperty = null,
                                       BindingMode mode = BindingMode.OneWay)
   {
      targetProperty ??= sourceProperty;
      var registry = AvaloniaPropertyRegistry.Instance;
      if (!registry.IsRegistered(source.GetType(), sourceProperty)) {
         throw new ArgumentException($"Relay source property is not registered for: {source.GetType().Name}.");
      }

      if (!registry.IsRegistered(target.GetType(), targetProperty)) {
         throw new ArgumentException($"Relay target property is not registered for: {target.GetType().Name}.");
      }

      var descriptor = new IndexerDescriptor
      {
         Source = source,
         Property = sourceProperty,
         Priority = BindingPriority.Inherited,
         Mode = mode
      };
      return target.Bind(targetProperty, descriptor);
   }

   public static IDisposable CreateTokenBinding(AvaloniaObject target,
                                                AvaloniaProperty targetProperty,
                                                string resourceKey)
   {
      return target.Bind(targetProperty, new DynamicResourceExtension(resourceKey));
   }
   
   public static IDisposable CreateTokenBinding(Control target,
                                                AvaloniaProperty targetProperty,
                                                string resourceKey,
                                                BindingPriority priority = BindingPriority.Style,
                                                Func<IObservable<object?>, IObservable<object?>>? observableConfigure = null)
   {
      var bindingObservable = target.GetResourceObservable(resourceKey);
      if (observableConfigure is not null) {
         bindingObservable = observableConfigure(bindingObservable);
      }
      return target.Bind(targetProperty, bindingObservable, priority);
   }
}