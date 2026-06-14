using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Reflection;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Fonts.AlibabaPuHuiTi;
using AtomUI.Icons.AntDesign;
using AtomUI.Theme.Styling;
using AtomUIGallery.Workspace.ViewModels;
using AtomUIGallery.Workspace.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using ReactiveUI;
using ReactiveUI.Avalonia;
using DesktopHyperLinkButton = AtomUI.Desktop.Controls.HyperLinkButton;
using DesktopTag = AtomUI.Desktop.Controls.Tag;
using SvgControl = Avalonia.Svg.Svg;

namespace AtomUIGallery.Browser;

internal sealed class BrowserGalleryView : UserControl, IScreen, IMediaBreakAwareControl
{
    private static readonly FontFamily s_appFontFamily =
        FontFamily.Parse($"fonts:AlibabaSans#Alibaba Sans, {AlibabaPuHuiTiFontConstants.FontFamily}, $Default");

    private readonly CaseNavigationViewModel _caseNavigationViewModel;
    private readonly CompositeDisposable     _themeBindings = new();

    public RoutingState Router { get; } = new();
    public MediaBreakPoint MediaBreakPoint { get; private set; } = MediaBreakPoint.Large;
    public event EventHandler<MediaBreakPointChangedEventArgs>? MediaBreakPointChanged;

    public BrowserGalleryView()
    {
        FontFamily               = s_appFontFamily;
        _caseNavigationViewModel = new CaseNavigationViewModel(this);

        var showCaseNavigation = new CaseNavigation
        {
            Name      = "ShowCaseNavigation",
            ViewModel = _caseNavigationViewModel
        };
        Grid.SetRow(showCaseNavigation, 1);

        var sidebar = new Border
        {
            BorderThickness = new Thickness(0, 0, 1, 0),
            Child           = CreateSidebar(showCaseNavigation)
        };
        BindToken(sidebar, Border.BackgroundProperty, SharedTokenKind.ColorBgContainer);
        BindToken(sidebar, Border.BorderBrushProperty, SharedTokenKind.ColorBorderSecondary);

        var routedViewHost = new RoutedViewHost
        {
            Name            = "RoutedViewHost",
            Router          = Router,
            PageTransition  = null,
            ClipToBounds    = true
        };

        var contentHost = new Border
        {
            Child = routedViewHost
        };
        contentHost.SizeChanged += HandleContentHostSizeChanged;
        BindToken(contentHost, Border.BackgroundProperty, SharedTokenKind.ColorBgLayout);
        Grid.SetColumn(contentHost, 1);

        var rootLayout = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("280,*"),
            Children =
            {
                sidebar,
                contentHost
            }
        };
        BindToken(rootLayout, Panel.BackgroundProperty, SharedTokenKind.ColorBgContainer);

