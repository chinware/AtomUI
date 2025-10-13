﻿using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Disposables;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Layout;
using Avalonia.Threading;

namespace AtomUI.Controls;

using AvaloniaContextMenu = Avalonia.Controls.ContextMenu;

public class ContextMenu : AvaloniaContextMenu,
                           ISizeTypeAware,
                           IMotionAwareControl,
                           IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<ContextMenu>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ContextMenu>();

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    #region 内部属性定义

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => MenuToken.ID;

    #endregion
    
    private Popup? _popup;
    private readonly Dictionary<MenuItem, CompositeDisposable> _itemsBindingDisposables = new();

    public ContextMenu()
    {
        this.RegisterResources();
        Items.CollectionChanged  += HandleItemsCollectionChanged;
        // 我们在这里有一次初始化的机会
        _popup = new Popup
        {
            WindowManagerAddShadowHint     = false,
            IsLightDismissEnabled          = false,
            OverlayDismissEventPassThrough = true,
            IsDetectMouseClickEnabled      = true,
            IgnoreFirstDetected            = false
        };
       
        _popup.Opened             += this.CreateEventHandler("PopupOpened");
        _popup.Closed             += this.CreateEventHandler<EventArgs>("PopupClosed");
        _popup.ClickHidePredicate =  MenuPopupClosePredicate;
        _popup.CloseAction        =  MenuPopupCloseAction;
        _popup.AddClosingEventHandler(this.CreateEventHandler<CancelEventArgs>("PopupClosing")!);
        _popup.KeyUp += this.CreateEventHandler<KeyEventArgs>("PopupKeyUp");
        if (_popup is IPopupHostProvider popupHostProvider)
        {
            popupHostProvider.PopupHostChanged += HandlePopupHostChanged;
        }
        Closing += (sender, args) =>
        {
            args.Cancel = true;
        };
        this.SetPopup(_popup);
        Opened += (sender, args) =>
        {
            _popup.SetIgnoreIsOpenChanged(true);
            _popup.IsMotionAwareOpen = true;
        };
    }
    
    private void HandleItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is MenuItem menuItem)
                    {
                        if (_itemsBindingDisposables.TryGetValue(menuItem, out var disposable))
                        {
                            disposable.Dispose();
                        }
                        _itemsBindingDisposables.Remove(menuItem);
                    }
                }
            }
        }
    }
    
    private void HandlePopupHostChanged(IPopupHost? host)
    {
        if (host is PopupRoot popupRoot)
        {
            if (popupRoot.ParentTopLevel is WindowBase window)
            {
                window.Deactivated += (sender, args) =>
                {
                    Close();
                };
            }
        }
    }
    
    private bool MenuPopupClosePredicate(IPopupHostProvider hostProvider, RawPointerEventArgs args)
    {
        var popupRoots = new HashSet<PopupRoot>();
        foreach (var child in Items)
        {
            if (child is MenuItem childMenuItem)
            {
                popupRoots.UnionWith(MenuItem.CollectPopupRoots(childMenuItem));
            }
        }

        if (_popup?.Host is PopupRoot popupRoot)
        {
            popupRoots.Add(popupRoot);
        }
        return !popupRoots.Contains(args.Root);
    }

    private void MenuPopupCloseAction(Popup popup)
    {
        Close();
    }
    
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        if (item is MenuSeparatorData)
        {
            return new MenuSeparator();
        }
        return new MenuItem();
    }
    
    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        if (item is MenuItem or MenuSeparator)
        {
            recycleKey = null;
            return false;
        }

        recycleKey = DefaultRecycleKey;
        return true;
    }
    
    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is MenuItem menuItem)
        {
            var disposables = new CompositeDisposable(4);
            
            if (item != null && item is not Visual)
            {
                if (!menuItem.IsSet(MenuItem.HeaderProperty))
                {
                    menuItem.SetCurrentValue(MenuItem.HeaderProperty, item);
                }

                if (item is IMenuItemData menuItemData)
                {
                    if (!menuItem.IsSet(MenuItem.IconProperty))
                    {
                        menuItem.SetCurrentValue(MenuItem.IconProperty, menuItemData.Icon);
                    }

                    if (menuItem.ItemKey == null)
                    {
                        menuItem.ItemKey = menuItemData.ItemKey;
                    }
                    if (!menuItem.IsSet(MenuItem.IsEnabledProperty))
                    {
                        menuItem.SetCurrentValue(IsEnabledProperty, menuItemData.IsEnabled);
                    }
                    if (!menuItem.IsSet(MenuItem.InputGestureProperty))
                    {
                        menuItem.SetCurrentValue(MenuItem.InputGestureProperty, menuItemData.InputGesture);
                    }
                }
            }
             
            if (ItemTemplate != null)
            {
                disposables.Add(BindUtils.RelayBind(this, ItemTemplateProperty, menuItem, MenuItem.HeaderTemplateProperty));
            }
            
            disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, menuItem, MenuItem.IsMotionEnabledProperty));
            disposables.Add(BindUtils.RelayBind(this, ItemTemplateProperty, menuItem, MenuItem.ItemTemplateProperty));
            disposables.Add(BindUtils.RelayBind(this, SizeTypeProperty, menuItem, MenuItem.SizeTypeProperty));
            
            PrepareMenuItem(menuItem, item, index, disposables);
            
            if (_itemsBindingDisposables.TryGetValue(menuItem, out var oldDisposables))
            {
                oldDisposables.Dispose();
                _itemsBindingDisposables.Remove(menuItem);
            }
            _itemsBindingDisposables.Add(menuItem, disposables);
        }
        else if (container is MenuSeparator menuSeparator)
        {
            menuSeparator.Orientation = Orientation.Vertical;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(container), "The container type is incorrect, it must be type MenuItem or MenuSeparator.");
        }
    }
    
    protected virtual void PrepareMenuItem(MenuItem menuItem, object? item, int index, CompositeDisposable compositeDisposable)
    {
    }
    
    public override void Close()
    {
        _popup?.SetIgnoreIsOpenChanged(true);
        if (!IsOpen || _popup == null || !_popup.IsVisible)
        {
            return;
        }
        
        if (IsMotionEnabled)
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                for (var i = 0; i < ItemCount; i++)
                {
                    var container = ContainerFromIndex(i);
                    if (container is MenuItem menuItem)
                    {
                        await menuItem.CloseItemAsync();
                    }
                }
                _popup.IsMotionAwareOpen = false;
            });
        }
        else
        {
            for (var i = 0; i < ItemCount; i++)
            {
                var container = ContainerFromIndex(i);
                if (container is MenuItem menuItem)
                {
                    menuItem.Close();
                }
            }
            _popup.IsMotionAwareOpen = false;
        }
    }
}