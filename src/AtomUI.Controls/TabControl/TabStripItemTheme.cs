using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TabStripItemTheme : BaseTabStripItemTheme
{
   public const string ID = "TabStripItem";
   
   public TabStripItemTheme() : base(typeof(TabStripItem)) { }
   
   public override string ThemeResourceKey()
   {
      return ID;
   }

   protected override void BuildStyles()
   {
      base.BuildStyles();
      var decoratorStyle = new Style(selector => selector.Nesting().Template().OfType<Border>().Name(DecoratorPart));
      decoratorStyle.Add(Border.MarginProperty, TabControlResourceKey.HorizontalItemMargin);
      Add(decoratorStyle);
      BuildSizeTypeStyle();
   }

   protected void BuildSizeTypeStyle()
   {
      var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(TabStripItem.SizeTypeProperty, SizeType.Large));
      {
         var decoratorStyle = new Style(selector => selector.Nesting().Template().OfType<Border>().Name(DecoratorPart));
         decoratorStyle.Add(Border.PaddingProperty, TabControlResourceKey.HorizontalItemPaddingLG);
         largeSizeStyle.Add(decoratorStyle);
      }
      Add(largeSizeStyle);

      var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(TabStripItem.SizeTypeProperty, SizeType.Middle));
      {
         var decoratorStyle = new Style(selector => selector.Nesting().Template().OfType<Border>().Name(DecoratorPart));
         decoratorStyle.Add(Border.PaddingProperty, TabControlResourceKey.HorizontalItemPadding);
         middleSizeStyle.Add(decoratorStyle);
      }
      Add(middleSizeStyle);

      var smallSizeType = new Style(selector => selector.Nesting().PropertyEquals(TabStripItem.SizeTypeProperty, SizeType.Small));
      {
         var decoratorStyle = new Style(selector => selector.Nesting().Template().OfType<Border>().Name(DecoratorPart));
         decoratorStyle.Add(Border.PaddingProperty, TabControlResourceKey.HorizontalItemPaddingSM);
         smallSizeType.Add(decoratorStyle);
      }
      Add(smallSizeType);
   }
}