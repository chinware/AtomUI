using AtomUI.Controls.Commons;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class Spin : AbstractSpin
{
    public Spin()
    {
        this.RegisterTokenResourceScope(SpinToken.ScopeProvider);
    }
}