using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class RangeTimePickerTheme : BaseControlTheme
{
    public const string DecoratedBoxPart = "PART_DecoratedBox";
    public const string RangePickerInnerPart = "PART_RangePickerInner";
    public const string RangeStartTextBoxPart = "PART_RangeStartTextBox";
    public const string RangeEndTextBoxPart = "PART_RangeEndTextBox";
    public const string RangePickerArrowPart = "PART_RangePickerArrow";
    public const string RangePickerIndicatorPart = "PART_RangePickerIndicator";

    public RangeTimePickerTheme() : base(typeof(RangeTimePicker))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<RangeTimePicker>((rangeTimePicker, scope) =>
        {
            var indicatorLayout = new Panel();
            var canvas          = new Canvas();
            var decoratedBox    = BuildRangePickerDecoratedBox(rangeTimePicker, scope);
            var innerBox        = BuildPickerContent(rangeTimePicker, scope);
            var pickerIndicator = BuildPickerIndicator(rangeTimePicker, scope);
            canvas.Children.Add(pickerIndicator);
            decoratedBox.Content = innerBox;
            indicatorLayout.Children.Add(decoratedBox);
            indicatorLayout.Children.Add(canvas);
            return indicatorLayout;
        });
    }

    protected virtual AddOnDecoratedBox BuildRangePickerDecoratedBox(RangeTimePicker rangeTimePicker, INameScope scope)
    {
        var decoratedBox = new AddOnDecoratedBox
        {
            Name      = DecoratedBoxPart,
            Focusable = true
        };
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.StyleVariantProperty,
            RangeTimePicker.StyleVariantProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.SizeTypeProperty, RangeTimePicker.SizeTypeProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.StatusProperty, RangeTimePicker.StatusProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.LeftAddOnProperty,
            RangeTimePicker.LeftAddOnProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.RightAddOnProperty,
            RangeTimePicker.RightAddOnProperty);
        decoratedBox.RegisterInNameScope(scope);
        return decoratedBox;
    }

    private AddOnDecoratedInnerBox BuildPickerContent(RangeTimePicker rangeTimePicker, INameScope scope)
    {
        var rangePickerInnerBox = new AddOnDecoratedInnerBox
        {
            Name                     = RangePickerInnerPart,
            HorizontalAlignment      = HorizontalAlignment.Stretch,
            VerticalContentAlignment = VerticalAlignment.Stretch
        };
        rangePickerInnerBox.RegisterInNameScope(scope);
        CreateTemplateParentBinding(rangePickerInnerBox, AddOnDecoratedInnerBox.LeftAddOnContentProperty,
            RangeTimePicker.InnerLeftContentProperty);
        CreateTemplateParentBinding(rangePickerInnerBox, AddOnDecoratedInnerBox.RightAddOnContentProperty,
            RangeTimePicker.InnerRightContentProperty);
        CreateTemplateParentBinding(rangePickerInnerBox, AddOnDecoratedInnerBox.StyleVariantProperty,
            RangeTimePicker.StyleVariantProperty);
        CreateTemplateParentBinding(rangePickerInnerBox, AddOnDecoratedInnerBox.StatusProperty,
            RangeTimePicker.StatusProperty);
        CreateTemplateParentBinding(rangePickerInnerBox, AddOnDecoratedInnerBox.SizeTypeProperty,
            RangeTimePicker.SizeTypeProperty);

        var rangeLayout = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions
            {
                new(GridLength.Star),
                new(GridLength.Auto),
                new(GridLength.Star)
            }
        };

        var arrowIcon = new PathIcon
        {
            Kind = "SwapRightOutlined",
            Name = RangePickerArrowPart
        };

        TokenResourceBinder.CreateGlobalTokenBinding(arrowIcon, Layoutable.HeightProperty,
            GlobalTokenResourceKey.IconSize);
        TokenResourceBinder.CreateGlobalTokenBinding(arrowIcon, Layoutable.WidthProperty,
            GlobalTokenResourceKey.IconSize);
        TokenResourceBinder.CreateGlobalTokenBinding(arrowIcon, PathIcon.NormalFilledBrushProperty,
            GlobalTokenResourceKey.ColorTextQuaternary);

        var rangeStartTextBox = BuildPickerTextBox(RangeStartTextBoxPart);
        CreateTemplateParentBinding(rangeStartTextBox, Avalonia.Controls.TextBox.WatermarkProperty,
            RangeTimePicker.RangeStartWatermarkProperty);

        var rangeEndTextBox = BuildPickerTextBox(RangeEndTextBoxPart);
        CreateTemplateParentBinding(rangeEndTextBox, Avalonia.Controls.TextBox.WatermarkProperty,
            RangeTimePicker.RangeEndWatermarkProperty);

        rangeStartTextBox.RegisterInNameScope(scope);
        rangeEndTextBox.RegisterInNameScope(scope);

        rangeLayout.Children.Add(rangeStartTextBox);
        rangeLayout.Children.Add(arrowIcon);
        rangeLayout.Children.Add(rangeEndTextBox);

        Grid.SetColumn(rangeStartTextBox, 0);
        Grid.SetColumn(arrowIcon, 1);
        Grid.SetColumn(rangeEndTextBox, 2);

        rangePickerInnerBox.Content = rangeLayout;

        return rangePickerInnerBox;
    }

    private Rectangle BuildPickerIndicator(RangeTimePicker rangeTimePicker, INameScope scope)
    {
        var indicator = new Rectangle
        {
            Name    = RangePickerIndicatorPart,
            Opacity = 0
        };
        indicator.RegisterInNameScope(scope);
        return indicator;
    }

    private TextBox BuildPickerTextBox(string name)
    {
        var rangeStartTextBox = new TextBox
        {
            Name                     = name,
            VerticalContentAlignment = VerticalAlignment.Center,
            VerticalAlignment        = VerticalAlignment.Stretch,
            HorizontalAlignment      = HorizontalAlignment.Stretch,
            BorderThickness          = new Thickness(0),
            TextWrapping             = TextWrapping.NoWrap,
            AcceptsReturn            = false,
            EmbedMode                = true
        };

        BindUtils.RelayBind(this, DataValidationErrors.ErrorsProperty, rangeStartTextBox,
            DataValidationErrors.ErrorsProperty);
        CreateTemplateParentBinding(rangeStartTextBox, TextBox.SizeTypeProperty, NumericUpDown.SizeTypeProperty);
        CreateTemplateParentBinding(rangeStartTextBox, Avalonia.Controls.TextBox.IsReadOnlyProperty,
            Avalonia.Controls.NumericUpDown.IsReadOnlyProperty);
        CreateTemplateParentBinding(rangeStartTextBox, Avalonia.Controls.TextBox.TextProperty,
            Avalonia.Controls.NumericUpDown.TextProperty);
        CreateTemplateParentBinding(rangeStartTextBox, Avalonia.Controls.TextBox.WatermarkProperty,
            Avalonia.Controls.NumericUpDown.WatermarkProperty);
        CreateTemplateParentBinding(rangeStartTextBox, TextBox.IsEnableClearButtonProperty,
            NumericUpDown.IsEnableClearButtonProperty);

        return rangeStartTextBox;
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());

        var arrowStyle = new Style(selector => selector.Nesting().Template().Name(RangePickerArrowPart));
        arrowStyle.Add(Layoutable.MarginProperty, TimePickerTokenResourceKey.RangePickerArrowMargin);

        commonStyle.Add(arrowStyle);

        var largeStyle =
            new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Large));
        largeStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusLG);
        commonStyle.Add(largeStyle);

        var middleStyle =
            new Style(
                selector => selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Middle));
        middleStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
        commonStyle.Add(middleStyle);

        var smallStyle =
            new Style(selector =>
                selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Small));
        smallStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
        commonStyle.Add(smallStyle);

        Add(commonStyle);

        BuildIndicatorStyle(commonStyle);
    }

    private void BuildIndicatorStyle(Style commonStyle)
    {
        {
            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(RangePickerIndicatorPart));
            indicatorStyle.Add(Layoutable.HeightProperty, TimePickerTokenResourceKey.RangePickerIndicatorThickness);
            commonStyle.Add(indicatorStyle);
        }

        var defaultStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.StatusProperty, AddOnDecoratedStatus.Default));

        {
            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(RangePickerIndicatorPart));
            indicatorStyle.Add(Shape.FillProperty, GlobalTokenResourceKey.ColorPrimary);

            defaultStyle.Add(indicatorStyle);
        }
        commonStyle.Add(defaultStyle);

        var errorStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.StatusProperty, AddOnDecoratedStatus.Error));

        {
            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(RangePickerIndicatorPart));
            indicatorStyle.Add(Shape.FillProperty, GlobalTokenResourceKey.ColorError);
            errorStyle.Add(indicatorStyle);
        }

        commonStyle.Add(errorStyle);

        var warningStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.StatusProperty, AddOnDecoratedStatus.Warning));

        {
            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(RangePickerIndicatorPart));
            indicatorStyle.Add(Shape.FillProperty, GlobalTokenResourceKey.ColorWarning);
            warningStyle.Add(indicatorStyle);
        }

        commonStyle.Add(warningStyle);
    }
}