using AtomUI.Data;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace AtomUI.Controls;

public class CardTabControl : BaseTabControl
{
    #region 公共属性实现

    public static readonly StyledProperty<bool> IsShowAddTabButtonProperty =
        AvaloniaProperty.Register<CardTabControl, bool>(nameof(IsShowAddTabButton));

    public static readonly RoutedEvent<RoutedEventArgs> AddTabRequestEvent =
        RoutedEvent.Register<CardTabControl, RoutedEventArgs>(
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
        AvaloniaProperty.Register<CardTabControl, CornerRadius>(nameof(CardBorderRadius));

    internal static readonly StyledProperty<Thickness> CardBorderThicknessProperty =
        AvaloniaProperty.Register<CardTabControl, Thickness>(nameof(CardBorderThickness));

    internal static readonly DirectProperty<CardTabControl, CornerRadius> CardBorderRadiusSizeProperty =
        AvaloniaProperty.RegisterDirect<CardTabControl, CornerRadius>(nameof(CardBorderRadiusSize),
            o => o.CardBorderRadiusSize,
            (o, v) => o.CardBorderRadiusSize = v);

    internal static readonly DirectProperty<CardTabControl, double> CardSizeProperty =
        AvaloniaProperty.RegisterDirect<CardTabControl, double>(nameof(CardSize),
            o => o.CardSize,
            (o, v) => o.CardSize = v);

    internal CornerRadius CardBorderRadius
    {
        get => GetValue(CardBorderRadiusProperty);
        set => SetValue(CardBorderRadiusProperty, value);
    }

    internal Thickness CardBorderThickness
    {
        get => GetValue(CardBorderThicknessProperty);
        set => SetValue(CardBorderThicknessProperty, value);
    }

    private CornerRadius _cardBorderRadiusSize;

    internal CornerRadius CardBorderRadiusSize
    {
        get => _cardBorderRadiusSize;
        set => SetAndRaise(CardBorderRadiusSizeProperty, ref _cardBorderRadiusSize, value);
    }

    private double _cardSize;

    internal double CardSize
    {
        get => _cardSize;
        set => SetAndRaise(CardSizeProperty, ref _cardSize, value);
    }

    #endregion
    
    private IconButton? _addTabButton;
    private ItemsPresenter? _itemsPresenter;

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new TabItem
        {
            Shape = TabSharp.Card
        };
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is TabItem tabItem)
        {
            tabItem.Shape = TabSharp.Card;
            BindUtils.RelayBind(this, CardBorderRadiusProperty, tabItem, CornerRadiusProperty);
            BindUtils.RelayBind(this, CardBorderThicknessProperty, tabItem, BorderThicknessProperty);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        TokenResourceBinder.CreateTokenBinding(this, CardBorderThicknessProperty,
            SharedTokenKey.BorderThickness, BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this));
        TokenResourceBinder.CreateTokenBinding(this, CardSizeProperty, TabControlTokenKey.CardSize);
        HandleTemplateApplied(e.NameScope);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SizeTypeProperty)
        {
            HandleSizeTypeChanged();
        }
        else if (change.Property == TabStripPlacementProperty)
        {
            HandleTabStripPlacementChanged();
        }
    }

    private void HandleTemplateApplied(INameScope scope)
    {
        _addTabButton   = scope.Find<IconButton>(CardTabControlTheme.AddTabButtonPart);
        _itemsPresenter = scope.Find<ItemsPresenter>(BaseTabControlTheme.ItemsPresenterPart);
        if (_addTabButton is not null)
        {
            _addTabButton.Click += HandleAddButtonClicked;
        }

        HandleSizeTypeChanged();
    }
    
    private void HandleAddButtonClicked(object? sender, RoutedEventArgs args)
    {
        RaiseEvent(new RoutedEventArgs(AddTabRequestEvent));
    }

    private void HandleSizeTypeChanged()
    {
        if (SizeType == SizeType.Large)
        {
            TokenResourceBinder.CreateSharedResourceBinding(this, CardBorderRadiusSizeProperty,
                SharedTokenKey.BorderRadiusLG);
        }
        else if (SizeType == SizeType.Middle)
        {
            TokenResourceBinder.CreateSharedResourceBinding(this, CardBorderRadiusSizeProperty,
                SharedTokenKey.BorderRadius);
        }
        else
        {
            TokenResourceBinder.CreateSharedResourceBinding(this, CardBorderRadiusSizeProperty,
                SharedTokenKey.BorderRadiusSM);
        }
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
            CardBorderRadius = new CornerRadius(_cardBorderRadiusSize.TopLeft,
                _cardBorderRadiusSize.TopRight, bottomLeft: 0, bottomRight: 0);
            if (_addTabButton is not null && _itemsPresenter is not null)
            {
                _addTabButton.Width  = _cardSize;
                _addTabButton.Height = _itemsPresenter.DesiredSize.Height;
            }
        }
        else if (TabStripPlacement == Dock.Bottom)
        {
            CardBorderRadius = new CornerRadius(0, 0, bottomLeft: _cardBorderRadiusSize.BottomLeft,
                bottomRight: _cardBorderRadiusSize.BottomRight);
            if (_addTabButton is not null && _itemsPresenter is not null)
            {
                _addTabButton.Width  = _cardSize;
                _addTabButton.Height = _itemsPresenter.DesiredSize.Height;
            }
        }
        else if (TabStripPlacement == Dock.Left)
        {
            CardBorderRadius = new CornerRadius(_cardBorderRadiusSize.TopLeft, 0,
                bottomLeft: _cardBorderRadiusSize.BottomLeft, bottomRight: 0);
            if (_addTabButton is not null && _itemsPresenter is not null)
            {
                _addTabButton.Width               = _cardSize;
                _addTabButton.Height              = _cardSize;
                _addTabButton.HorizontalAlignment = HorizontalAlignment.Right;
            }
        }
        else
        {
            CardBorderRadius = new CornerRadius(0, _cardBorderRadiusSize.TopRight, bottomLeft: 0,
                bottomRight: _cardBorderRadiusSize.BottomRight);
            if (_addTabButton is not null && _itemsPresenter is not null)
            {
                _addTabButton.Width               = _cardSize;
                _addTabButton.Height              = _cardSize;
                _addTabButton.HorizontalAlignment = HorizontalAlignment.Left;
            }
        }
    }
}