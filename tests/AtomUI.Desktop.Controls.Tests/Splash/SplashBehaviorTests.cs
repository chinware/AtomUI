using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AtomUI.Desktop.Controls.DesignTokens;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Splash;

public class SplashBehaviorTests
{
    static SplashBehaviorTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Splash_Default_State_Uses_Loading_And_Indeterminate_PseudoClasses()
    {
        var splash = new AtomUI.Desktop.Controls.Splash();

        splash.Status.ShouldBe(SplashStatus.Loading);
        splash.IsIndeterminate.ShouldBeTrue();
        splash.Progress.ShouldBeNull();
        splash.Classes.ShouldContain(SplashPseudoClass.Loading);
        splash.Classes.ShouldContain(SplashPseudoClass.Indeterminate);
        splash.Classes.ShouldNotContain(SplashPseudoClass.Determinate);
    }

    [Fact]
    public void Splash_Progress_Is_Normalized_And_Updates_PseudoClasses()
    {
        var splash = new AtomUI.Desktop.Controls.Splash
        {
            IsIndeterminate = false,
            Progress        = 1.5
        };

        splash.Progress.ShouldBe(1d);
        splash.Classes.ShouldContain(SplashPseudoClass.Determinate);
        splash.Classes.ShouldNotContain(SplashPseudoClass.Indeterminate);

        splash.Progress = -0.25;

        splash.Progress.ShouldBe(0d);

        splash.IsIndeterminate = true;

        splash.Classes.ShouldContain(SplashPseudoClass.Indeterminate);
        splash.Classes.ShouldNotContain(SplashPseudoClass.Determinate);
    }

    [Fact]
    public void Splash_Status_Updates_Status_PseudoClasses()
    {
        var splash = new AtomUI.Desktop.Controls.Splash();

        splash.Status = SplashStatus.Success;

        splash.Classes.ShouldContain(SplashPseudoClass.Success);
        splash.Classes.ShouldNotContain(SplashPseudoClass.Loading);
        splash.Classes.ShouldNotContain(SplashPseudoClass.Error);

        splash.Status = SplashStatus.Error;

        splash.Classes.ShouldContain(SplashPseudoClass.Error);
        splash.Classes.ShouldNotContain(SplashPseudoClass.Success);
    }

    [Fact]
    public void Splash_State_Methods_Update_Visual_State_Without_Owning_Business_Flow()
    {
        var splash = new AtomUI.Desktop.Controls.Splash();

        splash.SetProgress(0.6, "Loading modules", "Step 2");

        splash.Status.ShouldBe(SplashStatus.Loading);
        splash.IsIndeterminate.ShouldBeFalse();
        splash.Progress.ShouldBe(0.6);
        splash.Message.ShouldBe("Loading modules");
        splash.Detail.ShouldBe("Step 2");

        splash.SetError("Startup failed", "Configuration file is invalid");

        splash.Status.ShouldBe(SplashStatus.Error);
        splash.IsIndeterminate.ShouldBeFalse();
        splash.Progress.ShouldBeNull();
        splash.Message.ShouldBe("Startup failed");
        splash.Detail.ShouldBe("Configuration file is invalid");
    }

    [Fact]
    public void Splash_Declares_Template_Parts_For_Custom_Themes()
    {
        var expectedParts = new Dictionary<string, Type>
        {
            ["PART_RootLayout"]       = typeof(Border),
            ["PART_SurfaceLayout"]    = typeof(Border),
            ["PART_ContentLayout"]    = typeof(StackPanel),
            ["PART_LogoPresenter"]    = typeof(ContentPresenter),
            ["PART_TitleBlock"]       = typeof(AtomUI.Desktop.Controls.TextBlock),
            ["PART_SubtitleBlock"]    = typeof(AtomUI.Desktop.Controls.TextBlock),
            ["PART_ContentPresenter"] = typeof(ContentPresenter),
            ["PART_ProgressLayout"]   = typeof(Panel),
            ["PART_Spin"]             = typeof(AtomUI.Desktop.Controls.Spin),
            ["PART_ProgressBar"]      = typeof(AtomUI.Desktop.Controls.ProgressBar),
            ["PART_MessageBlock"]     = typeof(AtomUI.Desktop.Controls.TextBlock),
            ["PART_DetailBlock"]      = typeof(AtomUI.Desktop.Controls.TextBlock),
            ["PART_FooterPresenter"]  = typeof(ContentPresenter)
        };

        var parts = typeof(AtomUI.Desktop.Controls.Splash)
                    .GetCustomAttributes(typeof(TemplatePartAttribute), false)
                    .Cast<TemplatePartAttribute>()
                    .ToDictionary(part => part.Name, part => part.Type);

        parts.Count.ShouldBe(expectedParts.Count);
        foreach (var (name, expectedType) in expectedParts)
        {
            parts.TryGetValue(name, out var actualType).ShouldBeTrue(name);
            actualType.ShouldBe(expectedType);
        }
    }

