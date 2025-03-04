using AtomUI.Theme.Styling;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal abstract class BaseDefaultButtonTheme : BaseButtonTheme
{
    public BaseDefaultButtonTheme(Type targetType) : base(targetType)
    {
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        BuildEnabledStyle();
        BuildDisabledStyle();
    }

    private void BuildEnabledStyle()
    {
        var enabledStyle = new Style(selector => selector.Nesting());
        // 正常状态
        enabledStyle.Add(TemplatedControl.BackgroundProperty, ButtonTokenKey.DefaultBg);
        enabledStyle.Add(TemplatedControl.BorderBrushProperty, ButtonTokenKey.DefaultBorderColor);
        enabledStyle.Add(TemplatedControl.ForegroundProperty, ButtonTokenKey.DefaultColor);

        // 正常 hover
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.BorderBrushProperty, ButtonTokenKey.DefaultHoverBorderColor);
            hoverStyle.Add(TemplatedControl.ForegroundProperty, ButtonTokenKey.DefaultHoverColor);
            enabledStyle.Add(hoverStyle);
        }
        // 正常按下
        {
            var pressedStyle = new Style(selector =>
                selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.BorderBrushProperty, ButtonTokenKey.DefaultActiveBorderColor);
            pressedStyle.Add(TemplatedControl.ForegroundProperty, ButtonTokenKey.DefaultActiveColor);
            enabledStyle.Add(pressedStyle);
        }

        // 危险按钮状态
        var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
        dangerStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorError);
        dangerStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorError);

        // 危险状态 hover
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorErrorBorderHover);
            hoverStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorErrorBorderHover);
            dangerStyle.Add(hoverStyle);
        }

        // 危险状态按下
        {
            var pressedStyle = new Style(selector =>
                selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorErrorActive);
            pressedStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorErrorActive);
            dangerStyle.Add(pressedStyle);
        }
        enabledStyle.Add(dangerStyle);

        BuildEnabledGhostStyle(enabledStyle);
        Add(enabledStyle);
    }

    private void BuildEnabledGhostStyle(Style enabledStyle)
    {
        var ghostStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsGhostProperty, true));
        // 正常状态
        ghostStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextLightSolid);
        ghostStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorTextLightSolid);
        ghostStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorTransparent);

        // 正常 hover
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorPrimaryHover);
            hoverStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorPrimaryHover);
            ghostStyle.Add(hoverStyle);
        }

        // 正常按下
        {
            var pressedStyle = new Style(selector =>
                selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorPrimaryActive);
            pressedStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorPrimaryActive);
            ghostStyle.Add(pressedStyle);
        }

        // 危险按钮状态
        var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
        dangerStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorTransparent);
        dangerStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorError);
        dangerStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorError);

        // 危险状态 hover
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorErrorBorderHover);
            hoverStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorErrorBorderHover);
            dangerStyle.Add(hoverStyle);
        }

        // 危险状态按下
        {
            var pressedStyle = new Style(selector =>
                selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorErrorActive);
            pressedStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorErrorActive);
            dangerStyle.Add(pressedStyle);
        }
        ghostStyle.Add(dangerStyle);
        enabledStyle.Add(ghostStyle);
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextDisabled);
        disabledStyle.Add(TemplatedControl.BorderBrushProperty, ButtonTokenKey.BorderColorDisabled);
        disabledStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorBgContainerDisabled);
        Add(disabledStyle);
    }
}

[ControlThemeProvider]
internal class DefaultButtonTheme : BaseDefaultButtonTheme
{
    public const string ID = "DefaultButton";

    public DefaultButtonTheme()
        : base(typeof(Button))
    {
    }
    
    public DefaultButtonTheme(Type targetType) : base(targetType)
    {
    }

    public override string ThemeResourceKey()
    {
        return ID;
    }
}