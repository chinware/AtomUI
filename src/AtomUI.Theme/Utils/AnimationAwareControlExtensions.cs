using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.LogicalTree;

namespace AtomUI.Theme.Utils;

public static class AnimationAwareControlExtensions
{
    public static void BindAnimationProperties(this IAnimationAwareControl animationAwareControl,
                                               AvaloniaProperty isMotionEnabledProperty,
                                               AvaloniaProperty isWaveAnimationEnabledProperty)
    {
        var                  bindTarget          = animationAwareControl.PropertyBindTarget;
        CompositeDisposable? compositeDisposable = null;
        bindTarget.AttachedToLogicalTree += (object? sender, LogicalTreeAttachmentEventArgs args) =>
        {
            if (sender is Control control)
            {
                compositeDisposable = new CompositeDisposable();
                compositeDisposable.Add(TokenResourceBinder.CreateTokenBinding(control, isMotionEnabledProperty, SharedTokenKey.EnableMotion));
                compositeDisposable.Add(TokenResourceBinder.CreateTokenBinding(control, isWaveAnimationEnabledProperty,
                    SharedTokenKey.EnableWaveAnimation));
            }
        };

        bindTarget.DetachedFromLogicalTree += (object? sender, LogicalTreeAttachmentEventArgs args) =>
        {
            compositeDisposable?.Dispose();
            compositeDisposable = null;
        };

        // 如果被强行指定，那么在 resource 中记录下来，这样就屏蔽全局的
        bindTarget.PropertyChanged += HandlePropertyChanged;
    }

    private static void HandlePropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender is Control hostControl)
        {
            var isMotionEnabledChanged = e.Property.Name == AnimationAwareControlProperty.IsMotionEnabledPropertyName;
            var isWaveAnimationEnabledChanged =
                e.Property.Name == AnimationAwareControlProperty.IsWaveAnimationEnabledPropertyName;
            if (isMotionEnabledChanged || isWaveAnimationEnabledChanged)
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
                    TokenResourceKey? resourceKey;
                    if (isMotionEnabledChanged)
                    {
                        resourceKey = SharedTokenKey.EnableMotion;
                    }
                    else
                    {
                        resourceKey = SharedTokenKey.EnableWaveAnimation;
                    }

                    if (resourceDictionary.ContainsKey(resourceKey))
                    {
                        resourceDictionary.Remove(resourceKey);
                    }

                    resourceDictionary.Add(resourceKey, newValue);
                }
            }
        }
    }
}