using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlThemeProvider]
public class MenuFlyoutPresenterTheme : BaseControlTheme
{
   public const string ItemsPresenterPart  = "PART_ItemsPresenter";
   public const string RootContainerPart   = "PART_RootContainer";
   
   public MenuFlyoutPresenterTheme() : base(typeof(MenuFlyoutPresenter)) { }
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<MenuFlyoutPresenter>((menuFlyoutPresenter, scope) =>
      {
         var arrowDecorator = new ArrowDecoratedBox()
         {
            Name = RootContainerPart,
            ClipToBounds = false,
            UseLayoutRounding = false
         };
         CreateTemplateParentBinding(arrowDecorator, ArrowDecoratedBox.IsShowArrowProperty, MenuFlyoutPresenter.IsShowArrowProperty);
         CreateTemplateParentBinding(arrowDecorator, ArrowDecoratedBox.ArrowPositionProperty, MenuFlyoutPresenter.ArrowPositionProperty);
         
         TokenResourceBinder.CreateTokenBinding(arrowDecorator, ArrowDecoratedBox.BackgroundProperty, MenuTokenResourceKey.MenuBgColor);
         TokenResourceBinder.CreateTokenBinding(arrowDecorator, ArrowDecoratedBox.MinWidthProperty, MenuTokenResourceKey.MenuPopupMinWidth);
         TokenResourceBinder.CreateTokenBinding(arrowDecorator, ArrowDecoratedBox.MaxWidthProperty, MenuTokenResourceKey.MenuPopupMaxWidth);
         TokenResourceBinder.CreateTokenBinding(arrowDecorator, ArrowDecoratedBox.MinHeightProperty, MenuTokenResourceKey.MenuPopupMinHeight);
         TokenResourceBinder.CreateTokenBinding(arrowDecorator, ArrowDecoratedBox.MaxHeightProperty, MenuTokenResourceKey.MenuPopupMaxHeight);
         TokenResourceBinder.CreateTokenBinding(arrowDecorator, ArrowDecoratedBox.PaddingProperty, MenuTokenResourceKey.MenuPopupContentPadding);
         
         var scrollViewer = new MenuScrollViewer()
         {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
         };
         var itemsPresenter = new ItemsPresenter
         {
            Name = ItemsPresenterPart,
         };
         
         CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, MenuItem.ItemsPanelProperty);
         KeyboardNavigation.SetTabNavigation(itemsPresenter, KeyboardNavigationMode.Continue);
         Grid.SetIsSharedSizeScope(itemsPresenter, true);
         scrollViewer.Content = itemsPresenter;
         
         arrowDecorator.Content = scrollViewer;
         
         return arrowDecorator;
      });
   }
   
   protected override void BuildStyles()
   {
      this.Add(Avalonia.Controls.MenuFlyoutPresenter.BackgroundProperty, new SolidColorBrush(Colors.Transparent));
      this.Add(Avalonia.Controls.MenuFlyoutPresenter.CornerRadiusProperty, MenuTokenResourceKey.MenuPopupBorderRadius);
   }
}