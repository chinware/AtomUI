using AtomUI.Controls.Utils;
using AtomUI.Reflection;
using AtomUI.Theme;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

using ItemCollection = AtomUI.Collections.ItemCollection;

public enum SelectMode
{
    Single,
    Multiple,
    Tags
}

[PseudoClasses(SelectPseudoClass.DropdownOpen)]
public partial class Select : AbstractSelect
{
    #region 公共属性定义
    public static readonly StyledProperty<IEnumerable<ISelectOption>?> OptionsSourceProperty =
        AvaloniaProperty.Register<Select, IEnumerable<ISelectOption>?>(nameof(OptionsSource));

    public static readonly StyledProperty<IDataTemplate?> OptionTemplateProperty =
        AvaloniaProperty.Register<Select, IDataTemplate?>(nameof(OptionTemplate));

    public static readonly StyledProperty<bool> IsDefaultActiveFirstOptionProperty =
        AvaloniaProperty.Register<Select, bool>(nameof(IsDefaultActiveFirstOption));

    public static readonly StyledProperty<bool> IsGroupEnabledProperty =
        ListView.IsGroupEnabledProperty.AddOwner<Select>();

    public static readonly StyledProperty<DefaultFilterValueSelector?> GroupPropertySelectorProperty =
        ListView.GroupPropertySelectorProperty.AddOwner<Select>();

    public static readonly StyledProperty<bool> IsHideSelectedOptionsProperty =
        AvaloniaProperty.Register<Select, bool>(nameof(IsHideSelectedOptions));

    public static readonly StyledProperty<SelectMode> ModeProperty =
        AvaloniaProperty.Register<Select, SelectMode>(nameof(Mode));

    public static readonly DirectProperty<Select, IList<ISelectOption>?> SelectedOptionsProperty =
        AvaloniaProperty.RegisterDirect<Select, IList<ISelectOption>?>(
            nameof(SelectedOptions),
            o => o.SelectedOptions,
            (o, v) => o.SelectedOptions = v);

    public static readonly DirectProperty<Select, ISelectOption?> SelectedOptionProperty =
        AvaloniaProperty.RegisterDirect<Select, ISelectOption?>(
            nameof(SelectedOption),
            o => o.SelectedOption,
            (o, v) => o.SelectedOption = v);

    public static readonly StyledProperty<double> OptionFontSizeProperty =
        AvaloniaProperty.Register<Select, double>(nameof(OptionFontSize));

    public static readonly StyledProperty<bool> AutoScrollToSelectedOptionsProperty =
        AvaloniaProperty.Register<Select, bool>(
            nameof(AutoScrollToSelectedOptions),
            defaultValue: false);

    public static readonly DirectProperty<Select, object?> OptionsAsyncLoadContextProperty =
        AvaloniaProperty.RegisterDirect<Select, object?>(
            nameof(OptionsAsyncLoadContext),
            o => o.OptionsAsyncLoadContext,
            (o, v) => o.OptionsAsyncLoadContext = v);

    public static readonly StyledProperty<ISelectOptionsAsyncLoader?> OptionsLoaderProperty =
        AvaloniaProperty.Register<Select, ISelectOptionsAsyncLoader?>(nameof(OptionsLoader));

    public static readonly StyledProperty<TimeSpan> AsyncLoadTimeoutProperty =
        AvaloniaProperty.Register<Select, TimeSpan>(
            nameof(AsyncLoadTimeout),
            TimeSpan.FromSeconds(10));

    public static readonly StyledProperty<IValueFilter?> FilterProperty =
        AvaloniaProperty.Register<AbstractSelect, IValueFilter?>(nameof(Filter));

    public static readonly StyledProperty<DefaultFilterValueSelector?> FilterValueSelectorProperty =
        AvaloniaProperty.Register<AbstractSelect, DefaultFilterValueSelector?>(
            nameof(FilterValueSelector));

    public IEnumerable<ISelectOption>? OptionsSource
    {
        get => GetValue(OptionsSourceProperty);
        set => SetValue(OptionsSourceProperty, value);
    }

    [InheritDataTypeFromItems(nameof(OptionsSource))]
    public IDataTemplate? OptionTemplate
    {
        get => GetValue(OptionTemplateProperty);
        set => SetValue(OptionTemplateProperty, value);
    }

    public bool IsDefaultActiveFirstOption
    {
        get => GetValue(IsDefaultActiveFirstOptionProperty);
        set => SetValue(IsDefaultActiveFirstOptionProperty, value);
    }

