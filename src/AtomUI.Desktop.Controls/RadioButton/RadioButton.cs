using AtomUI.Controls.Commons;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class RadioButton : AbstractRadioButton
{
    public RadioButton()
    {
        this.RegisterTokenResourceScope(RadioButtonToken.ScopeProvider);
    }
}