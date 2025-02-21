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