using Avalonia;

namespace AtomUI.Controls.DialogPositioning;

public record CustomDialogPlacement
{
    public DialogHorizontalPlacement HorizontalPlacement { get; set; } =  DialogHorizontalPlacement.Center;
    public DialogVerticalPlacement PlacementAnchor { get; set; } =  DialogVerticalPlacement.Center;
    
    public Size DialogSize { get; }
    public Point Offset { get; set; }
    
    internal CustomDialogPlacement(Size size)
    {
        DialogSize = size;
    }
}