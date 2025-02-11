using AtomUI.Controls;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace AtomUI.Theme.Utils;

public static class AnimationAwareControlExtensions
{
    public static void BindAnimationProperties(this IAnimationAwareControl animationAwareControl, 
                                               AvaloniaProperty isMotionEnabledProperty,
                                               AvaloniaProperty isWaveAnimationEnabledProperty)
    {
        var bindTarget = animationAwareControl.PropertyBindTarget;
        bindTarget.AttachedToLogicalTree += (object? sender, LogicalTreeAttachmentEventArgs args) =>
        {
            if (sender is Control control)
            {
                TokenResourceBinder.CreateTokenBinding(control, isMotionEnabledProperty, SharedTokenKey.EnableMotion);
                TokenResourceBinder.CreateTokenBinding(control, isWaveAnimationEnabledProperty, SharedTokenKey.EnableWaveAnimation);
            }
        };
    }
}