using AtomUI.Controls.Utils;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Transformation;

namespace AtomUI.Controls;

public class TabStrip : BaseTabStrip
{
    #region 内部属性定义

    internal static readonly DirectProperty<TabStrip, double> SelectedIndicatorThicknessProperty =
        AvaloniaProperty.RegisterDirect<TabStrip, double>(nameof(SelectedIndicatorThickness),
            o => o.SelectedIndicatorThickness,
            (o, v) => o.SelectedIndicatorThickness = v);

    private double _selectedIndicatorThickness;

    internal double SelectedIndicatorThickness
    {
        get => _selectedIndicatorThickness;
        set => SetAndRaise(SelectedIndicatorThicknessProperty, ref _selectedIndicatorThickness, value);
    }

    #endregion

    private Border? _selectedIndicator;
    private ItemsPresenter? _itemsPresenter;

    public TabStrip()
    {
        SelectionChanged += HandleSelectionChanged;
        LayoutUpdated    += HandleLayoutUpdated;
    }

    private void HandleSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (VisualRoot is not null)
        {
            SetupSelectedIndicator();
        }
    }

    private void HandleLayoutUpdated(object? sender, EventArgs args)
    {
        if (_selectedIndicator?.Transitions is not null)
        {
            _selectedIndicator.Transitions = new Transitions()
            {
                AnimationUtils.CreateTransition<TransformOperationsTransition>(RenderTransformProperty,
                    SharedTokenKey.MotionDurationSlow, new ExponentialEaseOut())
            };
            // 只需要执行一次
            LayoutUpdated -= HandleLayoutUpdated;
        }
    }

    private void SetupSelectedIndicator()
    {
        if (_selectedIndicator is not null && SelectedItem is TabStripItem tabStripItem)
        {
            var selectedBounds = tabStripItem.Bounds;
            var builder        = new TransformOperations.Builder(1);
            var offset         = _itemsPresenter?.Bounds.Position ?? default;

            if (TabStripPlacement == Dock.Top)
            {
                _selectedIndicator.SetValue(WidthProperty, tabStripItem.DesiredSize.Width);
                _selectedIndicator.SetValue(HeightProperty, _selectedIndicatorThickness);
                builder.AppendTranslate(offset.X + selectedBounds.Left, 0);
            }
            else if (TabStripPlacement == Dock.Right)
            {
                _selectedIndicator.SetValue(HeightProperty, tabStripItem.DesiredSize.Height);
                _selectedIndicator.SetValue(WidthProperty, _selectedIndicatorThickness);
                builder.AppendTranslate(0, offset.Y + selectedBounds.Y);
            }
            else if (TabStripPlacement == Dock.Bottom)
            {
                _selectedIndicator.SetValue(WidthProperty, tabStripItem.DesiredSize.Width);
                _selectedIndicator.SetValue(HeightProperty, _selectedIndicatorThickness);
                builder.AppendTranslate(offset.X + selectedBounds.Left, 0);
            }
            else
            {
                _selectedIndicator.SetValue(HeightProperty, tabStripItem.DesiredSize.Height);
                _selectedIndicator.SetValue(WidthProperty, _selectedIndicatorThickness);
                builder.AppendTranslate(0, offset.Y + selectedBounds.Y);
            }

            _selectedIndicator.RenderTransform = builder.Build();
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        if (SelectedItem is TabStripItem)
        {
            SetupSelectedIndicator();
        }

        return size;
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        var tabStripItem = new TabStripItem
        {
            Shape = TabSharp.Line
        };
        return tabStripItem;
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is TabStripItem tabStripItem)
        {
            tabStripItem.Shape = TabSharp.Line;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _selectedIndicator = e.NameScope.Find<Border>(TabStripTheme.SelectedItemIndicatorPart);
        _itemsPresenter    = e.NameScope.Find<ItemsPresenter>(BaseTabStripTheme.ItemsPresenterPart);

        TokenResourceBinder.CreateSharedResourceBinding(this, SelectedIndicatorThicknessProperty,
            SharedTokenKey.LineWidthBold);
    }
}