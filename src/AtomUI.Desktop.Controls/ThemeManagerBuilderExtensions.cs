using AtomUI.Controls;
using AtomUI.Generated.AtomUI_Desktop_Controls;
using AtomUI.MotionScene;
using AtomUI.Theme;
using AtomUI.Theme.Language;
using AtomUI.Theme.Schema;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Input;
using Avalonia.Media.Transformation;

namespace AtomUI.Desktop.Controls;

public static class ThemeManagerBuilderExtensions
{
    private static readonly HashSet<string> s_browserExcludedControlIds = new(StringComparer.Ordinal)
    {
        "AdornerLayer",
        "IndicatorScrollViewer",
        "OtpLineEdit",
        "SplitView",
        "TextBox",
        "TreeFlyout",
        "Window",
        "WindowTitleBar"
    };

    public static IThemeManagerBuilder UseDesktopControls(this IThemeManagerBuilder themeManagerBuilder)
    {
        themeManagerBuilder.UseCommonControls();
        DialogInputCaptureTracker.Initialize();
        var controlDescriptors = RuntimePlatform.Features.SupportsNativeWindow
            ? GeneratedThemeSchema.GetControls()
            : GetBrowserControlDescriptors();
        foreach (var descriptor in controlDescriptors)
        {
            themeManagerBuilder.AddControlToken(descriptor);
        }
        themeManagerBuilder.AddControlThemesProvider(RuntimePlatform.Features.SupportsNativeWindow
            ? new DesktopControlThemesProvider()
            : new BrowserDesktopControlThemesProvider());

        var languageProviders = LanguageProviderPool.GetLanguageProviders();
        foreach (var languageProvider in languageProviders)
        {
            themeManagerBuilder.AddLanguageProviders(languageProvider);
        }

        themeManagerBuilder.AddInitializer(InitializeDesktopRuntime);

        return themeManagerBuilder;
    }

    private static void InitializeDesktopRuntime(IThemeManager manager)
    {
        Animation.RegisterCustomAnimator<TransformOperations, MotionTransformOptionsAnimator>();
        var inputManager = AvaloniaLocator.CurrentMutable.GetService<IInputManager>();
        if (inputManager is not null)
        {
            AvaloniaLocator.CurrentMutable.BindToSelf(new ToolTipService(inputManager));
        }

        if (!RuntimePlatform.Features.SupportsNativeWindow)
        {
            return;
        }

        if (manager is ThemeManager themeManager)
        {
            MediaBreakPointThemeBootstrapper.Attach(themeManager);
        }
    }

    private static IReadOnlyList<ControlTokenDescriptor> GetBrowserControlDescriptors()
    {
        var descriptors = GeneratedThemeSchema.GetControls();
        var browserDescriptors = new List<ControlTokenDescriptor>(
            descriptors.Count - s_browserExcludedControlIds.Count);
        foreach (var descriptor in descriptors)
        {
            if (!s_browserExcludedControlIds.Contains(descriptor.Identity.Id))
            {
                browserDescriptors.Add(descriptor);
            }
        }

        return browserDescriptors;
    }
}
