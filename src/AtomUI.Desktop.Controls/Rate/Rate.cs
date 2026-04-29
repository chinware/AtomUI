using AtomUI.Controls.Commons;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class Rate : AbstractRate
{
    public Rate()
    {
        this.RegisterTokenResourceScope(RateToken.ScopeProvider);
    }
}