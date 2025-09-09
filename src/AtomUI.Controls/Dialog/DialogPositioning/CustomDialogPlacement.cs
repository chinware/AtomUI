using Avalonia;

namespace AtomUI.Controls.DialogPositioning;

public record CustomDialogPlacement
{
    public DialogHorizontalAnchor HorizontalAnchor { get; set; }
    public DialogVerticalAnchor VerticalAnchor { get; set; }
    public Dimension HorizontalOffset { get; set; }
    public Dimension VerticalOffset { get; set; }
    
    public Size DialogSize { get; }
    public Visual Target { get; }
    public DialogPositionerConstraintAdjustment ConstraintAdjustment { get; set; }
    public Rect AnchorRectangle { get; set; }
    
    internal CustomDialogPlacement(Size size, Visual target)
    {
        DialogSize = size;
        Target     = target;
    }
}