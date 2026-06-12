using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;
using Avalonia.Media;

namespace AtomUIGallery.Controls;

public class GalleryStickyTabsHost : TemplatedControl
{
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<GalleryStickyTabsHost, object?>(nameof(Header));

    public static readonly StyledProperty<object?> StickyContentProperty =
        AvaloniaProperty.Register<GalleryStickyTabsHost, object?>(nameof(StickyContent));

    public static readonly StyledProperty<object?> ContentProperty =
        ContentControl.ContentProperty.AddOwner<GalleryStickyTabsHost>();

    public static readonly StyledProperty<Thickness> StickyContentPaddingProperty =
        AvaloniaProperty.Register<GalleryStickyTabsHost, Thickness>(nameof(StickyContentPadding));

    public static readonly StyledProperty<IBrush?> StickyBackgroundProperty =
        Border.BackgroundProperty.AddOwner<GalleryStickyTabsHost>();

    public static readonly StyledProperty<IBrush?> StickyBorderBrushProperty =
        Border.BorderBrushProperty.AddOwner<GalleryStickyTabsHost>();

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public object? StickyContent
    {
        get => GetValue(StickyContentProperty);
        set => SetValue(StickyContentProperty, value);
    }

    [Content]
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public Thickness StickyContentPadding
    {
        get => GetValue(StickyContentPaddingProperty);
        set => SetValue(StickyContentPaddingProperty, value);
    }

    public IBrush? StickyBackground
    {
        get => GetValue(StickyBackgroundProperty);
        set => SetValue(StickyBackgroundProperty, value);
    }

    public IBrush? StickyBorderBrush
    {
        get => GetValue(StickyBorderBrushProperty);
        set => SetValue(StickyBorderBrushProperty, value);
    }

    public GalleryStickyTabsHost()
    {
        this.RegisterTokenResourceScope(GalleryStickyTabsHostToken.ScopeProvider);
    }
}
