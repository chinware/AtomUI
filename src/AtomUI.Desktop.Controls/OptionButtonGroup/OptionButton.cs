using AtomUI.Controls.Commons;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class OptionButton : AbstractOptionButton
{
    public OptionButton()
    {
        this.RegisterTokenResourceScope(OptionButtonToken.ScopeProvider);
    }
}