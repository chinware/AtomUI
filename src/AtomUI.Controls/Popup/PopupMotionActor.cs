using AtomUI.Controls.Primitives;
using AtomUI.Media;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class PopupMotionActor : SceneMotionActorControl
{
    private readonly BoxShadows _boxShadows;
    private readonly Point _offset;
    private readonly double _scaling;

    public PopupMotionActor(BoxShadows boxShadows,
                            Point offset,
                            double scaling,
                            Control motionTarget)
        : base(motionTarget)
    {
        _offset     = offset;
        _scaling    = scaling;
        _boxShadows = boxShadows;
    }

    protected override Point CalculateTopLevelGhostPosition()
    {
        var boxShadowsThickness = _boxShadows.Thickness();
        var winPos              = _offset; // TODO 可能需要乘以 scaling
        var scaledThickness     = boxShadowsThickness * _scaling;
        return new Point(winPos.X - scaledThickness.Left, winPos.Y - scaledThickness.Top);
    }

    internal override void BuildGhost()
    {
        if (_ghost is null)
        {
            _ghost = new MotionGhostControl(MotionTarget, _boxShadows)
            {
                Shadows = _boxShadows
            };
            Child = _ghost;
        }
    }
}