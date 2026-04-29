using AtomUI.Controls.Commons;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class StepsProgressBar : AbstractGeneralStepsProgressBar
{
    public StepsProgressBar()
    {
        this.RegisterTokenResourceScope(ProgressBarToken.ScopeProvider);
    }
}