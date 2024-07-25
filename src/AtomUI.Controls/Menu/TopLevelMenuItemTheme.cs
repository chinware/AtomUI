using AtomUI.Styling;
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
public class TopLevelMenuItemTheme : ControlTheme
{
   public const string ID = "TopLevelMenuItem";
   
   public const string PopupPart = "PART_Popup";
   public const string HeaderPresenterPart = "PART_HeaderPresenter";
   public const string ItemsPresenterPart = "PART_ItemsPresenter";
   
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
            RecognizesAccessKey = true
         };
         
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, MenuItem.HeaderProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty, MenuItem.HeaderTemplateProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.CornerRadiusProperty, MenuItem.CornerRadiusProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.PaddingProperty, MenuItem.PaddingProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.MinHeightProperty, MenuItem.MinHeightProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.FontSizeProperty, MenuItem.FontSizeProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.BackgroundProperty, MenuItem.BackgroundProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ForegroundProperty, MenuItem.ForegroundProperty);
         contentPresenter.RegisterInNameScope(scope);
         panel.Children.Add(contentPresenter);

         var popup = CreateMenuPopup(menuItem);
         panel.Children.Add(popup);
         return panel;
      });
   }

   private Popup CreateMenuPopup(MenuItem menuItem)
   {

      var popup = new Popup()
      {
         Name = PopupPart,
         WindowManagerAddShadowHint = false,
         IsLightDismissEnabled = true,
         Placement = PlacementMode.BottomEdgeAlignedLeft,
         OverlayInputPassThroughElement = menuItem,
      };
      
      var border = new Border();
      BindUtils.CreateTokenBinding(border, Border.BackgroundProperty, GlobalResourceKey.ColorBgContainer);
      BindUtils.CreateTokenBinding(border, Border.CornerRadiusProperty, MenuResourceKey.MenuPopupBorderRadius);
      BindUtils.CreateTokenBinding(border, Border.MinWidthProperty, MenuResourceKey.MenuPopupMinWidth);
      BindUtils.CreateTokenBinding(border, Border.MaxWidthProperty, MenuResourceKey.MenuPopupMaxWidth);
      BindUtils.CreateTokenBinding(border, Border.MinHeightProperty, MenuResourceKey.MenuPopupMinHeight);
      BindUtils.CreateTokenBinding(border, Border.MaxHeightProperty, MenuResourceKey.MenuPopupMaxHeight);
      BindUtils.CreateTokenBinding(border, Border.PaddingProperty, MenuResourceKey.MenuPopupContentPadding);

      var scrollViewer = new MenuScrollViewer();
      var itemsPresenter = new ItemsPresenter()
      {
         Name = ItemsPresenterPart,
      };
      CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, MenuItem.ItemsPanelProperty);
      Grid.SetIsSharedSizeScope(itemsPresenter, true);
      scrollViewer.Content = itemsPresenter;
      border.Child = scrollViewer;
      popup.Child = border;

      BindUtils.CreateTokenBinding(popup, Popup.MarginToAnchorProperty, MenuResourceKey.TopLevelItemPopupMarginToAnchor);
      BindUtils.CreateTokenBinding(popup, Popup.MaskShadowsProperty, MenuResourceKey.MenuPopupBoxShadows);
      
      CreateTemplateParentBinding(popup, Popup.IsOpenProperty, MenuItem.IsSubMenuOpenProperty, BindingMode.TwoWay);
      return popup;
   }

   protected override void BuildStyles()
   {
      BuildCommonStyle();
      BuildSizeTypeStyle();
      BuildDisabledStyle();
   }

   private void BuildCommonStyle()
   {
      var commonStyle = new Style(selector => selector.Nesting().PropertyEquals(MenuItem.IsEnabledProperty, true));
      commonStyle.Add(MenuItem.BackgroundProperty, GlobalResourceKey.ColorTransparent);
      var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      hoverStyle.Add(MenuItem.BackgroundProperty, MenuResourceKey.TopLevelItemHoverBg);
      hoverStyle.Add(MenuItem.ForegroundProperty, MenuResourceKey.TopLevelItemHoverColor);
      hoverStyle.Add(MenuItem.CursorProperty, new Cursor(StandardCursorType.Hand));
      commonStyle.Add(hoverStyle);
      Add(commonStyle);
   }
   
   private void BuildSizeTypeStyle()
   {
      var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(MenuItem.SizeTypeProperty, SizeType.Large));
      largeSizeStyle.Add(MenuItem.CornerRadiusProperty, MenuResourceKey.TopLevelItemBorderRadiusLG);
      largeSizeStyle.Add(MenuItem.MinHeightProperty, GlobalResourceKey.ControlHeightLG);
      largeSizeStyle.Add(MenuItem.PaddingProperty, MenuResourceKey.TopLevelItemPaddingLG);
      largeSizeStyle.Add(MenuItem.FontSizeProperty, MenuResourceKey.TopLevelItemFontSizeLG);
      {
         var presenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
        
         presenterStyle.Add(ContentPresenter.LineHeightProperty, MenuResourceKey.TopLevelItemLineHeightLG);
         largeSizeStyle.Add(presenterStyle);
      }
      Add(largeSizeStyle);
      
      var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(MenuItem.SizeTypeProperty, SizeType.Middle));
      middleSizeStyle.Add(MenuItem.CornerRadiusProperty, MenuResourceKey.TopLevelItemBorderRadius);
      middleSizeStyle.Add(MenuItem.MinHeightProperty, GlobalResourceKey.ControlHeight);
      middleSizeStyle.Add(MenuItem.PaddingProperty, MenuResourceKey.TopLevelItemPadding);
      middleSizeStyle.Add(MenuItem.FontSizeProperty, MenuResourceKey.TopLevelItemFontSize);
      {
         var presenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
         presenterStyle.Add(ContentPresenter.LineHeightProperty, MenuResourceKey.TopLevelItemLineHeight);
         middleSizeStyle.Add(presenterStyle);
      }
      Add(middleSizeStyle);

      var smallSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(MenuItem.SizeTypeProperty, SizeType.Small));
      smallSizeStyle.Add(MenuItem.CornerRadiusProperty, MenuResourceKey.TopLevelItemBorderRadiusSM);
      smallSizeStyle.Add(MenuItem.MinHeightProperty, GlobalResourceKey.ControlHeightSM);
      smallSizeStyle.Add(MenuItem.PaddingProperty, MenuResourceKey.TopLevelItemPaddingSM);
      smallSizeStyle.Add(MenuItem.FontSizeProperty, MenuResourceKey.TopLevelItemFontSizeSM);
      {
         var presenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
         presenterStyle.Add(ContentPresenter.LineHeightProperty, MenuResourceKey.TopLevelItemLineHeightSM);
         smallSizeStyle.Add(presenterStyle);
      }
      Add(smallSizeStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Add(MenuItem.ForegroundProperty, MenuResourceKey.ItemDisabledColor);
      Add(disabledStyle);
   }
}