using AtomUI.Controls;
using AtomUI.Controls.Commons;
using AtomUI.Icons.AntDesign;
using AtomUI.Theme;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

public class Timeline : AbstractTimeline
{
    public Timeline()
    {
        this.RegisterTokenResourceScope(TimelineToken.ScopeProvider);
    }
    
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new TimelineItem
        {
            IsPending = false
        };
    }

    protected override AbstractTimelineItem CreatePendingItem()
    {
        var pathIcon = PendingIcon ?? new LoadingOutlined();
        if (pathIcon is Icon icon)
        {
            icon.LoadingAnimation = IconAnimation.Spin;
        }
        var item = new TimelineItem
        {
            Content       = Pending,
            IsPending     = true
        };
        return item;
    }
}