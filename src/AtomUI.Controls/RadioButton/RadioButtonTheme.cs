using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class RadioButtonTheme : BaseControlTheme
{
    internal const string FramePart = "PART_Frame";
    internal const string IndicatorPart = "PART_Indicator";
    internal const string ContentPresenterPart = "PART_ContentPresenter";

    public RadioButtonTheme()
        : base(typeof(RadioButton))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<RadioButton>((radioButton, scope) =>
        {
            var frame = new Border
            {
                Name = FramePart
            };

            CreateTemplateParentBinding(frame, Border.BackgroundProperty, RadioButton.BackgroundProperty);
            CreateTemplateParentBinding(frame, Border.BorderBrushProperty, RadioButton.BorderBrushProperty);
            CreateTemplateParentBinding(frame, Border.BorderThicknessProperty, RadioButton.BorderThicknessProperty);
            CreateTemplateParentBinding(frame, Border.CornerRadiusProperty, RadioButton.CornerRadiusProperty);

            var layout = new DockPanel
            {
                LastChildFill = true
            };

            var indicator = new RadioIndicator()
            {
                Name = IndicatorPart,
                VerticalAlignment = VerticalAlignment.Center
            };
            DockPanel.SetDock(indicator, Dock.Left);
            CreateTemplateParentBinding(indicator, RadioIndicator.IsEnabledProperty, RadioButton.IsEnabledProperty);
            CreateTemplateParentBinding(indicator, RadioIndicator.IsCheckedProperty, RadioButton.IsCheckedProperty);
            layout.Children.Add(indicator);

            var contentPresenter = new ContentPresenter()
            {
                Name                = ContentPresenterPart,
                RecognizesAccessKey = true
            };

            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                RadioButton.ContentTemplateProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                RadioButton.ContentProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.FontSizeProperty,
                RadioButton.FontSizeProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.IsVisibleProperty,
                RadioButton.ContentProperty,
                BindingMode.Default, ObjectConverters.IsNotNull);

            layout.Children.Add(contentPresenter);
            frame.Child = layout;
            return frame;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(RadioButton.CursorProperty, new Cursor(StandardCursorType.Hand));
        commonStyle.Add(RadioButton.HorizontalAlignmentProperty, HorizontalAlignment.Left);

        var disableStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, false));
        disableStyle.Add(TemplatedControl.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
        commonStyle.Add(disableStyle);

        var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
        contentPresenterStyle.Add(ContentPresenter.MarginProperty, RadioButtonTokenResourceKey.TextMargin);
        commonStyle.Add(contentPresenterStyle);

        Add(commonStyle);

        BuildIndicatorStyle();
    }

    private void BuildIndicatorStyle()
    {
        {
            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
            indicatorStyle.Add(RadioIndicator.RadioSizeProperty, RadioButtonTokenResourceKey.RadioSize);
            indicatorStyle.Add(RadioIndicator.WidthProperty, RadioButtonTokenResourceKey.RadioSize);
            indicatorStyle.Add(RadioIndicator.HeightProperty, RadioButtonTokenResourceKey.RadioSize);
            indicatorStyle.Add(RadioIndicator.DotSizeValueProperty, RadioButtonTokenResourceKey.DotSize);
            indicatorStyle.Add(RadioIndicator.DotPaddingProperty, RadioButtonTokenResourceKey.DotPadding);
            Add(indicatorStyle);
        }
        var disableStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, false));
        {
            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
            indicatorStyle.Add(RadioIndicator.RadioBackgroundProperty, GlobalTokenResourceKey.ColorBgContainerDisabled);
            indicatorStyle.Add(RadioIndicator.RadioBorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
            indicatorStyle.Add(RadioIndicator.RadioInnerBackgroundProperty,
                RadioButtonTokenResourceKey.DotColorDisabled);
            disableStyle.Add(indicatorStyle);
        }
        Add(disableStyle);

        var enabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, true));
        {
            {
                var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
                indicatorStyle.Add(RadioIndicator.RadioInnerBackgroundProperty, RadioButtonTokenResourceKey.RadioColor);
                indicatorStyle.Add(RadioIndicator.RadioBorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
                enabledStyle.Add(indicatorStyle);
            }

            var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
            {
                var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
                indicatorStyle.Add(RadioIndicator.RadioBorderBrushProperty, GlobalTokenResourceKey.ColorPrimary);
                indicatorStyle.Add(RadioIndicator.RadioBackgroundProperty, GlobalTokenResourceKey.ColorPrimary);
                checkedStyle.Add(indicatorStyle);
            }
            enabledStyle.Add(checkedStyle);

            var unCheckedStyle =
                new Style(selector => selector.Nesting().Not(x => x.Nesting().Class(StdPseudoClass.Checked)));
            {
                var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
                indicatorStyle.Add(RadioIndicator.RadioBackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
                unCheckedStyle.Add(indicatorStyle);
            }
            var unCheckedHoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
                indicatorStyle.Add(RadioIndicator.RadioBorderBrushProperty, GlobalTokenResourceKey.ColorPrimary);
                unCheckedHoverStyle.Add(indicatorStyle);
            }
            unCheckedStyle.Add(unCheckedHoverStyle);
            enabledStyle.Add(unCheckedStyle);
        }
        Add(enabledStyle);
    }
}