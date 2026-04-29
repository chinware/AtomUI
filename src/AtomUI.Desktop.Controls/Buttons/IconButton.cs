using AtomUI.Controls.Commons;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class IconButton : AbstractIconButton
{
    public IconButton()
    {
        this.RegisterTokenResourceScope(ButtonToken.ScopeProvider);
    }
}