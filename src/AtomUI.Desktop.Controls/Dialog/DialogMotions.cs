using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;

namespace AtomUI.Desktop.Controls;

internal sealed class DialogZoomInMotion : ZoomBigInMotion
{
    private readonly RelativePoint _origin;

    internal DialogZoomInMotion(RelativePoint origin, TimeSpan duration)
        : base(duration, new CircularEaseOut())
    {
        _origin = origin;
    }

    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        RenderTransformOrigin = _origin;
    }
}

internal sealed class DialogZoomOutMotion : ZoomBigOutMotion
{
    private readonly RelativePoint _origin;

    internal DialogZoomOutMotion(RelativePoint origin, TimeSpan duration)
        : base(duration, new CubicEaseIn(), FillMode.Forward)
    {
        _origin = origin;
    }

    protected override void ConfigureTransitions()
    {
        base.ConfigureTransitions();
        RenderTransformOrigin = _origin;
    }
}
