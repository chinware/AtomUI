using AtomUI.Icon;
using AtomUI.Theme;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Styling;
using AtomUI.Theme.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ToggleIconButtonTheme : BaseControlTheme
{
   public const string ContentPresenterPart = "PART_ContentPresenter";
   
   public ToggleIconButtonTheme()
      : base(typeof(ToggleIconButton))
   {}
   
   public ToggleIconButtonTheme(Type targetType) : base(targetType) { }
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<ToggleIconButton>((button, scope) =>
      {
         var contentPresenter = new ContentPresenter
         {
            Name = ContentPresenterPart,
         };
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, ToggleIconButton.ContentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty, ToggleIconButton.ContentTemplateProperty);
         return contentPresenter;
      });
   }
   
   protected override void BuildStyles()
   {
      var enabledStyle = new Style(selector => selector.Nesting().PropertyEquals(ToggleIconButton.IsEnabledProperty, true));
      {
         {
            var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
            iconStyle.Add(PathIcon.IconModeProperty, IconMode.Normal);
            enabledStyle.Add(iconStyle);
         }
         
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
         {
            var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
            iconStyle.Add(PathIcon.IconModeProperty, IconMode.Active);
            hoverStyle.Add(iconStyle);
         }
         enabledStyle.Add(hoverStyle);
         
         var checkedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
         {
            var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
            iconStyle.Add(PathIcon.IconModeProperty, IconMode.Selected);
            checkedStyle.Add(iconStyle);
         }
         enabledStyle.Add(checkedStyle);
      }
      Add(enabledStyle);
      
      var disabledStyle = new Style(selector => selector.Nesting().PropertyEquals(ToggleIconButton.IsEnabledProperty, false));
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.IconModeProperty, IconMode.Disabled);
         disabledStyle.Add(iconStyle);
      }
      Add(disabledStyle);
   }
}