using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

using AtomUITextBlock = AtomUI.Desktop.Controls.TextBlock;

namespace AtomUIGallery.ShowCases.Splash;

public partial class SplashShowCase : GalleryReactiveUserControl<SplashViewModel>
{
    public const string LanguageId = nameof(SplashShowCase);

    private const double WindowSplashWidth = 560;
    private const double WindowSplashMinHeight = 360;
    private static readonly TimeSpan WindowSplashDuration = TimeSpan.FromSeconds(5);

    private static readonly SplashLogoInfo WindowSplashLogo = new(
        "A6",
        CreateWindowSplashLogoBrush(),
        Brushes.White);

    private static readonly (SplashShowCaseLangResourceKind ResourceKind, string Fallback)[] WindowSplashProgressMessages =
    [
        (SplashShowCaseLangResourceKind.P2WindowSplashMessageLoadingTheme, en_US.P2WindowSplashMessageLoadingTheme),
        (SplashShowCaseLangResourceKind.P2WindowSplashMessageLoadingControls, en_US.P2WindowSplashMessageLoadingControls),
        (SplashShowCaseLangResourceKind.P2WindowSplashMessageLoadingRoutes, en_US.P2WindowSplashMessageLoadingRoutes),
        (SplashShowCaseLangResourceKind.P2WindowSplashMessageWarmingCache, en_US.P2WindowSplashMessageWarmingCache),
        (SplashShowCaseLangResourceKind.P2WindowSplashMessageFinalizing, en_US.P2WindowSplashMessageFinalizing)
    ];

    private bool _isWindowSplashRunning;

    public SplashShowCase()
    {
        InitializeComponent();
    }

    private async void HandleShowWindowSplashButtonClick(object? sender, RoutedEventArgs e)
    {
        e.Handled = true;
        if (_isWindowSplashRunning)
        {
            return;
        }

        _isWindowSplashRunning = true;
        var triggerControl = sender as Control;
        if (triggerControl is not null)
        {
            triggerControl.IsEnabled = false;
        }

        var splashService = new GallerySplashService();
        try
        {
            await splashService.ShowAsync(new SplashOptions
            {
                Logo                = WindowSplashLogo,
                LogoTemplate        = CreateWindowSplashLogoTemplate(),
                Title               = "AtomUI Gallery",
                Subtitle            = Lang(SplashShowCaseLangResourceKind.P2WindowSplashSubtitle, en_US.P2WindowSplashSubtitle),
                Message             = Lang(SplashShowCaseLangResourceKind.P2WindowSplashMessageStarting, en_US.P2WindowSplashMessageStarting),
                Detail              = Lang(SplashShowCaseLangResourceKind.P2WindowSplashDetailStarting, en_US.P2WindowSplashDetailStarting),
                Footer              = Lang(SplashShowCaseLangResourceKind.P2WindowSplashFooter, en_US.P2WindowSplashFooter),
                FooterTemplate      = CreateWindowSplashFooterTemplate(),
                Progress            = 0d,
                IsIndeterminate     = false,
                MinimumShowDuration = WindowSplashDuration,
                CloseDelay          = TimeSpan.Zero,
                FadeOutDuration     = TimeSpan.FromMilliseconds(180),
                Width               = WindowSplashWidth,
                MinHeight           = WindowSplashMinHeight
            });

            for (var step = 1; step <= WindowSplashProgressMessages.Length; step++)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                var progress = (double)step / WindowSplashProgressMessages.Length;
                var message  = WindowSplashProgressMessages[step - 1];
                await splashService.SetProgressAsync(
                    progress,
                    Lang(message.ResourceKind, message.Fallback),
                    Lang(SplashShowCaseLangResourceKind.P2WindowSplashDetailProgress, en_US.P2WindowSplashDetailProgress));
            }

            await splashService.SetStatusAsync(SplashStatus.Success,
                Lang(SplashShowCaseLangResourceKind.P2WindowSplashMessageComplete, en_US.P2WindowSplashMessageComplete),
                Lang(SplashShowCaseLangResourceKind.P2WindowSplashDetailComplete, en_US.P2WindowSplashDetailComplete));
        }
        finally
        {
            try
            {
                await splashService.CloseAsync();
            }
            finally
            {
                if (triggerControl is not null)
                {
                    triggerControl.IsEnabled = true;
                }
                _isWindowSplashRunning = false;
            }
        }
    }

    private static string Lang(SplashShowCaseLangResourceKind resourceKind, string fallback)
    {
        return Application.Current is null
            ? fallback
            : LanguageResourceBinder.GetLangResource(resourceKind) ?? fallback;
    }

    private static IDataTemplate CreateWindowSplashLogoTemplate()
    {
        return new FuncDataTemplate<SplashLogoInfo>((logo, _) =>
        {
            if (logo is null)
            {
                return new Panel();
            }

            return new Border
            {
                Width               = 44,
                Height              = 44,
                CornerRadius        = new CornerRadius(14),
                Background          = logo.Background,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center,
                Child = new AtomUITextBlock
                {
                    Text                = logo.Text,
                    Foreground          = logo.Foreground,
                    FontSize            = 15,
                    FontWeight          = FontWeight.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment   = VerticalAlignment.Center
                }
            };
        });
    }

    private static IDataTemplate CreateWindowSplashFooterTemplate()
    {
        return new FuncDataTemplate<string>((text, _) =>
        {
            return new Border
            {
                CornerRadius = new CornerRadius(999),
                Padding      = new Thickness(10, 4),
                Background   = new SolidColorBrush(Color.Parse("#F6FFED")),
                Child = new AtomUITextBlock
                {
                    Text                = text,
                    Foreground          = new SolidColorBrush(Color.Parse("#389E0D")),
                    FontSize            = 12,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment   = VerticalAlignment.Center
                }
            };
        });
    }

    private static LinearGradientBrush CreateWindowSplashLogoBrush()
    {
        return new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint   = new RelativePoint(1, 1, RelativeUnit.Relative),
            GradientStops = new GradientStops
            {
                new(Color.Parse("#1677FF"), 0),
                new(Color.Parse("#13C2C2"), 0.48),
                new(Color.Parse("#722ED1"), 1)
            }
        };
    }

    private sealed class GallerySplashService : SplashService
    {
        protected override SplashWindow CreateWindow(SplashOptions? options)
        {
            return new GallerySplashWindow();
        }
    }
}
