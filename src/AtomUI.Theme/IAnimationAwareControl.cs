using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

public interface IAnimationAwareControl
{
    bool IsMotionEnabled { get; }
    bool IsWaveAnimationEnabled { get; }
    Control PropertyBindTarget { get; }
}

public abstract class AnimationAwareControlProperty : AvaloniaObject
{
    public const string IsMotionEnabledPropertyName = "IsMotionEnabled";
    public const string IsWaveAnimationEnabledPropertyName = "IsWaveAnimationEnabled";
    public const string TokenResourceBindingDisposablePropertyName = "TokenResourceBindingDisposable";

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AvaloniaProperty.Register<AnimationAwareControlProperty, bool>(IsMotionEnabledPropertyName);

    public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = AvaloniaProperty.Register<AnimationAwareControlProperty, bool>(IsWaveAnimationEnabledPropertyName);

    internal static readonly AttachedProperty<CompositeDisposable?> TokenResourceBindingDisposablesProperty =
        AvaloniaProperty.RegisterAttached<AnimationAwareControlProperty, Control, CompositeDisposable?>(
            TokenResourceBindingDisposablePropertyName);
    
    internal static CompositeDisposable? GetTokenResourceBindingDisposables(StyledElement element)
    {
        Debug.Assert(element is IAnimationAwareControl);
        return element.GetValue(TokenResourceBindingDisposablesProperty);
    }

    internal static void SetTokenResourceBindingDisposables(StyledElement element, CompositeDisposable? compositeDisposable)
    {
        Debug.Assert(element is IAnimationAwareControl);
        element.SetValue(TokenResourceBindingDisposablesProperty, compositeDisposable);
    }
    
    internal static void AddTokenResourceBindingDisposable(StyledElement element, IDisposable disposable)
    {
        Debug.Assert(element is ITokenResourceConsumer);
        var compositeDisposable = element.GetValue(TokenResourceBindingDisposablesProperty);
        if (compositeDisposable == null)
        {
            compositeDisposable = new CompositeDisposable();
            element.SetValue(TokenResourceBindingDisposablesProperty, compositeDisposable);
        }
        compositeDisposable.Add(disposable);
    }
}