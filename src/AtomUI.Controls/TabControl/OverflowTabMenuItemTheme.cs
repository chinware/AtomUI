using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class OverflowTabMenuItemTheme : BaseControlTheme
{
   public const string ItemDecoratorPart     = "PART_ItemDecorator";
   public const string MainContainerPart     = "PART_MainContainer";
   public const string ItemTextPresenterPart = "PART_ItemTextPresenter";
   public const string ItemCloseButtonPart = "PART_ItemCloseIcon";
   
   public OverflowTabMenuItemTheme()
      : base(typeof(OverflowTabMenuItem))
   {
   }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<OverflowTabMenuItem>((item, scope) =>
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
               new ColumnDefinition(GridLength.Star),
               new ColumnDefinition(GridLength.Auto)
               {
                  SharedSizeGroup = "MenuCloseIcon"
               }
            }
         };
         
         var itemTextPresenter = new ContentPresenter
         {
            Name = ItemTextPresenterPart,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center,
            RecognizesAccessKey = true,
            IsHitTestVisible = false
         };
         
         Grid.SetColumn(itemTextPresenter, 0);
         TokenResourceBinder.CreateTokenBinding(itemTextPresenter, ContentPresenter.MarginProperty, MenuResourceKey.ItemMargin);
         CreateTemplateParentBinding(itemTextPresenter, ContentPresenter.ContentProperty, OverflowTabMenuItem.HeaderProperty);
         CreateTemplateParentBinding(itemTextPresenter, ContentPresenter.ContentTemplateProperty, OverflowTabMenuItem.HeaderTemplateProperty);

         itemTextPresenter.RegisterInNameScope(scope);
         
         var menuCloseIcon = new PathIcon
         {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Kind = "CloseOutlined"
         };

         var closeButton = new IconButton()
         {
            Name = ItemCloseButtonPart,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Icon = menuCloseIcon
         };


         CreateTemplateParentBinding(closeButton, IconButton.IsVisibleProperty, OverflowTabMenuItem.IsClosableProperty);
         TokenResourceBinder.CreateGlobalTokenBinding(menuCloseIcon, PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorIcon);
         TokenResourceBinder.CreateGlobalTokenBinding(menuCloseIcon, PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorIconHover);
         
         TokenResourceBinder.CreateGlobalTokenBinding(menuCloseIcon, PathIcon.WidthProperty, GlobalResourceKey.IconSizeSM);
         TokenResourceBinder.CreateGlobalTokenBinding(menuCloseIcon, PathIcon.HeightProperty, GlobalResourceKey.IconSizeSM);
         
         Grid.SetColumn(menuCloseIcon, 4);
         closeButton.RegisterInNameScope(scope);
         
         layout.Children.Add(itemTextPresenter);
         layout.Children.Add(closeButton);
         container.Child = layout;
         
         return container;
      });
   }
   
   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      BuildCommonStyle(commonStyle);
      BuildDisabledStyle();
      Add(commonStyle);
   }

   private void BuildCommonStyle(Style commonStyle)
   {
      commonStyle.Add(OverflowTabMenuItem.ForegroundProperty, MenuResourceKey.ItemColor);
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
      hoverStyle.Add(OverflowTabMenuItem.ForegroundProperty, MenuResourceKey.ItemHoverColor);
      {
         var borderStyle = new Style(selector => selector.Nesting().Template().Name(ItemDecoratorPart));
         borderStyle.Add(Border.BackgroundProperty, MenuResourceKey.ItemHoverBg);
         hoverStyle.Add(borderStyle);
      }
      commonStyle.Add(hoverStyle);
   }
   
   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Add(OverflowTabMenuItem.ForegroundProperty, MenuResourceKey.ItemDisabledColor);
      Add(disabledStyle);
   }
   
}