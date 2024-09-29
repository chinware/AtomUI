using AtomUI.Controls.Utils;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Animation;
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
}