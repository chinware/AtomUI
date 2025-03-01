using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

public interface IAnimationAwareControl
{
    public const string IsMotionEnabledPropertyName = "IsMotionEnabled";
    public const string IsWaveAnimationEnabledPropertyName = "IsWaveAnimationEnabled";

    bool IsMotionEnabled { get; }
    bool IsWaveAnimationEnabled { get; }
    Control PropertyBindTarget { get; }
}

public abstract class AnimationAwareControlProperty : AvaloniaObject
{
    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AvaloniaProperty.Register<AnimationAwareControlProperty, bool>(IAnimationAwareControl
            .IsMotionEnabledPropertyName);

    public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = AvaloniaProperty.Register<AnimationAwareControlProperty, bool>(IAnimationAwareControl
            .IsWaveAnimationEnabledPropertyName);
}