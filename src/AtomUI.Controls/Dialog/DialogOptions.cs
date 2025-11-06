using AtomUI.IconPkg;
using Avalonia.Controls;

namespace AtomUI.Controls;

public record DialogOptions
{
    public string? Title { get; init; }
    public Icon? TitleIcon { get; init; }
    public bool IsLightDismissEnabled { get; init; } = false;
    public bool IsModal { get; init; } = true;
    public bool IsResizable { get; init; } = false;
    public bool IsClosable { get; init; } = true;
    public bool IsMaximizable { get; init; } = false;

    /// <summary>
    /// 仅仅对 Window Host 类型的弹窗有效
    /// </summary>
    public bool IsMinimizable { get; init; } = true;

    public bool IsDragMovable { get; init; } = false;
    public bool IsFooterVisible { get; init; } = true;
    public Control? PlacementTarget { get; init; }
    public Dimension? HorizontalOffset { get; init; }
    public Dimension? VerticalOffset { get; init; }
    public DialogHostType DialogHostType { get; init; } = DialogHostType.Overlay;
    public DialogStandardButtons StandardButtons { get; init; } = DialogStandardButton.NoButton;
    public DialogStandardButton DefaultStandardButton { get; init; }
    public DialogHorizontalAnchor HorizontalStartupLocation { get; init; } =  DialogHorizontalAnchor.Custom;
    public DialogVerticalAnchor VerticalStartupLocation { get; init; } = DialogVerticalAnchor.Custom;
}