    [Fact]
    public async Task Splash_Static_Api_Delegates_To_Default_Service()
    {
        var original = AtomUI.Desktop.Controls.Splash.DefaultService;
        var service  = new RecordingSplashService();
        var cancellationToken = TestContext.Current.CancellationToken;
        AtomUI.Desktop.Controls.Splash.DefaultService = service;

        try
        {
            var options = new SplashOptions
            {
                Title = "AtomUI"
            };

            var splash = await AtomUI.Desktop.Controls.Splash.ShowAsync(options, cancellationToken);
            await AtomUI.Desktop.Controls.Splash.SetMessageAsync("Preparing", "Loading assets", cancellationToken);
            await AtomUI.Desktop.Controls.Splash.SetProgressAsync(0.4, "Halfway", cancellationToken: cancellationToken);
            await AtomUI.Desktop.Controls.Splash.SetErrorAsync("Failed", "Bad state", cancellationToken);
            await AtomUI.Desktop.Controls.Splash.CloseAsync(cancellationToken);

            splash.ShouldBeSameAs(service.CurrentSplash);
            service.ShowOptions.ShouldBeSameAs(options);
            service.Message.ShouldBe("Preparing");
            service.Detail.ShouldBe("Loading assets");
            service.Progress.ShouldBe(0.4);
            service.ProgressMessage.ShouldBe("Halfway");
            service.ProgressDetail.ShouldBeNull();
            service.ErrorMessage.ShouldBe("Failed");
            service.ErrorDetail.ShouldBe("Bad state");
            service.CloseCalls.ShouldBe(1);
        }
        finally
        {
            AtomUI.Desktop.Controls.Splash.DefaultService = original;
        }
    }

    [Fact]
    public async Task SplashWindow_CloseAsync_Is_Idempotent()
    {
        var window = new SplashWindow
        {
            MinimumShowDuration = TimeSpan.Zero,
            CloseDelay          = TimeSpan.Zero,
            FadeOutDuration     = TimeSpan.Zero
        };
        var cancellationToken = TestContext.Current.CancellationToken;

        await window.CloseAsync(cancellationToken);
        await window.CloseAsync(cancellationToken);

        window.IsCloseRequested.ShouldBeTrue();
    }

    [Fact]
    public async Task SplashWindow_CloseAsync_Closes_Window_Shown_With_Owner()
    {
        var owner = new Avalonia.Controls.Window
        {
            Width         = 240,
            Height        = 160,
            ShowInTaskbar = false
        };
        var window = new SplashWindow
        {
            Splash              = new AtomUI.Desktop.Controls.Splash(),
            MinimumShowDuration = TimeSpan.Zero,
            CloseDelay          = TimeSpan.Zero,
            FadeOutDuration     = TimeSpan.Zero
        };
        var cancellationToken = TestContext.Current.CancellationToken;
        var closed            = false;
        window.Closed += (_, _) => closed = true;

        try
        {
            owner.Show();
            window.Show(owner);

            await window.CloseAsync(cancellationToken);

            closed.ShouldBeTrue();
            window.IsVisible.ShouldBeFalse();
        }
        finally
        {
            if (window.IsVisible)
            {
                window.Close();
            }

            if (owner.IsVisible)
            {
                owner.Close();
            }
        }
    }

