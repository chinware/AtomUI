using Avalonia;

namespace AtomUI.Controls;

public class SelectOptionItem : ListBoxItem
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