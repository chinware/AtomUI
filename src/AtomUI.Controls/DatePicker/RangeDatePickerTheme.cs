using AtomUI.Controls.Internal;
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
        {
            var infoInputBoxStyle = new Style(selector => selector.Nesting().Template().Name(InfoInputBoxPart));
            infoInputBoxStyle.Add(TextBox.MinWidthProperty, DatePickerTokenResourceKey.PickerInputMinWidth);
            Add(infoInputBoxStyle);
        
            var secondaryInfoInputBoxStyle = new Style(selector => selector.Nesting().Template().Name(SecondaryInfoInputBoxPart));
            secondaryInfoInputBoxStyle.Add(TextBox.MinWidthProperty, DatePickerTokenResourceKey.PickerInputMinWidth);
            Add(secondaryInfoInputBoxStyle);
        }

        var withTimeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(DatePicker.IsShowTimeProperty, true));
        withTimeStyle.Add(DatePicker.MinWidthProperty, DatePickerTokenResourceKey.PickerInputWithTimeMinWidth);
        {
            var infoInputBoxStyle = new Style(selector => selector.Nesting().Template().Name(InfoInputBoxPart));
            infoInputBoxStyle.Add(TextBox.MinWidthProperty, DatePickerTokenResourceKey.PickerInputWithTimeMinWidth);
            withTimeStyle.Add(infoInputBoxStyle);
        
            var secondaryInfoInputBoxStyle = new Style(selector => selector.Nesting().Template().Name(SecondaryInfoInputBoxPart));
            secondaryInfoInputBoxStyle.Add(TextBox.MinWidthProperty, DatePickerTokenResourceKey.PickerInputWithTimeMinWidth);
            withTimeStyle.Add(secondaryInfoInputBoxStyle);
        }
        Add(withTimeStyle);
    }
}
