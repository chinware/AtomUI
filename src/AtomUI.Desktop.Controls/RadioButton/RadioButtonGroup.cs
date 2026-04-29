using AtomUI.Controls.Commons;
using AtomUI.Theme;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

public class RadioButtonGroup : AbstractRadioButtonGroup
{
    public RadioButtonGroup()
    {
        this.RegisterTokenResourceScope(RadioButtonToken.ScopeProvider);
    }
    
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new RadioButton();
    }
}