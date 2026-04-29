using AtomUI.Controls.Commons;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class CircleProgress : AbstractGeneralCircleProgress
{
    public CircleProgress()
    {
        this.RegisterTokenResourceScope(ProgressBarToken.ScopeProvider);
    }
}