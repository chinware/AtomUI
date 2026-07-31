using System.Collections;
using System.Diagnostics;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Controls.Utils;
using AtomUI.Desktop.Controls.DataLoad;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

using ItemCollection = AtomUI.Collections.ItemCollection;

public enum CascaderViewExpandTrigger
{
    Click,
    Hover
}

public partial class CascaderView : TemplatedControl, 
                                    IMotionAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<IEnumerable<ICascaderOption>?> OptionsSourceProperty =
        AvaloniaProperty.Register<CascaderView, IEnumerable<ICascaderOption>?>(nameof(OptionsSource));
    
    public static readonly StyledProperty<IDataTemplate?> OptionTemplateProperty =
        AvaloniaProperty.Register<CascaderView, IDataTemplate?>(nameof(OptionTemplate));
    
    public static readonly StyledProperty<IconTemplate?> ExpandIconProperty =
        AvaloniaProperty.Register<CascaderView, IconTemplate?>(nameof(ExpandIcon));
    
    public static readonly StyledProperty<IconTemplate?> LoadingIconProperty =
        AvaloniaProperty.Register<CascaderView, IconTemplate?>(nameof(LoadingIcon));
    
    public static readonly StyledProperty<CascaderViewExpandTrigger> ExpandTriggerProperty =
        AvaloniaProperty.Register<CascaderView, CascaderViewExpandTrigger>(nameof(ExpandTrigger), CascaderViewExpandTrigger.Click);
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<CascaderView>();
    
    public static readonly StyledProperty<bool> IsCheckableProperty =
        AvaloniaProperty.Register<CascaderView, bool>(nameof(IsCheckable));
    
    public static readonly StyledProperty<TreeNodePath?> DefaultExpandedPathProperty =
        AvaloniaProperty.Register<CascaderView, TreeNodePath?>(nameof(DefaultExpandedPath));
    
    public static readonly StyledProperty<ICascaderItemDataLoader?> DataLoaderProperty =
        AvaloniaProperty.Register<CascaderView, ICascaderItemDataLoader?>(nameof(DataLoader));
    
    public static readonly StyledProperty<IValueFilter?> FilterProperty =
        AvaloniaProperty.Register<CascaderView, IValueFilter?>(nameof(Filter));

    public static readonly StyledProperty<object?> FilterValueProperty =
        AvaloniaProperty.Register<CascaderView, object?>(nameof(FilterValue));

    public static readonly StyledProperty<DefaultFilterValueSelector?> FilterValueSelectorProperty =
        AvaloniaProperty.Register<CascaderView, DefaultFilterValueSelector?>(nameof(FilterValueSelector));
    
    public static readonly StyledProperty<TextBlockHighlightStrategy> FilterHighlightStrategyProperty =
        AvaloniaProperty.Register<CascaderView, TextBlockHighlightStrategy>(nameof(FilterHighlightStrategy), TextBlockHighlightStrategy.All);
    
    public static readonly DirectProperty<CascaderView, int> FilterResultCountProperty =
        AvaloniaProperty.RegisterDirect<CascaderView, int>(nameof(FilterResultCount),
            o => o.FilterResultCount);
    
    public static readonly StyledProperty<IBrush?> FilterHighlightForegroundProperty =
        AvaloniaProperty.Register<CascaderView, IBrush?>(nameof(FilterHighlightForeground));
    
    public static readonly StyledProperty<object?> EmptyIndicatorProperty =
        AvaloniaProperty.Register<CascaderView, object?>(nameof(EmptyIndicator));
    
    public static readonly StyledProperty<IDataTemplate?> EmptyIndicatorTemplateProperty =
        AvaloniaProperty.Register<CascaderView, IDataTemplate?>(nameof(EmptyIndicatorTemplate));
    
    public static readonly StyledProperty<bool> IsShowEmptyIndicatorProperty =
        AvaloniaProperty.Register<CascaderView, bool>(nameof(IsShowEmptyIndicator), true);
    
    public static readonly StyledProperty<bool> IsAllowSelectParentProperty =
        AvaloniaProperty.Register<CascaderView, bool>(nameof(IsAllowSelectParent));
    
    public static readonly StyledProperty<Thickness> EmptyIndicatorPaddingProperty =
        AvaloniaProperty.Register<CascaderView, Thickness>(nameof(EmptyIndicatorPadding));
    
    public static readonly DirectProperty<CascaderView, ICascaderOption?> SelectedOptionProperty =
        AvaloniaProperty.RegisterDirect<CascaderView, ICascaderOption?>(
            nameof(SelectedOption),
            o => o.SelectedOption,
            (o, v) => o.SelectedOption = v);
    
    public static readonly DirectProperty<CascaderView, IList<ICascaderOption>?> SelectedOptionsProperty =
        AvaloniaProperty.RegisterDirect<CascaderView, IList<ICascaderOption>?>(
            nameof(SelectedOptions),
            o => o.SelectedOptions,
            (o, v) => o.SelectedOptions = v);
    
    public IEnumerable<ICascaderOption>? OptionsSource
    {
        get => GetValue(OptionsSourceProperty);
        set => SetValue(OptionsSourceProperty, value);
    }
    
    [InheritDataTypeFromItems("OptionsSource")]
    public IDataTemplate? OptionTemplate
    {
        get => GetValue(OptionTemplateProperty);
        set => SetValue(OptionTemplateProperty, value);
    }

    public IconTemplate? ExpandIcon
    {
        get => GetValue(ExpandIconProperty);
        set => SetValue(ExpandIconProperty, value);
    }
    
    public IconTemplate? LoadingIcon
    {
        get => GetValue(LoadingIconProperty);
        set => SetValue(LoadingIconProperty, value);
    }
    
    public CascaderViewExpandTrigger ExpandTrigger
    {
        get => GetValue(ExpandTriggerProperty);
        set => SetValue(ExpandTriggerProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public bool IsCheckable
    {
        get => GetValue(IsCheckableProperty);
        set => SetValue(IsCheckableProperty, value);
    }
    
    public TreeNodePath? DefaultExpandedPath
    {
        get => GetValue(DefaultExpandedPathProperty);
        set => SetValue(DefaultExpandedPathProperty, value);
    }
    
    public ICascaderItemDataLoader? DataLoader
    {
        get => GetValue(DataLoaderProperty);
        set => SetValue(DataLoaderProperty, value);
    }
    
    public IValueFilter? Filter
    {
        get => GetValue(FilterProperty);
        set => SetValue(FilterProperty, value);
    }

    public object? FilterValue
    {
        get => GetValue(FilterValueProperty);
        set => SetValue(FilterValueProperty, value);
    }

    public DefaultFilterValueSelector? FilterValueSelector
    {
        get => GetValue(FilterValueSelectorProperty);
        set => SetValue(FilterValueSelectorProperty, value);
    }
    
    public TextBlockHighlightStrategy FilterHighlightStrategy
    {
        get => GetValue(FilterHighlightStrategyProperty);
        set => SetValue(FilterHighlightStrategyProperty, value);
    }
    
    private int _filterResultCount;
    
    public int FilterResultCount
    {
        get => _filterResultCount;
        set => SetAndRaise(FilterResultCountProperty, ref _filterResultCount, value);
    }
    
    public IBrush? FilterHighlightForeground
    {
        get => GetValue(FilterHighlightForegroundProperty);
        set => SetValue(FilterHighlightForegroundProperty, value);
    }
    
    [DependsOn(nameof(EmptyIndicatorTemplate))]
    public object? EmptyIndicator
    {
        get => GetValue(EmptyIndicatorProperty);
        set => SetValue(EmptyIndicatorProperty, value);
    }

    public IDataTemplate? EmptyIndicatorTemplate
    {
        get => GetValue(EmptyIndicatorTemplateProperty);
        set => SetValue(EmptyIndicatorTemplateProperty, value);
    }
    
    public bool IsShowEmptyIndicator
    {
        get => GetValue(IsShowEmptyIndicatorProperty);
        set => SetValue(IsShowEmptyIndicatorProperty, value);
    }
    
    /// <summary>
    /// 一版情况只有在点击叶子节点的时候才会触发 Select 事件
    /// </summary>
    public bool IsAllowSelectParent
    {
        get => GetValue(IsAllowSelectParentProperty);
        set => SetValue(IsAllowSelectParentProperty, value);
    }
    
    public Thickness EmptyIndicatorPadding
    {
        get => GetValue(EmptyIndicatorPaddingProperty);
        set => SetValue(EmptyIndicatorPaddingProperty, value);
    }
    
    private ICascaderOption? _selectedOption;
    
    public ICascaderOption? SelectedOption
    {
        get => _selectedOption;
        set => SetAndRaise(SelectedOptionProperty, ref _selectedOption, value);
    }
    
    private IList<ICascaderOption>? _checkedOptions;
    
    public IList<ICascaderOption>? SelectedOptions
    {
        get => _checkedOptions;
        set => SetAndRaise(SelectedOptionsProperty, ref _checkedOptions, value);
    }
    
    public ItemsSourceView OptionsView => ItemsSourceView.GetOrCreate(_options);
    
    [Content]
    public ItemCollection Options => _options;
    
    #endregion
    
    #region 公共事件定义
    
    public event EventHandler<CascaderOptionsSelectedChangedEventArgs>? SelectedOptionsChanged;
    public event EventHandler<CascaderViewItemLoadedEventArgs>? ItemAsyncLoaded;
    public event EventHandler<CascaderItemExpandedEventArgs>? ItemExpanded;
    public event EventHandler<CascaderItemCollapsedEventArgs>? ItemCollapsed;
    public event EventHandler<CascaderItemClickedEventArgs>? ItemClicked;
    public event EventHandler<CascaderItemDoubleClickedEventArgs>? ItemDoubleClicked;
    public event EventHandler<CascaderOptionSelectedEventArgs>? OptionSelected;
    #endregion

    #region 内部属性定义
    internal static readonly DirectProperty<CascaderView, bool> IsEffectiveEmptyVisibleProperty =
        AvaloniaProperty.RegisterDirect<CascaderView, bool>(
            nameof(IsEffectiveEmptyVisible),
            o => o.IsEffectiveEmptyVisible,
            (o, v) => o.IsEffectiveEmptyVisible = v);

    internal static readonly DirectProperty<CascaderView, bool> IsDefaultEmptyIndicatorVisibleProperty =
        AvaloniaProperty.RegisterDirect<CascaderView, bool>(
            nameof(IsDefaultEmptyIndicatorVisible),
            o => o.IsDefaultEmptyIndicatorVisible,
            (o, v) => o.IsDefaultEmptyIndicatorVisible = v);
    
    internal static readonly DirectProperty<CascaderView, ItemToggleType> EffectiveToggleTypeProperty =
        AvaloniaProperty.RegisterDirect<CascaderView, ItemToggleType>(
            nameof(EffectiveToggleType),
            o => o.EffectiveToggleType,
            (o, v) => o.EffectiveToggleType = v);
    
    internal static readonly DirectProperty<CascaderView, bool> IsMaxSelectReachedProperty =
        AvaloniaProperty.RegisterDirect<CascaderView, bool>(nameof(IsMaxSelectReached),
            o => o.IsMaxSelectReached,
            (o, v) => o.IsMaxSelectReached = v);
    
    private bool _isEffectiveEmptyVisible = false;
    internal bool IsEffectiveEmptyVisible
    {
        get => _isEffectiveEmptyVisible;
        set => SetAndRaise(IsEffectiveEmptyVisibleProperty, ref _isEffectiveEmptyVisible, value);
    }

    private bool _isDefaultEmptyIndicatorVisible;
    internal bool IsDefaultEmptyIndicatorVisible
    {
        get => _isDefaultEmptyIndicatorVisible;
        set => SetAndRaise(IsDefaultEmptyIndicatorVisibleProperty, ref _isDefaultEmptyIndicatorVisible, value);
    }
    
    private ItemToggleType _effectiveToggleType = ItemToggleType.None;
    internal ItemToggleType EffectiveToggleType
    {
        get => _effectiveToggleType;
        set => SetAndRaise(EffectiveToggleTypeProperty, ref _effectiveToggleType, value);
    }

    private bool _isMaxSelectReached;

    internal bool IsMaxSelectReached
    {
        get => _isMaxSelectReached;
        set => SetAndRaise(IsMaxSelectReachedProperty, ref _isMaxSelectReached, value);
    }
    
    #endregion
    
    private readonly ItemCollection _options = new();
    private bool _ignoreSyncSelectedOptions;
    private StackPanel? _itemsPanel;
    private int _ignoreExpandAndCollapseLevel;
    private bool _defaultExpandPathApplied;
    private bool _isSynchronizingSelectedOptionToView;
    private CascaderViewLevelList? _rootLevelList;
    private CascaderViewItem? _keyboardCandidateItem;
    private DispatcherOperation? _pendingSelectedOptionPresentation;
    
    static CascaderView()
    {
        FocusableProperty.OverrideDefaultValue<CascaderView>(true);
        SetupExpandAndCollapse();
        SetupChecked();
        CascaderViewItem.DoubleTappedEvent.AddClassHandler<CascaderView>((view, args) => view.HandleCascaderItemDoubleClicked(args));
        CascaderViewItem.ClickedEvent.AddClassHandler<CascaderView>((view, args) => view.HandleCascaderItemClicked(args));
        CascaderViewItem.SelectedEvent.AddClassHandler<CascaderView>((view, args) => view.HandleCascaderOptionSelected(args));
        OptionsSourceProperty.Changed.AddClassHandler<CascaderView>((view, args) => view.HandleCascaderSourceChanged(args));
    }
    
    public CascaderView()
    {
        _options.CollectionChanged += HandleCollectionChanged;
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
            SetCurrentValue(FilterValueSelectorProperty, DefaultCascaderFilterValueSelector);
        }
        ConfigureEmptyIndicator();
        ConfigureEffectiveToggleType();
    }

    internal static readonly DefaultFilterValueSelector DefaultCascaderFilterValueSelector = value =>
    {
        if (value is ICascaderItemInfo info)
        {
            return info.Path;
        }
        return null;
    };
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _itemsPanel    = e.NameScope.Find<StackPanel>("PART_ItemsPanel");
        _rootLevelList = e.NameScope.Find<CascaderViewLevelList>("PART_RootLevelList");
        if (_rootLevelList != null)
        {
            _rootLevelList.Level     = 1;
            _rootLevelList.OwnerView = this;
        }
        
        if (_filterList != null)
        {
            _filterList.SelectionChanged -= HandleFilterListSelectionChanged;
        }
        _filterList = e.NameScope.Find<CascaderViewFilterList>("PART_FilterList");
        
        if (_filterList != null)
        {
            _filterList.ClearCandidate();
            _filterList.SelectionChanged += HandleFilterListSelectionChanged;
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        FilterItems();
        if (DefaultExpandedPath != null && !_defaultExpandPathApplied)
        {
            ApplyDefaultExpandPath();
            _defaultExpandPathApplied = true;
        }
        
        RestoreSelectedOptionPath();
    }

    internal void ResetInteractionState()
    {
        CancelPendingSelectedOptionPresentation();
        SetKeyboardCandidate(null);
        _filterList?.ClearCandidate();
        CollapseAll();
    }

    internal void RestoreSelectedOptionPath()
    {
        if (SelectedOption is { } option)
        {
            SelectTargetOption(option);
        }
    }

    internal bool TryParseSelectPath(TreeNodePath path, out IList<ICascaderOption> pathNodes)
    {
        var                    segments     = path.Segments;
        var                    count        = path.Segments.Count;
        var                    isPathValid  = true;
        IEnumerable<ICascaderOption> currentItems = BuildOptionList(_options);
        
        var                          options    = new List<ICascaderOption>(count);
        for (var i = 0; i < count; i++)
        {
            var segment = segments[i];
            var found   = false;
            foreach (var currentItem in currentItems)
            {
                if (segment == currentItem.ItemKey || segment == currentItem.Value?.ToString())
                {
                    options.Add(currentItem);
                    currentItems = currentItem.Children;
                    found        = true;
                }
            }
    
            if (!found)
            {
                isPathValid = false;
                break;
            }
        }
        pathNodes = options;
    
        return isPathValid;
    }

    private static List<ICascaderOption> BuildOptionList(IEnumerable source)
    {
        var options = source switch
        {
            ICollection collection => new List<ICascaderOption>(collection.Count),
            IReadOnlyCollection<ICascaderOption> collection => new List<ICascaderOption>(collection.Count),
            _ => new List<ICascaderOption>()
        };
        foreach (var item in source)
        {
            options.Add((ICascaderOption)item!);
        }
        return options;
    }
    
    private void ApplyDefaultExpandPath()
    {
        Debug.Assert(DefaultExpandedPath != null);
        if (TryParseSelectPath(DefaultExpandedPath, out IList<ICascaderOption> pathNodes))
        {
            if (pathNodes.Count > 0)
            {
                Dispatcher.InvokeAsync(async () =>
                {
                    await ExpandItemAsync(pathNodes[^1]);
                });
            }
        }
    }

    private void HandleCascaderSourceChanged(AvaloniaPropertyChangedEventArgs args)
    {
        CancelPendingSelectedOptionPresentation();
        ResetLevelListsForOptionsSourceChanging();
        _options.SetItemsSource(args.GetNewValue<IEnumerable<ICascaderOption>?>());
        RestoreRootLevelListItemsSource();
        if (DefaultExpandedPath != null && IsLoaded)
        {
            _defaultExpandPathApplied = false;
            ApplyDefaultExpandPath();
            _defaultExpandPathApplied = true;
        }
    }

    private void ResetLevelListsForOptionsSourceChanging()
    {
        if (_itemsPanel == null)
        {
            return;
        }

        for (var i = _itemsPanel.Children.Count - 1; i >= 0; i--)
        {
            if (_itemsPanel.Children[i] is CascaderViewLevelList levelList)
            {
                levelList.ResetVirtualizingContext();
                levelList.SelectedIndex = -1;
                levelList.SelectedItem = null;
                levelList.ItemsSource  = null;
                if (i > 0)
                {
                    levelList.Items.Clear();
                    _itemsPanel.Children.RemoveAt(i);
                }
                levelList.ResetVirtualizingContext();
            }
        }
    }

    private void RestoreRootLevelListItemsSource()
    {
        if (_rootLevelList == null)
        {
            return;
        }

        _rootLevelList.ItemsSource = Options;
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
                
        if (change.Property == DataLoaderProperty)
        {
            HasItemAsyncDataLoader = DataLoader != null;
        }
        else if (change.Property == FilterProperty ||
                 change.Property == FilterHighlightStrategyProperty ||
                 change.Property == OptionsSourceProperty ||
                 change.Property == FilterValueProperty)
        {
            FilterItems();
        }
    
        if (change.Property == IsShowEmptyIndicatorProperty ||
            change.Property == OptionsSourceProperty ||
            change.Property == FilterResultCountProperty ||
            change.Property == EmptyIndicatorProperty ||
            change.Property == EmptyIndicatorTemplateProperty)
        {
            ConfigureEmptyIndicator();
        }
        else if (change.Property == IsCheckableProperty)
        {
            ConfigureEffectiveToggleType();
        }
        else if (change.Property == SelectedOptionProperty)
        {
            if (SelectedOption != null)
            {
                if (IsLoaded)
                {
                    SelectTargetOption(SelectedOption);
                }
            }
            else
            {
                CancelPendingSelectedOptionPresentation();
                CollapseAll();
            }
        }
    }
    
    private void SelectTargetOption(ICascaderOption option)
    {
        var isLeaf = option.IsLeaf || !option.HasChildren();
    
        if (!isLeaf && !IsAllowSelectParent)
        {
            throw new ArgumentException($"Option {option.Header} is not a Leaf node.");
        }
    
        CancelPendingSelectedOptionPresentation();
        _pendingSelectedOptionPresentation = Dispatcher.InvokeAsync<Task>(async () =>
        {
            await ExpandItemAsync(option);
            var targetLevelList = GetLevelListForOption(option);
            if (targetLevelList != null)
            {
                try
                {
                    _isSynchronizingSelectedOptionToView = true;
                    targetLevelList.SelectedItem          = option;
                }
                finally
                {
                    _isSynchronizingSelectedOptionToView = false;
                }
            }
        });
    }

    private void CancelPendingSelectedOptionPresentation()
    {
        _pendingSelectedOptionPresentation?.Abort();
        _pendingSelectedOptionPresentation = null;
    }

    private void SelectOptionFromInteraction(ICascaderOption option)
    {
        SetCurrentValue(SelectedOptionProperty, option);
        OptionSelected?.Invoke(this, new CascaderOptionSelectedEventArgs(option));
    }
    
    private void HandleCascaderItemClicked(RoutedEventArgs args)
    {
        if (args.Source is CascaderViewItem item)
        {
            NotifyCascaderItemClicked(item);
            ItemClicked?.Invoke(this, new CascaderItemClickedEventArgs(item));
            if (IsCheckable)
            {
                if (item.IsLeaf && item.IsCheckBoxEnabled)
                {
                    if (item.IsChecked == true)
                    {
                        item.IsChecked = false;
                    }
                    else
                    {
                        item.IsChecked = true;
                    }
                }
            }
        }
    }

    private void HandleCascaderOptionSelected(RoutedEventArgs args)
    {
        if (args.Source is CascaderViewItem item)
        {
            if (_isSynchronizingSelectedOptionToView)
            {
                return;
            }
            if (item.IsSelected)
            {
                var option = item.AttachedOption!;
                SelectOptionFromInteraction(option);
            }
        }
    }

    protected virtual void NotifyCascaderItemClicked(CascaderViewItem item)
    {
    }
    
    protected virtual void ConfigureEmptyIndicator()
    {
        var isEmpty = false;
        if (IsFiltering)
        {
            isEmpty = FilterResultCount == 0;
        }
        else
        {
            if (OptionsSource != null)
            {
                var enumerator = OptionsSource.GetEnumerator();
                isEmpty = !enumerator.MoveNext();
                if (enumerator is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            else
            {
                isEmpty = _options.Count == 0;
            }
        }
        IsEffectiveEmptyVisible        = IsShowEmptyIndicator && isEmpty;
        IsDefaultEmptyIndicatorVisible = IsEffectiveEmptyVisible && EmptyIndicator is null && EmptyIndicatorTemplate is null;
    }
    
    private void HandleCascaderItemDoubleClicked(RoutedEventArgs args)
    {
        if (args.Source is Control source)
        {
            var cascaderItem = source.FindAncestorOfType<CascaderViewItem>();
            if (cascaderItem != null)
            {
                ItemDoubleClicked?.Invoke(this, new CascaderItemDoubleClickedEventArgs(cascaderItem));
            }
        }
    }

    private void ConfigureEffectiveToggleType()
    {
        if (IsCheckable)
        {
            EffectiveToggleType = ItemToggleType.CheckBox;
        }
        else
        {
            EffectiveToggleType = ItemToggleType.None;
        }
    }
    
    private CascaderViewItem? GetContainerFromEventSource(object? eventSource)
    {
        for (var current = eventSource as Visual; current != null; current = current.GetVisualParent())
        {
            if (current is CascaderViewItem cascaderViewItem)
            {
                return cascaderViewItem;
            }
        }
        return null;
    }
    
    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (e.Source is Visual source)
        {
            if (ExpandTrigger == CascaderViewExpandTrigger.Hover)
            {
                var cascaderViewItem = GetContainerFromEventSource(e.Source);
                if (cascaderViewItem != null)
                {
                    if (!cascaderViewItem.IsExpanded)
                    {
                        Dispatcher.InvokeAsync(async () =>
                        {
                            await ExpandItemAsync(cascaderViewItem);
                        });
                    }
                    else
                    {
                        cascaderViewItem.NotifyClearDescendantExpanded();
                    }
                }
            }
        }
    }
    
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (!e.Handled)
        {
            HandleKeyDown(e);
        }
    }

    public void HandleKeyDown(KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Enter:
                e.Handled = IsFiltering ? TryCommitFilterCandidate() : TryCommitKeyboardCandidate();
                break;

            case Key.Up:
                e.Handled = IsFiltering ? TryMoveFilterCandidate(-1) : TryMoveKeyboardCandidate(-1);
                break;

            case Key.Down:
                e.Handled = IsFiltering ? TryMoveFilterCandidate(1) : TryMoveKeyboardCandidate(1);
                break;

            case Key.Left:
                e.Handled = !IsFiltering && TryMoveKeyboardCandidateToParent();
                break;

            case Key.Right:
                e.Handled = !IsFiltering && TryExpandKeyboardCandidateOrMoveToChild();
                break;

            default:
                break;
        }
    }

    private bool TryMoveKeyboardCandidate(int delta)
    {
        var candidates = GetVisibleKeyboardCandidates();
        if (candidates.Count == 0 || delta == 0)
        {
            return false;
        }

        var index = _keyboardCandidateItem != null ? candidates.IndexOf(_keyboardCandidateItem) : -1;
        if (index == -1)
        {
            index = delta > 0 ? 0 : candidates.Count - 1;
        }
        else
        {
            index += delta;
            if (index < 0)
            {
                index = candidates.Count - 1;
            }
            else if (index >= candidates.Count)
            {
                index = 0;
            }
        }

        SetKeyboardCandidate(candidates[index]);
        return true;
    }

    private bool TryExpandKeyboardCandidateOrMoveToChild()
    {
        var candidate = GetKeyboardCandidateOrFirstVisibleItem();
        if (candidate?.AttachedOption == null || candidate.IsLeaf || candidate.IsLoading || !candidate.IsEnabled)
        {
            return false;
        }

        Dispatcher.InvokeAsync(async () =>
        {
            await ExpandItemAsync(candidate);
            ExecuteLayoutPass();
            var child = GetFirstEnabledChildCandidate(candidate);
            if (child != null)
            {
                SetKeyboardCandidate(child);
            }
        });
        return true;
    }

    private bool TryMoveKeyboardCandidateToParent()
    {
        var candidate = GetKeyboardCandidateOrFirstVisibleItem();
        if (candidate == null)
        {
            return false;
        }

        if (candidate.AttachedOption?.ParentNode is ICascaderOption parentOption)
        {
            var parentList = GetLevelListForOption(parentOption);
            var parentItem = parentList?.ContainerFromItem(parentOption) as CascaderViewItem;
            if (parentItem != null)
            {
                SetKeyboardCandidate(parentItem);
                return true;
            }
        }

        if (!candidate.IsLeaf && candidate.IsExpanded)
        {
            CollapseItem(candidate);
            return true;
        }

        return false;
    }

    private bool TryCommitKeyboardCandidate()
    {
        var candidate = GetKeyboardCandidateOrFirstVisibleItem();
        if (candidate?.AttachedOption == null || candidate.IsLoading || !candidate.IsEnabled)
        {
            return false;
        }

        if (candidate.IsLeaf || IsAllowSelectParent)
        {
            SelectOptionFromInteraction(candidate.AttachedOption);
            return true;
        }

        return TryExpandKeyboardCandidateOrMoveToChild();
    }

    private CascaderViewItem? GetKeyboardCandidateOrFirstVisibleItem()
    {
        var candidates = GetVisibleKeyboardCandidates();
        if (_keyboardCandidateItem?.AttachedOption != null && candidates.Contains(_keyboardCandidateItem))
        {
            return _keyboardCandidateItem;
        }

        SetKeyboardCandidate(candidates.Count > 0 ? candidates[0] : null);
        return _keyboardCandidateItem;
    }

    private CascaderViewItem? GetFirstEnabledChildCandidate(CascaderViewItem parentItem)
    {
        if (parentItem.AttachedOption == null)
        {
            return null;
        }

        var childList = GetLevelList(GetViewOptionLevel(parentItem.AttachedOption));
        if (childList == null)
        {
            return null;
        }

        for (var i = 0; i < childList.ItemCount; i++)
        {
            if (childList.ContainerFromIndex(i) is CascaderViewItem child && child.IsEnabled)
            {
                return child;
            }
        }

        return null;
    }

    private void SetKeyboardCandidate(CascaderViewItem? item)
    {
        if (ReferenceEquals(_keyboardCandidateItem, item))
        {
            return;
        }

        _keyboardCandidateItem?.SetCurrentValue(CascaderViewItem.IsCandidateSelectedProperty, false);
        _keyboardCandidateItem = item;
        _keyboardCandidateItem?.SetCurrentValue(CascaderViewItem.IsCandidateSelectedProperty, true);
    }

    private List<CascaderViewItem> GetVisibleKeyboardCandidates()
    {
        var candidates = new List<CascaderViewItem>();
        if (_itemsPanel == null)
        {
            return candidates;
        }

        foreach (var child in _itemsPanel.Children)
        {
            if (child is not CascaderViewLevelList levelList)
            {
                continue;
            }

            for (var i = 0; i < levelList.ItemCount; i++)
            {
                if (levelList.ContainerFromIndex(i) is CascaderViewItem item && item.IsEnabled)
                {
                    candidates.Add(item);
                }
            }
        }

        return candidates;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        CancelPendingSelectedOptionPresentation();
        
        // 清理所有待处理的异步加载操作
        _asyncLoadCoordinator.CancelAll();
    }
}
