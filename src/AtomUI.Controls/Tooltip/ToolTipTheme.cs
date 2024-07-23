using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

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
            Name = ToolTipContainerPart
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
         CreateTemplateParentBinding(arrowDecoratedBox, ArrowDecoratedBox.IsShowArrowProperty, ToolTip.IsShowArrowEffectiveProperty);
         arrowDecoratedBox.RegisterInNameScope(scope);
         return arrowDecoratedBox;
      });
   }

   protected override void BuildStyles()
   {
      this.Add(ToolTip.ShadowsProperty, ToolTipResourceKey.ToolTipShadows);
      this.Add(ToolTip.DefaultMarginToAnchorProperty, ToolTipResourceKey.MarginToAnchor);
      this.Add(ToolTip.MotionDurationProperty, ToolTipResourceKey.ToolTipMotionDuration);
      this.Add(ToolTip.BackgroundProperty, GlobalResourceKey.ColorTransparent);

      var arrowDecoratedBoxStyle = new Style(selector => selector.Nesting().Template().OfType<ArrowDecoratedBox>());
      arrowDecoratedBoxStyle.Add(ArrowDecoratedBox.FontSizeProperty, GlobalResourceKey.FontSize);
      arrowDecoratedBoxStyle.Add(ArrowDecoratedBox.MaxWidthProperty, ToolTipResourceKey.ToolTipMaxWidth);
      arrowDecoratedBoxStyle.Add(ArrowDecoratedBox.BackgroundProperty, ToolTipResourceKey.ToolTipBackground);
      arrowDecoratedBoxStyle.Add(ArrowDecoratedBox.ForegroundProperty, ToolTipResourceKey.ToolTipColor);
      arrowDecoratedBoxStyle.Add(ArrowDecoratedBox.MinHeightProperty, GlobalResourceKey.ControlHeight);
      arrowDecoratedBoxStyle.Add(ArrowDecoratedBox.PaddingProperty, ToolTipResourceKey.ToolTipPadding);
      arrowDecoratedBoxStyle.Add(ArrowDecoratedBox.CornerRadiusProperty, ToolTipResourceKey.BorderRadiusOuter);
      Add(arrowDecoratedBoxStyle);
   }
}