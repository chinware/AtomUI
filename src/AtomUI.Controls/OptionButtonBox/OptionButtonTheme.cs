using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class OptionButtonTheme : BaseControlTheme
{
    public OptionButtonTheme()
        : base(typeof(OptionButton))
    {
    }

    protected override IControlTemplate? BuildControlTemplate()
    {
        return new FuncControlTemplate<OptionButton>((button, scope) =>
        {
            // TODO 暂时没有支持带 Icon，后面支持
            var stackPanel = new StackPanel
            {
                Orientation         = Orientation.Horizontal,
                VerticalAlignment   = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            var label = new Label
            {
                Padding             = new Thickness(0),
                VerticalAlignment   = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            stackPanel.Children.Add(label);
            CreateTemplateParentBinding(label, ContentControl.ContentProperty, OptionButton.TextProperty);
            return stackPanel;
        });
    }

    protected override void BuildStyles()
    {
        var largeSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(OptionButton.SizeTypeProperty, SizeType.Large));
        largeSizeStyle.Add(Layoutable.MinHeightProperty, SharedTokenKey.ControlHeightLG);
        largeSizeStyle.Add(TemplatedControl.FontSizeProperty, OptionButtonTokenKey.ContentFontSizeLG);
        largeSizeStyle.Add(TemplatedControl.PaddingProperty, OptionButtonTokenKey.PaddingLG);
        largeSizeStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusLG);
        Add(largeSizeStyle);

        var middleSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(OptionButton.SizeTypeProperty, SizeType.Middle));
        middleSizeStyle.Add(Layoutable.MinHeightProperty, SharedTokenKey.ControlHeight);
        middleSizeStyle.Add(TemplatedControl.FontSizeProperty, OptionButtonTokenKey.ContentFontSize);
        middleSizeStyle.Add(TemplatedControl.PaddingProperty, OptionButtonTokenKey.Padding);
        middleSizeStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadius);
        Add(middleSizeStyle);

        var smallSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(OptionButton.SizeTypeProperty, SizeType.Small));
        smallSizeStyle.Add(Layoutable.MinHeightProperty, SharedTokenKey.ControlHeightSM);
        smallSizeStyle.Add(TemplatedControl.FontSizeProperty, OptionButtonTokenKey.ContentFontSizeSM);
        smallSizeStyle.Add(TemplatedControl.PaddingProperty, OptionButtonTokenKey.PaddingSM);
        smallSizeStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusSM);

        Add(smallSizeStyle);

        BuildSolidStyle();
        BuildOutlineStyle();
    }

    private void BuildSolidStyle()
    {
        var solidStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(OptionButton.ButtonStyleProperty, OptionButtonStyle.Solid));
        BuildSolidEnabledStyle(solidStyle);
        BuildSolidDisabledStyle(solidStyle);
        Add(solidStyle);
    }

    private void BuildOutlineStyle()
    {
        var outlineStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(OptionButton.ButtonStyleProperty, OptionButtonStyle.Outline));
        outlineStyle.Add(TemplatedControl.BorderThicknessProperty, SharedTokenKey.BorderThickness);
        var inOptionGroupStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(OptionButton.InOptionGroupProperty, true));
        inOptionGroupStyle.Add(TemplatedControl.BorderThicknessProperty, new Thickness(0));
        outlineStyle.Add(inOptionGroupStyle);
        BuildOutlineEnabledStyle(outlineStyle);
        BuildOutlineDisabledStyle(outlineStyle);
        Add(outlineStyle);
    }

    private void BuildSolidEnabledStyle(Style solidStyle)
    {
        var enabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, true));
        // 选中状态
        {
            var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
            checkedStyle.Add(TemplatedControl.ForegroundProperty, OptionButtonTokenKey.ButtonSolidCheckedColor);
            checkedStyle.Add(TemplatedControl.BackgroundProperty,
                OptionButtonTokenKey.ButtonSolidCheckedBackground);

            // Hover 状态
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.BackgroundProperty,
                OptionButtonTokenKey.ButtonSolidCheckedHoverBackground);
            checkedStyle.Add(hoverStyle);

            // Pressed 状态
            var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.BackgroundProperty,
                OptionButtonTokenKey.ButtonSolidCheckedActiveBackground);
            checkedStyle.Add(pressedStyle);

            enabledStyle.Add(checkedStyle);
        }

        // 没选中状态
        {
            var unCheckedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.UnChecked));
            unCheckedStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorText);
            unCheckedStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorBgContainer);
            var inOptionGroupStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(OptionButton.InOptionGroupProperty, true));
            inOptionGroupStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorTransparent);

            unCheckedStyle.Add(inOptionGroupStyle);

            // Hover 状态
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorPrimaryHover);
            unCheckedStyle.Add(hoverStyle);

            // Pressed 状态
            var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorPrimaryActive);
            unCheckedStyle.Add(pressedStyle);

            enabledStyle.Add(unCheckedStyle);
        }

        solidStyle.Add(enabledStyle);
    }

    private void BuildSolidDisabledStyle(Style solidStyle)
    {
        var disabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, false));

        disabledStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorBorder);
        disabledStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextDisabled);
        disabledStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorBgContainerDisabled);

        var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
        checkedStyle.Add(TemplatedControl.ForegroundProperty, OptionButtonTokenKey.ButtonCheckedColorDisabled);
        checkedStyle.Add(TemplatedControl.BackgroundProperty, OptionButtonTokenKey.ButtonCheckedColorDisabled);
        disabledStyle.Add(checkedStyle);

        solidStyle.Add(disabledStyle);
    }

    private void BuildOutlineEnabledStyle(Style outlineStyle)
    {
        var enabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, true));

        // 选中状态
        {
            var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
            checkedStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorPrimary);
            checkedStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorPrimary);
            checkedStyle.Add(TemplatedControl.BackgroundProperty, OptionButtonTokenKey.ButtonBackground);
            var inOptionGroupStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(OptionButton.InOptionGroupProperty, true));
            inOptionGroupStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorTransparent);

            checkedStyle.Add(inOptionGroupStyle);

            enabledStyle.Add(checkedStyle);
            outlineStyle.Add(enabledStyle);
        }
        // 没选中状态
        {
            enabledStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorBorder);
            enabledStyle.Add(TemplatedControl.ForegroundProperty, OptionButtonTokenKey.ButtonColor);
            enabledStyle.Add(TemplatedControl.BackgroundProperty, OptionButtonTokenKey.ButtonCheckedBackground);

            var inOptionGroupStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(OptionButton.InOptionGroupProperty, true));
            inOptionGroupStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorTransparent);
            enabledStyle.Add(inOptionGroupStyle);
        }

        // Hover 状态
        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        hoverStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorPrimaryHover);
        hoverStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorPrimaryHover);
        enabledStyle.Add(hoverStyle);

        // Pressed 状态
        var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
        pressedStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorPrimaryActive);
        pressedStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorPrimaryActive);
        enabledStyle.Add(pressedStyle);
    }

    private void BuildOutlineDisabledStyle(Style outlineStyle)
    {
        var disabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, false));

        disabledStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorBorder);
        disabledStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextDisabled);
        disabledStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorBgContainerDisabled);

        var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
        checkedStyle.Add(TemplatedControl.ForegroundProperty, OptionButtonTokenKey.ButtonCheckedColorDisabled);
        checkedStyle.Add(TemplatedControl.BackgroundProperty, OptionButtonTokenKey.ButtonCheckedBgDisabled);
        disabledStyle.Add(checkedStyle);

        outlineStyle.Add(disabledStyle);
    }
}