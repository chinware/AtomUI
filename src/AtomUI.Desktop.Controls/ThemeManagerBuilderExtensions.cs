using AtomUI.Controls;
using AtomUI.Generated.AtomUIDesktopControls;
using AtomUI.MotionScene;
using AtomUI.Registration;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Input;
using Avalonia.Media.Transformation;

namespace AtomUI.Desktop.Controls;

public static class ThemeManagerBuilderExtensions
{
    internal const string PackageId = "AtomUI.Desktop.Controls";

    public static IAtomUIBuilder UseDesktopControls(this IAtomUIBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        PrepareDesktopPackageCore(builder);
        if (AotTrimRegistration.IsEnabled)
        {
            RegisterGeneratedDesktopPackage(builder);
        }
        else
        {
            RegisterFullDesktopPackage(builder);
        }
        CompleteDesktopPackageCore(builder);
        return builder;
    }

    public static IAtomUIBuilder UseAllDesktopControls(this IAtomUIBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        PrepareDesktopPackageCore(builder);
        RegisterFullDesktopPackage(builder);
        CompleteDesktopPackageCore(builder);
        return builder;
    }

    private static void PrepareDesktopPackageCore(IAtomUIBuilder builder)
    {
        builder.UseCommonControls();
        DialogInputCaptureTracker.Initialize();
    }

    private static void CompleteDesktopPackageCore(IAtomUIBuilder builder)
    {
        GeneratedLanguageModuleRegistration.Register(builder.Localization);
        builder.Theme.AddInitializer(InitializeDesktopRuntime);
    }

    private static void RegisterGeneratedDesktopPackage(IAtomUIBuilder builder)
    {
        if (RuntimePlatform.Features.SupportsNativeWindow)
        {
            var provider = new DesktopControlThemesProvider();
            AotTrimRegistrationPlanRegistry.ApplyPackage(
                builder,
                PackageId,
                provider,
                selectAssets: DesktopControlThemeAssetSelector.SelectNative);
        }
        else
        {
            var provider = new BrowserDesktopControlThemesProvider();
            AotTrimRegistrationPlanRegistry.ApplyPackage(
                builder,
                PackageId,
                provider,
                DesktopControlThemeAssetSelector.IsBrowserControlSupported,
                DesktopControlThemeAssetSelector.SelectBrowser);
        }
    }

    private static void RegisterFullDesktopPackage(IAtomUIBuilder builder)
    {
        if (RuntimePlatform.Features.SupportsNativeWindow)
        {
            GeneratedControlPackageRegistration.Register(
                builder.Theme,
                new DesktopControlThemesProvider(),
                selectAssets: DesktopControlThemeAssetSelector.SelectNative);
        }
        else
        {
            GeneratedControlPackageRegistration.Register(
                builder.Theme,
                new BrowserDesktopControlThemesProvider(),
                DesktopControlThemeAssetSelector.IsBrowserControlSupported,
                DesktopControlThemeAssetSelector.SelectBrowser);
        }
    }

    private static void InitializeDesktopRuntime(IThemeManager manager)
    {
        Animation.RegisterCustomAnimator<TransformOperations, MotionTransformOptionsAnimator>();
        var inputManager = AvaloniaLocator.CurrentMutable.GetService(typeof(IInputManager)) as IInputManager;
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
