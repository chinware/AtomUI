using AtomUI.Utils;
using Avalonia;

namespace AtomUI.Controls.DialogPositioning;

internal interface IManagedDialogPositionerDialog
{
    IReadOnlyList<ManagedDialogPositionerScreenInfo> ScreenInfos { get; }
    Rect ParentClientAreaScreenGeometry { get; }
    void Move(Point devicePoint);
}

internal class ManagedDialogPositionerScreenInfo
{
    public Rect Bounds { get; }
    public Rect WorkingArea { get; }

    public ManagedDialogPositionerScreenInfo(Rect bounds, Rect workingArea)
    {
        Bounds      = bounds;
        WorkingArea = workingArea;
    }
}

internal class ManagedDialogPositioner : IDialogPositioner
{
    private readonly IManagedDialogPositionerDialog _dialog;
    
    public ManagedDialogPositioner(IManagedDialogPositionerDialog dialog)
    {
        _dialog = dialog;
    }

    public void Update(DialogPositionerParameters parameters)
    {
        var rect = Calculate(
            parameters.Size,
            new Rect(
                parameters.AnchorRectangle.TopLeft,
                parameters.AnchorRectangle.Size),
            parameters.HorizontalOffset,
            parameters.VerticalOffset,
            parameters.ConstraintAdjustment);
           
        _dialog.Move(rect.Position);
    }
    
    private Rect Calculate(Size translatedSize, 
                           Rect anchorRect,
                           double horizontalOffset,
                           double verticalOffset,
                           DialogPositionerConstraintAdjustment constraintAdjustment)
    {
        var parentGeometry = _dialog.ParentClientAreaScreenGeometry;
        anchorRect = anchorRect.Translate(parentGeometry.TopLeft);
            
        Rect GetBounds()
        {
            var screens = _dialog.ScreenInfos;
                
            var targetScreen =  screens.FirstOrDefault(s => s.Bounds.ContainsExclusive(anchorRect.TopLeft))
                                ?? screens.FirstOrDefault(s => s.Bounds.Intersects(anchorRect))
                                ?? screens.FirstOrDefault(s => s.Bounds.ContainsExclusive(parentGeometry.TopLeft))
                                ?? screens.FirstOrDefault(s => s.Bounds.Intersects(parentGeometry))
                                ?? screens.FirstOrDefault();

            if (targetScreen != null &&
                (targetScreen.WorkingArea.Width == 0 && targetScreen.WorkingArea.Height == 0))
            {
                return targetScreen.Bounds;
            }
                
            return targetScreen?.WorkingArea ?? new Rect(0, 0, double.MaxValue, double.MaxValue);
        }

        var bounds = GetBounds();

        bool FitsInBounds(Rect rc, Direction direction)
        {
            if (direction == Direction.Left && rc.X < bounds.X ||
                direction == Direction.Top && rc.Y < bounds.Y || 
                direction == Direction.Right && rc.Right > bounds.Right ||
                direction == Direction.Bottom && rc.Bottom > bounds.Bottom)
            {
                return false;
            }

            return true;
        }

        static bool IsValid(in Rect rc) => rc.Width > 0 && rc.Height > 0;

        Rect geo = new Rect(new Point(horizontalOffset, verticalOffset), translatedSize);

        // If sliding is allowed, try moving the rect into the bounds
        if (constraintAdjustment.HasAllFlags(DialogPositionerConstraintAdjustment.SlideX))
        {
            geo = geo.WithX(Math.Max(geo.X, bounds.X));
            if (geo.Right > bounds.Right)
            {
                geo = geo.WithX(bounds.Right - geo.Width);
            }
        }
            
        // Resize the rect horizontally if allowed.
        if (constraintAdjustment.HasAllFlags(DialogPositionerConstraintAdjustment.ResizeX))
        {
            var unconstrainedRect = geo;

            if (!FitsInBounds(unconstrainedRect, Direction.Left))
            {
                unconstrainedRect = unconstrainedRect.WithX(bounds.X);
            }

            if (!FitsInBounds(unconstrainedRect, Direction.Right))
            {
                unconstrainedRect = unconstrainedRect.WithWidth(bounds.Width - unconstrainedRect.X);
            }

            if (IsValid(unconstrainedRect))
            {
                geo = unconstrainedRect;
            }
        }

        // If sliding is allowed, try moving the rect into the bounds
        if (constraintAdjustment.HasAllFlags(DialogPositionerConstraintAdjustment.SlideY))
        {
            geo = geo.WithY(Math.Max(geo.Y, bounds.Y));
            if (geo.Bottom > bounds.Bottom)
            {
                geo = geo.WithY(bounds.Bottom - geo.Height);
            }
        }

        // Resize the rect vertically if allowed.
        if (constraintAdjustment.HasAllFlags(DialogPositionerConstraintAdjustment.ResizeY))
        {
            var unconstrainedRect = geo;

            if (!FitsInBounds(unconstrainedRect, Direction.Top))
            {
                unconstrainedRect = unconstrainedRect.WithY(bounds.Y);
            }

            if (!FitsInBounds(unconstrainedRect, Direction.Bottom))
            {
                unconstrainedRect = unconstrainedRect.WithHeight(bounds.Bottom - unconstrainedRect.Y);
            }

            if (IsValid(unconstrainedRect))
            {
                geo = unconstrainedRect;
            }
        }

        return geo;
    }
}