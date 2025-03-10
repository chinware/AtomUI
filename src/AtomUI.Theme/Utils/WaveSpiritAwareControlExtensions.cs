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

public static class WaveSpiritAwareControlExtensions
{
    public static void BindWaveSpiritProperties(this IWaveSpiritAwareControl waveSpiritAwareControl)
    {
        var bindTarget = waveSpiritAwareControl.PropertyBindTarget;
        bindTarget.AttachedToLogicalTree += (object? sender, LogicalTreeAttachmentEventArgs args) =>
        {
            if (sender is Control control)
            {
                var compositeDisposable = new CompositeDisposable();
                compositeDisposable.Add(TokenResourceBinder.CreateTokenBinding(control,
                    WaveSpiritAwareControlProperty.IsMotionEnabledProperty,
                    SharedTokenKey.EnableMotion));
                compositeDisposable.Add(TokenResourceBinder.CreateTokenBinding(control,
                    WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty,
                    SharedTokenKey.EnableWaveSpirit));
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
            var isMotionEnabledChanged = e.Property.Name == WaveSpiritAwareControlProperty.IsMotionEnabledPropertyName;
            var isWaveSpiritEnabledChanged =
                e.Property.Name == WaveSpiritAwareControlProperty.IsWaveSpiritEnabledPropertyName;
            if (isMotionEnabledChanged || isWaveSpiritEnabledChanged)
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
                        resourceKey = SharedTokenKey.EnableWaveSpirit;
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