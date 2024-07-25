using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class MenuItemTheme : ControlTheme
{
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
         var container =  new Border();
         var layout = new StackPanel
         {
            Name = MainContainerPart,
            Orientation = Orientation.Horizontal,
         };
         layout.RegisterInNameScope(scope);

         var togglePresenter = new ContentControl
         {
            Name = TogglePresenterPart,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            IsVisible = false
         };
         togglePresenter.RegisterInNameScope(scope);

         var iconPresenter = new Viewbox
         {
            Name = ItemIconPresenterPart,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            IsVisible = false,
            Stretch = Stretch.Uniform
         };
         
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
         
         CreateTemplateParentBinding(itemTextPresenter, ContentPresenter.ContentProperty, MenuItem.HeaderProperty);
         CreateTemplateParentBinding(itemTextPresenter, ContentPresenter.ContentTemplateProperty, MenuItem.HeaderTemplateProperty);

         itemTextPresenter.RegisterInNameScope(scope);

         var inputGestureText = new TextBlock
         {
            Name = InputGestureTextPart,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
         };
         inputGestureText.RegisterInNameScope(scope);

         var menuIndicatorIcon = new PathIcon
         {
            Name = MenuIndicatorIconPart,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
         };
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
}