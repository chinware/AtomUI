using Avalonia;

namespace AtomUI.Controls;

internal class SelectOptionItem : ListItem
{
    protected override Type StyleKeyOverride { get; } = typeof(SelectOptionItem);
    
    public SelectOptionItem()
    {
        this.GetObservable(ComboBoxItem.IsFocusedProperty)
            .Subscribe(focused =>
            {
                if (focused)
                {
                    (Parent as Select)?.ItemFocused(this);
                }
            });
    }
}