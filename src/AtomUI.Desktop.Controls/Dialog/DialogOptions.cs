using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

public record DialogOptions
{
    public string? Title { get; init; }
    public PathIcon? TitleIcon { get; init; }
    public bool IsResizable { get; init; }
    public bool IsClosable { get; init; } = true;
    public bool IsMaximizable { get; init; }
    
    public double HostMaxHeight { get; init; } = double.PositiveInfinity;
    public double HostMaxWidth { get; init; } = double.PositiveInfinity;
    public double HostMinHeight { get; init; } = 0d;
    public double HostMinWidth { get; init; } = 0d;
    public double HostWidth { get; init; } = double.NaN;
    public double HostHeight { get; init; } = double.NaN;

    /// <summary>
    /// 次选项仅 Window Host 类型的弹窗有效
    /// </summary>
    public bool IsMinimizable { get; init; } = true;
    
    public bool IsDragMovable { get; init; } = true;
    public bool IsFooterVisible { get; init; } = true;
    public Control? PlacementTarget { get; init; }
    public Dimension? HorizontalOffset { get; init; }
    public Dimension? VerticalOffset { get; init; }
    public DialogHostType DialogHostType { get; init; } = DialogHostType.Overlay;
    public DialogStandardButtons StandardButtons { get; init; } = DialogStandardButton.NoButton;
    public DialogStandardButton DefaultStandardButton { get; init; }
    public DialogHorizontalAnchor HorizontalStartupLocation { get; init; } = DialogHorizontalAnchor.Custom;
    public DialogVerticalAnchor VerticalStartupLocation { get; init; } = DialogVerticalAnchor.Custom;
}
