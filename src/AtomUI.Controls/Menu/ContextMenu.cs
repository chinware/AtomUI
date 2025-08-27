using System.Collections.Specialized;
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

namespace AtomUI.Controls;

using AvaloniaContextMenu = Avalonia.Controls.ContextMenu;

public class ContextMenu : AvaloniaContextMenu,
                           IMotionAwareControl,
                           IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ContextMenu>();

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

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        if (container is MenuItem menuItem)
        {
            var disposables = new CompositeDisposable(1);
            disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, menuItem, MenuItem.IsMotionEnabledProperty));
            if (_itemsBindingDisposables.TryGetValue(menuItem, out var oldDisposables))
            {
                oldDisposables.Dispose();
                _itemsBindingDisposables.Remove(menuItem);
            }
            _itemsBindingDisposables.Add(menuItem, disposables);
        }

        base.PrepareContainerForItemOverride(container, item, index);
    }
    
    public override void Close()
    {
        _popup?.SetIgnoreIsOpenChanged(true);
        base.Close();
        if (_popup != null)
        {
            foreach (var childItem in Items)
            {
                if (childItem is MenuItem menuItem)
                {
                    menuItem.IsSubMenuOpen = false;
                }
            }
            _popup.IsMotionAwareOpen = false;
        }
    }
}