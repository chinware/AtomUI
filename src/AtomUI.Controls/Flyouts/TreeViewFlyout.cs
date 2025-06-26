using System.Collections;
using System.ComponentModel;
using AtomUI.Controls.Utils;
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

public class TreeViewFlyout : Flyout
{
    #region 公共属性定义

    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        AvaloniaProperty.Register<TreeViewFlyout, IEnumerable?>(
            nameof(ItemsSource));

    public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
        AvaloniaProperty.Register<TreeViewFlyout, IDataTemplate?>(nameof(ItemTemplate));

    public static readonly StyledProperty<ControlTheme?> ItemContainerThemeProperty =
        ItemsControl.ItemContainerThemeProperty.AddOwner<TreeViewFlyout>();

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

    #endregion

    public Func<IPopupHostProvider, RawPointerEventArgs, bool>? ClickHideFlyoutPredicate;

    private IDisposable? _detectMouseClickDisposable;

    [Content] public ItemCollection Items { get; }

    public TreeViewFlyout()
    {
        var itemCollectionType = typeof(ItemCollection);
        Items = (ItemCollection)Activator.CreateInstance(itemCollectionType, true)!;
    }

    protected override Control CreatePresenter()
    {
        var presenter = new TreeViewFlyoutPresenter
        {
            ItemsSource                                = Items,
            [!ItemsControl.ItemTemplateProperty]       = this[!ItemTemplateProperty],
            [!ItemsControl.ItemContainerThemeProperty] = this[!ItemContainerThemeProperty],
            TreeViewFlyout                             = this
        };
        BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, presenter, TreeViewFlyoutPresenter.IsShowArrowProperty);
        BindUtils.RelayBind(this, IsMotionEnabledProperty, presenter, TreeViewFlyoutPresenter.IsMotionEnabledProperty);
        SetupArrowPosition(Popup, presenter);
        CalculateShowArrowEffective();

        return presenter;
    }

    protected void SetupArrowPosition(Popup popup, TreeViewFlyoutPresenter? flyoutPresenter = null)
    {
        if (flyoutPresenter is null)
        {
            var child = popup.Child;
            if (child is TreeViewFlyoutPresenter childPresenter)
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
        if (IsDetectMouseClickEnabled)
        {
            _detectMouseClickDisposable = inputManager.Process.Subscribe(HandleMouseClick);
            CompositeDisposable?.Add(_detectMouseClickDisposable);
        }
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
            Items.SetItemsSource(change.GetNewValue<IEnumerable?>());
        }
        else if (change.Property == IsDetectMouseClickEnabledProperty && IsOpen)
        {
            if (IsDetectMouseClickEnabled)
            {
                if (_detectMouseClickDisposable is not null)
                {
                    CompositeDisposable?.Remove(_detectMouseClickDisposable);
                }

                var inputManager = AvaloniaLocator.Current.GetService<IInputManager>()!;
                _detectMouseClickDisposable = inputManager.Process.Subscribe(HandleMouseClick);
                CompositeDisposable?.Add(_detectMouseClickDisposable);
            }
            else
            {
                if (_detectMouseClickDisposable is not null)
                {
                    CompositeDisposable?.Remove(_detectMouseClickDisposable);
                }
            }
        }
    }

    protected internal override void NotifyPopupCreated(Popup popup)
    {
        base.NotifyPopupCreated(popup);
        popup.IsLightDismissEnabled = false;
    }
}