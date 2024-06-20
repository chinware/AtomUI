using AtomUI.TokenSystem;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace AtomUI.Data;

public class TokenResourceBinder : IDisposable
{
   /// <summary>
   /// 用于管理生命周期
   /// </summary>
   private Control _hostControl;

   private List<BindingInfo> _bindings;

   public TokenResourceBinder(Control hostControl)
   {
      _hostControl = hostControl;
      _bindings = new List<BindingInfo>();
      _hostControl.AttachedToLogicalTree += HandleAttachedToLogicalTree;
      _hostControl.DetachedFromLogicalTree += HandleDetachedFromLogicalTree;
   }

   public void AddBinding(AvaloniaProperty targetProperty,
                          string resourceKey,
                          BindingPriority priority = BindingPriority.Style,
                          Func<IObservable<object?>, IObservable<object?>>? observableConfigure = null,
                          string? bindingName = null)
   {
      AddBinding(_hostControl, targetProperty, resourceKey, priority, observableConfigure, bindingName);
   }

   public void AddBinding(AvaloniaObject target,
                          AvaloniaProperty targetProperty,
                          string resourceKey,
                          BindingPriority priority = BindingPriority.Style,
                          Func<IObservable<object?>, IObservable<object?>>? observableConfigure = null,
                          string? bindingName = null)
   {
      var visualParent = _hostControl.GetVisualParent();
      var activeTheme = ThemeManager.Current.ActivatedTheme!;
      // 探测绑定目标
      var tokenIdProvider = _hostControl as ITokenIdProvider;
      if (tokenIdProvider is null) {
         throw new ArgumentException("Add a binding to Control design token, " +
                                     "but host control is not a ITokenIdProvider");
      }

      var controlToken = activeTheme.GetControlToken(tokenIdProvider.TokenId);
      if (controlToken is null) {
         throw new ArgumentException($"Control token {tokenIdProvider.TokenId} for Control token id provider {nameof(target)} is not exist.");
      }
      
      if (controlToken.IsCustomTokenConfig) {
         // 自定义某些 token 值，有可能全局的 Token 也会被重定义
         if (controlToken.HasToken(resourceKey) || controlToken.CustomTokens.Contains(resourceKey)) {
            resourceKey = $"{tokenIdProvider.TokenId}.{resourceKey}";
         }
      } else {
         if (controlToken.HasToken(resourceKey)) {
            resourceKey = $"{tokenIdProvider.TokenId}.{resourceKey}";
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
      
      if (visualParent is not null) {
         var bindingObservable = _hostControl.GetResourceObservable(resourceKey);
         if (observableConfigure is not null) {
            bindingObservable = observableConfigure(bindingObservable);
         }
         IDisposable binding = target.Bind(targetProperty,
            bindingObservable,
            priority);
         bindingInfo.Binding = binding;
      }

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

   public void ReleaseTriggerBindings(Control target)
   {
      foreach (var bindingInfo in _bindings) {
         if (target == bindingInfo.Target && bindingInfo.Priority == BindingPriority.StyleTrigger) {
            bindingInfo.Binding?.Dispose();
         }
      }

      _bindings.RemoveAll(binding => binding.Target == target && binding.Priority == BindingPriority.StyleTrigger);
   }

   public void ReleaseBindings(Control target)
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

   public void Dispose()
   {
      foreach (var binding in _bindings) {
         binding.Binding?.Dispose();
      }

      _hostControl.AttachedToLogicalTree -= HandleAttachedToLogicalTree;
      _hostControl.DetachedFromLogicalTree -= HandleDetachedFromLogicalTree;
      _bindings.Clear();
   }

   private void HandleAttachedToLogicalTree(object? sender, LogicalTreeAttachmentEventArgs eventArgs)
   {
      // TODO 现在的架构，第一次初始化可能会多循环一次
      // 优先级高的后建立，不知道合适不
      var bindingInfos = _bindings.OrderByDescending(binding => binding.Priority);
      foreach (var bindingInfo in bindingInfos) {
         if (bindingInfo.Binding is null) {
            IDisposable binding = bindingInfo.Target.Bind(bindingInfo.TargetProperty,
               _hostControl.GetResourceObservable(bindingInfo.ResourceKey),
               bindingInfo.Priority);
            bindingInfo.Binding = binding;
         }
      }
   }

   private void HandleDetachedFromLogicalTree(object? sender, LogicalTreeAttachmentEventArgs eventArgs)
   {
      foreach (var binding in _bindings) {
         binding.Binding?.Dispose();
         binding.Binding = null;
      }
   }

   private class BindingInfo
   {
      public AvaloniaObject Target { get; set; } = default!;
      public AvaloniaProperty TargetProperty { get; set; } = default!;
      public string ResourceKey { get; set; } = string.Empty;
      public string? BindingName { get; set; }
      public BindingPriority Priority { get; set; }
      public IDisposable? Binding { get; set; }
   }
}