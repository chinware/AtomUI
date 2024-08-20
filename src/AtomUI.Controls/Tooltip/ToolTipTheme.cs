using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ToolTipTheme : BaseControlTheme
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
      this.Add(ToolTip.ShadowsProperty, ToolTipTokenResourceKey.ToolTipShadows);
      this.Add(ToolTip.DefaultMarginToAnchorProperty, ToolTipTokenResourceKey.MarginToAnchor);
      this.Add(ToolTip.MotionDurationProperty, ToolTipTokenResourceKey.ToolTipMotionDuration);
      this.Add(ToolTip.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);

      var arrowDecoratedBoxStyle = new Style(selector => selector.Nesting().Template().OfType<ArrowDecoratedBox>());
      arrowDecoratedBoxStyle.Add(ArrowDecoratedBox.FontSizeProperty, GlobalTokenResourceKey.FontSize);
      arrowDecoratedBoxStyle.Add(ArrowDecoratedBox.MaxWidthProperty, ToolTipTokenResourceKey.ToolTipMaxWidth);
      arrowDecoratedBoxStyle.Add(ArrowDecoratedBox.BackgroundProperty, ToolTipTokenResourceKey.ToolTipBackground);
      arrowDecoratedBoxStyle.Add(ArrowDecoratedBox.ForegroundProperty, ToolTipTokenResourceKey.ToolTipColor);
      arrowDecoratedBoxStyle.Add(ArrowDecoratedBox.MinHeightProperty, GlobalTokenResourceKey.ControlHeight);
      arrowDecoratedBoxStyle.Add(ArrowDecoratedBox.PaddingProperty, ToolTipTokenResourceKey.ToolTipPadding);
      arrowDecoratedBoxStyle.Add(ArrowDecoratedBox.CornerRadiusProperty, ToolTipTokenResourceKey.BorderRadiusOuter);
      Add(arrowDecoratedBoxStyle);
   }
}