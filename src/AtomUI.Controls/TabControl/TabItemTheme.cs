using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TabItemTheme : BaseTabItemTheme
{
   public const string ID = "TabItem";
   
   public TabItemTheme() : base(typeof(TabItem)) { }
   
   public override string ThemeResourceKey()
   {
      return ID;
   }
   
   protected override void BuildStyles()
   {
      base.BuildStyles();
      BuildSizeTypeStyle();
   }

   protected void BuildSizeTypeStyle()
   {
      var topOrBottomStyle = new Style(selector => Selectors.Or(selector.Nesting().PropertyEquals(TabItem.TabStripPlacementProperty, Dock.Top),
                                                                selector.Nesting().PropertyEquals(TabItem.TabStripPlacementProperty, Dock.Bottom)));

      {
         topOrBottomStyle.Add(Border.MarginProperty, TabControlResourceKey.HorizontalItemMargin);
         
         var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(TabItem.SizeTypeProperty, SizeType.Large));
         {
            var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(DecoratorPart));
            decoratorStyle.Add(Border.PaddingProperty, TabControlResourceKey.HorizontalItemPaddingLG);
            largeSizeStyle.Add(decoratorStyle);
         }
 
         topOrBottomStyle.Add(largeSizeStyle);

         var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(TabItem.SizeTypeProperty, SizeType.Middle));
         {
            var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(DecoratorPart));
            decoratorStyle.Add(Border.PaddingProperty, TabControlResourceKey.HorizontalItemPadding);
            middleSizeStyle.Add(decoratorStyle);
         }

         topOrBottomStyle.Add(middleSizeStyle);

         var smallSizeType = new Style(selector => selector.Nesting().PropertyEquals(TabItem.SizeTypeProperty, SizeType.Small));
         {
            var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(DecoratorPart));
            decoratorStyle.Add(Border.PaddingProperty, TabControlResourceKey.HorizontalItemPaddingSM);
            smallSizeType.Add(decoratorStyle);
         }
     
         topOrBottomStyle.Add(smallSizeType);
         
         Add(topOrBottomStyle);
      }
      
      var leftOrRightStyle = new Style(selector => Selectors.Or(selector.Nesting().PropertyEquals(TabItem.TabStripPlacementProperty, Dock.Left),
                                                                selector.Nesting().PropertyEquals(TabItem.TabStripPlacementProperty, Dock.Right)));
      {
         // 貌似没必要分大小，但是先放着吧，万一需要难得再加
         var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(TabItem.SizeTypeProperty, SizeType.Large));
         {
            var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(DecoratorPart));
            decoratorStyle.Add(Border.PaddingProperty, TabControlResourceKey.VerticalItemPadding);
            largeSizeStyle.Add(decoratorStyle);
         }
 
         leftOrRightStyle.Add(largeSizeStyle);

         var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(TabItem.SizeTypeProperty, SizeType.Middle));
         {
            var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(DecoratorPart));
            decoratorStyle.Add(Border.PaddingProperty, TabControlResourceKey.VerticalItemPadding);
            middleSizeStyle.Add(decoratorStyle);
         }

         leftOrRightStyle.Add(middleSizeStyle);

         var smallSizeType = new Style(selector => selector.Nesting().PropertyEquals(TabItem.SizeTypeProperty, SizeType.Small));
         {
            var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(DecoratorPart));
            decoratorStyle.Add(Border.PaddingProperty, TabControlResourceKey.VerticalItemPadding);
            smallSizeType.Add(decoratorStyle);
         }
     
         leftOrRightStyle.Add(smallSizeType);
         
         Add(leftOrRightStyle);
      }
   }
}