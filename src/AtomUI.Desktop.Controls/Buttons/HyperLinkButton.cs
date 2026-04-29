using AtomUI.Controls.Commons;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class HyperLinkButton : AbstractHyperLinkButton
{
    public HyperLinkButton()
    {
        this.RegisterTokenResourceScope(ButtonToken.ScopeProvider);
    }
}