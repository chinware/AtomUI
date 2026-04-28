using System.Diagnostics;
using AtomUI.Data;
using Avalonia;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;

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
        if (provideTarget?.TargetObject is not StyledElement)
        {
            var application = Application.Current;
            if (application is null)
            {
                throw new ApplicationException("The application instance does not exist");
            }
            dynamicResource.SetAnchor(application);
        }
        return dynamicResource;
    }
}