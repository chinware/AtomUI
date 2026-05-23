using AtomUI.Controls;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class ToggleSwitch : AbstractToggleSwitch
{
    public ToggleSwitch()
    {
        this.RegisterTokenResourceScope(ToggleSwitchToken.ScopeProvider);
    }
}