        var visualLayerManager = new VisualLayerManager
        {
            Child = rootLayout
        };
        ConfigureOverlayLayers(visualLayerManager);
        Content = visualLayerManager;
    }

    private Grid CreateSidebar(CaseNavigation showCaseNavigation)
    {
        var logoHost = new StackPanel
        {
            Margin = new Thickness(24, 28, 24, 24),
            Children =
            {
                new SvgControl(new Uri("https://atomui.net"))
                {
                    Path                = "avares://AtomUIGallery/Assets/atomui-oss.svg",
                    Width               = 190,
                    Height              = 45,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment   = VerticalAlignment.Center
                }
            }
        };
        Grid.SetRow(logoHost, 0);

        var footer = CreateSidebarFooter();
        Grid.SetRow(footer, 2);

        return new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,*,Auto"),
            Children =
            {
                logoHost,
                showCaseNavigation,
                footer
            }
        };
    }

    private Border CreateSidebarFooter()
    {
        var linkButtons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing     = 0,
            Children =
            {
                CreateFooterLinkButton(AntDesignIconKind.GlobalOutlined, "https://www.atomui.net"),
                CreateFooterLinkButton(AntDesignIconKind.GiteeOutlined, "https://gitee.com/chinware/AtomUI"),
                CreateFooterLinkButton(AntDesignIconKind.GithubOutlined, "https://github.com/chinware/atomui")
            }
        };

        var versionTag = new DesktopTag
        {
            Text                = GalleryVersionInfo.DisplayVersion,
            TagColor            = "Green",
            VerticalAlignment   = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        Grid.SetColumn(versionTag, 1);

        var footerLayout = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*"),
            Children =
            {
                linkButtons,
                versionTag
            }
        };

        var footer = new Border
        {
            BorderThickness = new Thickness(0, 1, 0, 0),
            Padding         = new Thickness(16, 12),
            Child           = footerLayout
        };
        BindToken(footer, Border.BorderBrushProperty, SharedTokenKind.ColorBorderSecondary);
        return footer;
    }

    private static DesktopHyperLinkButton CreateFooterLinkButton(AntDesignIconKind iconKind, string uri)
    {
        return new DesktopHyperLinkButton
        {
            Width       = 40,
            Height      = 40,
            IconWidth   = 22,
            IconHeight  = 22,
            Icon        = CreateIcon(iconKind),
            NavigateUri = new Uri(uri)
        };
    }

    private static PathIcon CreateIcon(AntDesignIconKind kind)
    {
        return (PathIcon)new AntDesignIconProvider(kind).ProvideValue(null!);
    }

    private void BindToken(AvaloniaObject target, AvaloniaProperty property, SharedTokenKind tokenKind)
    {
        _themeBindings.Add(TokenResourceBinder.CreateTokenBinding(target, property, tokenKind));
    }

    private void HandleContentHostSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        NotifyMediaBreakPointChanged(ResolveMediaBreakPoint(e.NewSize.Width));
    }

    private void NotifyMediaBreakPointChanged(MediaBreakPoint breakPoint)
    {
        if (MediaBreakPoint == breakPoint)
        {
            return;
        }

        MediaBreakPoint = breakPoint;
        MediaBreakPointChanged?.Invoke(this, new MediaBreakPointChangedEventArgs(breakPoint));
    }

    private static MediaBreakPoint ResolveMediaBreakPoint(double width)
    {
        if (width >= (double)MediaBreakPoint.ExtraExtraLarge)
        {
            return MediaBreakPoint.ExtraExtraLarge;
        }

        if (width >= (double)MediaBreakPoint.ExtraLarge)
        {
            return MediaBreakPoint.ExtraLarge;
        }

        if (width >= (double)MediaBreakPoint.Large)
        {
            return MediaBreakPoint.Large;
        }

        if (width >= (double)MediaBreakPoint.Medium)
        {
            return MediaBreakPoint.Medium;
        }

        if (width >= (double)MediaBreakPoint.Small)
        {
            return MediaBreakPoint.Small;
        }

        return MediaBreakPoint.ExtraSmall;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _themeBindings.Dispose();
        base.OnDetachedFromVisualTree(e);
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(VisualLayerManager))]
    private static void ConfigureOverlayLayers(VisualLayerManager visualLayerManager)
    {
        visualLayerManager.EnableOverlayLayer = true;
        SetVisualLayerManagerProperty(visualLayerManager, "EnablePopupOverlayLayer", true);

        _ = GetVisualLayerManagerPropertyValue(visualLayerManager, "OverlayLayer");
        _ = GetVisualLayerManagerPropertyValue(visualLayerManager, "PopupOverlayLayer");
        _ = GetVisualLayerManagerPropertyValue(visualLayerManager, "LightDismissOverlayLayer");
    }

    private static void SetVisualLayerManagerProperty(VisualLayerManager visualLayerManager,
                                                      string propertyName,
                                                      object? value)
    {
        var propertyInfo = typeof(VisualLayerManager)
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (propertyInfo is null)
        {
            throw new InvalidOperationException($"Unable to find {propertyName} on {nameof(VisualLayerManager)}.");
        }

        propertyInfo.SetValue(visualLayerManager, value);
    }

    private static object? GetVisualLayerManagerPropertyValue(VisualLayerManager visualLayerManager,
                                                              string propertyName)
    {
        var propertyInfo = typeof(VisualLayerManager)
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (propertyInfo is null)
        {
            throw new InvalidOperationException($"Unable to find {propertyName} on {nameof(VisualLayerManager)}.");
        }

        return propertyInfo.GetValue(visualLayerManager);
    }
}
