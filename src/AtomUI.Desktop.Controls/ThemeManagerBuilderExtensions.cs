using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.MotionScene;
using AtomUI.Theme;
using AtomUI.Theme.Language;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;
using Avalonia.Media.Transformation;

namespace AtomUI.Desktop.Controls
{
    public static class ThemeManagerBuilderExtensions
    {
        public static IThemeManagerBuilder UseDesktopControls(this IThemeManagerBuilder themeManagerBuilder)
        {
            var controlTokenTypes = ControlTokenTypePool.GetTokenTypes();
            foreach (var controlType in controlTokenTypes)
            {
                themeManagerBuilder.AddControlToken(controlType);
            }
            themeManagerBuilder.AddControlThemesProvider(new AtomUIOSSControlThemesProvider());

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
            AvaloniaLocator.CurrentMutable.BindToSelf(new ToolTipService());
            var colorTextTertiary = TokenResourceUtils.FindGlobalTokenResource(SharedTokenKey.ColorTextTertiary);
            if (colorTextTertiary is IBrush defaultFilledColor)
            {
                IconProvider.DefaultFilledColor = defaultFilledColor;
            }

            var colorInfoText = TokenResourceUtils.FindGlobalTokenResource(SharedTokenKey.ColorTextTertiary);
            var colorInfoBg   = TokenResourceUtils.FindGlobalTokenResource(SharedTokenKey.ColorInfoBg);

            if (colorInfoText is IBrush primaryFilledColor)
            {
                IconProvider.DefaultPrimaryFilledColor = primaryFilledColor;
            }

            if (colorInfoBg is IBrush secondaryFilledColor)
            {
                IconProvider.DefaultSecondaryFilledColor = secondaryFilledColor;
            }
        }
    }
}