using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Styling;
using ExCSS;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TimelineTheme : BaseControlTheme
{
   public const string FrameDecoratorPart = "PART_FrameDecorator";
   public const string ScrollViewerPart = "PART_ScrollViewer";
   public const string ItemsPresenterPart = "PART_ItemsPresenter";

   public TimelineTheme() : this(typeof(Timeline)) { }
   protected TimelineTheme(Type targetType) : base(targetType) { }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<Timeline>((timeline, scope) =>
      {
         var frameBorder = new Border()
         {
            Name = FrameDecoratorPart
         };
         
         var itemsPresenter = BuildItemsPresenter(timeline, scope);
         var scrollViewer = BuildScrollViewer(timeline, scope);

         scrollViewer.Content = itemsPresenter;
         frameBorder.Child = scrollViewer;

         return frameBorder;
      });
   }

   private ScrollViewer BuildScrollViewer(Timeline timeline, INameScope scope)
   {
      var scrollViewer = new ScrollViewer
      {
         Name = ScrollViewerPart
      };

      return scrollViewer;
   }

   private ItemsPresenter BuildItemsPresenter(Timeline timeline, INameScope scope)
   {
      var itemsPresenter = new ItemsPresenter
      {
         Name = ItemsPresenterPart,
      };
      itemsPresenter.RegisterInNameScope(scope);
      return itemsPresenter;
   }

   protected override void BuildStyles()
   {
      BuildFixedStyle();
      BuildCommonStyle();
   }
   private void BuildFixedStyle()
   {
      this.Add(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
      this.Add(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
      this.Add(ScrollViewer.IsScrollChainingEnabledProperty, true);
   }

   private void BuildCommonStyle()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(Border.BorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
      var frameStyle = new Style(selector => selector.Nesting().Template().Name(FrameDecoratorPart));
      frameStyle.Add(Border.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
      commonStyle.Add(frameStyle);
      Add(commonStyle);
   }
}