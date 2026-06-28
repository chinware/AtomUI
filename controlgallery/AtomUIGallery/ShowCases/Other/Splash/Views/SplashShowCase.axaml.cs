using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme.Styling;
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

    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private const double WindowSplashWidth = 560;
    private const double WindowSplashMinHeight = 360;

    private static readonly TimeSpan WindowSplashDuration = TimeSpan.FromSeconds(5);
    private static readonly IBrush WindowSplashTitleBrush = Brushes.White;
    private static readonly IBrush WindowSplashPrimaryTextBrush = new SolidColorBrush(Color.Parse("#F5F8FF"));
    private static readonly IBrush WindowSplashSubtleTextBrush = new SolidColorBrush(Color.Parse("#D6E4FF"));
    private static readonly IBrush WindowSplashStageTextBrush = new SolidColorBrush(Color.Parse("#314659"));
    private static readonly IBrush WindowSplashStageIndexBrush = Brushes.White;

    private static readonly SplashLogoInfo WindowSplashLogo = new(
        "A6",
        CreateWindowSplashLogoBrush(),
        Brushes.White);

    private static readonly WindowSplashStageInfo[] WindowSplashStages =
    [
        new("01", SplashShowCaseLangResourceKind.P2ContentModuleCore, en_US.P2ContentModuleCore, Color.Parse("#1677FF")),
        new("02", SplashShowCaseLangResourceKind.P2ContentModuleTheme, en_US.P2ContentModuleTheme, Color.Parse("#13C2C2")),
        new("03", SplashShowCaseLangResourceKind.P2ContentModuleGallery, en_US.P2ContentModuleGallery, Color.Parse("#722ED1"))
    ];

    private static readonly (SplashShowCaseLangResourceKind ResourceKind, string Fallback)[] WindowSplashProgressMessages =
    [
        (SplashShowCaseLangResourceKind.P2WindowSplashMessageLoadingTheme, en_US.P2WindowSplashMessageLoadingTheme),
        (SplashShowCaseLangResourceKind.P2WindowSplashMessageLoadingControls, en_US.P2WindowSplashMessageLoadingControls),
        (SplashShowCaseLangResourceKind.P2WindowSplashMessageLoadingRoutes, en_US.P2WindowSplashMessageLoadingRoutes),
        (SplashShowCaseLangResourceKind.P2WindowSplashMessageWarmingCache, en_US.P2WindowSplashMessageWarmingCache),
        (SplashShowCaseLangResourceKind.P2WindowSplashMessageFinalizing, en_US.P2WindowSplashMessageFinalizing)
    ];

    private readonly GalleryShowCaseScenarioController _scenarioController;
    private bool _isWindowSplashRunning;

    public SplashShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(
            ScenarioTabs,
            ScenarioContentHost,
            CreateScenarioContent,
            ExamplesContent);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _scenarioController.Attach(DataContext);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _scenarioController.Detach();
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        _scenarioController.UpdateDataContext(DataContext);
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            ApiScenario         => new SplashApiDataGrid(),
            DesignTokenScenario => new SplashDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Splash scenario: {scenario}")
        };
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
                Content             = WindowSplashStages,
                ContentTemplate     = CreateWindowSplashStagesTemplate(),
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

    private static IDataTemplate CreateWindowSplashStagesTemplate()
    {
        return new FuncDataTemplate<WindowSplashStageInfo[]>((stages, _) =>
        {
            var stagePanel = new WrapPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Orientation         = Orientation.Horizontal,
                ItemSpacing         = 8,
                LineSpacing         = 8
            };

            if (stages is not null)
            {
                foreach (var stage in stages)
                {
                    stagePanel.Children.Add(CreateWindowSplashStage(stage));
                }
            }

            return new Border
            {
                Width        = 480,
                CornerRadius = new CornerRadius(10),
                Padding      = new Thickness(12, 10),
                Background   = CreateWindowSplashPanelBrush(),

            };
        });
    }

    private static Control CreateWindowSplashStage(WindowSplashStageInfo stage)
    {
        return new Border
        {
            CornerRadius = new CornerRadius(999),
            Padding      = new Thickness(8, 4),
            Background   = new SolidColorBrush(Color.Parse("#FFFFFF")),
            Child = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing     = 6,
                Children =
                {
                    new Border
                    {
                        Width            = 22,
                        Height           = 22,
                        CornerRadius     = new CornerRadius(11),
                        Background       = new SolidColorBrush(stage.AccentColor),
                        VerticalAlignment = VerticalAlignment.Center,
                        Child = new AtomUITextBlock
                        {
                            Text                = stage.Index,
                            Foreground          = WindowSplashStageIndexBrush,
                            FontSize            = 10,
                            FontWeight          = FontWeight.Bold,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment   = VerticalAlignment.Center
                        }
                    },
                    new AtomUITextBlock
                    {
                        Text              = Lang(stage.ResourceKind, stage.Fallback),
                        Foreground        = WindowSplashStageTextBrush,
                        FontSize          = 12,
                        FontWeight        = FontWeight.SemiBold,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                }
            }
        };
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

    private static LinearGradientBrush CreateWindowSplashPanelBrush()
    {
        return new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0.5, RelativeUnit.Relative),
            EndPoint   = new RelativePoint(1, 0.5, RelativeUnit.Relative),
            GradientStops = new GradientStops
            {
                new(Color.Parse("#F0F7FF"), 0),
                new(Color.Parse("#F9F0FF"), 0.58),
                new(Color.Parse("#F6FFED"), 1)
            }
        };
    }

    private static LinearGradientBrush CreateWindowSplashSurfaceBrush()
    {
        return new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint   = new RelativePoint(1, 1, RelativeUnit.Relative),
            GradientStops = new GradientStops
            {
                new(Color.Parse("#0B1026"), 0),
                new(Color.Parse("#1D39C4"), 0.48),
                new(Color.Parse("#13C2C2"), 1)
            }
        };
    }

    private sealed class GallerySplashService : SplashService
    {
        protected override SplashWindow CreateWindow(SplashOptions? options)
        {
            var window = base.CreateWindow(null);

            window.Resources[SplashTokenKind.SurfaceBackground] = CreateWindowSplashSurfaceBrush();
            window.Resources[SharedTokenKind.ColorTextHeading]   = WindowSplashTitleBrush;
            window.Resources[SharedTokenKind.ColorText]          = WindowSplashPrimaryTextBrush;
            window.Resources[SplashTokenKind.SubtleForeground]   = WindowSplashSubtleTextBrush;

            return window;
        }
    }

    private sealed record WindowSplashStageInfo(
        string Index,
        SplashShowCaseLangResourceKind ResourceKind,
        string Fallback,
        Color AccentColor);
}
