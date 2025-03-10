using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.LogicalTree;

namespace AtomUI.Theme.Utils;

public static class MotionAwareControlExtensions
{
    public static void BindMotionProperties(this IMotionAwareControl motionAwareControl)
    {
        var                  bindTarget          = motionAwareControl.PropertyBindTarget;
        bindTarget.AttachedToLogicalTree += (object? sender, LogicalTreeAttachmentEventArgs args) =>
        {
            if (sender is Control control)
            {
                var compositeDisposable = new CompositeDisposable();
                compositeDisposable.Add(TokenResourceBinder.CreateTokenBinding(control, MotionAwareControlProperty.IsMotionEnabledProperty,
                    SharedTokenKey.EnableMotion));
                MotionAwareControlProperty.SetTokenResourceBindingDisposables(control, compositeDisposable);
            }
        };

        bindTarget.DetachedFromLogicalTree += (object? sender, LogicalTreeAttachmentEventArgs args) =>
        {
            if (sender is Control control)
            {
                var compositeDisposable = MotionAwareControlProperty.GetTokenResourceBindingDisposables(control);
                compositeDisposable?.Dispose();
                MotionAwareControlProperty.SetTokenResourceBindingDisposables(control, null);
            }
        };

        // 如果被强行指定，那么在 resource 中记录下来，这样就屏蔽全局的
        bindTarget.PropertyChanged += HandlePropertyChanged;
    }

    private static void HandlePropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender is Control hostControl)
        {
            var isMotionEnabledChanged = e.Property.Name == MotionAwareControlProperty.IsMotionEnabledPropertyName;
            if (isMotionEnabledChanged)
            {
                if (e.Priority == BindingPriority.LocalValue)
                {
                    ResourceDictionary? resourceDictionary;
                    var                 themeVariant = TokenFinderUtils.FindThemeVariant(hostControl);
                    if (!hostControl.Resources.ThemeDictionaries.ContainsKey(themeVariant))
                    {
                        resourceDictionary = new ResourceDictionary();
                        hostControl.Resources.ThemeDictionaries.Add(themeVariant, resourceDictionary);
                    }
                    else
                    {
                        resourceDictionary =
                            hostControl.Resources.ThemeDictionaries[themeVariant] as ResourceDictionary;
                    }

                    Debug.Assert(resourceDictionary != null);

                    var               newValue = e.GetNewValue<bool>();

                    if (resourceDictionary.ContainsKey(SharedTokenKey.EnableMotion))
                    {
                        resourceDictionary.Remove(SharedTokenKey.EnableMotion);
                    }

                    resourceDictionary.Add(SharedTokenKey.EnableMotion, newValue);
                }
            }
        }
    }
}