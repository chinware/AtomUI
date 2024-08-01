using System.Globalization;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Colors = Avalonia.Media.Colors;
using GradientStop = Avalonia.Media.GradientStop;

namespace AtomUI.Controls;

[TemplatePart(TabScrollViewerTheme.ScrollStartEdgeIndicatorPart, typeof(Control))]
[TemplatePart(TabScrollViewerTheme.ScrollEndEdgeIndicatorPart, typeof(Control))]
[TemplatePart(TabScrollViewerTheme.ScrollMenuIndicatorPart, typeof(IconButton))]
[TemplatePart(TabScrollViewerTheme.ScrollViewContentPart, typeof(ScrollContentPresenter))]
public class TabScrollViewer : ScrollViewer
{
   private const int EdgeIndicatorZIndex = 1000;

   #region 内部属性定义

   internal static readonly DirectProperty<TabScrollViewer, Dock> TabStripPlacementProperty =
      AvaloniaProperty.RegisterDirect<TabScrollViewer, Dock>(nameof(TabStripPlacement),
                                                             o => o.TabStripPlacement,
                                                             (o, v) => o.TabStripPlacement = v);
   
   internal static readonly DirectProperty<TabScrollViewer, IBrush?> EdgeShadowStartColorProperty =
      AvaloniaProperty.RegisterDirect<TabScrollViewer, IBrush?>(nameof(EdgeShadowStartColor),
                                                                o => o.EdgeShadowStartColor,
                                                                (o, v) => o.EdgeShadowStartColor = v);
   
   internal static readonly DirectProperty<TabScrollViewer, double> MenuEdgeThicknessProperty =
      AvaloniaProperty.RegisterDirect<TabScrollViewer, double>(nameof(MenuEdgeThickness),
                                                               o => o.MenuEdgeThickness,
                                                               (o, v) => o.MenuEdgeThickness = v);

   private Dock _tabStripPlacement;

   internal Dock TabStripPlacement
   {
      get => _tabStripPlacement;
      set => SetAndRaise(TabStripPlacementProperty, ref _tabStripPlacement, value);
   }
   
   private IBrush? _edgeShadowStartColor;

   internal IBrush? EdgeShadowStartColor
   {
      get => _edgeShadowStartColor;
      set => SetAndRaise(EdgeShadowStartColorProperty, ref _edgeShadowStartColor, value);
   }
   
   private double _menuEdgeThickness;

   internal double MenuEdgeThickness
   {
      get => _menuEdgeThickness;
      set => SetAndRaise(MenuEdgeThicknessProperty, ref _menuEdgeThickness, value);
   }
   
   internal BaseTabStrip? TabStrip { get; set; } 
   
   #endregion

   private IconButton? _menuIndicator;
   private Border? _startEdgeIndicator;
   private Border? _endEdgeIndicator;
   private MenuFlyout? _menuFlyout;

   static TabScrollViewer()
   {
      AffectsMeasure<TabScrollViewer>(TabStripPlacementProperty);
   }

   public TabScrollViewer()
   {
      HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      if (change.Property == TabStripPlacementProperty) {
         if (Presenter is TabStripScrollContentPresenter tabStripScrollContentPresenter) {
            tabStripScrollContentPresenter.TabStripPlacement = TabStripPlacement;
         }
      } else if (change.Property == VerticalScrollBarVisibilityProperty ||
                 change.Property == OffsetProperty ||
                 change.Property == ExtentProperty ||
                 change.Property == ViewportProperty) {
         SetupIndicatorsVisibility();
      }
   }