    [Fact]
    public void SplashWindow_Size_Options_Apply_To_Splash_Without_Constructor_Content_Host()
    {
        var service = new InspectingSplashService();
        var window  = new SplashWindow();

        service.ConfigureWindowForTest(window, new SplashOptions
        {
            Width     = 560,
            MinHeight = 360
        });

        double.IsNaN(window.Width).ShouldBeTrue();
        window.MinHeight.ShouldBe(360);
        window.Splash.ShouldNotBeNull();
        var splash = window.Splash!;
        splash.Width.ShouldBe(560);
        splash.MinHeight.ShouldBe(360);
        window.Content.ShouldBeNull();
    }

    [Fact]
    public void SplashWindow_Runtime_Uses_Transparent_TopLevel_Composition()
    {
        var window = new SplashWindow
        {
            Splash              = new AtomUI.Desktop.Controls.Splash(),
            MinimumShowDuration = TimeSpan.Zero,
            CloseDelay          = TimeSpan.Zero,
            FadeOutDuration     = TimeSpan.Zero
        };

        try
        {
            window.Show();

            GetSolidBrushColor(window.Background).ShouldBe(Colors.Transparent);
            GetSolidBrushColor(window.TransparencyBackgroundFallback).ShouldBe(Colors.Transparent);
            window.TransparencyLevelHint.ShouldContain(WindowTransparencyLevel.Transparent);
            window.ExtendClientAreaToDecorationsHint.ShouldBeFalse();
            window.WindowDecorations.ShouldBe(WindowDecorations.None);
            window.ShowInTaskbar.ShouldBeFalse();
            window.CanResize.ShouldBeFalse();
            window.SizeToContent.ShouldBe(SizeToContent.WidthAndHeight);
            window.WindowStartupLocation.ShouldBe(WindowStartupLocation.CenterScreen);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void SplashWindow_StyleKey_Uses_SplashWindow_For_Implicit_ControlTheme()
    {
        var window = new SplashWindow();

        window.StyleKey.ShouldBe(typeof(SplashWindow));
    }

    [Fact]
    public void SplashWindow_Property_Contract_Follows_Avalonia_Control_Rules()
    {
        var window = new SplashWindow();

        window.Splash.ShouldBeNull();
        SplashWindow.SplashProperty.ShouldBeAssignableTo<StyledProperty<AtomUI.Desktop.Controls.Splash?>>();

        GetPublicStaticPropertyField("MinimumShowDurationProperty")
            .FieldType.ShouldBe(typeof(StyledProperty<TimeSpan>));
        GetPublicStaticPropertyField("CloseDelayProperty")
            .FieldType.ShouldBe(typeof(StyledProperty<TimeSpan>));
        GetPublicStaticPropertyField("FadeOutDurationProperty")
            .FieldType.ShouldBe(typeof(StyledProperty<TimeSpan>));
        GetPublicStaticPropertyField("IsCloseRequestedProperty")
            .FieldType.ShouldBe(typeof(DirectProperty<SplashWindow, bool>));
    }

    [Fact]
    public void SplashService_Default_Window_Factory_Does_Not_Apply_Options_During_Creation()
    {
        var service = new InspectingSplashService();
        var options = new SplashOptions
        {
            Title               = "Configured title",
            Width               = 560,
            MinHeight           = 360,
            Topmost             = false,
            MinimumShowDuration = TimeSpan.Zero,
            CloseDelay          = TimeSpan.Zero,
            FadeOutDuration     = TimeSpan.Zero
        };

        var window = service.CreateWindowForTest(options);

        window.Splash.ShouldBeNull(
            "SplashService should create a plain SplashWindow and apply SplashOptions after construction");
        window.Content.ShouldBeNull();
    }

    [Fact]
    public void SplashService_Configures_Window_After_Default_Creation()
    {
        var service = new InspectingSplashService();
        var window  = service.CreateWindowForTest(new SplashOptions());
        var options = new SplashOptions
        {
            Title               = "Configured title",
            Width               = 560,
            MinHeight           = 360,
            Topmost             = false,
            MinimumShowDuration = TimeSpan.Zero,
            CloseDelay          = TimeSpan.Zero,
            FadeOutDuration     = TimeSpan.Zero
        };

        service.ConfigureWindowForTest(window, options);

        window.Splash.ShouldNotBeNull();
        var splash = window.Splash!;
        splash.Title.ShouldBe("Configured title");
        splash.Width.ShouldBe(560);
        splash.MinHeight.ShouldBe(360);
        window.MinHeight.ShouldBe(360);
        window.Topmost.ShouldBeFalse();
        window.MinimumShowDuration.ShouldBe(TimeSpan.Zero);
        window.CloseDelay.ShouldBe(TimeSpan.Zero);
        window.FadeOutDuration.ShouldBe(TimeSpan.Zero);
    }

    [Fact]
    public void SplashService_Source_Shows_SplashWindow_With_Desktop_MainWindow_Owner()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls.Extras/Splash/SplashService.cs"));

        source.ShouldContain("IClassicDesktopStyleApplicationLifetime");
        source.ShouldContain("MainWindow");
        source.ShouldContain("window.Show(ownerWindow)");
        source.ShouldContain("window.Show()");
    }

    [Fact]
    public void Splash_Themes_Include_SplashWindow_Theme()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls.Extras/Splash/Themes/SplashThemes.axaml"));

        source.ShouldContain("SplashTheme.axaml");
        source.ShouldContain("SplashWindowTheme.axaml");
        File.Exists(GetRepoFile("src/AtomUI.Desktop.Controls.Extras/Splash/Themes/SplashWindowTheme.axaml"))
            .ShouldBeTrue();
    }

