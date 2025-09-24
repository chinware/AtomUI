using System.Collections.Specialized;
using System.Reactive.Disposables;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal class CarouselPagination : SelectingItemsControl
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<CarouselPagination>();
    
    public static readonly StyledProperty<bool> IsShowTransitionProgressProperty = 
        Carousel.IsShowTransitionProgressProperty.AddOwner<CarouselPagination>();
    
    public static readonly StyledProperty<TimeSpan> AutoPlaySpeedProperty = 
        Carousel.AutoPlaySpeedProperty.AddOwner<CarouselPagination>();
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public bool IsShowTransitionProgress
    {
        get => GetValue(IsShowTransitionProgressProperty);
        set => SetValue(IsShowTransitionProgressProperty, value);
    }
    
    public TimeSpan AutoPlaySpeed
    {
        get => GetValue(AutoPlaySpeedProperty);
        set => SetValue(AutoPlaySpeedProperty, value);
    }

    #endregion
    
    #region 内部属性定义

    internal static readonly StyledProperty<double> ItemSpacingProperty =
        AvaloniaProperty.Register<CarouselPagination, double>(nameof(ItemSpacing));
    
    internal double ItemSpacing
    {
        get => GetValue(ItemSpacingProperty);
        set => SetValue(ItemSpacingProperty, value);
    }
    #endregion
    
    private readonly Dictionary<CarouselPageIndicator, CompositeDisposable> _itemsBindingDisposables = new();

    static CarouselPagination()
    {
        AutoScrollToSelectedItemProperty.OverrideDefaultValue<CarouselPagination>(false);
    }
    
    public CarouselPagination()
    {
        Items.CollectionChanged += HandleCollectionChanged;
        SelectionMode           =  SelectionMode.Single;
    }
    
    private void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems.Count > 0)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is CarouselPageIndicator indicator)
                    {
                        if (_itemsBindingDisposables.TryGetValue(indicator, out var disposable))
                        {
                            disposable.Dispose();
                            _itemsBindingDisposables.Remove(indicator);
                        }
                    }
                }
            }
        }
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new CarouselPageIndicator();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<CarouselPageIndicator>(item, out recycleKey);
    }
    
    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is CarouselPageIndicator pageIndicator)
        {
            var disposables = new CompositeDisposable(2);
            
            if (item != null && item is not Visual)
            {
                if (!pageIndicator.IsSet(CarouselPageIndicator.ContentProperty))
                {
                    pageIndicator.SetCurrentValue(CarouselPageIndicator.ContentProperty, item);
                }
            }
            
            if (ItemTemplate != null)
            {
                disposables.Add(BindUtils.RelayBind(this, ItemTemplateProperty, pageIndicator, CarouselPageIndicator.ContentTemplateProperty));
            }
            
            disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, pageIndicator, CarouselPageIndicator.IsMotionEnabledProperty));
            disposables.Add(BindUtils.RelayBind(this, IsShowTransitionProgressProperty, pageIndicator, CarouselPageIndicator.IsShowTransitionProgressProperty));
            disposables.Add(BindUtils.RelayBind(this, AutoPlaySpeedProperty, pageIndicator, CarouselPageIndicator.AutoPlaySpeedProperty));
            
            PreparePageIndicator(pageIndicator, item, index, disposables);
            
            if (_itemsBindingDisposables.TryGetValue(pageIndicator, out var oldDisposables))
            {
                oldDisposables.Dispose();
                _itemsBindingDisposables.Remove(pageIndicator);
            }
            _itemsBindingDisposables.Add(pageIndicator, disposables);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(container), "The container type is incorrect, it must be type CarouselPageIndicator.");
        }
    }
    
    protected virtual void PreparePageIndicator(CarouselPageIndicator pageIndicator, object? item, int index, CompositeDisposable compositeDisposable)
    {
    }
    
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && e.Pointer.Type == PointerType.Mouse)
        {
            e.Handled = UpdateSelectionFromEventSource(e.Source);
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == MouseButton.Left && e.Pointer.Type != PointerType.Mouse)
        {
            var container = GetContainerFromEventSource(e.Source);
            if (container != null
                && container.GetVisualsAt(e.GetPosition(container))
                            .Any(c => container == c || container.IsVisualAncestorOf(c)))
            {
                e.Handled = UpdateSelectionFromEventSource(e.Source);
            }
        }
    }
}