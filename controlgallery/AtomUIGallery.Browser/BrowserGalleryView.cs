using AtomUIGallery.ShowCases.AboutUs;
using AtomUIGallery.ShowCases.Button;
using AtomUIGallery.ShowCases.Icon;
using AtomUIGallery.ShowCases.Palette;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;

namespace AtomUIGallery.Browser;

internal sealed class BrowserGalleryView : UserControl, IScreen
{
    private static readonly FontFamily s_appFontFamily =
        FontFamily.Parse("fonts:AlibabaSans#Alibaba Sans, $Default");

    private readonly Border _aboutUsNavigationItem;
    private readonly Border _buttonNavigationItem;
    private readonly Border _paletteNavigationItem;
    private readonly Border _iconsNavigationItem;
    private readonly Grid _contentHost;
    private readonly Dictionary<BrowserGalleryPageKind, Control> _pageCache = new();
    private BrowserGalleryPageKind? _activePageKind;

    public RoutingState Router { get; } = new();

    public BrowserGalleryView()
    {
        FontFamily = s_appFontFamily;

        var header = new Border
        {
            BorderBrush     = new SolidColorBrush(Color.Parse("#E5E7EB")),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding         = new Thickness(24, 0),
            Child           = new StackPanel
            {
                Orientation       = Orientation.Horizontal,
                Spacing           = 12,
                VerticalAlignment = VerticalAlignment.Center,
                Children =
                {
                    new Image
                    {
                        Source = LoadGalleryLogo(),
                        Width  = 28,
                        Height = 28
                    },
                    new TextBlock
                    {
                        Text              = "AtomUI Browser Gallery",
                        FontSize          = 20,
                        FontWeight        = FontWeight.SemiBold,
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground        = new SolidColorBrush(Color.Parse("#111827"))
                    }
                }
            }
        };
        Grid.SetColumnSpan(header, 2);

        _aboutUsNavigationItem = CreateNavigationItem("AboutUs", () => ShowAboutUs());
        _buttonNavigationItem  = CreateNavigationItem("Button", () => ShowButton());
        _paletteNavigationItem = CreateNavigationItem("Palette", () => ShowPalette());
        _iconsNavigationItem   = CreateNavigationItem("Icons", () => ShowIcons());

        var navigation = new Border
        {
            BorderBrush     = new SolidColorBrush(Color.Parse("#E5E7EB")),
            BorderThickness = new Thickness(0, 0, 1, 0),
            Background      = new SolidColorBrush(Color.Parse("#F9FAFB")),
            Padding         = new Thickness(20),
            Child           = new StackPanel
            {
                Spacing = 10,
                Children =
                {
                    new TextBlock
                    {
                        Text       = "Showcases",
                        FontSize   = 16,
                        FontWeight = FontWeight.SemiBold,
                        Foreground = new SolidColorBrush(Color.Parse("#374151"))
                    },
                    _aboutUsNavigationItem,
                    _buttonNavigationItem,
                    _paletteNavigationItem,
                    _iconsNavigationItem
                }
            }
        };
        Grid.SetRow(navigation, 1);

        _contentHost = new Grid();
        Grid.SetColumn(_contentHost, 1);
        Grid.SetRow(_contentHost, 1);

        Content = new Grid
        {
            RowDefinitions    = new RowDefinitions("64,*"),
            ColumnDefinitions = new ColumnDefinitions("260,*"),
            Background        = Brushes.White,
            Children =
            {
                header,
                navigation,
                _contentHost
            }
        };

        ShowAboutUs();
    }

    private static Bitmap LoadGalleryLogo()
    {
        using var stream = AssetLoader.Open(new Uri("avares://AtomUIGallery.Browser/Assets/gallery-logo.png"));
        return new Bitmap(stream);
    }

    private Border CreateNavigationItem(string text, Action navigate)
    {
        var item = new Border
        {
            CornerRadius = new CornerRadius(4),
            Padding      = new Thickness(8, 6),
            Cursor       = new Cursor(StandardCursorType.Hand),
            Child        = new TextBlock
            {
                Text       = text,
                FontSize   = 14,
                Foreground = new SolidColorBrush(Color.Parse("#111827"))
            }
        };
        item.PointerPressed += (_, _) => navigate();
        return item;
    }

    private void ShowAboutUs()
    {
        ShowPage(BrowserGalleryPageKind.AboutUs, _aboutUsNavigationItem, () => new AboutUsPage
        {
            DataContext = new AboutUsViewModel(this)
        });
    }

    private void ShowButton()
    {
        ShowPage(BrowserGalleryPageKind.Button, _buttonNavigationItem, () => new ButtonShowCase
        {
            DataContext = new ButtonViewModel(this)
        });
    }

    private void ShowPalette()
    {
        ShowPage(BrowserGalleryPageKind.Palette, _paletteNavigationItem, () => new PaletteShowCase
        {
            DataContext = new PaletteViewModel(this)
        });
    }

    private void ShowIcons()
    {
        ShowPage(BrowserGalleryPageKind.Icons, _iconsNavigationItem, () => new IconShowCase
        {
            DataContext = new IconViewModel(this)
        });
    }

    private void ShowPage(BrowserGalleryPageKind pageKind, Border navigationItem, Func<Control> createPage)
    {
        if (_activePageKind == pageKind)
        {
            return;
        }

        if (!_pageCache.TryGetValue(pageKind, out var page))
        {
            page           = createPage();
            page.IsVisible = false;
            _pageCache.Add(pageKind, page);
            _contentHost.Children.Add(page);
        }

        foreach (var cachedPage in _pageCache)
        {
            cachedPage.Value.IsVisible = cachedPage.Key == pageKind;
        }

        _activePageKind = pageKind;
        UpdateNavigationSelection(navigationItem);
    }

    private void UpdateNavigationSelection(Border selectedItem)
    {
        ApplyNavigationItemState(_aboutUsNavigationItem, selectedItem == _aboutUsNavigationItem);
        ApplyNavigationItemState(_buttonNavigationItem, selectedItem == _buttonNavigationItem);
        ApplyNavigationItemState(_paletteNavigationItem, selectedItem == _paletteNavigationItem);
        ApplyNavigationItemState(_iconsNavigationItem, selectedItem == _iconsNavigationItem);
    }

    private static void ApplyNavigationItemState(Border item, bool isSelected)
    {
        item.Background = isSelected
            ? new SolidColorBrush(Color.Parse("#F3F4F6"))
            : Brushes.Transparent;

        if (item.Child is TextBlock textBlock)
        {
            textBlock.FontWeight = isSelected ? FontWeight.SemiBold : FontWeight.Normal;
        }
    }

    private enum BrowserGalleryPageKind
    {
        AboutUs,
        Button,
        Palette,
        Icons
    }
}
