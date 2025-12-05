using System.Collections.Specialized;
using System.Reactive.Disposables;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal class RateItemsControl : ItemsControl, ISizeTypeAware
{
    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<RateItemsControl>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<RateItemsControl>();
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion
    
    private readonly Dictionary<AbstractRateItem, CompositeDisposable> _itemsBindingDisposables = new();

    public RateItemsControl()
    {
        LogicalChildren.CollectionChanged += HandleCollectionChanged;
    }
    
    private void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems.Count > 0)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is AbstractRateItem rateItem)
                    {
                        if (_itemsBindingDisposables.TryGetValue(rateItem, out var disposable))
                        {
                            disposable.Dispose();
                            _itemsBindingDisposables.Remove(rateItem);
                        }
                    }
                }
            }
        }
    }
}