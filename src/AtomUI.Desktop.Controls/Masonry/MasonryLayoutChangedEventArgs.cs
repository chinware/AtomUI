using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

/// <summary>
/// Provides data for the <see cref="Masonry.LayoutChanged"/> event.
/// </summary>
/// <remarks>
/// The event only notifies effective column assignment changes, not pixel-level arrange
/// rectangles, to avoid frequent raising during scroll and resize. Data item identity is
/// maintained by the ItemsControl container and the user's view model; no key is required.
/// </remarks>
public class MasonryLayoutChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the effective column assignment for every child, in child order.
    /// </summary>
    public IReadOnlyList<MasonryItemLayout> Items { get; }

    public MasonryLayoutChangedEventArgs(IReadOnlyList<MasonryItemLayout> items)
    {
        Items = items;
    }
}

/// <summary>
/// Describes the effective layout result of a single Masonry child.
/// </summary>
public readonly record struct MasonryItemLayout
{
    /// <summary>
    /// The child element reference.
    /// </summary>
    public Control Element { get; init; }

    /// <summary>
    /// The index of the child in child order.
    /// </summary>
    public int Index { get; init; }

    /// <summary>
    /// The effective column the child was placed into.
    /// For a full-span item this is <c>0</c>.
    /// </summary>
    public int Column { get; init; }

    /// <summary>
    /// Whether the child occupies the full Masonry width.
    /// </summary>
    public bool IsFullSpan { get; init; }

    public MasonryItemLayout(Control element, int index, int column, bool isFullSpan)
    {
        Element     = element;
        Index       = index;
        Column      = column;
        IsFullSpan  = isFullSpan;
    }
}
