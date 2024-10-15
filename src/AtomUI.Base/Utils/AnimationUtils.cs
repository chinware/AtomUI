﻿using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;

namespace AtomUI.Utils;

public class AnimationUtils
{
    protected AnimationUtils() {}

    public static ITransition CreateTransition<T>(AvaloniaProperty targetProperty,
                                                  TimeSpan? duration = null,
                                                  Easing? easing = null)
        where T : TransitionBase, new()
    {
        easing ??= new LinearEasing();
        duration ??= TimeSpan.FromMilliseconds(300);
        var transition = new T
        {
            Property = targetProperty,
            Easing   = easing,
            Duration = duration.Value
        };
        return transition;
    }
}