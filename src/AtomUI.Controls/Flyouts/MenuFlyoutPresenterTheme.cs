using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlThemeProvider]
public class MenuFlyoutPresenterTheme : ControlTheme
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
         TokenResourceBinder.CreateTokenBinding(wrapper, Border.BackgroundProperty, MenuResourceKey.MenuBgColor);
         TokenResourceBinder.CreateTokenBinding(wrapper, Border.MinWidthProperty, MenuResourceKey.MenuPopupMinWidth);
         TokenResourceBinder.CreateTokenBinding(wrapper, Border.MaxWidthProperty, MenuResourceKey.MenuPopupMaxWidth);
         TokenResourceBinder.CreateTokenBinding(wrapper, Border.MinHeightProperty, MenuResourceKey.MenuPopupMinHeight);
         TokenResourceBinder.CreateTokenBinding(wrapper, Border.MaxHeightProperty, MenuResourceKey.MenuPopupMaxHeight);
         TokenResourceBinder.CreateTokenBinding(wrapper, Border.PaddingProperty, MenuResourceKey.MenuPopupContentPadding);
         TokenResourceBinder.CreateTokenBinding(wrapper, Border.CornerRadiusProperty, MenuResourceKey.MenuPopupBorderRadius);
         
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
      this.Add(MenuFlyoutPresenter.CornerRadiusProperty, MenuResourceKey.MenuPopupBorderRadius);
   }
}