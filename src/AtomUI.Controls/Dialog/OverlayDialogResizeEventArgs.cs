namespace AtomUI.Controls;

internal class OverlayDialogResizeEventArgs: EventArgs
{
    public ResizeHandleLocation Location { get; set; }
    public double DeltaOffsetX { get; set; }
    public double DeltaOffsetY { get; set; }

    public OverlayDialogResizeEventArgs(ResizeHandleLocation location,  double deltaOffsetX, double deltaOffsetY)
    {
        Location = location;
        DeltaOffsetX = deltaOffsetX;
        DeltaOffsetY = deltaOffsetY;
    }
}
