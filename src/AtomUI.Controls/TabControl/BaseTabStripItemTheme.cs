using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal class BaseTabStripItemTheme : BaseControlTheme
{
   public const string DecoratorPart = "Part_Decorator";
   public const string ContentLayoutPart = "Part_ContentLayout";
   public const string ContentPresenterPart = "PART_ContentPresenter";
   
   public BaseTabStripItemTheme(Type targetType) : base(targetType) { }
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<TabStripItem>((strip, scope) =>
      {
         // 做边框
         var decorator = new Border()
         {
            Name = DecoratorPart
         };
         decorator.RegisterInNameScope(scope);
         NotifyBuildControlTemplate(strip, scope, decorator);
         return decorator;
      });
   }

   protected virtual void NotifyBuildControlTemplate(TabStripItem stripItem, INameScope scope, Border container)
   {
      var containerLayout = new StackPanel()
      {
         Name = ContentLayoutPart,
         Orientation = Orientation.Horizontal
      };
      containerLayout.RegisterInNameScope(scope);
      
      var contentPresenter = new ContentPresenter()
      {
         Name = ContentPresenterPart
      };
      containerLayout.Children.Add(contentPresenter);
      CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, TabStripItem.ContentProperty);
      CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty, TabStripItem.ContentTemplateProperty);
      
      var iconButton = new IconButton();
      CreateTemplateParentBinding(iconButton, IconButton.IconProperty, TabStripItem.CloseIconProperty);
      containerLayout.Children.Add(iconButton);
      
      container.Child = containerLayout;
   }

   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(TabStripItem.CursorProperty, new Cursor(StandardCursorType.Hand));
      commonStyle.Add(TabStripItem.ForegroundProperty, TabControlResourceKey.ItemColor);
      
      // 增加文字 hover 和 选中的效果
      var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      hoverStyle.Add(TabStripItem.ForegroundProperty, TabControlResourceKey.ItemHoverColor);
      commonStyle.Add(hoverStyle);
      
      // 选中
      var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
      selectedStyle.Add(TabStripItem.ForegroundProperty, TabControlResourceKey.ItemSelectedColor);
      commonStyle.Add(selectedStyle);
      
      Add(commonStyle);
      BuildSizeTypeStyle();
      BuildDisabledStyle();
   }

   private void BuildSizeTypeStyle()
   {
      var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(TabStripItem.SizeTypeProperty, SizeType.Large));
      largeSizeStyle.Add(TabStripItem.FontSizeProperty, TabControlResourceKey.TitleFontSizeLG);
      Add(largeSizeStyle);

      var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(TabStripItem.SizeTypeProperty, SizeType.Middle));
      middleSizeStyle.Add(TabStripItem.FontSizeProperty, TabControlResourceKey.TitleFontSize);
      Add(middleSizeStyle);

      var smallSizeType = new Style(selector => selector.Nesting().PropertyEquals(TabStripItem.SizeTypeProperty, SizeType.Small));
      smallSizeType.Add(TabStripItem.FontSizeProperty, TabControlResourceKey.TitleFontSizeSM);
      Add(smallSizeType);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Add(TabStripItem.ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      Add(disabledStyle);
   }
}