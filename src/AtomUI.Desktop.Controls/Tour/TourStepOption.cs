using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

public interface ITourStepOption
{
    Control? Target { get; set; }
    bool? IsArrowVisible { get; set; }
    bool? IsPointAtCenter { get; set; }
    PathIcon? CloseIcon { get; set; }
    object? Cover { get; set; }
    object? Title { get; set; }
    IDataTemplate? TitleTemplate { get; set; }
    object? Description { get; set; }
    IDataTemplate? DescriptionTemplate { get; set; }
    TourPlacementMode? Placement { get; set; }
    TourStyleType? StyleType { get; set; }
    bool? IsShowMask { get; set; }
    IBrush? MaskColor { get; set; }
    bool? IsScrollIntoView { get; set; }
}

public class TourStepOption : ITourStepOption
{
    public Control? Target { get; set; }
    public bool? IsArrowVisible { get; set; }
    public bool? IsPointAtCenter { get; set; }
    public PathIcon? CloseIcon { get; set; }
    public object? Cover { get; set; }
    public object? Title { get; set; }
    public IDataTemplate? TitleTemplate { get; set; }
    public object? Description { get; set; }
    public IDataTemplate? DescriptionTemplate { get; set; }
    public TourPlacementMode? Placement { get; set; }
    public TourStyleType? StyleType { get; set; }
    public bool? IsShowMask { get; set; }
    public IBrush? MaskColor { get; set; }
    public bool? IsScrollIntoView { get; set; }
}