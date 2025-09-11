using Avalonia;
using Avalonia.Platform;

namespace AtomUI.Controls.DialogPositioning;

public class ManagedDialogPositionerDialogImplHelper : IManagedDialogPositionerDialog
{
    private readonly ITopLevelImpl _parent;
    public delegate void MoveResizeDelegate(PixelPoint position, Size size, double scaling);
    private readonly MoveResizeDelegate _moveResize;

    public ManagedDialogPositionerDialogImplHelper(ITopLevelImpl parent, MoveResizeDelegate moveResize)
    {
        _parent     = parent;
        _moveResize = moveResize;
    }
    
    public IReadOnlyList<ManagedDialogPositionerScreenInfo> Screens
    {
        get
        {
            if (_parent.TryGetFeature<IScreenImpl>() is not { } screenImpl)
            {
                return [];
            }

            return screenImpl.AllScreens
                             .Select(s => new ManagedDialogPositionerScreenInfo(s.Bounds.ToRect(1), s.WorkingArea.ToRect(1)))
                             .ToArray();
        }
    }
    
    public Rect ParentClientAreaScreenGeometry
    {
        get
        {
            // Popup positioner operates with abstract coordinates, but in our case they are pixel ones
            var point = _parent.PointToScreen(default);
            var size  = _parent.ClientSize * Scaling;
            return new Rect(point.X, point.Y, size.Width, size.Height);
        }
    }

    public Rect ClientAreaScreenGeometry
    {
        get
        {
            var targetScreen = Screens.FirstOrDefault(s => s.Bounds.ContainsExclusive(ParentClientAreaScreenGeometry.TopLeft))
                               ?? Screens.FirstOrDefault(s => s.Bounds.Intersects(ParentClientAreaScreenGeometry))
                               ?? Screens.FirstOrDefault();

            if (targetScreen != null &&
                (targetScreen.WorkingArea.Width == 0 && targetScreen.WorkingArea.Height == 0))
            {
                return targetScreen.Bounds;
            }
                
            return targetScreen?.WorkingArea
                   ?? new Rect(0, 0, double.MaxValue, double.MaxValue);
        }
    }
    
    public void MoveAndResize(Point devicePoint, Size virtualSize)
    {
        _moveResize(new PixelPoint((int)devicePoint.X, (int)devicePoint.Y), virtualSize, _parent.RenderScaling);
    }

    public virtual double Scaling => _parent.DesktopScaling;
}