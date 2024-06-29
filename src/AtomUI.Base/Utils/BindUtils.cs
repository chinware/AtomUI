using Avalonia;
using Avalonia.Data;

namespace AtomUI.Utils;

public static class BindUtils
{
   // public static IDisposable RelayBind(AvaloniaObject source, string sourcePropertyName, AvaloniaObject target,
   //                                     string? targetPropertyName = null,
   //                                     BindingMode mode = BindingMode.OneWay)
   // {
   //    // TODO 看看这里的 sourcePropertyName 能不能优化掉
   //    targetPropertyName ??= sourcePropertyName;
   //
   //    Type targetType = target.GetType();
   //    var targetUnderlyingPropertyName = $"{targetPropertyName}Property";
   //    var targetUnderlyingProperty = targetType.GetField(targetUnderlyingPropertyName,
   //                                                       BindingFlags.Public | BindingFlags.Static |
   //                                                       BindingFlags.FlattenHierarchy);
   //    if (targetUnderlyingProperty is null) {
   //       throw new ArgumentException($"Relay target property {targetUnderlyingPropertyName} is not exist.");
   //    }
   //
   //    Type sourceType = source.GetType();
   //    var sourceUnderlyingProperty = sourceType.GetProperty(sourcePropertyName,
   //                                                          BindingFlags.Public | BindingFlags.NonPublic |
   //                                                          BindingFlags.Instance |
   //                                                          BindingFlags.FlattenHierarchy); // 这里不需要加 Property 后缀
   //
   //    if (sourceUnderlyingProperty == null) {
   //       throw new ArgumentException($"Relay source property {sourcePropertyName} is not exist.");
   //    }
   //
   //    var targetPropertyValue = targetUnderlyingProperty.GetValue(null);
   //    if (targetPropertyValue is null) {
   //       throw new ArgumentException($"Relay target property value is null.");
   //    }
   //    
   //    return target.Bind((AvaloniaProperty)targetPropertyValue, new Binding(sourcePropertyName, mode)
   //    {
   //       Source = source
   //    });
   // }

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
}