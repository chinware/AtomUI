using System.Collections;
using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Controls.Data;
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

public enum SelectMode
{
    Single,
    Multiple,
    Tags
}

public class Select : TemplatedControl,
                      IMotionAwareControl,
                      IControlSharedTokenResourcesHost,
                      ISizeTypeAware
{
    #region 公共属性定义
    public static readonly StyledProperty<IEnumerable<SelectOption>?> OptionsSourceProperty =
        AvaloniaProperty.Register<Select, IEnumerable<SelectOption>?>(nameof(OptionsSource));
    
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
        AvaloniaProperty.Register<Select, bool>(nameof(IsPopupMatchSelectWidth), true);
    
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
    
    public static readonly StyledProperty<string> OptionFilterPropProperty =
        AvaloniaProperty.Register<Select, string>(
            nameof(OptionFilterProp), nameof(SelectedOption.Value));
    
    public IEnumerable<SelectOption>? OptionsSource
    {
        get => GetValue(OptionsSourceProperty);
        set => SetValue(OptionsSourceProperty, value);
    }
    
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
    
    public string OptionFilterProp
    {
        get => GetValue(OptionFilterPropProperty);
        set => SetValue(OptionFilterPropProperty, value);
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
    public Func<object, object, bool>? FilterFn { get; set; }
    
    public Func<object, SelectOption, bool>? DefaultValueCompareFn { get; set; }
    public IList<object>? DefaultValues { get; set; }
    
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
    
    internal static readonly DirectProperty<Select, double> EffectivePopupWidthProperty =
        AvaloniaProperty.RegisterDirect<Select, double>(
            nameof(EffectivePopupWidth),
            o => o.EffectivePopupWidth,
            (o, v) => o.EffectivePopupWidth = v);
    
    internal static readonly DirectProperty<Select, bool> IsPlaceholderTextVisibleProperty =
        AvaloniaProperty.RegisterDirect<Select, bool>(
            nameof(IsPlaceholderTextVisible),
            o => o.IsPlaceholderTextVisible,
            (o, v) => o.IsPlaceholderTextVisible = v);
    
    internal static readonly DirectProperty<Select, SelectOption?> SelectedOptionProperty =
        AvaloniaProperty.RegisterDirect<Select, SelectOption?>(
            nameof(SelectedOption),
            o => o.SelectedOption,
            (o, v) => o.SelectedOption = v);
    
    internal static readonly DirectProperty<Select, bool> IsSelectionEmptyProperty =
        AvaloniaProperty.RegisterDirect<Select, bool>(
            nameof(IsSelectionEmpty),
            o => o.IsSelectionEmpty,
            (o, v) => o.IsSelectionEmpty = v);
    
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
    
    private double _effectivePopupWidth;

    internal double EffectivePopupWidth
    {
        get => _effectivePopupWidth;
        set => SetAndRaise(EffectivePopupWidthProperty, ref _effectivePopupWidth, value);
    }
    
    private bool _isPlaceholderTextVisible;

    internal bool IsPlaceholderTextVisible
    {
        get => _isPlaceholderTextVisible;
        set => SetAndRaise(IsPlaceholderTextVisibleProperty, ref _isPlaceholderTextVisible, value);
    }
    
    private SelectOption? _selectedOption;

    internal SelectOption? SelectedOption
    {
        get => _selectedOption;
        set => SetAndRaise(SelectedOptionProperty, ref _selectedOption, value);
    }
    
    private bool _isSelectionEmpty = true;

    internal bool IsSelectionEmpty
    {
        get => _isSelectionEmpty;
        set => SetAndRaise(IsSelectionEmptyProperty, ref _isSelectionEmpty, value);
    }
    
    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => SelectToken.ID;

    #endregion
    
    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new VirtualizingStackPanel());
    
    private Popup? _popup;
    private SelectOptions? _optionsBox;
    private SelectSearchTextBox? _singleSearchInput;
    private readonly CompositeDisposable _subscriptionsOnOpen = new ();
    private ListFilterDescription? _filterDescription;
    
    static Select()
    {
        FocusableProperty.OverrideDefaultValue<Select>(true);
        SelectHandle.ClearRequestedEvent.AddClassHandler<Select>((target, args) =>
        {
            target.HandleClearRequest();
        });
        OptionsSourceProperty.Changed.AddClassHandler<Select>((x, e) => x.HandleOptionsSourcePropertyChanged(e));
    }
    
    public Select()
    {
        this.RegisterResources();
    }

    private void HandleClearRequest()
    {
        Clear();
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (Mode == SelectMode.Single && SelectedOption == null)
        {
            if (DefaultValues?.Count > 0)
            {
                var defaultValue = DefaultValues.First();
                foreach (var option in Options)
                {
                    if (OptionEqualByValue(defaultValue, option))
                    {
                        SelectedOption  = option;
                        break;
                    }
                }
            }
        }
        else if (Mode == SelectMode.Multiple && (SelectedOptions == null || SelectedOptions.Count == 0))
        {
            if (DefaultValues?.Count > 0)
            {
                var selectedOptions = new List<SelectOption>();
                foreach (var defaultValue in DefaultValues)
                {
                    foreach (var option in Options)
                    {
                        if (OptionEqualByValue(defaultValue, option))
                        {
                            selectedOptions.Add(option);
                        }
                    }
                }

                SelectedOptions = selectedOptions;
            }
        }
    }

    private bool OptionEqualByValue(object value, SelectOption selectOption)
    {
        if (DefaultValueCompareFn != null)
        {
            return DefaultValueCompareFn(value, selectOption);
        }
        var strValue = value.ToString();
        var optValue = selectOption.Value?.ToString();
        return strValue == optValue;
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
            SetCurrentValue(IsDropDownOpenProperty, false);
            e.Handled = true;
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
            if (_popup?.IsInsidePopup(source) == true)
            {
                if (Mode == SelectMode.Single)
                {
                    SetCurrentValue(IsDropDownOpenProperty, false);
                }
                e.Handled = true;
            }
            else if (PseudoClasses.Contains(StdPseudoClass.Pressed))
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
        _optionsBox        = e.NameScope.Get<SelectOptions>(SelectThemeConstants.OptionsBoxPart);
        _singleSearchInput = e.NameScope.Get<SelectSearchTextBox>(SelectThemeConstants.SingleSearchInputPart);
        if (_optionsBox != null)
        {
            _optionsBox.SelectionChanged += HandleOptionsBoxSelectionChanged;
        }
        _popup        =  e.NameScope.Get<Popup>(SelectThemeConstants.PopupPart);
        _popup.Opened += PopupOpened;
        _popup.Closed += PopupClosed;
        ConfigureMaxDropdownHeight();
        ConfigurePlaceholderVisible();
        ConfigureSelectionIsEmpty();
        UpdatePseudoClasses();
        ConfigureSingleSearchTextBox();
    }

    protected void HandleOptionsBoxSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        Debug.Assert(_optionsBox != null);
        if (Mode == SelectMode.Single)
        {
            if (_optionsBox.SelectedItem is SelectOption selectOption)
            {
                var selectedOptions = new List<SelectOption>();
                selectedOptions.Add(selectOption);
                SetCurrentValue(SelectedOptionProperty, selectOption);
                SetCurrentValue(SelectedOptionsProperty, selectedOptions);
                if (_singleSearchInput != null)
                {
                    _singleSearchInput.Clear();
                    _singleSearchInput.Width = double.NaN;
                }
            }
        }
        else
        {
            var selectedOptions = new List<SelectOption>();
            if (SelectedOptions != null)
            {
                foreach (var option in SelectedOptions)
                {
                    if (!selectedOptions.Contains(option))
                    {
                        selectedOptions.Add(option);
                    }
                }
            }

            foreach (var item in e.RemovedItems)
            {
                if (item is SelectOption selectOption)
                {
                    selectedOptions.Remove(selectOption);
                }
            }
            
            foreach (var item in e.AddedItems)
            {
                if (item is SelectOption selectOption && !selectedOptions.Contains(selectOption))
                {
                    selectedOptions.Add(selectOption);
                }
            }
            SetCurrentValue(SelectedOptionsProperty, selectedOptions);
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == SelectedOptionsProperty)
        {
            ConfigurePlaceholderVisible();
        }
        else if (change.Property == IsDropDownOpenProperty)
        {
            PseudoClasses.Set(SelectPseudoClass.DropdownOpen, change.GetNewValue<bool>());
            ConfigureSingleSearchTextBox();
        }
        else if (change.Property == DisplayPageSizeProperty ||
                 change.Property == ItemHeightProperty)
        {
            ConfigureMaxDropdownHeight();
        }
        else if (change.Property == IsPopupMatchSelectWidthProperty)
        {
            ConfigurePopupMinWith(DesiredSize.Width);
        }
        else if (change.Property == StyleVariantProperty ||
                 change.Property == StatusProperty)
        {
            UpdatePseudoClasses();
        }
        if (change.Property == SelectedOptionsProperty ||
            change.Property == SelectedOptionProperty)
        {
            ConfigureSelectionIsEmpty();
            ConfigurePlaceholderVisible();
        }
        else if (change.Property == OptionFilterPropProperty)
        {
            HandleOptionFilterPropChanged();
        }
        base.OnPropertyChanged(change);
    }
    
    private void PopupClosed(object? sender, EventArgs e)
    {
        _subscriptionsOnOpen.Clear();
        DropDownClosed?.Invoke(this, EventArgs.Empty);
        if (Mode == SelectMode.Single)
        {
            if (_singleSearchInput != null)
            {
                _singleSearchInput.Clear();
                _singleSearchInput.Width = double.NaN;
            }
        }
    }

    private void PopupOpened(object? sender, EventArgs e)
    {
        _subscriptionsOnOpen.Clear();
        this.GetObservable(IsVisibleProperty).Subscribe(IsVisibleChanged).DisposeWith(_subscriptionsOnOpen);
        foreach (var parent in this.GetVisualAncestors().OfType<Control>())
        {
            parent.GetObservable(IsVisibleProperty).Subscribe(IsVisibleChanged).DisposeWith(_subscriptionsOnOpen);
        }

        if (_optionsBox != null)
        {
            _optionsBox.SelectedItem  = SelectedOption;
            _optionsBox.SelectedItems = (IList?)SelectedOptions;
        }
        DropDownOpened?.Invoke(this, EventArgs.Empty);
        if (Mode == SelectMode.Single)
        {
            if (_singleSearchInput != null)
            {
                _singleSearchInput.Focus();
            }
        }
    }
    
    private void IsVisibleChanged(bool isVisible)
    {
        if (!isVisible && IsDropDownOpen)
        {
            SetCurrentValue(IsDropDownOpenProperty, false);
        }
    }
    
    public void Clear()
    {
        SelectedOptions = null;
        SelectedOption  = null;
    }
    
    private void ConfigureMaxDropdownHeight()
    {
        SetCurrentValue(MaxPopupHeightProperty, ItemHeight * DisplayPageSize + PopupContentPadding.Top + PopupContentPadding.Bottom);
    }
    
    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        ConfigurePopupMinWith(e.NewSize.Width);
    }

    private void ConfigurePopupMinWith(double selectWidth)
    {
        if (IsPopupMatchSelectWidth)
        {
            SetCurrentValue(EffectivePopupWidthProperty, selectWidth);
        }
        else
        {
            SetCurrentValue(EffectivePopupWidthProperty, double.NaN);
        }
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Error, Status == AddOnDecoratedStatus.Error);
        PseudoClasses.Set(StdPseudoClass.Warning, Status == AddOnDecoratedStatus.Warning);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Outline, StyleVariant == AddOnDecoratedVariant.Outline);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Filled, StyleVariant == AddOnDecoratedVariant.Filled);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Borderless, StyleVariant == AddOnDecoratedVariant.Borderless);
    }

    private void ConfigurePlaceholderVisible()
    {
        SetCurrentValue(IsPlaceholderTextVisibleProperty, (Mode == SelectMode.Single && SelectedOption == null) || 
                                                          (Mode != SelectMode.Single && SelectedOptions == null || SelectedOptions?.Count == 0));
    }

    private void ConfigureSelectionIsEmpty()
    {
        SetCurrentValue(IsSelectionEmptyProperty, SelectedOption == null && (SelectedOptions == null || SelectedOptions?.Count == 0));
    }

    private void ConfigureSingleSearchTextBox()
    {
        if (_singleSearchInput != null)
        {
            if (IsDropDownOpen)
            {
                _singleSearchInput.Width = _singleSearchInput.Bounds.Width;
            }
            _singleSearchInput.TextChanged += HandleSearchInputTextChanged;
        }
    }

    private void HandleSearchInputTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_optionsBox != null && _optionsBox.FilterDescriptions != null)
        {
            var filterValue = _singleSearchInput?.Text;
            if (string.IsNullOrEmpty(filterValue))
            {
                _optionsBox.FilterDescriptions.Clear();
            }
            else
            {
                if (_filterDescription != null)
                {
                    var oldFilter = _filterDescription;
                    Debug.Assert(oldFilter.FilterConditions.Count == 1);
                    var oldFilterValue = oldFilter.FilterConditions.First().ToString();
                    if (oldFilterValue != filterValue)
                    {
                        _filterDescription = new ListFilterDescription()
                        {
                            PropertyPath     = _filterDescription.PropertyPath,
                            Filter           =  _filterDescription.Filter,
                            FilterConditions = [filterValue]
                        };
                        _optionsBox.FilterDescriptions.Remove(oldFilter);
                        _optionsBox.FilterDescriptions.Add(_filterDescription);
                    }
                }
                else
                {
                    _filterDescription = new ListFilterDescription()
                    {
                        PropertyPath     = OptionFilterProp,
                        Filter           = FilterFn,
                        FilterConditions = [filterValue],
                    };
                    _optionsBox.FilterDescriptions.Add(_filterDescription);
                }
            }
        }
    }

    private void HandleOptionFilterPropChanged()
    {
        if (_filterDescription != null && _optionsBox!= null && _optionsBox.FilterDescriptions != null)
        {
            var oldFilter = _filterDescription;
            _filterDescription = new ListFilterDescription()
            {
                PropertyPath     = OptionFilterProp,
                Filter           =  oldFilter.Filter,
                FilterConditions = oldFilter.FilterConditions
            };
            _optionsBox.FilterDescriptions.Remove(oldFilter);
            _optionsBox.FilterDescriptions.Add(_filterDescription);
        }
    }
    
    private void HandleOptionsSourcePropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var newItemsSource = (IEnumerable<SelectOption>?)e.NewValue;
        if (newItemsSource != null)
        {
            Options.Clear();
            Options.AddRange(newItemsSource);
        }
    }
}