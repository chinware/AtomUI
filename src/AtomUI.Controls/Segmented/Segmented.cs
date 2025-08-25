using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public class Segmented : SelectingItemsControl,
                         IMotionAwareControl,
                         IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<Segmented>();

    public static readonly StyledProperty<bool> IsExpandingProperty =
        AvaloniaProperty.Register<Segmented, bool>(nameof(IsExpanding));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Segmented>();

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
        AvaloniaProperty.Register<Segmented, CornerRadius>(nameof(SelectedThumbCornerRadius));

    internal static readonly StyledProperty<IBrush?> SelectedThumbBgProperty =
        AvaloniaProperty.Register<Segmented, IBrush?>(
            nameof(SelectedThumbBg));

    internal static readonly StyledProperty<BoxShadows> SelectedThumbBoxShadowsProperty =
        AvaloniaProperty.Register<Segmented, BoxShadows>(
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
        AvaloniaProperty.Register<Segmented, Size>(nameof(SelectedThumbSize));

    internal Size SelectedThumbSize
    {
        get => GetValue(SelectedThumbSizeProperty);
        set => SetValue(SelectedThumbSizeProperty, value);
    }

    internal static readonly StyledProperty<Point> SelectedThumbPosProperty =
        AvaloniaProperty.Register<Segmented, Point>(nameof(SelectedThumbPos));

    internal Point SelectedThumbPos
    {
        get => GetValue(SelectedThumbPosProperty);
        set => SetValue(SelectedThumbPosProperty, value);
    }
    
    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => SegmentedToken.ID;
    
    #endregion

    static Segmented()
    {
        AffectsMeasure<Segmented>(IsExpandingProperty, SizeTypeProperty);
        AffectsRender<Segmented>(
            SelectedThumbCornerRadiusProperty, 
            SelectedThumbBgProperty,
            SelectedThumbBoxShadowsProperty,
            SelectedThumbSizeProperty, 
            SelectedThumbPosProperty);
        AutoScrollToSelectedItemProperty.OverrideDefaultValue<Segmented>(false);
    }

    public Segmented()
    {
        this.RegisterResources();
        SelectionChanged += HandleSelectionChanged;
        SelectionMode    =  SelectionMode.Single;
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

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new SegmentedItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<SegmentedItem>(item, out recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is SegmentedItem segmentedItem)
        {
            BindUtils.RelayBind(this, SizeTypeProperty, segmentedItem, SegmentedItem.SizeTypeProperty);
            BindUtils.RelayBind(this, IsMotionEnabledProperty, segmentedItem, SegmentedItem.IsMotionEnabledProperty);
        }
    }

    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions =
                [
                    TransitionUtils.CreateTransition<PointTransition>(SelectedThumbPosProperty),
                    TransitionUtils.CreateTransition<SizeTransition>(SelectedThumbSizeProperty)
                ];
            }
        }
        else
        {
            Transitions = null;
        }
    }

    internal bool UpdateSelectionFromPointerEvent(Control source, PointerEventArgs e)
    {
        return UpdateSelectionFromEventSource(
            source,
            true,
            false,
            false,
            e.GetCurrentPoint(source).Properties.IsRightButtonPressed);
    }

    public sealed override void Render(DrawingContext context)
    {
        context.DrawRectangle(Background, null, new RoundedRect(new Rect(DesiredSize.Deflate(Margin)), CornerRadius));
        context.DrawRectangle(SelectedThumbBg, null,
            new RoundedRect(new Rect(SelectedThumbPos, SelectedThumbSize), SelectedThumbCornerRadius),
            SelectedThumbBoxShadows);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions(false);
            }
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ConfigureTransitions(false);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        Transitions = null;
    }
}