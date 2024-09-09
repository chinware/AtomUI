using AtomUI.Theme.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DashboardProgressTheme : AbstractCircleProgressTheme
{
    public DashboardProgressTheme() : base(typeof(DashboardProgress))
    {
    }
}