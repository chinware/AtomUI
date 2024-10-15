using AtomUI.Controls.Internal;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme.Styling;
using Avalonia.Controls;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class RangeTimePickerTheme : RangeInfoPickerInputTheme
{
    public RangeTimePickerTheme() : base(typeof(RangeTimePicker))
    {
    }
    
    protected override Icon BuildInfoIcon(InfoPickerInput infoPickerInput, INameScope scope)
    {
        return AntDesignIconPackage.ClockCircleOutlined();
    }
}
