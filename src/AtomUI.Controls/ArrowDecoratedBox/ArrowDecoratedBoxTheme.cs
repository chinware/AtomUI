using AtomUI.Controls.Utils;
using AtomUI.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
public class ArrowDecoratedBoxTheme : ControlTheme
{
   public const string DecoratorPart = "Part_Decorator";
   
   public ArrowDecoratedBoxTheme() : base(typeof(ArrowDecoratedBox)) {}

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<ArrowDecoratedBox>((box, scope) =>
      {
         var decorator = new Border()
         {
            Name = DecoratorPart
         };
         decorator.RegisterInNameScope(scope);
         
         if (box.Child?.Parent is not null) {
            UIStructureUtils.ClearLogicalParentRecursive(box.Child, null);
            UIStructureUtils.ClearVisualParentRecursive(box.Child, null);
         }

         CreateTemplateParentBinding(decorator, Border.BackgroundSizingProperty, ArrowDecoratedBox.BackgroundSizingProperty);
         CreateTemplateParentBinding(decorator, Border.BackgroundProperty, ArrowDecoratedBox.BackgroundProperty);
         CreateTemplateParentBinding(decorator, Border.CornerRadiusProperty, ArrowDecoratedBox.CornerRadiusProperty);
         CreateTemplateParentBinding(decorator, Border.ChildProperty, ArrowDecoratedBox.ChildProperty);
         CreateTemplateParentBinding(decorator, Border.PaddingProperty, ArrowDecoratedBox.PaddingProperty);
         
         return decorator;
      });
   }

   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(ArrowDecoratedBox.BackgroundProperty, GlobalResourceKey.ColorBgContainer);
      commonStyle.Add(ArrowDecoratedBox.MinHeightProperty, GlobalResourceKey.ControlHeight);
      commonStyle.Add(ArrowDecoratedBox.PaddingProperty, ArrowDecoratedBoxResourceKey.Padding);
      commonStyle.Add(ArrowDecoratedBox.ArrowSizeProperty, ArrowDecoratedBoxResourceKey.ArrowSize);
      commonStyle.Add(ArrowDecoratedBox.CornerRadiusProperty, GlobalResourceKey.BorderRadius);
      Add(commonStyle);
   }
}