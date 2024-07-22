using AtomUI.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
public class MenuTheme : ControlTheme
{
   public const string ItemsPresenterPart = "PART_ItemsPresenter";
   public MenuTheme()
      : base(typeof(Menu))
   {
   }

   protected override IControlTemplate? BuildControlTemplate()
   {
      return new FuncControlTemplate<Menu>((menu, scope) =>
      {
         var itemPresenter = new ItemsPresenter()
         {
            Name = ItemsPresenterPart,
            VerticalAlignment = VerticalAlignment.Stretch,
         };

         KeyboardNavigation.SetTabNavigation(itemPresenter, KeyboardNavigationMode.Continue);
         
         var border = new Border()
         {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Child = itemPresenter,
         };
         CreateTemplateParentBinding(border, Border.PaddingProperty, Menu.PaddingProperty);
         CreateTemplateParentBinding(border, Border.BackgroundProperty, Menu.BackgroundProperty);
         CreateTemplateParentBinding(border, Border.BackgroundSizingProperty, Menu.BackgroundSizingProperty);
         CreateTemplateParentBinding(border, Border.BorderThicknessProperty, Menu.BorderThicknessProperty);
         CreateTemplateParentBinding(border, Border.BorderBrushProperty, Menu.BorderBrushProperty);
         CreateTemplateParentBinding(border, Border.CornerRadiusProperty, Menu.CornerRadiusProperty);
         return border;
      });
   }

   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(Menu.BackgroundProperty, GlobalResourceKey.ColorBgContainer);
      commonStyle.Add(Menu.PaddingProperty, new Thickness(0));
      commonStyle.Add(Menu.BorderBrushProperty, GlobalResourceKey.ColorBorder);
      var largeSizeType = new Style(selector => selector.Nesting().PropertyEquals(Menu.SizeTypeProperty, SizeType.Large));
      largeSizeType.Add(Menu.MinHeightProperty, GlobalResourceKey.ControlHeightLG);
      largeSizeType.Add(Menu.CornerRadiusProperty, GlobalResourceKey.BorderRadius);
      commonStyle.Add(largeSizeType);
      
      var middleSizeType = new Style(selector => selector.Nesting().PropertyEquals(Menu.SizeTypeProperty, SizeType.Middle));
      middleSizeType.Add(Menu.MinHeightProperty, GlobalResourceKey.ControlHeight);
      middleSizeType.Add(Menu.CornerRadiusProperty, GlobalResourceKey.BorderRadius);
      commonStyle.Add(middleSizeType);
      
      var smallSizeType = new Style(selector => selector.Nesting().PropertyEquals(Menu.SizeTypeProperty, SizeType.Small));
      smallSizeType.Add(Menu.MinHeightProperty, GlobalResourceKey.ControlHeightSM);
      smallSizeType.Add(Menu.CornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
      commonStyle.Add(smallSizeType);
      Add(commonStyle);
   }
}