    [Fact]
    public void SplashWindow_Theme_Provides_Transparent_Window_And_Surface_Host_Template()
    {
        var document = XDocument.Load(GetRepoFile("src/AtomUI.Desktop.Controls.Extras/Splash/Themes/SplashWindowTheme.axaml"));
        XNamespace av = "https://github.com/avaloniaui";
        XNamespace atom = "https://atomui.net";
        XNamespace x  = "http://schemas.microsoft.com/winfx/2006/xaml";

        var theme = document.Descendants(av + "ControlTheme").Single();
        theme.Attribute(x + "Key").ShouldNotBeNull().Value.ShouldBe("{x:Type atom:SplashWindow}");
        theme.Attribute("TargetType").ShouldNotBeNull().Value.ShouldBe("atom:SplashWindow");
        theme.Elements(av + "Setter").ShouldContain(setter =>
            (string?)setter.Attribute("Property") == "Background" &&
            (string?)setter.Attribute("Value") == "Transparent");
        theme.Elements(av + "Setter").ShouldContain(setter =>
            (string?)setter.Attribute("Property") == "TransparencyBackgroundFallback" &&
            (string?)setter.Attribute("Value") == "Transparent");
        theme.Elements(av + "Setter").ShouldContain(setter =>
            (string?)setter.Attribute("Property") == "TransparencyLevelHint" &&
            (string?)setter.Attribute("Value") == "Transparent");
        theme.Elements(av + "Setter").ShouldContain(setter =>
            (string?)setter.Attribute("Property") == "ExtendClientAreaToDecorationsHint" &&
            (string?)setter.Attribute("Value") == "False");
        theme.Elements(av + "Setter").ShouldContain(setter =>
            (string?)setter.Attribute("Property") == "WindowDecorations" &&
            (string?)setter.Attribute("Value") == "None");
        theme.Elements(av + "Setter").ShouldContain(setter =>
            (string?)setter.Attribute("Property") == "ShowInTaskbar" &&
            (string?)setter.Attribute("Value") == "False");
        theme.Elements(av + "Setter").ShouldContain(setter =>
            (string?)setter.Attribute("Property") == "CanResize" &&
            (string?)setter.Attribute("Value") == "False");
        theme.Elements(av + "Setter").ShouldContain(setter =>
            (string?)setter.Attribute("Property") == "SizeToContent" &&
            (string?)setter.Attribute("Value") == "WidthAndHeight");
        theme.Elements(av + "Setter").ShouldContain(setter =>
            (string?)setter.Attribute("Property") == "WindowStartupLocation" &&
            (string?)setter.Attribute("Value") == "CenterScreen");
        theme.Elements(av + "Setter").ShouldContain(setter =>
            (string?)setter.Attribute("Property") == "Topmost" &&
            (string?)setter.Attribute("Value") == "True");

        var templateSetter = theme.Elements(av + "Setter")
                                  .Single(element => (string?)element.Attribute("Property") == "Template");
        var template = templateSetter.Descendants(av + "ControlTemplate").Single();
        template.Attribute("TargetType").ShouldNotBeNull().Value.ShouldBe("atom:SplashWindow");

        var surfaceHost = template.Descendants(atom + "ShadowsAwareContainer")
                                  .Single(element =>
                                      (string?)element.Attribute("Name") == "PART_SurfaceHost");
        surfaceHost.Attribute("ClipToBounds").ShouldNotBeNull().Value.ShouldBe("False");
        surfaceHost.Attribute("BoxShadow").ShouldNotBeNull().Value.ShouldBe(
            "{atom:SplashTokenResource SurfaceBoxShadow}");
        surfaceHost.Attribute("CornerRadius").ShouldNotBeNull().Value.ShouldBe(
            "{atom:SplashTokenResource SurfaceCornerRadius}");
        surfaceHost.Attribute("Child").ShouldNotBeNull().Value.ShouldBe("{TemplateBinding Splash}");
        template.Descendants(av + "ContentPresenter").ShouldBeEmpty();
    }

