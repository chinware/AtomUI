using AtomUI.Media;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class CardTabStripItemTheme : BaseTabStripItemTheme
{
   public const string ID = "CardTabStripItem";
   
   public CardTabStripItemTheme() : base(typeof(TabStripItem)) { }
   
   public override string ThemeResourceKey()
   {
      return ID;
   }

   protected override void NotifyBuildControlTemplate(TabStripItem stripItem, INameScope scope, Border container)
   {
      base.NotifyBuildControlTemplate(stripItem, scope, container);

      if (container.Transitions is null) {
         var transitions = new Transitions();
         transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty));
         container.Transitions = transitions;
      }
      CreateTemplateParentBinding(container, Border.BorderThicknessProperty, TabStripItem.BorderThicknessProperty);
      CreateTemplateParentBinding(container, Border.CornerRadiusProperty, TabStripItem.CornerRadiusProperty);
   }
   
   protected override void BuildStyles()
   {
      base.BuildStyles();
      
      var commonStyle = new Style(selector => selector.Nesting());

      {
         var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(DecoratorPart));
         decoratorStyle.Add(Border.MarginProperty, TabControlResourceKey.HorizontalItemMargin);
         decoratorStyle.Add(Border.BackgroundProperty, TabControlResourceKey.CardBg);
         decoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorBorderSecondary);
         commonStyle.Add(decoratorStyle);
      }
      
      // 选中
      var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
      {
         var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(DecoratorPart));
         decoratorStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorBgContainer);
         selectedStyle.Add(decoratorStyle);
      }
      commonStyle.Add(selectedStyle);
      
      Add(commonStyle);
      
      BuildSizeTypeStyle();
      BuildPlacementStyle();
   }

   protected void BuildSizeTypeStyle()
   {
      var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(TabStripItem.SizeTypeProperty, SizeType.Large));
      {
         var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(DecoratorPart));
         decoratorStyle.Add(Border.PaddingProperty, TabControlResourceKey.CardPaddingLG);
         largeSizeStyle.Add(decoratorStyle);
      }
      Add(largeSizeStyle);

      var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(TabStripItem.SizeTypeProperty, SizeType.Middle));
      {
         var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(DecoratorPart));
         decoratorStyle.Add(Border.PaddingProperty, TabControlResourceKey.CardPadding);
         middleSizeStyle.Add(decoratorStyle);
      }
      Add(middleSizeStyle);

      var smallSizeType = new Style(selector => selector.Nesting().PropertyEquals(TabStripItem.SizeTypeProperty, SizeType.Small));
      {
         var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(DecoratorPart));
         decoratorStyle.Add(Border.PaddingProperty, TabControlResourceKey.CardPaddingSM);
         smallSizeType.Add(decoratorStyle);
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
         
         var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(DecoratorPart));
         decoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorBorderSecondary);
         
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