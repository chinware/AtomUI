using Avalonia.Controls;

namespace AtomUI.Controls;

public interface IAnimationAwareControl
{
    bool IsMotionEnabled { get; }
    bool IsWaveAnimationEnabled { get; }
    Control PropertyBindTarget { get; }
}