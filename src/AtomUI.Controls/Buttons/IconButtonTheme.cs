using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class IconButtonTheme : BaseControlTheme
{
   public const string IconContentPart = "PART_IconContent";
   public IconButtonTheme()
      : base(typeof(IconButton))
   {
   }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<IconButton>((button, scope) =>
      {
         BuildInstanceStyles(button);
         var iconContent = new ContentPresenter()
         {
            Name = IconContentPart,
            Transitions = new Transitions()
            {
               AnimationUtils.CreateTransition<SolidColorBrushTransition>(ContentPresenter.BackgroundProperty)
            },
         };
         CreateTemplateParentBinding(iconContent, ContentPresenter.CornerRadiusProperty, IconButton.CornerRadiusProperty);
         CreateTemplateParentBinding(iconContent, ContentPresenter.ContentProperty, IconButton.IconProperty);
         CreateTemplateParentBinding(iconContent, ContentPresenter.PaddingProperty, IconButton.PaddingProperty);
         return iconContent;
      });
   }

   protected override void BuildStyles()
   {
      {
         var contentStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart));
         contentStyle.Add(ContentPresenter.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
         Add(contentStyle);
      }
      var enableHoverBgStyle = new Style(selector => selector.Nesting().PropertyEquals(IconButton.IsEnableHoverEffectProperty, true).Class(StdPseudoClass.PointerOver));
      {
         var contentStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart));
         contentStyle.Add(ContentPresenter.BackgroundProperty, GlobalTokenResourceKey.ColorBgTextHover);
         Add(contentStyle);
         enableHoverBgStyle.Add(contentStyle);
      }
      Add(enableHoverBgStyle);
   }

   protected override void BuildInstanceStyles(Control control)
   {
      var iconStyle = new Style(selector => selector.Name(IconContentPart).Child().OfType<PathIcon>());
      iconStyle.Add(PathIcon.HorizontalAlignmentProperty, HorizontalAlignment.Center);
      iconStyle.Add(PathIcon.VerticalAlignmentProperty, VerticalAlignment.Center);
      control.Styles.Add(iconStyle);
   }
}