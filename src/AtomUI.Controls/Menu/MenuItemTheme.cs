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
   
   public MenuItemTheme()
      : base(typeof(MenuItem))
   {
   }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<MenuItem>((item, scope) =>
      {
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
            IsVisible = false,
            Stretch = Stretch.Uniform
         };
         
         Grid.SetColumn(iconPresenter, 1);
         iconPresenter.RegisterInNameScope(scope);
         
         BindUtils.CreateGlobalTokenBinding(iconPresenter, Viewbox.WidthProperty, GlobalResourceKey.IconSize);
         BindUtils.CreateGlobalTokenBinding(iconPresenter, Viewbox.HeightProperty, GlobalResourceKey.IconSize);

         var itemTextPresenter = new ContentPresenter
         {
            Name = ItemTextPresenterPart,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center,
            RecognizesAccessKey = true
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

         layout.Children.Add(togglePresenter);
         layout.Children.Add(iconPresenter);
         layout.Children.Add(itemTextPresenter);
         layout.Children.Add(inputGestureText);
         layout.Children.Add(menuIndicatorIcon);
         
         container.Child = layout;
         
         return container;
      });
   }

   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      BuildCommonStyle(commonStyle);
      BuildMenuIndicatorStyle();
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
}