    public bool IsGroupEnabled
    {
        get => GetValue(IsGroupEnabledProperty);
        set => SetValue(IsGroupEnabledProperty, value);
    }

    public DefaultFilterValueSelector? GroupPropertySelector
    {
        get => GetValue(GroupPropertySelectorProperty);
        set => SetValue(GroupPropertySelectorProperty, value);
    }

    public bool IsHideSelectedOptions
    {
        get => GetValue(IsHideSelectedOptionsProperty);
        set => SetValue(IsHideSelectedOptionsProperty, value);
    }

    public SelectMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    [Content]
    public ItemCollection Options { get; } = new();

    private IList<ISelectOption>? _selectedOptions;

    public IList<ISelectOption>? SelectedOptions
    {
        get => _selectedOptions;
        set => SetAndRaise(SelectedOptionsProperty, ref _selectedOptions, value);
    }

    private ISelectOption? _selectedOption;

    public ISelectOption? SelectedOption
    {
        get => _selectedOption;
        set => SetAndRaise(SelectedOptionProperty, ref _selectedOption, value);
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

    private object? _optionsAsyncLoadContext;

    public object? OptionsAsyncLoadContext
    {
        get => _optionsAsyncLoadContext;
        set => SetAndRaise(OptionsAsyncLoadContextProperty, ref _optionsAsyncLoadContext, value);
    }

    public IValueFilter? Filter
    {
        get => GetValue(FilterProperty);
        set => SetValue(FilterProperty, value);
    }

    public DefaultFilterValueSelector? FilterValueSelector
    {
        get => GetValue(FilterValueSelectorProperty);
        set => SetValue(FilterValueSelectorProperty, value);
    }

    public ISelectOptionsAsyncLoader? OptionsLoader
    {
        get => GetValue(OptionsLoaderProperty);
        set => SetValue(OptionsLoaderProperty, value);
    }

    public TimeSpan AsyncLoadTimeout
    {
        get => GetValue(AsyncLoadTimeoutProperty);
        set => SetValue(AsyncLoadTimeoutProperty, value);
    }
    #endregion

    #region 公共属性定义

    public event EventHandler<SelectOptionsLoadingEventArgs>? OptionsLoading;
    public event EventHandler<SelectOptionsLoadedEventArgs>? OptionsLoaded;
    public event EventHandler<SelectSelectionChangedEventArgs>? SelectionChanged;

    #endregion

    public static readonly DefaultFilterValueSelector HeaderFilterPropertySelector = data =>
    {
        if (data is ISelectOption option)
        {
            return option.Header;
        }
        return null;
    };

    public static readonly DefaultFilterValueSelector ValueFilterPropertySelector = data =>
    {
        if (data is ISelectOption option)
        {
            return option.Content;
        }
        return null;
    };

    public Func<object, ISelectOption, bool>? DefaultValueCompareFn { get; set; }

    public IList<object>? DefaultValues { get; set; }

    #region 内部属性定义

    internal static readonly DirectProperty<Select, bool> IsEffectiveFilterEnabledProperty =
        AvaloniaProperty.RegisterDirect<Select, bool>(nameof(IsEffectiveFilterEnabled),
            o => o.IsEffectiveFilterEnabled,
            (o, v) => o.IsEffectiveFilterEnabled = v);

    private bool _isEffectiveFilterEnabled;

    internal bool IsEffectiveFilterEnabled
    {
        get => _isEffectiveFilterEnabled;
        set => SetAndRaise(IsEffectiveFilterEnabledProperty, ref _isEffectiveFilterEnabled, value);
    }

    #endregion

    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new VirtualizingStackPanel());

    private Panel? _contentPanel;
    private SelectCandidateList? _candidateList;
    private SelectFilterTextBox? _singleFilterInput;
    private SelectResultOptionsBox? _selectedOptionsBox;
    private bool _ignoreSyncSelection;
    private bool _candidateListActivated;
    private ISelectOption? _addNewOption;

