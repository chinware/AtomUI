using System.Diagnostics;
using AtomUI.Controls.Themes;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace AtomUI.Controls;

public class CardTabStrip : BaseTabStrip
{
    #region 公共属性实现

    public static readonly StyledProperty<bool> IsShowAddTabButtonProperty =
        AvaloniaProperty.Register<CardTabStrip, bool>(nameof(IsShowAddTabButton));

    public static readonly RoutedEvent<RoutedEventArgs> AddTabRequestEvent =
        RoutedEvent.Register<CardTabStrip, RoutedEventArgs>(
            nameof(AddTabRequest),
            RoutingStrategies.Bubble);

    public bool IsShowAddTabButton
    {
        get => GetValue(IsShowAddTabButtonProperty);
        set => SetValue(IsShowAddTabButtonProperty, value);
    }

    public event EventHandler<RoutedEventArgs>? AddTabRequest
    {
        add => AddHandler(AddTabRequestEvent, value);
        remove => RemoveHandler(AddTabRequestEvent, value);
    }

    #endregion

    #region 内部属性实现

    internal static readonly StyledProperty<CornerRadius> CardBorderRadiusProperty =
        AvaloniaProperty.Register<CardTabStrip, CornerRadius>(nameof(CardBorderRadius));

    internal static readonly StyledProperty<Thickness> CardBorderThicknessProperty =
        AvaloniaProperty.Register<CardTabStrip, Thickness>(nameof(CardBorderThickness));
    
    internal static readonly StyledProperty<CornerRadius> EffectiveCardBorderRadiusProperty =
        AvaloniaProperty.Register<CardTabStrip, CornerRadius>(nameof(EffectiveCardBorderRadius));
    
    internal static readonly StyledProperty<double> CardSizeProperty =
        AvaloniaProperty.Register<CardTabStrip, double>(nameof(CardSize));

    internal CornerRadius CardBorderRadius
    {
        get => GetValue(CardBorderRadiusProperty);
        set => SetValue(CardBorderRadiusProperty, value);
    }
    
    internal CornerRadius EffectiveCardBorderRadius
    {
        get => GetValue(EffectiveCardBorderRadiusProperty);
        set => SetValue(EffectiveCardBorderRadiusProperty, value);
    }

    internal Thickness CardBorderThickness
    {
        get => GetValue(CardBorderThicknessProperty);
        set => SetValue(CardBorderThicknessProperty, value);
    }

    internal double CardSize
    {
        get => GetValue(CardSizeProperty);
        set => SetValue(CardSizeProperty, value);
    }

    #endregion

    private IconButton? _addTabButton;
    private ItemsPresenter? _itemsPresenter;
    private TabStripScrollViewer? _scrollViewer;

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new TabStripItem
        {
            Shape = TabSharp.Card
        };
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is TabStripItem tabStripItem)
        {
            tabStripItem.Shape = TabSharp.Card;
            Debug.Assert(ItemsBindingDisposables.ContainsKey(tabStripItem));
            var disposables = ItemsBindingDisposables[tabStripItem];
            disposables.Add(BindUtils.RelayBind(this, EffectiveCardBorderRadiusProperty, tabStripItem, CornerRadiusProperty));
            disposables.Add(BindUtils.RelayBind(this, CardBorderThicknessProperty, tabStripItem, BorderThicknessProperty));
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _addTabButton   = e.NameScope.Find<IconButton>(TabStripThemeConstants.AddTabButtonPart);
        _itemsPresenter = e.NameScope.Find<ItemsPresenter>(TabStripThemeConstants.ItemsPresenterPart);
        if (_addTabButton is not null)
        {
            _addTabButton.Click += HandleAddButtonClicked;
        }
        _scrollViewer      = e.NameScope.Find<TabStripScrollViewer>(TabStripThemeConstants.CardTabStripScrollViewerPart);
        if (_scrollViewer != null)
        {
            _scrollViewer.TabStrip = this;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        
        if (change.Property == TabStripPlacementProperty ||
            change.Property == CardSizeProperty)
        {
            HandleTabStripPlacementChanged();
        }
    }

    private void HandleAddButtonClicked(object? sender, RoutedEventArgs args)
    {
        RaiseEvent(new RoutedEventArgs(AddTabRequestEvent));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        HandleTabStripPlacementChanged();
        return size;
    }

    private void HandleTabStripPlacementChanged()
    {
        if (TabStripPlacement == Dock.Top)
        {
            EffectiveCardBorderRadius = new CornerRadius(CardBorderRadius.TopLeft, CardBorderRadius.TopRight,
                bottomLeft: 0, bottomRight: 0);
            if (_addTabButton is not null && _itemsPresenter is not null)
            {
                _addTabButton.Width  = CardSize;
                _addTabButton.Height = _itemsPresenter.DesiredSize.Height;
            }
        }
        else if (TabStripPlacement == Dock.Bottom)
        {
            EffectiveCardBorderRadius = new CornerRadius(0, 0, bottomLeft: CardBorderRadius.BottomLeft,
                bottomRight: CardBorderRadius.BottomRight);
            if (_addTabButton is not null && _itemsPresenter is not null)
            {
                _addTabButton.Width  = CardSize;
                _addTabButton.Height = _itemsPresenter.DesiredSize.Height;
            }
        }
        else if (TabStripPlacement == Dock.Left)
        {
            EffectiveCardBorderRadius = new CornerRadius(CardBorderRadius.TopLeft, 0,
                bottomLeft: CardBorderRadius.BottomLeft, bottomRight: 0);
            if (_addTabButton is not null && _itemsPresenter is not null)
            {
                _addTabButton.Width               = CardSize;
                _addTabButton.Height              = CardSize;
                _addTabButton.HorizontalAlignment = HorizontalAlignment.Right;
            }
        }
        else
        {
            EffectiveCardBorderRadius = new CornerRadius(0, CardBorderRadius.TopRight, bottomLeft: 0,
                bottomRight: CardBorderRadius.BottomRight);
            if (_addTabButton is not null && _itemsPresenter is not null)
            {
                _addTabButton.Width               = CardSize;
                _addTabButton.Height              = CardSize;
                _addTabButton.HorizontalAlignment = HorizontalAlignment.Left;
            }
        }
    }
}