using AtomUI.Controls;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

public enum CollapseTriggerType
{
    Header,
    Icon
}

public enum CollapseExpandIconPosition
{
    Start,
    End
}

[TemplatePart("PART_ItemsPresenter", typeof(ItemsPresenter))]
public class Collapse : SelectingItemsControl, IMotionAwareControl
{
    #region 公共属性定义

    public static readonly StyledProperty<CustomizableSizeType> SizeTypeProperty =
        CustomizableSizeTypeControlProperty.SizeTypeProperty.AddOwner<Collapse>();

    public static readonly StyledProperty<bool> IsGhostStyleProperty =
        AvaloniaProperty.Register<Collapse, bool>(nameof(IsGhostStyle));

    public static readonly StyledProperty<bool> IsBorderlessProperty =
        AvaloniaProperty.Register<Collapse, bool>(nameof(IsBorderless));

    public static readonly StyledProperty<bool> IsAccordionProperty =
        AvaloniaProperty.Register<Collapse, bool>(nameof(IsAccordion));

    public static readonly StyledProperty<CollapseTriggerType> TriggerTypeProperty =
        AvaloniaProperty.Register<Collapse, CollapseTriggerType>(nameof(TriggerType));

    public static readonly StyledProperty<CollapseExpandIconPosition> ExpandIconPositionProperty =
        AvaloniaProperty.Register<Collapse, CollapseExpandIconPosition>(nameof(ExpandIconPosition));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Collapse>();
    
    public static readonly StyledProperty<Thickness> ItemHeaderPaddingProperty =
        AvaloniaProperty.Register<Collapse, Thickness>(nameof(ItemHeaderPadding));

    public static readonly StyledProperty<Thickness> ItemContentPaddingProperty =
        AvaloniaProperty.Register<Collapse, Thickness>(nameof(ItemContentPadding));

    public CustomizableSizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public bool IsGhostStyle
    {
        get => GetValue(IsGhostStyleProperty);
        set => SetValue(IsGhostStyleProperty, value);
    }

    public bool IsBorderless
    {
        get => GetValue(IsBorderlessProperty);
        set => SetValue(IsBorderlessProperty, value);
    }

    public bool IsAccordion
    {
        get => GetValue(IsAccordionProperty);
        set => SetValue(IsAccordionProperty, value);
    }

    public CollapseTriggerType TriggerType
    {
        get => GetValue(TriggerTypeProperty);
        set => SetValue(TriggerTypeProperty, value);
    }

    public CollapseExpandIconPosition ExpandIconPosition
    {
        get => GetValue(ExpandIconPositionProperty);
        set => SetValue(ExpandIconPositionProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public Thickness ItemHeaderPadding
    {
        get => GetValue(ItemHeaderPaddingProperty);
        set => SetValue(ItemHeaderPaddingProperty, value);
    }

    public Thickness ItemContentPadding
    {
        get => GetValue(ItemContentPaddingProperty);
        set => SetValue(ItemContentPaddingProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<Collapse, Thickness> EffectiveBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<Collapse, Thickness>(nameof(EffectiveBorderThickness),
            o => o.EffectiveBorderThickness,
            (o, v) => o.EffectiveBorderThickness = v);

    private Thickness _effectiveBorderThickness;

    internal Thickness EffectiveBorderThickness
    {
        get => _effectiveBorderThickness;
        set => SetAndRaise(EffectiveBorderThicknessProperty, ref _effectiveBorderThickness, value);
    }

    #endregion

    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new StackPanel
        {
            Orientation = Orientation.Vertical
        });

    private CollapseItem? _accordionInitialActiveItem;

    static Collapse()
    {
        SelectionModeProperty.OverrideDefaultValue<Collapse>(SelectionMode.Multiple | SelectionMode.Toggle);
        ItemsPanelProperty.OverrideDefaultValue<Collapse>(DefaultPanel);
        AutoScrollToSelectedItemProperty.OverrideDefaultValue<Collapse>(false);
        AffectsRender<Collapse>(SelectionModeProperty);
    }

    public Collapse()
    {
        SelectionChanged += HandleSelectionChanged;
        this.RegisterTokenResourceScope(CollapseToken.ScopeProvider);
    }
    
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new CollapseItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<CollapseItem>(item, out recycleKey);
    }

    protected sealed override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        if (container is CollapseItem collapseItem)
        {
            TrackAccordionInitialActiveItem(collapseItem);
            if (item != null && item is not Visual)
            {
                if (!collapseItem.IsSet(CollapseItem.ContentProperty))
                {
                    collapseItem.SetCurrentValue(CollapseItem.ContentProperty, item);
                }

                if (item is ICollapseItemData itemData)
                {
                    if (!collapseItem.IsSet(CollapseItem.HeaderProperty))
                    {
                        collapseItem.SetCurrentValue(CollapseItem.HeaderProperty, itemData.Header);
                    }
                    if (!collapseItem.IsSet(CollapseItem.IsSelectedProperty))
                    {
                        collapseItem.SetCurrentValue(CollapseItem.IsSelectedProperty, itemData.IsSelected);
                        TrackAccordionInitialActiveItem(collapseItem);
                    }
                    if (!collapseItem.IsSet(CollapseItem.IsShowExpandIconProperty))
                    {
                        collapseItem.SetCurrentValue(CollapseItem.IsShowExpandIconProperty, itemData.IsShowExpandIcon);
                    }
                }
            }

            if (ItemTemplate != null)
            {
                collapseItem[!CollapseItem.ContentTemplateProperty] = this[!ItemTemplateProperty];
            }
            
            collapseItem[!CollapseItem.SizeTypeProperty]           = this[!SizeTypeProperty];
            collapseItem[!CollapseItem.BorderThicknessProperty]    = this[!EffectiveBorderThicknessProperty];
            collapseItem[!CollapseItem.IsGhostStyleProperty]       = this[!IsGhostStyleProperty];
            collapseItem[!CollapseItem.IsBorderlessProperty]       = this[!IsBorderlessProperty];
            collapseItem[!CollapseItem.TriggerTypeProperty]        = this[!TriggerTypeProperty];
            collapseItem[!CollapseItem.ExpandIconPositionProperty] = this[!ExpandIconPositionProperty];
            collapseItem[!CollapseItem.IsMotionEnabledProperty]    = this[!IsMotionEnabledProperty];
            collapseItem.MotionStateChanged -= HandleItemMotionStateChanged;
            collapseItem.MotionStateChanged += HandleItemMotionStateChanged;
            PrepareCollapseItem(collapseItem, item, index);
            ConfigureItemPaddings(collapseItem);
            NormalizeAccordionSelection();
            SetupCollapseBorderThickness(collapseItem, index);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(container), "The container type is incorrect, it must be type CollapseItem.");
        }
    }

