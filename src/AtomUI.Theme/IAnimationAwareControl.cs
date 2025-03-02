using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

public interface IAnimationAwareControl
{
    bool IsMotionEnabled { get; }
    bool IsWaveAnimationEnabled { get; }
    Control PropertyBindTarget { get; }
}

public abstract class AnimationAwareControlProperty : AvaloniaObject
{
    public const string IsMotionEnabledPropertyName = "IsMotionEnabled";
    public const string IsWaveAnimationEnabledPropertyName = "IsWaveAnimationEnabled";
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AvaloniaProperty.Register<AnimationAwareControlProperty, bool>(IsMotionEnabledPropertyName);

    public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = AvaloniaProperty.Register<AnimationAwareControlProperty, bool>(IsWaveAnimationEnabledPropertyName);
}
