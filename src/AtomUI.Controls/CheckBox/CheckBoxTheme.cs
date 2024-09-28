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
internal class CheckBoxTheme : BaseControlTheme
{
    internal const string FramePart = "PART_Frame";
    internal const string IndicatorPart = "PART_Indicator";
    internal const string ContentPresenterPart = "PART_ContentPresenter";
    
    public CheckBoxTheme()
        : base(typeof(CheckBox))
    {
    }
    
    protected override IControlTemplate? BuildControlTemplate()
    {
        return new FuncControlTemplate<CheckBox>((checkBox, scope) =>
        {
            var frame = new Border()
            {
                Name = FramePart
            };
            CreateTemplateParentBinding(frame, Border.BackgroundProperty, CheckBox.BackgroundProperty);
            CreateTemplateParentBinding(frame, Border.BorderBrushProperty, CheckBox.BorderBrushProperty);
            CreateTemplateParentBinding(frame, Border.BorderThicknessProperty, CheckBox.BorderThicknessProperty);
            CreateTemplateParentBinding(frame, Border.CornerRadiusProperty, CheckBox.CornerRadiusProperty);

            var layout = new DockPanel()
            {
                LastChildFill = true
            };

            var indicator = new CheckBoxIndicator()
            {
                Name = IndicatorPart
            };
            CreateTemplateParentBinding(indicator, CheckBoxIndicator.IsCheckedProperty, CheckBox.IsCheckedProperty);
            layout.Children.Add(indicator);

            var contentPresenter = new ContentPresenter()
            {
                Name = ContentPresenterPart,
                RecognizesAccessKey = true
            };
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty, CheckBox.ContentTemplateProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, CheckBox.ContentProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.FontSizeProperty, CheckBox.FontSizeProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.IsVisibleProperty, CheckBox.ContentProperty,
                BindingMode.Default, ObjectConverters.IsNotNull);
            layout.Children.Add(contentPresenter);
            frame.Child = layout;
            return frame;
        });
    }

    protected override void BuildStyles()
    {
        BuildIndicatorStyle();

        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(CheckBox.CursorProperty, new Cursor(StandardCursorType.Hand));
        commonStyle.Add(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Left);
        
        var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
        contentPresenterStyle.Add(ContentPresenter.MarginProperty, CheckBoxTokenResourceKey.TextMargin);
        commonStyle.Add(contentPresenterStyle);
        
        Add(commonStyle);
        
        var disableStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, false));
        disableStyle.Add(TemplatedControl.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
        Add(disableStyle);
        var enabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, true));
        enabledStyle.Add(TemplatedControl.ForegroundProperty, GlobalTokenResourceKey.ColorText);
        Add(enabledStyle);
    }

    private void BuildIndicatorStyle()
    {
        {
            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
            indicatorStyle.Add(CheckBoxIndicator.SizeProperty, CheckBoxTokenResourceKey.CheckIndicatorSize);
            indicatorStyle.Add(CheckBoxIndicator.WidthProperty, CheckBoxTokenResourceKey.CheckIndicatorSize);
            indicatorStyle.Add(CheckBoxIndicator.HeightProperty, CheckBoxTokenResourceKey.CheckIndicatorSize);
            indicatorStyle.Add(CheckBoxIndicator.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
            indicatorStyle.Add(CheckBoxIndicator.TristateMarkSizeProperty, CheckBoxTokenResourceKey.IndicatorTristateMarkSize);
            indicatorStyle.Add(CheckBoxIndicator.TristateMarkBrushProperty, GlobalTokenResourceKey.ColorPrimary);
            Add(indicatorStyle);
        }
        
        var disableStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, false));
        {
            {
                var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
                indicatorStyle.Add(CheckBoxIndicator.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainerDisabled);
                indicatorStyle.Add(CheckBoxIndicator.BorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
                disableStyle.Add(indicatorStyle);
            }
            var checkedStyle =
                new Style(selector => selector.Nesting().PropertyEquals(ToggleButton.IsCheckedProperty, true));
            {
                var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
                indicatorStyle.Add(CheckBoxIndicator.CheckedMarkBrushProperty, GlobalTokenResourceKey.ColorTextDisabled);
                checkedStyle.Add(indicatorStyle);
            }
            disableStyle.Add(checkedStyle);
            
            var indeterminateStyle =
                new Style(selector => selector.Nesting().PropertyEquals(ToggleButton.IsCheckedProperty, null));
            {
                var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
                indicatorStyle.Add(CheckBoxIndicator.TristateMarkBrushProperty, GlobalTokenResourceKey.ColorTextDisabled);
                indeterminateStyle.Add(indicatorStyle);
            }
            disableStyle.Add(indeterminateStyle);
        }
        
        Add(disableStyle);
        
        var enabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, true));
        
        {
            {
                var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
                indicatorStyle.Add(CheckBoxIndicator.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
                indicatorStyle.Add(CheckBoxIndicator.CheckedMarkBrushProperty, GlobalTokenResourceKey.ColorBgContainer);
                indicatorStyle.Add(CheckBoxIndicator.BorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
                enabledStyle.Add(indicatorStyle);
            }
            
            var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
            {
                var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
                indicatorStyle.Add(CheckBoxIndicator.BackgroundProperty, GlobalTokenResourceKey.ColorPrimary);
                indicatorStyle.Add(CheckBoxIndicator.BorderBrushProperty, GlobalTokenResourceKey.ColorPrimary);
                checkedStyle.Add(indicatorStyle);
            }
            
            var checkedHoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
                indicatorStyle.Add(CheckBoxIndicator.BackgroundProperty, GlobalTokenResourceKey.ColorPrimaryHover);
                indicatorStyle.Add(CheckBoxIndicator.BorderBrushProperty, GlobalTokenResourceKey.ColorPrimaryHover);
                checkedHoverStyle.Add(indicatorStyle);
            }
            checkedStyle.Add(checkedHoverStyle);
            enabledStyle.Add(checkedStyle);
            
            var unCheckedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
                indicatorStyle.Add(CheckBoxIndicator.BorderBrushProperty, GlobalTokenResourceKey.ColorPrimaryHover);
                checkedHoverStyle.Add(indicatorStyle);
            }
            enabledStyle.Add(unCheckedStyle);
            
            var indeterminateStyle = new Style(selector =>
                selector.Nesting().Class($"{StdPseudoClass.Indeterminate}{StdPseudoClass.PointerOver}"));
            {
                var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
                indicatorStyle.Add(CheckBoxIndicator.BorderBrushProperty, GlobalTokenResourceKey.ColorPrimaryHover);
                checkedHoverStyle.Add(indicatorStyle);
            }
            enabledStyle.Add(indeterminateStyle);
        }
        Add(enabledStyle);
    }
}