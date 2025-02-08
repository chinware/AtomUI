using AtomUI.Controls.Utils;
using AtomUI.MotionScene;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;
using Avalonia.Media.Transformation;

namespace AtomUI.Controls;

internal static class ThemeManagerExtensions
{
    public static ThemeManager ConfigureAtomUIControls(this ThemeManager themeManager)
    {
        Animation.RegisterCustomAnimator<TransformOperations, MotionTransformOptionsAnimator>();
        AvaloniaLocator.CurrentMutable.BindToSelf(new ToolTipService());
        ControlThemeRegister.Register();
        ControlTokenTypeRegister.Register();
        LanguageProviderRegister.Register();
        return themeManager;
    }

    public static ThemeManager ThemeInitialized(this ThemeManager themeManager)
    {
        var colorTextTertiary = TokenResourceUtils.FindSharedTokenResource(SharedTokenKey.ColorTextTertiary);
        if (colorTextTertiary is IBrush defaultFilledColor)
        {
            IconProvider.DefaultFilledColor = defaultFilledColor;
        }

        var colorInfoText = TokenResourceUtils.FindSharedTokenResource(SharedTokenKey.ColorTextTertiary);
        var colorInfoBg   = TokenResourceUtils.FindSharedTokenResource(SharedTokenKey.ColorInfoBg);

        if (colorInfoText is IBrush primaryFilledColor)
        {
            IconProvider.DefaultPrimaryFilledColor = primaryFilledColor;
        }

        if (colorInfoBg is IBrush secondaryFilledColor)
        {
            IconProvider.DefaultSecondaryFilledColor = secondaryFilledColor;
        }
        return themeManager;
    }
}