using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Transformation;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public class TabControl : BaseTabControl
{
    protected override Type StyleKeyOverride { get; } = typeof(TabControl);
    #region 内部属性定义

    internal static readonly StyledProperty<double> SelectedIndicatorThicknessProperty =
        AvaloniaProperty.Register<TabControl, double>(nameof(SelectedIndicatorThickness));
    
    internal double SelectedIndicatorThickness
    {
        get => GetValue(SelectedIndicatorThicknessProperty);
        set => SetValue(SelectedIndicatorThicknessProperty, value);
    }

    #endregion

    private Border? _selectedIndicator;
    private ItemsPresenter? _itemsPresenter;
    private TabControlScrollViewer? _scrollViewer;

    public TabControl()
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

    private void ConfigureSelectedIndicatorTransitions(bool force)
    {
        if (_selectedIndicator is not null)
        {
            if (IsMotionEnabled)
            {
                if (force || _selectedIndicator.Transitions == null)
                {
                    _selectedIndicator.Transitions =
                    [
                        TransitionUtils.CreateTransition<TransformOperationsTransition>(RenderTransformProperty,
                            SharedTokenKey.MotionDurationSlow, new ExponentialEaseOut())
                    ];
                }
            }
            else
            {
                _selectedIndicator.Transitions = null;
            }
        }
    }
    
    private void SetupSelectedIndicator()
    {
        if (_selectedIndicator is not null && SelectedItem is TabItem tabStripItem)
        {
            var selectedBounds = tabStripItem.Bounds;
            var builder        = new TransformOperations.Builder(1);
            var offset         = _itemsPresenter?.Bounds.Position ?? default;

            if (TabStripPlacement == Dock.Top)
            {
                _selectedIndicator.SetValue(WidthProperty, tabStripItem.DesiredSize.Width);
                _selectedIndicator.SetValue(HeightProperty, SelectedIndicatorThickness);
                builder.AppendTranslate(offset.X + selectedBounds.Left, 0);
            }
            else if (TabStripPlacement == Dock.Right)
            {
                _selectedIndicator.SetValue(HeightProperty, tabStripItem.DesiredSize.Height);
                _selectedIndicator.SetValue(WidthProperty, SelectedIndicatorThickness);
                builder.AppendTranslate(0, offset.Y + selectedBounds.Y);
            }
            else if (TabStripPlacement == Dock.Bottom)
            {
                _selectedIndicator.SetValue(WidthProperty, tabStripItem.DesiredSize.Width);
                _selectedIndicator.SetValue(HeightProperty, SelectedIndicatorThickness);
                builder.AppendTranslate(offset.X + selectedBounds.Left, 0);
            }
            else
            {
                _selectedIndicator.SetValue(HeightProperty, tabStripItem.DesiredSize.Height);
                _selectedIndicator.SetValue(WidthProperty, SelectedIndicatorThickness);
                builder.AppendTranslate(0, offset.Y + selectedBounds.Y);
            }

            _selectedIndicator.RenderTransform = builder.Build();
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        if (SelectedItem is TabItem)
        {
            SetupSelectedIndicator();
        }

        return size;
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        var tabItem = new TabItem
        {
            Shape = TabSharp.Line
        };
        return tabItem;
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is TabItem tabItem)
        {
            tabItem.Shape = TabSharp.Line;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _selectedIndicator = e.NameScope.Find<Border>(TabControlThemeConstants.SelectedItemIndicatorPart);
        _itemsPresenter    = e.NameScope.Find<ItemsPresenter>(TabControlThemeConstants.ItemsPresenterPart);
        _scrollViewer      = e.NameScope.Find<TabControlScrollViewer>(TabControlThemeConstants.TabsContainerPart);
        if (_scrollViewer != null)
        {
            _scrollViewer.TabControl = this;
        }

        if (_selectedIndicator != null)
        {
            _selectedIndicator.Loaded += (sender, args) =>
            {
                ConfigureSelectedIndicatorTransitions(false);
            };
            _selectedIndicator.Unloaded += (sender, args) =>
            {
                _selectedIndicator.Transitions = null;
            };
        }
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureSelectedIndicatorTransitions(true);
            }
        }
    }
}