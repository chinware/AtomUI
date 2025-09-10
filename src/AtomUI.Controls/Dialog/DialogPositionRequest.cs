using AtomUI.Controls.DialogPositioning;
using Avalonia;

namespace AtomUI.Controls;

public class DialogPositionRequest
{
    internal DialogPositionRequest(Visual target,
                                   Dimension horizontalOffset, 
                                   Dimension verticalOffset,
                                   Rect? anchorRect = null,
                                   CustomDialogPlacementCallback? placementCallback = null)
    {
        Target            = target;
        HorizontalOffset  = horizontalOffset;
        VerticalOffset    = verticalOffset;
        AnchorRect        = anchorRect;
        PlacementCallback = placementCallback;
    }
    
    public Visual Target { get; }
    public Dimension HorizontalOffset { get; set; }
    public Dimension VerticalOffset { get; set; }
    public Rect? AnchorRect { get; }
    public DialogPositionerConstraintAdjustment ConstraintAdjustment {get;}
    public CustomDialogPlacementCallback? PlacementCallback { get; }
}