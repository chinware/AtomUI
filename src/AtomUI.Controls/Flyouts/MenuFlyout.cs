using System.Collections;
using System.ComponentModel;
using System.Reactive.Disposables;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Templates;
using Avalonia.Input.Raw;
using Avalonia.Metadata;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public class MenuFlyout : Flyout
{
    #region 公共属性定义

    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        AvaloniaProperty.Register<MenuFlyout, IEnumerable?>(
            nameof(ItemsSource));
    
    public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
        AvaloniaProperty.Register<MenuFlyout, IDataTemplate?>(nameof(ItemTemplate));
    
    public static readonly StyledProperty<ControlTheme?> ItemContainerThemeProperty =
        ItemsControl.ItemContainerThemeProperty.AddOwner<MenuFlyout>();
    
    /// <summary>
    /// Gets or sets the items of the MenuFlyout
    /// </summary>
    public IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the template used for the items
    /// </summary>
    public IDataTemplate? ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    [Content] 
    public ItemCollection Items { get; }
    
    public Func<IPopupHostProvider, RawPointerEventArgs, bool>? ClickHideFlyoutPredicate;
    #endregion
    
    private MenuFlyoutPresenter? _presenter;
    private CompositeDisposable? _presenterBindingDisposables;
    
    public MenuFlyout()
    {
        var itemCollectionType = typeof(ItemCollection);
        Items = (ItemCollection)Activator.CreateInstance(itemCollectionType, true)!;
    }
    
    protected override Control CreatePresenter()
    {
        _presenterBindingDisposables?.Dispose();
        _presenterBindingDisposables = new CompositeDisposable(4);
        _presenter = new MenuFlyoutPresenter
        {
            ItemsSource = Items,
            MenuFlyout  = this
        };
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, ItemTemplateProperty, _presenter, MenuFlyoutPresenter.ItemTemplateProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, ItemContainerThemeProperty, _presenter, MenuFlyoutPresenter.ItemContainerThemeProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, _presenter, MenuFlyoutPresenter.IsShowArrowProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, _presenter, MenuFlyoutPresenter.IsMotionEnabledProperty));
        SetupArrowPosition(Popup, _presenter);
        CalculateShowArrowEffective();

        return _presenter;
    }

    protected void SetupArrowPosition(Popup popup, MenuFlyoutPresenter? flyoutPresenter = null)
    {
        if (flyoutPresenter is null)
        {
            var child = popup.Child;
            if (child is MenuFlyoutPresenter childPresenter)
            {
                flyoutPresenter = childPresenter;
            }
        }

        var placement = popup.Placement;
        var anchor    = popup.PlacementAnchor;
        var gravity   = popup.PlacementGravity;

        if (flyoutPresenter is not null)
        {
            var arrowPosition = PopupUtils.CalculateArrowPosition(placement, anchor, gravity);
            if (arrowPosition.HasValue)
            {
                flyoutPresenter.ArrowPosition = arrowPosition.Value;
            }
        }
    }

    protected override void OnOpening(CancelEventArgs args)
    {
        if (Popup.Child is { } presenter)
        {
            if (FlyoutPresenterTheme is { } theme)
            {
                presenter.SetValue(StyledElement.ThemeProperty, theme);
            }
        }

        base.OnOpening(args);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ItemsSourceProperty)
        {
            Items.SetItemsSource(change.GetNewValue<IEnumerable?>());   
        }
    }

    protected internal override void NotifyPopupCreated(Popup popup)
    {
        base.NotifyPopupCreated(popup);
        popup.IsLightDismissEnabled     = false;
        popup.IsDetectMouseClickEnabled = true;
        popup.ClickHidePredicate        = ClickHideFlyoutPredicate;
        popup.CloseAction               = PopupCloseAction;
    }

    private void PopupCloseAction(Popup popup)
    {
        Hide();
    }
    
    protected override bool HideCore(bool canCancel = true)
    {
        if (!IsOpen)
        {
            return false;
        }

        if (canCancel)
        {
            if (CancelClosing())
            {
                return false;
            }
        }

        if (Popup.PlacementTarget?.GetVisualRoot() is null)
        {
            return base.HideCore(false);
        }
        
        NotifyAboutToClose();
        
        if (_presenter != null)
        {
            if (IsMotionEnabled)
            {
                Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    foreach (var childItem in _presenter.Items)
                    {
                        if (childItem is MenuItem menuItem)
                        {
                            await menuItem.CloseItemAsync();
                        }
                    }
                    IsOpen                  = false;
                    Popup.MotionAwareClose(HandlePopupClosed);
                });
            }
            else
            {
                foreach (var childItem in _presenter.Items)
                {
                    if (childItem is MenuItem menuItem)
                    {
                        menuItem.Close();
                    }
                }
            
                IsOpen = false;
                Popup.MotionAwareClose(HandlePopupClosed);
            }
        }

        return true;
    }
}