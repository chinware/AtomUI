using AtomUI.Controls.Internal;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class RangeDatePickerTheme : RangeInfoPickerInputTheme
{
    public RangeDatePickerTheme() : base(typeof(RangeDatePicker))
    {
    }
    
    protected override Icon BuildInfoIcon(InfoPickerInput infoPickerInput, INameScope scope)
    {
        return AntDesignIconPackage.CalendarOutlined();
    }
    
    protected override void BuildStyles()
    {
        base.BuildStyles();

        var withTimeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(DatePicker.IsShowTimeProperty, true));
        Add(withTimeStyle);
    }
    
    protected override TextBox BuildPickerTextBox(string name)
    {
        var pickerTextBox = base.BuildPickerTextBox(name);
        CreateTemplateParentBinding(pickerTextBox, TextBox.WidthProperty, InfoPickerInput.PreferredInputWidthProperty);
        return pickerTextBox;
    }
}
