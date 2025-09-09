using Avalonia;

namespace AtomUI.Controls.DialogPositioning;

public record CustomDialogPlacement
{
    public DialogHorizontalPlacement HorizontalPlacement { get; set; } =  DialogHorizontalPlacement.Center;
    public DialogVerticalPlacement VerticalPlacement { get; set; } =  DialogVerticalPlacement.Center;
    
    public Size DialogSize { get; }
    public Visual Target { get; }
    public Point Offset { get; set; }
    public DialogPositionerConstraintAdjustment ConstraintAdjustment { get; set; }
    public Rect AnchorRectangle { get; set; }
    
    internal CustomDialogPlacement(Size size, Visual target)
    {
        DialogSize = size;
        Target     = target;
    }
}