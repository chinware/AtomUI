using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Controls.Themes;
using AtomUI.Data;
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

public class CardTabControl : BaseTabControl
{
    protected override Type StyleKeyOverride { get; } = typeof(CardTabControl);
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
    private TabControlScrollViewer? _scrollViewer;
    private CompositeDisposable? _tokenBindingDisposables;
    private IDisposable? _borderBindingDisposable;
    
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
            Debug.Assert(ItemsBindingDisposables.ContainsKey(tabItem));
            var disposables = ItemsBindingDisposables[tabItem];
            disposables.Add(BindUtils.RelayBind(this, CardBorderRadiusProperty, tabItem, TabItem.CornerRadiusProperty));
            disposables.Add(BindUtils.RelayBind(this, CardBorderThicknessProperty, tabItem, TabItem.BorderThicknessProperty));
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _addTabButton   = e.NameScope.Find<IconButton>(TabControlThemeConstants.AddTabButtonPart);
        _itemsPresenter = e.NameScope.Find<ItemsPresenter>(TabControlThemeConstants.ItemsPresenterPart);
        if (_addTabButton is not null)
        {
            _addTabButton.Click += HandleAddButtonClicked;
        }
        _scrollViewer      = e.NameScope.Find<TabControlScrollViewer>(TabControlThemeConstants.CardTabStripScrollViewerPart);
        if (_scrollViewer != null)
        {
            _scrollViewer.TabControl = this;
        }
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
            _tokenBindingDisposables?.Add(TokenResourceBinder.CreateTokenBinding(this, CardBorderRadiusSizeProperty,
                SharedTokenKey.BorderRadiusLG));
        }
        else if (SizeType == SizeType.Middle)
        {
            _tokenBindingDisposables?.Add(TokenResourceBinder.CreateTokenBinding(this, CardBorderRadiusSizeProperty,
                SharedTokenKey.BorderRadius));
        }
        else
        {
            _tokenBindingDisposables?.Add(TokenResourceBinder.CreateTokenBinding(this, CardBorderRadiusSizeProperty,
                SharedTokenKey.BorderRadiusSM));
        }
    }
    
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _tokenBindingDisposables = new CompositeDisposable();
        _tokenBindingDisposables.Add(TokenResourceBinder.CreateTokenBinding(this, CardSizeProperty, TabControlTokenKey.CardSize));
        HandleSizeTypeChanged();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        _tokenBindingDisposables?.Dispose();
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _borderBindingDisposable = TokenResourceBinder.CreateTokenBinding(this, CardBorderThicknessProperty,
            SharedTokenKey.BorderThickness, BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this));
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _borderBindingDisposable?.Dispose();
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