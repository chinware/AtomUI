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

   public static IDisposable RelayBind<TSource, TResult>(AvaloniaObject source, AvaloniaProperty<TSource> sourceProperty,
                                                         AvaloniaObject target, AvaloniaProperty<TResult> targetProperty,
                                                         Func<TSource, TResult> converter,
                                                         BindingPriority priority = BindingPriority.Template)
   {
      var registry = AvaloniaPropertyRegistry.Instance;
      if (!registry.IsRegistered(source.GetType(), sourceProperty)) {
         throw new ArgumentException($"Relay source property is not registered for: {source.GetType().Name}.");
      }
      
      if (!registry.IsRegistered(target.GetType(), targetProperty)) {
         throw new ArgumentException($"Relay target property is not registered for: {target.GetType().Name}.");
      }
      
      return target.Bind(targetProperty, source.GetObservable(sourceProperty, converter), priority);
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
                                                BindingPriority priority = BindingPriority.Template,
                                                Func<object?, object?>? converter = null)
   {
      return target.Bind(targetProperty, target.GetResourceObservable(resourceKey, converter), priority);
   }
}