using AtomUI.Controls.Commons;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class CheckBoxGroup : AbstractCheckBoxGroup
{
    public CheckBoxGroup()
    {
        this.RegisterTokenResourceScope(CheckBoxToken.ScopeProvider);
    }
}