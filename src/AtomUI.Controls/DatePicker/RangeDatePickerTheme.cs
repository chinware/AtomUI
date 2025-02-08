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
        {
            var infoInputBoxStyle = new Style(selector => selector.Nesting().Template().Name(InfoInputBoxPart));
            infoInputBoxStyle.Add(TextBox.MinWidthProperty, DatePickerTokenKey.PickerInputMinWidth);
            Add(infoInputBoxStyle);
        
            var secondaryInfoInputBoxStyle = new Style(selector => selector.Nesting().Template().Name(SecondaryInfoInputBoxPart));
            secondaryInfoInputBoxStyle.Add(TextBox.MinWidthProperty, DatePickerTokenKey.PickerInputMinWidth);
            Add(secondaryInfoInputBoxStyle);
        }

        var withTimeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(DatePicker.IsShowTimeProperty, true));
        withTimeStyle.Add(DatePicker.MinWidthProperty, DatePickerTokenKey.PickerInputWithTimeMinWidth);
        {
            var infoInputBoxStyle = new Style(selector => selector.Nesting().Template().Name(InfoInputBoxPart));
            infoInputBoxStyle.Add(TextBox.MinWidthProperty, DatePickerTokenKey.PickerInputWithTimeMinWidth);
            withTimeStyle.Add(infoInputBoxStyle);
        
            var secondaryInfoInputBoxStyle = new Style(selector => selector.Nesting().Template().Name(SecondaryInfoInputBoxPart));
            secondaryInfoInputBoxStyle.Add(TextBox.MinWidthProperty, DatePickerTokenKey.PickerInputWithTimeMinWidth);
            withTimeStyle.Add(secondaryInfoInputBoxStyle);
        }
        Add(withTimeStyle);
    }
}
