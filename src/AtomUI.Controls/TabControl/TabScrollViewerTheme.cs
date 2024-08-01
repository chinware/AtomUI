﻿using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TabScrollViewerTheme : BaseControlTheme
{
   public const string ScrollStartEdgeIndicatorPart = "Part_ScrollStartEdgeIndicator";
   public const string ScrollEndEdgeIndicatorPart = "Part_ScrollEndEdgeIndicator";
   public const string ScrollMenuIndicatorPart = "Part_ScrollMenuIndicator";
   public const string ScrollViewContentPart = "PART_ContentPresenter";
   public const string ScrollViewLayoutPart = "PART_ScrollViewLayout";
   public const string ScrollViewWrapperLayoutPart = "PART_ScrollViewWrapperLayout";
   
   public TabScrollViewerTheme()
      : base(typeof(TabScrollViewer)) { }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<TabScrollViewer>((scrollViewer, scope) =>
      {
         scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
         var containerLayout = new Panel()
         {
            Name = ScrollViewWrapperLayoutPart
         };
         
         var scrollViewLayout = new DockPanel()
         {
            Name = ScrollViewLayoutPart
         };

         var menuIndicatorIcon = new PathIcon()
         {
            Kind = "EllipsisOutlined",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
         };
         
         TokenResourceBinder.CreateTokenBinding(menuIndicatorIcon, PathIcon.WidthProperty, GlobalResourceKey.IconSize);
         TokenResourceBinder.CreateTokenBinding(menuIndicatorIcon, PathIcon.HeightProperty, GlobalResourceKey.IconSize);
         TokenResourceBinder.CreateTokenBinding(menuIndicatorIcon, PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorTextSecondary);
         
         var menuIndicator = new IconButton()
         {
            Name = ScrollMenuIndicatorPart,
            Icon = menuIndicatorIcon
         };

         menuIndicator.RegisterInNameScope(scope);

         var scrollViewContent = CreateScrollContentPresenter();
         
         DockPanel.SetDock(scrollViewContent, Dock.Left);
         DockPanel.SetDock(menuIndicator, Dock.Right);
         
         scrollViewLayout.Children.Add(menuIndicator);
         scrollViewLayout.Children.Add(scrollViewContent);
         
         scrollViewContent.RegisterInNameScope(scope);
         
         var startEdgeIndicator = new Border()
         {
            Name = ScrollStartEdgeIndicatorPart,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            IsHitTestVisible = false
         };
         startEdgeIndicator.RegisterInNameScope(scope);

         containerLayout.Children.Add(startEdgeIndicator);
         
         var endEdgeIndicator = new Border()
         {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            Name = ScrollEndEdgeIndicatorPart,
            IsHitTestVisible = false
         };
         endEdgeIndicator.RegisterInNameScope(scope);
         
         containerLayout.Children.Add(endEdgeIndicator);
         containerLayout.Children.Add(scrollViewLayout);
         
         return containerLayout;
      });
   }

   private ScrollContentPresenter CreateScrollContentPresenter()
   {
      var scrollViewContent = new TabStripScrollContentPresenter()
      {
         Name = ScrollViewContentPart,
      };
      
      CreateTemplateParentBinding(scrollViewContent, ScrollContentPresenter.MarginProperty,
                                  TabScrollViewer.PaddingProperty);
      CreateTemplateParentBinding(scrollViewContent, ScrollContentPresenter.HorizontalSnapPointsAlignmentProperty,
                                  TabScrollViewer.HorizontalSnapPointsAlignmentProperty);
      CreateTemplateParentBinding(scrollViewContent, ScrollContentPresenter.HorizontalSnapPointsTypeProperty,
                                  TabScrollViewer.HorizontalSnapPointsTypeProperty);
      CreateTemplateParentBinding(scrollViewContent, ScrollContentPresenter.VerticalSnapPointsAlignmentProperty,
                                  TabScrollViewer.VerticalSnapPointsAlignmentProperty);
      CreateTemplateParentBinding(scrollViewContent, ScrollContentPresenter.VerticalSnapPointsTypeProperty,
                                  TabScrollViewer.VerticalSnapPointsTypeProperty);
      var scrollGestureRecognizer = new ScrollGestureRecognizer();
      BindUtils.RelayBind(scrollViewContent, ScrollContentPresenter.CanHorizontallyScrollProperty, scrollGestureRecognizer,
                          ScrollGestureRecognizer.CanHorizontallyScrollProperty);
      BindUtils.RelayBind(scrollViewContent, ScrollContentPresenter.CanVerticallyScrollProperty, scrollGestureRecognizer,
                          ScrollGestureRecognizer.CanVerticallyScrollProperty);
      
      CreateTemplateParentBinding(scrollGestureRecognizer, ScrollGestureRecognizer.IsScrollInertiaEnabledProperty, 
                                  TabScrollViewer.IsScrollInertiaEnabledProperty);
      scrollViewContent.GestureRecognizers.Add(scrollGestureRecognizer);
      
      return scrollViewContent;
   }

   protected override void BuildStyles()
   {
      var topPlacementStyle = new Style(selector => selector.Nesting().PropertyEquals(TabScrollViewer.TabStripPlacementProperty, Dock.Top));
      {
         var contentPresenterStyle =
            new Style(selector => selector.Nesting().Template().OfType<TabStripScrollContentPresenter>());
         contentPresenterStyle.Add(DockPanel.DockProperty, Dock.Left);
         var menuIndicatorStyle =
            new Style(selector => selector.Nesting().Template().Name(ScrollMenuIndicatorPart));
         menuIndicatorStyle.Add(DockPanel.DockProperty, Dock.Right);
         menuIndicatorStyle.Add(IconButton.PaddingProperty, TabControlResourceKey.MenuIndicatorPaddingHorizontal);

         var startEdgeIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(ScrollStartEdgeIndicatorPart));
         startEdgeIndicatorStyle.Add(Control.WidthProperty, TabControlResourceKey.MenuEdgeThickness);
         topPlacementStyle.Add(startEdgeIndicatorStyle);
         
         var endEdgeIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(ScrollEndEdgeIndicatorPart));
         endEdgeIndicatorStyle.Add(Control.WidthProperty, TabControlResourceKey.MenuEdgeThickness);
         topPlacementStyle.Add(endEdgeIndicatorStyle);
         
         topPlacementStyle.Add(menuIndicatorStyle);
         topPlacementStyle.Add(contentPresenterStyle);
      }
      
      Add(topPlacementStyle);

      var rightPlacementStyle = new Style(selector => selector.Nesting().PropertyEquals(TabScrollViewer.TabStripPlacementProperty, Dock.Right));
      
      {
         var contentPresenterStyle =
            new Style(selector => selector.Nesting().Template().OfType<TabStripScrollContentPresenter>());
         contentPresenterStyle.Add(DockPanel.DockProperty, Dock.Top);
         var menuIndicatorStyle =
            new Style(selector => selector.Nesting().Template().Name(ScrollMenuIndicatorPart));
         menuIndicatorStyle.Add(DockPanel.DockProperty, Dock.Bottom);
         menuIndicatorStyle.Add(IconButton.PaddingProperty, TabControlResourceKey.MenuIndicatorPaddingVertical);
         
         var startEdgeIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(ScrollStartEdgeIndicatorPart));
         startEdgeIndicatorStyle.Add(Control.HeightProperty, TabControlResourceKey.MenuEdgeThickness);
         topPlacementStyle.Add(startEdgeIndicatorStyle);
         
         var endEdgeIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(ScrollStartEdgeIndicatorPart));
         endEdgeIndicatorStyle.Add(Control.HeightProperty, TabControlResourceKey.MenuEdgeThickness);
         topPlacementStyle.Add(endEdgeIndicatorStyle);
      
         rightPlacementStyle.Add(menuIndicatorStyle);
         rightPlacementStyle.Add(contentPresenterStyle);
      }
      
      Add(rightPlacementStyle);

      var bottomPlacementStyle = new Style(selector => selector.Nesting().PropertyEquals(TabScrollViewer.TabStripPlacementProperty, Dock.Bottom));

      {
         var contentPresenterStyle =
            new Style(selector => selector.Nesting().Template().OfType<TabStripScrollContentPresenter>());
         contentPresenterStyle.Add(DockPanel.DockProperty, Dock.Left);
         var menuIndicatorStyle =
            new Style(selector => selector.Nesting().Template().Name(ScrollMenuIndicatorPart));
         menuIndicatorStyle.Add(DockPanel.DockProperty, Dock.Right);
         menuIndicatorStyle.Add(IconButton.PaddingProperty, TabControlResourceKey.MenuIndicatorPaddingHorizontal);
         
         var startEdgeIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(ScrollStartEdgeIndicatorPart));
         startEdgeIndicatorStyle.Add(Control.WidthProperty, TabControlResourceKey.MenuEdgeThickness);
         topPlacementStyle.Add(startEdgeIndicatorStyle);
         
         var endEdgeIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(ScrollStartEdgeIndicatorPart));
         endEdgeIndicatorStyle.Add(Control.WidthProperty, TabControlResourceKey.MenuEdgeThickness);
         topPlacementStyle.Add(endEdgeIndicatorStyle);
      
         bottomPlacementStyle.Add(menuIndicatorStyle);
         bottomPlacementStyle.Add(contentPresenterStyle);
      }
      
      Add(bottomPlacementStyle);
      
      var leftPlacementStyle = new Style(selector => selector.Nesting().PropertyEquals(TabScrollViewer.TabStripPlacementProperty, Dock.Left));
      
      {
         var contentPresenterStyle =
            new Style(selector => selector.Nesting().Template().OfType<TabStripScrollContentPresenter>());
         contentPresenterStyle.Add(DockPanel.DockProperty, Dock.Top);
         var menuIndicatorStyle =
            new Style(selector => selector.Nesting().Template().Name(ScrollMenuIndicatorPart));
         menuIndicatorStyle.Add(DockPanel.DockProperty, Dock.Bottom);
         menuIndicatorStyle.Add(IconButton.PaddingProperty, TabControlResourceKey.MenuIndicatorPaddingVertical);
         
         var startEdgeIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(ScrollStartEdgeIndicatorPart));
         startEdgeIndicatorStyle.Add(Control.HeightProperty, TabControlResourceKey.MenuEdgeThickness);
         topPlacementStyle.Add(startEdgeIndicatorStyle);
         
         var endEdgeIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(ScrollStartEdgeIndicatorPart));
         endEdgeIndicatorStyle.Add(Control.HeightProperty, TabControlResourceKey.MenuEdgeThickness);
         topPlacementStyle.Add(endEdgeIndicatorStyle);
      
         leftPlacementStyle.Add(menuIndicatorStyle);
         leftPlacementStyle.Add(contentPresenterStyle);
      }

      Add(leftPlacementStyle);
   }
}