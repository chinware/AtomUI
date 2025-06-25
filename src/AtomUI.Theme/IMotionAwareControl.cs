using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

public interface IMotionAwareControl
{
    bool IsMotionEnabled { get; }
    Control PropertyBindTarget { get; }
}

public abstract class MotionAwareControlProperty : AvaloniaObject
{
    public const string IsMotionEnabledPropertyName = "IsMotionEnabled";
    public const string MotionDurationPropertyName = "MotionDuration";
    public const string TokenResourceBindingDisposablePropertyName = "TokenResourceBindingDisposable";

    public static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        AvaloniaProperty.Register<MotionAwareControlProperty, bool>(IsMotionEnabledPropertyName);
    
    public static readonly StyledProperty<TimeSpan> MotionDurationProperty = 
        AvaloniaProperty.Register<MotionAwareControlProperty, TimeSpan>(MotionDurationPropertyName, TimeSpan.FromMilliseconds(200));

    internal static readonly AttachedProperty<CompositeDisposable?> TokenResourceBindingDisposablesProperty =
        AvaloniaProperty.RegisterAttached<MotionAwareControlProperty, Control, CompositeDisposable?>(
            TokenResourceBindingDisposablePropertyName);
    
    internal static CompositeDisposable? GetTokenResourceBindingDisposables(StyledElement element)
    {
        Debug.Assert(element is IMotionAwareControl);
        return element.GetValue(TokenResourceBindingDisposablesProperty);
    }

    internal static void SetTokenResourceBindingDisposables(StyledElement element, CompositeDisposable? compositeDisposable)
    {
        Debug.Assert(element is IMotionAwareControl);
        element.SetValue(TokenResourceBindingDisposablesProperty, compositeDisposable);
    }
    
    internal static void AddTokenResourceBindingDisposable(StyledElement element, IDisposable disposable)
    {
        Debug.Assert(element is IResourceBindingManager);
        var compositeDisposable = element.GetValue(TokenResourceBindingDisposablesProperty);
        if (compositeDisposable == null)
        {
            compositeDisposable = new CompositeDisposable();
            element.SetValue(TokenResourceBindingDisposablesProperty, compositeDisposable);
        }
        compositeDisposable.Add(disposable);
    }
}