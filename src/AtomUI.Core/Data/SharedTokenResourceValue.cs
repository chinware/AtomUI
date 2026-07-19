using System.Diagnostics;
using AtomUI.Theme.Resources;
using Avalonia;
using Avalonia.Markup.Xaml;

namespace AtomUI.Data;

public class SharedTokenResourceValue
{
    public SharedTokenKind? Kind { get; set; }
    
    public object? ProvideValue(IServiceProvider serviceProvider)
    {
        Debug.Assert(Kind != null);
        var provideTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        Debug.Assert(provideTarget != null);
        var targetObject = provideTarget.TargetObject as AvaloniaObject;
        Debug.Assert(targetObject != null);
        var targetProperty = provideTarget.TargetProperty as AvaloniaProperty;
        Debug.Assert(targetProperty != null);
        
        var application = Application.Current;
        Debug.Assert(application != null);
        var themeVariant = application.ActualThemeVariant;
        
        if (application.TryGetResource(Kind, themeVariant, out var value))
        {
            return value;
        }
        return null;
    }
}