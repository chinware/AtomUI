using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Styling;

namespace AtomUI.Controls.Switch;

[ControlThemeProvider]
internal class ToggleSwitchTheme : BaseControlTheme
{
    public const string SwitchKnobPart = "PART_SwitchKnob";
    public const string MainContainerPart = "PART_MainContainer";

    public ToggleSwitchTheme()
        : base(typeof(ToggleSwitch))
    {
    }

    protected override IControlTemplate? BuildControlTemplate()
    {
        return new FuncControlTemplate<ToggleSwitch>((toggleSwitch, scope) =>
        {
            var layout = new Canvas
            {
                Name = MainContainerPart
            };
            var switchKnob = new SwitchKnob
            {
                Name = SwitchKnobPart
            };
            CreateTemplateParentBinding(switchKnob, SwitchKnob.IsCheckedStateProperty, ToggleButton.IsCheckedProperty);
            switchKnob.RegisterInNameScope(scope);

            layout.Children.Add(switchKnob);
            layout.RegisterInNameScope(scope);
            return layout;
        });
    }

    protected override void BuildStyles()
    {
        this.Add(TemplatedControl.ForegroundProperty, GlobalTokenResourceKey.ColorTextLightSolid);
        this.Add(ToggleSwitch.TrackPaddingProperty, ToggleSwitchTokenResourceKey.TrackPadding);
        BuildSizeTypeStyle();
        BuildEnabledStyle();

        var disabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, false));
        disabledStyle.Add(ToggleSwitch.SwitchOpacityProperty, ToggleSwitchTokenResourceKey.SwitchDisabledOpacity);
        Add(disabledStyle);

        var loadingStyle =
            new Style(selector => selector.Nesting().PropertyEquals(ToggleSwitch.IsLoadingProperty, true));
        loadingStyle.Add(ToggleSwitch.SwitchOpacityProperty, ToggleSwitchTokenResourceKey.SwitchDisabledOpacity);
        Add(loadingStyle);
    }

    private void BuildEnabledStyle()
    {
        var enabledStyle = new Style(selector => selector.Nesting());
        enabledStyle.Add(ToggleSwitch.SwitchOpacityProperty, 1d);
        var checkedStyle =
            new Style(selector => selector.Nesting().PropertyEquals(ToggleButton.IsCheckedProperty, true));
        checkedStyle.Add(ToggleSwitch.GrooveBackgroundProperty, ToggleSwitchTokenResourceKey.SwitchColor);
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(ToggleSwitch.GrooveBackgroundProperty, GlobalTokenResourceKey.ColorPrimaryHover);
            checkedStyle.Add(hoverStyle);
        }
        enabledStyle.Add(checkedStyle);

        var unCheckedStyle =
            new Style(selector => selector.Nesting().PropertyEquals(ToggleButton.IsCheckedProperty, false));
        unCheckedStyle.Add(ToggleSwitch.GrooveBackgroundProperty, GlobalTokenResourceKey.ColorTextQuaternary);
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(ToggleSwitch.GrooveBackgroundProperty, GlobalTokenResourceKey.ColorTextTertiary);
            unCheckedStyle.Add(hoverStyle);
        }
        enabledStyle.Add(unCheckedStyle);
        Add(enabledStyle);
    }

    private void BuildSizeTypeStyle()
    {
        var smallSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(ToggleSwitch.SizeTypeProperty, SizeType.Small));
        smallSizeStyle.Add(TemplatedControl.FontSizeProperty, ToggleSwitchTokenResourceKey.ExtraInfoFontSizeSM);
        {
            var knobSizeStyle = new Style(selector => selector.Nesting().Template().OfType<SwitchKnob>());
            knobSizeStyle.Add(SwitchKnob.KnobSizeProperty, ToggleSwitchTokenResourceKey.HandleSizeSM);
            knobSizeStyle.Add(SwitchKnob.OriginKnobSizeProperty, ToggleSwitchTokenResourceKey.HandleSizeSM);
            smallSizeStyle.Add(knobSizeStyle);
        }
        smallSizeStyle.Add(ToggleSwitch.InnerMaxMarginProperty, ToggleSwitchTokenResourceKey.InnerMaxMarginSM);
        smallSizeStyle.Add(ToggleSwitch.InnerMinMarginProperty, ToggleSwitchTokenResourceKey.InnerMinMarginSM);
        smallSizeStyle.Add(ToggleSwitch.TrackHeightProperty, ToggleSwitchTokenResourceKey.TrackHeightSM);
        smallSizeStyle.Add(ToggleSwitch.TrackMinWidthProperty, ToggleSwitchTokenResourceKey.TrackMinWidthSM);
        smallSizeStyle.Add(ToggleSwitch.IconSizeProperty, ToggleSwitchTokenResourceKey.IconSizeSM);

        Add(smallSizeStyle);

        var middleSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(ToggleSwitch.SizeTypeProperty, SizeType.Middle));
        middleSizeStyle.Add(TemplatedControl.FontSizeProperty, ToggleSwitchTokenResourceKey.ExtraInfoFontSize);
        {
            var knobSizeStyle = new Style(selector => selector.Nesting().Template().OfType<SwitchKnob>());
            knobSizeStyle.Add(SwitchKnob.KnobSizeProperty, ToggleSwitchTokenResourceKey.HandleSize);
            knobSizeStyle.Add(SwitchKnob.OriginKnobSizeProperty, ToggleSwitchTokenResourceKey.HandleSize);
            middleSizeStyle.Add(knobSizeStyle);
        }
        middleSizeStyle.Add(ToggleSwitch.InnerMaxMarginProperty, ToggleSwitchTokenResourceKey.InnerMaxMargin);
        middleSizeStyle.Add(ToggleSwitch.InnerMinMarginProperty, ToggleSwitchTokenResourceKey.InnerMinMargin);
        middleSizeStyle.Add(ToggleSwitch.TrackHeightProperty, ToggleSwitchTokenResourceKey.TrackHeight);
        middleSizeStyle.Add(ToggleSwitch.TrackMinWidthProperty, ToggleSwitchTokenResourceKey.TrackMinWidth);
        middleSizeStyle.Add(ToggleSwitch.IconSizeProperty, ToggleSwitchTokenResourceKey.IconSize);
        Add(middleSizeStyle);

        var largeSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(ToggleSwitch.SizeTypeProperty, SizeType.Large));
        largeSizeStyle.Add(TemplatedControl.FontSizeProperty, ToggleSwitchTokenResourceKey.ExtraInfoFontSize);
        {
            var knobSizeStyle = new Style(selector => selector.Nesting().Template().OfType<SwitchKnob>());
            knobSizeStyle.Add(SwitchKnob.KnobSizeProperty, ToggleSwitchTokenResourceKey.HandleSize);
            knobSizeStyle.Add(SwitchKnob.OriginKnobSizeProperty, ToggleSwitchTokenResourceKey.HandleSize);
            largeSizeStyle.Add(knobSizeStyle);
        }
        largeSizeStyle.Add(ToggleSwitch.InnerMaxMarginProperty, ToggleSwitchTokenResourceKey.InnerMaxMargin);
        largeSizeStyle.Add(ToggleSwitch.InnerMinMarginProperty, ToggleSwitchTokenResourceKey.InnerMinMargin);
        largeSizeStyle.Add(ToggleSwitch.TrackHeightProperty, ToggleSwitchTokenResourceKey.TrackHeight);
        largeSizeStyle.Add(ToggleSwitch.TrackMinWidthProperty, ToggleSwitchTokenResourceKey.TrackMinWidth);
        largeSizeStyle.Add(ToggleSwitch.IconSizeProperty, ToggleSwitchTokenResourceKey.IconSize);
        Add(largeSizeStyle);

        {
            var switchKnobStyle = new Style(selector => selector.Nesting().Template().OfType<SwitchKnob>());
            switchKnobStyle.Add(SwitchKnob.KnobBackgroundColorProperty, ToggleSwitchTokenResourceKey.HandleBg);
            switchKnobStyle.Add(SwitchKnob.KnobBoxShadowProperty, ToggleSwitchTokenResourceKey.HandleShadow);
            switchKnobStyle.Add(SwitchKnob.LoadIndicatorBrushProperty, GlobalTokenResourceKey.ColorTextQuaternary);
            var checkedStyle =
                new Style(selector => selector.Nesting().PropertyEquals(SwitchKnob.IsCheckedStateProperty, true));
            checkedStyle.Add(SwitchKnob.LoadIndicatorBrushProperty, ToggleSwitchTokenResourceKey.SwitchColor);
            switchKnobStyle.Add(checkedStyle);
            Add(switchKnobStyle);
        }
    }
}