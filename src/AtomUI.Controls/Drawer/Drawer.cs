using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public class Drawer : TemplatedControl
{
    [Content]
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }
    public static readonly StyledProperty<object?> ContentProperty = AvaloniaProperty
        .Register<Drawer, object?>(nameof(Content));
}