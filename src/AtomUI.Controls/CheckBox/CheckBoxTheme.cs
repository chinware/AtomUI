using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
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
    internal const string LabelTextPart = "PART_LabelText";
    
    public CheckBoxTheme()
        : base(typeof(CheckBox))
    {
    }
    
    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<CheckBox>((checkBox, scope) =>
        {
            var frame = new Border()
            {
                Name = FramePart,
                Padding = new Thickness(0, 1, 0, 1)
            };
            CreateTemplateParentBinding(frame, Border.BackgroundProperty, CheckBox.BackgroundProperty);
            CreateTemplateParentBinding(frame, Border.BorderBrushProperty, CheckBox.BorderBrushProperty);
            CreateTemplateParentBinding(frame, Border.BorderThicknessProperty, CheckBox.BorderThicknessProperty);
            CreateTemplateParentBinding(frame, Border.CornerRadiusProperty, CheckBox.CornerRadiusProperty);

            var layout = new DockPanel
            {
                LastChildFill = true
            };

            var indicator = new CheckBoxIndicator()
            {
                Name = IndicatorPart
            };
            
            DockPanel.SetDock(indicator, Dock.Left);
            CreateTemplateParentBinding(indicator, CheckBoxIndicator.IsEnabledProperty, CheckBox.IsEnabledProperty);
            CreateTemplateParentBinding(indicator, CheckBoxIndicator.IsCheckedProperty, CheckBox.IsCheckedProperty);
            layout.Children.Add(indicator);

            var labelText = new TextBlock
            {
                Name = LabelTextPart,
                VerticalAlignment = VerticalAlignment.Center,
            };
            CreateTemplateParentBinding(labelText, TextBlock.TextProperty, CheckBox.ContentProperty, BindingMode.Default,
                new FuncValueConverter<object?, string?>(content => content?.ToString()));
            CreateTemplateParentBinding(labelText, TextBlock.FontSizeProperty, CheckBox.FontSizeProperty);
            CreateTemplateParentBinding(labelText, TextBlock.IsVisibleProperty, CheckBox.ContentProperty,
                BindingMode.Default, ObjectConverters.IsNotNull);
            layout.Children.Add(labelText);
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
        
        var labelTextStyle = new Style(selector => selector.Nesting().Template().Name(LabelTextPart));
        labelTextStyle.Add(ContentPresenter.MarginProperty, CheckBoxTokenResourceKey.TextMargin);
        commonStyle.Add(labelTextStyle);
        
        Add(commonStyle);
        
        var disableStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, false));
        disableStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorTextDisabled);
        Add(disableStyle);
        var enabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, true));
        enabledStyle.Add(TemplatedControl.ForegroundProperty, DesignTokenKey.ColorText);
        Add(enabledStyle);
    }

    private void BuildIndicatorStyle()
    {
        {
            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
            indicatorStyle.Add(CheckBoxIndicator.SizeProperty, CheckBoxTokenResourceKey.CheckIndicatorSize);
            indicatorStyle.Add(CheckBoxIndicator.WidthProperty, CheckBoxTokenResourceKey.CheckIndicatorSize);
            indicatorStyle.Add(CheckBoxIndicator.HeightProperty, CheckBoxTokenResourceKey.CheckIndicatorSize);
            indicatorStyle.Add(CheckBoxIndicator.CornerRadiusProperty, DesignTokenKey.BorderRadiusSM);
            indicatorStyle.Add(CheckBoxIndicator.TristateMarkSizeProperty, CheckBoxTokenResourceKey.IndicatorTristateMarkSize);
            indicatorStyle.Add(CheckBoxIndicator.TristateMarkBrushProperty, DesignTokenKey.ColorPrimary);
            Add(indicatorStyle);
        }
        
        var disableStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, false));
        {
            {
                var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
                indicatorStyle.Add(CheckBoxIndicator.BackgroundProperty, DesignTokenKey.ColorBgContainerDisabled);
                indicatorStyle.Add(CheckBoxIndicator.BorderBrushProperty, DesignTokenKey.ColorBorder);
                disableStyle.Add(indicatorStyle);
            }
            var checkedStyle =
                new Style(selector => selector.Nesting().PropertyEquals(ToggleButton.IsCheckedProperty, true));
            {
                var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
                indicatorStyle.Add(CheckBoxIndicator.CheckedMarkBrushProperty, DesignTokenKey.ColorTextDisabled);
                checkedStyle.Add(indicatorStyle);
            }
            disableStyle.Add(checkedStyle);
            
            var indeterminateStyle =
                new Style(selector => selector.Nesting().PropertyEquals(ToggleButton.IsCheckedProperty, null));
            {
                var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
                indicatorStyle.Add(CheckBoxIndicator.TristateMarkBrushProperty, DesignTokenKey.ColorTextDisabled);
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
                indicatorStyle.Add(CheckBoxIndicator.BackgroundProperty, DesignTokenKey.ColorBgContainer);
                indicatorStyle.Add(CheckBoxIndicator.CheckedMarkBrushProperty, DesignTokenKey.ColorBgContainer);
                indicatorStyle.Add(CheckBoxIndicator.BorderBrushProperty, DesignTokenKey.ColorBorder);
                enabledStyle.Add(indicatorStyle);
            }
            
            var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Checked));
            {
                var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
                indicatorStyle.Add(CheckBoxIndicator.BackgroundProperty, DesignTokenKey.ColorPrimary);
                indicatorStyle.Add(CheckBoxIndicator.BorderBrushProperty, DesignTokenKey.ColorPrimary);
                checkedStyle.Add(indicatorStyle);
            }
            
            var checkedHoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
                indicatorStyle.Add(CheckBoxIndicator.BackgroundProperty, DesignTokenKey.ColorPrimaryHover);
                indicatorStyle.Add(CheckBoxIndicator.BorderBrushProperty, DesignTokenKey.ColorPrimaryHover);
                checkedHoverStyle.Add(indicatorStyle);
            }
            checkedStyle.Add(checkedHoverStyle);
            enabledStyle.Add(checkedStyle);
            
            var unCheckedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            {
                var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
                indicatorStyle.Add(CheckBoxIndicator.BorderBrushProperty, DesignTokenKey.ColorPrimaryHover);
                checkedHoverStyle.Add(indicatorStyle);
            }
            enabledStyle.Add(unCheckedStyle);
            
            var indeterminateStyle = new Style(selector =>
                selector.Nesting().Class($"{StdPseudoClass.Indeterminate}{StdPseudoClass.PointerOver}"));
            {
                var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
                indicatorStyle.Add(CheckBoxIndicator.BorderBrushProperty, DesignTokenKey.ColorPrimaryHover);
                checkedHoverStyle.Add(indicatorStyle);
            }
            enabledStyle.Add(indeterminateStyle);
        }
        Add(enabledStyle);
    }
}