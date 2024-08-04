using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal class BaseTabStripTheme : BaseControlTheme
{
   public const string FrameDecoratorPart = "PART_FrameDecorator";
   public const string ItemsPresenterPart = "PART_ItemsPresenter";
   
   public BaseTabStripTheme(Type targetType) : base(targetType) { }
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<BaseTabStrip>((strip, scope) =>
      {
         var frameDecorator = new Border()
         {
            Name = FrameDecoratorPart
         };
         frameDecorator.RegisterInNameScope(scope);
         NotifyBuildControlTemplate(strip, scope, frameDecorator);
         return frameDecorator;
      });
   }

   protected virtual void NotifyBuildControlTemplate(BaseTabStrip baseTabStrip, INameScope scope, Border container)
   {
   }

   protected override void BuildStyles()
   {
      base.BuildStyles();
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(BaseTabStrip.BorderBrushProperty, GlobalResourceKey.ColorBorderSecondary);
      
      // 设置 items presenter 是否居中
      // 分为上、右、下、左
      {
         // 上
         var topStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.TopPC));
         topStyle.Add(BaseTabStrip.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
         topStyle.Add(BaseTabStrip.VerticalAlignmentProperty, VerticalAlignment.Top);
         
         // tabs 是否居中
         var tabAlignCenterStyle = new Style(selector => selector.Nesting().PropertyEquals(TabStrip.TabAlignmentCenterProperty, true));
         {
            var itemsPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart));
            itemsPresenterStyle.Add(ItemsPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            tabAlignCenterStyle.Add(itemsPresenterStyle);
         }
         topStyle.Add(tabAlignCenterStyle);
         
         commonStyle.Add(topStyle);
      }

      {
         // 右
         var rightStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.RightPC));
         
         rightStyle.Add(BaseTabStrip.HorizontalAlignmentProperty, HorizontalAlignment.Left);
         rightStyle.Add(BaseTabStrip.VerticalAlignmentProperty, VerticalAlignment.Stretch);
         
         // tabs 是否居中
         var tabAlignCenterStyle = new Style(selector => selector.Nesting().PropertyEquals(TabStrip.TabAlignmentCenterProperty, true));
         {
            var itemsPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart));
            itemsPresenterStyle.Add(ItemsPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            tabAlignCenterStyle.Add(itemsPresenterStyle);
         }
         rightStyle.Add(tabAlignCenterStyle);
         
         commonStyle.Add(rightStyle);
      }
      {
         // 下
         var bottomStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.BottomPC));
         bottomStyle.Add(BaseTabStrip.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
         bottomStyle.Add(BaseTabStrip.VerticalAlignmentProperty, VerticalAlignment.Top);
         
         // tabs 是否居中
         var tabAlignCenterStyle = new Style(selector => selector.Nesting().PropertyEquals(TabStrip.TabAlignmentCenterProperty, true));
         {
            var itemsPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart));
            itemsPresenterStyle.Add(ItemsPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            tabAlignCenterStyle.Add(itemsPresenterStyle);
         }
         bottomStyle.Add(tabAlignCenterStyle);
         
         commonStyle.Add(bottomStyle);
      }
      {
         // 左
         var leftStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.LeftPC));
         
                  
         leftStyle.Add(BaseTabStrip.HorizontalAlignmentProperty, HorizontalAlignment.Left);
         leftStyle.Add(BaseTabStrip.VerticalAlignmentProperty, VerticalAlignment.Stretch);
         
         // tabs 是否居中
         var tabAlignCenterStyle = new Style(selector => selector.Nesting().PropertyEquals(TabStrip.TabAlignmentCenterProperty, true));
         {
            var itemsPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart));
            itemsPresenterStyle.Add(ItemsPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            tabAlignCenterStyle.Add(itemsPresenterStyle);
         }
         leftStyle.Add(tabAlignCenterStyle);
         
         commonStyle.Add(leftStyle);
      }
      
      Add(commonStyle);
   }
}