using AtomUI.Controls.DialogPositioning;
using Avalonia;

namespace AtomUI.Controls;

public class DialogPositionRequest
{
    internal DialogPositionRequest(Visual target,
                                   DialogHorizontalPlacement horizontalPlacement, 
                                   DialogVerticalPlacement verticalPlacement,
                                   Point offset,
                                   Rect? anchorRect = null,
                                   CustomDialogPlacementCallback? placementCallback = null)
    {
        Target              = target;
        HorizontalPlacement = horizontalPlacement;
        VerticalPlacement   = verticalPlacement;
        Offset              = offset;
    }
    
    public Visual Target { get; }
    public DialogHorizontalPlacement HorizontalPlacement { get; set; }
    public DialogVerticalPlacement VerticalPlacement { get; set; }
    public Point Offset { get; }
    public Rect? AnchorRect { get; }
    public DialogPositionerConstraintAdjustment ConstraintAdjustment {get;}
    public CustomDialogPlacementCallback? PlacementCallback { get; }
}