using System.Reactive.Disposables;
using AtomUI.Data;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal static class NavMenuEntryContainerCoordinator
{
    private static readonly object NodeRecycleKey = new();
    private static readonly object GroupRecycleKey = new();
    private static readonly object DividerRecycleKey = new();

    public static bool NeedsContainer(
        ItemsControl owner,
        object? item,
        int index,
        out object? recycleKey)
    {
        var entry = NavMenuEntryValidation.Validate(item, owner, index);
        recycleKey = entry switch
        {
            INavMenuNode => NodeRecycleKey,
            NavMenuGroup => GroupRecycleKey,
            NavMenuDivider => DividerRecycleKey,
            _ => throw new InvalidOperationException($"Unsupported NavMenu entry type {entry.GetType().Name}.")
        };
        return true;
    }

    public static Control CreateContainer(object? item, int index, object? recycleKey)
    {
        return item switch
        {
            INavMenuNode when ReferenceEquals(recycleKey, NodeRecycleKey) => new NavMenuItem(),
            NavMenuGroup when ReferenceEquals(recycleKey, GroupRecycleKey) => new NavMenuGroupItem(),
            NavMenuDivider when ReferenceEquals(recycleKey, DividerRecycleKey) => new NavMenuDividerItem(),
            _ => throw new InvalidOperationException(
                $"Cannot create a NavMenu container for index {index} and type {item?.GetType().Name ?? "<null>"}.")
        };
    }

    public static void PrepareContainer(
        ItemsControl owner,
        Control container,
        object? item,
        int index)
    {
        var context = ResolveContext(owner);
        switch (container, item)
        {
            case (NavMenuItem menuItem, INavMenuNode):
                PrepareNodeContainer(owner, menuItem, item, index, context);
                break;

            case (NavMenuGroupItem groupItem, NavMenuGroup group):
                PrepareGroupContainer(owner, groupItem, group, context);
                break;

            case (NavMenuDividerItem dividerItem, NavMenuDivider):
                PrepareDividerContainer(owner, dividerItem, context);
                break;

            default:
                throw new InvalidOperationException(
                    $"NavMenu container {container.GetType().Name} is incompatible with entry {item?.GetType().Name ?? "<null>"} at index {index}.");
        }
    }

    public static void ClearContainer(ItemsControl owner, Control container)
    {
        switch (container)
        {
            case NavMenuItem menuItem:
                menuItem.SetCurrentValue(NavMenuItem.IsKeyboardActiveProperty, false);
                menuItem.OwnerMenu?.ForgetGeneratedContainer(menuItem);
                menuItem.ClearNodeBindingDisposables();
                ClearNodeContainerBindings(menuItem);
                menuItem.ClearEntryContext();
                break;

            case NavMenuGroupItem groupItem:
                groupItem.ClearEntryBindingDisposables();
                groupItem.ClearValue(ItemsControl.ItemsSourceProperty);
                ClearGroupOwnerBindings(groupItem);
                groupItem.ClearEntryContext();
                break;

            case NavMenuDividerItem dividerItem:
                dividerItem.ClearEntryBindingDisposables();
                dividerItem.ClearEntryContext();
                break;
        }
    }

    private static void PrepareNodeContainer(
        ItemsControl owner,
        NavMenuItem menuItem,
        object item,
        int index,
        EntryContext context)
    {
        menuItem.UpdateEntryContext(
            context.OwnerMenu,
            context.SemanticParentItem,
            context.Level,
            context.IsTopLevel,
            owner);

        var nodeBindingDisposables = menuItem.ResetNodeBindingDisposables();
        var resourceHost = context.OwnerMenu is not null ? context.OwnerMenu : owner;
        NavMenuItemContainerBinder.BindNode(menuItem, item, resourceHost, nodeBindingDisposables);

        if (!NavMenuItemContainerBinder.TryBindNodeHeaderTemplate(menuItem, item, nodeBindingDisposables) &&
            owner.ItemTemplate != null)
        {
            nodeBindingDisposables.Add(BindUtils.RelayBind(
                owner,
                ItemsControl.ItemTemplateProperty,
                menuItem,
                NavMenuItem.HeaderTemplateProperty));
        }

        BindNodeOwnerState(owner, menuItem, nodeBindingDisposables);

        if (context.OwnerMenu is { } ownerMenu)
        {
            nodeBindingDisposables.Add(BindUtils.RelayBind(
                ownerMenu,
                NavMenu.IsCollapsedTooltipEnabledProperty,
                menuItem,
                NavMenuItem.IsCollapsedTooltipEnabledProperty));
            nodeBindingDisposables.Add(BindUtils.RelayBind(
                ownerMenu,
                NavMenu.CollapsedTooltipPlacementProperty,
                menuItem,
                NavMenuItem.CollapsedTooltipPlacementProperty));
            nodeBindingDisposables.Add(BindUtils.RelayBind(
                ownerMenu,
                NavMenu.CollapsedTooltipShowDelayProperty,
                menuItem,
                NavMenuItem.CollapsedTooltipShowDelayProperty));
            nodeBindingDisposables.Add(BindUtils.RelayBind(
                ownerMenu,
                NavMenu.CollapsedTooltipBetweenShowDelayProperty,
                menuItem,
                NavMenuItem.CollapsedTooltipBetweenShowDelayProperty));
        }

        switch (owner)
        {
            case NavMenu menu:
                menu.PrepareNavMenuItem(menuItem, item, index);
                break;

            case NavMenuItem parentItem:
                parentItem.PrepareGeneratedNavMenuItem(menuItem, item, index);
                break;
        }

        context.OwnerMenu?.ApplySelectionStateToPreparedContainer(menuItem);
    }

    private static void PrepareGroupContainer(
        ItemsControl owner,
        NavMenuGroupItem groupItem,
        NavMenuGroup group,
        EntryContext context)
    {
        groupItem.UpdateEntryContext(
            context.OwnerMenu,
            context.SemanticParentItem,
            context.Level,
            context.IsTopLevel);

        var disposables = groupItem.ResetEntryBindingDisposables();
        var resourceHost = context.OwnerMenu is not null ? context.OwnerMenu : owner;
        disposables.Add(group.AttachResourceHost(resourceHost));
        disposables.Add(BindUtils.RelayBind(
            group,
            NavMenuGroup.HeaderProperty,
            groupItem,
            NavMenuGroupItem.HeaderProperty));
        disposables.Add(BindUtils.RelayBind(
            group,
            NavMenuGroup.HeaderTemplateProperty,
            groupItem,
            NavMenuGroupItem.HeaderTemplateProperty));

        groupItem.SetCurrentValue(ItemsControl.ItemsSourceProperty, group.Entries);
        BindGroupOwnerState(owner, groupItem, disposables);
    }

    private static void PrepareDividerContainer(
        ItemsControl owner,
        NavMenuDividerItem dividerItem,
        EntryContext context)
    {
        var disposables = dividerItem.ResetEntryBindingDisposables();
        dividerItem.SetCurrentValue(NavMenuDividerItem.IsTopLevelProperty, context.IsTopLevel);
        switch (owner)
        {
            case NavMenu menu:
                disposables.Add(BindUtils.RelayBind(
                    menu,
                    NavMenu.EffectiveModeProperty,
                    dividerItem,
                    NavMenuDividerItem.ModeProperty));
                disposables.Add(BindUtils.RelayBind(
                    menu,
                    NavMenu.IsDarkStyleProperty,
                    dividerItem,
                    NavMenuDividerItem.IsDarkStyleProperty));
                break;

            case NavMenuItem menuItem:
                disposables.Add(BindUtils.RelayBind(
                    menuItem,
                    NavMenuItem.ModeProperty,
                    dividerItem,
                    NavMenuDividerItem.ModeProperty));
                disposables.Add(BindUtils.RelayBind(
                    menuItem,
                    NavMenuItem.IsDarkStyleProperty,
                    dividerItem,
                    NavMenuDividerItem.IsDarkStyleProperty));
                break;

            case NavMenuGroupItem groupItem:
                disposables.Add(BindUtils.RelayBind(
                    groupItem,
                    NavMenuGroupItem.ModeProperty,
                    dividerItem,
                    NavMenuDividerItem.ModeProperty));
                disposables.Add(BindUtils.RelayBind(
                    groupItem,
                    NavMenuGroupItem.IsDarkStyleProperty,
                    dividerItem,
                    NavMenuDividerItem.IsDarkStyleProperty));
                break;
        }
    }

    private static void BindNodeOwnerState(
        ItemsControl owner,
        NavMenuItem menuItem,
        CompositeDisposable disposables)
    {
        switch (owner)
        {
            case NavMenu menu:
                disposables.Add(BindUtils.RelayBind(menu, NavMenu.EffectiveModeProperty, menuItem, NavMenuItem.ModeProperty));
                disposables.Add(BindUtils.RelayBind(menu, NavMenu.IsEffectiveInlineCollapsedProperty, menuItem, NavMenuItem.IsInlineCollapsedProperty));
                disposables.Add(BindUtils.RelayBind(menu, NavMenu.IsDarkStyleProperty, menuItem, NavMenuItem.IsDarkStyleProperty));
                disposables.Add(BindUtils.RelayBind(menu, NavMenu.IsItemBackgroundEnabledProperty, menuItem, NavMenuItem.IsItemBackgroundEnabledProperty));
                disposables.Add(BindUtils.RelayBind(menu, NavMenu.IsMotionEnabledProperty, menuItem, NavMenuItem.IsMotionEnabledProperty));
                disposables.Add(BindUtils.RelayBind(menu, NavMenu.ShouldUseOverlayPopupProperty, menuItem, NavMenuItem.ShouldUseOverlayPopupProperty));
                break;

            case NavMenuItem parentItem:
                disposables.Add(BindUtils.RelayBind(parentItem, NavMenuItem.ModeProperty, menuItem, NavMenuItem.ModeProperty));
                menuItem.ClearValue(NavMenuItem.IsInlineCollapsedProperty);
                menuItem.SetCurrentValue(NavMenuItem.IsInlineCollapsedProperty, false);
                disposables.Add(BindUtils.RelayBind(parentItem, NavMenuItem.IsDarkStyleProperty, menuItem, NavMenuItem.IsDarkStyleProperty));
                disposables.Add(BindUtils.RelayBind(parentItem, NavMenuItem.IsItemBackgroundEnabledProperty, menuItem, NavMenuItem.IsItemBackgroundEnabledProperty));
                disposables.Add(BindUtils.RelayBind(parentItem, NavMenuItem.IsMotionEnabledProperty, menuItem, NavMenuItem.IsMotionEnabledProperty));
                disposables.Add(BindUtils.RelayBind(parentItem, NavMenuItem.ItemContainerThemeProperty, menuItem, NavMenuItem.ItemContainerThemeProperty));
                disposables.Add(BindUtils.RelayBind(parentItem, NavMenuItem.ShouldUseOverlayPopupProperty, menuItem, NavMenuItem.ShouldUseOverlayPopupProperty));
                break;

            case NavMenuGroupItem groupItem:
                disposables.Add(BindUtils.RelayBind(groupItem, NavMenuGroupItem.ModeProperty, menuItem, NavMenuItem.ModeProperty));
                disposables.Add(BindUtils.RelayBind(
                    groupItem,
                    NavMenuGroupItem.IsInlineCollapsedProperty,
                    menuItem,
                    NavMenuItem.IsInlineCollapsedProperty));
                disposables.Add(BindUtils.RelayBind(groupItem, NavMenuGroupItem.IsDarkStyleProperty, menuItem, NavMenuItem.IsDarkStyleProperty));
                disposables.Add(BindUtils.RelayBind(groupItem, NavMenuGroupItem.IsItemBackgroundEnabledProperty, menuItem, NavMenuItem.IsItemBackgroundEnabledProperty));
                disposables.Add(BindUtils.RelayBind(groupItem, NavMenuGroupItem.IsMotionEnabledProperty, menuItem, NavMenuItem.IsMotionEnabledProperty));
                disposables.Add(BindUtils.RelayBind(groupItem, NavMenuGroupItem.ItemContainerThemeProperty, menuItem, NavMenuItem.ItemContainerThemeProperty));
                disposables.Add(BindUtils.RelayBind(groupItem, NavMenuGroupItem.ShouldUseOverlayPopupProperty, menuItem, NavMenuItem.ShouldUseOverlayPopupProperty));
                break;
        }
    }

    private static void BindGroupOwnerState(
        ItemsControl owner,
        NavMenuGroupItem groupItem,
        CompositeDisposable disposables)
    {
        disposables.Add(BindUtils.RelayBind(owner, ItemsControl.ItemTemplateProperty, groupItem, ItemsControl.ItemTemplateProperty));
        disposables.Add(BindUtils.RelayBind(owner, ItemsControl.ItemContainerThemeProperty, groupItem, ItemsControl.ItemContainerThemeProperty));

        switch (owner)
        {
            case NavMenu menu:
                disposables.Add(BindUtils.RelayBind(menu, NavMenu.EffectiveModeProperty, groupItem, NavMenuGroupItem.ModeProperty));
                disposables.Add(BindUtils.RelayBind(menu, NavMenu.IsEffectiveInlineCollapsedProperty, groupItem, NavMenuGroupItem.IsInlineCollapsedProperty));
                disposables.Add(BindUtils.RelayBind(menu, NavMenu.IsDarkStyleProperty, groupItem, NavMenuGroupItem.IsDarkStyleProperty));
                disposables.Add(BindUtils.RelayBind(menu, NavMenu.IsItemBackgroundEnabledProperty, groupItem, NavMenuGroupItem.IsItemBackgroundEnabledProperty));
                disposables.Add(BindUtils.RelayBind(menu, NavMenu.IsMotionEnabledProperty, groupItem, NavMenuGroupItem.IsMotionEnabledProperty));
                disposables.Add(BindUtils.RelayBind(menu, NavMenu.ShouldUseOverlayPopupProperty, groupItem, NavMenuGroupItem.ShouldUseOverlayPopupProperty));
                break;

            case NavMenuItem menuItem:
                disposables.Add(BindUtils.RelayBind(menuItem, NavMenuItem.ModeProperty, groupItem, NavMenuGroupItem.ModeProperty));
                groupItem.ClearValue(NavMenuGroupItem.IsInlineCollapsedProperty);
                groupItem.SetCurrentValue(NavMenuGroupItem.IsInlineCollapsedProperty, false);
                disposables.Add(BindUtils.RelayBind(menuItem, NavMenuItem.IsDarkStyleProperty, groupItem, NavMenuGroupItem.IsDarkStyleProperty));
                disposables.Add(BindUtils.RelayBind(menuItem, NavMenuItem.IsItemBackgroundEnabledProperty, groupItem, NavMenuGroupItem.IsItemBackgroundEnabledProperty));
                disposables.Add(BindUtils.RelayBind(menuItem, NavMenuItem.IsMotionEnabledProperty, groupItem, NavMenuGroupItem.IsMotionEnabledProperty));
                disposables.Add(BindUtils.RelayBind(menuItem, NavMenuItem.ShouldUseOverlayPopupProperty, groupItem, NavMenuGroupItem.ShouldUseOverlayPopupProperty));
                break;

            case NavMenuGroupItem parentGroup:
                disposables.Add(BindUtils.RelayBind(parentGroup, NavMenuGroupItem.ModeProperty, groupItem, NavMenuGroupItem.ModeProperty));
                disposables.Add(BindUtils.RelayBind(parentGroup, NavMenuGroupItem.IsInlineCollapsedProperty, groupItem, NavMenuGroupItem.IsInlineCollapsedProperty));
                disposables.Add(BindUtils.RelayBind(parentGroup, NavMenuGroupItem.IsDarkStyleProperty, groupItem, NavMenuGroupItem.IsDarkStyleProperty));
                disposables.Add(BindUtils.RelayBind(parentGroup, NavMenuGroupItem.IsItemBackgroundEnabledProperty, groupItem, NavMenuGroupItem.IsItemBackgroundEnabledProperty));
                disposables.Add(BindUtils.RelayBind(parentGroup, NavMenuGroupItem.IsMotionEnabledProperty, groupItem, NavMenuGroupItem.IsMotionEnabledProperty));
                disposables.Add(BindUtils.RelayBind(parentGroup, NavMenuGroupItem.ShouldUseOverlayPopupProperty, groupItem, NavMenuGroupItem.ShouldUseOverlayPopupProperty));
                break;
        }
    }

    private static void ClearGroupOwnerBindings(NavMenuGroupItem groupItem)
    {
        groupItem.ClearValue(ItemsControl.ItemTemplateProperty);
        groupItem.ClearValue(ItemsControl.ItemContainerThemeProperty);
        groupItem.ClearValue(NavMenuGroupItem.ModeProperty);
        groupItem.ClearValue(NavMenuGroupItem.IsInlineCollapsedProperty);
        groupItem.ClearValue(NavMenuGroupItem.IsDarkStyleProperty);
        groupItem.ClearValue(NavMenuGroupItem.IsItemBackgroundEnabledProperty);
        groupItem.ClearValue(NavMenuGroupItem.IsMotionEnabledProperty);
        groupItem.ClearValue(NavMenuGroupItem.ShouldUseOverlayPopupProperty);
    }

    private static void ClearNodeContainerBindings(NavMenuItem menuItem)
    {
        menuItem.ClearValue(NavMenuItem.HeaderProperty);
        menuItem.ClearValue(NavMenuItem.HeaderTemplateProperty);
        menuItem.ClearValue(NavMenuItem.NodeHeaderProperty);
        menuItem.ClearValue(NavMenuItem.TooltipProperty);
        menuItem.ClearValue(NavMenuItem.IsTooltipEnabledProperty);
        menuItem.ClearValue(NavMenuItem.IconProperty);
        menuItem.ClearValue(NavMenuItem.ItemKeyProperty);
        menuItem.ClearValue(NavMenuItem.CommandProperty);
        menuItem.ClearValue(NavMenuItem.CommandParameterProperty);
        menuItem.ClearValue(NavMenuItem.IsEnabledProperty);
        menuItem.ClearValue(ItemsControl.ItemsSourceProperty);

        menuItem.ClearValue(NavMenuItem.ModeProperty);
        menuItem.ClearValue(NavMenuItem.IsInlineCollapsedProperty);
        menuItem.ClearValue(NavMenuItem.IsCollapsedTooltipEnabledProperty);
        menuItem.ClearValue(NavMenuItem.CollapsedTooltipPlacementProperty);
        menuItem.ClearValue(NavMenuItem.CollapsedTooltipShowDelayProperty);
        menuItem.ClearValue(NavMenuItem.CollapsedTooltipBetweenShowDelayProperty);
        menuItem.ClearValue(NavMenuItem.IsDarkStyleProperty);
        menuItem.ClearValue(NavMenuItem.IsItemBackgroundEnabledProperty);
        menuItem.ClearValue(NavMenuItem.IsMotionEnabledProperty);
        menuItem.ClearValue(NavMenuItem.ShouldUseOverlayPopupProperty);
        menuItem.ClearValue(NavMenuItem.ItemContainerThemeProperty);
        menuItem.ClearValue(NavMenuItem.IsInSelectedPathProperty);
        menuItem.ClearValue(NavMenuItem.IsSelectedProperty);
        menuItem.ClearValue(NavMenuItem.IsSubMenuOpenProperty);
        menuItem.ClearValue(NavMenuItem.IsPopupPinnedOpenProperty);
    }

    private static EntryContext ResolveContext(ItemsControl owner)
    {
        return owner switch
        {
            NavMenu menu => new EntryContext(menu, null, 0, true),
            NavMenuItem item => new EntryContext(item.OwnerMenu, item, item.Level + 1, false),
            NavMenuGroupItem group => new EntryContext(
                group.OwnerMenu,
                group.SemanticParentItem,
                group.Level,
                group.IsTopLevel),
            _ => throw new InvalidOperationException($"Unsupported NavMenu entry owner {owner.GetType().Name}.")
        };
    }

    private readonly record struct EntryContext(
        NavMenu? OwnerMenu,
        NavMenuItem? SemanticParentItem,
        int Level,
        bool IsTopLevel);
}
