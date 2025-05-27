using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal abstract class BasePrimaryButtonTheme : BaseButtonTheme
{
    public BasePrimaryButtonTheme(Type targetType) : base(targetType)
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
        enabledStyle.Add(TemplatedControl.ForegroundProperty, ButtonTokenKey.PrimaryColor);
        
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorPrimary);
            enabledStyle.Add(frameStyle);
        }

        // 正常 hover
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
                frameStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorPrimaryHover);
                hoverStyle.Add(frameStyle);
            }
            enabledStyle.Add(hoverStyle);
        }

        // 正常按下
        {
            var pressedStyle = new Style(selector =>
                selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
            {
                var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
                frameStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorPrimaryActive);
                pressedStyle.Add(frameStyle);
            }
            enabledStyle.Add(pressedStyle);
        }

        // 危险按钮状态
        var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorError);
            dangerStyle.Add(frameStyle);
        }

        // 危险状态 hover
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
                frameStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorErrorHover);
                hoverStyle.Add(frameStyle);
            }
            dangerStyle.Add(hoverStyle);
        }

        // 危险状态按下
        {
            var pressedStyle = new Style(selector =>
                selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
            {
                var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
                frameStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorErrorActive);
                pressedStyle.Add(frameStyle);
            }
            dangerStyle.Add(pressedStyle);
        }
        enabledStyle.Add(dangerStyle);
        Add(enabledStyle);

        BuildEnabledGhostStyle();
    }

    private void BuildEnabledGhostStyle()
    {
        var ghostStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsGhostProperty, true));
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Border.BackgroundProperty, Brushes.Transparent);
            ghostStyle.Add(frameStyle);
        }
        // 正常状态
        ghostStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorPrimary);
        ghostStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorPrimary);

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
        dangerStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorError);
        dangerStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorError);

        // 危险按钮状态 hover
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorErrorBorderHover);
            hoverStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorErrorBorderHover);
            dangerStyle.Add(hoverStyle);
        }

        // 危险状态按下
        {
            var pressedStyle = new Style(selector =>
                selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorErrorActive);
            pressedStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorErrorActive);
            dangerStyle.Add(pressedStyle);
        }

        ghostStyle.Add(dangerStyle);
        Add(ghostStyle);
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextDisabled);
        disabledStyle.Add(TemplatedControl.BorderBrushProperty, ButtonTokenKey.BorderColorDisabled);
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorBgContainerDisabled);
            disabledStyle.Add(frameStyle);
        }
        Add(disabledStyle);
    }
}

[ControlThemeProvider]
internal class PrimaryButtonTheme : BasePrimaryButtonTheme
{
    public const string ID = "PrimaryButton";

    public PrimaryButtonTheme()
        : base(typeof(Button))
    {
    }

    public override object ThemeResourceKey()
    {
        return ID;
    }
}