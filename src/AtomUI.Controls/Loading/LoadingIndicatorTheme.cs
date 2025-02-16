using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class LoadingIndicatorTheme : BaseControlTheme
{
    public const string MainContainerPart = "PART_MainContainer";
    public const string LoadingTextPart = "PART_LoadingText";

    public LoadingIndicatorTheme()
        : base(typeof(LoadingIndicator))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<LoadingIndicator>((indicator, scope) =>
        {
            var loadingText = new SingleLineText()
            {
                Name                = LoadingTextPart,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center,
            };
            CreateTemplateParentBinding(loadingText, SingleLineText.TextProperty, LoadingIndicator.LoadingMsgProperty);
            loadingText.RegisterInNameScope(scope);
            var mainContainer = new Canvas
            {
                Name = MainContainerPart
            };
            mainContainer.Children.Add(loadingText);
            mainContainer.RegisterInNameScope(scope);
            return mainContainer;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
        commonStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);
        commonStyle.Add(TemplatedControl.FontSizeProperty, SharedTokenKey.FontSize);
        commonStyle.Add(LoadingIndicator.MotionEasingCurveProperty, new SetterValueFactory<Easing>(() => new LinearEasing()));
        commonStyle.Add(LoadingIndicator.MotionDurationProperty, LoadingIndicatorTokenKey.IndicatorDuration);
        commonStyle.Add(LoadingIndicator.DotBgBrushProperty, SharedTokenKey.ColorPrimary);
        commonStyle.Add(LoadingIndicator.IndicatorTextMarginProperty, SharedTokenKey.MarginXXS);
        var loadingTextStyle = new Style(selector => selector.Nesting().Template().OfType<SingleLineText>());
        loadingTextStyle.Add(SingleLineText.ForegroundProperty, SharedTokenKey.ColorPrimary);
        commonStyle.Add(loadingTextStyle);
        BuildDotSizeStyle(commonStyle);
        BuildCustomIconStyle();
        Add(commonStyle);
    }

    private void BuildCustomIconStyle()
    {
        var customIconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
        customIconStyle.Add(Icon.IconModeProperty, IconMode.Normal);
        customIconStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
        customIconStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center);
        Add(customIconStyle);

        var largeSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(LoadingIndicator.SizeTypeProperty, SizeType.Large));
        var largeIconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
        largeIconStyle.Add(Layoutable.WidthProperty, LoadingIndicator.LARGE_INDICATOR_SIZE);
        largeIconStyle.Add(Layoutable.HeightProperty, LoadingIndicator.LARGE_INDICATOR_SIZE);
        largeSizeStyle.Add(largeIconStyle);
        Add(largeSizeStyle);

        var middleSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(LoadingIndicator.SizeTypeProperty, SizeType.Middle));
        var middleIconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
        middleIconStyle.Add(Layoutable.WidthProperty, LoadingIndicator.MIDDLE_INDICATOR_SIZE);
        middleIconStyle.Add(Layoutable.HeightProperty, LoadingIndicator.MIDDLE_INDICATOR_SIZE);
        middleSizeStyle.Add(middleIconStyle);
        Add(middleSizeStyle);

        var smallSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(LoadingIndicator.SizeTypeProperty, SizeType.Small));
        var smallIconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
        smallIconStyle.Add(Layoutable.WidthProperty, LoadingIndicator.SMALL_INDICATOR_SIZE);
        smallIconStyle.Add(Layoutable.HeightProperty, LoadingIndicator.SMALL_INDICATOR_SIZE);
        smallSizeStyle.Add(smallIconStyle);
        Add(smallSizeStyle);
    }

    private void BuildDotSizeStyle(Style commonStyle)
    {
        var largeSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(LoadingIndicator.SizeTypeProperty, SizeType.Large));
        largeSizeStyle.Add(LoadingIndicator.DotSizeProperty, LoadingIndicatorTokenKey.DotSizeLG);
        commonStyle.Add(largeSizeStyle);

        var middleSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(LoadingIndicator.SizeTypeProperty, SizeType.Middle));
        middleSizeStyle.Add(LoadingIndicator.DotSizeProperty, LoadingIndicatorTokenKey.DotSize);
        commonStyle.Add(middleSizeStyle);

        var smallSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(LoadingIndicator.SizeTypeProperty, SizeType.Small));
        smallSizeStyle.Add(LoadingIndicator.DotSizeProperty, LoadingIndicatorTokenKey.DotSizeSM);
        commonStyle.Add(smallSizeStyle);
    }
}