using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class CheckBoxTheme : BaseControlTheme
{
    public CheckBoxTheme()
        : base(typeof(CheckBox))
    {
    }

    protected override void BuildStyles()
    {
        this.Add(CheckBox.CheckIndicatorSizeProperty, CheckBoxTokenResourceKey.CheckIndicatorSize);
        this.Add(CheckBox.PaddingInlineProperty, GlobalTokenResourceKey.PaddingXS);
        this.Add(CheckBox.IndicatorBorderRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
        this.Add(CheckBox.IndicatorTristateMarkSizeProperty, CheckBoxTokenResourceKey.IndicatorTristateMarkSize);
        this.Add(CheckBox.IndicatorTristateMarkBrushProperty, GlobalTokenResourceKey.ColorPrimary);
        BuildEnabledStyle();
        BuildDisabledStyle();
    }

    private void BuildDisabledStyle()
    {
        var disableStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, false));
        disableStyle.Add(CheckBox.IndicatorBackgroundProperty, GlobalTokenResourceKey.ColorBgContainerDisabled);
        disableStyle.Add(CheckBox.IndicatorBorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
        disableStyle.Add(TemplatedControl.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);

        var checkedStyle =
            new Style(selector => selector.Nesting().PropertyEquals(ToggleButton.IsCheckedProperty, true));
        checkedStyle.Add(CheckBox.IndicatorCheckedMarkBrushProperty, GlobalTokenResourceKey.ColorTextDisabled);
        disableStyle.Add(checkedStyle);

        var indeterminateStyle =
            new Style(selector => selector.Nesting().PropertyEquals(ToggleButton.IsCheckedProperty, null));
        indeterminateStyle.Add(CheckBox.IndicatorTristateMarkBrushProperty, GlobalTokenResourceKey.ColorTextDisabled);
        disableStyle.Add(indeterminateStyle);
        Add(disableStyle);
    }

    private void BuildEnabledStyle()
    {
        var enabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, true));
        enabledStyle.Add(TemplatedControl.ForegroundProperty, GlobalTokenResourceKey.ColorText);
        enabledStyle.Add(CheckBox.IndicatorBackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
        enabledStyle.Add(CheckBox.IndicatorCheckedMarkBrushProperty, GlobalTokenResourceKey.ColorBgContainer);
        enabledStyle.Add(CheckBox.IndicatorBorderBrushProperty, GlobalTokenResourceKey.ColorBorder);

        // 选中
        var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
        checkedStyle.Add(CheckBox.IndicatorBackgroundProperty, GlobalTokenResourceKey.ColorPrimary);
        checkedStyle.Add(CheckBox.IndicatorBorderBrushProperty, GlobalTokenResourceKey.ColorPrimary);

        // 选中 hover
        var checkedHoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        checkedHoverStyle.Add(CheckBox.IndicatorBackgroundProperty, GlobalTokenResourceKey.ColorPrimaryHover);
        checkedHoverStyle.Add(CheckBox.IndicatorBorderBrushProperty, GlobalTokenResourceKey.ColorPrimaryHover);
        checkedStyle.Add(checkedHoverStyle);
        enabledStyle.Add(checkedStyle);

        // 没选中
        var unCheckedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        unCheckedStyle.Add(CheckBox.IndicatorBorderBrushProperty, GlobalTokenResourceKey.ColorPrimaryHover);
        enabledStyle.Add(unCheckedStyle);

        // 中间状态
        var indeterminateStyle = new Style(selector =>
            selector.Nesting().Class($"{StdPseudoClass.Indeterminate}{StdPseudoClass.PointerOver}"));
        indeterminateStyle.Add(CheckBox.IndicatorBorderBrushProperty, GlobalTokenResourceKey.ColorPrimaryHover);
        enabledStyle.Add(indeterminateStyle);

        Add(enabledStyle);
    }
}