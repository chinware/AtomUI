using AtomUI.Theme.Styling;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DefaultButtonTheme : BaseButtonTheme
{
    public const string ID = "DefaultButton";

    public DefaultButtonTheme()
        : base(typeof(Button))
    {
    }

    public override string? ThemeResourceKey()
    {
        return ID;
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
        enabledStyle.Add(TemplatedControl.BackgroundProperty, ButtonTokenResourceKey.DefaultBg);
        enabledStyle.Add(TemplatedControl.BorderBrushProperty, ButtonTokenResourceKey.DefaultBorderColor);
        enabledStyle.Add(TemplatedControl.ForegroundProperty, ButtonTokenResourceKey.DefaultColor);

        // 正常 hover
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.BorderBrushProperty, ButtonTokenResourceKey.DefaultHoverBorderColor);
            hoverStyle.Add(TemplatedControl.ForegroundProperty, ButtonTokenResourceKey.DefaultHoverColor);
            enabledStyle.Add(hoverStyle);
        }
        // 正常按下
        {
            var pressedStyle = new Style(selector =>
                selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.BorderBrushProperty, ButtonTokenResourceKey.DefaultActiveBorderColor);
            pressedStyle.Add(TemplatedControl.ForegroundProperty, ButtonTokenResourceKey.DefaultActiveColor);
            enabledStyle.Add(pressedStyle);
        }

        // 危险按钮状态
        var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
        dangerStyle.Add(TemplatedControl.BorderBrushProperty, DesignTokenKey.ColorError);
        dangerStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorError);

        // 危险状态 hover
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.BorderBrushProperty, DesignTokenKey.ColorErrorBorderHover);
            hoverStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorErrorBorderHover);
            dangerStyle.Add(hoverStyle);
        }

        // 危险状态按下
        {
            var pressedStyle = new Style(selector =>
                selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.BorderBrushProperty, DesignTokenKey.ColorErrorActive);
            pressedStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorErrorActive);
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
        ghostStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorTextLightSolid);
        ghostStyle.Add(TemplatedControl.BorderBrushProperty, DesignTokenKey.ColorTextLightSolid);
        ghostStyle.Add(TemplatedControl.BackgroundProperty, DesignTokenKey.ColorTransparent);

        // 正常 hover
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorPrimaryHover);
            hoverStyle.Add(TemplatedControl.BorderBrushProperty, DesignTokenKey.ColorPrimaryHover);
            ghostStyle.Add(hoverStyle);
        }

        // 正常按下
        {
            var pressedStyle = new Style(selector =>
                selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorPrimaryActive);
            pressedStyle.Add(TemplatedControl.BorderBrushProperty, DesignTokenKey.ColorPrimaryActive);
            ghostStyle.Add(pressedStyle);
        }

        // 危险按钮状态
        var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
        dangerStyle.Add(TemplatedControl.BackgroundProperty, DesignTokenKey.ColorTransparent);
        dangerStyle.Add(TemplatedControl.BorderBrushProperty, DesignTokenKey.ColorError);
        dangerStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorError);

        // 危险状态 hover
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.BorderBrushProperty, DesignTokenKey.ColorErrorBorderHover);
            hoverStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorErrorBorderHover);
            dangerStyle.Add(hoverStyle);
        }

        // 危险状态按下
        {
            var pressedStyle = new Style(selector =>
                selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.BorderBrushProperty, DesignTokenKey.ColorErrorActive);
            pressedStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorErrorActive);
            dangerStyle.Add(pressedStyle);
        }
        ghostStyle.Add(dangerStyle);
        enabledStyle.Add(ghostStyle);
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorTextDisabled);
        disabledStyle.Add(TemplatedControl.BorderBrushProperty, ButtonTokenResourceKey.BorderColorDisabled);
        disabledStyle.Add(TemplatedControl.BackgroundProperty, DesignTokenKey.ColorBgContainerDisabled);
        Add(disabledStyle);
    }
}