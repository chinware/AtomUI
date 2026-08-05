using System.Reactive.Disposables;
using AtomUI.Data;
using AtomUI.Theme.Resources;
using AtomUI.Toolkits.GalleryBase.Configuration;
using AtomUI.Toolkits.GalleryBase.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using ReactiveUI;
using ReactiveUI.Avalonia;
using DesktopHyperLinkButton = AtomUI.Desktop.Controls.HyperLinkButton;
using DesktopNavMenu = AtomUI.Desktop.Controls.NavMenu;
using DesktopNavMenuMode = AtomUI.Desktop.Controls.NavMenuMode;
using DesktopTag = AtomUI.Desktop.Controls.Tag;
using SvgControl = Avalonia.Svg.Svg;

namespace AtomUI.Toolkits.GalleryBase.Shell;

public sealed class GalleryShellView : UserControl, IDisposable
{
    private readonly CompositeDisposable _themeBindings = new();
    private readonly CompositeDisposable _sidebarBindings = new();
    private readonly Border              _navigationSeparator;
    private bool _isDisposed;

    public Border ContentHost { get; }

    public RoutedViewHost RoutedViewHost { get; }

    public GalleryShellView(GalleryBaseConfiguration configuration,
                            Control navigationView,
                            RoutingState router)
    {
        var sidebarHost    = navigationView as IGallerySidebarNavMenuHost;
        var sidebarNavMenu = sidebarHost?.SidebarNavMenu;
        sidebarNavMenu?.SetCurrentValue(WidthProperty, configuration.Shell.SidebarWidth);

        var sidebar = new Border
        {
            Name         = "WorkspaceSidebar",
            ClipToBounds = sidebarNavMenu is not null,
            Child        = CreateSidebar(configuration,
                                         navigationView,
                                         sidebarNavMenu,
                                         sidebarHost?.SidebarHeaderAction)
        };
        BindToken(sidebar, Border.BackgroundProperty, SharedTokenKind.ColorBgContainer);
        if (sidebarNavMenu is not null)
        {
            _sidebarBindings.Add(sidebar.Bind(WidthProperty, sidebarNavMenu.GetObservable(WidthProperty)));
        }

        RoutedViewHost = new RoutedViewHost
        {
            Name           = "RoutedViewHost",
            Router         = router,
            PageTransition = null,
            ClipToBounds   = true
        };

        ContentHost = new Border
        {
            Child = RoutedViewHost
        };
        BindToken(ContentHost, Border.BackgroundProperty, SharedTokenKind.ColorBgLayout);
        Grid.SetColumn(ContentHost, 1);

        _navigationSeparator = new Border
        {
            Name             = "WorkspaceNavigationSeparator",
            Width            = 1,
            HorizontalAlignment = HorizontalAlignment.Left,
            IsHitTestVisible = false
        };
        BindToken(_navigationSeparator, Border.BackgroundProperty, SharedTokenKind.ColorBorderSecondary);
        Grid.SetColumn(_navigationSeparator, 1);

        var rootLayout = new Grid
        {
            Name = "WorkspaceRootLayout",
            ColumnDefinitions = sidebarNavMenu is not null
                ? new ColumnDefinitions("Auto,*")
                : new ColumnDefinitions($"{configuration.Shell.SidebarWidth},*"),
            Children =
            {
                sidebar,
                ContentHost,
                _navigationSeparator
            }
        };
        BindToken(rootLayout, Panel.BackgroundProperty, SharedTokenKind.ColorBgContainer);

        var codeDrawerHost = new GalleryShowCaseCodeDrawerHost(configuration)
        {
            PageContent = rootLayout
        };

        Content = codeDrawerHost;
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        _sidebarBindings.Dispose();
        _themeBindings.Dispose();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        Dispose();
        base.OnDetachedFromVisualTree(e);
    }

    private Grid CreateSidebar(GalleryBaseConfiguration configuration,
                               Control navigationView,
                               DesktopNavMenu? sidebarNavMenu,
                               Control? sidebarHeaderAction)
    {
        var brandContent = new Border
        {
            Name                = "WorkspaceBrandContent",
            Margin              = new Thickness(24, 0, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
            Child               = CreateBrandContent(configuration.Branding)
        };
        var brandHeader = new Grid
        {
            Name   = "WorkspaceBrandHost",
            Margin = new Thickness(0, 20, 0, 16),
            Children =
            {
                brandContent
            }
        };

        Border? sidebarHeaderActionHost = null;
        if (sidebarHeaderAction is not null)
        {
            sidebarHeaderActionHost = new Border
            {
                Name                = "WorkspaceSidebarHeaderActionHost",
                Margin              = new Thickness(0, 0, 4, 0),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment   = VerticalAlignment.Center,
                Child               = sidebarHeaderAction
            };
            brandHeader.Children.Add(sidebarHeaderActionHost);
        }
        Grid.SetRow(brandHeader, 0);

        Grid.SetRow(navigationView, 1);

        var sidebar = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,*,Auto"),
            Children =
            {
                brandHeader,
                navigationView
            }
        };

        var footer = CreateSidebarFooter(configuration);
        if (footer is not null)
        {
            Grid.SetRow(footer, 2);
            sidebar.Children.Add(footer);
        }

        BindSidebarPresentation(brandHeader,
                                brandContent,
                                sidebarHeaderActionHost,
                                footer,
                                sidebarNavMenu);

        return sidebar;
    }

