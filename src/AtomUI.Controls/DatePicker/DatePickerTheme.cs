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

        var withTimeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(DatePicker.IsShowTimeProperty, true));
        withTimeStyle.Add(DatePicker.MinWidthProperty, DatePickerTokenResourceKey.PickerInputWithTimeMinWidth);
        commonStyle.Add(withTimeStyle);
        
        Add(commonStyle);
    }
}