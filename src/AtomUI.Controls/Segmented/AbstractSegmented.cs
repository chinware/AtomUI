using AtomUI.Animations;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Commons;

public abstract class AbstractSegmented : SelectingItemsControl,
                                          IMotionAwareControl,
                                          ISizeTypeAware,
                                          IFormItemAware
{
    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<AbstractSegmented>();

    public static readonly StyledProperty<bool> IsExpandingProperty =
        AvaloniaProperty.Register<AbstractSegmented, bool>(nameof(IsExpanding));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractSegmented>();

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public bool IsExpanding
    {
        get => GetValue(IsExpandingProperty);
        set => SetValue(IsExpandingProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    #region 内部属性定义

    public static readonly StyledProperty<CornerRadius> SelectedThumbCornerRadiusProperty =
        AvaloniaProperty.Register<AbstractSegmented, CornerRadius>(nameof(SelectedThumbCornerRadius));

    internal static readonly StyledProperty<IBrush?> SelectedThumbBgProperty =
        AvaloniaProperty.Register<AbstractSegmented, IBrush?>(
            nameof(SelectedThumbBg));

    internal static readonly StyledProperty<BoxShadows> SelectedThumbBoxShadowsProperty =
        AvaloniaProperty.Register<AbstractSegmented, BoxShadows>(
            nameof(SelectedThumbBoxShadows));

    internal CornerRadius SelectedThumbCornerRadius
    {
        get => GetValue(SelectedThumbCornerRadiusProperty);
        set => SetValue(SelectedThumbCornerRadiusProperty, value);
    }

    internal IBrush? SelectedThumbBg
    {
        get => GetValue(SelectedThumbBgProperty);
        set => SetValue(SelectedThumbBgProperty, value);
    }

    internal BoxShadows SelectedThumbBoxShadows
    {
        get => GetValue(SelectedThumbBoxShadowsProperty);
        set => SetValue(SelectedThumbBoxShadowsProperty, value);
    }

    // 内部动画属性
    internal static readonly StyledProperty<Size> SelectedThumbSizeProperty =
        AvaloniaProperty.Register<AbstractSegmented, Size>(nameof(SelectedThumbSize));

    internal Size SelectedThumbSize
    {
        get => GetValue(SelectedThumbSizeProperty);
        set => SetValue(SelectedThumbSizeProperty, value);
    }

    internal static readonly StyledProperty<Point> SelectedThumbPosProperty =
        AvaloniaProperty.Register<AbstractSegmented, Point>(nameof(SelectedThumbPos));

    internal Point SelectedThumbPos
    {
        get => GetValue(SelectedThumbPosProperty);
        set => SetValue(SelectedThumbPosProperty, value);
    }
    
    #endregion

    static AbstractSegmented()
    {
        AffectsMeasure<AbstractSegmented>(IsExpandingProperty, SizeTypeProperty);
        AffectsRender<AbstractSegmented>(
            SelectedThumbCornerRadiusProperty, 
            SelectedThumbBgProperty,
            SelectedThumbBoxShadowsProperty,
            SelectedThumbSizeProperty, 
            SelectedThumbPosProperty);
        AutoScrollToSelectedItemProperty.OverrideDefaultValue<AbstractSegmented>(false);
        SelectedItemProperty.Changed.AddClassHandler<AbstractSegmented>((segmented, args) => segmented.NotifyFormValueChanged(args.NewValue));
    }

    public AbstractSegmented()
    {
        SelectionMode = SelectionMode.Single;
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var hasDefaultSelected = false;
        foreach (var item in Items)
        {
            if (item is not null)
            {
                var container = ContainerFromItem(item);
                if (container is not null)
                {
                    if (GetIsSelected(container))
                    {
                        hasDefaultSelected = true;
                    }
                }
            }
        }
        
        if (!hasDefaultSelected)
        {
            SelectedIndex = 0;
        }
        
        SetupSelectedThumbRect();
    }
    
    private void HandleSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (this.IsAttachedToVisualTree())
        {
            SetupSelectedThumbRect();
        }
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        SetupSelectedThumbRect();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SelectionChanged += HandleSelectionChanged;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        SelectionChanged -= HandleSelectionChanged;
    }

    private void SetupSelectedThumbRect()
    {
        if (SelectedItem is not null)
        {
            var segmentedItem = ContainerFromItem(SelectedItem);
            if (segmentedItem is not null)
            {
                var offset    = segmentedItem.TranslatePoint(new Point(0, 0), this) ?? default;
                var offsetX   = offset.X;
                var targetPos = new Point(offsetX, offset.Y);
                SelectedThumbPos  = targetPos;
                SelectedThumbSize = segmentedItem.DesiredSize;
            }
        }
    }
    
    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<AbstractSegmentedItem>(item, out recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is AbstractSegmentedItem segmentedItem)
        {
            
            if (item != null && item is not Visual)
            {
                segmentedItem.SetCurrentValue(AbstractSegmentedItem.ContentProperty, item);
                if (ItemTemplate != null)
                {
                    segmentedItem[!AbstractSegmentedItem.ContentTemplateProperty] = this[!ItemTemplateProperty];
                }
            }
            
            segmentedItem[!SizeTypeProperty]        = this[!SizeTypeProperty];
            segmentedItem[!IsMotionEnabledProperty] = this[!IsMotionEnabledProperty];

            if (segmentedItem.IsSelected)
            {
                SetCurrentValue(SelectedItemProperty, ItemFromContainer(segmentedItem));
            }
            
            PrepareSegmentedItem(segmentedItem, item, index);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(container), "The container type is incorrect, it must be type AbstractSegmentedItem.");
        }
    }
    
    protected virtual void PrepareSegmentedItem(AbstractSegmentedItem segmentedItem, object? item, int index)
    {
    }

    internal bool UpdateSelectionFromPointerEvent(Control source, PointerEventArgs e)
    {
        return UpdateSelectionFromEvent(source, e);
    }

    protected override bool ShouldTriggerSelection(Visual selectable, PointerEventArgs eventArgs)
    {
        // Segmented should trigger selection on pointer release for better UX
        if (eventArgs.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased &&
            eventArgs.Pointer.Type == PointerType.Mouse)
        {
            return true;
        }

        return base.ShouldTriggerSelection(selectable, eventArgs);
    }
    
    public sealed override void Render(DrawingContext context)
    {
        context.DrawRectangle(Background, null, new RoundedRect(new Rect(DesiredSize.Deflate(Margin)), CornerRadius));
        context.DrawRectangle(SelectedThumbBg, null,
            new RoundedRect(new Rect(SelectedThumbPos, SelectedThumbSize), SelectedThumbCornerRadius),
            SelectedThumbBoxShadows);
    }

    #region 实现 FormItem 接口
    
    private EventHandler? _formValueChanged;
    event EventHandler? IFormItemAware.ValueChanged
    {
        add => _formValueChanged += value;
        remove => _formValueChanged -= value;
    }

    void IFormItemAware.SetFormValue(object? value) => NotifySetFormValue(value as bool?);

    object? IFormItemAware.GetFormValue() => NotifyGetFormValue();
    void IFormItemAware.ClearFormValue() => NotifyClearFormValue();
    void IFormItemAware.NotifyValidateStatus(FormValidateStatus status) => NotifyValidateStatus(status);
    
    protected virtual void NotifyFormValueChanged(object? value)
    {
        _formValueChanged?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void NotifySetFormValue(object? value)
    {
        SelectedItem = value;
    }

    protected virtual object? NotifyGetFormValue()
    {
        return SelectedItem;
    }

    protected virtual void NotifyClearFormValue()
    {
        SelectedItem = null;
    }

    protected virtual void NotifyValidateStatus(FormValidateStatus status)
    {
    }
    #endregion

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