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

   public void AddGlobalBinding(AvaloniaProperty targetProperty,
                                string resourceKey,
                                BindingPriority priority = BindingPriority.Style,
                                Func<IObservable<object?>, IObservable<object?>>? observableConfigure = null,
                                string? bindingName = null) { }

   public void AddGlobalBinding(AvaloniaObject target,
                                AvaloniaProperty targetProperty,
                                string resourceKey,
                                BindingPriority priority = BindingPriority.Style,
                                Func<IObservable<object?>, IObservable<object?>>? observableConfigure = null,
                                string? bindingName = null) { }
   
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