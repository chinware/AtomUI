using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;

namespace AtomUI.MotionScene;

internal interface IMotion
{
    public RelativePoint RenderTransformOrigin { get; }
    public IList<Animation> Animations { get; }
    public TimeSpan Duration { get; }
    public Easing Easing { get; }
    public FillMode PropertyValueFillMode { get; }

    public Task RunAsync(MotionActorControl actor,
                         Action? aboutToStart = null,
                         Action? completedAction = null,
                         CancellationToken cancellationToken = default);
}