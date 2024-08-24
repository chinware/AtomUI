using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
public class TopLevelMenuItemTheme : BaseControlTheme
{
   public const string ID = "TopLevelMenuItem";
   
   public const string PopupPart           = "PART_Popup";
   public const string HeaderPresenterPart = "PART_HeaderPresenter";
   public const string ItemsPresenterPart  = "PART_ItemsPresenter";
   
   public TopLevelMenuItemTheme() : base(typeof(MenuItem)) {}
   
   public override string? ThemeResourceKey()
   {
      return ID;
   }

   protected override IControlTemplate? BuildControlTemplate()
   {
      return new FuncControlTemplate<MenuItem>((menuItem, scope) =>
      {
         var panel = new Panel();
         var contentPresenter = new ContentPresenter()
         {
            Name = HeaderPresenterPart,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center,
            RecognizesAccessKey = true,
         };
         
         // TODO 后面需要评估一下，能直接绑定到对象，是否还需要这样通过模板绑定
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, MenuItem.HeaderProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty, MenuItem.HeaderTemplateProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.CornerRadiusProperty, MenuItem.CornerRadiusProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.PaddingProperty, MenuItem.PaddingProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.MinHeightProperty, MenuItem.MinHeightProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.FontSizeProperty, MenuItem.FontSizeProperty);
         
         contentPresenter.RegisterInNameScope(scope);
         panel.Children.Add(contentPresenter);

         var popup = CreateMenuPopup();
         popup.RegisterInNameScope(scope);
         panel.Children.Add(popup);
         return panel;
      });
   }

   private Popup CreateMenuPopup()
   {
      var popup = new Popup()
      {
         Name = PopupPart,
         WindowManagerAddShadowHint = false,
         IsLightDismissEnabled = false,
         Placement = PlacementMode.BottomEdgeAlignedLeft,
      };
      
      var border = new Border();
      
      TokenResourceBinder.CreateTokenBinding(border, Border.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
      TokenResourceBinder.CreateTokenBinding(border, Border.CornerRadiusProperty, MenuTokenResourceKey.MenuPopupBorderRadius);
      TokenResourceBinder.CreateTokenBinding(border, Border.MinWidthProperty, MenuTokenResourceKey.MenuPopupMinWidth);
      TokenResourceBinder.CreateTokenBinding(border, Border.MaxWidthProperty, MenuTokenResourceKey.MenuPopupMaxWidth);
      TokenResourceBinder.CreateTokenBinding(border, Border.MinHeightProperty, MenuTokenResourceKey.MenuPopupMinHeight);
      TokenResourceBinder.CreateTokenBinding(border, Border.MaxHeightProperty, MenuTokenResourceKey.MenuPopupMaxHeight);
      TokenResourceBinder.CreateTokenBinding(border, Border.PaddingProperty, MenuTokenResourceKey.MenuPopupContentPadding);

      var scrollViewer = new MenuScrollViewer();
      var itemsPresenter = new ItemsPresenter
      {
         Name = ItemsPresenterPart,
      };
      CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, MenuItem.ItemsPanelProperty);
      Grid.SetIsSharedSizeScope(itemsPresenter, true);
      KeyboardNavigation.SetTabNavigation(itemsPresenter, KeyboardNavigationMode.Continue);
      scrollViewer.Content = itemsPresenter;
      border.Child = scrollViewer;
      popup.Child = border;

      TokenResourceBinder.CreateTokenBinding(popup, Popup.MarginToAnchorProperty, MenuTokenResourceKey.TopLevelItemPopupMarginToAnchor);
      TokenResourceBinder.CreateTokenBinding(popup, Popup.MaskShadowsProperty, MenuTokenResourceKey.MenuPopupBoxShadows);
      
      CreateTemplateParentBinding(popup, Popup.IsOpenProperty, MenuItem.IsSubMenuOpenProperty, BindingMode.TwoWay);
      
      return popup;
   }

   protected override void BuildStyles()
   {
      var topLevelStyle = new Style(selector => selector.Nesting().Class(MenuItem.TopLevelPC));
      BuildCommonStyle(topLevelStyle);
      BuildSizeTypeStyle(topLevelStyle);
      BuildDisabledStyle(topLevelStyle);
      Add(topLevelStyle);
   }

   private void BuildCommonStyle(Style topLevelStyle)
   {
      var commonStyle = new Style(selector => selector.Nesting().PropertyEquals(MenuItem.IsEnabledProperty, true));
      commonStyle.Add(MenuItem.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
      
      // 正常状态
      {
         var contentPresenterStyle = new Style(selector => selector.Nesting().Template().OfType<ContentPresenter>().Name(HeaderPresenterPart));
         contentPresenterStyle.Add(ContentPresenter.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
         commonStyle.Add(contentPresenterStyle);
      }
      
      // hover 状态
      var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      {
         var contentPresenterHoverStyle = new Style(selector => selector.Nesting().Template().OfType<ContentPresenter>().Name(HeaderPresenterPart));
         contentPresenterHoverStyle.Add(ContentPresenter.BackgroundProperty, MenuTokenResourceKey.TopLevelItemHoverBg);
         contentPresenterHoverStyle.Add(ContentPresenter.ForegroundProperty, MenuTokenResourceKey.TopLevelItemHoverColor);
         contentPresenterHoverStyle.Add(ContentPresenter.CursorProperty, new Cursor(StandardCursorType.Hand));
         hoverStyle.Add(contentPresenterHoverStyle);
      }
      commonStyle.Add(hoverStyle);
      
      // 在打开状态下的
      var openedStyle = new Style(selector => selector.Nesting().PropertyEquals(MenuItem.IsSubMenuOpenProperty, true));
      {
         var contentPresenterHoverStyle = new Style(selector => selector.Nesting().Template().OfType<ContentPresenter>().Name(HeaderPresenterPart));
         contentPresenterHoverStyle.Add(ContentPresenter.BackgroundProperty, MenuTokenResourceKey.TopLevelItemHoverBg);
         contentPresenterHoverStyle.Add(ContentPresenter.ForegroundProperty, MenuTokenResourceKey.TopLevelItemHoverColor);
         contentPresenterHoverStyle.Add(ContentPresenter.CursorProperty, new Cursor(StandardCursorType.Hand));
         openedStyle.Add(contentPresenterHoverStyle);
      }
      
      commonStyle.Add(openedStyle);
      
      topLevelStyle.Add(commonStyle);
   }
   
   private void BuildSizeTypeStyle(Style topLevelStyle)
   {
      var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(MenuItem.SizeTypeProperty, SizeType.Large));
      largeSizeStyle.Add(MenuItem.CornerRadiusProperty, MenuTokenResourceKey.TopLevelItemBorderRadiusLG);
      largeSizeStyle.Add(MenuItem.MinHeightProperty, GlobalTokenResourceKey.ControlHeightLG);
      largeSizeStyle.Add(MenuItem.PaddingProperty, MenuTokenResourceKey.TopLevelItemPaddingLG);
      largeSizeStyle.Add(MenuItem.FontSizeProperty, MenuTokenResourceKey.TopLevelItemFontSizeLG);
      {
         var presenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
        
         presenterStyle.Add(ContentPresenter.LineHeightProperty, MenuTokenResourceKey.TopLevelItemLineHeightLG);
         largeSizeStyle.Add(presenterStyle);
      }
      topLevelStyle.Add(largeSizeStyle);
      
      var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(MenuItem.SizeTypeProperty, SizeType.Middle));
      middleSizeStyle.Add(MenuItem.CornerRadiusProperty, MenuTokenResourceKey.TopLevelItemBorderRadius);
      middleSizeStyle.Add(MenuItem.MinHeightProperty, GlobalTokenResourceKey.ControlHeight);
      middleSizeStyle.Add(MenuItem.PaddingProperty, MenuTokenResourceKey.TopLevelItemPadding);
      middleSizeStyle.Add(MenuItem.FontSizeProperty, MenuTokenResourceKey.TopLevelItemFontSize);
      {
         var presenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
         presenterStyle.Add(ContentPresenter.LineHeightProperty, MenuTokenResourceKey.TopLevelItemLineHeight);
         middleSizeStyle.Add(presenterStyle);
      }
      topLevelStyle.Add(middleSizeStyle);

      var smallSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(MenuItem.SizeTypeProperty, SizeType.Small));
      smallSizeStyle.Add(MenuItem.CornerRadiusProperty, MenuTokenResourceKey.TopLevelItemBorderRadiusSM);
      smallSizeStyle.Add(MenuItem.MinHeightProperty, GlobalTokenResourceKey.ControlHeightSM);
      smallSizeStyle.Add(MenuItem.PaddingProperty, MenuTokenResourceKey.TopLevelItemPaddingSM);
      smallSizeStyle.Add(MenuItem.FontSizeProperty, MenuTokenResourceKey.TopLevelItemFontSizeSM);
      {
         var presenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
         presenterStyle.Add(ContentPresenter.LineHeightProperty, MenuTokenResourceKey.TopLevelItemLineHeightSM);
         smallSizeStyle.Add(presenterStyle);
      }
      topLevelStyle.Add(smallSizeStyle);
   }

   private void BuildDisabledStyle(Style topLevelStyle)
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Add(MenuItem.ForegroundProperty, MenuTokenResourceKey.ItemDisabledColor);
      topLevelStyle.Add(disabledStyle);
   }
}