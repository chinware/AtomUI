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
        largeSizeStyle.Add(Layoutable.MinHeightProperty, DesignTokenKey.ControlHeightLG);
        largeSizeStyle.Add(TemplatedControl.FontSizeProperty, OptionButtonTokenResourceKey.ContentFontSizeLG);
        largeSizeStyle.Add(TemplatedControl.PaddingProperty, OptionButtonTokenResourceKey.PaddingLG);
        largeSizeStyle.Add(TemplatedControl.CornerRadiusProperty, DesignTokenKey.BorderRadiusLG);
        Add(largeSizeStyle);

        var middleSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(OptionButton.SizeTypeProperty, SizeType.Middle));
        middleSizeStyle.Add(Layoutable.MinHeightProperty, DesignTokenKey.ControlHeight);
        middleSizeStyle.Add(TemplatedControl.FontSizeProperty, OptionButtonTokenResourceKey.ContentFontSize);
        middleSizeStyle.Add(TemplatedControl.PaddingProperty, OptionButtonTokenResourceKey.Padding);
        middleSizeStyle.Add(TemplatedControl.CornerRadiusProperty, DesignTokenKey.BorderRadius);
        Add(middleSizeStyle);

        var smallSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(OptionButton.SizeTypeProperty, SizeType.Small));
        smallSizeStyle.Add(Layoutable.MinHeightProperty, DesignTokenKey.ControlHeightSM);
        smallSizeStyle.Add(TemplatedControl.FontSizeProperty, OptionButtonTokenResourceKey.ContentFontSizeSM);
        smallSizeStyle.Add(TemplatedControl.PaddingProperty, OptionButtonTokenResourceKey.PaddingSM);
        smallSizeStyle.Add(TemplatedControl.CornerRadiusProperty, DesignTokenKey.BorderRadiusSM);

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
        outlineStyle.Add(TemplatedControl.BorderThicknessProperty, DesignTokenKey.BorderThickness);
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
            checkedStyle.Add(TemplatedControl.ForegroundProperty, OptionButtonTokenResourceKey.ButtonSolidCheckedColor);
            checkedStyle.Add(TemplatedControl.BackgroundProperty,
                OptionButtonTokenResourceKey.ButtonSolidCheckedBackground);

            // Hover 状态
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.BackgroundProperty,
                OptionButtonTokenResourceKey.ButtonSolidCheckedHoverBackground);
            checkedStyle.Add(hoverStyle);

            // Pressed 状态
            var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.BackgroundProperty,
                OptionButtonTokenResourceKey.ButtonSolidCheckedActiveBackground);
            checkedStyle.Add(pressedStyle);

            enabledStyle.Add(checkedStyle);
        }

        // 没选中状态
        {
            var unCheckedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.UnChecked));
            unCheckedStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorText);
            unCheckedStyle.Add(TemplatedControl.BackgroundProperty, DesignTokenKey.ColorBgContainer);
            var inOptionGroupStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(OptionButton.InOptionGroupProperty, true));
            inOptionGroupStyle.Add(TemplatedControl.BackgroundProperty, DesignTokenKey.ColorTransparent);

            unCheckedStyle.Add(inOptionGroupStyle);

            // Hover 状态
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorPrimaryHover);
            unCheckedStyle.Add(hoverStyle);

            // Pressed 状态
            var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorPrimaryActive);
            unCheckedStyle.Add(pressedStyle);

            enabledStyle.Add(unCheckedStyle);
        }

        solidStyle.Add(enabledStyle);
    }

    private void BuildSolidDisabledStyle(Style solidStyle)
    {
        var disabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, false));

        disabledStyle.Add(TemplatedControl.BorderBrushProperty, DesignTokenKey.ColorBorder);
        disabledStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorTextDisabled);
        disabledStyle.Add(TemplatedControl.BackgroundProperty, DesignTokenKey.ColorBgContainerDisabled);

        var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
        checkedStyle.Add(TemplatedControl.ForegroundProperty, OptionButtonTokenResourceKey.ButtonCheckedColorDisabled);
        checkedStyle.Add(TemplatedControl.BackgroundProperty, OptionButtonTokenResourceKey.ButtonCheckedColorDisabled);
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
            checkedStyle.Add(TemplatedControl.BorderBrushProperty, DesignTokenKey.ColorPrimary);
            checkedStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorPrimary);
            checkedStyle.Add(TemplatedControl.BackgroundProperty, OptionButtonTokenResourceKey.ButtonBackground);
            var inOptionGroupStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(OptionButton.InOptionGroupProperty, true));
            inOptionGroupStyle.Add(TemplatedControl.BackgroundProperty, DesignTokenKey.ColorTransparent);

            checkedStyle.Add(inOptionGroupStyle);

            enabledStyle.Add(checkedStyle);
            outlineStyle.Add(enabledStyle);
        }
        // 没选中状态
        {
            enabledStyle.Add(TemplatedControl.BorderBrushProperty, DesignTokenKey.ColorBorder);
            enabledStyle.Add(TemplatedControl.ForegroundProperty, OptionButtonTokenResourceKey.ButtonColor);
            enabledStyle.Add(TemplatedControl.BackgroundProperty, OptionButtonTokenResourceKey.ButtonCheckedBackground);

            var inOptionGroupStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(OptionButton.InOptionGroupProperty, true));
            inOptionGroupStyle.Add(TemplatedControl.BackgroundProperty, DesignTokenKey.ColorTransparent);
            enabledStyle.Add(inOptionGroupStyle);
        }

        // Hover 状态
        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        hoverStyle.Add(TemplatedControl.BorderBrushProperty, DesignTokenKey.ColorPrimaryHover);
        hoverStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorPrimaryHover);
        enabledStyle.Add(hoverStyle);

        // Pressed 状态
        var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
        pressedStyle.Add(TemplatedControl.BorderBrushProperty, DesignTokenKey.ColorPrimaryActive);
        pressedStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorPrimaryActive);
        enabledStyle.Add(pressedStyle);
    }

    private void BuildOutlineDisabledStyle(Style outlineStyle)
    {
        var disabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, false));

        disabledStyle.Add(TemplatedControl.BorderBrushProperty, DesignTokenKey.ColorBorder);
        disabledStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorTextDisabled);
        disabledStyle.Add(TemplatedControl.BackgroundProperty, DesignTokenKey.ColorBgContainerDisabled);

        var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
        checkedStyle.Add(TemplatedControl.ForegroundProperty, OptionButtonTokenResourceKey.ButtonCheckedColorDisabled);
        checkedStyle.Add(TemplatedControl.BackgroundProperty, OptionButtonTokenResourceKey.ButtonCheckedBgDisabled);
        disabledStyle.Add(checkedStyle);

        outlineStyle.Add(disabledStyle);
    }
}