using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Splash;

[LanguageProvider(LanguageCode.en_US, SplashShowCase.LanguageId)]
internal partial class en_US
{
    public const string ScenarioExamples = "Examples";
    public const string ComponentCategory = "Other";
    public const string ComponentStatusPreview = "Preview";
    public const string ComponentIntroducedVersion = "v6.0.7";
    public const string PageSubtitle = "Present desktop startup progress before the main workspace is ready.";
    public const string PageDescription = "Splash combines a compact branded surface, loading indicator, determinate progress, status copy, and optional footer. The service API can host it in a borderless startup window, while the visual control can be previewed directly in Gallery.";
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Use the visual Splash control to show startup copy with an indeterminate loading indicator.";
    public const string DeterminateTitle = "Determinate progress";
    public const string DeterminateDescription = "Set Progress and disable IsIndeterminate when startup has measurable phases.";
    public const string StatusTitle = "Status";
    public const string StatusDescription = "Switch Status to Success or Error when the startup flow reaches a terminal state.";
    public const string ComposedTitle = "Logo, content and footer";
    public const string ComposedDescription = "Template the logo and footer when a product needs richer startup metadata.";
    public const string WindowServiceTitle = "Window service";
    public const string WindowServiceDescription = "Open a real desktop Splash window, run a five-second mock startup flow, then close it automatically.";
    public const string P2SubtitleDesktopBoot = "Desktop boot sequence";
    public const string P2MessagePreparingShell = "Preparing workspace";
    public const string P2DetailLoadingThemeAndResources = "Loading theme, language resources and cached state.";
    public const string P2MessageLoadingModules = "Loading modules";
    public const string P2DetailProgress = "Theme, icons and route catalog are ready. Optional packages are being initialized.";
    public const string P2MessageReady = "Workspace ready";
    public const string P2DetailReady = "The main window can be shown after the minimum splash duration.";
    public const string P2MessageError = "Startup failed";
    public const string P2DetailError = "Use SetErrorAsync to surface a blocking startup error before closing.";
    public const string P2FooterStaticPreview = "Static Gallery preview";
    public const string P2ContentModuleCore = "Core";
    public const string P2ContentModuleTheme = "Theme";
    public const string P2ContentModuleGallery = "Gallery";
    public const string P2FooterDesktopOnly = "Window service is intended for desktop startup.";
    public const string P2ContentShowWindowSplash = "Show window Splash";
    public const string P2WindowSplashSubtitle = "Desktop startup simulation";
    public const string P2WindowSplashMessageStarting = "Starting Gallery workspace";
    public const string P2WindowSplashDetailStarting = "A temporary Splash window is showing a mocked five-second startup.";
    public const string P2WindowSplashMessageLoadingTheme = "Loading theme resources";
    public const string P2WindowSplashMessageLoadingControls = "Registering control packages";
    public const string P2WindowSplashMessageLoadingRoutes = "Preparing route catalog";
    public const string P2WindowSplashMessageWarmingCache = "Warming Gallery cache";
    public const string P2WindowSplashMessageFinalizing = "Finalizing workspace";
    public const string P2WindowSplashDetailProgress = "This is a fake loading step for the Gallery demo.";
    public const string P2WindowSplashMessageComplete = "Gallery ready";
    public const string P2WindowSplashDetailComplete = "The fake startup flow has completed and the window will close.";
    public const string P2WindowSplashFooter = "Desktop Extras package / window-hosted startup surface";
    public const string ApiStaticShowAsync = "Static convenience API that delegates to Splash.DefaultService.";
    public const string ApiServiceShowAsync = "Instance service API for applications that want dependency-injected startup flow control.";
    public const string ApiMethodSetProgress = "Updates progress, message and detail on the splash instance.";
    public const string ApiOptionMinimumShowDuration = "Minimum display time before the splash window may close.";

}
