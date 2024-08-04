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

public class CardTabControl : BaseTabControl, IControlCustomStyle
{
   #region 公共属性实现
   public readonly static StyledProperty<bool> IsShowAddTabButtonProperty =
      AvaloniaProperty.Register<CardTabControl, bool>(nameof(IsShowAddTabButton), false);
   
   public readonly static RoutedEvent<RoutedEventArgs> AddTabRequestEvent =
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
   
    private IControlCustomStyle _customStyle;
   private IconButton? _addTabButton;
   private ItemsPresenter? _itemsPresenter;
   private Grid? _tabsContainer;
   private BaseTabScrollViewer? _tabScrollViewer;

   public CardTabControl()
   {
      _customStyle = this;
   }
   
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
      if (container is TabItem tabItem) {
         tabItem.Shape = TabSharp.Card;
         BindUtils.RelayBind(this, CardBorderRadiusProperty, tabItem, TabItem.CornerRadiusProperty);
         BindUtils.RelayBind(this, CardBorderThicknessProperty, tabItem, TabItem.BorderThicknessProperty);
      }
   }
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      TokenResourceBinder.CreateTokenBinding(this, CardBorderThicknessProperty, GlobalResourceKey.BorderThickness, BindingPriority.Template,
                                             new RenderScaleAwareThicknessConfigure(this));
      TokenResourceBinder.CreateTokenBinding(this, CardSizeProperty, TabControlResourceKey.CardSize);
      _customStyle.HandleTemplateApplied(e.NameScope);
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      if (change.Property == SizeTypeProperty) {
         HandleSizeTypeChanged();
      } else if (change.Property == TabStripPlacementProperty) {
         // SetupCardTabStripContainer();
         HandleTabStripPlacementChanged();
      }
   }

   // private void SetupCardTabStripContainer(Size finalSize)
   // {
   //    if (_tabsContainer is not null) {
   //       double addButtonOffset = 0;
   //       double markOffset = 0;
   //       if (TabStripPlacement == Dock.Top || TabStripPlacement == Dock.Bottom) {
   //          addButtonOffset = _addTabButton?.Bounds.Right ?? 0;
   //          markOffset = finalSize.Width;
   //          if (addButtonOffset > markOffset) {
   //             _tabsContainer.ColumnDefinitions[0].Width = GridLength.Star;
   //          } else {
   //             _tabsContainer.ColumnDefinitions[0].Width = GridLength.Auto;
   //          }
   //       } else {
   //          addButtonOffset = _addTabButton?.Bounds.Bottom ?? 0;
   //          markOffset = finalSize.Height;
   //          if (addButtonOffset > markOffset) {
   //             _tabsContainer.RowDefinitions[0].Height = GridLength.Star;
   //          } else {
   //             _tabsContainer.RowDefinitions[0].Height = GridLength.Auto;
   //          }
   //       }
   //      
   //    }
   // }

   #region IControlCustomStyle 实现
   
   void IControlCustomStyle.HandleTemplateApplied(INameScope scope)
   {
      _addTabButton = scope.Find<IconButton>(CardTabControlTheme.AddTabButtonPart);
      _itemsPresenter = scope.Find<ItemsPresenter>(CardTabControlTheme.ItemsPresenterPart);
      // _tabsContainer = scope.Find<Grid>(CardTabControlTheme.TabsContainerPart);
      _tabScrollViewer = scope.Find<BaseTabScrollViewer>(CardTabControlTheme.CardTabStripScrollViewerPart);
      if (_addTabButton is not null) {
         _addTabButton.Click += HandleAddButtonClicked;
      }
      HandleSizeTypeChanged();
      // SetupCardTabStripContainer();
   }
   #endregion

   private void HandleAddButtonClicked(object? sender, RoutedEventArgs args)
   {
      RaiseEvent(new RoutedEventArgs(AddTabRequestEvent));
   }
   
   private void HandleSizeTypeChanged()
   {
      if (SizeType == SizeType.Large) {
         TokenResourceBinder.CreateGlobalResourceBinding(this, CardBorderRadiusSizeProperty, GlobalResourceKey.BorderRadiusLG);
      } else if (SizeType == SizeType.Middle) {
         TokenResourceBinder.CreateGlobalResourceBinding(this, CardBorderRadiusSizeProperty, GlobalResourceKey.BorderRadius);
      } else {
         TokenResourceBinder.CreateGlobalResourceBinding(this, CardBorderRadiusSizeProperty, GlobalResourceKey.BorderRadiusSM);
      }
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      var size = base.ArrangeOverride(finalSize);
      // SetupCardTabStripContainer(finalSize);
      HandleTabStripPlacementChanged();
      return size;
   }

   // private void SetupCardTabStripContainer()
   // {
   //    if (TabStripPlacement == Dock.Top ||
   //        TabStripPlacement == Dock.Bottom) {
   //       if (_tabsContainer is not null) {
   //          _tabsContainer.Children.Clear();
   //          _tabsContainer.RowDefinitions.Clear();
   //          _tabsContainer.ColumnDefinitions = new ColumnDefinitions()
   //          {
   //             new ColumnDefinition(GridLength.Auto),
   //             new ColumnDefinition(GridLength.Auto),
   //          };
   //       }
   //
   //       if (_tabScrollViewer is not null) {
   //          Grid.SetColumn(_tabScrollViewer, 0);
   //       }
   //
   //       if (_addTabButton is not null) {
   //          Grid.SetColumn(_addTabButton, 1);
   //       }
   //
   //       _tabsContainer!.Children.Add(_tabScrollViewer!);
   //       _tabsContainer.Children.Add(_addTabButton!);
   //    } else {
   //       if (_tabsContainer is not null) {
   //          _tabsContainer.Children.Clear();
   //          _tabsContainer.ColumnDefinitions.Clear();
   //          _tabsContainer.RowDefinitions = new RowDefinitions()
   //          {
   //             new RowDefinition(GridLength.Auto),
   //             new RowDefinition(GridLength.Auto),
   //          };
   //       }
   //       if (_tabScrollViewer is not null) {
   //          Grid.SetRow(_tabScrollViewer, 0);
   //       }
   //
   //       if (_addTabButton is not null) {
   //          Grid.SetRow(_addTabButton, 1);
   //       }
   //       _tabsContainer!.Children.Add(_tabScrollViewer!);
   //       _tabsContainer.Children.Add(_addTabButton!);
   //    }
   // }

   private void HandleTabStripPlacementChanged()
   {
      if (TabStripPlacement == Dock.Top) {
         CardBorderRadius = new CornerRadius(topLeft: _cardBorderRadiusSize.TopLeft,
                                             topRight: _cardBorderRadiusSize.TopRight, bottomLeft: 0, bottomRight: 0);
         if (_addTabButton is not null && _itemsPresenter is not null) {
            _addTabButton.Width = _cardSize;
            _addTabButton.Height = _itemsPresenter.DesiredSize.Height;
         }
      } else if (TabStripPlacement == Dock.Bottom) {
         CardBorderRadius = new CornerRadius(topLeft: 0, 0, bottomLeft: _cardBorderRadiusSize.BottomLeft,
                                             bottomRight: _cardBorderRadiusSize.BottomRight);
         if (_addTabButton is not null && _itemsPresenter is not null) {
            _addTabButton.Width = _cardSize;
            _addTabButton.Height = _itemsPresenter.DesiredSize.Height;
         }
      } else if (TabStripPlacement == Dock.Left) {
         CardBorderRadius = new CornerRadius(topLeft: _cardBorderRadiusSize.TopLeft, 0,
                                             bottomLeft: _cardBorderRadiusSize.BottomLeft, bottomRight: 0);
         if (_addTabButton is not null && _itemsPresenter is not null) {
            _addTabButton.Width = _cardSize;
            _addTabButton.Height = _cardSize;
            _addTabButton.HorizontalAlignment = HorizontalAlignment.Right;
         }
      } else {
         CardBorderRadius = new CornerRadius(topLeft: 0, topRight: _cardBorderRadiusSize.TopRight, bottomLeft: 0,
                                             bottomRight: _cardBorderRadiusSize.BottomRight);
         if (_addTabButton is not null && _itemsPresenter is not null) {
            _addTabButton.Width = _cardSize;
            _addTabButton.Height = _cardSize;
            _addTabButton.HorizontalAlignment = HorizontalAlignment.Left;
         }
      }
   }
}