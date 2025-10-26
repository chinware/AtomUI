using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public class SelectGroupHeader : TemplatedControl
{
    public static readonly StyledProperty<object?> HeaderProperty = AvaloniaProperty.Register<ContentControl, object?>(nameof (Header));
    public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty = AvaloniaProperty.Register<ContentControl, IDataTemplate?>(nameof (HeaderTemplate));
    
    [Content]
    [DependsOn(nameof(HeaderTemplate))]
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
    
    public IDataTemplate? HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }
}