   private void SetupEdgeIndicatorSize()
   {
      if (Presenter is not null) {
         if (TabStripPlacement == Dock.Top || TabStripPlacement == Dock.Bottom) {
            if (_startEdgeIndicator is not null) {
               _startEdgeIndicator.Height = Presenter.DesiredSize.Height;
               _startEdgeIndicator.ZIndex = EdgeIndicatorZIndex;
               _startEdgeIndicator.Background = BuildEdgeIndicatorBrush(TabStripPlacement, true);
            }

            if (_endEdgeIndicator is not null) {
               _endEdgeIndicator.Height = Presenter.DesiredSize.Height;
               _endEdgeIndicator.Margin = new Thickness(0, 0, _menuEdgeThickness, 0);
               _endEdgeIndicator.ZIndex = EdgeIndicatorZIndex;
               _endEdgeIndicator.Background = BuildEdgeIndicatorBrush(TabStripPlacement, false);
            }
         } else {
            if (_startEdgeIndicator is not null) {
               _startEdgeIndicator.Width = Presenter.DesiredSize.Width;
               _startEdgeIndicator.ZIndex = EdgeIndicatorZIndex;
               _startEdgeIndicator.Background = BuildEdgeIndicatorBrush(TabStripPlacement, false);
            }

            if (_endEdgeIndicator is not null) {
               _endEdgeIndicator.Width = Presenter.DesiredSize.Width;
               _endEdgeIndicator.Margin = new Thickness(0, _menuEdgeThickness, 0, 0);
               _endEdgeIndicator.ZIndex = EdgeIndicatorZIndex;
               _endEdgeIndicator.Background = BuildEdgeIndicatorBrush(TabStripPlacement, false);
            }
         }
      }
   }

   private LinearGradientBrush BuildEdgeIndicatorBrush(Dock placement, bool isStart)
   {
      var linearGradientBrush = new LinearGradientBrush()
      {
         GradientStops =
         {
            new GradientStop((_edgeShadowStartColor as SolidColorBrush)!.Color, 0),
            new GradientStop(Colors.Transparent, 1),
         }
      };
      if (placement == Dock.Top || placement == Dock.Bottom) {
         if (isStart) {
            linearGradientBrush.StartPoint = new RelativePoint(0, 0.5, RelativeUnit.Relative);
            linearGradientBrush.EndPoint = new RelativePoint(1, 0.5, RelativeUnit.Relative);
         } else {
            linearGradientBrush.StartPoint = new RelativePoint(1, 0.5, RelativeUnit.Relative);
            linearGradientBrush.EndPoint = new RelativePoint(0, 0.5, RelativeUnit.Relative);
         }
      } else {
         if (isStart) {
            linearGradientBrush.StartPoint = new RelativePoint(0.5, 0, RelativeUnit.Relative);
            linearGradientBrush.EndPoint = new RelativePoint(0.5, 1, RelativeUnit.Relative);
         } else {
            linearGradientBrush.StartPoint = new RelativePoint(0.5, 1, RelativeUnit.Relative);
            linearGradientBrush.EndPoint = new RelativePoint(0.5, 0, RelativeUnit.Relative);
         }
      }
      return linearGradientBrush;
   }

   protected override bool RegisterContentPresenter(ContentPresenter presenter)
   {
      if (presenter is TabStripScrollContentPresenter tabStripScrollContentPresenter) {
         tabStripScrollContentPresenter.TabStripPlacement = TabStripPlacement;
      }

      return base.RegisterContentPresenter(presenter);
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _menuIndicator = e.NameScope.Find<IconButton>(TabScrollViewerTheme.ScrollMenuIndicatorPart);
      _startEdgeIndicator = e.NameScope.Find<Border>(TabScrollViewerTheme.ScrollStartEdgeIndicatorPart);
      _endEdgeIndicator = e.NameScope.Find<Border>(TabScrollViewerTheme.ScrollEndEdgeIndicatorPart);
      
      TokenResourceBinder.CreateTokenBinding(this, EdgeShadowStartColorProperty, GlobalResourceKey.ColorFillSecondary);
      TokenResourceBinder.CreateTokenBinding(this, MenuEdgeThicknessProperty, TabControlResourceKey.MenuEdgeThickness);

      if (_menuIndicator is not null) {
         _menuIndicator.Click += HandleMenuIndicator;
      }
      
      SetupIndicatorsVisibility();
   }

