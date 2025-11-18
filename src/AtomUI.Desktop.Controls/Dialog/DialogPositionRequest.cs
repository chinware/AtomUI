using AtomUI.Desktop.Controls.DialogPositioning;
using Avalonia;

namespace AtomUI.Desktop.Controls;

public class DialogPositionRequest
{
    internal DialogPositionRequest(Visual target,
                                   double horizontalOffset, 
                                   double verticalOffset,
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
    public double HorizontalOffset { get; set; }
    public double VerticalOffset { get; set; }
    public Rect? AnchorRect { get; }
    public DialogPositionerConstraintAdjustment ConstraintAdjustment {get;}
    public CustomDialogPlacementCallback? PlacementCallback { get; }
}