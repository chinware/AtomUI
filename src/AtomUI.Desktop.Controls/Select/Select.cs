using System.Collections;
using System.Reactive.Disposables;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Reflection;
using AtomUI.Theme;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
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

    private SelectCandidateList? _candidateList;
    private Border? _popupFrame;
    private SelectFilterTextBox? _singleFilterInput;
    private CompositeDisposable? _contentRightAddOnBindings;
    private bool _ignoreSyncSelection;
    private bool _candidateListActivated;
    private bool _syncingSingleFilterInputText;
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
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (FilterValueSelector == null)
        {
            SetCurrentValue(FilterValueSelectorProperty, HeaderFilterPropertySelector);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ClearPopupContent();
        base.OnDetachedFromVisualTree(e);
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
            _candidateList.SelectedItems = CopySelectedOptions(SelectedOptions);
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

        var lastIndex   = SelectedOptions.Count - 1;
        var removedItem = SelectedOptions[lastIndex];
        var newSelection = new List<ISelectOption>(lastIndex);
        for (var i = 0; i < lastIndex; i++)
        {
            newSelection.Add(SelectedOptions[i]);
        }

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
                var filterTextBox = sourceControl.FindAncestorOfType<SelectFilterTextBox>(includeSelf: true);
                if (filterTextBox != null)
                {
                    return;
                }

                if (SelectTag.IsCloseButtonSource(sourceControl))
                {
                    e.Handled = true;
                    return;
                }
            }

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
            if (Popup?.IsInsidePopup(source) == true)
            {
                e.Handled = true;
            }
            else if (PseudoClasses.Contains(StdPseudoClass.Pressed))
            {
                var clickInTagCloseButton = false;
                if (e.Source is Control sourceControl)
                {
                    clickInTagCloseButton = SelectTag.IsCloseButtonSource(sourceControl);
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
        ClearPopupContent();
        base.OnApplyTemplate(e);

        _singleFilterInput = e.NameScope.Get<SelectFilterTextBox>("PART_SingleFilterInput");
        if (Popup != null)
        {
            Popup.OverlayInputPassThroughElement =
                e.NameScope.Find<AddOnDecoratedBox>(AddOnDecoratedBox.AddOnDecoratedBoxPart);
        }

        ConfigurePlaceholderVisible();
        ConfigureSelectionIsEmpty();
        ConfigureSingleResultVisible();
        UpdatePseudoClasses();
        ConfigureSingleFilterTextBox();
        ConfigureEffectiveSearchEnabled();
        SetupContentRightAddOnBindings(e);
        if (IsDropDownOpen)
        {
            EnsurePopupContent();
        }
    }

    protected override void EnsurePopupContent()
    {
        if (Popup == null)
        {
            return;
        }

        if (_popupFrame == null)
        {
            _popupFrame = new Border
            {
                Name = "PopupFrame"
            };
            _popupFrame.SetTemplatedParent(this);
            _popupFrame[!Layoutable.MaxHeightProperty] = this[!MaxPopupHeightProperty];
            _popupFrame[!Layoutable.MinWidthProperty]  = this[!EffectivePopupWidthProperty];
            _popupFrame[!Border.PaddingProperty]       = this[!PopupContentPaddingProperty];
        }

        if (_candidateList == null)
        {
            _candidateList = new SelectCandidateList
            {
                Name                 = "PART_CandidateList",
                BorderThickness      = new Thickness(0),
                IsShowEmptyIndicator = true,
                ItemsSource          = Options
            };
            _candidateList.SetTemplatedParent(this);
            _candidateList[!ListView.FilterProperty]                    = this[!FilterProperty];
            _candidateList[!ListView.FilterValueProperty]               = this[!FilterValueProperty];
            _candidateList[!ListView.FilterValueSelectorProperty]       = this[!FilterValueSelectorProperty];
            _candidateList[!ListView.IsGroupEnabledProperty]            = this[!IsGroupEnabledProperty];
            _candidateList[!ListView.GroupPropertySelectorProperty]     = this[!GroupPropertySelectorProperty];
            _candidateList[!ListView.IsMotionEnabledProperty]           = this[!IsMotionEnabledProperty];
            _candidateList[!SelectCandidateList.IsHideSelectedOptionsProperty] = this[!IsHideSelectedOptionsProperty];
            _candidateList[!SelectCandidateList.MaxCountProperty]       = this[!MaxCountProperty];
            _candidateList[!ListView.AutoScrollToSelectedItemProperty]  = this[!AutoScrollToSelectedOptionsProperty];
            _candidateList[!ItemsControl.ItemTemplateProperty]          = this[!OptionTemplateProperty];
            _candidateList.SelectionChanged += HandleCandidateListSelectionChanged;
            _candidateList.Commit           += HandleCandidateListComplete;
            _candidateList.Cancel           += HandleCandidateListCanceled;
            ConfigureOptionsBoxSelectionMode();
            SyncSelectionToCandidateList();
        }

        if (!ReferenceEquals(_popupFrame.Child, _candidateList))
        {
            _popupFrame.Child = _candidateList;
        }
        if (!ReferenceEquals(Popup.Child, _popupFrame))
        {
            Popup.Child = _popupFrame;
        }
    }

    private void ClearPopupContent()
    {
        if (_candidateList != null)
        {
            _candidateList.SelectionChanged -= HandleCandidateListSelectionChanged;
            _candidateList.Commit           -= HandleCandidateListComplete;
            _candidateList.Cancel           -= HandleCandidateListCanceled;
            _candidateList.ItemsSource      =  null;
            _candidateList.SetTemplatedParent(null);
        }

        if (_popupFrame != null)
        {
            if (ReferenceEquals(_popupFrame.Child, _candidateList))
            {
                _popupFrame.Child = null;
            }
            _popupFrame.SetTemplatedParent(null);
        }

        if (Popup != null && ReferenceEquals(Popup.Child, _popupFrame))
        {
            Popup.Child = null;
        }

        _candidateList          = null;
        _popupFrame             = null;
        _candidateListActivated = false;
    }

    private void SetupContentRightAddOnBindings(TemplateAppliedEventArgs e)
    {
        _contentRightAddOnBindings?.Dispose();
        _contentRightAddOnBindings = new CompositeDisposable();

        if (e.NameScope.Find<SelectMaxCountIndicator>("PART_SelectMaxCountIndicator") is { } indicator)
        {
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, MaxCountProperty, indicator,
                SelectMaxCountIndicator.MaxCountProperty));
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, SelectedCountProperty, indicator,
                SelectMaxCountIndicator.SelectedCountProperty));
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, IsShowMaxCountIndicatorProperty, indicator,
                Visual.IsVisibleProperty));
        }

        if (e.NameScope.Find<ContentPresenter>("PART_ContentRightAddOnPresenter") is { } contentPresenter)
        {
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, ContentRightAddOnProperty, contentPresenter,
                ContentPresenter.ContentProperty));
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, ContentRightAddOnTemplateProperty,
                contentPresenter, ContentPresenter.ContentTemplateProperty));
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, ContentRightAddOnProperty, contentPresenter,
                Visual.IsVisibleProperty, value => value is not null));
        }

        if (e.NameScope.Find<SelectHandle>("PART_SelectHandle") is { } handle)
        {
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, FormFeedbackProperty, handle,
                SelectHandle.FormFeedbackProperty));
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, SuffixLoadingIconProperty, handle,
                SelectHandle.LoadingIconProperty));
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, SuffixIconProperty, handle,
                SelectHandle.OpenIndicatorProperty));
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, IsEffectiveFilterEnabledProperty, handle,
                SelectHandle.IsFilterEnabledProperty));
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, IsEnabledProperty, handle,
                InputElement.IsEnabledProperty));
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, handle,
                SelectHandle.IsMotionEnabledProperty));
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, IsLoadingProperty, handle,
                SelectHandle.IsLoadingProperty));
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, IsAllowClearProperty, handle,
                SelectHandle.IsAllowClearProperty));
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, IsSelectionEmptyProperty, handle,
                SelectHandle.IsSelectionEmptyProperty));
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, IsDropDownOpenProperty, handle,
                SelectHandle.IsDropDownOpenProperty));

            var addOnBox = e.NameScope.Find<AddOnDecoratedBox>(AddOnDecoratedBox.AddOnDecoratedBoxPart);
            if (addOnBox != null)
            {
                _contentRightAddOnBindings.Add(BindUtils.RelayBind(addOnBox,
                    AddOnDecoratedBox.IsInnerBoxHoverProperty, handle, SelectHandle.IsInputHoverProperty));
                _contentRightAddOnBindings.Add(BindUtils.RelayBind(addOnBox,
                    AddOnDecoratedBox.IsInnerBoxPressedProperty, handle, SelectHandle.IsInputPressedProperty));
            }
        }
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
                SelectedOptions = BuildSelectedOptionsList(_candidateList.SelectedItems);
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
            if ((SelectedOptions == null || SelectedOptions.Count == 0) && e.AddedItems.Count == 0)
            {
                return;
            }

            if (SelectedOptions == null || SelectedOptions.Count == 0)
            {
                var addedSelectedSet = new HashSet<ISelectOption>(e.AddedItems.Count);
                foreach (var item in e.AddedItems)
                {
                    if (item is ISelectOption selectOption)
                    {
                        addedSelectedSet.Add(selectOption);
                    }
                }

                if (addedSelectedSet.Count > 0)
                {
                    SelectedOptions = CopySelectedOptions(addedSelectedSet);
                }
                return;
            }

            var currentSelectedSet = BuildSelectedOptionSet(SelectedOptions);
            var newSelectedSet     = new HashSet<ISelectOption>(currentSelectedSet.Count + e.AddedItems.Count);
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
                SelectedOptions = CopySelectedOptions(newSelectedSet);
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsDropDownOpenProperty)
        {
            ConfigureSingleResultVisible();
            ConfigureSingleFilterTextBox();
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
            ConfigureSingleResultVisible();
            SetCurrentValue(SelectedCountProperty, SelectedOptions?.Count ?? 0);
            CleanDynamicAddedOptions();
            ConfigureSingleFilterTextBox();
        }
        else if (change.Property == ModeProperty)
        {
            ConfigureOptionsBoxSelectionMode();
            ConfigureSingleResultVisible();
            ConfigureSingleFilterTextBox();
        }
        else if (change.Property == FilterValueProperty)
        {
            ConfigurePlaceholderVisible();
            ConfigureSingleResultVisible();
            if (!IsDropDownOpen)
            {
                ConfigureSingleFilterTextBox();
            }
        }
        else if (change.Property == PlaceholderTextProperty)
        {
            ConfigureSingleFilterTextBox();
        }

        if (change.Property == IsFilterEnabledProperty ||
            change.Property == ModeProperty)
        {
            ConfigureEffectiveSearchEnabled();
            ConfigureSingleFilterTextBox();
        }
    }

    protected override void PopupClosed(object? sender, EventArgs e)
    {
        if (Mode == SelectMode.Single)
        {
            FilterValue = null;
            ConfigureSingleFilterTextBox();
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
                    _candidateList.SelectedItems = CopySelectedOptions(SelectedOptions);
                }
            }
        }

        if (Mode == SelectMode.Single && IsEffectiveFilterEnabled)
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
                _candidateList.SelectedItems = CopySelectedOptions(SelectedOptions);
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

    public void ClearValue()
    {
        SelectedOptions = null;
        SelectedOption  = null;
    }

    private void ConfigurePlaceholderVisible()
    {
        if (Mode == SelectMode.Single)
        {
            SetCurrentValue(IsPlaceholderTextVisibleProperty, false);
        }
        else
        {
            SetCurrentValue(IsPlaceholderTextVisibleProperty, (SelectedOptions == null || SelectedOptions?.Count == 0) && string.IsNullOrEmpty(FilterValue?.ToString()));
        }
    }

    private void ConfigureSingleResultVisible()
    {
        SetCurrentValue(IsSingleResultVisibleProperty, false);
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
        if (_singleFilterInput == null)
        {
            return;
        }

        _singleFilterInput.Width = double.NaN;

        if (Mode != SelectMode.Single)
        {
            _singleFilterInput.Focusable            = false;
            _singleFilterInput.IsCaretLockedToStart = true;
            _singleFilterInput.IsReadOnly           = true;
            _singleFilterInput.PlaceholderText      = null;
            SetSingleFilterInputText(null);
            ResetSingleFilterInputCaret();
            return;
        }

        var selectedText = SelectedOption?.Header?.ToString();
        var isEditableSearch = IsDropDownOpen && IsEffectiveFilterEnabled;
        _singleFilterInput.Focusable            = IsEffectiveFilterEnabled;
        _singleFilterInput.IsCaretLockedToStart = !isEditableSearch;
        if (isEditableSearch)
        {
            _singleFilterInput.IsReadOnly           = false;
            _singleFilterInput.PlaceholderText      = selectedText ?? PlaceholderText;
            SetSingleFilterInputText(string.Empty);
            ResetSingleFilterInputCaret();
        }
        else
        {
            _singleFilterInput.IsReadOnly           = true;
            _singleFilterInput.PlaceholderText      = PlaceholderText;
            SetSingleFilterInputText(selectedText ?? string.Empty);
            ResetSingleFilterInputCaret();
        }
    }

    private void SetSingleFilterInputText(string? text)
    {
        if (_singleFilterInput == null || _singleFilterInput.Text == text)
        {
            return;
        }

        _syncingSingleFilterInputText = true;
        try
        {
            _singleFilterInput.Text = text;
        }
        finally
        {
            _syncingSingleFilterInputText = false;
        }
    }

    private void ResetSingleFilterInputCaret()
    {
        if (_singleFilterInput == null)
        {
            return;
        }

        _singleFilterInput.ResetCaretToStart();
    }

    private void HandleSearchInputTextChanged(TextChangedEventArgs e)
    {
        if (_candidateList != null)
        {
            if (e.Source is TextBox textBox)
            {
                if (ReferenceEquals(textBox, _singleFilterInput) &&
                    Mode == SelectMode.Single &&
                    (_syncingSingleFilterInputText || !IsDropDownOpen || !IsEffectiveFilterEnabled))
                {
                    e.Handled = true;
                    return;
                }

                var searchText = textBox.Text?.Trim();
                FilterValue = string.IsNullOrEmpty(searchText) ? null : searchText;
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
            ConfigureSingleResultVisible();

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
                var selectedOptions = new List<ISelectOption>(SelectedOptions.Count > 0 ? SelectedOptions.Count - 1 : 0);
                var removed         = false;
                foreach (var selectedItem in SelectedOptions)
                {
                    if (!removed && EqualityComparer<ISelectOption>.Default.Equals(selectedItem, tagOption))
                    {
                        removed = true;
                        continue;
                    }
                    selectedOptions.Add(selectedItem);
                }
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
    
    private void HandleOptionsSourcePropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        var selectedOptionIdentity  = Mode == SelectMode.Single ? BuildOptionIdentity(SelectedOption) : null;
        var selectedOptionIdentities = Mode != SelectMode.Single ? BuildSelectedOptionIdentities(SelectedOptions) : null;

        if (!Options.IsReadOnly)
        {
            Options.Clear();
        }
        Options.SetItemsSource(change.GetNewValue<IEnumerable<ISelectOption>?>());

        if (Mode == SelectMode.Single)
        {
            if (selectedOptionIdentity != null &&
                TryFindOptionByIdentity(selectedOptionIdentity, out var remappedOption))
            {
                SelectedOption = remappedOption;
            }
            else
            {
                SelectedOption = null;
                ConfigureDefaultValues();
            }
        }
        else if (selectedOptionIdentities != null)
        {
            var remappedOptions = new List<ISelectOption>(selectedOptionIdentities.Count);
            foreach (var identity in selectedOptionIdentities)
            {
                if (TryFindOptionByIdentity(identity, out var remappedOption))
                {
                    remappedOptions.Add(remappedOption);
                }
            }

            SelectedOptions = remappedOptions.Count > 0 ? remappedOptions : null;
            ConfigureDefaultValues();
        }
        else
        {
            ConfigureDefaultValues();
        }
    }

    private static List<string>? BuildSelectedOptionIdentities(ICollection<ISelectOption>? options)
    {
        if (options == null)
        {
            return null;
        }

        var identities = new List<string>(options.Count);
        foreach (var option in options)
        {
            var identity = BuildOptionIdentity(option);
            if (identity != null)
            {
                identities.Add(identity);
            }
        }
        return identities;
    }

    private static string? BuildOptionIdentity(ISelectOption? option)
    {
        if (option == null)
        {
            return null;
        }

        var itemKey = option.ItemKey?.ToString();
        if (!string.IsNullOrEmpty(itemKey))
        {
            return itemKey;
        }
        return option.Content?.ToString();
    }

    private bool TryFindOptionByIdentity(string identity, out ISelectOption option)
    {
        foreach (var item in Options)
        {
            if (item is ISelectOption currentOption &&
                identity == BuildOptionIdentity(currentOption))
            {
                option = currentOption;
                return true;
            }
        }

        option = null!;
        return false;
    }

    private void ConfigureDefaultValues()
    {
        if (Mode == SelectMode.Single)
        {
            if (SelectedOption == null)
            {
                if (DefaultValues?.Count > 0)
                {
                    var defaultValue = DefaultValues[0];
                    foreach (var item in Options)
                    {
                        if (item is ISelectOption option)
                        {
                            if (OptionEqualByValue(defaultValue, option))
                            {
                                SelectedOption = option;
                                break;
                            }
                        }
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
                    var selectedOptions = new List<ISelectOption>(DefaultValues.Count);
                    foreach (var defaultValue in DefaultValues)
                    {
                        foreach (var item in Options)
                        {
                            if (item is ISelectOption option)
                            {
                                if (OptionEqualByValue(defaultValue, option))
                                {
                                    selectedOptions.Add(option);
                                }
                            }
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

    private void CleanDynamicAddedOptions()
    {
        if (Mode != SelectMode.Tags)
        {
            return;
        }

        HashSet<ISelectOption>? selected = null;
        if (SelectedOptions is { Count: > 0 })
        {
            selected = new HashSet<ISelectOption>(SelectedOptions.Count);
            foreach (var opt in SelectedOptions)
            {
                selected.Add(opt);
            }
        }

        List<ISelectOption>? toRemove = null;
        foreach (var item in Options)
        {
            if (item is ISelectOption option)
            {
                if (option.IsDynamicAdded && selected?.Contains(option) != true)
                {
                    toRemove ??= new List<ISelectOption>(Options.Count);
                    toRemove.Add(option);
                }
            }
        }

        if (toRemove == null)
        {
            return;
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

    private static List<ISelectOption>? BuildSelectedOptionsList(IEnumerable? source)
    {
        if (source == null)
        {
            return null;
        }

        var selectedOptions = source switch
        {
            ICollection collection => new List<ISelectOption>(collection.Count),
            IReadOnlyCollection<ISelectOption> collection => new List<ISelectOption>(collection.Count),
            _ => new List<ISelectOption>()
        };
        foreach (var item in source)
        {
            selectedOptions.Add((ISelectOption)item!);
        }
        return selectedOptions;
    }

    private static List<ISelectOption>? CopySelectedOptions(ICollection<ISelectOption>? source)
    {
        if (source == null)
        {
            return null;
        }

        var selectedOptions = new List<ISelectOption>(source.Count);
        foreach (var option in source)
        {
            selectedOptions.Add(option);
        }
        return selectedOptions;
    }

    private static HashSet<ISelectOption> BuildSelectedOptionSet(ICollection<ISelectOption>? source)
    {
        var selectedOptions = source == null
            ? new HashSet<ISelectOption>()
            : new HashSet<ISelectOption>(source.Count);
        if (source != null)
        {
            foreach (var option in source)
            {
                selectedOptions.Add(option);
            }
        }
        return selectedOptions;
    }
}
