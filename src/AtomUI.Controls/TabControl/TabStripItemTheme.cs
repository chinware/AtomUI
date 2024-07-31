using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Layout;
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
      
      // Icon 一些通用属性
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.MarginProperty, TabControlResourceKey.ItemIconMargin);
         Add(iconStyle);
      }
      
      var decoratorStyle = new Style(selector => selector.Nesting().Template().OfType<Border>().Name(DecoratorPart));
      decoratorStyle.Add(Border.MarginProperty, TabControlResourceKey.HorizontalItemMargin);
      Add(decoratorStyle);
      BuildSizeTypeStyle();
      BuildPlacementStyle();
   }

   protected void BuildSizeTypeStyle()
   {
      var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(TabStripItem.SizeTypeProperty, SizeType.Large));
      {
         var decoratorStyle = new Style(selector => selector.Nesting().Template().OfType<Border>().Name(DecoratorPart));
         decoratorStyle.Add(Border.PaddingProperty, TabControlResourceKey.HorizontalItemPaddingLG);
         largeSizeStyle.Add(decoratorStyle);
      }
      {
         // Icon
         var iconStyle = new Style(selector => selector.Nesting().Template().Name(ItemIconPart));
         iconStyle.Add(PathIcon.WidthProperty, GlobalResourceKey.IconSize);
         iconStyle.Add(PathIcon.HeightProperty, GlobalResourceKey.IconSize);
         largeSizeStyle.Add(iconStyle);
      }
      Add(largeSizeStyle);

      var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(TabStripItem.SizeTypeProperty, SizeType.Middle));
      {
         var decoratorStyle = new Style(selector => selector.Nesting().Template().OfType<Border>().Name(DecoratorPart));
         decoratorStyle.Add(Border.PaddingProperty, TabControlResourceKey.HorizontalItemPadding);
         middleSizeStyle.Add(decoratorStyle);
      }
      {
         // Icon
         var iconStyle = new Style(selector => selector.Nesting().Template().Name(ItemIconPart));
         iconStyle.Add(PathIcon.WidthProperty, GlobalResourceKey.IconSize);
         iconStyle.Add(PathIcon.HeightProperty, GlobalResourceKey.IconSize);
         middleSizeStyle.Add(iconStyle);
      }
      Add(middleSizeStyle);

      var smallSizeType = new Style(selector => selector.Nesting().PropertyEquals(TabStripItem.SizeTypeProperty, SizeType.Small));
      {
         var decoratorStyle = new Style(selector => selector.Nesting().Template().OfType<Border>().Name(DecoratorPart));
         decoratorStyle.Add(Border.PaddingProperty, TabControlResourceKey.HorizontalItemPaddingSM);
         smallSizeType.Add(decoratorStyle);
      }
      {
         // Icon
         var iconStyle = new Style(selector => selector.Nesting().Template().Name(ItemIconPart));
         iconStyle.Add(PathIcon.WidthProperty, GlobalResourceKey.IconSizeSM);
         iconStyle.Add(PathIcon.HeightProperty, GlobalResourceKey.IconSizeSM);
         smallSizeType.Add(iconStyle);
      }
      Add(smallSizeType);
   }

   private void BuildPlacementStyle()
   {
      // 设置 items presenter 面板样式
      // 分为上、右、下、左
      {
         // 上
         var topStyle = new Style(selector => selector.Nesting().PropertyEquals(TabStripItem.TabStripPlacementProperty, Dock.Top));
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.VerticalAlignmentProperty, VerticalAlignment.Center);
         topStyle.Add(iconStyle);
         Add(topStyle);
      }

      {
         // 右
         var rightStyle = new Style(selector => selector.Nesting().PropertyEquals(TabStripItem.TabStripPlacementProperty, Dock.Right));
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.HorizontalAlignmentProperty, HorizontalAlignment.Center);
         rightStyle.Add(iconStyle);
         Add(rightStyle);
      }
      {
         // 下
         var bottomStyle = new Style(selector => selector.Nesting().PropertyEquals(TabStripItem.TabStripPlacementProperty, Dock.Bottom));
         
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.VerticalAlignmentProperty, VerticalAlignment.Center);
         bottomStyle.Add(iconStyle);
         Add(bottomStyle);
      }
      {
         // 左
         var leftStyle = new Style(selector => selector.Nesting().PropertyEquals(TabStripItem.TabStripPlacementProperty, Dock.Left));
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.HorizontalAlignmentProperty, HorizontalAlignment.Center);
         leftStyle.Add(iconStyle);
         Add(leftStyle);
      }
   }
}