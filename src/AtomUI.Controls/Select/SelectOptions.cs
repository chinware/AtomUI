using System.Reactive.Disposables;
using AtomUI.Data;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace AtomUI.Controls;

internal class SelectOptions : List
{
    protected override Type StyleKeyOverride { get; } = typeof(List);
    
    internal override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        if (item is ListGroupData)
        {
            return new SelectGroupHeader();
        }
        return new SelectOptionItem();
    }
    
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
    
    internal override void PrepareContainerForItemOverride(CompositeDisposable disposables, Control container, object? item, int index)
    {
        if (container is SelectOptionItem listItem)
        {
            if (item != null && item is not Visual)
            {
                if (ItemTemplate != null)
                {
                    listItem.SetCurrentValue(SelectOptionItem.ContentProperty, item);
                }
                else if (item is SelectOption selectOption)
                {
                    listItem.SetCurrentValue(SelectOptionItem.ContentProperty, selectOption.Header);
                }
                if (item is SelectOption data)
                {
                    if (!listItem.IsSet(SelectOptionItem.IsEnabledProperty))
                    {
                        listItem.SetCurrentValue(IsEnabledProperty, data.IsEnabled);
                    }
                }
            }
            
            if (ItemTemplate != null)
            {
                disposables.Add(BindUtils.RelayBind(this, ItemTemplateProperty, listItem, SelectOptionItem.ContentTemplateProperty));
            }
            disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, listItem, SelectOptionItem.IsMotionEnabledProperty));
            disposables.Add(BindUtils.RelayBind(this, SizeTypeProperty, listItem, SelectOptionItem.SizeTypeProperty));
            disposables.Add(BindUtils.RelayBind(this, IsShowSelectedIndicatorProperty, listItem, SelectOptionItem.IsShowSelectedIndicatorProperty));
            disposables.Add(BindUtils.RelayBind(this, DisabledItemHoverEffectProperty, listItem,
                SelectOptionItem.DisabledItemHoverEffectProperty));
        }
        else if (container is SelectGroupHeader groupItem)
        {
            if (item != null && item is not Visual)
            {
                if (!groupItem.IsSet(SelectGroupHeader.ContentProperty))
                {
                    if (GroupItemTemplate != null)
                    {
                        groupItem.SetCurrentValue(SelectGroupHeader.ContentProperty, item);
                    }
                    else if (item is ListGroupData groupData)
                    {
                        groupItem.SetCurrentValue(SelectGroupHeader.ContentProperty, groupData.Header);
                    }
                }
            }
            
            if (GroupItemTemplate != null)
            {
                disposables.Add(BindUtils.RelayBind(this, GroupItemTemplateProperty, groupItem, SelectGroupHeader.ContentTemplateProperty));
            }
            disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, groupItem, SelectGroupHeader.IsMotionEnabledProperty));
            disposables.Add(BindUtils.RelayBind(this, SizeTypeProperty, groupItem, SelectGroupHeader.SizeTypeProperty));
        }
    }
}