using Avalonia;

namespace AtomUI.Controls.DialogPositioning;

public record struct DialogPositionerParameters
{
    public Rect AnchorRectangle { get; set; }
    public DialogHorizontalAnchor HorizontalAnchor { get; set; }
    public DialogVerticalAnchor VerticalAnchor { get; set; }
    public Dimension HorizontalOffset { get; set; }
    public Dimension VerticalOffset { get; set; }
    public Size Size { get; set; }
    public DialogPositionerConstraintAdjustment ConstraintAdjustment { get; set; }
}

public interface IDialogPositioner
{
    void Update(DialogPositionerParameters parameters);
}

[Flags]
public enum DialogPositionerConstraintAdjustment
{
    /// <summary>
    /// Don't alter the surface position even if it is constrained on some
    /// axis, for example partially outside the edge of an output.
    /// </summary>
    None = 0,

    /// <summary>
    /// Slide the surface along the x axis until it is no longer constrained.
    /// </summary>
    /// <remarks>
    /// First try to slide towards the direction of the gravity on the x axis until either the
    /// edge in the opposite direction of the gravity is unconstrained or the edge in the
    /// direction of the gravity is constrained.
    ///
    /// Then try to slide towards the opposite direction of the gravity on the x axis until
    /// either the edge in the direction of the gravity is unconstrained or the edge in the
    /// opposite direction of the gravity is constrained.
    /// </remarks>
    SlideX = 1,

    /// <summary>
    /// Slide the surface along the y axis until it is no longer constrained.
    /// </summary>
    /// <remarks>
    /// First try to slide towards the direction of the gravity on the y axis until either the
    /// edge in the opposite direction of the gravity is unconstrained or the edge in the
    /// direction of the gravity is constrained.
    /// 
    /// Then try to slide towards the opposite direction of the gravity on the y axis until
    /// either the edge in the direction of the gravity is unconstrained or the edge in the
    /// opposite direction of the gravity is constrained.
    /// </remarks>
    SlideY = 2,

    /// <summary>
    /// Horizontally resize the surface
    /// </summary>
    /// <remarks>
    /// Resize the surface horizontally so that it is completely unconstrained.
    /// </remarks>
    ResizeX = 16,

    /// <summary>
    /// Vertically resize the surface
    /// </summary>
    /// <remarks>
    /// Resize the surface vertically so that it is completely unconstrained.
    /// </remarks>
    ResizeY = 32,

    All = SlideX|SlideY|ResizeX|ResizeY
}

internal static class DialogPositionerExtensions
{
    public static void Update(
        this IDialogPositioner positioner,
        DialogPositionRequest positionRequest,
        Size dialogSize)
    {
        if (dialogSize == default)
        {
            return;
        }

        var parameters = BuildParameters(positionRequest, dialogSize);
        positioner.Update(parameters);
    }

    private static DialogPositionerParameters BuildParameters(
        DialogPositionRequest positionRequest,
        Size dialogSize)
    {
        DialogPositionerParameters positionerParameters = default;
        positionerParameters.HorizontalAnchor     = positionRequest.HorizontalAnchor;
        positionerParameters.VerticalAnchor       = positionRequest.VerticalAnchor;
        positionerParameters.HorizontalOffset     = positionRequest.HorizontalOffset;
        positionerParameters.VerticalOffset       = positionRequest.VerticalOffset;
        positionerParameters.Size                 = dialogSize;
        positionerParameters.ConstraintAdjustment = positionRequest.ConstraintAdjustment;

        if (positionRequest.PlacementCallback != null && 
            (positionRequest.HorizontalAnchor == DialogHorizontalAnchor.Custom ||
             positionRequest.VerticalAnchor == DialogVerticalAnchor.Custom))
        {
            var customPlacementParameters = new CustomDialogPlacement(
                dialogSize,
                positionRequest.Target)
            {
                HorizontalAnchor     = positionerParameters.HorizontalAnchor,
                VerticalAnchor       = positionerParameters.VerticalAnchor,
                HorizontalOffset     = positionerParameters.HorizontalOffset,
                VerticalOffset       = positionerParameters.VerticalOffset,
                ConstraintAdjustment = positionerParameters.ConstraintAdjustment,
            };

            positionRequest.PlacementCallback.Invoke(customPlacementParameters);
            positionerParameters.HorizontalAnchor     = positionerParameters.HorizontalAnchor;
            positionerParameters.VerticalAnchor       = positionerParameters.VerticalAnchor;
            positionerParameters.HorizontalOffset     = customPlacementParameters.HorizontalOffset;
            positionerParameters.VerticalOffset       = customPlacementParameters.VerticalOffset;
            positionerParameters.ConstraintAdjustment = customPlacementParameters.ConstraintAdjustment;
        }
        return positionerParameters;
    }
}