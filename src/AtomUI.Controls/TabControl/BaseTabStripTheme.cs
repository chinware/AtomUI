using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal class BaseTabStripTheme : BaseControlTheme
{
   public const string MainContainerPart = "Part_MainContainer";
   public const string ItemsPresenterPart = "PART_ItemsPresenter";
   
   public BaseTabStripTheme(Type targetType) : base(targetType) { }
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<BaseTabStrip>((strip, scope) =>
      {
         var mainContainer = new Border()
         {
            Name = MainContainerPart
         };
         mainContainer.RegisterInNameScope(scope);
         NotifyBuildControlTemplate(strip, scope, mainContainer);
         return mainContainer;
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
      Add(commonStyle);
   }
}