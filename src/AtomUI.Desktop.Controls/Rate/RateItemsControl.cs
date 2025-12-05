using System.Collections.Specialized;
using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

internal class RateItemsControl : ItemsControl, ISizeTypeAware
{
    #region 公共属性定义
    
    public static readonly StyledProperty<bool> IsAllowClearProperty =
       Rate.IsAllowClearProperty.AddOwner<RateItemsControl>();
    
    public static readonly StyledProperty<bool> IsAllowHalfProperty =
        Rate.IsAllowHalfProperty.AddOwner<RateItemsControl>();
    
    public static readonly StyledProperty<IBrush?> StarColorProperty =
        Rate.StarColorProperty.AddOwner<RateItemsControl>();
    
    public static readonly StyledProperty<IBrush?> StarBgColorProperty =
        Rate.StarBgColorProperty.AddOwner<RateItemsControl>();

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<RateItemsControl>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<RateItemsControl>();
    
    public bool IsAllowClear
    {
        get => GetValue(IsAllowClearProperty);
        set => SetValue(IsAllowClearProperty, value);
    }
    
    public bool IsAllowHalf
    {
        get => GetValue(IsAllowHalfProperty);
        set => SetValue(IsAllowHalfProperty, value);
    }
    
    public IBrush? StarColor
    {
        get => GetValue(StarColorProperty);
        set => SetValue(StarColorProperty, value);
    }
    
    public IBrush? StarBgColor
    {
        get => GetValue(StarBgColorProperty);
        set => SetValue(StarBgColorProperty, value);
    }
    
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

    #region 内部属性定义
    
    internal static readonly DirectProperty<RateItemsControl, object?> CharacterProperty =
        AvaloniaProperty.RegisterDirect<RateItemsControl, object?>(
            nameof(Character),
            o => o.Character,
            (o, v) => o.Character = v);
    
    private object? _character;

    internal object? Character
    {
        get => _character;
        set => SetAndRaise(CharacterProperty, ref _character, value);
    }
    
    #endregion
    
    private readonly Dictionary<RateItem, CompositeDisposable> _itemsBindingDisposables = new();

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
                    if (item is RateItem rateItem)
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
    
    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is RateItem rateItem)
        {
            var disposables = new CompositeDisposable(2);
            
            disposables.Add(BindUtils.RelayBind(this, SizeTypeProperty, rateItem, RateItem.SizeTypeProperty));
            disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, rateItem, RateItem.IsMotionEnabledProperty));
            disposables.Add(BindUtils.RelayBind(this, CharacterProperty, rateItem, RateItem.CharacterProperty));
            disposables.Add(BindUtils.RelayBind(this, StarColorProperty, rateItem, RateItem.StarColorProperty));
            disposables.Add(BindUtils.RelayBind(this, StarBgColorProperty, rateItem, RateItem.StarBgColorProperty));
            disposables.Add(BindUtils.RelayBind(this, IsAllowClearProperty, rateItem, RateItem.IsAllowClearProperty));
            disposables.Add(BindUtils.RelayBind(this, IsAllowHalfProperty, rateItem, RateItem.IsAllowHalfProperty));
            
            if (_itemsBindingDisposables.TryGetValue(rateItem, out var oldDisposables))
            {
                oldDisposables.Dispose();
                _itemsBindingDisposables.Remove(rateItem);
            }
            _itemsBindingDisposables.Add(rateItem, disposables);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(container), "The container type is incorrect, it must be type RateItem.");
        }
    }
}