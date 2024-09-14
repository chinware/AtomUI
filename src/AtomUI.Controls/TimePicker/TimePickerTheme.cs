using AtomUI.Controls.Internal;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TimePickerTheme : InfoPickerInputTheme
{
    public TimePickerTheme() : base(typeof(TimePicker))
    {
    }
    
    protected override PathIcon BuildInfoIcon(InfoPickerInput infoPickerInput, INameScope scope)
    {
        return new PathIcon()
        {
            Kind = "ClockCircleOutlined"
        };
    }
    
    protected override void BuildStyles()
    {
        base.BuildStyles();
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(TimePicker.MinWidthProperty, TimePickerTokenResourceKey.PickerInputMinWidth);
        
        Add(commonStyle);
    }
}