    protected virtual void PrepareCollapseItem(CollapseItem collapseItem, object? item, int index)
    {
    }

    protected override void ClearContainerForItemOverride(Control container)
    {
        if (container is CollapseItem collapseItem)
        {
            collapseItem.MotionStateChanged -= HandleItemMotionStateChanged;
        }

        base.ClearContainerForItemOverride(container);
    }

    protected override void ContainerIndexChangedOverride(Control container, int oldIndex, int newIndex)
    {
        if (container is CollapseItem collapseItem)
        {
            SetupCollapseBorderThickness(collapseItem, newIndex);
        }
    }

    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);

        if (e.NavigationMethod == NavigationMethod.Directional)
        {
            var containerFromEventSource = GetContainerFromEventSource(e.Source);
            if (containerFromEventSource is CollapseItem collapseItem)
            {
                e.Handled = UpdateSelectionFromEvent(collapseItem, e);
            }
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && e.Pointer.Type == PointerType.Mouse)
        {
            var containerFromEventSource = GetContainerFromEventSource(e.Source);
            if (containerFromEventSource is CollapseItem collapseItem)
            {
                if (collapseItem.IsPointInHeaderBounds(e.GetPosition(collapseItem)))
                {
                    e.Handled = UpdateSelectionFromEvent(collapseItem, e);
                }
            }
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == MouseButton.Left && e.Pointer.Type != PointerType.Mouse)
        {
            var container = GetContainerFromEventSource(e.Source);
            if (container != null
                && container.ContainsSelfOrDescendantAt(e.GetPosition(container)))
            {
                var containerFromEventSource = GetContainerFromEventSource(e.Source);
                if (containerFromEventSource is CollapseItem collapseItem)
                {
                    if (collapseItem.IsPointInHeaderBounds(e.GetPosition(collapseItem)))
                    {
                        e.Handled = UpdateSelectionFromEvent(collapseItem, e);
                    }
                }
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsAccordionProperty)
        {
            SetupSelectionMode();
            NormalizeAccordionSelection();
        }

        if (change.Property == BorderThicknessProperty ||
            change.Property == IsBorderlessProperty ||
            change.Property == IsGhostStyleProperty)
        {
            SetupEffectiveBorderThickness();
            SetupItemsBorderThickness();
        }

        if (change.Property == ItemHeaderPaddingProperty ||
            change.Property == ItemContentPaddingProperty)
        {
            ConfigureItemsPaddings();
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SetupEffectiveBorderThickness();
        NormalizeAccordionSelection();
        _accordionInitialActiveItem = null;
    }

    public override bool UpdateSelectionFromEvent(Control container, RoutedEventArgs eventArgs)
    {
        if (IsAccordion && container is CollapseItem collapseItem)
        {
            var index = IndexFromContainer(collapseItem);
            if (index < 0)
            {
                return false;
            }

            ApplyAccordionSelection(collapseItem.IsSelected ? null : collapseItem);
            return true;
        }

        return base.UpdateSelectionFromEvent(container, eventArgs);
    }

    private void HandleSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        NormalizeAccordionSelection(args);
        SetupItemsBorderThickness();
    }

    private void HandleItemMotionStateChanged(object? sender, EventArgs args)
    {
        if (sender is CollapseItem collapseItem)
        {
            var index = IndexFromContainer(collapseItem);
            if (index >= 0)
            {
                SetupCollapseBorderThickness(collapseItem, index);
            }
        }
    }

    private void SetupItemsBorderThickness()
    {
        if (this.IsAttachedToVisualTree())
        {
            for (var i = 0; i < ItemCount; ++i)
            {
                if (GetCollapseItemAt(i) is { } collapseItem)
                {
                    SetupCollapseBorderThickness(collapseItem, i);
                }
            }
        }
    }

    private void SetupCollapseBorderThickness(CollapseItem collapseItem, int index)
    {
        var headerBorderBottom  = 0d;
        var contentBorderTop    = 0d;
        var contentBorderBottom = 0d;

        if (!IsGhostStyle)
        {
            var borderLine       = BorderThickness.Bottom;
            var isLastItem       = index == ItemCount - 1;
            var isContentVisible = collapseItem.IsSelected || collapseItem.InAnimating;

            if (isContentVisible)
            {
                if (!IsBorderless)
                {
                    contentBorderTop = borderLine;
                }

                if (!isLastItem)
                {
                    contentBorderBottom = borderLine;
                }
            }
            else if (!isLastItem)
            {
                headerBorderBottom = borderLine;
            }
        }

        collapseItem.HeaderBorderThickness  = new Thickness(0, 0, 0, headerBorderBottom);
        collapseItem.ContentBorderThickness = new Thickness(0, contentBorderTop, 0, contentBorderBottom);
    }

    private CollapseItem? GetCollapseItemAt(int index)
    {
        return ContainerFromIndex(index) as CollapseItem;
    }

    private void NormalizeAccordionSelection(SelectionChangedEventArgs? args = null)
    {
        if (!IsAccordion || !this.IsAttachedToVisualTree())
        {
            return;
        }

        var activeItem = FindAccordionActiveItemFromInitialState(args) ??
                         FindAccordionActiveItemFromSelectionChange(args) ??
                         FindFirstSelectedCollapseItem();
        ApplyAccordionSelection(activeItem);
    }

    private void TrackAccordionInitialActiveItem(CollapseItem collapseItem)
    {
        if (IsAccordion &&
            _accordionInitialActiveItem is null &&
            collapseItem.IsSelected)
        {
            _accordionInitialActiveItem = collapseItem;
        }
    }

    private CollapseItem? FindAccordionActiveItemFromInitialState(SelectionChangedEventArgs? args)
    {
        if (args is not null ||
            _accordionInitialActiveItem is not { } initialActiveItem)
        {
            return null;
        }

        if (IndexFromContainer(initialActiveItem) < 0)
        {
            return null;
        }

        return initialActiveItem;
    }

    private CollapseItem? FindAccordionActiveItemFromSelectionChange(SelectionChangedEventArgs? args)
    {
        if (args is null)
        {
            return null;
        }

        foreach (var item in args.AddedItems)
        {
            if (FindCollapseItemFromSelectionItem(item) is { } collapseItem && collapseItem.IsSelected)
            {
                return collapseItem;
            }
        }

        return null;
    }

    private CollapseItem? FindCollapseItemFromSelectionItem(object? item)
    {
        if (item is CollapseItem collapseItem && IndexFromContainer(collapseItem) >= 0)
        {
            return collapseItem;
        }

        return item is not null ? ContainerFromItem(item) as CollapseItem : null;
    }

    private CollapseItem? FindFirstSelectedCollapseItem()
    {
        for (var i = 0; i < ItemCount; i++)
        {
            if (GetCollapseItemAt(i) is { IsSelected: true } collapseItem)
            {
                return collapseItem;
            }
        }

        return null;
    }

    private void ApplyAccordionSelection(CollapseItem? activeItem)
    {
        for (var i = 0; i < ItemCount; i++)
        {
            if (GetCollapseItemAt(i) is { } collapseItem)
            {
                var shouldSelect = ReferenceEquals(collapseItem, activeItem);
                if (collapseItem.IsSelected != shouldSelect)
                {
                    collapseItem.SetCurrentValue(CollapseItem.IsSelectedProperty, shouldSelect);
                }
            }
        }
    }

    private void ConfigureItemsPaddings()
    {
        if (Items.Count > 0)
        {
            for (var i = 0; i < ItemCount; i++)
            {
                if (GetCollapseItemAt(i) is { } collapseItem)
                {
                    ConfigureItemPaddings(collapseItem);
                }
            }
        }
    }

    private void ConfigureItemPaddings(CollapseItem collapseItem)
    {
        collapseItem.OwnerHeaderPadding = IsSet(ItemHeaderPaddingProperty)
            ? ItemHeaderPadding
            : null;
        collapseItem.OwnerContentPadding = IsSet(ItemContentPaddingProperty)
            ? ItemContentPadding
            : null;
    }

    private void SetupEffectiveBorderThickness()
    {
        if (IsBorderless || IsGhostStyle)
        {
            EffectiveBorderThickness = default;
        }
        else
        {
            EffectiveBorderThickness = BorderThickness;
        }
    }

    private void SetupSelectionMode()
    {
        SelectionMode = SelectionMode.Multiple | SelectionMode.Toggle;
    }
}
