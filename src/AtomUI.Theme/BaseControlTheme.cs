using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Styling;

namespace AtomUI.Theme;

public abstract class BaseControlTheme : ControlTheme
{
   public BaseControlTheme() { }
   public BaseControlTheme(Type targetType) : base(targetType) { }

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

   protected virtual IControlTemplate? BuildControlTemplate()
   {
      return default;
   }

   protected virtual void BuildStyles() { }
   protected virtual void NotifyPreBuild() { }
   protected virtual void NotifyBuildCompleted() { }

   protected static IDisposable CreateTemplateParentBinding(AvaloniaObject target, AvaloniaProperty property,
                                                            string templateParentPath,
                                                            BindingMode mode = BindingMode.Default,
                                                            IValueConverter? converter = null)
   {
      return target.Bind(property, new Binding(templateParentPath)
      {
         RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
         Mode = mode,
         Converter = converter
      });
   }

   protected static IDisposable CreateTemplateParentBinding<T>(AvaloniaObject target, StyledProperty<T> property,
                                                               string templateParentPath,
                                                               BindingMode mode = BindingMode.Default,
                                                               IValueConverter? converter = null)
   {
      return target.Bind(property, new Binding(templateParentPath)
      {
         RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
         Mode = mode,
         Converter = converter
      });
   }

   protected static IDisposable CreateTemplateParentBinding<T>(AvaloniaObject target, DirectPropertyBase<T> property,
                                                               string templateParentPath,
                                                               BindingMode mode = BindingMode.Default,
                                                               IValueConverter? converter = null)
   {
      return target.Bind(property, new Binding(templateParentPath)
      {
         RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
         Mode = mode,
         Converter = converter
      });
   }

   protected static IDisposable CreateTemplateParentBinding(AvaloniaObject target, AvaloniaProperty property,
                                                            AvaloniaProperty templateParentProperty,
                                                            BindingMode mode = BindingMode.Default,
                                                            IValueConverter? converter = null)
   {
      return CreateTemplateParentBinding(target, property, templateParentProperty.Name, mode, converter);
   }
}