    static Select()
    {
        FocusableProperty.OverrideDefaultValue<Select>(true);
        SelectHandle.ClearRequestedEvent.AddClassHandler<Select>((select, args) => select.ClearValue());
        OptionsSourceProperty.Changed.AddClassHandler<Select>((select, e) => select.HandleOptionsSourcePropertyChanged(e));
        SelectFilterTextBox.TextChangedEvent.AddClassHandler<Select>((select, e) => select.HandleSearchInputTextChanged(e));
        SelectTag.ClosedEvent.AddClassHandler<Select>((select, args) => select.HandleTagCloseRequest(args));
        SelectedOptionsProperty.Changed.AddClassHandler<Select>((select, args) => select.HandleSelectedOptionsChanged(args));
        SelectedOptionProperty.Changed.AddClassHandler<Select>((select, args) => select.HandleSelectedOptionChanged(args));
        SelectResultOptionsBox.KeyDownEvent.AddClassHandler<Select>(
            (x, e) => x.HandleFilterInputKeyDown(e),
            RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
    }

    public Select()
    {
        this.RegisterTokenResourceScope(SelectToken.ScopeProvider);
        SetCurrentValue(FilterProperty, ValueFilterFactory.BuildFilter(ValueFilterMode.Contains));
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (Filter == null)
        {
            SetCurrentValue(FilterProperty, ValueFilterFactory.BuildFilter(ValueFilterMode.Contains));
        }
        if (FilterValueSelector == null)
        {
            SetCurrentValue(FilterValueSelectorProperty, HeaderFilterPropertySelector);
        }
    }

    private void HandleSelectedOptionsChanged(AvaloniaPropertyChangedEventArgs args)
    {
        NotifyFormValueChanged(args.NewValue);
        SelectionChanged?.Invoke(this, new SelectSelectionChangedEventArgs(Mode, args.OldValue, args.NewValue));
        if (_ignoreSyncSelection)
        {
            _ignoreSyncSelection = false;
            return;
        }
        if (_candidateList != null)
        {
            _candidateList.SelectedItems = SelectedOptions?.ToList();
        }
    }

    private void HandleSelectedOptionChanged(AvaloniaPropertyChangedEventArgs args)
    {
        NotifyFormValueChanged(args.NewValue);
        SelectionChanged?.Invoke(this, new SelectSelectionChangedEventArgs(Mode, args.OldValue, args.NewValue));
        if (_ignoreSyncSelection)
        {
            _ignoreSyncSelection = false;
            return;
        }

        if (_candidateList != null)
        {
            _candidateList.SelectedItem = SelectedOption;
        }
    }

    private bool OptionEqualByValue(object value, ISelectOption selectOption)
    {
        if (DefaultValueCompareFn != null)
        {
            return DefaultValueCompareFn(value, selectOption);
        }
        var strValue = value.ToString();
        var optValue = selectOption.Content?.ToString();
        return strValue == optValue;
    }

    private Dictionary<string, ISelectOption>? BuildDefaultValueLookup()
    {
        if (DefaultValueCompareFn != null)
        {
            return null;
        }

        var lookup = new Dictionary<string, ISelectOption>(StringComparer.Ordinal);
        foreach (var item in Options)
        {
            if (item is ISelectOption option &&
                option.Content?.ToString() is { } optionValue &&
                !lookup.ContainsKey(optionValue))
            {
                lookup.Add(optionValue, option);
            }
        }
        return lookup;
    }

    private bool TryFindDefaultOption(object defaultValue,
                                      Dictionary<string, ISelectOption>? lookup,
                                      out ISelectOption option)
    {
        if (lookup != null)
        {
            var key = defaultValue.ToString();
            if (key != null && lookup.TryGetValue(key, out option!))
            {
                return true;
            }

            option = default!;
            return false;
        }

        foreach (var item in Options)
        {
            if (item is ISelectOption candidate &&
                OptionEqualByValue(defaultValue, candidate))
            {
                option = candidate;
                return true;
            }
        }

        option = default!;
        return false;
    }

    private bool TryHandleDeleteKey(KeyEventArgs e)
    {
        if (Mode == SelectMode.Single || SelectedOptions == null || SelectedOptions.Count == 0)
        {
            return false;
        }

        if (e.Key != Key.Back && e.Key != Key.Delete)
        {
            return false;
        }

        if (e.Source is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text) == false)
        {
            return false;
        }

        var newSelection = new List<ISelectOption>();
        foreach (var selectedItem in SelectedOptions)
        {
            newSelection.Add(selectedItem);
        }

        if (newSelection.Count == 0)
        {
            return false;
        }

        var lastIndex   = newSelection.Count - 1;
        var removedItem = newSelection[lastIndex];
        newSelection.RemoveAt(lastIndex);
        SelectedOptions = newSelection;

        if (Mode == SelectMode.Tags && removedItem.IsDynamicAdded)
        {
            Options.Remove(removedItem);
            if (ReferenceEquals(_addNewOption, removedItem))
            {
                _addNewOption = null;
            }
        }

        e.Handled = true;
        return true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Handled)
        {
            return;
        }

