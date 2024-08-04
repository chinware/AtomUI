using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal class TabsContainerPanel : Panel
{
   #region 公共属性定义

   public static readonly DirectProperty<TabsContainerPanel, BaseTabScrollViewer?> TabScrollViewerProperty =
      AvaloniaProperty.RegisterDirect<TabsContainerPanel, BaseTabScrollViewer?>(nameof(TabScrollViewer),
         o => o.TabScrollViewer,
         (o, v) => o.TabScrollViewer = v);

   private BaseTabScrollViewer? _tabScrollViewer;
   public BaseTabScrollViewer? TabScrollViewer
   {
      get => _tabScrollViewer;
      set => SetAndRaise(TabScrollViewerProperty, ref _tabScrollViewer, value);
   }
   
   public static readonly DirectProperty<TabsContainerPanel, IconButton?> AddTabButtonProperty =
      AvaloniaProperty.RegisterDirect<TabsContainerPanel, IconButton?>(nameof(AddTabButton),
                                                                       o => o.AddTabButton,
                                                                       (o, v) => o.AddTabButton = v);

   private IconButton? _addTabButton;
   public IconButton? AddTabButton
   {
      get => _addTabButton;
      set => SetAndRaise(AddTabButtonProperty, ref _addTabButton, value);
   }
   
   #endregion

   #region 内部属性定义

   internal static readonly DirectProperty<TabsContainerPanel, Dock> TabStripPlacementProperty =
      AvaloniaProperty.RegisterDirect<TabsContainerPanel, Dock>(nameof(TabStripPlacement),
                                                                o => o.TabStripPlacement,
                                                                (o, v) => o.TabStripPlacement = v);
   
   private Dock _tabStripPlacement;

   internal Dock TabStripPlacement
   {
      get => _tabStripPlacement;
      set => SetAndRaise(TabStripPlacementProperty, ref _tabStripPlacement, value);
   }

   #endregion

   static TabsContainerPanel()
   {
      AffectsMeasure<TabsContainerPanel>(TabScrollViewerProperty, AddTabButtonProperty);
      AffectsArrange<TabsContainerPanel>(TabStripPlacementProperty);
   }
   
   protected override Size ArrangeOverride(Size arrangeSize)
   {
      // TODO 暂时不做验证，默认认为两个元素都存在
      // 理论上这里要报错，但是我们是内部使用
      if (_tabScrollViewer is not null && _addTabButton is not null) {
         if (TabStripPlacement == Dock.Top || TabStripPlacement == Dock.Bottom) {
            var scrollViewerDesiredWidth = _tabScrollViewer.DesiredSize.Width;
            var addTabButtonDesiredWidth = _addTabButton.DesiredSize.Width;
            var totalDesiredWidth = scrollViewerDesiredWidth + addTabButtonDesiredWidth;
            if (totalDesiredWidth > arrangeSize.Width) {
               _tabScrollViewer.Arrange(new Rect(new Point(0, 0), new Size(arrangeSize.Width - addTabButtonDesiredWidth, arrangeSize.Height)));
               _addTabButton.Arrange(new Rect(new Point(arrangeSize.Width - addTabButtonDesiredWidth, 0), _addTabButton.DesiredSize));
            } else {
               _tabScrollViewer.Arrange(new Rect(new Point(0, 0), _tabScrollViewer.DesiredSize));
               _addTabButton.Arrange(new Rect(new Point(scrollViewerDesiredWidth, 0), _addTabButton.DesiredSize));
            }
         } else {
            var scrollViewerDesiredHeight = _tabScrollViewer.DesiredSize.Height;
            var addTabButtonDesiredHeight = _addTabButton.DesiredSize.Height;
            var totalDesiredHeight = scrollViewerDesiredHeight + addTabButtonDesiredHeight;
            if (totalDesiredHeight > arrangeSize.Height) {
               _tabScrollViewer.Arrange(new Rect(new Point(0, 0), new Size(arrangeSize.Width, arrangeSize.Height - addTabButtonDesiredHeight)));
               _addTabButton.Arrange(new Rect(new Point(0, arrangeSize.Width - addTabButtonDesiredHeight), _addTabButton.DesiredSize));
            } else {
               _tabScrollViewer.Arrange(new Rect(new Point(0, 0), _tabScrollViewer.DesiredSize));
               _addTabButton.Arrange(new Rect(new Point(scrollViewerDesiredHeight, 0), _addTabButton.DesiredSize));
            }
         }
      }
      
      return (arrangeSize);
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      if (change.Property == TabScrollViewerProperty) {
         var oldScrollViewer = change.GetOldValue<BaseTabScrollViewer?>();
         if (oldScrollViewer is not null) {
            Children.Remove(oldScrollViewer);
         }

         if (TabScrollViewer is not null) {
            Children.Add(TabScrollViewer);
         }
      } else if (change.Property == AddTabButtonProperty) {
         var oldAddTabButton = change.GetOldValue<IconButton?>();
         if (oldAddTabButton is not null) {
            Children.Remove(oldAddTabButton);
         }

         if (AddTabButton is not null) {
            Children.Add(AddTabButton);
         }
      }
   }
}