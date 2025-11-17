using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.MotionScene;
using AtomUI.Theme;
using AtomUI.Theme.Language;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;
using Avalonia.Media.Transformation;

namespace AtomUI.Controls
{
    public static class ThemeManagerBuilderExtensions
    {
        public static IThemeManagerBuilder UseOSSControls(this IThemeManagerBuilder themeManagerBuilder)
        {
            // ControlTheme 必须在 UI 线程创建好了才能实例化，不然会炸，切记切记
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