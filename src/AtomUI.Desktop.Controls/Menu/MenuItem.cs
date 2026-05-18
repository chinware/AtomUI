using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Icons.AntDesign;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace AtomUI.Desktop.Controls;

using AvaloniaMenuItem = Avalonia.Controls.MenuItem;

[PseudoClasses(MenuItemPseudoClass.TopLevel)]
[TemplatePart("PART_MenuIndicatorIconHost", typeof(Panel))]
public class MenuItem : AvaloniaMenuItem, IMenuItemData
{
    private const string MenuIndicatorIconName = "MenuIndicatorIcon";

    #region 公共属性定义

    public new static readonly StyledProperty<PathIcon?> IconProperty =
        AvaloniaProperty.Register<MenuItem, PathIcon?>(nameof(Icon));

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<MenuItem>();

    public static readonly StyledProperty<int> DisplayPageSizeProperty =
        Menu.DisplayPageSizeProperty.AddOwner<MenuItem>();

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public new PathIcon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public int DisplayPageSize
    {
        get => GetValue(DisplayPageSizeProperty);
        set => SetValue(DisplayPageSizeProperty, value);
    }

    #endregion

    IEnumerable<IMenuItemData> ITreeNode<IMenuItemData>.Children => Items.OfType<IMenuItemData>();
    public ITreeNode<IMenuItemData>? ParentNode => Parent as ITreeNode<IMenuItemData>;
    public EntityKey? ItemKey { get; set; }

    private Panel? _menuIndicatorIconHost;
    private RightOutlined? _menuIndicatorIcon;
    private Grid? _itemLayout;
    private Panel? _toggleItemsLayout;
    private CheckBox? _toggleCheckBox;
    private RadioButton? _toggleRadio;
    private IconPresenter? _itemIconPresenter;
    private TextBlock? _inputGestureTextBlock;
    private Popup? _submenuPopup;
    private Border? _submenuPopupFrame;
    private ScrollViewer? _submenuScrollViewer;
    private ItemsPresenter? _submenuItemsPresenter;

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

    internal static readonly StyledProperty<double> MaxPopupHeightProperty =
        AvaloniaProperty.Register<MenuItem, double>(nameof(MaxPopupHeight));

    internal static readonly StyledProperty<double> ItemHeightProperty =
        AvaloniaProperty.Register<MenuItem, double>(nameof(ItemHeight));

    internal static readonly StyledProperty<Thickness> PopupPaddingProperty =
        AvaloniaProperty.Register<MenuItem, Thickness>(nameof(PopupPadding));

    internal static readonly StyledProperty<bool> ShouldUseOverlayPopupProperty =
        AvaloniaProperty.Register<MenuItem, bool>(nameof(ShouldUseOverlayPopup));

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    internal double MaxPopupHeight
    {
        get => GetValue(MaxPopupHeightProperty);
        set => SetValue(MaxPopupHeightProperty, value);
    }

    internal double ItemHeight
    {
        get => GetValue(ItemHeightProperty);
        set => SetValue(ItemHeightProperty, value);
    }

    internal Thickness PopupPadding
    {
        get => GetValue(PopupPaddingProperty);
        set => SetValue(PopupPaddingProperty, value);
    }

    internal bool ShouldUseOverlayPopup
    {
        get => GetValue(ShouldUseOverlayPopupProperty);
        set => SetValue(ShouldUseOverlayPopupProperty, value);
    }

    #endregion

    static MenuItem()
    {
        AffectsRender<MenuItem>(BackgroundProperty);
        AffectsMeasure<MenuItem>(IconProperty);
        AutoScrollToSelectedItemProperty.OverrideDefaultValue<MenuItem>(false);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == IsSubMenuOpenProperty && change.GetNewValue<bool>())
        {
            EnsureSubmenuPopupContent();
        }

