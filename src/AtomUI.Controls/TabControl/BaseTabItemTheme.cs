using AtomUI.Theme;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls;

internal class BaseTabItemTheme : BaseControlTheme
{
   public const string DecoratorPart = "Part_Decorator";
   public const string ContentLayoutPart = "Part_ContentLayout";
   public const string ContentPresenterPart = "PART_ContentPresenter";
   public const string ItemIconPart = "PART_ItemIcon";
   public const string ItemCloseButtonPart = "PART_ItemCloseButton";
   
   public BaseTabItemTheme(Type targetType) : base(targetType) { }
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<TabItem>((tabItem, scope) =>
      {
         // 做边框
         var decorator = new Border()
         {
            Name = DecoratorPart
         };
         decorator.RegisterInNameScope(scope);
         NotifyBuildControlTemplate(tabItem, scope, decorator);
         return decorator;
      });
   }

   protected virtual void NotifyBuildControlTemplate(TabItem stripItem, INameScope scope, Border container)
   {
      
   }
}