        if (IsDropDownOpen)
        {
            if (_candidateList != null)
            {
                _candidateList.HandleKeyDown(e);
                if (e.Handled)
                {
                    return;
                }
            }
        }

        if ((e.Key == Key.F4 && e.KeyModifiers.HasAllFlags(KeyModifiers.Alt) == false) ||
            ((e.Key == Key.Down || e.Key == Key.Up) && e.KeyModifiers.HasAllFlags(KeyModifiers.Alt)))
        {
            SetCurrentValue(IsDropDownOpenProperty, !IsDropDownOpen);
            e.Handled = true;
        }
        else if (!IsDropDownOpen && (e.Key == Key.Down || e.Key == Key.Up))
        {
            SetCurrentValue(IsDropDownOpenProperty, true);
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
    }

    private void HandleFilterInputKeyDown(KeyEventArgs e)
    {
        if (TryHandleDeleteKey(e))
        {
            return;
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if(!e.Handled && e.Source is Visual source)
        {
            if (Popup?.IsInsidePopup(source) == true)
            {
                e.Handled = true;
                return;
            }
        }

        if (IsDropDownOpen)
        {
            // When a drop-down is open with OverlayDismissEventPassThrough enabled and the control
            // is pressed, close the drop-down
            if (e.Source is Control sourceControl)
            {
                var parent = sourceControl.FindAncestorOfType<IconButton>();
                var tag    = parent?.FindAncestorOfType<SelectTag>();
                if (tag != null)
                {
                    e.Handled = true;
                }
            }
            else if (!IsHideSelectedOptions)
            {
                SetCurrentValue(IsDropDownOpenProperty, false);
                e.Handled = true;
            }
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
            if (Popup?.IsInsidePopup(source) == true)
            {
                e.Handled = true;
            }
            else if (PseudoClasses.Contains(StdPseudoClass.Pressed))
            {
                var clickInTagCloseButton = false;
                if (e.Source is Control sourceControl)
                {
                    var parent = sourceControl.FindAncestorOfType<IconButton>();
                    var tag    = parent?.FindAncestorOfType<SelectTag>();
                    if (tag != null)
                    {
                        clickInTagCloseButton = true;
                    }
                }

                if (!clickInTagCloseButton)
                {
                    if (!IsDropDownOpen)
                    {
                        HandleOpenDropRequest();
                    }
                    else
                    {
                        SetCurrentValue(IsDropDownOpenProperty, false);
                    }
                }

                e.Handled = true;
            }
        }

        PseudoClasses.Set(StdPseudoClass.Pressed, false);
        base.OnPointerReleased(e);
    }

    private void HandleOpenDropRequest()
    {
        // 暂时设计只加载一次，如果加载出错不改变状态
        if (OptionsLoader != null && !_asyncOptionsLoaded)
        {
            LoadOptionsAsync();
        }
        else
        {
            SetCurrentValue(IsDropDownOpenProperty, true);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        ClearSingleFilterInput();
        ClearSelectedOptionsBox();

        base.OnApplyTemplate(e);

        _contentPanel     = e.NameScope.Find<Panel>("PART_ContentPanel");
        _singleFilterInput = e.NameScope.Find<SelectFilterTextBox>("PART_SingleFilterInput");
        _selectedOptionsBox = e.NameScope.Find<SelectResultOptionsBox>("SelectedOptionsBox");

        ConfigurePlaceholderVisible();
        ConfigureSelectionIsEmpty();
        UpdatePseudoClasses();
        ConfigureSingleFilterTextBox();
        ConfigureEffectiveSearchEnabled();
        ConfigureModeSpecificContent();
    }

    private void HandleCandidateListComplete(object? sender, RoutedEventArgs e)
    {
        if (_candidateList != null)
        {
            _ignoreSyncSelection = true;
            if (Mode == SelectMode.Single)
            {
                SelectedOption = (ISelectOption?)_candidateList.SelectedItem;
            }
            else
            {
                SelectedOptions = _candidateList.SelectedItems?.Cast<ISelectOption>().ToList();
            }
        }
        if (IsDropDownOpen)
        {
            SetCurrentValue(IsDropDownOpenProperty, false);
        }
    }

    private void HandleCandidateListCanceled(object? sender, RoutedEventArgs e)
    {
        SetCurrentValue(IsDropDownOpenProperty, false);
    }

    private void HandleCandidateListSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!_candidateListActivated)
        {
            return;
        }
        if (Mode != SelectMode.Single)
        {
            var currentSelectedSet = SelectedOptions?.ToHashSet() ?? new HashSet<ISelectOption>();
            var newSelectedSet     = new HashSet<ISelectOption>();
            foreach (var item in currentSelectedSet)
            {
                newSelectedSet.Add(item);
            }

            foreach (var item in e.AddedItems)
            {
                if (item is ISelectOption selectOption)
                {
                    newSelectedSet.Add(selectOption);
                }
            }
            foreach (var item in e.RemovedItems)
            {
                if (item is ISelectOption selectOption)
                {
                    newSelectedSet.Remove(selectOption);
                }
            }

            if (!newSelectedSet.SetEquals(currentSelectedSet))
            {
                SelectedOptions = newSelectedSet.ToList();
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsDropDownOpenProperty)
        {
            ConfigureSingleFilterTextBox();
            SyncSelectedOptionsBoxProperties();
            SyncCandidateListProperties();
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
            SetCurrentValue(SelectedCountProperty, SelectedOptions?.Count ?? 0);
            CleanDynamicAddedOptions();
        }
        else if (change.Property == FilterValueSelectorProperty)
        {
            HandleFilterValueSelectorChanged();
        }
        else if (change.Property == ModeProperty)
        {
            ConfigureOptionsBoxSelectionMode();
            ConfigureModeSpecificContent();
            SyncCandidateListProperties();
        }

        if (change.Property == IsFilterEnabledProperty ||
            change.Property == ModeProperty)
        {
            ConfigureEffectiveSearchEnabled();
            ConfigureModeSpecificContent();
            SyncCandidateListProperties();
            UpdateOwnerDrivenAccessoryState();
        }

        if (change.Property == SizeTypeProperty ||
            change.Property == SelectedOptionsProperty ||
            change.Property == MaxTagCountProperty ||
            change.Property == IsResponsiveTagModeProperty)
        {
            SyncSelectedOptionsBoxProperties();
        }

        if (change.Property == SelectedOptionProperty ||
            change.Property == FilterValueProperty)
        {
            SyncSingleFilterInputProperties();
        }

        if (change.Property == FilterProperty ||
            change.Property == FilterValueProperty ||
            change.Property == FilterValueSelectorProperty ||
            change.Property == IsGroupEnabledProperty ||
            change.Property == GroupPropertySelectorProperty ||
            change.Property == IsMotionEnabledProperty ||
            change.Property == IsHideSelectedOptionsProperty ||
            change.Property == MaxCountProperty ||
            change.Property == AutoScrollToSelectedOptionsProperty ||
            change.Property == OptionTemplateProperty)
        {
            SyncCandidateListProperties();
        }
    }

    protected override void PopupClosed(object? sender, EventArgs e)
    {
        if (Mode == SelectMode.Single)
        {
            if (_singleFilterInput != null)
            {
                _singleFilterInput.Clear();
                _singleFilterInput.Width = double.NaN;
            }
        }

        _candidateListActivated = false;
        base.PopupClosed(sender, e);
    }

    protected override void PopupOpened(object? sender, EventArgs e)
    {
        _candidateListActivated = true;
        if (_candidateList != null)
        {
            if (Mode == SelectMode.Single)
            {
                _candidateList.SelectedItem = SelectedOption;
            }
            else
            {
                if (SelectedOptions != null && SelectedOptions.Count > 0)
                {
                    _candidateList.SelectedItems = SelectedOptions?.ToList();
                }
            }
        }

        if (Mode == SelectMode.Single)
        {
            _singleFilterInput?.Focus();
        }
        base.PopupOpened(sender, e);
    }

    private void SyncSelectionToCandidateList()
    {
        if (_candidateList != null)
        {
            if (Mode == SelectMode.Single)
            {
                _candidateList.SelectedItem = SelectedOption;
            }
            else
            {
                _candidateList.SelectedItems = SelectedOptions?.ToList();
            }
        }
    }

    private void ConfigureOptionsBoxSelectionMode()
    {
        if (_candidateList == null)
        {
            return;
        }

        _candidateList.SetCurrentValue(ListView.SelectionModeProperty,
            Mode == SelectMode.Single ? SelectionMode.Single : SelectionMode.Multiple);
    }

    private void ConfigureModeSpecificContent()
    {
        if (_contentPanel == null)
        {
            return;
        }

        if (Mode == SelectMode.Single)
        {
            ClearSelectedOptionsBox();
            if (IsEffectiveFilterEnabled)
            {
                EnsureSingleFilterInput();
            }
            else
            {
                ClearSingleFilterInput();
            }
        }
        else
        {
            ClearSingleFilterInput();
            EnsureSelectedOptionsBox();
        }
    }

    private void EnsureSingleFilterInput()
    {
        if (_contentPanel == null)
        {
            return;
        }

        if (_singleFilterInput == null)
        {
            _singleFilterInput = new SelectFilterTextBox
            {
                Name      = "PART_SingleFilterInput",
                IsVisible = true
            };
            _singleFilterInput.SetTemplatedParent(this);
            _contentPanel.Children.Add(_singleFilterInput);
        }

        SyncSingleFilterInputProperties();
    }

    private void ClearSingleFilterInput()
    {
        if (_singleFilterInput == null)
        {
            return;
        }

        _singleFilterInput.Clear();
        _contentPanel?.Children.Remove(_singleFilterInput);
        _singleFilterInput.SetTemplatedParent(null);
        _singleFilterInput = null;
    }

    private void SyncSingleFilterInputProperties()
    {
        if (_singleFilterInput == null)
        {
            return;
        }

        _singleFilterInput.SetCurrentValue(TextBox.SizeTypeProperty, SizeType);
        _singleFilterInput.SetCurrentValue(TextBox.PlaceholderTextProperty,
            SelectedOption?.Header?.ToString());
        _singleFilterInput.SetCurrentValue(Visual.IsVisibleProperty, true);
    }

    private void EnsureSelectedOptionsBox()
    {
        if (_contentPanel == null)
        {
            return;
        }

        if (_selectedOptionsBox == null)
        {
            _selectedOptionsBox = new SelectResultOptionsBox
            {
                Name                = "SelectedOptionsBox",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                VerticalAlignment   = Avalonia.Layout.VerticalAlignment.Center,
                Focusable           = true,
                IsVisible           = true
            };
            _selectedOptionsBox.SetTemplatedParent(this);
            _contentPanel.Children.Add(_selectedOptionsBox);
        }

        SyncSelectedOptionsBoxProperties();
    }

    private void ClearSelectedOptionsBox()
    {
        if (_selectedOptionsBox == null)
        {
            return;
        }

        _selectedOptionsBox.SetCurrentValue(SelectResultOptionsBox.SelectedOptionsProperty, null);
        _contentPanel?.Children.Remove(_selectedOptionsBox);
        _selectedOptionsBox.SetTemplatedParent(null);
        _selectedOptionsBox = null;
    }

    private void SyncSelectedOptionsBoxProperties()
    {
        if (_selectedOptionsBox == null)
        {
            return;
        }

        _selectedOptionsBox.SetCurrentValue(SelectResultOptionsBox.ModeProperty, Mode);
        _selectedOptionsBox.SetCurrentValue(SelectResultOptionsBox.SizeTypeProperty, SizeType);
        _selectedOptionsBox.SetCurrentValue(SelectResultOptionsBox.SelectedOptionsProperty, SelectedOptions);
        _selectedOptionsBox.SetCurrentValue(SelectResultOptionsBox.IsFilterEnabledProperty, IsEffectiveFilterEnabled);
        _selectedOptionsBox.SetCurrentValue(SelectResultOptionsBox.IsDropDownOpenProperty, IsDropDownOpen);
        _selectedOptionsBox.SetCurrentValue(SelectResultOptionsBox.MaxTagCountProperty, MaxTagCount);
        _selectedOptionsBox.SetCurrentValue(SelectResultOptionsBox.IsResponsiveTagModeProperty, IsResponsiveTagMode);
        _selectedOptionsBox.SetCurrentValue(Visual.IsVisibleProperty, true);
    }

    public void ClearValue()
    {
        SelectedOptions = null;
        SelectedOption  = null;
    }

    private void ConfigurePlaceholderVisible()
    {
        if (Mode == SelectMode.Single)
        {
            SetCurrentValue(IsPlaceholderTextVisibleProperty, SelectedOption == null && string.IsNullOrEmpty(FilterValue?.ToString()));
        }
        else
        {
            SetCurrentValue(IsPlaceholderTextVisibleProperty, (SelectedOptions == null || SelectedOptions?.Count == 0) && string.IsNullOrEmpty(FilterValue?.ToString()));
        }
    }

    private void ConfigureSelectionIsEmpty()
    {
        if (Mode == SelectMode.Single)
        {
            SetCurrentValue(IsSelectionEmptyProperty, SelectedOption == null);
        }
        else
        {
            SetCurrentValue(IsSelectionEmptyProperty, SelectedOptions == null || SelectedOptions?.Count == 0);
        }
    }

    private void ConfigureSingleFilterTextBox()
    {
        if (_singleFilterInput != null)
        {
            if (IsDropDownOpen)
            {
                _singleFilterInput.Width = _singleFilterInput.Bounds.Width;
            }
        }
    }

    private void HandleSearchInputTextChanged(TextChangedEventArgs e)
    {
        if (_candidateList != null)
        {
            if (e.Source is TextBox textBox)
            {
                FilterValue = textBox.Text?.Trim();
            }

            var filterValue = FilterValue?.ToString();
            if (_addNewOption != null)
            {
                var isSelected     = SelectedOptions?.Contains(_addNewOption) == true;
                var isCurrentInput = filterValue == _addNewOption.Header?.ToString();
                if (!isSelected && !isCurrentInput)
                {
                    Options.Remove(_addNewOption);
                    _addNewOption = null;
                }
            }
            ConfigurePlaceholderVisible();

            // Only allow "create from search text" in Tags mode.
            // For Single/Multiple, when data is empty (or filter results are empty),
            // the dropdown should show the empty indicator instead of adding a temporary option.
            if (Mode == SelectMode.Tags &&
                _candidateList.TotalItemCount == 0 &&
                !string.IsNullOrWhiteSpace(filterValue))
            {
                _addNewOption = new SelectOption()
                {
                    Header         = filterValue,
                    Content        = filterValue,
                    IsDynamicAdded = true
                };
                Options.Add(_addNewOption);
            }
            Dispatcher.Post(SyncSelectionToCandidateList);
        }
        e.Handled = true;
    }

    private void HandleTagCloseRequest(RoutedEventArgs e)
    {
        if (Mode == SelectMode.Single)
        {
            return;
        }
        if (e.Source is SelectTag tag && tag.Item is  ISelectOption tagOption)
        {
            if (SelectedOptions != null)
            {
                var selectedOptions = new List<ISelectOption>();
                foreach (var selectedItem in SelectedOptions)
                {
                    selectedOptions.Add(selectedItem);
                }
                selectedOptions.Remove(tagOption);
                SelectedOptions = selectedOptions;
            }

            if (Mode == SelectMode.Tags)
            {
                if (tag.Item is ISelectOption selectOption && selectOption.IsDynamicAdded)
                {
                    Options.Remove(selectOption);
                }
            }
        }
        e.Handled = true;
    }

    private void HandleFilterValueSelectorChanged()
    {
        // if (_filterDescription != null && _candidateList!= null && _candidateList.FilterDescriptions != null)
        // {
        //     var oldFilter = _filterDescription;
        //     _filterDescription = new ListFilterDescription()
        //     {
        //         FilterPropertySelector = FilterValueSelector,
        //         Filter                 =  oldFilter.Filter,
        //         FilterConditions       = oldFilter.FilterConditions
        //     };
        //     _candidateList.FilterDescriptions.Remove(oldFilter);
        //     _candidateList.FilterDescriptions.Add(_filterDescription);
        // }
    }

    private void HandleOptionsSourcePropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        ClearValue();
        if (!Options.IsReadOnly)
        {
            Options.Clear();
        }
        Options.SetItemsSource(change.GetNewValue<IEnumerable<ISelectOption>?>());
    }

