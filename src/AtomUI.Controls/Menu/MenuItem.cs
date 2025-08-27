using System.Collections.Specialized;
using System.Reactive.Disposables;
using AtomUI.Animations;
using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.IconPkg;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

using AvaloniaMenuItem = Avalonia.Controls.MenuItem;

[PseudoClasses(MenuItemPseudoClass.TopLevel)]
public class MenuItem : AvaloniaMenuItem
{
    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<MenuItem>();

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    #endregion

    #region 公共事件定义

    public static readonly RoutedEvent<RoutedEventArgs> IsCheckStateChangedEvent = 
        RoutedEvent.Register<MenuItem, RoutedEventArgs>(nameof(IsCheckStateChanged), RoutingStrategies.Bubble);
    
    public event EventHandler<RoutedEventArgs>? IsCheckStateChanged
    {
        add => AddHandler(IsCheckStateChangedEvent, value);
        remove => RemoveHandler(IsCheckStateChangedEvent, value);
    }
    
    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<MenuItem>();

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    internal PopupRoot? SubmenuPopupRoot => _popup?.Host as PopupRoot;
    
    private Border? _itemDecorator;
    private ContentPresenter? _headerPresenter;
    private RadioButton? _radioButton;
    private CheckBox? _checkBox;
    private Popup? _popup;
    private Dictionary<MenuItem, CompositeDisposable> _itemsBindingDisposables = new();
    
    static MenuItem()
    {
        AffectsRender<MenuItem>(BackgroundProperty);
        AffectsMeasure<MenuItem>(IconProperty);
    }

    public MenuItem()
    {
        Items.CollectionChanged  += HandleItemsCollectionChanged;
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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ParentProperty)
        {
            UpdatePseudoClasses();
        }
        else if (change.Property == IconProperty)
        {
            // 不要删掉，因为父类添加了，删除这几行会导致 icon 到 icon presenter 失败
            if (change.OldValue is Icon oldIcon)
            {
                oldIcon.SetTemplatedParent(null);
            }

            if (change.NewValue is Icon newIcon)
            {
                LogicalChildren.Remove(newIcon);
                newIcon.SetTemplatedParent(this);
            }
        }
        else if (change.Property == IsCheckedProperty)
        {
            RaiseEvent(new RoutedEventArgs(IsCheckStateChangedEvent, this));
        }

        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureHeaderPresenterTransitions(true);
                ConfigureItemDecoratorTransitions(true);
            }
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(MenuItemPseudoClass.TopLevel, IsTopLevel);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        if (container is MenuItem menuItem)
        {
            var disposables = new CompositeDisposable(2);
            disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, menuItem, MenuItem.IsMotionEnabledProperty));
            disposables.Add(BindUtils.RelayBind(this, SizeTypeProperty, menuItem, MenuItem.SizeTypeProperty));
            if (_itemsBindingDisposables.TryGetValue(menuItem, out var oldDisposables))
            {
                oldDisposables.Dispose();
                _itemsBindingDisposables.Remove(menuItem);
            }
            _itemsBindingDisposables.Add(menuItem, disposables);
        }

        base.PrepareContainerForItemOverride(container, item, index);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _itemDecorator       = e.NameScope.Find<Border>(MenuItemThemeConstants.ItemDecoratorPart);
        _headerPresenter = e.NameScope.Find<ContentPresenter>(TopLevelMenuItemThemeConstants.HeaderPresenterPart);
        _radioButton         = e.NameScope.Find<RadioButton>(MenuItemThemeConstants.ToggleRadioPart);
        _checkBox            = e.NameScope.Find<CheckBox>(MenuItemThemeConstants.ToggleCheckboxPart);
        _popup               = e.NameScope.Find<Popup>(MenuItemThemeConstants.PopupPart);
        if (_popup != null)
        {
            _popup.ClickHidePredicate = MenuPopupClosePredicate;
        }
        if (_radioButton != null)
        {
            _radioButton.IsCheckedChanged += (sender, args) =>
            {
                if (_radioButton.IsVisible)
                {
                    IsChecked = _radioButton.IsChecked == true;
                }
            };
        }

        if (_checkBox != null)
        {
            _checkBox.IsCheckedChanged += (sender, args) =>
            {
                if (_checkBox.IsVisible)
                {
                    IsChecked = _checkBox.IsChecked == true;
                }
            };
        }

        if (_itemDecorator != null)
        {
            _itemDecorator.Loaded += (sender, args) =>
            {
                ConfigureItemDecoratorTransitions(false);
            };
            _itemDecorator.Unloaded += (sender, args) =>
            {
                _itemDecorator.Transitions = null;
            };
        }

        if (_headerPresenter != null)
        {
            _headerPresenter.Loaded += (sender, args) =>
            {
                ConfigureHeaderPresenterTransitions(false);
            };
            _headerPresenter.Unloaded += (sender, args) =>
            {
                _headerPresenter.Transitions = null;
            };
        }
        
        UpdatePseudoClasses();
    }

    private bool MenuPopupClosePredicate(IPopupHostProvider hostProvider, RawPointerEventArgs args)
    {
        var popupRoots = CollectPopupRoots(this);
        
        return !popupRoots.Contains(args.Root);
    }

    internal static HashSet<PopupRoot> CollectPopupRoots(MenuItem menuItem)
    {
        var popupRoots = new HashSet<PopupRoot>();
        if (menuItem.IsSubMenuOpen)
        {
            if (menuItem.SubmenuPopupRoot != null)
            {
                popupRoots.Add(menuItem.SubmenuPopupRoot);
            }
        }

        foreach (var child in menuItem.Items)
        {
            if (child is MenuItem childMenuItem)
            {
                var childPopupRoots = CollectPopupRoots(childMenuItem);
                popupRoots.UnionWith(childPopupRoots);
            }
        }
        return popupRoots;
    }

    private void ConfigureItemDecoratorTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (_itemDecorator != null)
            {
                if (force || _itemDecorator.Transitions == null)
                {
                    _itemDecorator.Transitions = [
                        TransitionUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty)
                    ];
                }
            }
        }
        else
        {
            if (_itemDecorator != null)
            {
                _itemDecorator.Transitions = null;
            }
        }
    }
    
    private void ConfigureHeaderPresenterTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (IsTopLevel)
            {
                if (_headerPresenter != null)
                {
                    if (force || _headerPresenter.Transitions == null)
                    {
                        _headerPresenter.Transitions = [
                            TransitionUtils.CreateTransition<SolidColorBrushTransition>(ContentPresenter.BackgroundProperty),
                            TransitionUtils.CreateTransition<SolidColorBrushTransition>(ContentPresenter.ForegroundProperty)
                        ];
                    }
                }
            }
        }
        else
        {
            if (_headerPresenter != null)
            {
                _headerPresenter.Transitions = null;
            }
        }
    }
}