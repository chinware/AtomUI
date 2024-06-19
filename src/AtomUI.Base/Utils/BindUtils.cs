using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace AtomUI.Utils;

public static class BindUtils
{
   public static IDisposable RelayBind(Control source, string sourcePropertyName, Control target,
                                       string? targetPropertyName = null,
                                       BindingMode mode = BindingMode.OneWay)
   {
      // TODO 看看这里的 sourcePropertyName 能不能优化掉
      if (targetPropertyName == null) {
         targetPropertyName = sourcePropertyName;
      }

      Type targetType = target.GetType();
      var targetUnderlyingPropertyName = $"{targetPropertyName}Property";
      var targetUnderlyingProperty = targetType.GetField(targetUnderlyingPropertyName,
                                                         BindingFlags.Public | BindingFlags.Static |
                                                         BindingFlags.FlattenHierarchy);

      if (targetUnderlyingProperty is null) {
         throw new ArgumentException($"Relay target property {targetUnderlyingPropertyName} is not exist.");
      }

      Type sourceType = source.GetType();
      var sourceUnderlyingProperty = sourceType.GetProperty(sourcePropertyName,
                                                            BindingFlags.Public | BindingFlags.NonPublic |
                                                            BindingFlags.Instance |
                                                            BindingFlags.FlattenHierarchy); // 这里不需要加 Property 后缀

      if (sourceUnderlyingProperty == null) {
         throw new ArgumentException($"Relay source property {sourcePropertyName} is not exist.");
      }

      var targetPropertyValue = targetUnderlyingProperty.GetValue(null);
      if (targetPropertyValue is null) {
         throw new ArgumentException($"Relay target property value is null.");
      }

      return target.Bind((AvaloniaProperty)targetPropertyValue, new Binding(sourcePropertyName, mode)
      {
         Source = source
      });
   }
}