    private void BindSidebarPresentation(Control brandHeader,
                                         Control brandContent,
                                         Border? sidebarHeaderActionHost,
                                         Control? footer,
                                         DesktopNavMenu? sidebarNavMenu)
    {
        if (sidebarNavMenu is null)
        {
            return;
        }

        void UpdatePresentation()
        {
            var isInlineCollapsed = sidebarNavMenu.Mode == DesktopNavMenuMode.Inline &&
                                    sidebarNavMenu.IsInlineCollapsed;
            brandHeader.Margin = isInlineCollapsed
                ? new Thickness(0, 20, 0, 0)
                : new Thickness(0, 20, 0, 16);
            brandContent.IsVisible = !isInlineCollapsed;
            if (footer is not null)
            {
                footer.IsVisible = !isInlineCollapsed;
            }
            if (sidebarHeaderActionHost is not null)
            {
                sidebarHeaderActionHost.HorizontalAlignment = isInlineCollapsed
                    ? HorizontalAlignment.Center
                    : HorizontalAlignment.Right;
                sidebarHeaderActionHost.Margin = isInlineCollapsed
                    ? default
                    : new Thickness(0, 0, 4, 0);
            }
        }

        _sidebarBindings.Add(sidebarNavMenu.GetObservable(DesktopNavMenu.IsInlineCollapsedProperty)
                                           .Subscribe(_ => UpdatePresentation()));
        _sidebarBindings.Add(sidebarNavMenu.GetObservable(DesktopNavMenu.ModeProperty)
                                           .Subscribe(_ => UpdatePresentation()));
    }

    private static Control CreateBrandContent(GalleryBrandingConfiguration branding)
    {
        return branding.Logo switch
        {
            Control control => control,
            string resourcePath => new SvgControl(new Uri("https://atomui.net"))
            {
                Path                = resourcePath,
                Width               = 190,
                Height              = 45,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment   = VerticalAlignment.Center
            },
            null => new TextBlock
            {
                Text         = branding.AppName,
                FontSize     = 20,
                FontWeight   = FontWeight.SemiBold,
                TextWrapping = TextWrapping.NoWrap
            },
            _ => new ContentControl
            {
                Content = branding.Logo
            }
        };
    }

    private Border? CreateSidebarFooter(GalleryBaseConfiguration configuration)
    {
        if (configuration.Branding.Links.Count == 0 &&
            string.IsNullOrWhiteSpace(configuration.Branding.VersionText) &&
            !configuration.Shell.IsFooterVisibleWhenEmpty)
        {
            return null;
        }

        var footerLayout = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*")
        };

        var linkButtons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing     = 0
        };
        foreach (var link in configuration.Branding.Links)
        {
            linkButtons.Children.Add(CreateFooterLinkButton(link));
        }
        footerLayout.Children.Add(linkButtons);

        if (!string.IsNullOrWhiteSpace(configuration.Branding.VersionText))
        {
            var versionTag = new DesktopTag
            {
                Text                = configuration.Branding.VersionText,
                TagColor            = "Green",
                VerticalAlignment   = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Grid.SetColumn(versionTag, 1);
            footerLayout.Children.Add(versionTag);
        }

        var footer = new Border
        {
            Name            = "WorkspaceSidebarFooter",
            BorderThickness = new Thickness(0, 1, 0, 0),
            Padding         = new Thickness(16, 12),
            Child           = footerLayout
        };
        BindToken(footer, Border.BorderBrushProperty, SharedTokenKind.ColorBorderSecondary);
        return footer;
    }

    private static DesktopHyperLinkButton CreateFooterLinkButton(GalleryLink link)
    {
        return new DesktopHyperLinkButton
        {
            Width       = 40,
            Height      = 40,
            IconWidth   = 22,
            IconHeight  = 22,
            Icon        = ResolveLinkIcon(link.Icon),
            NavigateUri = new Uri(link.Uri)
        };
    }

    private static PathIcon? ResolveLinkIcon(object? icon)
    {
        return icon switch
        {
            null                   => null,
            PathIcon pathIcon      => pathIcon,
            Func<PathIcon> factory => factory(),
            _                      => null
        };
    }

    private void BindToken(AvaloniaObject target, AvaloniaProperty property, SharedTokenKind tokenKind)
    {
        _themeBindings.Add(TokenResourceBinder.CreateGlobalTokenBinding(target, property, tokenKind));
    }
}
