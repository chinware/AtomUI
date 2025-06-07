using AtomUI.Controls.Primitives;
using AtomUI.MotionScene;
using Avalonia;

namespace AtomUI.Controls;

internal class PopupMotionActor : SceneMotionActorControl
{
    private readonly Point _offset;

    public PopupMotionActor(Point offset, MotionGhostControl motionGhost)
        : base(motionGhost)
    {
        _offset     = offset;
    }

    protected override Point CalculateTopLevelGhostPosition()
    {
        return _offset;
    }
}