using AtomUI.Controls.Commons;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class CheckBox : AbstractCheckBox
{
    public CheckBox()
    {
        this.RegisterTokenResourceScope(CheckBoxToken.ScopeProvider);
    }
}