using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class MenuItemTheme : ControlTheme
{
   public const string ItemDecoratorPart     = "Part_ItemDecorator";
   public const string MainContainerPart     = "Part_MainContainer";
   public const string TogglePresenterPart   = "Part_TogglePresenter";
   public const string ItemIconPresenterPart = "Part_ItemIconPresenter";
   public const string ItemTextPresenterPart = "Part_ItemTextPresenter";
   public const string InputGestureTextPart  = "Part_InputGestureText";
   public const string MenuIndicatorIconPart = "Part_MenuIndicatorIcon";
   public const string PopupPart             = "PART_Popup";
   public const string ItemsPresenterPart    = "PART_ItemsPresenter";
   
   public MenuItemTheme()
      : base(typeof(MenuItem))
   {
   }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<MenuItem>((item, scope) =>
      {
         // 仅仅为了把 Popup 包进来，没有其他什么作用
         var layoutWrapper = new Panel();
         var container = new Border()
         {
            Name = ItemDecoratorPart
         };
         
         var transitions = new Transitions();
         transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty));
         container.Transitions = transitions;
         
         var layout = new Grid()
         {
            Name = MainContainerPart,
            ColumnDefinitions =
            {
               new ColumnDefinition(GridLength.Auto)
               {
                  SharedSizeGroup = "TogglePresenter"
               },
               new ColumnDefinition(GridLength.Auto)
               {
                  SharedSizeGroup = "IconPresenter"
               },
               new ColumnDefinition(GridLength.Star),
               new ColumnDefinition(GridLength.Auto)
               {
                  SharedSizeGroup = "InputGestureText"
               },
               new ColumnDefinition(GridLength.Auto)
               {
                  SharedSizeGroup = "MenuIndicatorIcon"
               }
            }
         };
         layout.RegisterInNameScope(scope);

         var togglePresenter = new ContentControl
         {
            Name = TogglePresenterPart,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            IsVisible = false
         };
         Grid.SetColumn(togglePresenter, 0);
         togglePresenter.RegisterInNameScope(scope);

         var iconPresenter = new Viewbox
         {
            Name = ItemIconPresenterPart,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Stretch = Stretch.Uniform
         };
         
         Grid.SetColumn(iconPresenter, 1);
         iconPresenter.RegisterInNameScope(scope);
         CreateTemplateParentBinding(iconPresenter, Viewbox.ChildProperty, MenuItem.IconProperty);
         BindUtils.CreateTokenBinding(iconPresenter, Viewbox.MarginProperty, MenuResourceKey.ItemMargin);
         BindUtils.CreateGlobalTokenBinding(iconPresenter, Viewbox.WidthProperty, MenuResourceKey.ItemIconSize);
         BindUtils.CreateGlobalTokenBinding(iconPresenter, Viewbox.HeightProperty, MenuResourceKey.ItemIconSize);

         var itemTextPresenter = new ContentPresenter
         {
            Name = ItemTextPresenterPart,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center,
            RecognizesAccessKey = true,
            IsHitTestVisible = false
         };
         Grid.SetColumn(itemTextPresenter, 2);
         BindUtils.CreateTokenBinding(itemTextPresenter, ContentPresenter.MarginProperty, MenuResourceKey.ItemMargin);
         CreateTemplateParentBinding(itemTextPresenter, ContentPresenter.ContentProperty, MenuItem.HeaderProperty);
         CreateTemplateParentBinding(itemTextPresenter, ContentPresenter.ContentTemplateProperty, MenuItem.HeaderTemplateProperty);

         itemTextPresenter.RegisterInNameScope(scope);

         var inputGestureText = new TextBlock
         {
            Name = InputGestureTextPart,
            HorizontalAlignment = HorizontalAlignment.Right,
            TextAlignment = TextAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
         };
         Grid.SetColumn(inputGestureText, 3);
         BindUtils.CreateTokenBinding(inputGestureText, ContentPresenter.MarginProperty, MenuResourceKey.ItemMargin);
         CreateTemplateParentBinding(inputGestureText, 
                                     TextBlock.TextProperty, 
                                     MenuItem.InputGestureProperty,
                                     BindingMode.Default,
                                     MenuItem.KeyGestureConverter);
         
         inputGestureText.RegisterInNameScope(scope);

         var menuIndicatorIcon = new PathIcon
         {
            Name = MenuIndicatorIconPart,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Kind = "RightOutlined"
         };
         BindUtils.CreateGlobalTokenBinding(menuIndicatorIcon, PathIcon.WidthProperty, GlobalResourceKey.IconSizeXS);
         BindUtils.CreateGlobalTokenBinding(menuIndicatorIcon, PathIcon.HeightProperty, GlobalResourceKey.IconSizeXS);
         Grid.SetColumn(menuIndicatorIcon, 4);
         menuIndicatorIcon.RegisterInNameScope(scope);

         var popup = CreateMenuPopup();
         popup.RegisterInNameScope(scope);

         layout.Children.Add(togglePresenter);
         layout.Children.Add(iconPresenter);
         layout.Children.Add(itemTextPresenter);
         layout.Children.Add(inputGestureText);
         layout.Children.Add(menuIndicatorIcon);
         layout.Children.Add(popup);
         
         container.Child = layout;
         layoutWrapper.Children.Add(container);
         return layoutWrapper;
      });
   }

   private Popup CreateMenuPopup()
   {
      var popup = new Popup()
      {
         Name = PopupPart,
         WindowManagerAddShadowHint = false,
         IsLightDismissEnabled = false,
         Placement = PlacementMode.RightEdgeAlignedTop,
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
      var itemsPresenter = new ItemsPresenter
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
      var commonStyle = new Style(selector => selector.Nesting());
      BuildCommonStyle(commonStyle);
      BuildMenuIndicatorStyle();
      BuildMenuIconStyle();
      Add(commonStyle);
   }

   private void BuildCommonStyle(Style commonStyle)
   {
      commonStyle.Add(MenuItem.ForegroundProperty, MenuResourceKey.ItemColor);
      {
         var keyGestureStyle = new Style(selector => selector.Nesting().Template().Name(InputGestureTextPart));
         keyGestureStyle.Add(TextBlock.ForegroundProperty, MenuResourceKey.KeyGestureColor);
         commonStyle.Add(keyGestureStyle);
      }
      {
         var borderStyle = new Style(selector => selector.Nesting().Template().Name(ItemDecoratorPart));
         borderStyle.Add(Border.MinHeightProperty, MenuResourceKey.ItemHeight);
         borderStyle.Add(Border.PaddingProperty, MenuResourceKey.ItemPaddingInline);
         borderStyle.Add(Border.BackgroundProperty, MenuResourceKey.ItemBg);
         borderStyle.Add(Border.CornerRadiusProperty, MenuResourceKey.ItemBorderRadius);
         commonStyle.Add(borderStyle);
      }
      
      // Hover 状态
      var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      hoverStyle.Add(MenuItem.ForegroundProperty, MenuResourceKey.ItemHoverColor);
      {
         var borderStyle = new Style(selector => selector.Nesting().Template().Name(ItemDecoratorPart));
         borderStyle.Add(Border.BackgroundProperty, MenuResourceKey.ItemHoverBg);
         hoverStyle.Add(borderStyle);
      }
      commonStyle.Add(hoverStyle);
   }

   private void BuildMenuIndicatorStyle()
   {
      {
         var menuIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(MenuIndicatorIconPart));
         menuIndicatorStyle.Add(PathIcon.IsVisibleProperty, true);
         Add(menuIndicatorStyle);
      }
      var hasSubMenuStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Empty));
      {
         var menuIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(MenuIndicatorIconPart));
         menuIndicatorStyle.Add(PathIcon.IsVisibleProperty, false);
         hasSubMenuStyle.Add(menuIndicatorStyle);
      }
      Add(hasSubMenuStyle);
   }

   private void BuildMenuIconStyle()
   {
      {
         var iconViewBoxStyle = new Style(selector => selector.Nesting().Template().Name(ItemIconPresenterPart));
         iconViewBoxStyle.Add(Viewbox.IsVisibleProperty, false);
         Add(iconViewBoxStyle);
      }

      var hasIconStyle = new Style(selector => selector.Nesting().Class(":icon"));
      {
         var iconViewBoxStyle = new Style(selector => selector.Nesting().Template().Name(ItemIconPresenterPart));
         iconViewBoxStyle.Add(Viewbox.IsVisibleProperty, true);
         hasIconStyle.Add(iconViewBoxStyle);
      }
      Add(hasIconStyle);
   }
}