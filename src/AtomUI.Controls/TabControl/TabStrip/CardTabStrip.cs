using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

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

    internal static readonly DirectProperty<CardTabStrip, CornerRadius> CardBorderRadiusSizeProperty =
        AvaloniaProperty.RegisterDirect<CardTabStrip, CornerRadius>(nameof(CardBorderRadiusSize),
            o => o.CardBorderRadiusSize,
            (o, v) => o.CardBorderRadiusSize = v);

    internal static readonly DirectProperty<CardTabStrip, double> CardSizeProperty =
        AvaloniaProperty.RegisterDirect<CardTabStrip, double>(nameof(CardSize),
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
            BindUtils.RelayBind(this, CardBorderRadiusProperty, tabStripItem, CornerRadiusProperty);
            BindUtils.RelayBind(this, CardBorderThicknessProperty, tabStripItem, BorderThicknessProperty);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _addTabButton   = e.NameScope.Find<IconButton>(CardTabStripTheme.AddTabButtonPart);
        _itemsPresenter = e.NameScope.Find<ItemsPresenter>(BaseTabStripTheme.ItemsPresenterPart);
        if (_addTabButton is not null)
        {
            _addTabButton.Click += HandleAddButtonClicked;
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        this.AddResourceBindingDisposable(
            TokenResourceBinder.CreateTokenBinding(this, CardSizeProperty, TabControlTokenKey.CardSize));
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, CardBorderThicknessProperty,
            SharedTokenKey.BorderThickness, BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
        HandleSizeTypeChanged();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == SizeTypeProperty)
            {
                HandleSizeTypeChanged();
            }
        }

        if (change.Property == TabStripPlacementProperty)
        {
            HandleTabStripPlacementChanged();
        }
    }

    private void HandleAddButtonClicked(object? sender, RoutedEventArgs args)
    {
        RaiseEvent(new RoutedEventArgs(AddTabRequestEvent));
    }

    private void HandleSizeTypeChanged()
    {
        if (SizeType == SizeType.Large)
        {
            this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, CardBorderRadiusSizeProperty,
                SharedTokenKey.BorderRadiusLG));
        }
        else if (SizeType == SizeType.Middle)
        {
            this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, CardBorderRadiusSizeProperty,
                SharedTokenKey.BorderRadius));
        }
        else
        {
            this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, CardBorderRadiusSizeProperty,
                SharedTokenKey.BorderRadiusSM));
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
            CardBorderRadius = new CornerRadius(_cardBorderRadiusSize.TopLeft, _cardBorderRadiusSize.TopRight,
                bottomLeft: 0, bottomRight: 0);
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