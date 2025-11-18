using AtomUI.Controls;
using AtomUI.Desktop.Controls.Themes;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

public class TabStrip : BaseTabStrip
{
    #region 内部属性定义

    internal static readonly StyledProperty<double> SelectedIndicatorThicknessProperty =
        AvaloniaProperty.Register<TabStrip, double>(nameof(SelectedIndicatorThickness));
    
    internal static readonly StyledProperty<ITransform?> SelectedIndicatorRenderTransformProperty = 
        AvaloniaProperty.Register<TabStrip, ITransform?>(nameof (SelectedIndicatorRenderTransform));
    
    internal double SelectedIndicatorThickness
    {
        get => GetValue(SelectedIndicatorThicknessProperty);
        set => SetValue(SelectedIndicatorThicknessProperty, value);
    }
    
    internal ITransform? SelectedIndicatorRenderTransform
    {
        get => GetValue(SelectedIndicatorRenderTransformProperty);
        set => SetValue(SelectedIndicatorRenderTransformProperty, value);
    }
    
    #endregion

    private Border? _selectedIndicator;
    private ItemsPresenter? _itemsPresenter;
    private TabStripScrollViewer? _scrollViewer;

    public TabStrip()
    {
        SelectionChanged += HandleSelectionChanged;
    }

    private void HandleSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (this.IsAttachedToVisualTree())
        {
            SetupSelectedIndicator();
        }
    }
    
    private void SetupSelectedIndicator()
    {
        if (Items.Count == 0)
        {
            _selectedIndicator?.SetCurrentValue(IsVisibleProperty, false);
        }
        else
        {
            if (_selectedIndicator is not null && SelectedItem is not null && ContainerFromItem(SelectedItem) is TabStripItem tabStripItem)
            {
                _selectedIndicator.SetCurrentValue(IsVisibleProperty, true);
                var selectedBounds = tabStripItem.Bounds;
                var builder        = new TransformOperations.Builder(1);
                var offset         = _itemsPresenter?.Bounds.Position ?? default;

                if (TabStripPlacement == Dock.Top)
                {
                    _selectedIndicator.SetCurrentValue(WidthProperty, tabStripItem.DesiredSize.Width);
                    _selectedIndicator.SetCurrentValue(HeightProperty, SelectedIndicatorThickness);
                    builder.AppendTranslate(offset.X + selectedBounds.Left, 0);
                }
                else if (TabStripPlacement == Dock.Right)
                {
                    _selectedIndicator.SetCurrentValue(HeightProperty, tabStripItem.DesiredSize.Height);
                    _selectedIndicator.SetCurrentValue(WidthProperty, SelectedIndicatorThickness);
                    builder.AppendTranslate(0, offset.Y + selectedBounds.Y);
                }
                else if (TabStripPlacement == Dock.Bottom)
                {
                    _selectedIndicator.SetCurrentValue(WidthProperty, tabStripItem.DesiredSize.Width);
                    _selectedIndicator.SetCurrentValue(HeightProperty, SelectedIndicatorThickness);
                    builder.AppendTranslate(offset.X + selectedBounds.Left, 0);
                }
                else
                {
                    _selectedIndicator.SetCurrentValue(HeightProperty, tabStripItem.DesiredSize.Height);
                    _selectedIndicator.SetCurrentValue(WidthProperty, SelectedIndicatorThickness);
                    builder.AppendTranslate(0, offset.Y + selectedBounds.Y);
                }

                SelectedIndicatorRenderTransform = builder.Build();
            }
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        if (SelectedItem is not null && ContainerFromItem(SelectedItem) is TabStripItem)
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
        _selectedIndicator = e.NameScope.Find<Border>(TabStripThemeConstants.SelectedItemIndicatorPart);
        _itemsPresenter    = e.NameScope.Find<ItemsPresenter>(TabStripThemeConstants.ItemsPresenterPart);
        _scrollViewer      = e.NameScope.Find<TabStripScrollViewer>(TabStripThemeConstants.TabsContainerPart);
        if (_scrollViewer != null)
        {
            _scrollViewer.TabStrip = this;
        }
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        
        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions(true);
            }
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
                    TransitionUtils.CreateTransition<TransformOperationsTransition>(SelectedIndicatorRenderTransformProperty,
                        SharedTokenKey.MotionDurationSlow, new ExponentialEaseOut())
                ];
            }
        }
        else
        {
            Transitions = null;
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