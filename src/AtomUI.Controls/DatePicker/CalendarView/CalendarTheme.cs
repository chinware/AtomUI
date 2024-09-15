﻿using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls.CalendarView;

[ControlThemeProvider]
internal class CalendarTheme : BaseControlTheme
{
    public const string CalendarItemPart = "PART_CalendarItem";
    public const string FramePart = "PART_Frame";
    
    public CalendarTheme()
        : this(typeof(Calendar))
    {
    }

    public CalendarTheme(Type targetType) : base(targetType)
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<Calendar>((calendar, scope) =>
        {
            var frame = new Border
            {
                Name         = FramePart,
                ClipToBounds = true
            };
            
            CreateTemplateParentBinding(frame, Border.BackgroundProperty, Calendar.BackgroundProperty);
            CreateTemplateParentBinding(frame, Border.PaddingProperty, Calendar.PaddingProperty);
            CreateTemplateParentBinding(frame, Border.MinWidthProperty, Calendar.MinWidthProperty);
            CreateTemplateParentBinding(frame, Border.MinHeightProperty, Calendar.MinHeightProperty);

            var calendarItem = new CalendarItem
            {
                Name                = CalendarItemPart,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            calendarItem.RegisterInNameScope(scope);

            frame.Child = calendarItem;

            return frame;
        });
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(Calendar.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
        commonStyle.Add(Calendar.PaddingProperty, DatePickerTokenResourceKey.PanelContentPadding);
        commonStyle.Add(Calendar.HorizontalAlignmentProperty, HorizontalAlignment.Left);
        commonStyle.Add(Calendar.VerticalAlignmentProperty, VerticalAlignment.Top);
        Add(commonStyle);
    }
}