        base.OnPropertyChanged(change);
        if (change.Property == ParentProperty)
        {
            UpdatePseudoClasses();
            UpdateToggleContent();
            ConfigureSubmenuItemsPresenterKeyboardNavigation();
        }
        else if (change.Property == IconProperty)
        {
            if (change.OldValue is Icon oldIcon)
            {
                oldIcon.SetTemplatedParent(null);
            }

            if (change.NewValue is Icon newIcon)
            {
                LogicalChildren.Remove(newIcon);
                newIcon.SetTemplatedParent(this);
            }

            UpdateItemIconPresenter();
        }
        else if (change.Property == IsCheckedProperty)
        {
            RaiseEvent(new RoutedEventArgs(IsCheckStateChangedEvent, this));
        }
        else if (change.Property == InputGestureProperty)
        {
            UpdateInputGestureTextBlock();
        }
        else if (change.Property == ToggleTypeProperty)
        {
            UpdateToggleContent();
        }
        else if (change.Property == IsSubMenuOpenProperty)
        {
            SyncSubmenuPopupOpenState();
        }
        else if (change.Property == DisplayPageSizeProperty ||
                 change.Property == ItemHeightProperty)
        {
            ConfigureMaxPopupHeight();
        }
        else if (change.Property == ItemCountProperty)
        {
            UpdateMenuIndicatorIcon();
            UpdateToggleContent();
            if (ItemCount == 0)
            {
                DetachSubmenuPopupContent();
            }
            else if (IsSubMenuOpen)
            {
                EnsureSubmenuPopupContent();
            }
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(MenuItemPseudoClass.TopLevel, IsTopLevel);
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
            if (item != null && item is not Visual)
            {
                if (!menuItem.IsSet(HeaderProperty))
                {
                    menuItem.SetCurrentValue(HeaderProperty, item);
                }

                if (item is IMenuItemData menuItemData)
                {
                    if (!menuItem.IsSet(IconProperty))
                    {
                        menuItem.SetCurrentValue(IconProperty, menuItemData.Icon);
                    }

                    if (menuItem.ItemKey == null)
                    {
                        menuItem.ItemKey = menuItemData.ItemKey;
                    }

                    if (!menuItem.IsSet(IsEnabledProperty))
                    {
                        menuItem.SetCurrentValue(IsEnabledProperty, menuItemData.IsEnabled);
                    }

                    if (!menuItem.IsSet(InputGestureProperty))
                    {
                        menuItem.SetCurrentValue(InputGestureProperty, menuItemData.InputGesture);
                    }
                }
            }

            if (ItemTemplate != null)
            {
                menuItem[!HeaderTemplateProperty] = this[!ItemTemplateProperty];
            }

            menuItem[!ItemTemplateProperty]          = this[!ItemTemplateProperty];
            menuItem[!SizeTypeProperty]              = this[!SizeTypeProperty];
            menuItem[!IsMotionEnabledProperty]       = this[!IsMotionEnabledProperty];
            menuItem[!ShouldUseOverlayPopupProperty] = this[!ShouldUseOverlayPopupProperty];
            PrepareMenuItem(menuItem, item, index);
        }
        else if (container is MenuSeparator menuSeparator)
        {
            menuSeparator.Orientation = Orientation.Horizontal;
        }
        else if (container is not MenuSeparator)
        {
            throw new ArgumentOutOfRangeException(nameof(container),
                "The container type is incorrect, it must be type MenuItem or MenuSeparator.");
        }
    }

    protected virtual void PrepareMenuItem(MenuItem menuItem, object? item, int index)
    {
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachMenuIndicatorIcon();
        DetachToggleContent();
        DetachItemIconPresenter();
        DetachInputGestureTextBlock();
        DetachSubmenuPopup();
        base.OnApplyTemplate(e);
        _itemLayout            = e.NameScope.Find<Grid>("PART_ItemLayout");
        _toggleItemsLayout     = e.NameScope.Find<Panel>("ToggleItemsLayout");
        _menuIndicatorIconHost = e.NameScope.Find<Panel>("PART_MenuIndicatorIconHost");
        _submenuPopup          = e.NameScope.Find<Popup>("PART_Popup");
        if (_submenuPopup != null)
        {
            _submenuPopup.Opened += HandleSubmenuPopupOpened;
            _submenuPopup.Closed += HandleSubmenuPopupClosed;
        }

        UpdatePseudoClasses();
        ConfigureMaxPopupHeight();
        UpdateMenuIndicatorIcon();
        UpdateToggleContent();
        UpdateItemIconPresenter();
        UpdateInputGestureTextBlock();
        if (IsSubMenuOpen)
        {
            EnsureSubmenuPopupContent();
        }
        SyncSubmenuPopupOpenState();
    }

    public async Task CloseItemAsync(CancellationToken cancellationToken = default)
    {
        for (var i = 0; i < ItemCount; i++)
        {
            var container = ContainerFromIndex(i);
            if (container is MenuItem childMenuItem)
            {
                await childMenuItem.CloseItemAsync(cancellationToken);
            }
        }

        IsSubMenuOpen = false;
    }

    private void ConfigureMaxPopupHeight()
    {
        SetCurrentValue(MaxPopupHeightProperty,
            ItemHeight * DisplayPageSize + PopupPadding.Top + PopupPadding.Bottom);
    }

    private void UpdateMenuIndicatorIcon()
    {
        if (_menuIndicatorIconHost is null)
        {
            return;
        }

        if (ItemCount == 0)
        {
            DetachMenuIndicatorIcon();
            return;
        }

        if (_menuIndicatorIcon is not null)
        {
            return;
        }

        _menuIndicatorIcon = new RightOutlined
        {
            Name                = MenuIndicatorIconName,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment   = VerticalAlignment.Center
        };
        _menuIndicatorIcon.SetTemplatedParent(this);
        _menuIndicatorIconHost.Children.Add(_menuIndicatorIcon);
    }

    private void DetachMenuIndicatorIcon()
    {
        if (_menuIndicatorIcon is null)
        {
            return;
        }

        _menuIndicatorIconHost?.Children.Remove(_menuIndicatorIcon);
        _menuIndicatorIcon.SetTemplatedParent(null);
        _menuIndicatorIcon = null;
    }

    private bool ShouldShowToggleContent()
    {
        return !IsTopLevel && ToggleType != MenuItemToggleType.None && ItemCount == 0;
    }

    private void UpdateToggleContent()
    {
        if (_toggleItemsLayout is null)
        {
            return;
        }

        if (!ShouldShowToggleContent())
        {
            DetachToggleContent();
            return;
        }

        if (ToggleType == MenuItemToggleType.CheckBox)
        {
            DetachToggleRadio();
            EnsureToggleCheckBox();
        }
        else if (ToggleType == MenuItemToggleType.Radio)
        {
            DetachToggleCheckBox();
            EnsureToggleRadio();
        }
    }

    private void EnsureToggleCheckBox()
    {
        if (_toggleItemsLayout is null || _toggleCheckBox is not null)
        {
            return;
        }

        _toggleCheckBox = new CheckBox
        {
            Name                = "PART_ToggleCheckbox",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center,
            IsVisible           = true,
            IsWaveSpiritEnabled = false
        };
        _toggleCheckBox[!ToggleButton.IsCheckedProperty] = this[!IsCheckedProperty];
        _toggleCheckBox.SetTemplatedParent(this);
        _toggleItemsLayout.Children.Add(_toggleCheckBox);
    }

    private void EnsureToggleRadio()
    {
        if (_toggleItemsLayout is null || _toggleRadio is not null)
        {
            return;
        }

        _toggleRadio = new RadioButton
        {
            Name                = "PART_ToggleRadio",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center,
            IsVisible           = true,
            IsWaveSpiritEnabled = false
        };
        _toggleRadio[!ToggleButton.IsCheckedProperty]   = this[!IsCheckedProperty];
        _toggleRadio[!RadioButton.GroupNameProperty]    = this[!GroupNameProperty];
        _toggleRadio.SetTemplatedParent(this);
        _toggleItemsLayout.Children.Add(_toggleRadio);
    }

    private void DetachToggleContent()
    {
        DetachToggleCheckBox();
        DetachToggleRadio();
    }

    private void DetachToggleCheckBox()
    {
        if (_toggleCheckBox is null)
        {
            return;
        }

        _toggleItemsLayout?.Children.Remove(_toggleCheckBox);
        _toggleCheckBox.ClearValue(ToggleButton.IsCheckedProperty);
        _toggleCheckBox.SetTemplatedParent(null);
        _toggleCheckBox = null;
    }

    private void DetachToggleRadio()
    {
        if (_toggleRadio is null)
        {
            return;
        }

        _toggleItemsLayout?.Children.Remove(_toggleRadio);
        _toggleRadio.ClearValue(ToggleButton.IsCheckedProperty);
        _toggleRadio.ClearValue(RadioButton.GroupNameProperty);
        _toggleRadio.SetTemplatedParent(null);
        _toggleRadio = null;
    }

    private void UpdateItemIconPresenter()
    {
        if (_itemLayout is null)
        {
            return;
        }

        if (Icon is null)
        {
            DetachItemIconPresenter();
            return;
        }

        if (_itemIconPresenter is not null)
        {
            return;
        }

        _itemIconPresenter = new IconPresenter
        {
            Name                = "ItemIconPresenter",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center
        };
        Grid.SetColumn(_itemIconPresenter, 1);
        _itemIconPresenter[!IconPresenter.IconProperty]            = this[!IconProperty];
        _itemIconPresenter[!IconPresenter.IsMotionEnabledProperty] = this[!IsMotionEnabledProperty];
        _itemIconPresenter.SetTemplatedParent(this);
        _itemLayout.Children.Add(_itemIconPresenter);
    }

    private void DetachItemIconPresenter()
    {
        if (_itemIconPresenter is null)
        {
            return;
        }

        _itemLayout?.Children.Remove(_itemIconPresenter);
        _itemIconPresenter.ClearValue(IconPresenter.IconProperty);
        _itemIconPresenter.ClearValue(IconPresenter.IsMotionEnabledProperty);
        _itemIconPresenter.SetTemplatedParent(null);
        _itemIconPresenter = null;
    }

    private void UpdateInputGestureTextBlock()
    {
        if (_itemLayout is null)
        {
            return;
        }

        if (InputGesture is null)
        {
            DetachInputGestureTextBlock();
            return;
        }

        if (_inputGestureTextBlock is null)
        {
            _inputGestureTextBlock = new TextBlock
            {
                Name                = "InputGestureText",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment   = VerticalAlignment.Center,
                TextAlignment       = Avalonia.Media.TextAlignment.Right
            };
            Grid.SetColumn(_inputGestureTextBlock, 3);
            _inputGestureTextBlock.SetTemplatedParent(this);
            _itemLayout.Children.Add(_inputGestureTextBlock);
        }

        _inputGestureTextBlock.Text = PlatformKeyGestureConverter.ToPlatformString(InputGesture);
    }

    private void DetachInputGestureTextBlock()
    {
        if (_inputGestureTextBlock is null)
        {
            return;
        }

        _itemLayout?.Children.Remove(_inputGestureTextBlock);
        _inputGestureTextBlock.Text = null;
        _inputGestureTextBlock.SetTemplatedParent(null);
        _inputGestureTextBlock = null;
    }

    private void HandleSubmenuPopupOpened(object? sender, EventArgs e)
    {
        EnsureSubmenuPopupContent();
    }

    private void HandleSubmenuPopupClosed(object? sender, EventArgs e)
    {
        if (IsSubMenuOpen)
        {
            SetCurrentValue(IsSubMenuOpenProperty, false);
        }
    }

    private void SyncSubmenuPopupOpenState()
    {
        if (_submenuPopup is null)
        {
            return;
        }

        if (IsSubMenuOpen)
        {
            EnsureSubmenuPopupContent();
        }

        if (_submenuPopup.IsOpen != IsSubMenuOpen)
        {
            _submenuPopup.IsOpen = IsSubMenuOpen;
        }
    }

    private void EnsureSubmenuPopupContent()
    {
        if (_submenuPopup is null || ItemCount == 0 || _submenuPopupFrame is not null)
        {
            return;
        }

        _submenuItemsPresenter = new ItemsPresenter
        {
            Name = "PART_ItemsPresenter"
        };
        _submenuItemsPresenter[!ItemsPresenter.ItemsPanelProperty] = this[!ItemsPanelProperty];
        Grid.SetIsSharedSizeScope(_submenuItemsPresenter, true);
        ConfigureSubmenuItemsPresenterKeyboardNavigation();

        _submenuScrollViewer = new ScrollViewer
        {
            IsScrollChainingEnabled = false,
            Content                 = _submenuItemsPresenter
        };
        _submenuScrollViewer[!ScrollViewer.IsMotionEnabledProperty] = this[!IsMotionEnabledProperty];
        _submenuScrollViewer[!ScrollViewer.IsLiteModeProperty]      = this[!ScrollViewer.IsLiteModeProperty];
        _submenuScrollViewer[!ScrollViewer.AllowAutoHideProperty]   = this[!ScrollViewer.AllowAutoHideProperty];

        _submenuPopupFrame = new Border
        {
            Name  = "PopupFrame",
            Child = _submenuScrollViewer
        };
        _submenuPopupFrame[!Border.MaxHeightProperty] = this[!MaxPopupHeightProperty];
        _submenuPopupFrame[!Border.PaddingProperty]   = this[!PopupPaddingProperty];

        _submenuItemsPresenter.SetTemplatedParent(this);
        _submenuScrollViewer.SetTemplatedParent(this);
        _submenuPopupFrame.SetTemplatedParent(this);
        _submenuPopup.Child = _submenuPopupFrame;
    }

    private void ConfigureSubmenuItemsPresenterKeyboardNavigation()
    {
        if (_submenuItemsPresenter is null)
        {
            return;
        }

        KeyboardNavigation.SetTabNavigation(_submenuItemsPresenter,
            IsTopLevel ? KeyboardNavigationMode.Continue : KeyboardNavigationMode.Local);
    }

    private void DetachSubmenuPopup()
    {
        if (_submenuPopup is not null)
        {
            _submenuPopup.Opened -= HandleSubmenuPopupOpened;
            _submenuPopup.Closed -= HandleSubmenuPopupClosed;
        }
        DetachSubmenuPopupContent();
        _submenuPopup = null;
    }

    private void DetachSubmenuPopupContent()
    {
        if (_submenuPopupFrame is null)
        {
            return;
        }

        if (_submenuPopup?.Child == _submenuPopupFrame)
        {
            _submenuPopup.Child = null;
        }

        _submenuPopupFrame.Child = null;
        _submenuPopupFrame.ClearValue(Border.MaxHeightProperty);
        _submenuPopupFrame.ClearValue(Border.PaddingProperty);
        _submenuPopupFrame.SetTemplatedParent(null);

        if (_submenuScrollViewer is not null)
        {
            _submenuScrollViewer.Content = null;
            _submenuScrollViewer.ClearValue(ScrollViewer.IsMotionEnabledProperty);
            _submenuScrollViewer.ClearValue(ScrollViewer.IsLiteModeProperty);
            _submenuScrollViewer.ClearValue(ScrollViewer.AllowAutoHideProperty);
            _submenuScrollViewer.SetTemplatedParent(null);
        }

        if (_submenuItemsPresenter is not null)
        {
            _submenuItemsPresenter.ClearValue(ItemsPresenter.ItemsPanelProperty);
            _submenuItemsPresenter.SetTemplatedParent(null);
        }

        _submenuPopupFrame     = null;
        _submenuScrollViewer   = null;
        _submenuItemsPresenter = null;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.DisableTransitions();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Dispatcher.Post(this.EnableTransitions);
    }
}
