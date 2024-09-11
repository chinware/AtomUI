using AtomUI.Controls.Internal;
using AtomUI.Theme.Styling;
using Avalonia.Controls;

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
}