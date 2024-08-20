using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlThemeProvider]
public class MenuFlyoutPresenterTheme : BaseControlTheme
{
   public const string ItemsPresenterPart  = "PART_ItemsPresenter";
   public const string RootContainerPart   = "PART_RootContainer";
   
   public MenuFlyoutPresenterTheme() : base(typeof(MenuFlyoutPresenter)) { }
   
   
   protected override IControlTemplate? BuildControlTemplate()
   {
      return new FuncControlTemplate<MenuFlyoutPresenter>((theme, scope) =>
      {
         var wrapper = new Border()
         {
            Name = RootContainerPart,
            ClipToBounds = false,
            UseLayoutRounding = false
         };
         TokenResourceBinder.CreateTokenBinding(wrapper, Border.BackgroundProperty, MenuTokenResourceKey.MenuBgColor);
         TokenResourceBinder.CreateTokenBinding(wrapper, Border.MinWidthProperty, MenuTokenResourceKey.MenuPopupMinWidth);
         TokenResourceBinder.CreateTokenBinding(wrapper, Border.MaxWidthProperty, MenuTokenResourceKey.MenuPopupMaxWidth);
         TokenResourceBinder.CreateTokenBinding(wrapper, Border.MinHeightProperty, MenuTokenResourceKey.MenuPopupMinHeight);
         TokenResourceBinder.CreateTokenBinding(wrapper, Border.MaxHeightProperty, MenuTokenResourceKey.MenuPopupMaxHeight);
         TokenResourceBinder.CreateTokenBinding(wrapper, Border.PaddingProperty, MenuTokenResourceKey.MenuPopupContentPadding);
         TokenResourceBinder.CreateTokenBinding(wrapper, Border.CornerRadiusProperty, MenuTokenResourceKey.MenuPopupBorderRadius);
         
         var scrollViewer = new MenuScrollViewer();
         var itemsPresenter = new ItemsPresenter
         {
            Name = ItemsPresenterPart,
         };
         CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, MenuItem.ItemsPanelProperty);
         KeyboardNavigation.SetTabNavigation(itemsPresenter, KeyboardNavigationMode.Continue);
         Grid.SetIsSharedSizeScope(itemsPresenter, true);
         scrollViewer.Content = itemsPresenter;
         wrapper.Child = scrollViewer;
         return wrapper;
      });
   }
   
   protected override void BuildStyles()
   {
      this.Add(MenuFlyoutPresenter.BackgroundProperty, new SolidColorBrush(Colors.Transparent));
      this.Add(MenuFlyoutPresenter.CornerRadiusProperty, MenuTokenResourceKey.MenuPopupBorderRadius);
   }
}