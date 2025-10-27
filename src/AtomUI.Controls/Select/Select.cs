using System.Reactive.Disposables;
using AtomUI.Controls.Themes;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public class Select : TemplatedControl,
                      IMotionAwareControl,
                      IControlSharedTokenResourcesHost,
                      ISizeTypeAware
{
    #region 公共属性定义
    public static readonly StyledProperty<bool> IsAllowClearProperty =
        AvaloniaProperty.Register<Select, bool>(nameof(IsAllowClear));
    
    public static readonly StyledProperty<bool> IsAutoClearSearchValueProperty =
        AvaloniaProperty.Register<Select, bool>(nameof(IsAutoClearSearchValue));
    
    public static readonly StyledProperty<bool> IsDefaultActiveFirstOptionProperty =
        AvaloniaProperty.Register<Select, bool>(nameof(IsDefaultActiveFirstOption));
    
    public static readonly StyledProperty<bool> IsDefaultOpenProperty =
        AvaloniaProperty.Register<Select, bool>(nameof(IsDefaultOpen));
    
    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        AvaloniaProperty.Register<Select, bool>(nameof(IsDropDownOpen));
    
    public static readonly StyledProperty<string?> PlaceholderTextProperty =
        AvaloniaProperty.Register<Select, string?>(nameof(PlaceholderText));
    
    public static readonly StyledProperty<IBrush?> PlaceholderForegroundProperty =
        AvaloniaProperty.Register<Select, IBrush?>(nameof(PlaceholderForeground));
    
    public static readonly StyledProperty<bool> IsPopupMatchSelectWidthProperty =
        AvaloniaProperty.Register<Select, bool>(nameof(IsPopupMatchSelectWidth));
    
    public static readonly StyledProperty<bool> IsFilterOptionProperty =
        AvaloniaProperty.Register<Select, bool>(nameof(IsFilterOption));
    
    public static readonly StyledProperty<bool> IsSearchEnabledProperty =
        AvaloniaProperty.Register<Select, bool>(nameof(IsSearchEnabled));
    
    public static readonly StyledProperty<int> DisplayPageSizeProperty = 
        AvaloniaProperty.Register<Select, int>(nameof (DisplayPageSize), 10);
    
    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<Select, bool>(nameof(IsLoading));
    
    public static readonly StyledProperty<int> MaxCountProperty =
        AvaloniaProperty.Register<Select, int>(nameof(MaxCount));
        
    public static readonly StyledProperty<int> MaxTagCountProperty =
        AvaloniaProperty.Register<Select, int>(nameof(MaxTagCount));
    
    public static readonly StyledProperty<string?> MaxTagPlaceholderProperty =
        AvaloniaProperty.Register<Select, string?>(nameof(MaxTagPlaceholder));
    
    public static readonly StyledProperty<int> MaxTagTextLengthProperty =
        AvaloniaProperty.Register<Select, int>(nameof(MaxTagPlaceholder));
    
    public static readonly StyledProperty<SelectMode> ModeProperty =
        AvaloniaProperty.Register<Select, SelectMode>(nameof(Mode));
   
    public static readonly StyledProperty<object?> LeftAddOnProperty =
        AddOnDecoratedBox.LeftAddOnProperty.AddOwner<Select>();
    
    public static readonly StyledProperty<IDataTemplate?> LeftAddOnTemplateProperty =
        AddOnDecoratedBox.LeftAddOnTemplateProperty.AddOwner<Select>();

    public static readonly StyledProperty<object?> RightAddOnProperty =
        AddOnDecoratedBox.RightAddOnProperty.AddOwner<Select>();
    
    public static readonly StyledProperty<IDataTemplate?> RightAddOnTemplateProperty =
        AddOnDecoratedBox.RightAddOnTemplateProperty.AddOwner<Select>();
    
    public static readonly StyledProperty<object?> ContentLeftAddOnProperty =
        AddOnDecoratedBox.ContentLeftAddOnProperty.AddOwner<Select>();
    
    public static readonly StyledProperty<IDataTemplate?> ContentLeftAddOnTemplateProperty =
        AddOnDecoratedBox.ContentLeftAddOnTemplateProperty.AddOwner<Select>();

    public static readonly StyledProperty<object?> ContentRightAddOnProperty =
        AddOnDecoratedBox.ContentRightAddOnProperty.AddOwner<Select>();
    
    public static readonly StyledProperty<IDataTemplate?> ContentRightAddOnTemplateProperty =
        AddOnDecoratedBox.ContentRightAddOnTemplateProperty.AddOwner<Select>();
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<Select>();

    public static readonly StyledProperty<AddOnDecoratedVariant> StyleVariantProperty =
        AddOnDecoratedBox.StyleVariantProperty.AddOwner<Select>();

    public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
        AddOnDecoratedBox.StatusProperty.AddOwner<Select>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Select>();
    
    public static readonly DirectProperty<Select, IList<SelectOption>?> SelectedOptionsProperty =
        AvaloniaProperty.RegisterDirect<Select, IList<SelectOption>?>(
            nameof(SelectedOptions),
            o => o.SelectedOptions,
            (o, v) => o.SelectedOptions = v);
    
    public static readonly StyledProperty<double> OptionFontSizeProperty =
        AvaloniaProperty.Register<Select, double>(nameof(OptionFontSize));
    
    public static readonly StyledProperty<bool> AutoScrollToSelectedOptionsProperty =
        AvaloniaProperty.Register<Select, bool>(
            nameof(AutoScrollToSelectedOptions),
            defaultValue: false);
    
    public bool IsAllowClear
    {
        get => GetValue(IsAllowClearProperty);
        set => SetValue(IsAllowClearProperty, value);
    }
    
    public bool IsAutoClearSearchValue
    {
        get => GetValue(IsAutoClearSearchValueProperty);
        set => SetValue(IsAutoClearSearchValueProperty, value);
    }
    
    public bool IsDefaultActiveFirstOption
    {
        get => GetValue(IsDefaultActiveFirstOptionProperty);
        set => SetValue(IsDefaultActiveFirstOptionProperty, value);
    }
    
    public bool IsDefaultOpen
    {
        get => GetValue(IsDefaultOpenProperty);
        set => SetValue(IsDefaultOpenProperty, value);
    }
    
    public bool IsDropDownOpen
    {
        get => GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }
    
    public string? PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }
    
    public IBrush? PlaceholderForeground
    {
        get => GetValue(PlaceholderForegroundProperty);
        set => SetValue(PlaceholderForegroundProperty, value);
    }

    public bool IsPopupMatchSelectWidth
    {
        get => GetValue(IsPopupMatchSelectWidthProperty);
        set => SetValue(IsPopupMatchSelectWidthProperty, value);
    }
    
    public bool IsFilterOption
    {
        get => GetValue(IsFilterOptionProperty);
        set => SetValue(IsFilterOptionProperty, value);
    }
    
    public bool IsSearchEnabled
    {
        get => GetValue(IsSearchEnabledProperty);
        set => SetValue(IsSearchEnabledProperty, value);
    }
    
    public int DisplayPageSize
    {
        get => GetValue(DisplayPageSizeProperty);
        set => SetValue(DisplayPageSizeProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public int MaxCount
    {
        get => GetValue(MaxCountProperty);
        set => SetValue(MaxCountProperty, value);
    }
    
    public int MaxTagCount
    {
        get => GetValue(MaxTagCountProperty);
        set => SetValue(MaxTagCountProperty, value);
    }
    
    public string? MaxTagPlaceholder
    {
        get => GetValue(MaxTagPlaceholderProperty);
        set => SetValue(MaxTagPlaceholderProperty, value);
    }
    
    public int MaxTagTextLength
    {
        get => GetValue(MaxTagTextLengthProperty);
        set => SetValue(MaxTagTextLengthProperty, value);
    }
    
    public SelectMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }
    
    public object? LeftAddOn
    {
        get => GetValue(LeftAddOnProperty);
        set => SetValue(LeftAddOnProperty, value);
    }
    
    public IDataTemplate? LeftAddOnTemplate
    {
        get => GetValue(LeftAddOnTemplateProperty);
        set => SetValue(LeftAddOnTemplateProperty, value);
    }

    public object? RightAddOn
    {
        get => GetValue(RightAddOnProperty);
        set => SetValue(RightAddOnProperty, value);
    }
    
    public IDataTemplate? RightAddOnTemplate
    {
        get => GetValue(RightAddOnTemplateProperty);
        set => SetValue(RightAddOnTemplateProperty, value);
    }
    
    public object? ContentLeftAddOn
    {
        get => GetValue(ContentLeftAddOnProperty);
        set => SetValue(ContentLeftAddOnProperty, value);
    }
    
    public IDataTemplate? ContentLeftAddOnTemplate
    {
        get => GetValue(ContentLeftAddOnTemplateProperty);
        set => SetValue(ContentLeftAddOnTemplateProperty, value);
    }

    public object? ContentRightAddOn
    {
        get => GetValue(ContentRightAddOnProperty);
        set => SetValue(ContentRightAddOnProperty, value);
    }
    
    public IDataTemplate? ContentRightAddOnTemplate
    {
        get => GetValue(ContentRightAddOnTemplateProperty);
        set => SetValue(ContentRightAddOnTemplateProperty, value);
    }
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public AddOnDecoratedVariant StyleVariant
    {
        get => GetValue(StyleVariantProperty);
        set => SetValue(StyleVariantProperty, value);
    }

    public AddOnDecoratedStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    [Content]
    public AvaloniaList<SelectOption> Options { get; set; } = new();
    
    private IList<SelectOption>? _selectedOptions;

    public IList<SelectOption>? SelectedOptions
    {
        get => _selectedOptions;
        set => SetAndRaise(SelectedOptionsProperty, ref _selectedOptions, value);
    }

    public double OptionFontSize
    {
        get => GetValue(OptionFontSizeProperty);
        set => SetValue(OptionFontSizeProperty, value);
    }
    
    public bool AutoScrollToSelectedOptions
    {
        get => GetValue(AutoScrollToSelectedOptionsProperty);
        set => SetValue(AutoScrollToSelectedOptionsProperty, value);
    }
    #endregion
    
    public IComparer<SelectOption>? FilterSortFn { get; set; }
    public Func<object?, SelectOption, bool>? FilterFn { get; set; }
    public IList<object>? DefaultSelectedOptions { get; set; }
    
    #region 公共事件定义
    
    public event EventHandler? DropDownClosed;
    public event EventHandler? DropDownOpened;

    #endregion

    #region 内部属性定义
    
    internal static readonly StyledProperty<double> ItemHeightProperty =
        AvaloniaProperty.Register<Select, double>(nameof(ItemHeight));
    
    internal static readonly StyledProperty<double> MaxPopupHeightProperty =
        AvaloniaProperty.Register<Select, double>(nameof(MaxPopupHeight));
    
    internal static readonly StyledProperty<Thickness> PopupContentPaddingProperty =
        AvaloniaProperty.Register<Select, Thickness>(nameof(PopupContentPadding));
    
    internal static readonly DirectProperty<Select, bool> IsEffectiveShowClearButtonProperty =
        AvaloniaProperty.RegisterDirect<Select, bool>(nameof(IsEffectiveShowClearButton),
            o => o.IsEffectiveShowClearButton,
            (o, v) => o.IsEffectiveShowClearButton = v);
    
    internal double ItemHeight
    {
        get => GetValue(ItemHeightProperty);
        set => SetValue(ItemHeightProperty, value);
    }
    
    internal double MaxPopupHeight
    {
        get => GetValue(MaxPopupHeightProperty);
        set => SetValue(MaxPopupHeightProperty, value);
    }
    
    internal Thickness PopupContentPadding
    {
        get => GetValue(PopupContentPaddingProperty);
        set => SetValue(PopupContentPaddingProperty, value);
    }
    
    private bool _isEffectiveShowClearButton;

    internal bool IsEffectiveShowClearButton
    {
        get => _isEffectiveShowClearButton;
        set => SetAndRaise(IsEffectiveShowClearButtonProperty, ref _isEffectiveShowClearButton, value);
    }
    
    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => SelectToken.ID;

    #endregion
    
    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new VirtualizingStackPanel());
    
    private Popup? _popup;
    private readonly CompositeDisposable _subscriptionsOnOpen = new ();
    
    static Select()
    {
        FocusableProperty.OverrideDefaultValue<Select>(true);
    }
    
    public Select()
    {
        this.RegisterResources();
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        // UpdateSelectionBoxItem(SelectedItem);
    }
    
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Handled)
        {
            return;
        }

        if ((e.Key == Key.F4 && e.KeyModifiers.HasAllFlags(KeyModifiers.Alt) == false) ||
            ((e.Key == Key.Down || e.Key == Key.Up) && e.KeyModifiers.HasAllFlags(KeyModifiers.Alt)))
        {
            SetCurrentValue(IsDropDownOpenProperty, !IsDropDownOpen);
            e.Handled = true;
        }
        else if (IsDropDownOpen && e.Key == Key.Escape)
        {
            SetCurrentValue(IsDropDownOpenProperty, false);
            e.Handled = true;
        }
        else if (!IsDropDownOpen && (e.Key == Key.Enter || e.Key == Key.Space))
        {
            SetCurrentValue(IsDropDownOpenProperty, true);
            e.Handled = true;
        }
        else if (IsDropDownOpen && (e.Key == Key.Enter || e.Key == Key.Space))
        {
            // SelectFocusedItem();
            SetCurrentValue(IsDropDownOpenProperty, false);
            e.Handled = true;
        }
        // // Ignore key buttons, if they are used for XY focus.
        // else if (!IsDropDownOpen
        //          && !XYFocusHelpers.IsAllowedXYNavigationMode(this, e.KeyDeviceType))
        // {
        //     if (e.Key == Key.Down)
        //     {
        //         e.Handled = SelectNext();
        //     }
        //     else if (e.Key == Key.Up)
        //     {
        //         e.Handled = SelectPrevious();
        //     }
        // }
        // This part of code is needed just to acquire initial focus, subsequent focus navigation will be done by ItemsControl.
        // else if (IsDropDownOpen && SelectedIndex < 0 && ItemCount > 0 &&
        //          (e.Key == Key.Up || e.Key == Key.Down) && IsFocused == true)
        // {
        //     var firstChild = Presenter?.Panel?.Children.FirstOrDefault(c => CanFocus(c));
        //     if (firstChild != null)
        //     {
        //         e.Handled = firstChild.Focus(NavigationMethod.Directional);
        //     }
        // }
    }
    
    internal void ItemFocused(SelectOptionItem selectOptionItem)
    {
        if (IsDropDownOpen && selectOptionItem.IsFocused && selectOptionItem.IsArrangeValid)
        {
            selectOptionItem.BringIntoView();
        }
    }
    
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if(!e.Handled && e.Source is Visual source)
        {
            if (_popup?.IsInsidePopup(source) == true)
            {
                e.Handled = true;
                return;
            }
        }

        if (IsDropDownOpen)
        {
            // When a drop-down is open with OverlayDismissEventPassThrough enabled and the control
            // is pressed, close the drop-down
            SetCurrentValue(IsDropDownOpenProperty, false);
            e.Handled = true;
        }
        else
        {
            PseudoClasses.Set(StdPseudoClass.Pressed, true);
        }
    }
    
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (!e.Handled && e.Source is Visual source)
        {
            // if (_popup?.IsInsidePopup(source) == true)
            // {
            //     if (UpdateSelectionFromEventSource(e.Source))
            //     {
            //         _popup?.Close();
            //         e.Handled = true;
            //     }
            // }
            // else 
            if (PseudoClasses.Contains(StdPseudoClass.Pressed))
            {
                SetCurrentValue(IsDropDownOpenProperty, !IsDropDownOpen);
                e.Handled = true;
            }
        }

        PseudoClasses.Set(StdPseudoClass.Pressed, false);
        base.OnPointerReleased(e);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_popup != null)
        {
            _popup.Opened -= PopupOpened;
            _popup.Closed -= PopupClosed;
        }

        _popup        =  e.NameScope.Get<Popup>(SelectThemeConstants.PopupPart);
        _popup.Opened += PopupOpened;
        _popup.Closed += PopupClosed;
        ConfigureMaxDropdownHeight();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == SelectedOptionsProperty)
        {
            UpdateSelectionBoxItem(change.NewValue);
            TryFocusSelectedOptions();
        }
        else if (change.Property == IsDropDownOpenProperty)
        {
            PseudoClasses.Set(SelectPseudoClass.DropdownOpen, change.GetNewValue<bool>());
        }
        else if (change.Property == DisplayPageSizeProperty ||
                 change.Property == ItemHeightProperty)
        {
            ConfigureMaxDropdownHeight();
        }

        base.OnPropertyChanged(change);
    }
    
    private void PopupClosed(object? sender, EventArgs e)
    {
        _subscriptionsOnOpen.Clear();
        DropDownClosed?.Invoke(this, EventArgs.Empty);
    }

    private void PopupOpened(object? sender, EventArgs e)
    {
        TryFocusSelectedOptions();
        
        _subscriptionsOnOpen.Clear();
        
        this.GetObservable(IsVisibleProperty).Subscribe(IsVisibleChanged).DisposeWith(_subscriptionsOnOpen);
        
        foreach (var parent in this.GetVisualAncestors().OfType<Control>())
        {
            parent.GetObservable(IsVisibleProperty).Subscribe(IsVisibleChanged).DisposeWith(_subscriptionsOnOpen);
        }
        
        DropDownOpened?.Invoke(this, EventArgs.Empty);
    }
    
    private void IsVisibleChanged(bool isVisible)
    {
        if (!isVisible && IsDropDownOpen)
        {
            SetCurrentValue(IsDropDownOpenProperty, false);
        }
    }
    
    private void TryFocusSelectedOptions()
    {
        
        // var selectedIndex = SelectedIndex;
        // if (IsDropDownOpen && selectedIndex != -1)
        // {
        //     var container = ContainerFromIndex(selectedIndex);
        //
        //     if (container == null && SelectedIndex != -1)
        //     {
        //         ScrollIntoView(Selection.SelectedIndex);
        //         container = ContainerFromIndex(selectedIndex);
        //     }
        //
        //     if (container != null && CanFocus(container))
        //     {
        //         container.Focus();
        //     }
        // }
    }
    
    private bool CanFocus(Control control) => control.Focusable && control.IsEffectivelyEnabled && control.IsVisible;
    
    private void UpdateSelectionBoxItem(object? item)
    {
        // var contentControl = item as IContentControl;
        //
        // if (contentControl != null)
        // {
        //     item = contentControl.Content;
        // }
        //
        // var control = item as Control;
        //
        // if (control != null)
        // {
        //     if (VisualRoot is object)
        //     {
        //         control.Measure(Size.Infinity);
        //
        //         SelectionBoxItem = new Rectangle
        //         {
        //             Width  = control.DesiredSize.Width,
        //             Height = control.DesiredSize.Height,
        //             Fill = new VisualBrush
        //             {
        //                 Visual     = control,
        //                 Stretch    = Stretch.None,
        //                 AlignmentX = AlignmentX.Left,
        //             }
        //         };
        //     }
        //
        //     UpdateFlowDirection();
        // }
        // else
        // {
        //     if (item is not null && ItemTemplate is null && SelectionBoxItemTemplate is null && DisplayMemberBinding is { } binding)
        //     {
        //         var template = new FuncDataTemplate<object?>((_, _) =>
        //             new TextBlock
        //             {
        //                 [TextBlock.DataContextProperty] = item,
        //                 [!TextBlock.TextProperty]       = binding,
        //             });
        //         var text = template.Build(item);
        //         SelectionBoxItem = text;
        //     }
        //     else
        //     {
        //         SelectionBoxItem = item;
        //     }
        //         
        // }
    }
    
    private void SelectFocusedItem()
    {
        // foreach (var dropdownItem in GetRealizedContainers())
        // {
        //     if (dropdownItem.IsFocused)
        //     {
        //         SelectedIndex = IndexFromContainer(dropdownItem);
        //         break;
        //     }
        // }
    }
    //
    // private bool SelectNext() => MoveSelection(SelectedIndex, 1, WrapSelection);
    // private bool SelectPrevious() => MoveSelection(SelectedIndex, -1, WrapSelection);
    
    private bool MoveSelection(int startIndex, int step, bool wrap)
    {
        // static bool IsSelectable(object? o) => (o as AvaloniaObject)?.GetValue(IsEnabledProperty) ?? true;
        //
        // var count = ItemCount;
        //
        // for (int i = startIndex + step; i != startIndex; i += step)
        // {
        //     if (i < 0 || i >= count)
        //     {
        //         if (wrap)
        //         {
        //             if (i < 0)
        //                 i += count;
        //             else if (i >= count)
        //                 i %= count;
        //         }
        //         else
        //         {
        //             return false;
        //         }
        //     }
        //
        //     var item      = ItemsView[i];
        //     var container = ContainerFromIndex(i);
        //         
        //     if (IsSelectable(item) && IsSelectable(container))
        //     {
        //         SelectedIndex = i;
        //         return true;
        //     }
        // }

        return false;
    }
    
    public void Clear()
    {
        SelectedOptions = null;
    }
    
    private void ConfigureMaxDropdownHeight()
    {
        SetCurrentValue(MaxPopupHeightProperty, ItemHeight * DisplayPageSize + PopupContentPadding.Top + PopupContentPadding.Bottom);
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Error, Status == AddOnDecoratedStatus.Error);
        PseudoClasses.Set(StdPseudoClass.Warning, Status == AddOnDecoratedStatus.Warning);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Outline, StyleVariant == AddOnDecoratedVariant.Outline);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Filled, StyleVariant == AddOnDecoratedVariant.Filled);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Borderless, StyleVariant == AddOnDecoratedVariant.Borderless);
    }
}