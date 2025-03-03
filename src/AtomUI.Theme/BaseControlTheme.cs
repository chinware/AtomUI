using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.LogicalTree;
using Avalonia.Styling;

namespace AtomUI.Theme;

public abstract class BaseControlTheme : ControlTheme
{
    public BaseControlTheme()
    {
    }

    public BaseControlTheme(Type targetType) : base(targetType)
    {
    }

    public void Build()
    {
        NotifyPreBuild();
        BuildThemeAnimations();
        BuildStyles();
        BuildTemplateStyle();
        NotifyBuildCompleted();
    }

    protected virtual void BuildTemplateStyle()
    {
        var template = BuildControlTemplate();
        if (template is not null)
        {
            Add(new Setter(TemplatedControl.TemplateProperty, template));
        }
    }

    public virtual string? ThemeResourceKey()
    {
        return default;
    }

    protected virtual IControlTemplate? BuildControlTemplate()
    {
        return default;
    }

    protected virtual void BuildThemeAnimations()
    {
    }

    protected virtual void BuildStyles()
    {
    }

    protected virtual void BuildInstanceStyles(Control control)
    {
    }

    protected virtual void NotifyPreBuild()
    {
    }

    protected virtual void NotifyBuildCompleted()
    {
    }

    protected static void RegisterTokenResourceBindings(Control hostControl, Action resourceBindingAction)
    {
        if (((ILogical)hostControl).IsAttachedToLogicalTree)
        {
            resourceBindingAction();
        }
        hostControl.AttachedToLogicalTree += (sender, args) =>
        {
            resourceBindingAction();
        };
    }

    protected static IDisposable CreateTemplateParentBinding<T>(
        AvaloniaObject target,
        StyledProperty<T> property,
        AvaloniaProperty parentProperty,
        BindingMode mode = BindingMode.Default,
        IValueConverter? converter = null)
    {
        return target.Bind(property, new TemplateBinding(parentProperty)
        {
            Mode      = mode,
            Converter = converter
        }, BindingPriority.Template);
    }

    protected static IDisposable CreateTemplateParentBinding<T>(
        AvaloniaObject target,
        DirectPropertyBase<T> property,
        AvaloniaProperty parentProperty,
        BindingMode mode = BindingMode.Default,
        IValueConverter? converter = null)
    {
        return target.Bind(property, new TemplateBinding(parentProperty)
        {
            Mode      = mode,
            Converter = converter
        }, BindingPriority.Template);
    }

    protected static IDisposable CreateTemplateParentBinding(
        AvaloniaObject target,
        AvaloniaProperty property,
        AvaloniaProperty parentProperty,
        BindingMode mode = BindingMode.Default,
        IValueConverter? converter = null)
    {
        return target.Bind(property, new TemplateBinding(parentProperty)
        {
            Mode      = mode,
            Converter = converter
        }, BindingPriority.Template);
    }

    protected static IDisposable CreateTemplateParentBinding(AvaloniaObject target, AvaloniaProperty property,
                                                             string templateParentPath,
                                                             BindingPriority priority,
                                                             BindingMode mode = BindingMode.Default,
                                                             IValueConverter? converter = null)
    {
        return target.Bind(property, new Binding(templateParentPath)
        {
            RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
            Mode           = mode,
            Converter      = converter,
            Priority       = priority
        });
    }

    protected static IDisposable CreateTemplateParentBinding<T>(AvaloniaObject target, StyledProperty<T> property,
                                                                string templateParentPath,
                                                                BindingPriority priority,
                                                                BindingMode mode = BindingMode.Default,
                                                                IValueConverter? converter = null)
    {
        return target.Bind(property, new Binding(templateParentPath)
        {
            RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
            Mode           = mode,
            Converter      = converter,
            Priority       = priority
        });
    }

    protected static IDisposable CreateTemplateParentBinding<T>(AvaloniaObject target, DirectPropertyBase<T> property,
                                                                string templateParentPath,
                                                                BindingPriority priority,
                                                                BindingMode mode = BindingMode.Default,
                                                                IValueConverter? converter = null)
    {
        return target.Bind(property, new Binding(templateParentPath)
        {
            RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
            Mode           = mode,
            Converter      = converter,
            Priority       = priority
        });
    }
    
    protected static IDisposable CreateTemplateParentBinding(AvaloniaObject target, AvaloniaProperty property,
                                                             AvaloniaProperty parentProperty,
                                                             BindingPriority priority,
                                                             BindingMode mode = BindingMode.Default,
                                                             IValueConverter? converter = null)
    {
        return CreateTemplateParentBinding(target, property, parentProperty.Name, priority, mode, converter);
    }
    
    protected static IDisposable CreateTemplateParentBinding<T>(AvaloniaObject target, StyledProperty<T> property,
                                                                AvaloniaProperty parentProperty,
                                                                BindingPriority priority,
                                                                BindingMode mode = BindingMode.Default,
                                                                IValueConverter? converter = null)
    {
        return CreateTemplateParentBinding(target, property, parentProperty.Name, priority, mode, converter);
    }
    
    protected static IDisposable CreateTemplateParentBinding<T>(AvaloniaObject target, DirectPropertyBase<T> property,
                                                                AvaloniaProperty parentProperty,
                                                                BindingPriority priority,
                                                                BindingMode mode = BindingMode.Default,
                                                                IValueConverter? converter = null)
    {
        return CreateTemplateParentBinding(target, property, parentProperty.Name, priority, mode, converter);
    }
}