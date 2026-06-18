using Avalonia.Collections;
using AtomUI.Controls;

namespace AtomUI.Desktop.Controls;

public record DescriptionItem
{
    public string Label { get; set; } = string.Empty;
    public object? Content { get; set; }
    public bool IsFilled { get; set; } = false;
    public ResponsiveInt Span { get; set; } = new (1);
}

public class DescriptionItems : AvaloniaList<DescriptionItem> {}
