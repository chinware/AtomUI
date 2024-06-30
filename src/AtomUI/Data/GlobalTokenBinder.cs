using AtomUI.TokenSystem;
using Avalonia;
using Avalonia.Data;

namespace AtomUI.Data;

public class GlobalTokenBinder : IDisposable
{
   protected List<BindingInfo> _bindings;

   public GlobalTokenBinder()
   {
      _bindings = new List<BindingInfo>();
   }

   public virtual void Dispose()
   {
      foreach (var binding in _bindings) {
         binding.Binding?.Dispose();
      }

      _bindings.Clear();
   }

   public void AddGlobalBinding(AvaloniaObject target,
                                AvaloniaProperty targetProperty,
                                string resourceKey,
                                string? controlTokenId = null,
                                BindingPriority priority = BindingPriority.Style,
                                Func<IObservable<object?>, IObservable<object?>>? observableConfigure = null,
                                string? bindingName = null)
   {
      var activeTheme = ThemeManager.Current.ActivatedTheme;
      if (activeTheme is null) {
         throw new ArgumentException("There are currently no active theme.");
      }

      if (controlTokenId != null) {
         // 探测绑定目标
         var controlToken = activeTheme.GetControlToken(controlTokenId);
         if (controlToken is null) {
            throw new ArgumentException(
               $"Control token {controlTokenId} for token provider {nameof(target)} is not exist.");
         }
         // 全局也是有 control 的 token 存在的，只不过直接读取的全局的值，有些绑定对象可能不是 control
         if (controlToken.IsCustomTokenConfig) {
            // 自定义某些 token 值，有可能全局的 Token 也会被重定义
            if (controlToken.HasToken(resourceKey) || controlToken.CustomTokens.Contains(resourceKey)) {
               resourceKey = $"{controlTokenId}.{resourceKey}";
            }
         } else {
            if (controlToken.HasToken(resourceKey)) {
               resourceKey = $"{controlTokenId}.{resourceKey}";
            }
         }
      }

      bindingName ??= $"{target.GetType().Name}-{target.GetHashCode()}-{targetProperty.Name}-{resourceKey}";

      var bindingInfo = new BindingInfo
      {
         Target = target,
         TargetProperty = targetProperty,
         ResourceKey = resourceKey,
         Priority = priority,
         BindingName = bindingName
      };

      var bindingObservable = ThemeResourceUtils.GetGlobalTokenResourceObservable(resourceKey);
      if (observableConfigure is not null) {
         bindingObservable = observableConfigure(bindingObservable);
      }

      var binding = target.Bind(targetProperty, bindingObservable, priority);
      bindingInfo.Binding = binding;

      if (HasBinding(bindingName, priority)) {
         ReleaseBinding(bindingName, priority);
      }

      _bindings.Add(bindingInfo);
   }

   public void ReleaseBinding(string bindingName, BindingPriority priority)
   {
      var bindingInfo = _bindings.Find(binding => binding.BindingName == bindingName && binding.Priority == priority);
      if (bindingInfo is not null) {
         bindingInfo.Binding?.Dispose();
         _bindings.Remove(bindingInfo);
      }
   }

   public void ReleaseTriggerBinding(string bindingName)
   {
      ReleaseBinding(bindingName, BindingPriority.StyleTrigger);
   }

   public void ReleaseTriggerBindings(AvaloniaObject target)
   {
      foreach (var bindingInfo in _bindings) {
         if (target == bindingInfo.Target && bindingInfo.Priority == BindingPriority.StyleTrigger) {
            bindingInfo.Binding?.Dispose();
         }
      }

      _bindings.RemoveAll(binding => binding.Target == target && binding.Priority == BindingPriority.StyleTrigger);
   }

   public void ReleaseBindings(AvaloniaObject target)
   {
      foreach (var bindingInfo in _bindings) {
         if (target == bindingInfo.Target) {
            bindingInfo.Binding?.Dispose();
         }
      }

      _bindings.RemoveAll(binding => binding.Target == target);
   }

   public bool HasBinding(string bindingName, BindingPriority priority)
   {
      return _bindings.Any(binding => binding.BindingName == bindingName && binding.Priority == priority);
   }

   protected class BindingInfo
   {
      public AvaloniaObject Target { get; set; } = default!;
      public AvaloniaProperty TargetProperty { get; set; } = default!;
      public string ResourceKey { get; set; } = string.Empty;
      public string? BindingName { get; set; }
      public BindingPriority Priority { get; set; }
      public IDisposable? Binding { get; set; }
   }
}