   private void HandleMenuIndicator(object? sender, RoutedEventArgs args)
   {
      if (_menuFlyout is null) {
         _menuFlyout = new MenuFlyout();
      }

      if (TabStripPlacement == Dock.Top || TabStripPlacement == Dock.Bottom) {
         _menuFlyout.Placement = PlacementMode.Bottom;
      } else {
         _menuFlyout.Placement = PlacementMode.Right;
      }

      // 收集没有完全显示的 Tab 列表
      _menuFlyout.Items.Clear();
      if (TabStrip is not null) {
         for (int i = 0; i < TabStrip.ItemCount; i++) {
            var itemContainer = TabStrip.ContainerFromIndex(i)!;
            if (itemContainer is TabStripItem tabStripItem) {
               var itemBounds = itemContainer.Bounds;
               var left = Math.Floor(itemBounds.Left - Offset.X);
               var right = Math.Floor(itemBounds.Right - Offset.X);
               if (TabStripPlacement == Dock.Top || TabStripPlacement == Dock.Bottom) {
                  if (left < 0 || right > Viewport.Width) {
                     var menuItem = new MenuItem()
                     {
                        Header = tabStripItem.Content,
                     };
                     menuItem.Click += HandleMenuItemClicked;
                     _menuFlyout.Items.Add(menuItem);
                  }
               }
            }
         }

         if (_menuFlyout.Items.Count > 0) {
            _menuFlyout.ShowAt(_menuIndicator!);
         }
      }
   }

   private void HandleMenuItemClicked(object? sender, RoutedEventArgs args)
   {
      if (TabStrip is not null) {
         TabStrip.BringIntoView();
         Dispatcher.UIThread.Post(sender =>
         {
            var item = TabStrip.Items[5];
            if (item is TabStripItem tabStripItem) {
               tabStripItem.BringIntoView();
            }
          }, sender);
      }
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      var size = base.MeasureOverride(availableSize);
      SetupEdgeIndicatorSize();
      return size;
   }

   private void SetupIndicatorsVisibility()
   {
      var args = new List<object?>();

      object? scrollUpVisibility = default;
      object? scrollDownVisibility = default;
      if (TabStripPlacement == Dock.Top || TabStripPlacement == Dock.Bottom) {
         args.Add(ScrollBarVisibility.Auto);
         args.Add(Offset.X);
         args.Add(Extent.Width);
         args.Add(Viewport.Width);
         scrollUpVisibility =
            MenuScrollingVisibilityConverter.Instance.Convert(args, typeof(bool), 0d, CultureInfo.CurrentCulture);
         scrollDownVisibility =
            MenuScrollingVisibilityConverter.Instance.Convert(args, typeof(bool), 100d, CultureInfo.CurrentCulture);
      } else {
         args.Add(ScrollBarVisibility.Auto);
         args.Add(Offset.Y);
         args.Add(Extent.Height);
         args.Add(Viewport.Height);
         scrollUpVisibility =
            MenuScrollingVisibilityConverter.Instance.Convert(args, typeof(bool), 0d, CultureInfo.CurrentCulture);
         scrollDownVisibility =
            MenuScrollingVisibilityConverter.Instance.Convert(args, typeof(bool), 100d, CultureInfo.CurrentCulture);
      }

      if (_startEdgeIndicator is not null &&
          scrollUpVisibility is not null &&
          scrollUpVisibility != AvaloniaProperty.UnsetValue) {
         _startEdgeIndicator.IsVisible = (bool)scrollUpVisibility;
      }

      if (_endEdgeIndicator is not null &&
          scrollDownVisibility is not null &&
          scrollDownVisibility != AvaloniaProperty.UnsetValue) {
         _endEdgeIndicator.IsVisible = (bool)scrollDownVisibility;
      }

      if (_menuIndicator is not null) {
         var startEdgeVisible = _startEdgeIndicator?.IsVisible ?? false;
         var endEdgeVisible = _endEdgeIndicator?.IsVisible ?? false;
         _menuIndicator.IsVisible = startEdgeVisible || endEdgeVisible;
      }
   }
}