using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class CalendarTheme : BaseControlTheme
{
    public const string RootPart = "PART_Root";
    public const string CalendarItemPart = "PART_CalendarItem";
    public const string FramePart = "PART_Frame";

    public CalendarTheme()
        : base(typeof(Calendar))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<Calendar>((calendar, scope) =>
        {
            var frame = new Border
            {
                Name = FramePart
            };

            CreateTemplateParentBinding(frame, Border.BorderBrushProperty, TemplatedControl.BorderBrushProperty);
            CreateTemplateParentBinding(frame, Border.BorderThicknessProperty,
                TemplatedControl.BorderThicknessProperty);
            CreateTemplateParentBinding(frame, Border.CornerRadiusProperty, TemplatedControl.CornerRadiusProperty);
            CreateTemplateParentBinding(frame, Border.BackgroundProperty, TemplatedControl.BackgroundProperty);
            CreateTemplateParentBinding(frame, Decorator.PaddingProperty, TemplatedControl.PaddingProperty);

            var rootLayout = new Panel
            {
                Name         = RootPart,
                ClipToBounds = true
            };
            rootLayout.RegisterInNameScope(scope);

            var calendarItem = new CalendarItem
            {
                Name = CalendarItemPart
            };

            calendarItem.RegisterInNameScope(scope);

            rootLayout.Children.Add(calendarItem);
            frame.Child = rootLayout;

            return frame;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(TemplatedControl.BorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
        commonStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
        commonStyle.Add(TemplatedControl.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
        commonStyle.Add(TemplatedControl.PaddingProperty, CalendarTokenResourceKey.PanelContentPadding);
        commonStyle.Add(Layoutable.MinWidthProperty, CalendarTokenResourceKey.ItemPanelMinWidth);
        commonStyle.Add(Layoutable.MinHeightProperty, CalendarTokenResourceKey.ItemPanelMinHeight);
        Add(commonStyle);
    }
}