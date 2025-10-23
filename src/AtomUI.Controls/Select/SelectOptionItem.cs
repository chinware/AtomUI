using Avalonia;

namespace AtomUI.Controls;

internal class SelectOptionItem : ListBoxItem
{
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