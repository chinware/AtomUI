using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls.Primitives;

[ControlThemeProvider]
internal class RangeInfoPickerInputTheme : InfoPickerInputTheme
{
    public const string SecondaryInfoInputBoxPart = "PART_SecondaryInfoInputBox";
    public const string RangePickerArrowPart = "PART_RangePickerArrow";
    public const string RangePickerIndicatorPart = "PART_RangePickerIndicator";

    public RangeInfoPickerInputTheme() : this(typeof(RangeInfoPickerInput))
    {
    }

    protected RangeInfoPickerInputTheme(Type targetType) : base(targetType)
    {
    }

    protected override Control BuildPickerContent(InfoPickerInput infoPickerInput, INameScope scope)
    {
        var rangeLayout = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions
            {
                new(GridLength.Star),
                new(GridLength.Auto),
                new(GridLength.Star)
            }
        };

        var arrowIcon = AntDesignIconPackage.SwapRightOutlined();
        arrowIcon.Name = RangePickerArrowPart;
        arrowIcon.RegisterInNameScope(scope);

        var rangeStartTextBox = BuildPickerTextBox(InfoInputBoxPart);
        
        CreateTemplateParentBinding(rangeStartTextBox, TextBox.TextProperty, RangeInfoPickerInput.TextProperty);
        CreateTemplateParentBinding(rangeStartTextBox, TextBox.WatermarkProperty,
            RangeInfoPickerInput.WatermarkProperty);

        var rangeEndTextBox = BuildPickerTextBox(SecondaryInfoInputBoxPart);
        CreateTemplateParentBinding(rangeEndTextBox, TextBox.TextProperty, RangeInfoPickerInput.SecondaryTextProperty);
        CreateTemplateParentBinding(rangeEndTextBox, TextBox.WatermarkProperty,
            RangeInfoPickerInput.SecondaryWatermarkProperty);

        rangeStartTextBox.RegisterInNameScope(scope);
        rangeEndTextBox.RegisterInNameScope(scope);

        rangeLayout.Children.Add(rangeStartTextBox);
        rangeLayout.Children.Add(arrowIcon);
        rangeLayout.Children.Add(rangeEndTextBox);

        Grid.SetColumn(rangeStartTextBox, 0);
        Grid.SetColumn(arrowIcon, 1);
        Grid.SetColumn(rangeEndTextBox, 2);
        return rangeLayout;
    }

    protected override void BuildExtraElement(Panel containerLayout, InfoPickerInput infoPickerInput, INameScope scope)
    {
        var canvas          = new Canvas();
        var pickerIndicator = BuildPickerIndicator(infoPickerInput, scope);
        canvas.Children.Add(pickerIndicator);
        containerLayout.Children.Add(canvas);
    }

    private Rectangle BuildPickerIndicator(InfoPickerInput infoPickerInput, INameScope scope)
    {
        var indicator = new Rectangle
        {
            Name    = RangePickerIndicatorPart,
            Opacity = 0
        };
        indicator.RegisterInNameScope(scope);
        return indicator;
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        var commonStyle = new Style(selector => selector.Nesting());

        var arrowIconStyle = new Style(selector => selector.Nesting().Template().Name(RangePickerArrowPart));
        arrowIconStyle.Add(Layoutable.MarginProperty, InfoPickerInputTokenKey.RangePickerArrowMargin);
        arrowIconStyle.Add(Layoutable.HeightProperty,  SharedTokenKey.IconSize);
        arrowIconStyle.Add(Layoutable.WidthProperty,  SharedTokenKey.IconSize);
        arrowIconStyle.Add(Icon.NormalFilledBrushProperty,  SharedTokenKey.ColorTextQuaternary);

        commonStyle.Add(arrowIconStyle);

        BuildIndicatorStyle(commonStyle);
        Add(commonStyle);
    }

    private void BuildIndicatorStyle(Style commonStyle)
    {
        {
            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(RangePickerIndicatorPart));
            indicatorStyle.Add(Layoutable.HeightProperty, InfoPickerInputTokenKey.RangePickerIndicatorThickness);
            commonStyle.Add(indicatorStyle);
        }

        var defaultStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.StatusProperty, AddOnDecoratedStatus.Default));

        {
            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(RangePickerIndicatorPart));
            indicatorStyle.Add(Shape.FillProperty, SharedTokenKey.ColorPrimary);

            defaultStyle.Add(indicatorStyle);
        }
        commonStyle.Add(defaultStyle);

        var errorStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.StatusProperty, AddOnDecoratedStatus.Error));

        {
            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(RangePickerIndicatorPart));
            indicatorStyle.Add(Shape.FillProperty, SharedTokenKey.ColorError);
            errorStyle.Add(indicatorStyle);
        }

        commonStyle.Add(errorStyle);

        var warningStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.StatusProperty, AddOnDecoratedStatus.Warning));

        {
            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(RangePickerIndicatorPart));
            indicatorStyle.Add(Shape.FillProperty, SharedTokenKey.ColorWarning);
            warningStyle.Add(indicatorStyle);
        }

        commonStyle.Add(warningStyle);
    }
}