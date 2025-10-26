// Modified based on https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Controls/ListBox.cs
 
using System.Collections;
using System.Collections.Specialized;
using System.Reactive.Disposables;
using AtomUI.Controls.Themes;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Selection;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Metadata;

namespace AtomUI.Controls;


public class ListBox : SelectingItemsControl,
                       IMotionAwareControl,
                       IControlSharedTokenResourcesHost
{
    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new VirtualizingStackPanel());
    
    #region 公共属性定义
    
    public static readonly DirectProperty<ListBox, IScrollable?> ScrollProperty =
        AvaloniaProperty.RegisterDirect<ListBox, IScrollable?>(nameof(Scroll), o => o.Scroll);
    
    public static readonly StyledProperty<object?> EmptyIndicatorProperty =
        AvaloniaProperty.Register<ListBox, object?>(nameof(EmptyIndicator));
    
    public static readonly StyledProperty<IDataTemplate?> EmptyIndicatorTemplateProperty =
        AvaloniaProperty.Register<ListBox, IDataTemplate?>(nameof(EmptyIndicatorTemplate));
    
    public static readonly StyledProperty<bool> IsShowEmptyIndicatorProperty =
        AvaloniaProperty.Register<ListBox, bool>(nameof(IsShowEmptyIndicator), false);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("AvaloniaProperty", "AVP1010",
        Justification = "This property is owned by SelectingItemsControl, but protected there. ListBox changes its visibility.")]
    public new static readonly DirectProperty<SelectingItemsControl, IList?> SelectedItemsProperty =
        SelectingItemsControl.SelectedItemsProperty;
    
    [System.Diagnostics.CodeAnalysis.SuppressMessage("AvaloniaProperty", "AVP1010",
        Justification = "This property is owned by SelectingItemsControl, but protected there. ListBox changes its visibility.")]
    public new static readonly DirectProperty<SelectingItemsControl, ISelectionModel> SelectionProperty =
        SelectingItemsControl.SelectionProperty;
    
    [System.Diagnostics.CodeAnalysis.SuppressMessage("AvaloniaProperty", "AVP1010",
        Justification = "This property is owned by SelectingItemsControl, but protected there. ListBox changes its visibility.")]
    public new static readonly StyledProperty<SelectionMode> SelectionModeProperty =
        SelectingItemsControl.SelectionModeProperty;
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<ListBox>();

    public static readonly StyledProperty<bool> DisabledItemHoverEffectProperty =
        AvaloniaProperty.Register<ListBox, bool>(nameof(DisabledItemHoverEffect));
    
    public static readonly StyledProperty<bool> IsBorderlessProperty =
        AvaloniaProperty.Register<ListBox, bool>(nameof(IsBorderless), false);
    
    public static readonly StyledProperty<bool> IsShowSelectedIndicatorProperty =
        AvaloniaProperty.Register<ListBox, bool>(nameof(IsShowSelectedIndicator), false);
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ListBox>();
    
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
    
    private IScrollable? _scroll;
    public IScrollable? Scroll
    {
        get => _scroll;
        private set => SetAndRaise(ScrollProperty, ref _scroll, value);
    }
    
    public new IList? SelectedItems
    {
        get => base.SelectedItems;
        set => base.SelectedItems = value;
    }
    
    public new ISelectionModel Selection
    {
        get => base.Selection;
        set => base.Selection = value;
    }
    
    [System.Diagnostics.CodeAnalysis.SuppressMessage("AvaloniaProperty", "AVP1012",
        Justification = "This property is owned by SelectingItemsControl, but protected there. ListBox changes its visibility.")]
    public new SelectionMode SelectionMode
    {
        get => base.SelectionMode;
        set => base.SelectionMode = value;
    }
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public bool DisabledItemHoverEffect
    {
        get => GetValue(DisabledItemHoverEffectProperty);
        set => SetValue(DisabledItemHoverEffectProperty, value);
    }
    
    public bool IsShowSelectedIndicator
    {
        get => GetValue(IsShowSelectedIndicatorProperty);
        set => SetValue(IsShowSelectedIndicatorProperty, value);
    }
    
    public bool IsBorderless
    {
        get => GetValue(IsBorderlessProperty);
        set => SetValue(IsBorderlessProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion
    
    #region 内部属性定义
    
    internal static readonly DirectProperty<ListBox, Thickness> EffectiveBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<ListBox, Thickness>(nameof(EffectiveBorderThickness),
            o => o.EffectiveBorderThickness,
            (o, v) => o.EffectiveBorderThickness = v);
    
    private Thickness _effectiveBorderThickness;

    internal Thickness EffectiveBorderThickness
    {
        get => _effectiveBorderThickness;
        set => SetAndRaise(EffectiveBorderThicknessProperty, ref _effectiveBorderThickness, value);
    }

    protected override Type StyleKeyOverride { get; } = typeof(ListBox);
    
    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ListBoxToken.ID;

    #endregion
    
    private protected readonly Dictionary<object, CompositeDisposable> _itemsBindingDisposables = new();
    private IDisposable? _borderThicknessDisposable;

    static ListBox()
    {
        ItemsPanelProperty.OverrideDefaultValue<ListBox>(DefaultPanel);
        KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue(
            typeof(ListBox),
            KeyboardNavigationMode.Once);
    }
    
    public ListBox()
    {
        this.RegisterResources();
        LogicalChildren.CollectionChanged += HandleChildrenChanged;
    }
    
    public void SelectAll() => Selection.SelectAll();
    
    private void HandleChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems.Count > 0)
            {
                foreach (var item in e.OldItems)
                {
                    DisposableListItem(item);
                }
            }
        }
    }

    private protected void DisposableListItem(object item)
    {
        if (_itemsBindingDisposables.TryGetValue(item, out var disposable))
        {
            disposable.Dispose();
            _itemsBindingDisposables.Remove(item);
        }
    }
    
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new ListBoxItem();
    }
    
    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        var disposables = new CompositeDisposable(4);
        if (container is ListBoxItem listBoxItem)
        {
            if (item != null && item is not Visual)
            {
                ApplyListItemData(listBoxItem, item);
            }
            
            if (ItemTemplate != null)
            {
                disposables.Add(BindUtils.RelayBind(this, ItemTemplateProperty, listBoxItem, ListBoxItem.ContentTemplateProperty));
            }
            
            disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, listBoxItem, ListBoxItem.IsMotionEnabledProperty));
            disposables.Add(BindUtils.RelayBind(this, SizeTypeProperty, listBoxItem, ListBoxItem.SizeTypeProperty));
            disposables.Add(BindUtils.RelayBind(this, IsShowSelectedIndicatorProperty, listBoxItem, ListBoxItem.IsShowSelectedIndicatorProperty));
            disposables.Add(BindUtils.RelayBind(this, DisabledItemHoverEffectProperty, listBoxItem,
                ListBoxItem.DisabledItemHoverEffectProperty));
            
            PrepareListBoxItem(listBoxItem, item, index, disposables);
            DisposableListItem(listBoxItem);
            _itemsBindingDisposables.Add(listBoxItem, disposables);
        }
    }
    
    protected virtual void PrepareListBoxItem(ListBoxItem listBoxItem, object? item, int index, CompositeDisposable compositeDisposable)
    {
    }

    protected virtual void ApplyListItemData(ListBoxItem listBoxItem, object item)
    {
        if (!listBoxItem.IsSet(ListBoxItem.ContentProperty))
        {
            listBoxItem.SetCurrentValue(ListBoxItem.ContentProperty, item);
        }

        if (item is IListBoxItemData listBoxItemData)
        {
            if (!listBoxItem.IsSet(ListBoxItem.IsSelectedProperty))
            {
                listBoxItem.SetCurrentValue(ListBoxItem.IsSelectedProperty, listBoxItemData.IsSelected);
            }
            if (!listBoxItem.IsSet(ListBoxItem.IsEnabledProperty))
            {
                listBoxItem.SetCurrentValue(IsEnabledProperty, listBoxItemData.IsEnabled);
            }
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _borderThicknessDisposable = TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this));
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _borderThicknessDisposable?.Dispose();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == BorderThicknessProperty ||
            change.Property == IsBorderlessProperty)
        {
            ConfigureEffectiveBorderThickness();
        }
    }

    private void ConfigureEffectiveBorderThickness()
    {
        if (IsBorderless)
        {
            SetCurrentValue(EffectiveBorderThicknessProperty, new Thickness(0));
        }
        else
        {
            SetCurrentValue(EffectiveBorderThicknessProperty, BorderThickness);
        }
    }
    
    protected override void OnKeyDown(KeyEventArgs e)
    {
        var hotkeys = Application.Current!.PlatformSettings?.HotkeyConfiguration;
        var ctrl    = hotkeys is not null && e.KeyModifiers.HasAllFlags(hotkeys.CommandModifiers);

        if (!ctrl &&
            e.Key.ToNavigationDirection() is { } direction && 
            direction.IsDirectional())
        {
            e.Handled |= MoveSelection(
                direction,
                WrapSelection,
                e.KeyModifiers.HasAllFlags(KeyModifiers.Shift));
        }
        else if (SelectionMode.HasAllFlags(SelectionMode.Multiple) &&
                 hotkeys is not null && hotkeys.SelectAll.Any(x => x.Matches(e)))
        {
            Selection.SelectAll();
            e.Handled = true;
        }
        else if (e.Key == Key.Space || e.Key == Key.Enter)
        {
            UpdateSelectionFromEventSource(
                e.Source,
                true,
                e.KeyModifiers.HasFlag(KeyModifiers.Shift),
                ctrl);
        }

        base.OnKeyDown(e);
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        Scroll = e.NameScope.Find<IScrollable>(ListBoxThemeConstants.ScrollViewerPart);
        if (!IsSet(EmptyIndicatorProperty))
        {
            SetValue(EmptyIndicatorProperty, new Empty()
            {
                SizeType    = SizeType.Small,
                PresetImage = PresetEmptyImage.Simple
            }, BindingPriority.Template);
        }
    }

    protected internal bool UpdateSelectionFromPointerEvent(Control source, PointerEventArgs e)
    {
        var hotkeys = Application.Current!.PlatformSettings?.HotkeyConfiguration;
        var toggle  = hotkeys is not null && e.KeyModifiers.HasAllFlags(hotkeys.CommandModifiers);
        return UpdateSelectionFromEventSource(
            source,
            true,
            e.KeyModifiers.HasAllFlags(KeyModifiers.Shift),
            toggle,
            e.GetCurrentPoint(source).Properties.IsRightButtonPressed);
    }
    
}