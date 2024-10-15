using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;

namespace AtomUI.Controls.Utils;

public sealed class AnimationUtils : AtomUI.Utils.AnimationUtils
{
    public static ITransition CreateTransition<T>(AvaloniaProperty targetProperty,
                                                  TokenResourceKey? durationResourceKey = null,
                                                  Easing? easing = null)
        where T : TransitionBase, new()
    {
        easing              ??= new LinearEasing();
        durationResourceKey ??= GlobalTokenResourceKey.MotionDurationMid;
        var transition = new T
        {
            Property = targetProperty,
            Easing   = easing
        };
        var application = Application.Current;
        if (application is not null)
        {
            transition.Bind(TransitionBase.DurationProperty, application.GetResourceObservable(durationResourceKey));
        }
    
        return transition;
    }
}