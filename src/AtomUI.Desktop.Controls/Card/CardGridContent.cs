using System.Collections.Specialized;
using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

public class CardGridContent : ItemsControl
{
    #region 公共属性定义

    public static readonly StyledProperty<ColumnDefinitions> ColumnDefinitionsProperty =
        AvaloniaProperty.Register<CardGridContent, ColumnDefinitions>(nameof(ColumnDefinitions));
    
    public static readonly StyledProperty<RowDefinitions> RowDefinitionsProperty =
        AvaloniaProperty.Register<CardGridContent, RowDefinitions>(nameof(RowDefinitions));
    
    public static readonly StyledProperty<bool> IsHoverableProperty = 
        AvaloniaProperty.Register<CardGridContent, bool>(nameof (IsHoverable), true);

    public ColumnDefinitions ColumnDefinitions { get; set; } = new();

    public RowDefinitions RowDefinitions { get; set; } = new();
    
    public bool IsHoverable
    {
        get => GetValue(IsHoverableProperty);
        set => SetValue(IsHoverableProperty, value);
    }

    #endregion
    
    #region 内部属性定义
    
    internal static readonly StyledProperty<SizeType> SizeTypeProperty = 
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<CardGridContent>();

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<CardGridContent>();
    
    private readonly Dictionary<CardGridItem, CompositeDisposable> _itemsBindingDisposables = new();
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion

    private ItemsPresenter? _itemsPresenter;
    
    public CardGridContent()
    {
        Items.CollectionChanged += HandleCollectionChanged;
    }
    
    private void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems.Count > 0)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is CardGridItem gridItem)
                    {
                        if (_itemsBindingDisposables.TryGetValue(gridItem, out var disposable))
                        {
                            disposable.Dispose();
                            _itemsBindingDisposables.Remove(gridItem);
                        }
                    }
                }
            }
        }
    }
    
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new CardGridItem();
    }
    
    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<CardGridItem>(item, out recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is CardGridItem gridItem)
        {
            var disposables = new CompositeDisposable(8);
            if (item != null && item is not Visual)
            {
                if (!gridItem.IsSet(CardGridItem.ContentProperty))
                {
                    gridItem.SetCurrentValue(CardGridItem.ContentProperty, item);
                }
            }

            if (ItemTemplate != null)
            {
                disposables.Add(BindUtils.RelayBind(this, ItemTemplateProperty, gridItem, CardGridItem.ContentTemplateProperty));
            }
            disposables.Add(BindUtils.RelayBind(this, SizeTypeProperty, gridItem, CardGridItem.SizeTypeProperty));
            disposables.Add(BindUtils.RelayBind(this, IsEnabledProperty, gridItem, CardGridItem.IsEnabledProperty));
            disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, gridItem, CardGridItem.IsMotionEnabledProperty));
            PrepareCardGridItem(gridItem, item, index, disposables);
        
            if (_itemsBindingDisposables.TryGetValue(gridItem, out var oldDisposables))
            {
                oldDisposables.Dispose();
                _itemsBindingDisposables.Remove(gridItem);
            }
            _itemsBindingDisposables.Add(gridItem, disposables);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(container), "The container type is incorrect, it must be type CardGridItem.");
        }
    }
    
    protected virtual void PrepareCardGridItem(CardGridItem cardGridItem, object? item, int index, CompositeDisposable compositeDisposable)
    {
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ColumnDefinitionsProperty ||
            change.Property == RowDefinitionsProperty)
        {
            SyncGridPanelProperties();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _itemsPresenter = e.NameScope.Find<ItemsPresenter>(CardGridContentThemeConstants.ItemsPresenterPart);
        if (_itemsPresenter != null)
        {
            _itemsPresenter.ApplyTemplate();
            SyncGridPanelProperties();
        }
    }

    private void SyncGridPanelProperties()
    {
        if (_itemsPresenter?.Panel != null)
        {
            var panel = _itemsPresenter?.Panel;
            if (panel is Grid gridPanel)
            {
                gridPanel.ColumnDefinitions = ColumnDefinitions;
                gridPanel.RowDefinitions = RowDefinitions;
            }
        }
    }
}