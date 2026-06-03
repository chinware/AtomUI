using System.Diagnostics;
using AtomUI.Data;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace AtomUI.Theme.Language;

public abstract class LanguageResourceExtension<TResourceKind> : MarkupExtension
    where TResourceKind : Enum
{
    public TResourceKind? Kind { get; set; }
    
    public LanguageResourceExtension()
    {
    }

    public LanguageResourceExtension(TResourceKind kind)
    {
        Kind = kind;
    }
    
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        Debug.Assert(Kind != null);
        var provideTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        var dynamicResource = new DynamicResourceExtension(Kind);
        var targetObject    = provideTarget?.TargetObject;
        if (targetObject is not StyledElement)
        {
            var application = Application.Current;
            if (application is null)
            {
                throw new ApplicationException("The application instance does not exist");
            }
            dynamicResource.SetAnchor(application);
            if (ShouldUseStaticResourceValue(targetObject))
            {
                return LanguageResourceBinder.GetLangResource(Kind) ?? Kind.ToString()!;
            }
        }
        return dynamicResource;
    }

    private static bool ShouldUseStaticResourceValue(object? targetObject)
    {
        return targetObject is not null &&
               targetObject is not AvaloniaObject &&
               targetObject is not SetterBase;
    }
}
