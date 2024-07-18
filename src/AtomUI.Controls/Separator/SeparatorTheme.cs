using AtomUI.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class SeparatorTheme : ControlTheme
{
   public const string TitlePart = "PART_CloseBtn";
   
   public SeparatorTheme()
      : base(typeof(Separator))
   {
   }

   protected override IControlTemplate? BuildControlTemplate()
   {
      return new FuncControlTemplate<Separator>((separator, scope) =>
      {
         var titleLabel = new Label
         {
            Name = TitlePart,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            Padding = new Thickness(0)
         };
         CreateTemplateParentBinding(titleLabel, Label.ContentProperty, Separator.TitleProperty);
         CreateTemplateParentBinding(titleLabel, Label.FontSizeProperty, Separator.FontSizeProperty);
         CreateTemplateParentBinding(titleLabel, Label.ForegroundProperty, Separator.TitleColorProperty);
         CreateTemplateParentBinding(titleLabel, Label.FontStyleProperty, Separator.FontStyleProperty);
         CreateTemplateParentBinding(titleLabel, Label.FontWeightProperty, Separator.FontWeightProperty);
         titleLabel.RegisterInNameScope(scope);
         return titleLabel;
      });
   }

   protected override void BuildStyles()
   {
      // 默认的一些样式
      Add(new Setter(Separator.TitleColorProperty, new DynamicResourceExtension(GlobalResourceKey.ColorText)));
      Add(new Setter(Separator.FontSizeProperty, new DynamicResourceExtension(GlobalResourceKey.FontSize)));
      Add(new Setter(Separator.LineColorProperty, new DynamicResourceExtension(GlobalResourceKey.ColorSplit)));

      var titleSelector = default(Selector).Nesting().Template().OfType<Label>().Name(TitlePart);
      var horizontalStyle = new Style(selector => selector.Nesting().PropertyEquals(Separator.OrientationProperty, Orientation.Horizontal));
      horizontalStyle.Add(new Setter(Separator.HorizontalAlignmentProperty, HorizontalAlignment.Stretch));
      horizontalStyle.Add(new Setter(Separator.VerticalAlignmentProperty, VerticalAlignment.Center));
      {
         var titleStyle = new Style(selector => titleSelector);
         titleStyle.Add(new Setter(Label.IsVisibleProperty, true));
         horizontalStyle.Add(titleStyle);
      }
      Add(horizontalStyle);
      
      var verticalStyle = new Style(selector => selector.Nesting().PropertyEquals(Separator.OrientationProperty, Orientation.Vertical));
      verticalStyle.Add(new Setter(Separator.HorizontalAlignmentProperty, HorizontalAlignment.Center));
      verticalStyle.Add(new Setter(Separator.VerticalAlignmentProperty, VerticalAlignment.Center));
      {
         var titleStyle = new Style(selector => titleSelector);
         titleStyle.Add(new Setter(Label.IsVisibleProperty, false));
         verticalStyle.Add(titleStyle);
      }
      Add(verticalStyle);
   }
}