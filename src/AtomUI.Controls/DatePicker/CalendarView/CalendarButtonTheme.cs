﻿using AtomUI.Theme;
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

namespace AtomUI.Controls.CalendarView;

[ControlThemeProvider]
internal class CalendarButtonTheme : BaseControlTheme
{
    private const string ContentPart = "PART_Content";

    public CalendarButtonTheme()
        : base(typeof(CalendarButton))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<CalendarButton>((calendarButton, scope) =>
        {
            var contentPresenter = new ContentPresenter
            {
                Name = ContentPart
            };

            CreateTemplateParentBinding(contentPresenter, ContentPresenter.PaddingProperty,
                TemplatedControl.PaddingProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ForegroundProperty,
                TemplatedControl.ForegroundProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.BackgroundProperty,
                TemplatedControl.BackgroundProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.CornerRadiusProperty,
                TemplatedControl.CornerRadiusProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.BorderBrushProperty,
                TemplatedControl.BorderBrushProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.BorderThicknessProperty,
                TemplatedControl.BorderThicknessProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.FontSizeProperty,
                TemplatedControl.FontSizeProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, ContentControl.ContentProperty, 
                BindingMode.Default, 
                new FuncValueConverter<object?, object?>(
                o =>
                {
                    if (o is string str)
                    {
                        return new TextBlock()
                        {
                            Text              = str,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                    }
                    return o;
                }));
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                ContentControl.ContentTemplateProperty);
            CreateTemplateParentBinding(contentPresenter, Layoutable.HorizontalAlignmentProperty,
                Layoutable.HorizontalAlignmentProperty);
            CreateTemplateParentBinding(contentPresenter, Layoutable.VerticalAlignmentProperty,
                Layoutable.VerticalAlignmentProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.HorizontalContentAlignmentProperty,
                ContentControl.HorizontalContentAlignmentProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.VerticalContentAlignmentProperty,
                ContentControl.VerticalContentAlignmentProperty);

            return contentPresenter;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());

        commonStyle.Add(Avalonia.Controls.Button.ClickModeProperty, ClickMode.Release);
        commonStyle.Add(InputElement.CursorProperty, new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
        commonStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorTransparent);
        commonStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextLabel);
        commonStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusSM);
        commonStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorTransparent);
        commonStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
        commonStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch);
        commonStyle.Add(ContentControl.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
        commonStyle.Add(ContentControl.VerticalContentAlignmentProperty, VerticalAlignment.Center);
        commonStyle.Add(TemplatedControl.FontSizeProperty, SharedTokenKey.FontSize);
        commonStyle.Add(Layoutable.HeightProperty, DatePickerTokenKey.CellHeight);
        commonStyle.Add(Layoutable.MarginProperty, DatePickerTokenKey.CellMargin);

        var contentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPart));
        contentStyle.Add(ContentPresenter.LineHeightProperty, DatePickerTokenKey.CellLineHeight);
        commonStyle.Add(contentStyle);

        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        hoverStyle.Add(TemplatedControl.BackgroundProperty, DatePickerTokenKey.CellHoverBg);
        commonStyle.Add(hoverStyle);

        var inactiveStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.InActive));
        inactiveStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextDisabled);
        commonStyle.Add(inactiveStyle);

        var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
        selectedStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorPrimary);
        selectedStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorWhite);
        selectedStyle.Add(TemplatedControl.BorderThicknessProperty, new Thickness(0));
        commonStyle.Add(selectedStyle);

        Add(commonStyle);
    }
}