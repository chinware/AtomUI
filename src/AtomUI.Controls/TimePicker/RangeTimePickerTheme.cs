using AtomUI.Controls.Internal;
using AtomUI.Theme.Styling;
using Avalonia.Controls;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class RangeTimePickerTheme : RangeInfoPickerInputTheme
{
    public RangeTimePickerTheme() : base(typeof(RangeTimePicker))
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
