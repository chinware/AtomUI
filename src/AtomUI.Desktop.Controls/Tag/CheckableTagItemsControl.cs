using AtomUI.Controls.Commons;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal class CheckableTagItemsControl : AbstractCheckableTagItemsControl
{
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new CheckableTag();
    }
}
