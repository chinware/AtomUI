using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace AtomUI;

using AvaloniaControlTheme = Avalonia.Styling.ControlTheme;

public class ControlTheme : AvaloniaControlTheme
{
   public ControlTheme() {}
   public ControlTheme(Type targetType) : base(targetType) {}
   
   public void Build()
   {
      NotifyPreBuild();
      BuildStyles();
      var template = BuildControlTemplate();
      if (template is not null) {
         Add(new Setter(TemplatedControl.TemplateProperty, template));
      }
      NotifyBuildCompleted();
   }

   public virtual string? ThemeResourceKey()
   {
      return default;
   }

   protected virtual IControlTemplate? BuildControlTemplate() { return default; }
   protected virtual void BuildStyles() {}
   protected virtual void NotifyPreBuild() {}
   protected virtual void NotifyBuildCompleted() {}
}