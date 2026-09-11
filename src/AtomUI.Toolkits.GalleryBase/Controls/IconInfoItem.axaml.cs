using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Toolkits.GalleryBase.Controls;

public class IconInfoItem : Button, IMotionAwareControl
{
    public static readonly StyledProperty<string> IconNameProperty =
        AvaloniaProperty.Register<IconInfoItem, string>(nameof(IconName));

    public static readonly StyledProperty<Icon?> IconProperty =
        AvaloniaProperty.Register<IconInfoItem, Icon?>(nameof(Icon));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<IconInfoItem>();

    public string IconName
    {
        get => GetValue(IconNameProperty);
        set => SetValue(IconNameProperty, value);
    }

    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
}
