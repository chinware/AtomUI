using Avalonia;

namespace AtomUI.Desktop.Controls;

internal class VerticalNavMenuItemHeader : BaseNavMenuItemHeader
{
    internal static readonly StyledProperty<object?> NodeHeaderProperty =
        AvaloniaProperty.Register<VerticalNavMenuItemHeader, object?>(nameof(NodeHeader));

    internal object? NodeHeader
    {
        get => GetValue(NodeHeaderProperty);
        set => SetValue(NodeHeaderProperty, value);
    }
}
