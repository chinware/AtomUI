using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ToolTipTheme : ControlTheme
{
   public const string ToolTipContainerPart = "PART_ToolTipContainer";
   
   public ToolTipTheme()
      : base(typeof(ToolTip))
   {
   }
   
   protected override IControlTemplate? BuildControlTemplate()
   {
      return new FuncControlTemplate<ToolTip>((tip, scope) =>
      {
         var arrowDecoratedBox = new ArrowDecoratedBox()
         {
            Name = ToolTipContainerPart,
         };
         if (tip.Content is string text) {
            arrowDecoratedBox.Child = new TextBlock
            {
               Text = text,
               VerticalAlignment = VerticalAlignment.Center,
               HorizontalAlignment = HorizontalAlignment.Center,
               TextWrapping = TextWrapping.Wrap,
            };
         } else if (tip.Content is Control control) {
            arrowDecoratedBox.Child = control;
         }
         
         BindUtils.CreateTokenBinding(arrowDecoratedBox, ArrowDecoratedBox.FontSizeProperty, GlobalResourceKey.FontSize);
         BindUtils.CreateTokenBinding(arrowDecoratedBox, ArrowDecoratedBox.MaxWidthProperty, ToolTipResourceKey.ToolTipMaxWidth);
         BindUtils.CreateTokenBinding(arrowDecoratedBox, ArrowDecoratedBox.BackgroundProperty, ToolTipResourceKey.ToolTipBackground);
         BindUtils.CreateTokenBinding(arrowDecoratedBox, ArrowDecoratedBox.ForegroundProperty, ToolTipResourceKey.ToolTipColor);
         BindUtils.CreateTokenBinding(arrowDecoratedBox, ArrowDecoratedBox.MinHeightProperty, GlobalResourceKey.ControlHeight);
         BindUtils.CreateTokenBinding(arrowDecoratedBox, ArrowDecoratedBox.PaddingProperty, ToolTipResourceKey.ToolTipPadding);
         BindUtils.CreateTokenBinding(arrowDecoratedBox, ArrowDecoratedBox.CornerRadiusProperty, ToolTipResourceKey.BorderRadiusOuter);
         
         CreateTemplateParentBinding(arrowDecoratedBox, ArrowDecoratedBox.IsShowArrowProperty, ToolTip.IsShowArrowEffectiveProperty);
         arrowDecoratedBox.RegisterInNameScope(scope);
         return arrowDecoratedBox;
      });
   }
}