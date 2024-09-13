using AtomUI.Controls.Internal;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DatePickerTheme : InfoPickerInputTheme
{
    public DatePickerTheme() : base(typeof(DatePicker))
    {
    }
    
    protected override PathIcon BuildInfoIcon(InfoPickerInput infoPickerInput, INameScope scope)
    {
        return new PathIcon()
        {
            Kind = "CalendarOutlined"
        };
    }
    
    protected override void BuildStyles()
    {
        base.BuildStyles();
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(DatePicker.MinWidthProperty, DatePickerTokenResourceKey.PickerInputMinWidth);

        var choosingStyle = new Style(selector => selector.Nesting().Class(DatePicker.ChoosingPC));
        choosingStyle.Add(DatePicker.InputTextBrushProperty, GlobalTokenResourceKey.ColorTextTertiary);
        commonStyle.Add(choosingStyle);
        
        Add(commonStyle);
    }
}