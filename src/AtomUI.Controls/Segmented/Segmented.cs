using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Media;

namespace AtomUI.Controls;

[TemplatePart(CollapseTheme.ItemsPresenterPart, typeof(ItemsPresenter))]
public class Segmented : SelectingItemsControl
{
    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        AvaloniaProperty.Register<Segmented, SizeType>(nameof(SizeType), SizeType.Middle);

    public static readonly StyledProperty<bool> IsExpandingProperty =
        AvaloniaProperty.Register<Segmented, bool>(nameof(IsExpanding));

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

    #endregion

    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new SegmentedStackPanel());

    static Segmented()
    {
        AffectsMeasure<Segmented>(IsExpandingProperty, SizeTypeProperty);
        AffectsRender<Segmented>(SelectedThumbCornerRadiusProperty, SelectedThumbBgProperty,
            SelectedThumbBoxShadowsProperty,
            SelectedThumbSizeProperty, SelectedThumbPosProperty);
        ItemsPanelProperty.OverrideDefaultValue<Segmented>(DefaultPanel);
    }

    public Segmented()
    {
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
                var offsetY   = (DesiredSize.Height - segmentedItem.DesiredSize.Height) / 2;
                var targetPos = new Point(offsetX, offsetY);
                SelectedThumbPos  = targetPos;
                SelectedThumbSize = segmentedItem.Bounds.Size;
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
        if (container is SegmentedItem segmentedItemX)
        {
            BindUtils.RelayBind(this, SizeTypeProperty, segmentedItemX, SegmentedItem.SizeTypeProperty);
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Presenter?.Panel is SegmentedStackPanel segmentedStackPanel)
        {
            segmentedStackPanel.IsExpanding = IsExpanding;
        }

        base.ArrangeOverride(finalSize);
        if (Transitions is null)
        {
            SetupSelectedThumbRect();
            Transitions = new Transitions
            {
                AnimationUtils.CreateTransition<PointTransition>(SelectedThumbPosProperty),
                AnimationUtils.CreateTransition<SizeTransition>(SelectedThumbSizeProperty,
                    DesignTokenKey.MotionDurationFast)
            };
        }

        return finalSize;
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
        context.DrawRectangle(Background, null, new RoundedRect(new Rect(new Point(0, 0), Bounds.Size), CornerRadius));
        context.DrawRectangle(SelectedThumbBg, null,
            new RoundedRect(new Rect(SelectedThumbPos, SelectedThumbSize), SelectedThumbCornerRadius),
            SelectedThumbBoxShadows);
    }
}