    [Fact]
    public void SplashWindow_Source_Does_Not_Keep_CSharp_Surface_Host_Token_Bridge()
    {
        var sourcePath = GetRepoFile("src/AtomUI.Desktop.Controls.Extras/Splash/SplashWindow.cs");
        var source     = File.ReadAllText(sourcePath);
        var hostPath   = Path.Combine(Path.GetDirectoryName(sourcePath)!, "SplashWindowSurfaceHost.cs");

        File.Exists(hostPath).ShouldBeFalse();
        source.ShouldNotContain("SplashWindowSurfaceHost");
        source.ShouldNotContain("TokenResourceBinder.CreateTokenBinding");
        source.ShouldNotContain("Content =");
    }

    [Fact]
    public void Splash_Source_Does_Not_Expose_SplashController()
    {
        var splashDirectory = Path.GetDirectoryName(GetRepoFile("src/AtomUI.Desktop.Controls.Extras/Splash/Splash.cs"))!;
        var controllerPath  = Path.Combine(splashDirectory, "SplashController.cs");
        var sourceFiles     = Directory.GetFiles(splashDirectory, "*.cs", SearchOption.TopDirectoryOnly);

        File.Exists(controllerPath).ShouldBeFalse();
        foreach (var sourceFile in sourceFiles)
        {
            File.ReadAllText(sourceFile).ShouldNotContain("SplashController");
            File.ReadAllText(sourceFile).ShouldNotContain("CurrentController");
        }
    }

    [Fact]
    public void Splash_Theme_Separates_Rounded_Clip_And_Surface_Background()
    {
        var document = XDocument.Load(GetRepoFile("src/AtomUI.Desktop.Controls.Extras/Splash/Themes/SplashTheme.axaml"));
        XNamespace av = "https://github.com/avaloniaui";

        var rootLayout = document.Descendants(av + "Border")
                                 .Single(element =>
                                     (string?)element.Attribute("Name") == "PART_RootLayout");

        rootLayout.Attribute("CornerRadius").ShouldNotBeNull().Value.ShouldBe(
            "{atom:SplashTokenResource SurfaceCornerRadius}");
        rootLayout.Attribute("ClipToBounds").ShouldNotBeNull().Value.ShouldBe("True");
        rootLayout.Attribute("Background").ShouldBeNull();
        rootLayout.Attribute("Padding").ShouldBeNull();
        rootLayout.Attribute("BoxShadow").ShouldBeNull();

        var surfaceLayout = rootLayout.Elements(av + "Border")
                                      .Single(element =>
                                          (string?)element.Attribute("Name") == "PART_SurfaceLayout");

        surfaceLayout.Attribute("Background").ShouldNotBeNull().Value.ShouldBe(
            "{atom:SplashTokenResource SurfaceBackground}");
        surfaceLayout.Attribute("CornerRadius").ShouldNotBeNull().Value.ShouldBe(
            "{atom:SplashTokenResource SurfaceCornerRadius}");
        surfaceLayout.Attribute("ClipToBounds").ShouldNotBeNull().Value.ShouldBe("True");
        surfaceLayout.Attribute("Padding").ShouldNotBeNull().Value.ShouldBe(
            "{atom:SplashTokenResource ContentPadding}");
        surfaceLayout.Elements(av + "StackPanel")
                     .Single(element => (string?)element.Attribute("Name") == "PART_ContentLayout");
    }

