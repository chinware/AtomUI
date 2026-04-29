using AtomUI.Controls.Commons;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class SpinIndicator : AbstractSpinIndicator
{
    public SpinIndicator()
    {
        this.RegisterTokenResourceScope(SpinToken.ScopeProvider);
    }
}
