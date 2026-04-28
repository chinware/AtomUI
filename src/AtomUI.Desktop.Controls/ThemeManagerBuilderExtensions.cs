using System.Diagnostics;
using AtomUI.Controls;
using AtomUI.MotionScene;
using AtomUI.Theme;
using AtomUI.Theme.Language;
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
        var controlTokenTypes = ControlTokenTypePool.GetTokenTypes();
        foreach (var controlType in controlTokenTypes)
        {
            themeManagerBuilder.AddControlToken(controlType);
        }
        themeManagerBuilder.AddControlThemesProvider(new DesktopControlThemesProvider());

        var languageProviders = LanguageProviderPool.GetLanguageProviders();
        foreach (var languageProvider in languageProviders)
        {
            themeManagerBuilder.AddLanguageProviders(languageProvider);
        }

        themeManagerBuilder.InitializedHandlers.Add(HandleThemeManagerInitialized);

        return themeManagerBuilder;
    }

    private static void HandleThemeManagerInitialized(object? sender, EventArgs e)
    {
        Animation.RegisterCustomAnimator<TransformOperations, MotionTransformOptionsAnimator>();
        var inputManager = AvaloniaLocator.CurrentMutable.GetService<IInputManager>();
        Debug.Assert(inputManager != null);
        AvaloniaLocator.CurrentMutable.BindToSelf(new ToolTipService(inputManager));
    }
}