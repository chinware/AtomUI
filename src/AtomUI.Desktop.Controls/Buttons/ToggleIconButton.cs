using AtomUI.Controls.Commons;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class ToggleIconButton : AbstractToggleIconButton
{
    public ToggleIconButton()
    {
        this.RegisterTokenResourceScope(ButtonToken.ScopeProvider);
    }
}