namespace AtomUI.Desktop.Controls;

/// <summary>
/// Cached layout information for a single element to avoid redundant calculations
/// </summary>
internal readonly struct ElementLayoutCache
{
    public AxisCoordinate DesiredAxisCoord { get; }
    public int FlexValue { get; }
    public bool IsFlexItem { get; }

    public ElementLayoutCache(AxisCoordinate desiredAxisCoord, int flexValue)
    {
        DesiredAxisCoord = desiredAxisCoord;
        FlexValue        = flexValue;
        IsFlexItem       = flexValue > 0;
    }
}
