using System.Diagnostics;
using AtomUI.Animations;
using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;

namespace AtomUI.Controls.Utils;

public sealed class TransitionUtils : BaseTransitionUtils
{
    public static ITransition CreateTransition<T>(AvaloniaProperty targetProperty,
                                                  TokenResourceKey? durationResourceKey = null,
                                                  Easing? easing = null)
        where T : TransitionBase, new()
    {
        easing              ??= new LinearEasing();
        durationResourceKey ??= SharedTokenKey.MotionDurationMid;
        var transition = new T
        {
            Property = targetProperty,
            Easing   = easing
        };
        var application = Application.Current;
        Debug.Assert(application != null);
        var themeVariant = application.ActualThemeVariant;
        if (application.TryGetResource(durationResourceKey, themeVariant, out var value))
        {
            if (value is TimeSpan duration)
            {
                transition.Duration = duration;
            }
        }
        return transition;
    }
}