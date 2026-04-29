using AtomUI.Controls.Commons;
using AtomUI.Theme;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

public class Segmented : AbstractSegmented
{
    public Segmented()
    {
        this.RegisterTokenResourceScope(SegmentedToken.ScopeProvider);
    }
    
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new SegmentedItem();
    }
}