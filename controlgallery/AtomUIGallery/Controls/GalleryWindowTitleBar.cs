using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.Controls;

public class GalleryWindowTitleBar : WindowTitleBar
{
    public static readonly StyledProperty<Control?> MenuProperty =
        AvaloniaProperty.Register<GalleryWindowTitleBar, Control?>(nameof(Menu));

    public Control? Menu
    {
        get => GetValue(MenuProperty);
        set => SetValue(MenuProperty, value);
    }

    static GalleryWindowTitleBar()
    {
        MenuProperty.Changed.AddClassHandler<GalleryWindowTitleBar>((titleBar, args) =>
        {
            titleBar.LeftAddOn = args.GetNewValue<Control?>();
        });
    }
}
