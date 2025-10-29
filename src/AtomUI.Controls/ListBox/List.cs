// Modified based on https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Controls/List.cs
 
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

public class List : SelectingItemsControl,
                    IMotionAwareControl,
                    IControlSharedTokenResourcesHost
{
    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new VirtualizingStackPanel());
    
    #region 公共属性定义
    
    public static readonly DirectProperty<List, IScrollable?> ScrollProperty =
        AvaloniaProperty.RegisterDirect<List, IScrollable?>(nameof(Scroll), o => o.Scroll);
    
    public static readonly StyledProperty<object?> EmptyIndicatorProperty =
        AvaloniaProperty.Register<List, object?>(nameof(EmptyIndicator));
    
    public static readonly StyledProperty<IDataTemplate?> EmptyIndicatorTemplateProperty =
        AvaloniaProperty.Register<List, IDataTemplate?>(nameof(EmptyIndicatorTemplate));
    
    public static readonly StyledProperty<bool> IsShowEmptyIndicatorProperty =
        AvaloniaProperty.Register<List, bool>(nameof(IsShowEmptyIndicator), false);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("AvaloniaProperty", "AVP1010",
        Justification = "This property is owned by SelectingItemsControl, but protected there. List changes its visibility.")]
    public new static readonly DirectProperty<SelectingItemsControl, IList?> SelectedItemsProperty =
        SelectingItemsControl.SelectedItemsProperty;
    
    [System.Diagnostics.CodeAnalysis.SuppressMessage("AvaloniaProperty", "AVP1010",
        Justification = "This property is owned by SelectingItemsControl, but protected there. List changes its visibility.")]
    public new static readonly DirectProperty<SelectingItemsControl, ISelectionModel> SelectionProperty =
        SelectingItemsControl.SelectionProperty;
    
    [System.Diagnostics.CodeAnalysis.SuppressMessage("AvaloniaProperty", "AVP1010",
        Justification = "This property is owned by SelectingItemsControl, but protected there. List changes its visibility.")]
    public new static readonly StyledProperty<SelectionMode> SelectionModeProperty =
        SelectingItemsControl.SelectionModeProperty;
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<List>();

    public static readonly StyledProperty<bool> DisabledItemHoverEffectProperty =
        AvaloniaProperty.Register<List, bool>(nameof(DisabledItemHoverEffect));
    
    public static readonly StyledProperty<bool> IsBorderlessProperty =
        AvaloniaProperty.Register<List, bool>(nameof(IsBorderless), false);
    
    public static readonly StyledProperty<bool> IsShowSelectedIndicatorProperty =
        AvaloniaProperty.Register<List, bool>(nameof(IsShowSelectedIndicator), false);
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<List>();
    
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
        Justification = "This property is owned by SelectingItemsControl, but protected there. List changes its visibility.")]
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
    
    internal static readonly DirectProperty<List, Thickness> EffectiveBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<List, Thickness>(nameof(EffectiveBorderThickness),
            o => o.EffectiveBorderThickness,
            (o, v) => o.EffectiveBorderThickness = v);
    
    private Thickness _effectiveBorderThickness;

    internal Thickness EffectiveBorderThickness
    {
        get => _effectiveBorderThickness;
        set => SetAndRaise(EffectiveBorderThicknessProperty, ref _effectiveBorderThickness, value);
    }

    protected override Type StyleKeyOverride { get; } = typeof(List);
    
    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ListToken.ID;

    #endregion
    
    private protected readonly Dictionary<object, CompositeDisposable> _itemsBindingDisposables = new();
    private IDisposable? _borderThicknessDisposable;

    static List()
    {
        ItemsPanelProperty.OverrideDefaultValue<List>(DefaultPanel);
        KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue(
            typeof(List),
            KeyboardNavigationMode.Once);
    }
    
    public List()
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
        return new ListItem();
    }
    
    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        var disposables = new CompositeDisposable(4);
        if (container is ListItem listBoxItem)
        {
            if (item != null && item is not Visual)
            {
                ApplyListItemData(listBoxItem, item);
            }
            
            if (ItemTemplate != null)
            {
                disposables.Add(BindUtils.RelayBind(this, ItemTemplateProperty, listBoxItem, ListItem.ContentTemplateProperty));
            }
            
            disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, listBoxItem, ListItem.IsMotionEnabledProperty));
            disposables.Add(BindUtils.RelayBind(this, SizeTypeProperty, listBoxItem, ListItem.SizeTypeProperty));
            disposables.Add(BindUtils.RelayBind(this, IsShowSelectedIndicatorProperty, listBoxItem, ListItem.IsShowSelectedIndicatorProperty));
            disposables.Add(BindUtils.RelayBind(this, DisabledItemHoverEffectProperty, listBoxItem,
                ListItem.DisabledItemHoverEffectProperty));
            
            PrepareListBoxItem(listBoxItem, item, index, disposables);
            DisposableListItem(listBoxItem);
            _itemsBindingDisposables.Add(listBoxItem, disposables);
        }
    }
    
    protected virtual void PrepareListBoxItem(ListItem listItem, object? item, int index, CompositeDisposable compositeDisposable)
    {
    }

    protected virtual void ApplyListItemData(ListItem listItem, object item)
    {
        if (!listItem.IsSet(ListItem.ContentProperty))
        {
            listItem.SetCurrentValue(ListItem.ContentProperty, item);
        }

        if (item is IListBoxItemData listBoxItemData)
        {
            if (!listItem.IsSet(ListItem.IsSelectedProperty))
            {
                listItem.SetCurrentValue(ListItem.IsSelectedProperty, listBoxItemData.IsSelected);
            }
            if (!listItem.IsSet(ListItem.IsEnabledProperty))
            {
                listItem.SetCurrentValue(IsEnabledProperty, listBoxItemData.IsEnabled);
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
        Scroll = e.NameScope.Find<IScrollable>(ListThemeConstants.ScrollViewerPart);
        if (!IsSet(EmptyIndicatorProperty))
        {
            SetValue(EmptyIndicatorProperty, new Empty()
            {
                SizeType    = SizeType.Small,
                PresetImage = PresetEmptyImage.Simple
            }, BindingPriority.Template);
        }
    }

    protected internal virtual bool UpdateSelectionFromPointerEvent(Control source, PointerEventArgs e)
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