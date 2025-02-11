using Avalonia.Controls;

namespace AtomUI.Controls;

public interface IAnimationAwareControl
{
    bool IsMotionEnabled { get; set; }
    bool IsWaveAnimationEnabled { get; set; }
    Control PropertyBindTarget { get; }
}