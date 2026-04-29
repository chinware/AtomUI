namespace AtomUI.Controls;

public enum ProgressStatus
{
    Normal,
    Success,
    Exception,
    Active
}

public enum DashboardGapPosition
{
    Left,
    Top,
    Right,
    Bottom
}

public record struct PercentPosition
{
    public PercentPosition() {}
    public bool IsInner { get; set; } = false;
    public LinePercentAlignment Alignment { get; set; } = LinePercentAlignment.End;
}

public enum LinePercentAlignment
{
    Start,
    Center,
    End
}