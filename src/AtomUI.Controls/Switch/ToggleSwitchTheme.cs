using AtomUI.Controls.Switch;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ToggleSwitchTheme : BaseControlTheme
{
    public const string SwitchKnobPart = "PART_SwitchKnob";
    public const string MainContainerPart = "PART_MainContainer";

    public ToggleSwitchTheme()
        : base(typeof(ToggleSwitch))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
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
        this.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextLightSolid);
        this.Add(ToggleSwitch.TrackPaddingProperty, ToggleSwitchTokenKey.TrackPadding);
        BuildSizeTypeStyle();
        BuildCommonStyle();

        var disabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, false));
        disabledStyle.Add(ToggleSwitch.SwitchOpacityProperty, ToggleSwitchTokenKey.SwitchDisabledOpacity);
        Add(disabledStyle);

        var loadingStyle =
            new Style(selector => selector.Nesting().PropertyEquals(ToggleSwitch.IsLoadingProperty, true));
        loadingStyle.Add(ToggleSwitch.SwitchOpacityProperty, ToggleSwitchTokenKey.SwitchDisabledOpacity);
        Add(loadingStyle);
    }

    private void BuildCommonStyle()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(ToggleSwitch.SwitchOpacityProperty, 1d);
        commonStyle.Add(ToggleSwitch.HorizontalAlignmentProperty, HorizontalAlignment.Left);
        commonStyle.Add(ToggleSwitch.VerticalAlignmentProperty, VerticalAlignment.Top);
        
        var checkedStyle =
            new Style(selector => selector.Nesting().PropertyEquals(ToggleButton.IsCheckedProperty, true));
        checkedStyle.Add(ToggleSwitch.GrooveBackgroundProperty, ToggleSwitchTokenKey.SwitchColor);
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(ToggleSwitch.GrooveBackgroundProperty, SharedTokenKey.ColorPrimaryHover);
            checkedStyle.Add(hoverStyle);
        }
        commonStyle.Add(checkedStyle);

        var unCheckedStyle =
            new Style(selector => selector.Nesting().PropertyEquals(ToggleButton.IsCheckedProperty, false));
        unCheckedStyle.Add(ToggleSwitch.GrooveBackgroundProperty, SharedTokenKey.ColorTextQuaternary);
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(ToggleSwitch.GrooveBackgroundProperty, SharedTokenKey.ColorTextTertiary);
            unCheckedStyle.Add(hoverStyle);
        }
        commonStyle.Add(unCheckedStyle);
        Add(commonStyle);
    }

    private void BuildSizeTypeStyle()
    {
        var smallSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(ToggleSwitch.SizeTypeProperty, SizeType.Small));
        smallSizeStyle.Add(TemplatedControl.FontSizeProperty, ToggleSwitchTokenKey.ExtraInfoFontSizeSM);
        {
            var knobSizeStyle = new Style(selector => selector.Nesting().Template().OfType<SwitchKnob>());
            knobSizeStyle.Add(SwitchKnob.KnobSizeProperty, ToggleSwitchTokenKey.HandleSizeSM);
            knobSizeStyle.Add(SwitchKnob.OriginKnobSizeProperty, ToggleSwitchTokenKey.HandleSizeSM);
            smallSizeStyle.Add(knobSizeStyle);
        }
        smallSizeStyle.Add(ToggleSwitch.InnerMaxMarginProperty, ToggleSwitchTokenKey.InnerMaxMarginSM);
        smallSizeStyle.Add(ToggleSwitch.InnerMinMarginProperty, ToggleSwitchTokenKey.InnerMinMarginSM);
        smallSizeStyle.Add(ToggleSwitch.TrackHeightProperty, ToggleSwitchTokenKey.TrackHeightSM);
        smallSizeStyle.Add(ToggleSwitch.TrackMinWidthProperty, ToggleSwitchTokenKey.TrackMinWidthSM);
        smallSizeStyle.Add(ToggleSwitch.IconSizeProperty, ToggleSwitchTokenKey.IconSizeSM);

        Add(smallSizeStyle);

        var middleSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(ToggleSwitch.SizeTypeProperty, SizeType.Middle));
        middleSizeStyle.Add(TemplatedControl.FontSizeProperty, ToggleSwitchTokenKey.ExtraInfoFontSize);
        {
            var knobSizeStyle = new Style(selector => selector.Nesting().Template().OfType<SwitchKnob>());
            knobSizeStyle.Add(SwitchKnob.KnobSizeProperty, ToggleSwitchTokenKey.HandleSize);
            knobSizeStyle.Add(SwitchKnob.OriginKnobSizeProperty, ToggleSwitchTokenKey.HandleSize);
            middleSizeStyle.Add(knobSizeStyle);
        }
        middleSizeStyle.Add(ToggleSwitch.InnerMaxMarginProperty, ToggleSwitchTokenKey.InnerMaxMargin);
        middleSizeStyle.Add(ToggleSwitch.InnerMinMarginProperty, ToggleSwitchTokenKey.InnerMinMargin);
        middleSizeStyle.Add(ToggleSwitch.TrackHeightProperty, ToggleSwitchTokenKey.TrackHeight);
        middleSizeStyle.Add(ToggleSwitch.TrackMinWidthProperty, ToggleSwitchTokenKey.TrackMinWidth);
        middleSizeStyle.Add(ToggleSwitch.IconSizeProperty, ToggleSwitchTokenKey.IconSize);
        Add(middleSizeStyle);

        var largeSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(ToggleSwitch.SizeTypeProperty, SizeType.Large));
        largeSizeStyle.Add(TemplatedControl.FontSizeProperty, ToggleSwitchTokenKey.ExtraInfoFontSize);
        {
            var knobSizeStyle = new Style(selector => selector.Nesting().Template().OfType<SwitchKnob>());
            knobSizeStyle.Add(SwitchKnob.KnobSizeProperty, ToggleSwitchTokenKey.HandleSize);
            knobSizeStyle.Add(SwitchKnob.OriginKnobSizeProperty, ToggleSwitchTokenKey.HandleSize);
            largeSizeStyle.Add(knobSizeStyle);
        }
        largeSizeStyle.Add(ToggleSwitch.InnerMaxMarginProperty, ToggleSwitchTokenKey.InnerMaxMargin);
        largeSizeStyle.Add(ToggleSwitch.InnerMinMarginProperty, ToggleSwitchTokenKey.InnerMinMargin);
        largeSizeStyle.Add(ToggleSwitch.TrackHeightProperty, ToggleSwitchTokenKey.TrackHeight);
        largeSizeStyle.Add(ToggleSwitch.TrackMinWidthProperty, ToggleSwitchTokenKey.TrackMinWidth);
        largeSizeStyle.Add(ToggleSwitch.IconSizeProperty, ToggleSwitchTokenKey.IconSize);
        Add(largeSizeStyle);

        {
            var switchKnobStyle = new Style(selector => selector.Nesting().Template().OfType<SwitchKnob>());
            switchKnobStyle.Add(SwitchKnob.KnobBackgroundColorProperty, ToggleSwitchTokenKey.HandleBg);
            switchKnobStyle.Add(SwitchKnob.KnobBoxShadowProperty, ToggleSwitchTokenKey.HandleShadow);
            switchKnobStyle.Add(SwitchKnob.LoadIndicatorBrushProperty, SharedTokenKey.ColorTextQuaternary);
            var checkedStyle =
                new Style(selector => selector.Nesting().PropertyEquals(SwitchKnob.IsCheckedStateProperty, true));
            checkedStyle.Add(SwitchKnob.LoadIndicatorBrushProperty, ToggleSwitchTokenKey.SwitchColor);
            switchKnobStyle.Add(checkedStyle);
            Add(switchKnobStyle);
        }
    }
}