    [Fact]
    public void SplashWindow_Content_Host_Measure_Includes_Surface_Shadow_Thickness()
    {
        var service = new InspectingSplashService();
        var window  = new SplashWindow();
        service.ConfigureWindowForTest(window, new SplashOptions
        {
            Width     = 560,
            MinHeight = 360
        });
        window.Resources[SplashTokenKind.SurfaceBoxShadow] = BoxShadows.Parse("0 0 24 0 #99000000");

        try
        {
            window.Show();

            var surfaceHost = window.GetVisualDescendants()
                                    .OfType<ShadowsAwareContainer>()
                                    .Single();
            surfaceHost.Child.ShouldBeSameAs(window.Splash);
            surfaceHost.ClipToBounds.ShouldBeFalse();

            surfaceHost.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            window.Splash.ShouldNotBeNull();
            var splash = window.Splash!;
            surfaceHost.DesiredSize.Width.ShouldBeGreaterThan(splash.Width);
            surfaceHost.DesiredSize.Height.ShouldBeGreaterThan(splash.MinHeight);
        }
        finally
        {
            window.Close();
        }
    }

    private sealed class RecordingSplashService : ISplashService
    {
        public SplashWindow? CurrentWindow { get; private set; }
        public AtomUI.Desktop.Controls.Splash? CurrentSplash { get; private set; }
        public SplashOptions? ShowOptions { get; private set; }
        public string? Message { get; private set; }
        public string? Detail { get; private set; }
        public double? Progress { get; private set; }
        public string? ProgressMessage { get; private set; }
        public string? ProgressDetail { get; private set; }
        public string? ErrorMessage { get; private set; }
        public string? ErrorDetail { get; private set; }
        public int CloseCalls { get; private set; }

        public Task<AtomUI.Desktop.Controls.Splash> ShowAsync(
            SplashOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            ShowOptions   = options;
            CurrentSplash = new AtomUI.Desktop.Controls.Splash();
            return Task.FromResult(CurrentSplash);
        }

        public Task SetMessageAsync(string? message, string? detail = null, CancellationToken cancellationToken = default)
        {
            Message = message;
            Detail  = detail;
            return Task.CompletedTask;
        }

        public Task SetProgressAsync(double? progress, string? message = null, string? detail = null, CancellationToken cancellationToken = default)
        {
            Progress        = progress;
            ProgressMessage = message;
            ProgressDetail  = detail;
            return Task.CompletedTask;
        }

        public Task SetStatusAsync(SplashStatus status, string? message = null, string? detail = null, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task SetErrorAsync(string message, string? detail = null, CancellationToken cancellationToken = default)
        {
            ErrorMessage = message;
            ErrorDetail  = detail;
            return Task.CompletedTask;
        }

        public Task CloseAsync(CancellationToken cancellationToken = default)
        {
            CloseCalls++;
            return Task.CompletedTask;
        }
    }

    private sealed class InspectingSplashService : SplashService
    {
        public SplashWindow CreateWindowForTest(SplashOptions? options) => CreateWindow(options);

        public void ConfigureWindowForTest(SplashWindow window, SplashOptions options)
        {
            ConfigureWindow(window, options);
        }
    }

    private static Color GetSolidBrushColor(IBrush? brush)
    {
        brush.ShouldBeAssignableTo<ISolidColorBrush>();
        return ((ISolidColorBrush)brush!).Color;
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
    }

    private static System.Reflection.FieldInfo GetPublicStaticPropertyField(string name)
    {
        return typeof(SplashWindow).GetField(
                   name,
                   System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
               ?? throw new MissingFieldException(typeof(SplashWindow).FullName, name);
    }
}
