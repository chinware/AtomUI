using AtomUI.Controls;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Styling;

namespace AtomUI.Theme.Utils;

public static class MotionAwareControlExtensions
{
    public static void ConfigureMotionBindingStyle(this IMotionAwareControl motionAwareControl)
    {
        if (motionAwareControl is StyledElement styledElement)
        {
            var bindingStyle = new Style();
            bindingStyle.Add(MotionAwareControlProperty.IsMotionEnabledProperty, SharedTokenKey.EnableMotion);
            styledElement.Styles.Add(bindingStyle);
        }
    }
}