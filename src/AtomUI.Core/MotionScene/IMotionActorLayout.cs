using Avalonia;

namespace AtomUI.MotionScene;

/// <summary>
/// Supplies layout for an actor while a motion owns its content geometry.
/// The motion installs and removes this strategy for its active execution.
/// </summary>
internal interface IMotionActorLayout
{
    Size Measure(Size availableSize);
    Rect ConstrainArrangeRect(Rect finalRect);
    Size Arrange(Size finalSize);
}
