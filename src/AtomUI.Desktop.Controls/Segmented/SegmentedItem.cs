using AtomUI.Controls.Commons;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class SegmentedItem : AbstractSegmentedItem
{
    public SegmentedItem()
    {
        this.RegisterTokenResourceScope(SegmentedToken.ScopeProvider);
    }
}