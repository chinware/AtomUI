using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls;

internal class BaseTabStripTheme : ControlTheme
{
   public const string MainContainerPart = "Part_MainContainer";
   public const string ItemsPresenterPart = "PART_ItemsPresenter";
   
   public BaseTabStripTheme(Type targetType) : base(targetType) { }
   
   protected override IControlTemplate? BuildControlTemplate()
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
}