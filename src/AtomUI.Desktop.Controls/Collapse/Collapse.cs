using AtomUI.Controls;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
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

    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new StackPanel
        {
            Orientation = Orientation.Vertical
        });

    #endregion

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
            PrepareCollapseItem(collapseItem, item, index);
            ConfigureItemPaddings(collapseItem);
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
                if (!collapseItem.InAnimating)
                {
                    e.Handled = UpdateSelectionFromEvent(collapseItem, e);
                }
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
                if (!collapseItem.InAnimating && collapseItem.IsPointInHeaderBounds(e.GetPosition(collapseItem)))
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
                    if (!collapseItem.InAnimating && collapseItem.IsPointInHeaderBounds(e.GetPosition(collapseItem)))
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
    }

    private void HandleSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        SetupItemsBorderThickness();
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
        var headerBorderBottom  = BorderThickness.Bottom;
        var contentBorderBottom = BorderThickness.Bottom;
        if (!IsGhostStyle)
        {
            if (!IsBorderless)
            {
                if (index == ItemCount - 1 && !collapseItem.IsSelected)
                {
                    headerBorderBottom = 0d;
                }
            }
            else
            {
                if (collapseItem.IsSelected || (index == ItemCount - 1 && !collapseItem.IsSelected))
                {
                    headerBorderBottom = 0d;
                }
            }

            if (index == ItemCount - 1 &&
                (collapseItem.IsSelected || (!collapseItem.IsSelected && collapseItem.InAnimating)))
            {
                contentBorderBottom = 0d;
            }
        }
        else
        {
            headerBorderBottom  = 0d;
            contentBorderBottom = 0d;
        }

        collapseItem.HeaderBorderThickness  = new Thickness(0, 0, 0, headerBorderBottom);
        collapseItem.ContentBorderThickness = new Thickness(0, 0, 0, contentBorderBottom);
    }

    private CollapseItem? GetCollapseItemAt(int index)
    {
        return ContainerFromIndex(index) as CollapseItem;
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
        if (IsAccordion)
        {
            SelectionMode = SelectionMode.Single | SelectionMode.Toggle;
        }
        else
        {
            SelectionMode = SelectionMode.Multiple | SelectionMode.Toggle;
        }
    }
}
