﻿using AtomUI.Theme.Styling;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;
using Colors = Avalonia.Media.Colors;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class PrimaryButtonTheme : BaseButtonTheme
{
    public const string ID = "PrimaryButton";

    public PrimaryButtonTheme()
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
        enabledStyle.Add(TemplatedControl.ForegroundProperty, ButtonTokenResourceKey.PrimaryColor);
        enabledStyle.Add(TemplatedControl.BackgroundProperty, DesignTokenKey.ColorPrimary);

        // 正常 hover
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.BackgroundProperty, DesignTokenKey.ColorPrimaryHover);
            enabledStyle.Add(hoverStyle);
        }

        // 正常按下
        {
            var pressedStyle = new Style(selector =>
                selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.BackgroundProperty, DesignTokenKey.ColorPrimaryActive);
            enabledStyle.Add(pressedStyle);
        }

        // 危险按钮状态
        var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
        dangerStyle.Add(TemplatedControl.BackgroundProperty, DesignTokenKey.ColorError);

        // 危险状态 hover
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.BackgroundProperty, DesignTokenKey.ColorErrorHover);
            dangerStyle.Add(hoverStyle);
        }

        // 危险状态按下
        {
            var pressedStyle = new Style(selector =>
                selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.BackgroundProperty, DesignTokenKey.ColorErrorActive);
            dangerStyle.Add(pressedStyle);
        }
        enabledStyle.Add(dangerStyle);
        Add(enabledStyle);

        BuildEnabledGhostStyle();
    }

    private void BuildEnabledGhostStyle()
    {
        var ghostStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsGhostProperty, true));
        ghostStyle.Add(TemplatedControl.BackgroundProperty, new SolidColorBrush(Colors.Transparent));
        // 正常状态
        ghostStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorPrimary);
        ghostStyle.Add(TemplatedControl.BorderBrushProperty, DesignTokenKey.ColorPrimary);

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
        dangerStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorError);
        dangerStyle.Add(TemplatedControl.BorderBrushProperty, DesignTokenKey.ColorError);

        // 危险按钮状态 hover
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorErrorBorderHover);
            hoverStyle.Add(TemplatedControl.BorderBrushProperty, DesignTokenKey.ColorErrorBorderHover);
            dangerStyle.Add(hoverStyle);
        }

        // 危险状态按下
        {
            var pressedStyle = new Style(selector =>
                selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorErrorActive);
            pressedStyle.Add(TemplatedControl.BorderBrushProperty, DesignTokenKey.ColorErrorActive);
            dangerStyle.Add(pressedStyle);
        }

        ghostStyle.Add(dangerStyle);
        Add(ghostStyle);
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