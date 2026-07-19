using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme.Resources;
using Avalonia;

namespace AtomUI.Utils;

public static class MotionAwareControlExtensions
{
    public static void ConfigureMotionBindingStyle(this IMotionAwareControl motionAwareControl)
    {
        if (motionAwareControl is StyledElement styledElement)
        {
            TokenResourceBinder.CreateTokenBinding(styledElement, MotionAwareControlProperty.IsMotionEnabledProperty, SharedTokenKind.EnableMotion);
        }
    }
}