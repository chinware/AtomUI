using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUIGallery.Controls;

public class IconInfoItem : TemplatedControl
{
    public static readonly StyledProperty<string> IconNameProperty = 
        AvaloniaProperty.Register<IconInfoItem, string>(nameof(IconName));
    
    public static readonly StyledProperty<Icon> IconProperty = 
        AvaloniaProperty.Register<IconInfoItem, Icon>(nameof(Icon));

    public string IconName
    {
        get => GetValue(IconNameProperty);
        set => SetValue(IconNameProperty, value);
    }
    
    public Icon Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}