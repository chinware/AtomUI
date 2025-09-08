using Avalonia;

namespace AtomUI.Controls.DialogPositioning;

public interface IManagedDialogPositionerDialog
{
    IReadOnlyList<ManagedDialogPositionerScreenInfo> Screens { get; }
    Rect ParentClientAreaScreenGeometry { get; }
    double Scaling { get; }
    void MoveAndResize(Point devicePoint, Size virtualSize);
}

public class ManagedDialogPositionerScreenInfo
{
    public Rect Bounds { get; }
    public Rect WorkingArea { get; }

    public ManagedDialogPositionerScreenInfo(Rect bounds, Rect workingArea)
    {
        Bounds      = bounds;
        WorkingArea = workingArea;
    }
}

public class ManagedDialogPositioner : IDialogPositioner
{
    private readonly IManagedDialogPositionerDialog _dialog;
    
    public ManagedDialogPositioner(IManagedDialogPositionerDialog dialog)
    {
        _dialog = dialog;
    }

    public void Update(PopupPositionerParameters parameters)
    {
    }
}