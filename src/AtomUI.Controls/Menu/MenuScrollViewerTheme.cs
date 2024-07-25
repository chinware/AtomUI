using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Input.GestureRecognizers;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class MenuScrollViewerTheme : ControlTheme
{
   public const string ScrollUpButtonPart = "Part_ScrollUpButton";
   public const string ScrollDownButtonPart = "Part_ScrollDownButton";
   public const string ScrollViewContentPart = "PART_ContentPresenter";
   public const string MainContainerPart = "Part_MainContainer";

   public MenuScrollViewerTheme()
      : base(typeof(MenuScrollViewer)) { }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<MenuScrollViewer>((viewer, scope) =>
      {
         var dockPanel = new DockPanel();
         var scrollUpButton = new IconButton()
         {
            Name = ScrollUpButtonPart,
            Width = 16,
            Height = 16
         };
         DockPanel.SetDock(scrollUpButton, Dock.Top);
         var scrollDownButton = new IconButton()
         {
            Name = ScrollDownButtonPart,
            Width = 16,
            Height = 16
         };
         DockPanel.SetDock(scrollDownButton, Dock.Bottom);

         var scrollViewContent = CreateScrollContentPresenter(viewer);

         dockPanel.Children.Add(scrollUpButton);
         dockPanel.Children.Add(scrollDownButton);
         dockPanel.Children.Add(scrollViewContent);
         scrollUpButton.RegisterInNameScope(scope);
         scrollDownButton.RegisterInNameScope(scope);
         scrollViewContent.RegisterInNameScope(scope);
         return dockPanel;
      });
   }

   private ScrollContentPresenter CreateScrollContentPresenter(MenuScrollViewer viewer)
   {
      var scrollViewContent = new ScrollContentPresenter()
      {
         Name = ScrollViewContentPart
      };
      CreateTemplateParentBinding(scrollViewContent, ScrollContentPresenter.MarginProperty,
                                  MenuScrollViewer.PaddingProperty);
      CreateTemplateParentBinding(scrollViewContent, ScrollContentPresenter.HorizontalSnapPointsAlignmentProperty,
                                  MenuScrollViewer.HorizontalSnapPointsAlignmentProperty);
      CreateTemplateParentBinding(scrollViewContent, ScrollContentPresenter.HorizontalSnapPointsTypeProperty,
                                  MenuScrollViewer.HorizontalSnapPointsTypeProperty);
      CreateTemplateParentBinding(scrollViewContent, ScrollContentPresenter.VerticalSnapPointsAlignmentProperty,
                                  MenuScrollViewer.VerticalSnapPointsAlignmentProperty);
      CreateTemplateParentBinding(scrollViewContent, ScrollContentPresenter.VerticalSnapPointsTypeProperty,
                                  MenuScrollViewer.VerticalSnapPointsTypeProperty);
      var scrollGestureRecognizer = new ScrollGestureRecognizer();
      BindUtils.RelayBind(scrollViewContent, ScrollContentPresenter.CanHorizontallyScrollProperty, scrollGestureRecognizer,
                          ScrollGestureRecognizer.CanHorizontallyScrollProperty);
      BindUtils.RelayBind(scrollViewContent, ScrollContentPresenter.CanVerticallyScrollProperty, scrollGestureRecognizer,
                          ScrollGestureRecognizer.CanVerticallyScrollProperty);

      CreateTemplateParentBinding(scrollGestureRecognizer, ScrollGestureRecognizer.IsScrollInertiaEnabledProperty, 
                                  MenuScrollViewer.IsScrollInertiaEnabledProperty);
      scrollViewContent.GestureRecognizers.Add(scrollGestureRecognizer);

      return scrollViewContent;
   }

   protected override void BuildStyles()
   {
      
   }
}