using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace AtomUI.Controls;

internal class SelectOptions : List
{
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        if (item is SelectOptionGroup)
        {
            return new SelectGroupHeader();
        }
        return new SelectOptionItem();
    }

    protected override void ApplyListItemData(ListItem listItem, object item)
    {
        if (listItem is SelectOptionItem optionItem)
        {
            if (item is SelectOption selectOption)
            {
                if (!optionItem.IsSet(SelectOptionItem.ContentProperty))
                {
                    optionItem.SetCurrentValue(SelectOptionItem.ContentProperty, selectOption.Header);
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
    
    protected internal override bool UpdateSelectionFromPointerEvent(Control source, PointerEventArgs e)
    {
        var container = GetContainerFromEventSource(source);
        
        if (container != null)
        {
            DoSelectOption(container);
            return true;
        }
        return false;
    }

    private void DoSelectOption(Control container)
    {
        var index = IndexFromContainer(container);
        if (index != -1)
        {
            if (index < 0 || index >= ItemCount)
            {
                return;
            }
            
            var mode   = SelectionMode;
            var single  = mode.HasAllFlags(SelectionMode.Single);
            if (single)
            {
                Selection.Select(index);
            }
            else
            {
                if (Selection.IsSelected(index))
                {
                    Selection.Deselect(index);
                }
                else
                {
                    Selection.Select(index);
                }
            }
        }
    }
}