    private void ConfigureDefaultValues()
    {
        if (Mode == SelectMode.Single)
        {
            if (SelectedOption == null)
            {
                if (DefaultValues?.Count > 0)
                {
                    var defaultValue = DefaultValues.First();
                    var lookup       = BuildDefaultValueLookup();
                    if (TryFindDefaultOption(defaultValue, lookup, out var option))
                    {
                        SelectedOption = option;
                    }
                }
            }
        }
        else
        {
            if (SelectedOptions == null || SelectedOptions.Count == 0)
            {
                if (DefaultValues?.Count > 0)
                {
                    var selectedOptions = new List<ISelectOption>();
                    var lookup          = BuildDefaultValueLookup();
                    foreach (var defaultValue in DefaultValues)
                    {
                        if (TryFindDefaultOption(defaultValue, lookup, out var option))
                        {
                            selectedOptions.Add(option);
                        }
                    }

                    SelectedOptions = selectedOptions;
                }
            }
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ConfigureDefaultValues();
    }

    private void ConfigureEffectiveSearchEnabled()
    {
        if (Mode == SelectMode.Tags)
        {
            SetCurrentValue(IsEffectiveFilterEnabledProperty, true);
        }
        else
        {
            SetCurrentValue(IsEffectiveFilterEnabledProperty, IsFilterEnabled);
        }
    }

    private protected override void EnsurePopupContent()
    {
        if (_candidateList != null)
        {
            return;
        }

        _candidateList = new SelectCandidateList
        {
            Name                = "PART_CandidateList",
            BorderThickness     = new Thickness(0),
            IsShowEmptyIndicator = true
        };
        _candidateList.SetTemplatedParent(this);
        _candidateList.SelectionChanged += HandleCandidateListSelectionChanged;
        _candidateList.Commit           += HandleCandidateListComplete;
        _candidateList.Cancel           += HandleCandidateListCanceled;

        SyncCandidateListProperties();
        ConfigureOptionsBoxSelectionMode();
        SyncSelectionToCandidateList();
        EnsurePopupFrame(_candidateList);
    }

    private protected override void ClearPopupContent()
    {
        if (_candidateList == null)
        {
            return;
        }

        _candidateList.SelectionChanged -= HandleCandidateListSelectionChanged;
        _candidateList.Commit           -= HandleCandidateListComplete;
        _candidateList.Cancel           -= HandleCandidateListCanceled;
        _candidateList.SetCurrentValue(ItemsControl.ItemsSourceProperty, null);
        _candidateList.SelectedItems = null;
        _candidateList.SelectedItem  = null;
        _candidateList.SetTemplatedParent(null);
        _candidateList = null;
        _candidateListActivated = false;
    }

    private void SyncCandidateListProperties()
    {
        if (_candidateList == null)
        {
            return;
        }

        _candidateList.SetCurrentValue(SelectCandidateList.FilterProperty, Filter);
        _candidateList.SetCurrentValue(SelectCandidateList.FilterValueProperty, FilterValue);
        _candidateList.SetCurrentValue(SelectCandidateList.FilterValueSelectorProperty, FilterValueSelector);
        _candidateList.SetCurrentValue(ListView.IsGroupEnabledProperty, IsGroupEnabled);
        _candidateList.SetCurrentValue(ListView.GroupPropertySelectorProperty, GroupPropertySelector);
        _candidateList.SetCurrentValue(SelectCandidateList.IsMotionEnabledProperty, IsMotionEnabled);
        _candidateList.SetCurrentValue(SelectCandidateList.IsHideSelectedOptionsProperty, IsHideSelectedOptions);
        _candidateList.SetCurrentValue(SelectCandidateList.MaxCountProperty, MaxCount);
        _candidateList.SetCurrentValue(ListView.AutoScrollToSelectedItemProperty, AutoScrollToSelectedOptions);
        _candidateList.SetCurrentValue(ItemsControl.ItemTemplateProperty, OptionTemplate);
        _candidateList.SetCurrentValue(ItemsControl.ItemsSourceProperty, Options);
        _candidateList.SetCurrentValue(ListView.IsShowSelectedIndicatorProperty, Mode != SelectMode.Single);
        ConfigureOptionsBoxSelectionMode();
    }

    private void CleanDynamicAddedOptions()
    {
        if (Mode != SelectMode.Tags)
        {
            return;
        }

        var selected = new HashSet<ISelectOption>();
        if (SelectedOptions != null)
        {
            foreach (var opt in SelectedOptions)
            {
                selected.Add(opt);
            }
        }

        var toRemove = new List<ISelectOption>();
        foreach (var item in Options)
        {
            if (item is ISelectOption option)
            {
                if (option.IsDynamicAdded && !selected.Contains(option))
                {
                    toRemove.Add(option);
                }
            }
        }

        foreach (var option in toRemove)
        {
            Options.Remove(option);
            if (ReferenceEquals(_addNewOption, option))
            {
                _addNewOption = null;
            }
        }
    }

    #region 实现 FormItem 接口

    protected override void NotifySetFormValue(object? value)
    {
        if (Mode == SelectMode.Single)
        {
            SelectedOption = value as ISelectOption;
        }
        else
        {
            SelectedOptions = value as IList<ISelectOption>;
        }
    }

    protected override object? NotifyGetFormValue()
    {
        if (Mode == SelectMode.Single)
        {
            return SelectedOption;
        }
        return SelectedOptions;
    }

    protected override void NotifyClearFormValue()
    {
        if (Mode == SelectMode.Single)
        {
            SelectedOption = null;
        }
        else
        {
            SelectedOptions = null;
        }
    }
    #endregion
}
