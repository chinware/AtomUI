using AtomUI.Controls.Internal;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
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
    
    protected override Icon BuildInfoIcon(InfoPickerInput infoPickerInput, INameScope scope)
    {
        return AntDesignIconPackage.CalendarOutlined();
    }
    
    protected override void BuildStyles()
    {
        base.BuildStyles();
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(DatePicker.MinWidthProperty, DatePickerTokenKey.PickerInputMinWidth);

        var withTimeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(DatePicker.IsShowTimeProperty, true));
        withTimeStyle.Add(DatePicker.MinWidthProperty, DatePickerTokenKey.PickerInputWithTimeMinWidth);
        commonStyle.Add(withTimeStyle);
        
        Add(commonStyle);
    }
}