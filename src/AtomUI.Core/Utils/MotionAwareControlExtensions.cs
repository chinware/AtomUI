using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme.Resources;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Utils;

public static class MotionAwareControlExtensions
{
    public static IDisposable? ConfigureMotionBindingStyle(this IMotionAwareControl motionAwareControl)
    {
        if (motionAwareControl is Control control)
        {
            return TokenResourceBinder.CreateControlTokenBinding(
                control,
                MotionAwareControlProperty.IsMotionEnabledProperty,
                SharedTokenKind.EnableMotion);
        }

        return motionAwareControl is StyledElement styledElement
            ? TokenResourceBinder.CreateGlobalTokenBinding(
                styledElement,
                MotionAwareControlProperty.IsMotionEnabledProperty,
                SharedTokenKind.EnableMotion)
            : null;
    }
}
