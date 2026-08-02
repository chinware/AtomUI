using AtomUI.Controls;
using AtomUI.Generated.AtomUI_Desktop_Controls;
using AtomUI.MotionScene;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Input;
using Avalonia.Media.Transformation;

namespace AtomUI.Desktop.Controls;

public static class ThemeManagerBuilderExtensions
{
    public static IThemeManagerBuilder UseDesktopControls(this IThemeManagerBuilder themeManagerBuilder)
    {
        themeManagerBuilder.UseCommonControls();
        DialogInputCaptureTracker.Initialize();
        if (RuntimePlatform.Features.SupportsNativeWindow)
        {
            GeneratedControlPackageRegistration.Register(
                themeManagerBuilder,
                new DesktopControlThemesProvider(),
                selectAssets: DesktopControlThemeAssetSelector.SelectNative);
        }
        else
        {
            GeneratedControlPackageRegistration.Register(
                themeManagerBuilder,
                new BrowserDesktopControlThemesProvider(),
                DesktopControlThemeAssetSelector.IsBrowserControlSupported,
                DesktopControlThemeAssetSelector.SelectBrowser);
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

}
