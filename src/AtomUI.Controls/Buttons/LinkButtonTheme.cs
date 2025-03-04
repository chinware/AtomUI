using AtomUI.Theme.Styling;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal abstract class BaseLinkButtonTheme : BaseButtonTheme
{
    public BaseLinkButtonTheme(Type targetType) : base(targetType)
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
        enabledStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorLink);

        // 正常 hover
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorLinkHover);
            enabledStyle.Add(hoverStyle);
        }
        // 正常按下
        {
            var pressedStyle = new Style(selector =>
                selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorLinkActive);
            enabledStyle.Add(pressedStyle);
        }

        // 危险按钮状态
        var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
        dangerStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorError);

        // 危险状态 hover
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorErrorHover);
            dangerStyle.Add(hoverStyle);
        }

        // 危险状态按下
        {
            var pressedStyle = new Style(selector =>
                selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorErrorActive);
            dangerStyle.Add(pressedStyle);
        }
        enabledStyle.Add(dangerStyle);

        Add(enabledStyle);

        BuildEnabledGhostStyle();
    }

    private void BuildEnabledGhostStyle()
    {
        var ghostStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsGhostProperty, true));
        // 正常状态
        ghostStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorTransparent);

        Add(ghostStyle);
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextDisabled);
        Add(disabledStyle);
    }
}

[ControlThemeProvider]
internal class LinkButtonTheme : BaseLinkButtonTheme
{
    public const string ID = "LinkButton";

    public LinkButtonTheme()
        : base(typeof(Button))
    {
    }
    
    public override string ThemeResourceKey()
    {
        return ID;
    }
}