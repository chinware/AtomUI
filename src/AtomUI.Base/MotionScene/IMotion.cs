using AtomUI.Animations;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;

namespace AtomUI.MotionScene;

internal enum MotionSpiritType
{
    Animation,
    Transition
}

internal interface IMotion
{
    RelativePoint RenderTransformOrigin { get; }
    IList<Animation> Animations { get; }
    IList<INotifyTransitionCompleted> Transitions { get; }
    TimeSpan Duration { get; }
    Easing Easing { get; }
    FillMode PropertyValueFillMode { get; }
    MotionSpiritType SpiritType { get; set; }

    Task RunAsync(MotionActorControl actor,
                  Action? aboutToStart = null,
                  Action? completedAction = null,
                  CancellationToken cancellationToken = default);
}