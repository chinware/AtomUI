using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal abstract class BaseTextButtonTheme : BaseButtonTheme
{
    
    public BaseTextButtonTheme(Type targetType) : base(targetType)
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
        enabledStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorTransparent);
        enabledStyle.Add(TemplatedControl.ForegroundProperty, ButtonTokenKey.DefaultColor);

        // 正常 hover
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.BackgroundProperty, ButtonTokenKey.TextHoverBg);
            enabledStyle.Add(hoverStyle);
        }
        // 正常按下
        {
            var pressedStyle = new Style(selector =>
                selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorBgTextActive);
            enabledStyle.Add(pressedStyle);
        }

        // 危险按钮状态
        var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
        dangerStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorError);

        // 危险状态 hover
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorErrorBgHover);
            dangerStyle.Add(hoverStyle);
        }

        // 危险状态按下
        {
            var pressedStyle = new Style(selector =>
                selector.Nesting().Class(StdPseudoClass.PointerOver).Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorErrorBgActive);
            dangerStyle.Add(pressedStyle);
        }
        enabledStyle.Add(dangerStyle);

        Add(enabledStyle);
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextDisabled);
        Add(disabledStyle);
    }
}


internal class TextButtonTheme : BaseTextButtonTheme
{
    public const string ID = "TextButton";

    public TextButtonTheme()
        : base(typeof(Button))
    {
    }

    public override object ThemeResourceKey()
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
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Border.BackgroundProperty, Brushes.Transparent);
            enabledStyle.Add(frameStyle);
        }
        enabledStyle.Add(TemplatedControl.ForegroundProperty, ButtonTokenKey.DefaultColor);

        // 正常 hover
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
                frameStyle.Add(Border.BackgroundProperty, ButtonTokenKey.TextHoverBg);
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
                frameStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorBgTextActive);
                pressedStyle.Add(frameStyle);
            }
            enabledStyle.Add(pressedStyle);
        }

        // 危险按钮状态
        var dangerStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsDangerProperty, true));
        dangerStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorError);

        // 危险状态 hover
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
                frameStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorErrorBgHover);
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
                frameStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorErrorBgActive);
                pressedStyle.Add(frameStyle);
            }
            dangerStyle.Add(pressedStyle);
        }
        enabledStyle.Add(dangerStyle);

        Add(enabledStyle);
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextDisabled);
        Add(disabledStyle);
    }
}