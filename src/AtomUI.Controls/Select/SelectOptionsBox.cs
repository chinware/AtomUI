using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal class SelectOptionsBox : ListBox
{
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        if (item is SelectOptionGroup)
        {
            return new SelectGroupHeader();
        }
        return new SelectOptionItem();
    }

    protected override void ApplyListItemData(ListBoxItem listBoxItem, object item)
    {
        if (listBoxItem is SelectOptionItem optionItem)
        {
            if (item is SelectOption selectOption)
            {
                if (!optionItem.IsSet(SelectOptionItem.ContentProperty))
                {
                    optionItem.SetCurrentValue(SelectOptionItem.ContentProperty, selectOption.Header);
                }
                
                if (!optionItem.IsSet(SelectOptionItem.IsSelectedProperty))
                {
                    optionItem.SetCurrentValue(SelectOptionItem.IsSelectedProperty, selectOption.IsSelected);
                }
                if (!optionItem.IsSet(SelectOptionItem.IsEnabledProperty))
                {
                    optionItem.SetCurrentValue(IsEnabledProperty, selectOption.IsEnabled);
                }
            }
        }
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is SelectGroupHeader headerItem)
        {
            if (item != null && item is not Visual)
            {
                if (item is SelectOptionGroup optionGroup)
                {
                    if (!headerItem.IsSet(SelectGroupHeader.HeaderProperty))
                    {
                        headerItem.SetCurrentValue(SelectGroupHeader.HeaderProperty, optionGroup.Header);
                    }
                }
            }
        }
    }
}