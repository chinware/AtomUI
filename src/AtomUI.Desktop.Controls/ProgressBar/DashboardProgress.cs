using AtomUI.Controls.Commons;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class DashboardProgress : AbstractGeneralDashboardProgress
{
    public DashboardProgress()
    {
        this.RegisterTokenResourceScope(ProgressBarToken.ScopeProvider);
    }
}