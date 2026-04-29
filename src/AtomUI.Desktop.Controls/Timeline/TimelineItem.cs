using AtomUI.Controls.Commons;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class TimelineItem : AbstractTimelineItem
{
    public TimelineItem()
    {
        this.RegisterTokenResourceScope(TimelineToken.ScopeProvider);
    }
}