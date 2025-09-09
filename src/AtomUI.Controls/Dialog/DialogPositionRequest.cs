using AtomUI.Controls.DialogPositioning;
using Avalonia;

namespace AtomUI.Controls;

public class DialogPositionRequest
{
    internal DialogPositionRequest(Visual target,
                                   DialogHorizontalAnchor  horizontalAnchor,
                                   DialogVerticalAnchor  verticalAnchor,
                                   Dimension horizontalOffset, 
                                   Dimension verticalOffset,
                                   Rect? anchorRect = null,
                                   CustomDialogPlacementCallback? placementCallback = null)
    {
        Target            = target;
        HorizontalAnchor  = horizontalAnchor;
        VerticalAnchor    = verticalAnchor;
        HorizontalOffset  = horizontalOffset;
        VerticalOffset    = verticalOffset;
        AnchorRect        = anchorRect;
        PlacementCallback = placementCallback;
    }
    
    public Visual Target { get; }
    public DialogHorizontalAnchor HorizontalAnchor { get; }
    public DialogVerticalAnchor VerticalAnchor { get; }
    public Dimension HorizontalOffset { get; set; }
    public Dimension VerticalOffset { get; set; }
    public Rect? AnchorRect { get; }
    public DialogPositionerConstraintAdjustment ConstraintAdjustment {get;}
    public CustomDialogPlacementCallback? PlacementCallback { get; }
}