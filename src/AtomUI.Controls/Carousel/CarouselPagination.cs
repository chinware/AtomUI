using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

internal class CarouselPagination : SelectingItemsControl
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<CarouselPagination>();
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
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
    
    private readonly Dictionary<SegmentedItem, CompositeDisposable> _itemsBindingDisposables = new();

    static CarouselPagination()
    {
        AutoScrollToSelectedItemProperty.OverrideDefaultValue<Segmented>(false);
    }
}