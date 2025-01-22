using System.Collections;
using System.ComponentModel;
using System.Reflection;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Metadata;
using Avalonia.Styling;

namespace AtomUI.Controls;

public class MenuFlyout : Flyout
{
    private static readonly MethodInfo SetItemsSourceMethodInfo;
    public Func<IPopupHostProvider, RawPointerEventArgs, bool>? ClickHideFlyoutPredicate;
    
    static MenuFlyout()
    {
        SetItemsSourceMethodInfo =
            typeof(ItemCollection).GetMethod("SetItemsSource", BindingFlags.Instance | BindingFlags.NonPublic)!;
    }

    public MenuFlyout()
    {
        var itemCollectionType = typeof(ItemCollection);
        Items = (ItemCollection)Activator.CreateInstance(itemCollectionType, true)!;
    }

    /// <summary>
    /// Defines the <see cref="ItemsSource" /> property
    /// </summary>
    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        AvaloniaProperty.Register<MenuFlyout, IEnumerable?>(
            nameof(ItemsSource));

    /// <summary>
    /// Defines the <see cref="ItemTemplate" /> property
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
        AvaloniaProperty.Register<MenuFlyout, IDataTemplate?>(nameof(ItemTemplate));
    
    public static readonly StyledProperty<ControlTheme?> ItemContainerThemeProperty =
        ItemsControl.ItemContainerThemeProperty.AddOwner<MenuFlyout>();

    [Content] public ItemCollection Items { get; }

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

    protected override Control CreatePresenter()
    {
        var presenter = new MenuFlyoutPresenter
        {
            ItemsSource                                = Items,
            [!ItemsControl.ItemTemplateProperty]       = this[!ItemTemplateProperty],
            [!ItemsControl.ItemContainerThemeProperty] = this[!ItemContainerThemeProperty],
            MenuFlyout                                 = new WeakReference<MenuFlyout>(this)
        };
        BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, presenter, IsShowArrowProperty);
        SetupArrowPosition(Popup, presenter);
        CalculateShowArrowEffective();

        return presenter;
    }

    private void SetupArrowPosition(Popup popup, MenuFlyoutPresenter? flyoutPresenter = null)
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

    protected override void OnOpened()
    {
        base.OnOpened();
        var inputManager = AvaloniaLocator.Current.GetService<IInputManager>()!;
        _compositeDisposable?.Add(inputManager.Process.Subscribe(HandleMouseClick));
    }

    private void HandleMouseClick(RawInputEventArgs args)
    {
        if (!IsOpen)
        {
            return;
        }
        if (args is RawPointerEventArgs pointerEventArgs)
        {
            if (pointerEventArgs.Type == RawPointerEventType.LeftButtonUp)
            {
                if (this is IPopupHostProvider popupHostProvider)
                {
                    if (ClickHideFlyoutPredicate is not null)
                    {
                        if (ClickHideFlyoutPredicate(popupHostProvider, pointerEventArgs))
                        {
                            Hide();
                        }
                    }
                    else
                    {
                        if (popupHostProvider.PopupHost != pointerEventArgs.Root)
                        {
                            Hide();
                        }
                    }
                }
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ItemsSourceProperty)
        {
            SetItemsSourceMethodInfo.Invoke(Items, new object?[] { change.GetNewValue<IEnumerable?>() });
        }
    }

    protected internal override void NotifyPopupCreated(Popup popup)
    {
        base.NotifyPopupCreated(popup);
        popup.IsLightDismissEnabled = false;
        popup.IsDetectMouseClickEnabled = false;
    }
}