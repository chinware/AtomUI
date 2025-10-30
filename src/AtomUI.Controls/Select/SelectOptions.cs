using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Input;

namespace AtomUI.Controls;

internal class SelectOptions : List
{
    internal override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        if (item is ListGroupData)
        {
            return new SelectGroupHeader();
        }
        return new SelectOptionItem();
    }
    
    // internal override void ApplyListItemData(ListItem listItem, object item)
    // {
    //     if (listItem is SelectOptionItem optionItem)
    //     {
    //         if (item is SelectOption selectOption)
    //         {
    //             if (!optionItem.IsSet(SelectOptionItem.ContentProperty))
    //             {
    //                 optionItem.SetCurrentValue(SelectOptionItem.ContentProperty, selectOption.Header);
    //             }
    //             
    //             if (!optionItem.IsSet(SelectOptionItem.IsEnabledProperty))
    //             {
    //                 optionItem.SetCurrentValue(IsEnabledProperty, selectOption.IsEnabled);
    //             }
    //         }
    //     }
    // }
    
    // protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    // {
    //     base.PrepareContainerForItemOverride(container, item, index);
    //     if (container is SelectGroupHeader headerItem)
    //     {
    //         if (item != null && item is not Visual)
    //         {
    //             if (item is SelectOptionGroup optionGroup)
    //             {
    //                 if (!headerItem.IsSet(SelectGroupHeader.HeaderProperty))
    //                 {
    //                     headerItem.SetCurrentValue(SelectGroupHeader.HeaderProperty, optionGroup.Header);
    //                 }
    //             }
    //         }
    //     }
    // }
    
    
    private void DoSelectOption(Control container)
    {
        if (ListDefaultView != null)
        {
            var index = ListDefaultView.IndexFromContainer(container);
            if (index != -1)
            {
                if (index < 0 || index >= ItemCount)
                {
                    return;
                }
            
                var mode   = SelectionMode;
                var single = mode.HasAllFlags(SelectionMode.Single);
                if (single)
                {
                    ListDefaultView.Selection.Select(index);
                }
                else
                {
                    if (ListDefaultView.Selection.IsSelected(index))
                    {
                        ListDefaultView.Selection.Deselect(index);
                    }
                    else
                    {
                        ListDefaultView.Selection.Select(index);
                    }
                }
            }
        }
    }
    
    internal override bool UpdateSelectionFromPointerEvent(Control source, PointerEventArgs e)
    {
        var container = GetContainerFromEventSource(source);
        if (container != null)
        {
            DoSelectOption(container);
            return true;
        }
        return false;
    }
}