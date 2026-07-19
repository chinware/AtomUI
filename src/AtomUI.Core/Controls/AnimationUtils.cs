using System.Diagnostics;
using AtomUI.Animations;
using AtomUI.Theme.Resources;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;

namespace AtomUI.Controls;

public sealed class TransitionUtils : BaseTransitionUtils
{
    public static ITransition CreateTransition<T>(AvaloniaProperty targetProperty,
                                                  SharedTokenKind? durationResourceKey = null,
                                                  Easing? easing = null,
                                                  TimeSpan? delay = null)
        where T : TransitionBase, new()
    {
        easing              ??= new LinearEasing();
        durationResourceKey ??= SharedTokenKind.MotionDurationMid;
        var transition = new T
        {
            Property = targetProperty,
            Easing   = easing,
            Delay    = delay ??  TimeSpan.Zero,
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